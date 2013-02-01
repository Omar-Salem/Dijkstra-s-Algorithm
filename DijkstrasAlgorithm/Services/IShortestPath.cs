using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entities;

namespace Services
{
    public interface IShortestPath
    {
        IEnumerable<Edge> GetShortestPath(string source, string destination);
    }
}