using System;
using System.Collections.Generic;
using System.Text;

namespace AutoDiffNet
{
    public class VariableVect
    {
        public double[] v;
        public int Length => v.Length;
        public double this[int i] { get => v[i];  set { v[i] = value; } }

        public VariableVect(double[] v)
        {
            this.v = v;
        }
        public static double[] operator*(VariableVect l, double[] r)
        {
            var res = new double[l.Length];
            for (int i = 0; i < l.Length; i++) res[i] = l[i] * r[i];
            return res;
        }


    }
}
