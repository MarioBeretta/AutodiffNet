using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace AutoDiffNet.Terms
{
    public class Sum:Term
    {
        Term[] elems;
        

        public Sum(Term[] elems)
        {
            this.elems = elems;
        }

        internal static Term Build(Term[] elems)
        {
            return new Sum(elems);
        }


        public override Expression Expr(Expression param)
        {
            var result = Expression.Parameter(typeof(double),"result");
            List<Expression> arr = new List<Expression>();
            arr.Add(Expression.Assign(result, Expression.Constant(0.0)));
            arr.AddRange(elems.Select(x => Expression.AddAssign(result, x.Expr(param))));
            var block = Expression.Block(
                new[] { result },
                arr
                );
            return block;
        }

        public override Expression GradExpr(Expression param, int dx)
        {
            var result = Expression.Parameter(typeof(double));
            List<Expression> arr = new List<Expression>();
            arr.Add(Expression.Assign(result, Expression.Constant(0.0)));
            arr.AddRange(elems.Select(x => Expression.AddAssign(result, x.GradExpr(param, dx))).Where(x=>!x.IsZero()));
            
            var block = Expression.Block(
                new[] {result},
                arr
                );
            return block;
        }
        public override double EvalGradient(double[] x, int dx)
        {
            double r = 0;
            foreach (var e in elems) r += e.EvalGradient(x, dx);
            return r;
        }
        public override double Eval(double[] x)
        {
            double r = 0;
            foreach (var e in elems) r += e.Eval(x);
            return r;
        }
        public override string GradientString(int dx)
        {
            return $"Sum({String.Join(",", elems.Select(x => x.GradientString(dx)))})";
        }

        public override string ToString()
        {
            return $"Sum({String.Join(",", elems.Select(x=>x.ToString()))})";
        }
    }
}

