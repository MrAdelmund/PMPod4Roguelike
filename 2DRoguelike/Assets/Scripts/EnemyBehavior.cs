using UnityEngine;

public abstract class EnemyBehavior : MonoBehaviour
{
    protected Enemy enemy;

    public virtual void Initialize(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public virtual void Tick() 
    { 

    }
}