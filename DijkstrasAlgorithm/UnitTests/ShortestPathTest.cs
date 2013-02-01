﻿using Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Entities;
using System.Collections.Generic;
using System.Linq;

namespace UnitTests
{
    [TestClass()]
    public class ShortestPathTest
    {
        [TestMethod()]
        public void CreateTreeTest()
        {
            //Arrange
            IEnumerable<Edge> edges = new Edge[4]
            {
                new Edge{Start="a",End="b",Cost=1},
                new Edge{Start="b",End="c",Cost=1000},
                new Edge{Start="c",End="d",Cost=1},
                new Edge{Start="d",End="a",Cost=100},
            };
            ShortestPath_Accessor target = new ShortestPath_Accessor(edges);

            //Act
            Dictionary<string, Node> tree = target.CreateMatrix(edges);

            //Assert
            Assert.AreEqual(2, tree["a"].Neighbors.Count);
        }

        [TestMethod()]
        public void GetShortestPathTest()
        {
            //Arrange
            //IEnumerable<Edge> edges = new Edge[4]
            //{
            //    new Edge{Start="a",End="b",Cost=1},
            //    new Edge{Start="b",End="c",Cost=1000},
            //    new Edge{Start="c",End="d",Cost=1},
            //    new Edge{Start="d",End="a",Cost=100},
            //};

            IEnumerable<Edge> edges = new Edge[14]
            {
                new Edge{Start="o",End="a",Cost=2},
                new Edge{Start="o",End="b",Cost=5},
                new Edge{Start="o",End="c",Cost=4},

                new Edge{Start="a",End="f",Cost=12},
                new Edge{Start="a",End="d",Cost=7},
                new Edge{Start="a",End="b",Cost=2},

                new Edge{Start="b",End="d",Cost=4},
                new Edge{Start="b",End="e",Cost=3},
                new Edge{Start="b",End="c",Cost=1},

                new Edge{Start="c",End="e",Cost=4},

                new Edge{Start="d",End="e",Cost=1},
                new Edge{Start="d",End="t",Cost=5},

                new Edge{Start="e",End="t",Cost=7},

                new Edge{Start="f",End="t",Cost=3}
            };
            IShortestPath targt = new ShortestPath(edges);

            //Act
            var path = targt.GetShortestPath("o", "t");

            //Assert
            Assert.AreEqual(4, path.Count());
            Assert.AreEqual(13, path.Sum(e => e.Cost));
        }
    }
}