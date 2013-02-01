﻿using System.Collections.Generic;
using System.Linq;
using Entities;

namespace Services
{
    public class ShortestPath : IShortestPath
    {
        #region Member Variables

        readonly Dictionary<string, Node> _matrix; 

        #endregion

        #region Constructor

        public ShortestPath(IEnumerable<Edge> graph)
        {
            _matrix = CreateMatrix(graph);
        } 

        #endregion

        #region Public Methods

        IEnumerable<Edge> IShortestPath.GetShortestPath(string source, string destination)
        {
            var visited = new HashSet<Node> { _matrix[source] };
            _matrix[source].CostFromSource = 0;
            Node destinationNode = _matrix[destination];
            Node minimumNode = new Node();

            while (!visited.Contains(destinationNode))
            {
                minimumNode = GetMinimumNode(visited);
                visited.Add(minimumNode);
            }

            return GetPath(source, minimumNode);
        }

        #endregion

        #region Private Methods

        private Dictionary<string, Node> CreateMatrix(IEnumerable<Edge> graph)
        {
            var matrix = new Dictionary<string, Node>();

            foreach (var edge in graph)
            {
                Node firstVertex;

                if (!matrix.ContainsKey(edge.Start))
                {
                    firstVertex = new Node { Label = edge.Start, Neighbors = new Dictionary<string, int>() };
                    matrix.Add(edge.Start, firstVertex);
                }

                firstVertex = matrix[edge.Start];

                Node secondVertex;

                if (!matrix.ContainsKey(edge.End))
                {
                    secondVertex = new Node { Label = edge.End, Neighbors = new Dictionary<string, int>() };
                    matrix.Add(edge.End, secondVertex);
                }

                secondVertex = matrix[edge.End];

                if (!firstVertex.Neighbors.ContainsKey(secondVertex.Label))
                {
                    firstVertex.Neighbors.Add(secondVertex.Label, edge.Cost);
                }
                if (!secondVertex.Neighbors.ContainsKey(firstVertex.Label))
                {
                    secondVertex.Neighbors.Add(firstVertex.Label, edge.Cost);
                }
            }

            return matrix;
        }

        private Node GetMinimumNode(HashSet<Node> visited)
        {
            Node minimumNodeSoFar = new Node();
            int? minimumCostSoFar = minimumNodeSoFar.CostFromSource;

            foreach (Node currentNode in visited)
            {
                IEnumerable<Node> unVisitedNeighbors = GetUnVisitedNeighbors(visited, currentNode);

                if (unVisitedNeighbors.Any())
                {
                    RelaxNeighboringNodes(currentNode, unVisitedNeighbors);

                    Node minimumNodeTillNow = unVisitedNeighbors.Aggregate((a, b) => a.CostFromSource < b.CostFromSource ? a : b);

                    if (!minimumCostSoFar.HasValue || minimumCostSoFar > minimumNodeTillNow.CostFromSource)
                    {
                        minimumNodeSoFar = minimumNodeTillNow;
                        minimumCostSoFar = minimumNodeSoFar.CostFromSource;
                    }
                }
            }

            return minimumNodeSoFar;
        }

        private IEnumerable<Node> GetUnVisitedNeighbors(HashSet<Node> visited, Node currentNode)
        {
            IEnumerable<Node> unVisitedNeighbors = currentNode.Neighbors.Where(n => !visited.Contains(new Node { Label = n.Key })).Select(n => _matrix[n.Key]);
            return unVisitedNeighbors;
        }

        private void RelaxNeighboringNodes(Node currentNode, IEnumerable<Node> unVisitedNeighbors)
        {
            foreach (var neighbor in unVisitedNeighbors)
            {
                int? newCost = currentNode.Neighbors[neighbor.Label] + currentNode.CostFromSource;

                if (neighbor.CostFromSource.HasValue)
                {
                    if (newCost < neighbor.CostFromSource)
                    {
                        neighbor.CostFromSource = newCost;
                        neighbor.Parent = currentNode;
                    }
                }
                else
                {
                    neighbor.CostFromSource = newCost;
                    neighbor.Parent = currentNode;
                }
            }
        }

        private IEnumerable<Edge> GetPath(string source, Node minimumNode)
        {
            var edges = new List<Edge>();

            while (minimumNode.Label != source)
            {
                var e = new Edge
                {
                    Start = minimumNode.Parent.Label,
                    End = minimumNode.Label,
                    Cost = _matrix[minimumNode.Parent.Label].Neighbors[minimumNode.Label]
                };

                edges.Add(e);

                minimumNode = minimumNode.Parent;
            }

            return edges;
        }

        #endregion
    }
}