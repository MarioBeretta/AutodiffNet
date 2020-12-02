using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace AutoDiffNet.Terms
{
    class Add : BinaryTerm
    {
        Add(Term Left, Term Right)
        {
            this.Left = Left;
            this.Right = Right;
            this._rappr = ToString();
        }

        internal static Term Build(Term Left, Term Right)
        {
            return new Add(Left, Right);
        }


        public override Expression Expr(Expression param)
        {
            return Expression.Add(Left.Expr(param), Right.Expr(param));
        }

        public override Expression GradExpr(Expression param, int dx)
        {
            return Expression.Add(Left.GradExpr(param, dx), Right.GradExpr(param, dx));
        }
        public override double EvalGradient(double[] x, int dx) => Left.EvalGradient(x, dx) + Right.EvalGradient(x, dx);

        public override double Eval(double[] x) => Left.Eval(x) + Right.Eval(x);

        public override string GradientString(int dx)
        {
            var f1 = Left.GradientString(dx);
            var g1 = Right.GradientString(dx);
            if (f1 == "0") return $"{g1}";
            if (g1 == "0") return $"{f1}";

            return $"({Left.GradientString(dx)} + {Right.GradientString(dx)})";
        }

        public override string ToString()
        {
            return $"({Left.ToString()} + {Right.ToString() })";
        }
    }
}


