using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Rendering.VolumeComponent;

public class GoapAgent : EnemyBehavior
{
    [Header("Sensors")]
    Dictionary<string, Sensor> sensors;

    Rigidbody2D rb;

    [Header("Stats / Config")]
    public float health = 1500;
    public float stamina = 100;
    public int dmg = 25;

    public float moveSpeed = 1.5f;
    public bool isMoving = false;
    public float lungeSpeed = 6f;
    public float lungeCooldown = 3f;

    // --- Axe system ---
    public GameObject axePrefab;
    BossAxe currentAxe;
    bool axeThrown = false;
    public Vector2 currentAxePos;
    public bool IsAxeOnGround() => currentAxe != null;
    public bool IsNearAxe() => currentAxe != null && InRangeOf(currentAxePos, 1f);

    float axePickupDelay = 0.5f;
    float axeSpawnTime;
    public float attackTimer = 0f;
    public float axeTimer = 0f;
    public float axeCooldown;

    public Vector2 minBounds;
    public Vector2 maxBounds;

    CountdownTimer statsTimer;

    GameObject player;
    public Vector2 PlayerPos => sensors["chase"].TargetPosition;

    public bool InRangeOf(Vector2 pos, float range) => Vector2.Distance(new Vector2(transform.position.x, transform.position.y), pos) <= range;

    AgentGoal lastGoal;
    public AgentGoal currentGoal;
    public ActionPlan actionPlan;
    public AgentAction currentAction;

    public Dictionary<string, AgentBelief> beliefs;
    public HashSet<AgentAction> actions;
    public HashSet<AgentGoal> goals;

    IGoapPlanner gPlanner;

    public override void Initialize(Enemy enemy)
    {
        base.Initialize(enemy);

        health = enemy.currentHealth;
        player = enemy.GetPlayerObject();
        rb = transform.parent.GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;

        gPlanner = new GoapPlanner();

        SetupTimers();
        GatherSensors();
        SetupBeliefs();
        SetupActions();
        SetupGoals();
    }

    void GatherSensors()
    {
        sensors = new Dictionary<string, Sensor>();
        
        foreach (var sensor in transform.parent.GetComponentsInChildren<Sensor>())
        {
            sensors[sensor.sensorName] = sensor;
            Debug.Log($"Registered sensor: {sensor.sensorName}");
        }
    }

    void SetupTimers()
    {
        statsTimer = new CountdownTimer(2f);
        statsTimer.OnTimerStop += () =>
        {
            UpdateStats();
            statsTimer.Start();
        };
        statsTimer.Start();
    }

    void UpdateStats()
    {
        stamina += !isMoving ? 20 : -10;
        stamina = Mathf.Clamp(stamina, 0, 100);
        health = enemy.currentHealth;
    }

    void SetupBeliefs()
    {
        beliefs = new Dictionary<string, AgentBelief>();

        BeliefFactory factory = new BeliefFactory(this, beliefs);

        factory.AddBelief("AgentIdle", () => !isMoving);
        factory.AddBelief("AgentMoving", () => isMoving);

        factory.AddBelief("AgentStaminaLow", () => stamina < 40);
        factory.AddBelief("AgentIsRested", () => stamina >= 50);

        factory.AddBelief("PlayerInMeleeRange", () => sensors["melee"].IsTargetInRange);
        factory.AddBelief("PlayerInMidRange", () => sensors["mid"].IsTargetInRange && !sensors["melee"].IsTargetInRange);
        factory.AddBelief("PlayerFarAway", () => !sensors["chase"].IsTargetInRange);

        factory.AddBelief("LungeReady", () => attackTimer >= lungeCooldown);
        factory.AddBelief("AxeReady", () => axeTimer >= axeCooldown);

        factory.AddBelief("HasAxe", () => currentAxe == null && !axeThrown);
        factory.AddBelief("AxeThrown", () => axeThrown);
        factory.AddBelief("AxeOnGround", () => currentAxe != null);
        factory.AddBelief("NearAxe", () => currentAxe != null && InRangeOf(currentAxePos, 1f));

        factory.AddBelief("PlayerTooClose", () => InRangeOf(PlayerPos, 1.5f));
        factory.AddBelief("UnderPressure", () => InRangeOf(PlayerPos, 1.5f) && stamina < 30);

        factory.AddBelief("WantsToEngage", () => stamina >= 20);
        factory.AddBelief("WantsToDisengage", () => stamina < 20 || beliefs["PlayerTooClose"].Evaluate());

        factory.AddBelief("IsEnraged", () => health < 1000);
    }

