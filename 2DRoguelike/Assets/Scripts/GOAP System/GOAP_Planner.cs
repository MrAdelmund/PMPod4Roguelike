using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IGoapPlanner
{
    ActionPlan Plan(GoapAgent agent, HashSet<AgentGoal> goals, AgentGoal mostRecentGoal = null);
}

public class GoapPlanner : IGoapPlanner
{
    public ActionPlan Plan(GoapAgent agent, HashSet<AgentGoal> goals, AgentGoal mostRecentGoal = null)
    {
        List<AgentGoal> orderedGoals = goals
            .Where(g => g.DesiredEffects.Any(b => !b.Evaluate()))
            .OrderByDescending(g => g == mostRecentGoal ? g.Priority - 0.01f : g.Priority)
            .ToList();

        foreach (var goal in orderedGoals)
        {
            Debug.Log($"Trying goal: {goal.Name}");
            var rootNode = new Node(null, null, goal.DesiredEffects, 0);

            if (FindPath(rootNode, agent.actions))
            {
                // Walk to cheapest leaf chain
                Stack<AgentAction> actionStack = new Stack<AgentAction>();
                var node = rootNode;

                while (node.Leaves.Count > 0)
                {
                    var cheapest = node.Leaves.OrderBy(l => l.Cost).First();
                    actionStack.Push(cheapest.Action);
                    node = cheapest;
                }

                Debug.Log($"Plan found for goal: {goal.Name} with {actionStack.Count} actions.");
                return new ActionPlan(goal, actionStack, node.Cost);
            }
        }

        Debug.LogWarning("No plan found!");
        return null;
    }

    bool FindPath(Node parent, HashSet<AgentAction> actions)
    {
        // Kabloink beliefs that are already true right now
        var requiredEffects = new HashSet<AgentBelief>(parent.RequiredEffects);
        requiredEffects.RemoveWhere(b => b.Evaluate());

        // BASE CASE: everything is already satisfied, path is complete
        if (requiredEffects.Count == 0)
        {
            Debug.Log("All requirements satisfied.");
            return true;
        }

        foreach (var action in actions)
        {
            // Skip actions that don't produce anything we need
            if (!action.Effects.Any(e => requiredEffects.Contains(e)))
                continue;

            // Build the next set of requirements:
            var newRequired = new HashSet<AgentBelief>(requiredEffects);
            newRequired.ExceptWith(action.Effects);
            newRequired.UnionWith(action.Preconditions);

            // Remove this action from the available pool to prevent infinite loops
            var remainingActions = new HashSet<AgentAction>(actions);
            remainingActions.Remove(action);

            var childNode = new Node(parent, action, newRequired, parent.Cost + action.Cost);

            if (FindPath(childNode, remainingActions))
            {
                parent.Leaves.Add(childNode);
                return true;
            }
        }

        // No action could complete the path from this point
        return false;
    }
}


public class Node
{
    public Node Parent { get; }
    public AgentAction Action { get; }
    public HashSet<AgentBelief> RequiredEffects { get; }
    public List<Node> Leaves { get; }
    public float Cost { get; }


    public Node(Node parent, AgentAction action, HashSet<AgentBelief> effects, float cost)
    {
        Parent = parent;
        Action = action;
        RequiredEffects = new HashSet<AgentBelief>(effects);
        Leaves = new List<Node>();
        Cost = cost;
    }
}

public class ActionPlan
{
    public AgentGoal AgentGoal { get; }
    public Stack<AgentAction> Actions { get; }
    public float TotalCost { get; set; }

    public ActionPlan(AgentGoal goal, Stack<AgentAction> actions, float totalCost)
    {
        AgentGoal = goal;
        Actions = actions;
        TotalCost = totalCost;
    }
}
