using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace AutoDiffNet.Terms
{
    class Divide : BinaryTerm
    {

        Divide(Term left, Term right)
        {
            this.Left = left;
            this.Right = right;
        }

        internal static Term Build(Term left, Term right)
        {
            return new Divide(left, right);
        }


        public override Expression Expr(Expression param)
        {
            return Expression.Divide(Left.Expr(param), Right.Expr(param));
        }

        public override Expression GradExpr(Expression param, int dx)
        {
            var f1 = Left.GradExpr(param, dx);
            var g1 = Right.GradExpr(param, dx);
            var f = Left.Expr(param);
            var g = Right.Expr(param);

            return Expression.Subtract(
                Expression.Divide(f1, g),
                Expression.Divide(
                    Expression.Multiply(f, g1),
                    Expression.Multiply(g, g)));

        }

        public override double EvalGradient(double[] x, int dx) 
        {
            double g = Right.Eval(x);
            double g1 = Right.EvalGradient(x, dx);
            double f = Left.Eval(x);
            double f1 = Left.EvalGradient(x, dx);
            return f1 / g - f * g1 / (g * g);
        }
        public override double Eval(double[] x) => Left.Eval(x) * Right.Eval(x);


        public override string GradientString(int dx)
        {
            return $"{Left.GradientString(dx)} / {Right.ToString()} - ( {Left.ToString()} * {Right.GradientString(dx)} )/ ({Right.ToString()}*{Right.ToString()})";
        }

        public override string ToString()
        {
            return $" {Left.ToString()} / {Right.ToString() } ";
        }
    }
}
