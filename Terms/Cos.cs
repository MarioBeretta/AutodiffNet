using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace AutoDiffNet.Terms
{
    class Cos : Term
    {
        Term body;
        public Cos(Term f)
        {
            body = f;
        }

        public override Expression Expr(Expression param)
        {
            return ExpressionEx.Cos(body.Expr(param));
            
        }

        public override Expression GradExpr(Expression param, int dx)
        {
            return Expression.Multiply(Expression.Constant(-1.0), ExpressionEx.Sin(body.Expr(param)));
        }

        public override double EvalGradient(double[] x, int dx) => -Math.Sin(body.EvalGradient(x,dx));
        public override double Eval(double[] x) => Math.Cos(body.Eval(x));
        public override string GradientString(int dx)
        {
            return $" -Sin({body.GradientString(dx)}) ";
        }

        public override string ToString()
        {
            return $"Cos({body.ToString()})";
        }
    }
}
