using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace AutoDiffNet.Terms
{
    class Exp : Term
    {
        Term body;
        public Exp(Term f)
        {
            body = f;
        }

        public override Expression Expr(Expression param)
        {
            return ExpressionEx.Exp(body.Expr(param));
            
        }

        public override Expression GradExpr(Expression param, int dx)
        {
            return Expression.Multiply(ExpressionEx.Exp(body.Expr(param)), body.GradExpr(param, dx));
        }

        public override double EvalGradient(double[] x, int dx) => body.EvalGradient(x, dx) * Math.Exp(body.Eval(x));
        public override double Eval(double[] x) => Math.Exp(body.Eval(x));
        public override string GradientString(int dx)
        {
            return $"( exp( {body.ToString()} )* {body.GradientString(dx)} )";
        }

        public override string ToString()
        {
            return $"exp({body.ToString()})";
        }
    }
}
