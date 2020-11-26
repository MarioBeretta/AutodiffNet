﻿using System;
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
    }
}