using System.Collections.Generic;
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

        IList<List<Edge>> IShortestPath.GetShortestPath(string source, string destination)
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

            var edges = new List<List<Edge>>();
            edges.Add(new List<Edge>());
            GetPaths(source, minimumNode, edges, edges.Count - 1);

            return edges;
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

            foreach (Node currentNode in visited)
            {
                IEnumerable<Node> unVisitedNeighbors = GetUnVisitedNeighbors(visited, currentNode);

                if (unVisitedNeighbors.Any())
                {
                    RelaxNeighboringNodes(currentNode, unVisitedNeighbors);

                    Node minimumNodeTillNow = unVisitedNeighbors.Aggregate((a, b) => a.CostFromSource < b.CostFromSource ? a : b);

                    if (!minimumNodeSoFar.CostFromSource.HasValue || minimumNodeSoFar.CostFromSource > minimumNodeTillNow.CostFromSource)
                    {
                        minimumNodeSoFar = minimumNodeTillNow;
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
            foreach (Node neighbor in unVisitedNeighbors)
            {
                int? newCost = currentNode.Neighbors[neighbor.Label] + currentNode.CostFromSource;

                if (neighbor.CostFromSource.HasValue)
                {
                    if (newCost < neighbor.CostFromSource)
                    {
                        neighbor.CostFromSource = newCost;
                        neighbor.Parents = new HashSet<Node> { currentNode };
                    }
                    else if (newCost == neighbor.CostFromSource && !neighbor.Parents.Contains(currentNode))//in case of tie
                    {
                        neighbor.Parents.Add(currentNode);
                    }
                }
                else
                {
                    neighbor.CostFromSource = newCost;
                    neighbor.Parents = new HashSet<Node> { currentNode };
                }
            }
        }

        private void GetPaths(string source, Node node, List<List<Edge>> edges, int level)
        {
            if (node.Parents == null)
            {
                return;
            }

            int i = 0;
            List<Edge> routeCopy = new List<Edge>(edges[level]);//make a copy of the current route

            foreach (Node originNode in node.Parents)
            {
                if (i > 0)//only branch after first edge, as first edge is added to default route
                {
                    edges.Add(routeCopy);
                    level++;
                }

                var edge = new Edge
                {
                    Start = originNode.Label,
                    End = node.Label,
                    Cost = _matrix[originNode.Label].Neighbors[node.Label]
                };

                edges[level].Add(edge);
                GetPaths(source, originNode, edges, level);
                i++;
            }
        }

        #endregion
    }
}