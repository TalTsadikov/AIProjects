using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Node
{
    public abstract bool Execute();
}

public class Selector : Node
{
    private List<Node> nodes;

    public Selector(List<Node> nodes)
    {
        this.nodes = nodes;
    }

    public override bool Execute()
    {
        foreach (var node in nodes)
        {
            if (node.Execute()) return true;
        }
        return false;
    }
}

public class Sequence : Node
{
    private List<Node> nodes;

    public Sequence(List<Node> nodes)
    {
        this.nodes = nodes;
    }

    public override bool Execute()
    {
        foreach (var node in nodes)
        {
            if (!node.Execute()) return false;
        }
        return true;
    }
}

public class ConditionNode : Node
{
    private System.Func<bool> condition;

    public ConditionNode(System.Func<bool> condition)
    {
        this.condition = condition;
    }

    public override bool Execute()
    {
        return condition();
    }
}

public class ActionNode : Node
{
    private System.Action action;

    public ActionNode(System.Action action)
    {
        this.action = action;
    }

    public override bool Execute()
    {
        action();
        return true;
    }
}

public class BehavierTree : MonoBehaviour
{
    private Node rootNode;

    private void Start()
    {
        rootNode = CreateBehaviorTree();
    }

    private void Update()
    {
        rootNode.Execute();
    }

    private Node CreateBehaviorTree()
    {
        var patrolNode = new ActionNode(() => GetComponent<EnemyController>().Patrol());
        var chaseNode = new ActionNode(() => GetComponent<EnemyController>().ChasePlayer());
        var attackNode = new ActionNode(() => GetComponent<EnemyController>().AttackPlayer());
        var searchNode = new ActionNode(() => GetComponent<EnemyController>().SearchPlayer());

        var isDeadNode = new ConditionNode(() => GetComponent<EnemyController>().health <= 0);
        var isChasingNode = new ConditionNode(() => GetComponent<EnemyController>().isChasing);
        var isAttackingNode = new ConditionNode(() => GetComponent<EnemyController>().isAttacking);
        var isSearchingNode = new ConditionNode(() => GetComponent<EnemyController>().isSearching);

        var attackSequence = new Sequence(new List<Node> { isAttackingNode, attackNode });
        var chaseSequence = new Sequence(new List<Node> { isChasingNode, chaseNode });
        var searchSequence = new Sequence(new List<Node> { isSearchingNode, searchNode });

        var behaviorSelector = new Selector(new List<Node> { attackSequence, chaseSequence, searchSequence, patrolNode });

        return new Selector(new List<Node> { new Sequence(new List<Node> { isDeadNode, new ActionNode(() => GetComponent<EnemyController>().Die()) }), behaviorSelector });
    }
}
