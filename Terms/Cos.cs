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
    }
}
