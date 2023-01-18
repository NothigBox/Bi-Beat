using System;
using UnityEngine;
using UnityEngine.Serialization;

public class Note : MonoBehaviour
{
    [SerializeField] private float lifeTime;
    [SerializeField] private AnimationCurve speedOverLifeTime;
    [SerializeField] private float dispertion;
    [SerializeField] private TrailRenderer trail;

    private Transform target;
    private CircleCollider2D collider;
    private float initialOffset;
    private float speed;
    private float lifeTimer;
    private float originalTrailTime;

    public static bool canMove;

    public Action<Note> OnDeath;

    public int ID { get; set; }
    public bool DoSound { get; private set; }
    
    private Vector3 TargetPos => target.position + Vector3.right * initialOffset;

    private void Awake()
    {
        canMove = true;
        DoSound = true;
        lifeTimer = 0;
        originalTrailTime = trail.time;

        collider = GetComponent<CircleCollider2D>();
        
        target = GameObject.FindWithTag("Player").transform;
    }

    private void OnEnable()
    {
        OnDeath = null;

        lifeTimer = 0;
        DoSound = true;
        collider.enabled = true;
        initialOffset = transform.localPosition.x * dispertion;
    }

    private void OnDisable()
    {
        trail.Clear();
        //OnDeath?.Invoke(this);
    }

    private void FixedUpdate()
    {
        if(!canMove) return;
        
        Move();
    }

    public void AutoSetSpeed()
    {
        speed = ((TargetPos - transform.position).sqrMagnitude - collider.radius) / lifeTime;
    }
    
    void Move()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            TargetPos,
            speed * speedOverLifeTime.Evaluate(lifeTimer / lifeTime) * Time.fixedDeltaTime);

        lifeTimer += Time.fixedDeltaTime;
    }

    public static void Stop()
    {
        canMove = false;
    }
    
    public void Kill(bool doSound)
    {
        DoSound = doSound;
        OnDeath?.Invoke(this);
        gameObject.SetActive(false);
    }
}
