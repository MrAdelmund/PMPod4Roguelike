using System;
using UnityUtils;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class Sensor : MonoBehaviour
{
    [SerializeField] public string sensorName;
    [SerializeField] float detectionRadius = 5f;
    [SerializeField] float timerInterval = 1f;

    CircleCollider2D detectionRange;

    public event Action OnTargetChanged = delegate { };

    public bool IsTargetInRange => target != null;
    public Vector2 TargetPosition => target != null ? (Vector2)target.transform.position : Vector2.zero;

    GameObject target;
    Vector2 lastknownPosition;
    CountdownTimer timer;

    void Awake()
    {
        detectionRange = GetComponent<CircleCollider2D>();
        detectionRange.isTrigger = true;
        detectionRange.radius = detectionRadius;

        timer = new CountdownTimer(timerInterval);
        timer.OnTimerStop += () =>
        {
            UpdateTargetPosition(target.OrNull());
            timer.Start();
        };
        
    }

    void Start()
    {
        timer.Start();
    }

    void Update()
    {
        timer.Tick(Time.deltaTime);
    }

    void UpdateTargetPosition(GameObject newTarget = null)
    {
        if (newTarget == target) return;
        target = newTarget;
        if (IsTargetInRange)
        {
            lastknownPosition = TargetPosition;
            OnTargetChanged.Invoke();
        }
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if (!other.CompareTag("Player")) return;
        UpdateTargetPosition(other.gameObject);
    }

    void OnTriggerExit2D(Collider2D other) 
    {
        if (!other.CompareTag("Player")) return;
        UpdateTargetPosition();
    }

    void OnDrawGizmos() 
    { 
        Gizmos.color = IsTargetInRange ? Color.red : Color.green;
        Gizmos.DrawWireSphere(TargetPosition, detectionRadius);
    }
}
