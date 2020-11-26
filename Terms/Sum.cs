using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace AutoDiffNet.Terms
{
    class Sum : Term
    {
        Term left;
        Term right;

        Sum(Term left, Term right)
        {
            this.left = left;
            this.right = right;
        }

        internal static Term Build(Term left, Term right)
        {
            return new Sum(left, right);
        }


        public override Expression Expr(Expression param)
        {
            return Expression.Add(left.Expr(param), right.Expr(param));
        }

        public override Expression GradExpr(Expression param, int dx)
        {
            return Expression.Add(left.GradExpr(param, dx), right.GradExpr(param, dx));
        }
    }
}


