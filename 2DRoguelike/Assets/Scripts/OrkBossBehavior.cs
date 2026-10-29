using UnityEngine;

public class OrkBossBehavior : EnemyBehavior
{
    enum BossState
    {
        Chase,
        Lunge,
        Dodge,

        EnrageTransition,

        Kite_With_Axe,
        Retrieve_Axe
    }

    private BossState state;
    private float stateTimer;
    private bool enraged = false;

    private Transform player;
    private Rigidbody2D rb;

    // Tuning
    public float moveSpeed = 1.5f;
    public float lungeSpeed = 6f;
    public float lungeCooldown = 3f;
    public float dodgeChance = 0.1f;

    float axeCooldown = 3f;
    float axeTimer = 0f;

    private float attackTimer;

    // Phase timing
    float phaseDuration;

    public float minAxeTime = 3f;
    public float maxAxeTime = 6f;
    public float meleeDuration = 3f;

    public GameObject axePrefab;

    private BossAxe currentAxe;
    private bool axeThrown = false;
    private Vector2 axePosition;

    public override void Initialize(Enemy enemy)
    {
        base.Initialize(enemy);

        player = enemy.GetPlayerObject().transform;
        if (player == null) Debug.Log("Where da Player at?");
        Debug.Log(player.position);
        Debug.Log(player.name);
        rb = enemy.GetRigidbody();

        state = BossState.Chase;
    }

    public override void Tick()
    {
        if (player == null) return;

        stateTimer += Time.fixedDeltaTime;
        attackTimer += Time.fixedDeltaTime;
        axeTimer += Time.fixedDeltaTime;

        CheckEnrage();

        switch (state)
        {
            case BossState.Chase:
                HandleChase();
                break;

            case BossState.Lunge:
                HandleLunge();
                break;

            case BossState.Dodge:
                HandleDodge();
                break;

            case BossState.EnrageTransition:
                HandleEnrageTransition();
                break;

            case BossState.Kite_With_Axe:
                HandleAxePhase();
                break;

            case BossState.Retrieve_Axe:
                HandleRetrieveAxe();
                break;
        }
    }

    void CheckEnrage()
    {
        if (!enraged && enemy.currentHealth <= enemy.GetData().maxHealth * 0.5f)
        {
            enraged = true;
            SwitchState(BossState.EnrageTransition);
        }
    }

    void SwitchState(BossState newState)
    {
        state = newState;
        stateTimer = 0f;
    }

    // PHASE 1

    void HandleChase()
    {
        MoveTowardPlayer(moveSpeed);

        if (attackTimer >= lungeCooldown)
        {
            attackTimer = 0f;
            SwitchState(BossState.Lunge);
            return;
        }

        if (Random.value < dodgeChance * Time.fixedDeltaTime)
        {
            SwitchState(BossState.Dodge);
        }
    }

    Vector2 lungeDirection;

    void HandleLunge()
    {
        if (stateTimer < 0.5f)
        {
            lungeDirection = (player.position - transform.position).normalized;
            return;
        }

        if (stateTimer < 0.7f)
        {
            rb.MovePosition(rb.position + lungeDirection * lungeSpeed * Time.fixedDeltaTime);
            return;
        }

            SwitchState(BossState.Chase);
    }

    Vector2 dodgeDirection;

    void HandleDodge()
    {
        if (stateTimer < 0.02f)
        {
            dodgeDirection = Random.insideUnitCircle.normalized;
        }

        rb.MovePosition(rb.position + dodgeDirection * moveSpeed * 2f * Time.fixedDeltaTime);

        if (stateTimer > 0.3f)
            SwitchState(BossState.Chase);
    }

    // ENRAGE

    void HandleEnrageTransition()
    {
        if (stateTimer < 1.5f)
            return;

        StartAxePhase();
    }

    void StartAxePhase()
    {
        phaseDuration = Random.Range(minAxeTime, maxAxeTime);
        phaseDuration = Mathf.Max(phaseDuration, axeCooldown + 0.5f);

        axeTimer = 0f;

        SwitchState(BossState.Kite_With_Axe);
    }

    // AXE PHASE

    void HandleAxePhase()
    {
        // Slower movement while enraged
        MaintainDistance(6f, moveSpeed);

        axeTimer += Time.fixedDeltaTime;

        if (!axeThrown && axeTimer >= axeCooldown)
        {
            ThrowAxe();
            axeTimer = 0f;
        }

        if (axeThrown)
        {
            SwitchState(BossState.Retrieve_Axe);
        }
    }

    void ThrowAxe()
    {
        if (currentAxe != null) return;

        Vector2 dir = (player.position - transform.position).normalized;

        GameObject axeObj = Instantiate(
            axePrefab,
            transform.position,
            Quaternion.identity
        );

        currentAxe = axeObj.GetComponent<BossAxe>();
        currentAxe.Initialize(dir);

        Debug.Log($"Axe Direction: {dir}");

        axeThrown = true;
    }

    void HandleRetrieveAxe()
    {
        if (currentAxe == null)
        {
            axeThrown = false;
            SwitchState(BossState.Kite_With_Axe);
            return;
        }

        axePosition = currentAxe.GetPosition();

        Vector2 dir = (axePosition - (Vector2)transform.position).normalized;

        rb.MovePosition(rb.position + dir * lungeSpeed * Time.fixedDeltaTime);

        if (Vector2.Distance(transform.position, axePosition) < 0.5f && axeTimer >= 0.5f)
        {
            Destroy(currentAxe.gameObject);
            currentAxe = null;
            axeThrown = false;

            SwitchState(BossState.Kite_With_Axe);
        }
    }

    // MOVEMENT

    void MoveTowardPlayer(float speed)
    {
        Vector2 dir = (player.position - transform.position).normalized;
        rb.MovePosition(rb.position + dir * speed * Time.fixedDeltaTime);
    }

    void MoveAwayFromPlayer(float speed)
    {
        Vector2 dir = (transform.position - player.position).normalized;
        rb.MovePosition(rb.position + dir * speed * Time.fixedDeltaTime);
    }

    void MaintainDistance(float desiredDistance, float speed)
    {
        float dist = Vector2.Distance(transform.position, player.position);

        if (dist < desiredDistance)
            MoveAwayFromPlayer(speed);
        else if (dist > desiredDistance + 1f)
            MoveTowardPlayer(speed);
    }
}