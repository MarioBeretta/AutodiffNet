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
            Expression<Func<double,double>> expExpression= x=>Math.Exp(x);
            return Expression.Invoke(expExpression, body.Expr(param));
            
        }

        public override Expression GradExpr(Expression param, int dx)
        {
            Expression<Func<double, double>> expExpression = x => Math.Exp(x);
            return Expression.Invoke(expExpression, body.Expr(param));

        }
    }
}
