using AutoDiffNet;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoDiffNetUnitTests
{
    [TestClass]
    public class TestGradient
    {
        [TestMethod]
        public void FullGradient()
        {
            Variable x = new Variable();
            Term f = 2*x[0] + x[1];
            var grad = f.Gradient(new[] { 0, 1 });
            var g=grad(new double[] { 1.0, 2.0 });
            Assert.AreEqual(2, g[0]);
            Assert.AreEqual(1, g[1]);
        }

        [TestMethod]
        public void FullGradientNoOptimizationVar()
        {
            Variable x = new Variable();
            Term f = Term.Exp(x[0]+ x[1]);
            var grad = f.Gradient(new[] { 0, 1 }, ExpressionOptimizerFlags.DisableAll);
            var g = grad(new double[] { 1.0, 2.0 });
            Assert.AreEqual(Math.Exp(1 + 2) * 1, g[0]);
            Assert.AreEqual(Math.Exp(1 + 2) * 1, g[1]);
        }

        [TestMethod]
        public void FullGradientOptimizationVar()
        {
            Variable x = new Variable();
            Term f = Term.Exp(x[0] + x[1]);
            var grad = f.Gradient(new[] { 0, 1 }, ExpressionOptimizerFlags.Aggressive);
            var g = grad(new double[] { 1.0, 2.0 });
            Assert.AreEqual(Math.Exp(1 + 2) * 1, g[0]);
            Assert.AreEqual(Math.Exp(1 + 2) * 1, g[1]);
        }


        [TestMethod]
        public void FullGradientReuseVar1()
        {
            Variable x = new Variable();
            Term f = Term.Pow(2, x[0]) * Term.Pow(3, x[1]);
            var grad = f.Gradient(new[] { 0, 1 });
            var g = grad(new double[] { 1.0, 2.0 });
            Assert.AreEqual(Math.Pow(2, 1)*Math.Pow(3, 2)*Math.Log(2), g[0]);
            Assert.AreEqual(Math.Pow(2, 1) * Math.Pow(3, 2) * Math.Log(3), g[1]);
        }
    }
}
