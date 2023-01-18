using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

enum ControllerType
{
    touch,
    buttons,
    keyboard
}

public class GameManager : MonoBehaviour
{
    private const string TUTORIAL_SONG_NAME_1 = "little star";
    private const string TUTORIAL_SONG_NAME_2 = "chicks say";
    
    private const float LOSE_WAIT_TIME = 2f;
    private const float WIN_WAIT_TIME = 1.5f;
    
    private const float GONG_SLOW_SPEED = 0.5f;
    private const float GONG_FAST_SPEED = 1.9f;
    private const float GONG_FAST_SPEED_DURATION = 5f;
    private const float GONG_SLOW_SPEED_DURATION = 1.25f;
    
    [SerializeField] private MusicManager music;
    [SerializeField] private LevelManager levels;
    [SerializeField] private TutorialController tutorial;
    [SerializeField] private UIManager ui;
    [SerializeField] private DiscManager disc;
    [SerializeField] private AnimateTransformPosition[] arcs;
    [SerializeField] private Progression progression;
    [SerializeField] private Transform gongShardsParent;

    [SerializeField] private ControllerType controllerType;
    [SerializeField] private AnimateColor[] background;
    
    [SerializeField] private WaveGong winningParticle;
    [SerializeField] private WaveGong tutorialParticle;
    [SerializeField] private ParticleSystem speedParticle;

    private UIState lastUIState;
    private float horizontal;
    private float totalPoints;
    private Vector2 screenSize;
    private int totalNotesCount;
    private int tutorialLevelIndex;
    private bool tutorialSpeedShown;
    private string currentSongName;

    private bool HasWon => progression.Counter / (float) totalNotesCount >= 1f;

    private void Awake()
    {
        totalPoints = 0;
        progression.Counter = 0;
        tutorialLevelIndex = 0;
        tutorialSpeedShown = false;

        screenSize = new Vector2(Screen.width, Screen.height);
        
        music.OnDarkNoteSpawned = arcs[1].Play;
        music.OnLightNoteSpawned = arcs[0].Play;

        lastUIState = UIState.home;

        levels.OnLevelChosen = PlayScore;
        
        DiscReceptors.OnReception = null;
        
        DiscReceptors.OnReception += Progress;
        Limit.OnLimitReached = Lose;
        WaveGong.OnGongEnd = AutoSetDiscAfterGong;
        Progression.OnGongReached = ActivateGong;

        if (controllerType == ControllerType.buttons)
        {
            foreach (var button in FindObjectsOfType<ControllerButtons>(true))
            {
                button.OnButtonPressed = SetHorizontal;
            }
        }

        tutorial.gameObject.SetActive(true);
        tutorial.ClearScreen();

        speedParticle.Stop();
        
        ArcsSetActive(false);
    }

    private void Start()
    {
        if (PlayerPrefs.GetString("Retry") != "")
        {
            PlayScore(PlayerPrefs.GetString("Retry"));
        }
        else
        {
            levels.SetUpLevels(music.Reader.Scores);
        }
    }

    private void Update()
    {
        switch (controllerType)
        {
            case ControllerType.keyboard:
                horizontal = Input.GetAxisRaw("Horizontal");
                break;
            
            case ControllerType.touch:
                if(ui.State == UIState.onePlayer)
                {
                    if (Input.GetMouseButtonUp(0)) horizontal = 0;
                    if (Input.GetMouseButton(0))
                    {
                        var mousePosX = Mathf.Clamp(Input.mousePosition.x, 0, screenSize.x);
                        var positionPercentage = mousePosX / screenSize.x;
                        horizontal = positionPercentage < 0.5 ? -1f : 1f;
                    }
                }
                else if (ui.State == UIState.twoPlayers)
                {
                    if(Input.touchCount == 2)
                    {
                        var touches = Input.touches;

                        int x = touches[0].position.x / screenSize.x >= 0.5f? -1 : 1;
                        int y = touches[0].position.y / screenSize.y >= 0.5f? -1 : 1;
                
                        int _x = touches[1].position.x / screenSize.x >= 0.5f? -1 : 1;
                        int _y = touches[1].position.y / screenSize.y >= 0.5f? -1 : 1;

                        bool xPositive = x >= 0;
                        bool yPositive = y >= 0;
                
                        bool _xPositive = _x >= 0;
                        bool _yPositive = _y >= 0;

                        bool canMove = xPositive != _xPositive && yPositive != _yPositive;

                        if (canMove)
                        {
                            horizontal = (x == y && _x == _y) ? -1 : 1;
                        }
                        else
                        {
                            horizontal = 0;
                        }
                        //Debug.Log($"{canMove}");
                        //Debug.Log($"{new Vector2(x, y)} | {new Vector2(_x, _y)}");
                
                        disc.SetMovement(horizontal, canMove);
                    }
                    return;
                }
                break;
        }

        disc.SetMovement(horizontal);
    }

