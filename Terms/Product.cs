using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace AutoDiffNet.Terms
{
    class Product : BinaryTerm
    {

        Product(Term left, Term right)
        {
            this.Left = left;
            this.Right = right;
        }

        internal static Term Build(Term left, Term right)
        {
            return new Product(left, right);
        }

       
        public override Expression Expr(Expression param)
        {
            return Expression.Multiply(Left.Expr(param), Right.Expr(param));
        }

        public override Expression GradExpr(Expression param, int dx)
        {
            var f = Left.Expr(param);
            var f1 = Left.GradExpr(param, dx);
            var g = Right.Expr(param);
            var g1 = Right.GradExpr(param, dx);
            if (f1.IsZero() && g1.IsZero()) return Expression.Constant(0.0);
            if (f.IsZero() || g.IsZero()) return Expression.Constant(0.0);
            

            if (g1.IsZero()) return Expression.Multiply(f1, g);
            
            return Expression.Add(
                Expression.Multiply(f1, g),
                Expression.Multiply(g1, f)
                );
        }

        public override double EvalGradient(double[] x, int dx) => Left.EvalGradient(x, dx) * Right.Eval(x) + Left.Eval(x) * Right.EvalGradient(x, dx);

        public override double Eval(double[] x) => Left.Eval(x) * Right.Eval(x);
        
        public override string GradientString(int dx)
        {
            var f1 = Left.GradientString(dx);
            var g1 = Right.GradientString(dx);
            if (g1 == "0" && f1 == "0") return "0";
            if (g1 == "0") return $"{f1} * {Right.ToString()}";
            if (f1 == "0") return $"{Left.ToString()}*{g1}";
            return $"( {Left.GradientString(dx)} * {Right.ToString()} + {Left.ToString()}*{Right.GradientString(dx)} )";
        }

        public override string ToString()
        {
            return $"( {Left.ToString()} * {Right.ToString() } )";
        }
    }
}
