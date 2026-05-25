using System.Collections.Generic;

namespace LabyrinthGenerator.Core;

public interface INode<TVertexWeight, TEdgeWeight>
{
    public TVertexWeight Value
    {
        get; set;
    }

    public Dictionary<INode<TVertexWeight, TEdgeWeight>, TEdgeWeight> Connections
    {
        get;
        protected set;
    }

    /// <summary>
    /// Add neighbors to the node
    /// </summary>
    /// <param name="neighbor"></param>
    /// <param name="edgeWeight"></param>
    public void AddNeighbor(INode<TVertexWeight, TEdgeWeight> neighbor, TEdgeWeight edgeWeight);
}

public interface IGraph<TVertexWeight, TEdgeWeight> : IGetRandom<INode<TVertexWeight, TEdgeWeight>>
{
    /// <summary>
    /// Add a new unconnected node.
    /// </summary>
    /// <param name="node">the added node</param>
    public void AddNode(INode<TVertexWeight, TEdgeWeight> node);
    
    /// <summary>
    /// Add an oriented edge from one node to another. Adds nodes if they are not in the graph 
    /// </summary>
    public void AddOrientedEdge(INode<TVertexWeight, TEdgeWeight> from, INode<TVertexWeight, TEdgeWeight> to, TEdgeWeight weight);

    /// <summary>
    /// Add an unoriented edge from one to another. Adds notes if they are not in the graph
    /// 
    /// </summary>
    /// <param name="first"> first node </param>
    /// <param name="second"> second node</param>
    /// <param name="weightFirst"> weight of first -> second egde</param>
    /// <param name="weightSecond"> weight of second -> first edge</param>
    public void AddUnorientedEdge(INode<TVertexWeight, TEdgeWeight> first, INode<TVertexWeight, TEdgeWeight> second, TEdgeWeight weightFirst, TEdgeWeight weightSecond);

    /// <summary>
    /// Remove a node from a graph if the graph contains it
    /// </summary>
    /// <param name="node"></param>
    public void RemoveNode(INode<TVertexWeight, TEdgeWeight> node);
}
