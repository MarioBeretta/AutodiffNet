using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AutoDiffNet
{
    public class Variable 
    {
        class VariableElem : Term
        {
            int i;
           
            public VariableElem(int i)
            {
                this.i = i;
            }
            public override Expression Expr(Expression param)
            {
                List<Expression> idx = new List<Expression>() { Expression.Constant(i) };
                
                return Expression.ArrayAccess(param, idx);
            }

            public override Expression GradExpr(Expression param, int dx)
            {
                if (dx == i)
                    return Expression.Constant(1.0);
                return Expression.Constant(0.0);
            }
        }
        Expression X = Expression.Parameter(typeof(double[]), "X");


        public Term this[int i] { get { return new VariableElem(i); } }

    }

    
}
