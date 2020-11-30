using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace AutoDiffNet.Terms
{
    class Constant : Term
    {
        double v;
        public Constant(double v)
        {
            this.v=v;
        }
        internal static Term Build(double value)
        {
            return new Constant(value);
        }

        public override double EvalGradient(double[] x, int dx) => 0;

        public override double Eval(double[] x) => v;


        public override Expression Expr(Expression param)
        {
            return Expression.Constant(v);
        }

        public override Expression GradExpr(Expression param, int dx)
        {
            return Expression.Constant(0.0);
        }
        public override string GradientString(int dx)
        {
            return $"0";
        }

        public override string ToString()
        {
            return $"{v}";
        }
    }
}
