using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace AutoDiffNet.Terms
{
    class Ln : Term
    {
        Term body;
        public Ln(Term f)
        {
            body = f;
        }

        

        public override Expression Expr(Expression param)
        {
            return ExpressionEx.Ln(body.Expr(param));
        }

        public override Expression GradExpr(Expression param, int dx)
        {
            return Expression.Divide(body.GradExpr(param, dx), body.Expr(param));
        }

        public override double EvalGradient(double[] x, int dx) => body.EvalGradient(x, dx) / body.Eval(x);
        public override double Eval(double[] x) => Math.Log(body.Eval(x));

        public override string GradientString(int dx)
        {
            return $"( {body.GradientString(dx)} / {body.ToString()} )";
        }

        public override string ToString()
        {
            return $"Ln({body.ToString()})";
        }
    }
}
