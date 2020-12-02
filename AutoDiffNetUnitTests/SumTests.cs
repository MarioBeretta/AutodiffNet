using AutoDiffNet;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoDiffNetUnitTests
{
    [TestClass]
    public class SumTests
    {
        [TestMethod]
        public void BasicSum()
        {
            Variable x = new Variable();
            Term f = Term.Sum(new[] { x[0], x[1], x[2] });
            double[] p = new double[] { 2, 6, 3 };
            var F = f.Compile();
            Assert.AreEqual(11, F(p));
        }

        [TestMethod]
        public void NestedSum()
        {
            Variable x = new Variable();
            Term g = Term.Sum(new[] { x[0], x[1], x[2] });
            Term f = Term.Sum(new[] { g, g, x[0] });

            double[] p = new double[] { 2, 6, 3 };
            var F = f.Compile();
            Assert.AreEqual(11+11+2, F(p));
        }

        [TestMethod]
        public void BasicSumGradient()
        {
            Variable x = new Variable();
            Term f = Term.Sum(new[] { 2*x[0], x[1], x[2] });
            double[] p = new double[] { 2, 6, 3 };
            var gradX0 = f.Grad(0);
            Assert.AreEqual(2, gradX0(p));
        }

        [TestMethod]
        public void BasicSumGradientSqr()
        {
            Variable x = new Variable();
            Term f = Term.Sum(new[] { 2 * x[0]*x[0], x[1], x[2] });
            double[] p = new double[] { 2, 6, 3 };
            var gradX0 = f.Grad(0);
            Assert.AreEqual(8, gradX0(p));
        }

        [TestMethod]
        public void BasicSumGradient3()
        {
            Variable x = new Variable();
            Term f = Term.Sum(new[] { 2 * x[0] * x[0], 3*x[0], x[2] });
            double[] p = new double[] { 2, 6, 3 };
            var gradX0 = f.Grad(0);
            Assert.AreEqual(11, gradX0(p));
        }
    }
}
