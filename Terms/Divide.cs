using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace AutoDiffNet.Terms
{
    class Divide : Term
    {
        Term left;
        Term right;

        Divide(Term left, Term right)
        {
            this.left = left;
            this.right = right;
        }

        internal static Term Build(Term left, Term right)
        {
            return new Divide(left, right);
        }


        public override Expression Expr(Expression param)
        {
            return Expression.Divide(left.Expr(param), right.Expr(param));
        }

        public override Expression GradExpr(Expression param, int dx)
        {
            var f1 = left.GradExpr(param, dx);
            var g1 = right.GradExpr(param, dx);
            var f = left.Expr(param);
            var g = right.Expr(param);

            return Expression.Subtract(
                Expression.Divide(f1, g),
                Expression.Divide(
                    Expression.Multiply(f, g1),
                    Expression.Multiply(g, g)));

        }

        public override double EvalGradient(double[] x, int dx) 
        {
            double g = right.Eval(x);
            double g1 = right.EvalGradient(x, dx);
            double f = left.Eval(x);
            double f1 = left.EvalGradient(x, dx);
            return f1 / g - f * g1 / (g * g);
        }
        public override double Eval(double[] x) => left.Eval(x) * right.Eval(x);


        public override string GradientString(int dx)
        {
            return $"{left.GradientString(dx)} / {right.ToString()} - ( {left.ToString()} * {right.GradientString(dx)} )/ ({right.ToString()}*{right.ToString()})";
        }

        public override string ToString()
        {
            return $" {left.ToString()} / {right.ToString() } ";
        }
    }
}
