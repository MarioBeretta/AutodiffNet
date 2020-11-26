using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace AutoDiffNet.Terms
{
    class Product : Term
    {
        Term left;
        Term right;

        Product(Term left, Term right)
        {
            this.left = left;
            this.right = right;
        }

        internal static Term Build(Term left, Term right)
        {
            return new Product(left, right);
        }

       
        public override Expression Expr(Expression param)
        {
            return Expression.Multiply(left.Expr(param), right.Expr(param));
        }

        public override Expression GradExpr(Expression param, int dx)
        {
            return Expression.Add(
                Expression.Multiply(left.GradExpr(param, dx), right.Expr(param)),
                Expression.Multiply(left.Expr(param), right.GradExpr(param, dx))
                );
        }
    }
}