    void SetupActions()
    {
        actions = new HashSet<AgentAction>();

        // CHAIN: EngagePlayer / ApplyPressure
        // Moves agent to melee range — foundation of most chains
        actions.Add(new AgentAction.Builder("MoveToPlayer")
            .WithStrategy(new MoveToPlayerStrategy(this, rb, moveSpeed))
            .AddPrecondition(beliefs["WantsToEngage"])
            .AddEffect(beliefs["PlayerInMeleeRange"])
            .WithCost(2f)
            .Build());

        // CHAIN: ApplyPressure
        // Requires being in melee range first (MoveToPlayer), then lunges
        actions.Add(new AgentAction.Builder("Lunge")
            .WithStrategy(new LungeStrategy(this, rb))
            .AddPrecondition(beliefs["PlayerInMeleeRange"])
            .AddPrecondition(beliefs["LungeReady"])
            .AddEffect(beliefs["PlayerTooClose"])
            .WithCost(1f)
            .Build());

        // CHAIN: HarassWithAxe
        // Move to mid range first, then throw
        actions.Add(new AgentAction.Builder("MoveToMidRange")
            .WithStrategy(new KiteStrategy(this, rb, 5f, moveSpeed))
            .AddPrecondition(beliefs["WantsToEngage"])
            .AddEffect(beliefs["PlayerInMidRange"])
            .WithCost(2f)
            .Build());

        actions.Add(new AgentAction.Builder("ThrowAxe")
            .WithStrategy(new ThrowAxeStrategy(this))
            .AddPrecondition(beliefs["HasAxe"])
            .AddPrecondition(beliefs["AxeReady"])
            .AddPrecondition(beliefs["PlayerInMidRange"])
            .AddEffect(beliefs["AxeThrown"])
            .WithCost(1.5f)
            .Build());

        // CHAIN: Rearm
        // Move to axe then pick it up
        actions.Add(new AgentAction.Builder("RetrieveAxe")
            .WithStrategy(new RetrieveAxeStrategy(this, rb))
            .AddPrecondition(beliefs["AxeOnGround"])
            .AddEffect(beliefs["HasAxe"])
            .WithCost(1.2f)
            .Build());

        // CHAIN: Escape
        // Only when under pressure
        actions.Add(new AgentAction.Builder("Dodge")
            .WithStrategy(new DodgeStrategy(rb, moveSpeed))
            .AddPrecondition(beliefs["UnderPressure"])
            .AddEffect(beliefs["PlayerFarAway"])
            .WithCost(1.5f)
            .Build());

        // CHAIN: Recover
        actions.Add(new AgentAction.Builder("Rest")
            .WithStrategy(new IdleStrategy(2f))
            .AddPrecondition(beliefs["AgentStaminaLow"])
            .AddEffect(beliefs["AgentIsRested"])
            .WithCost(3f)
            .Build());

        // CHAIN: Patrol
        actions.Add(new AgentAction.Builder("Wander")
            .WithStrategy(new WanderStrategy(this, rb, 6f, moveSpeed))
            .AddPrecondition(beliefs["AgentIsRested"])
            .AddEffect(beliefs["AgentMoving"])
            .WithCost(5f)
            .Build());
    }