    public void SetHardPause(bool paused)
    {
        SetSoftPause(paused);
        ui.SetState(paused? UIState.pause : UIState.onePlayer);
        SetTimeScale(paused ? 0f : 1f);
    }
    
    public void SetSoftPause(bool paused)
    {
        music.SetIsPlaying(!paused);
        ArcsSetActive(!paused);
    }

    public void GoHome(bool isRetry = false)
    {
        SetTimeScale(1f);
        if(!isRetry) PlayerPrefs.SetString("Retry", "");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Retry()
    {
        PlayerPrefs.SetString("Retry", currentSongName);
        
        GoHome(true);
    }

    void PlayGame()
    {
        ArcsSetActive(true);
        music.PlayDelay();
        
        if (tutorialLevelIndex == 1)
        {
            SetTimeScale(0.8f);
            
            SetSoftPause(true);

            Action onStatePassed = () =>
            {
                tutorial.SetState(TutorialState.notes, false);
                SetSoftPause(false);
            };
            
            tutorial.SetState(TutorialState.buttons, onStatePassed: onStatePassed, canPassStateDelay: 2f);
        }
        else if (tutorialLevelIndex == 2)
        {
            SetTimeScale(0.9f);

            SetSoftPause(true);

            Action onStatePassed = () =>
            {
                SetSoftPause(false);
            };
            
            tutorial.SetState(TutorialState.shards, onStatePassed: onStatePassed, canPassStateDelay: 2f);
        }
    }

    public void PlayOnePlayer()
    {
        lastUIState = UIState.onePlayer;
        ui.SetState(UIState.onePlayer);
        PlayGame();
    }
    
    public void PlayTwoPlayers()
    {
        lastUIState = UIState.twoPlayers;
        ui.SetState(UIState.twoPlayers);
        PlayGame();
    }

    public void UpdateInvertRotation(InvertRotation invertRotation)
    {
        disc.SetInvertRotation(invertRotation.IsOn);
    }

    private void Lose(Vector3 killNotePos)
    {
        PlayerPrefs.SetInt("Celebrate", 0);
        
        music.SpawnKillWave(killNotePos);
        music.SetIsPlaying(false);
        
        Invoke(nameof(SetUILoseState), LOSE_WAIT_TIME);
    }

    private void SetUILoseState()
    {
        tutorial.gameObject.SetActive(false);
        ui.SetState(UIState.lose);
        ArcsSetActive(false);
    }
    
    private void Win()
    {
        PlayerPrefs.SetInt("Celebrate", 1);
        
        levels.OnLevelPassed(currentSongName);

        winningParticle.gameObject.SetActive(true);
        winningParticle.Play();

        disc.SetDiscState(DiscState.celebrate);

        music.Celebrate();
        progression.SetColor(disc.CelebrateStateColor);
        
        UseGong();
        
        Invoke(nameof(SetUIWinState), WIN_WAIT_TIME);
    }
    
    private void SetUIWinState()
    {
        ui.SetState(UIState.win);
        ArcsSetActive(false);
    }

    void ArcsSetActive(bool setActive)
    {
        foreach (var arc in arcs)
        {
            arc.gameObject.SetActive(setActive);
        }
    }
    
    void Progress()
    {
        progression.Counter++;

        progression.SetPercentage(progression.Counter / (float) totalNotesCount);

        if ((tutorialLevelIndex == 1 && !tutorial.ProgressionShown) && progression.Counter >= 7)
        {
            SetSoftPause(true);
            progression.PlayParticles(true);
            progression.SetColor(disc.CelebrateStateColor);

            Action onStatePassed = () =>
            {
                progression.SetColor(disc.NormalStateColor);
                progression.StopParticles();
                SetSoftPause(false);
            };
            
            tutorial.SetState(TutorialState.progression, onStatePassed: onStatePassed, canPassStateDelay: 2f);
        }

        if (HasWon)
        {
            Win();
        }
    }

    void ActivateGong()
    {
        if (tutorialLevelIndex == 2 && !tutorial.GongShown)
        {
            SetSoftPause(true);
            tutorial.SetState(TutorialState.gong, false);
        }
        
        disc.SetDiscState(DiscState.gong);
        progression.SetColor(disc.GongStateColor);
    }

    public void UseGong()
    {
        if(!disc.PlayGong()) return;

        if (tutorialLevelIndex == 2 && !tutorial.SpeedShown)
        {
            SetSoftPause(true);

            tutorialParticle.gameObject.SetActive(true);
            tutorialParticle.Play();

            Action onStatePassed = () =>
            {
                SetSoftPause(false);

                tutorialSpeedShown = true;

                tutorialParticle.gameObject.SetActive(false);

                foreach (var anim in background)
                {
                    anim.SetAnimCurveValue(0f);
                }
            };

            tutorial.SetState(TutorialState.speed, onStatePassed: onStatePassed, canPassStateDelay: 4f);
        }

        foreach (var animation in background)
        {
            animation.Play();
        }

        StartCoroutine(GongSpeedCoroutine());
    }

    void AutoSetDiscAfterGong()
    {
        disc.StopGong();
        disc.SetDiscState(HasWon? DiscState.celebrate : DiscState.normal);
        progression.SetColor(HasWon? disc.CelebrateStateColor : disc.NormalStateColor);
    }

    public void SetHorizontal(float horizontal)
    {
        this.horizontal = horizontal;
    }

    void PlayScore(string songName)
    {
        currentSongName = songName;

        if (currentSongName.ToLower() == TUTORIAL_SONG_NAME_1) tutorialLevelIndex = 1;
        if (currentSongName.ToLower() == TUTORIAL_SONG_NAME_2) tutorialLevelIndex = 2;
        
        music.SetScore(songName);
        ActivateGongShards(songName);
        print(songName);

        totalNotesCount = music.GetTotalNotesCount();
        PlayOnePlayer();
    }

    void SetTimeScale(float timeScale)
    {
        Time.timeScale = timeScale;
    }

    IEnumerator GongSpeedCoroutine()
    {
        SetTimeScale(GONG_SLOW_SPEED);

        yield return new WaitForSecondsRealtime(GONG_SLOW_SPEED_DURATION);

        SetTimeScale(GONG_FAST_SPEED);

        speedParticle.Play();

        if (tutorialLevelIndex == 2 && !tutorialSpeedShown)
        {
            foreach (var anim in background) 
            {
                anim.Stop();
                anim.SetAnimCurveValue(0.5f);
            }
            
            SetTimeScale(GONG_FAST_SPEED * 0.9f);

            yield return new WaitUntil(() => tutorialSpeedShown);
        }

        yield return new WaitForSecondsRealtime(GONG_FAST_SPEED_DURATION);

        if (!HasWon)
        {
            speedParticle.Stop();
            SetTimeScale(1f);
        }
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetString("Retry", "");
        PlayerPrefs.SetInt("Celebrate", 0);
    }

    private void ActivateGongShards(string songName)
    {
        for(int i = 1; i < gongShardsParent.childCount - 1; i++)
        {
            gongShardsParent.GetChild(i).gameObject.SetActive(false);
        }
        
        gongShardsParent.Find(songName.ToLower())?.gameObject.SetActive(true);
    }
}
