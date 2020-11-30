using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace AutoDiffNet.Terms
{
    class Sin : Term
    {
        Term body;
        public Sin(Term f)
        {
            body = f;
        }

        public override Expression Expr(Expression param) => ExpressionEx.Sin(body.Expr(param));
        public override Expression GradExpr(Expression param, int dx) => ExpressionEx.Cos(body.Expr(param));
        public override double EvalGradient(double[] x, int dx) => Math.Cos(body.EvalGradient(x, dx));
        public override double Eval(double[] x) => Math.Sin(body.Eval(x));
        public override string GradientString(int dx)
        {
            return $" Cos({body.GradientString(dx)}) ";
        }

        public override string ToString()
        {
            return $"Sin({body.ToString()})";
        }
    }
}
