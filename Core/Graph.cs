using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace LabyrinthGenerator.Core;

[DebuggerDisplay("Value = {Value}")]
public sealed class Node<TVertexWeight, TEdgeWeight> : INode<TVertexWeight, TEdgeWeight>
{
    public Dictionary<INode<TVertexWeight, TEdgeWeight>, TEdgeWeight> Connections
    {
        get;
        set;
    } = [];

    public TVertexWeight Value { get; set; }

    public Node(TVertexWeight value)
    {
        Value = value;
    }

    public void AddNeighbor(INode<TVertexWeight, TEdgeWeight> neighbor, TEdgeWeight edgeWeight) => Connections[neighbor] = edgeWeight;
}

public sealed class Graph<TVertexWeight, TEdgeWeight> : IGraph<TVertexWeight, TEdgeWeight>
{
    private readonly HashSet<INode<TVertexWeight, TEdgeWeight>> Nodes = [];

    public void AddNode(INode<TVertexWeight, TEdgeWeight> node)
    {
        Nodes.Add(node);
    }

    public void AddOrientedEdge(INode<TVertexWeight, TEdgeWeight> from, INode<TVertexWeight, TEdgeWeight> to, TEdgeWeight weight)
    {
        Nodes.Add(from);
        Nodes.Add(to);

        from.AddNeighbor(to, weight);
    }

    public void AddUnorientedEdge(INode<TVertexWeight, TEdgeWeight> first, INode<TVertexWeight, TEdgeWeight> second, TEdgeWeight weightFirst, TEdgeWeight weightSecond)
    {
        Nodes.Add(first);
        Nodes.Add(second);

        first.AddNeighbor(second, weightFirst);
        second.AddNeighbor(first, weightSecond);
    }

    /// <summary>
    /// Remove a node from a graph if the graph contains it
    /// </summary>
    /// <param name="node"></param>
    public void RemoveNode(INode<TVertexWeight, TEdgeWeight> node)
    {
        Nodes.Remove(node);

        foreach(var remainingNode in Nodes)
        {
            remainingNode.Connections.Remove(node);
        }
    }

    public IEnumerator<INode<TVertexWeight, TEdgeWeight>> GetEnumerator()
    {
        return Nodes.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public INode<TVertexWeight, TEdgeWeight> GetRandom()
    {
        return Nodes.ElementAt(Random.Shared.Next(Nodes.Count));
    }
}
