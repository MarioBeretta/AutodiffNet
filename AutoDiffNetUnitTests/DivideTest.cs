using AutoDiffNet;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoDiffNetUnitTests
{
    [TestClass]
    public class DivideTest
    {
        [TestMethod]
        public void Divide1()
        {
            Variable x = new Variable();
            Term f = x[0]/(1-x[0]);

            var grad = f.Grad(0);
            double[] p = new double[1];
            p[0] = 0;
            Assert.AreEqual(1, grad(p));
            p[0] = 2;
            Assert.AreEqual(1, grad(p));

            p[0] = 5;
            Assert.AreEqual(1.0/16, grad(p));
        }
    }
}
