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

        }

        public override Expression Expr(Expression param)
        {
            return ExpressionEx.Pow(Base.Expr(param), exponent.Expr(param));

        }

        public override Expression GradExpr(Expression param, int dx)
        {
            Expression<Func<double, double, double>> powExpression = (x, y) => Math.Pow(x, y);

            var f = Base.Expr(param);
            var f1 = Base.GradExpr(param, dx);

            var g = exponent.Expr(param);
            var g1 = exponent.GradExpr(param, dx);

            var res = Expression.Condition(
                        Expression.Equal(g1, Expression.Constant(0.0)),
                            ExpressionEx.Pow(f, Expression.Subtract(g, Expression.Constant(1.0))),
                            Expression.Multiply(
                                ExpressionEx.Pow(f, g),
                                Expression.Add(
                                    Expression.Divide(
                                        Expression.Multiply(g, f1),
                                        f),
                                    Expression.Multiply(
                                        ExpressionEx.Ln(f),
                                        g1)
                                )
                            )
                        );

            return res;

        }
    }
}
