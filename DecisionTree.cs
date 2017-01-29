using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

public interface ITreeNode
{
    bool NodeState { get; }
    bool Evaluate();
}

/// <summary>
/// Returns true when one of childs returns true
/// </summary>
public class Selector : ITreeNode
{
    private List<ITreeNode> childNodes;
    public bool NodeState { get; private set; } = false;

    public Selector(List<ITreeNode> childNodes) { this.childNodes = childNodes; }
    public bool Evaluate() => NodeState = childNodes.Any(node => node.Evaluate());
}

/// <summary>
/// Returns true when all childs return true
/// </summary>
public class Sequence : ITreeNode
{
    private List<ITreeNode> childNodes;
    public bool NodeState { get; private set; } = false;

    public Sequence(List<ITreeNode> childNodes) { this.childNodes = childNodes; }
    public bool Evaluate() => NodeState = childNodes.All(node => node.Evaluate());
}

/// <summary>
/// Has only one child, negate it
/// </summary>
public class Inverter : ITreeNode
{
    private ITreeNode nodeToInvert;
    public bool NodeState { get; private set; } = false;

    public Inverter(ITreeNode nodeToInvert) { this.nodeToInvert = nodeToInvert; }
    public bool Evaluate() => NodeState = !nodeToInvert.Evaluate();
}

/// <summary>
/// Leaf of tree, returns delegate of bool function that is setted in it's constuctor
/// </summary>
public class ActionNode : ITreeNode
{
    private Func<bool> action;
    public bool NodeState { get; private set; } = false;

    public ActionNode(Func<bool> action){
        this.action = action;
    }
    public bool Evaluate() => NodeState = action();
}

public static class DecisionTree
{
    public static void Test()
    {
        while (true)
        {
            AITree tree = new AITree();
            Console.ReadKey();
            Console.Clear();
        }
    }
}

public class AITree
{
    private float      playerDistanceFromEnemy;
    private int        playerPower;

    private readonly ActionNode IsInAttackRange;
    private readonly ActionNode IsVisible;
    private readonly ActionNode EstimatePlayerPower;
    private readonly Sequence   Attack;
    private readonly Inverter   Patrol;
    private readonly Sequence   Escape;
    private readonly Selector   Root;

    bool PlayerIsInAttackRange() => playerDistanceFromEnemy < 5;
    bool PlayerIsVisible()       => playerDistanceFromEnemy < 8;
    bool PlayerIsTooPowerful()   => playerPower > 3;

    public AITree()
    {
        Random rnd = new Random();
        playerDistanceFromEnemy = (float)rnd.Next(10, 100) / 10;
        playerPower = rnd.Next(1, 6);

        IsInAttackRange     = new ActionNode(PlayerIsInAttackRange);
        IsVisible           = new ActionNode(PlayerIsVisible);
        EstimatePlayerPower = new ActionNode(PlayerIsTooPowerful);
        Attack              = new Sequence(new List<ITreeNode> { IsInAttackRange, IsVisible });     // Attack only when player is visible and is in attack range
        Patrol              = new Inverter(Attack);                                                // Patrol only when not attacking
        Escape              = new Sequence(new List<ITreeNode> { IsVisible, EstimatePlayerPower }); // Escape when player is visible and player is too powerful 
        Root                = new Selector(new List<ITreeNode> { Escape, Patrol, Attack });         // Escape has the biggest priority

        Root.Evaluate();
        ShowCommunicats();
    }

    private void ShowCommunicats()
    {
        StringBuilder sb = new StringBuilder();
        Console.WriteLine($"Player distance: {playerDistanceFromEnemy}, Player power: {playerPower}");
        sb.AppendLine();

        Console.WriteLine(Patrol.NodeState          ? "enemy will patrol"                      : "enemy will not patrol");
        Console.WriteLine(Escape.NodeState          ? "enemy escapes"                          : "enemy will not escape");
        Console.WriteLine(IsVisible.NodeState       ? "enemy see player"                       : "enemy dont see player");
        Console.WriteLine(IsInAttackRange.NodeState ? "player is in the enemy attack distance" : "player is too far to hit");
        Console.WriteLine(Attack.NodeState          ? "enemy attacks"                          : "enemy will not attack");
    }
}