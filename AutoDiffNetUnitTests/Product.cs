using AutoDiffNet;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutoDiffNetUnitTests
{
    [TestClass]
    public class Product
    {
        [TestMethod]
        public void Product1()
        {
            Variable x = new Variable();
            Term f = 2*x[0];

            var grad = f.Grad(0);
            double[] p = new double[1];
            p[0] = 0;
            Assert.AreEqual(2, grad(p));
            p[0] = 2;
            Assert.AreEqual(2, grad(p));
        }

        [TestMethod]
        public void Product2()
        {
            Variable x = new Variable();
            Term f = x[0] * x[1];

            var grad = f.Grad(0);
            double[] p = new double[2];
            p[0] = 0;
            p[1] = 3;
            Assert.AreEqual(p[1], grad(p));
            p[0] = 2;
            Assert.AreEqual(p[1], grad(p));
        }

        [TestMethod]
        public void Product3()
        {
            Variable x = new Variable();
            Term f = x[0] * Term.Pow(4, x[1]);

            var grad = f.Grad(0);
            double[] p = new double[2];
            p[0] = 0;
            p[1] = 3;
            Assert.AreEqual(64, grad(p));
            p[0] = 2;
            Assert.AreEqual(64, grad(p));
        }

        [TestMethod]
        public void Product4()
        {
            Variable x = new Variable();
            Term f = x[0] * x[1]*0;
            var F = f.Compile();

            var grad = f.Grad(0);
            double[] p = new double[2];
            p[0] = 0;
            p[1] = 3;
            Assert.AreEqual(0, grad(p));
            p[0] = 2;
            Assert.AreEqual(0, grad(p));
        }
    }
}
