using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace AutoDiffNet.Terms
{
    class Add : Term
    {
        Term left;
        Term right;
        
        Add(Term left, Term right)
        {
            this.left = left;
            this.right = right;
            this._rappr = ToString();
        }

        internal static Term Build(Term left, Term right)
        {
            return new Add(left, right);
        }


        public override Expression Expr(Expression param)
        {
            return Expression.Add(left.Expr(param), right.Expr(param));
        }

        public override Expression GradExpr(Expression param, int dx)
        {
            return Expression.Add(left.GradExpr(param, dx), right.GradExpr(param, dx));
        }
        public override double EvalGradient(double[] x, int dx) => left.EvalGradient(x, dx) + right.EvalGradient(x, dx);

        public override double Eval(double[] x) => left.Eval(x) + right.Eval(x);

        public override string GradientString(int dx)
        {
            var f1 = left.GradientString(dx);
            var g1 = right.GradientString(dx);
            if (f1 == "0") return $"{g1}";
            if (g1 == "0") return $"{f1}";

            return $"({left.GradientString(dx)} + {right.GradientString(dx)})";
        }

        public override string ToString()
        {
            return $"({left.ToString()} + {right.ToString() })";
        }
    }
}


