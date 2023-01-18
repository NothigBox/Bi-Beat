using System.Collections;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.UI;

public enum UIState
{
    home,
    onePlayer,
    twoPlayers,
    lose,
    win,
    pause
}

public class UIManager : MonoBehaviour
{
    [SerializeField] private float homeColorChangeSpeedMultiplier;
    [SerializeField] private bool animateHomeColor;
    [SerializeField] private GameObject home;
    [SerializeField] private GameObject lose;
    [SerializeField] private GameObject win;
    [SerializeField] private GameObject controlsPlayer1;
    [SerializeField] private GameObject controlsPlayer2;
    [SerializeField] private GameObject pause;

    [SerializeField] private Image[] controlsPlayer1Images;
    [SerializeField] private Image[] controlsPlayer2Images;

    private UIState state;
    private Image homePanel;
    private Color homePanelColor;

    private float homePanelTimer;

    Vector3[] player1ImagesInitialScales;
    Vector3[] player2ImagesInitialScales;

    public UIState State => state;
    
    private void Awake()
    {
        homePanelTimer = 0f;

        homePanel = home.GetComponent<Image>();
        homePanelColor = homePanel.color;

        int controlsPlayer1ImagesLength = controlsPlayer1Images.Length;
        int controlsPlayer2ImagesLength = controlsPlayer2Images.Length;
        
        player1ImagesInitialScales = new Vector3[controlsPlayer1ImagesLength];
        player2ImagesInitialScales = new Vector3[controlsPlayer2ImagesLength];
        
        for (int i = 0; i < controlsPlayer1ImagesLength; i++)
        {
            player1ImagesInitialScales[i] = controlsPlayer1Images[i].transform.localScale;
        }

        /*
        for (int i = 0; i < controlsPlayer2ImagesLength; i++)
        {
            player2ImagesInitialScales[i] = controlsPlayer2Images[i].transform.localScale;
        }
        */

        SetState(UIState.home);
    }

    private void Update()
    {
        if(!animateHomeColor) return;
        
        float delta = Mathf.Abs(Mathf.Sin(homePanelTimer * homeColorChangeSpeedMultiplier));
        Color deltaColor = homePanelColor * Mathf.Clamp(delta, 0.3f, 1f);
        
        homePanel.color = new Color(deltaColor.r, deltaColor.g, deltaColor.b, homePanelColor.a);
        
        homePanelTimer += Time.deltaTime;

        
        if (homePanelTimer > 1000f)
        {
            homePanelTimer = 0f;
        }
    }

    public void SetState(UIState state)
    {
        this.state = state;
        int rotation = PlayerPrefs.GetInt("Rotation");

        ClearScreen();
        
        switch (state)
        {
            case UIState.home:
                home.SetActive(true);
                break;
            
            case UIState.lose:
                lose.SetActive(true);
                break;
            
            case UIState.win:
                win.SetActive(true);
                break;
            
            
            case UIState.onePlayer:
                if (rotation == 1)
                {
                    for (int i = 0; i < controlsPlayer1Images.Length; i++)
                    {
                        controlsPlayer1Images[i].transform.localScale = player1ImagesInitialScales[i];
                    }
                }
                else if (rotation == 0)
                {
                    for (int i = 0; i < controlsPlayer1Images.Length; i++)
                    {
                        Vector3 scale = player1ImagesInitialScales[i];
                        controlsPlayer1Images[i].transform.localScale = new Vector3(scale.x , -scale.y, scale.z);
                    }
                }
                
                controlsPlayer1.SetActive(true);
                break;
            
            case UIState.twoPlayers:
                if (rotation == 1)
                {
                    for (int i = 0; i < controlsPlayer1Images.Length; i++)
                    {
                        controlsPlayer1Images[i].transform.localScale = player1ImagesInitialScales[i];
                    }
                    
                    for (int i = 0; i < controlsPlayer2Images.Length; i++)
                    {
                        controlsPlayer2Images[i].transform.localScale = player2ImagesInitialScales[i];
                    }
                }
                else if (rotation == 0)
                {
                    for (int i = 0; i < controlsPlayer1Images.Length; i++)
                    {
                        Vector3 scale = player1ImagesInitialScales[i];
                        controlsPlayer1Images[i].transform.localScale = new Vector3(scale.x , -scale.y, scale.z);
                    }
                    
                    for (int i = 0; i < controlsPlayer2Images.Length; i++)
                    {
                        Vector3 scale = player2ImagesInitialScales[i];
                        controlsPlayer2Images[i].transform.localScale = new Vector3(scale.x , -scale.y, scale.z);
                    }
                }
                
                controlsPlayer1.SetActive(true);
                controlsPlayer2.SetActive(true);
                break;
            
            case UIState.pause:
                pause.SetActive(true);
                break;
            
            default:
                break;
        }
    }

    public void ClearScreen()
    {
        home.SetActive(false);
        lose.SetActive(false);
        win.SetActive(false);
        controlsPlayer1.SetActive(false);
//        controlsPlayer2.SetActive(false);
        pause.SetActive(false);
    }
}
  