    void SetupGoals()
    {
        goals = new HashSet<AgentGoal>();

        // Priority 4 — highest, boss should always want to not be exhausted
        goals.Add(new AgentGoal.Builder("Recover")
            .WithPriority(4)
            .WithDesiredEffect(beliefs["AgentIsRested"])
            .Build());

        // Priority 3.5 — get axe back if it's on the ground
        goals.Add(new AgentGoal.Builder("Rearm")
            .WithPriority(3.5f)
            .WithDesiredEffect(beliefs["HasAxe"])
            .Build());

        // Priority 3 — throw axe at player when at mid range
        goals.Add(new AgentGoal.Builder("HarassWithAxe")
            .WithPriority(3f)
            .WithDesiredEffect(beliefs["AxeThrown"])
            .Build());

        // Priority 2.5 — get in range of player
        goals.Add(new AgentGoal.Builder("EngagePlayer")
            .WithPriority(2.5f)
            .WithDesiredEffect(beliefs["PlayerInMeleeRange"])
            .Build());

        // Priority 1.5 — lunge onto player
        goals.Add(new AgentGoal.Builder("ApplyPressure")
            .WithPriority(1.5f)
            .WithDesiredEffect(beliefs["PlayerTooClose"])
            .Build());

        // Priority 1 — escape when overwhelmed
        goals.Add(new AgentGoal.Builder("Escape")
            .WithPriority(1f)
            .WithDesiredEffect(beliefs["PlayerFarAway"])
            .Build());

        // Priority 0.5 — lowest, just move around when nothing else to do
        goals.Add(new AgentGoal.Builder("Patrol")
            .WithPriority(0.5f)
            .WithDesiredEffect(beliefs["AgentMoving"])
            .Build());
    }

    public override void Tick()
    {
        attackTimer += Time.deltaTime;
        axeTimer += Time.deltaTime;
        statsTimer.Tick(Time.deltaTime);

        if (currentAxe != null)
            currentAxePos = currentAxe.GetPosition();

        if (currentAction == null)
        {
            // Only replan if there are no actions left in the current plan
            if (actionPlan == null || actionPlan.Actions.Count == 0)
            {
                Debug.Log("Calculating potential new plans.");
                CalculatePlan();
            }

            // Pop next action from existing plan OR freshly calculated plan
            if (actionPlan != null && actionPlan.Actions.Count > 0)
            {
                rb.velocity = Vector2.zero;
                currentGoal = actionPlan.AgentGoal;
                currentAction = actionPlan.Actions.Pop();
                currentAction.Start();
                Debug.Log($"Goal: {currentGoal.Name} | Popped action: {currentAction.Name} | Remaining: {actionPlan.Actions.Count}");
            }
        }

        if (currentAction != null)
        {
            currentAction.Update(Time.deltaTime);

            if (currentAction.Complete)
            {
                Debug.Log($"{currentAction.Name} complete.");
                currentAction.Stop();
                currentAction = null;

                if (actionPlan.Actions.Count == 0)
                {
                    Debug.Log("Plan complete.");
                    lastGoal = currentGoal;
                    currentGoal = null;
                    actionPlan = null;
                }
            }
        }
    }

    void CalculatePlan()
    {
        var potentialPlan = gPlanner.Plan(this, goals, lastGoal);
        if (potentialPlan != null)
        {
            actionPlan = potentialPlan;
        }
    }

    public void ThrowAxe()
    {
        if (currentAxe != null) return;

        Vector2 dir = (PlayerPos - (Vector2)transform.position).normalized;

        GameObject axeObj = Instantiate(
            axePrefab,
            transform.position,
            Quaternion.identity
        );

        currentAxe = axeObj.GetComponent<BossAxe>();
        currentAxe.Initialize(dir);

        axeThrown = true;
        axeTimer = 0f;
        axeSpawnTime = Time.time;
    }

    public void RetrieveAxe()
    {
        if (currentAxe == null) return;

        if (Time.time - axeSpawnTime < axePickupDelay)
            return;

        if (Vector2.Distance(transform.position, currentAxePos) < 0.5f)
        {
            Destroy(currentAxe.gameObject);
            currentAxe = null;
            axeThrown = false;
            axeTimer = 0f;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerStats player = other.GetComponent<PlayerStats>();

        if (player != null)
        {
            player.TakeDamage(dmg);
        }
    }
}
