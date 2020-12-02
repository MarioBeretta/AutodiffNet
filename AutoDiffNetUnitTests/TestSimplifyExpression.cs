using AutoDiffNet;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoDiffNetUnitTests
{
    [TestClass]
    public class TestSimplifyExpression
    {
        [TestMethod]
        public void TestSimplify1()
        {
            Variable x = new Variable();
            Term f = x[0] + x[1] + x[0];
            var v = f.Compile();

            Assert.AreEqual(5, v(new[] { 1.0, 3.0 }));
        }

        [TestMethod]
        public void TestSimplify2()
        {
            Variable x = new Variable();
            Term f = x[0] + Term.Pow(x[1],x[0]) + Term.Pow(x[1],x[0]);
            var v = f.Compile();

            Assert.AreEqual(2 + Math.Pow(3,2)+Math.Pow(3,2), v(new[] { 2.0, 3.0 }));
        }

        [TestMethod]
        public void TestSimplify3()
        {
            Variable x = new Variable();
            Term f = Term.Pow(x[0],2) + Term.Sum(new[] { Term.Pow(x[0],2), Term.Pow(x[1],2) });
            var v = f.Compile();

            Assert.AreEqual(4 + 4 + 9, v(new[] { 2.0, 3.0 }));
        }

        [TestMethod]
        public void TestSimplify4()
        {
            Variable x = new Variable();
            Term f = Term.Sum(new[] { Term.Pow(x[0], 2), Term.Pow(x[1], 2) })/ Term.Sum(new[] { Term.Pow(x[0], 2), Term.Pow(x[1], 2) });
            var v = f.Compile();

            Assert.AreEqual(1, v(new[] { 2.0, 3.0 }));
        }
    }
}
