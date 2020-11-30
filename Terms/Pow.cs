using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace AutoDiffNet.Terms
{
    class Pow : Term
    {
        Term Base;
        Term exponent;

        public Pow(Term value, Term exponent)
        {
            this.Base = value;
            this.exponent = exponent;
            this._rappr = ToString();
        }

        public override Expression Expr(Expression param)
        {
            return Expression.Power(Base.Expr(param), exponent.Expr(param));

        }

        public override Expression GradExpr(Expression param, int dx)
        {
            Expression<Func<double, double, double>> powExpression = (x, y) => Math.Pow(x, y);

            var f = Base.Expr(param);
            var f1 = Base.GradExpr(param, dx);

            var g = exponent.Expr(param);
            var g1 = exponent.GradExpr(param, dx);
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
            var f1 = Base.EvalGradient(x, dx);
            var g1 = exponent.EvalGradient(x, dx);
            if (f1 == 0 && g1 == 0) return 0;
            if (g1 == 0) return Math.Pow(Base.Eval(x), exponent.Eval(x) - 1);
            var f = Base.Eval(x);
            var g = exponent.Eval(x);
            return Math.Pow(f, g) * (g * f1 / f + Math.Log(f) * g1);
        }


        public override double Eval(double[] x) => Math.Pow(Base.Eval(x), exponent.Eval(x));

        public override string GradientString(int dx)
        {
            var f1 = Base.GradientString(dx);
            var g1 = exponent.GradientString(dx);
            if (g1 == "0" && f1 == "0") return "0";
            if (g1 == "0") return $"Pow({Base.ToString()}, {exponent}-1)";
            return $"Pow({Base.ToString()}, {exponent.ToString()}) * ( {exponent.ToString()} * {Base.GradientString(dx)} / {Base.ToString()} + Ln({Base.ToString()})*{exponent.GradientString(dx)}";
        }

        public override string ToString()
        {
            return $"Pow({Base.ToString()},{exponent.ToString()} )";
        }
    }
}
