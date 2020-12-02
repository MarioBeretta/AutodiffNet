using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace AutoDiffNet.Terms
{
    class Pow : BinaryTerm
    {
        public Pow(Term value, Term exponent)
        {
            this.Left = value;
            this.Right = exponent;
            this._rappr = ToString();
        }

        public override Expression Expr(Expression param)
        {
            return Expression.Power(Left.Expr(param), Right.Expr(param));

        }

        public override Expression GradExpr(Expression param, int dx)
        {
            Expression<Func<double, double, double>> powExpression = (x, y) => Math.Pow(x, y);

            var f = Left.Expr(param);
            var f1 = Left.GradExpr(param, dx);

            var g = Right.Expr(param);
            var g1 = Right.GradExpr(param, dx);
            if (f1.IsZero() && g1.IsZero()) return Expression.Constant(0.0);
            if (g1.IsZero())
                return Expression.Power(f, Expression.Subtract(g, Expression.Constant(1.0)));
            return Expression.Multiply(
                                Expression.Power(f, g),
                                Expression.Add(
                                    Expression.Divide(
                                        Expression.Multiply(g, f1),
                                        f),
                                    Expression.Multiply(
                                        ExpressionEx.Ln(f),
                                        g1)
                                )
                            );
        }

        public override double EvalGradient(double[] x, int dx) 
        {
            var f1 = Left.EvalGradient(x, dx);
            var g1 = Right.EvalGradient(x, dx);
            if (f1 == 0 && g1 == 0) return 0;
            if (g1 == 0) return Math.Pow(Left.Eval(x), Right.Eval(x) - 1);
            var f = Left.Eval(x);
            var g = Right.Eval(x);
            return Math.Pow(f, g) * (g * f1 / f + Math.Log(f) * g1);
        }


        public override double Eval(double[] x) => Math.Pow(Left.Eval(x), Right.Eval(x));

        public override string GradientString(int dx)
        {
            var f1 = Left.GradientString(dx);
            var g1 = Right.GradientString(dx);
            if (g1 == "0" && f1 == "0") return "0";
            if (g1 == "0") return $"Pow({Left.ToString()}, {Right}-1)";
            return $"Pow({Left.ToString()}, {Right.ToString()}) * ( {Right.ToString()} * {Left.GradientString(dx)} / {Left.ToString()} + Ln({Left.ToString()})*{Right.GradientString(dx)}";
        }

        public override string ToString()
        {
            return $"Pow({Left.ToString()},{Right.ToString()} )";
        }
    }
}
