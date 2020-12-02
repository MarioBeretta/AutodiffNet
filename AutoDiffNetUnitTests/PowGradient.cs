using AutoDiffNet;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoDiffNetUnitTests
{
    [TestClass]
    public class PowGradient
    {
        [TestMethod]
        public void Pow1()
        {
            Variable x=new Variable();
            Term f = Term.Pow(4, x[0]);

            var grad = f.Grad(0);
            double[] p = new double[1];
            p[0] = 0;
            Assert.AreEqual(Math.Log(4), grad(p));
            p[0] = 1;
            Assert.AreEqual(4*Math.Log(4), grad(p));
            p[0] = 2;
            Assert.AreEqual(Math.Pow(4,p[0]) * Math.Log(4), grad(p));
        }

        [TestMethod]
        public void Pow2()
        {
            Variable x = new Variable();
            Term f = Term.Pow(x[1], x[0]);

            var grad = f.Grad(0);
            double[] p = new double[2];
            p[0] = 0;
            p[1] = 2;
            Assert.AreEqual(Math.Pow(p[1], p[0]) * Math.Log(p[1]), grad(p));
            p[0] = 1;
            Assert.AreEqual(Math.Pow(p[1], p[0]) * Math.Log(p[1]), grad(p));
            p[0] = 2;
            Assert.AreEqual(Math.Pow(p[1], p[0]) * Math.Log(p[1]), grad(p));
        }

        [TestMethod]
        public void Pow3()
        {
            Variable x = new Variable();
            Term f = 1;
            int n = 30;

           
            for(int i=0;i<n/2;i++) 
                f = f* Term.Pow(x[n/2+i], x[i]);

            
            double[] p = new double[n];
            for (int i = 0; i < n; i++) p[i] = 1;

            p[0] = 0;
            Assert.AreEqual(Math.Pow(p[3],p[2])*Math.Pow(p[1], p[0]) * Math.Log(p[1]), f.EvalGradient(p,0 ));
            p[0] = 2;
            Assert.AreEqual(Math.Pow(p[3], p[2]) * Math.Pow(p[1], p[0]) * Math.Log(p[1]), f.EvalGradient(p, 0));
            p[0] = 3;
            Assert.AreEqual(Math.Pow(p[3], p[2]) * Math.Pow(p[1], p[0]) * Math.Log(p[1]), f.EvalGradient(p, 0));
        }
    }
}
