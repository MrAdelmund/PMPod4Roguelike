using UnityEngine;

public interface IActionStrategy { 
    bool CanPerform { get; }
    bool Complete { get; }

    void Start() 
    {
        
    }

    void Update(float deltaTime) 
    { 
        
    }

    void Stop() 
    { 
        
    }
}

public class IdleStrategy : IActionStrategy
{
    public bool CanPerform => true;
    public bool Complete { get; private set; }

    readonly CountdownTimer timer;

    public IdleStrategy(float duration)
    {
        timer = new CountdownTimer(duration);
        timer.OnTimerStart += () => Complete = false;
        timer.OnTimerStop += () => Complete = true;
    }

    public void Start() => timer.Start();
    public void Update(float deltaTime) => timer.Tick(deltaTime);
}

public class WanderStrategy : IActionStrategy
{
    readonly GoapAgent agent;
    readonly Rigidbody2D rb;
    readonly float wanderRadius;
    readonly float speed;

    Vector2 target;
    bool hasTarget;

    public bool CanPerform => !Complete;
    public bool Complete => hasTarget && Vector2.Distance(rb.position, target) <= 0.2f;

    public WanderStrategy(GoapAgent agent, Rigidbody2D rb, float wanderRadius, float speed)
    {
        this.agent = agent;
        this.rb = rb;
        this.wanderRadius = wanderRadius;
        this.speed = speed;
    }

    public void Start()
    {
        Vector2 randomOffset = Random.insideUnitCircle * wanderRadius;
        Vector2 rawTarget = rb.position + randomOffset;

        target = new Vector2(
            Mathf.Clamp(rawTarget.x, agent.minBounds.x, agent.maxBounds.x),
            Mathf.Clamp(rawTarget.y, agent.minBounds.y, agent.maxBounds.y)
        );

        Debug.Log($"target at: {target}");

        hasTarget = true;
        agent.isMoving = true;
    }

    public void Update(float deltaTime)
    {
        if (!hasTarget) return;

        float distance = Vector2.Distance(rb.position, target);

        if (distance <= 0.2f)
        {
            hasTarget = false;
            agent.isMoving = false;
            return;
        }

        Vector2 dir = (target - rb.position).normalized;
        rb.MovePosition(rb.position + dir * speed * Time.deltaTime);
    }

    public void Stop()
    {
        hasTarget = false;
        agent.isMoving = false;
    }
}

public class MoveToPlayerStrategy : IActionStrategy
{
    readonly GoapAgent agent;
    readonly Rigidbody2D rb;
    readonly float speed;

    public bool CanPerform => true;
    public bool Complete => agent.InRangeOf(agent.PlayerPos, 2f);

    public MoveToPlayerStrategy(GoapAgent agent, Rigidbody2D rb, float speed)
    {
        this.agent = agent;
        this.rb = rb;
        this.speed = speed;
    }

    public void Start()
    {
        agent.isMoving = true;
    }

    public void Update(float deltaTime)
    {
        Vector2 dir = (agent.PlayerPos - rb.position).normalized;
        rb.MovePosition(rb.position + dir * speed * deltaTime);
    }

    public void Stop()
    {
        agent.isMoving = false;
    }
}

public class LungeStrategy : IActionStrategy
{
    readonly GoapAgent agent;
    readonly Rigidbody2D rb;

    float timer;
    Vector2 direction;

    public bool CanPerform => agent.attackTimer >= agent.lungeCooldown;
    public bool Complete => timer >= 0.7f;

    public LungeStrategy(GoapAgent agent, Rigidbody2D rb)
    {
        this.agent = agent;
        this.rb = rb;
    }

    public void Start()
    {
        timer = 0f;
        direction = (agent.PlayerPos - rb.position).normalized;
        agent.attackTimer = 0f;
    }

    public void Update(float deltaTime)
    {
        timer += deltaTime;

        if (timer < 0.5f)
            return;

        if (timer < 0.7f)
        {
            rb.MovePosition(rb.position + direction * agent.lungeSpeed * deltaTime);
        }
    }
}

public class KiteStrategy : IActionStrategy
{
    readonly GoapAgent agent;
    readonly Rigidbody2D rb;
    readonly float desiredDistance;
    readonly float speed;

    public bool CanPerform => true;
    public bool Complete => Mathf.Abs(Vector2.Distance(rb.position, agent.PlayerPos) - desiredDistance) < 0.5f;

    public KiteStrategy(GoapAgent agent, Rigidbody2D rb, float desiredDistance, float speed)
    {
        this.agent = agent;
        this.rb = rb;
        this.desiredDistance = desiredDistance;
        this.speed = speed;
    }

    public void Update(float deltaTime)
    {
        float dist = Vector2.Distance(rb.position, agent.PlayerPos);

        Vector2 dir;

        if (dist < desiredDistance)
            dir = (rb.position - agent.PlayerPos).normalized;
        else
            dir = (agent.PlayerPos - rb.position).normalized;

        rb.MovePosition(rb.position + dir * speed * deltaTime);
    }
}

public class ThrowAxeStrategy : IActionStrategy
{
    readonly GoapAgent agent;

    public bool CanPerform => agent.beliefs["HasAxe"].Evaluate() && agent.axeTimer >= agent.axeCooldown;
    public bool Complete { get; private set; }

    public ThrowAxeStrategy(GoapAgent agent)
    {
        this.agent = agent;
    }

    public void Start()
    {
        agent.ThrowAxe();
        Complete = true;
    }
}

public class RetrieveAxeStrategy : IActionStrategy
{
    readonly GoapAgent agent;
    readonly Rigidbody2D rb;

    public bool CanPerform => agent.IsAxeOnGround();
    public bool Complete => !agent.IsAxeOnGround();

    public RetrieveAxeStrategy(GoapAgent agent, Rigidbody2D rb)
    {
        this.agent = agent;
        this.rb = rb;
    }

    public void Update(float deltaTime)
    {
        if (!agent.IsAxeOnGround()) return;

        Vector2 dir = (agent.currentAxePos - rb.position).normalized;
        rb.MovePosition(rb.position + dir * agent.lungeSpeed * deltaTime);

        agent.RetrieveAxe();
    }
}

public class DodgeStrategy : IActionStrategy
{
    readonly Rigidbody2D rb;
    readonly float speed;

    float timer;
    Vector2 dir;

    public bool CanPerform => true;
    public bool Complete => timer > 0.3f;

    public DodgeStrategy(Rigidbody2D rb, float speed)
    {
        this.rb = rb;
        this.speed = speed;
    }

    public void Start()
    {
        timer = 0f;
        dir = Random.insideUnitCircle.normalized;
    }

    public void Update(float deltaTime)
    {
        timer += deltaTime;
        rb.MovePosition(rb.position + dir * speed * 2f * deltaTime);
    }
}