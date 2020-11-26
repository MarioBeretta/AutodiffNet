using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace AutoDiffNet
{
    public static class ExpressionEx
    {
        public static Expression Ln(Expression f)
        {
            Expression<Func<double, double>> e = x => Math.Log(x);
            return Expression.Invoke(e, f);
        }

        public static Expression Exp(Expression f)
        {
            Expression<Func<double, double>> e = x => Math.Exp(x);
            return Expression.Invoke(e, f);
        }

        public static Expression Pow(Expression b, Expression exponent)
        {
            Expression<Func<double, double, double>> e = (x,y) => Math.Pow(x,y);
            return Expression.Invoke(e, b, exponent);
        }

        public static Expression Sin(Expression f)
        {
            Expression<Func<double, double>> e = x => Math.Sin(x);
            return Expression.Invoke(e, f);
        }

        public static Expression Cos(Expression f)
        {
            Expression<Func<double, double>> e = x => Math.Cos(x);
            return Expression.Invoke(e, f);
        }

        public static Expression Tan(Expression f)
        {
            Expression<Func<double, double>> e = x => Math.Tan(x);
            return Expression.Invoke(e, f);
        }

    }
}
