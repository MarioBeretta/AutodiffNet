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
            var f = left.Expr(param);
            var f1 = left.GradExpr(param, dx);
            var g = right.Expr(param);
            var g1 = right.GradExpr(param, dx);
            if (f1.IsZero() && g1.IsZero()) return Expression.Constant(0.0);
            if (g1.IsZero()) return Expression.Multiply(f1, g);
            return Expression.Add(
                Expression.Multiply(f1, g),
                Expression.Multiply(g1, f)
                );
        }

        public override double EvalGradient(double[] x, int dx) => left.EvalGradient(x, dx) * right.Eval(x) + left.Eval(x) * right.EvalGradient(x, dx);

        public override double Eval(double[] x) => left.Eval(x) * right.Eval(x);
        
        public override string GradientString(int dx)
        {
            var f1 = left.GradientString(dx);
            var g1 = right.GradientString(dx);
            if (g1 == "0" && f1 == "0") return "0";
            if (g1 == "0") return $"{f1} * {right.ToString()}";
            if (f1 == "0") return $"{left.ToString()}*{g1}";
            return $"( {left.GradientString(dx)} * {right.ToString()} + {left.ToString()}*{right.GradientString(dx)} )";
        }

        public override string ToString()
        {
            return $"( {left.ToString()} * {right.ToString() } )";
        }
    }
}
