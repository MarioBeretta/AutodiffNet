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
            var Log = typeof(Math).GetMethod("Log",
                System.Reflection.BindingFlags.Public| System.Reflection.BindingFlags.Static,
                null,
                System.Reflection.CallingConventions.Any,
                new Type[] { typeof(double) }, null);
            return Expression.Call(Log, f);
        }

        public static Expression Exp(Expression f)
        {
            var Exp = typeof(Math).GetMethod("Exp",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static,
                null,
                System.Reflection.CallingConventions.Any,
                new Type[] { typeof(double) }, null);

            return Expression.Call(Exp, f);
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

        public static bool IsZero(this Expression f)
        {
            return (f.NodeType == ExpressionType.Constant && (double)((ConstantExpression)f).Value == 0.0);
        }


        

    }
}
