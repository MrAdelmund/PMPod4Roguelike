using System;
using System.Collections.Generic;
using UnityEngine;

//has info and methods to help make a dictionary of beliefs
public class BeliefFactory
{
    readonly GoapAgent agent;
    readonly Dictionary<string, AgentBelief> beliefs;

    public BeliefFactory(GoapAgent agent, Dictionary<string, AgentBelief> beliefs) 
    { 
        this.agent = agent;
        this.beliefs = beliefs;
    }

    //simple true / false belief
    public void AddBelief(string key, Func<bool> condition) 
    { 
        beliefs.Add(key, new AgentBelief.Builder(key)
            .WithCondition(condition)
            .Build()); ;
    }

    //location dependent beliefs
    public void AddLocationBelief(string key, float distance, Transform locationCondition) 
    { 
        AddLocationBelief(key, distance, locationCondition.position);
    }

    public void AddLocationBelief(string key, float distance, Vector2 locationCondition) 
    {
        beliefs.Add(key, new AgentBelief.Builder(key)
            .WithCondition(() => InRangeOf(locationCondition, distance))
            .WithLocation(() => locationCondition)
            .Build());
    }

    //sensor dependent beliefs
    public void AddSensorBelief(string key, Sensor sensor) 
    {
        beliefs.Add(key, new AgentBelief.Builder(key)
            .WithCondition(() => sensor.IsTargetInRange)
            .WithLocation(() => sensor.TargetPosition)
            .Build());
    }

    //proximity helper method
    bool InRangeOf(Vector2 pos, float range) => Vector2.Distance(agent.transform.position, pos) < range;
}

//beliefs themselves
public class AgentBelief
{
    public string Name { get; }

    Func<bool> condition = () => false;
    Func<Vector2> observedLocation = () => Vector3.zero;

    public Vector2 Location => observedLocation();

    AgentBelief(string name)
    {
        Name = name;
    }

    public bool Evaluate() => condition();


    //BUILDER
    public class Builder 
    {
        readonly AgentBelief belief;

        public Builder(string name) 
        {
            belief = new AgentBelief(name);
        }

        public Builder WithCondition(Func<bool> condition) 
        {
            belief.condition = condition;
            return this;
        }

        public Builder WithLocation(Func<Vector2> observedLocation) 
        {
            belief.observedLocation = observedLocation;
            return this;
        }

        public AgentBelief Build()
        {
            return belief;
        }
    }
}
