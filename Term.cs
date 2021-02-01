using AutoDiffNet.Terms;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace AutoDiffNet
{
    public abstract class Term
    {
        protected string _rappr;

        //public abstract TermTypes TermType {  get; }

        public Term()
        {
            
        }
        public static implicit operator Term(double value)
        {
            return Constant.Build(value);
        }

        public static Term Exp(Term f) => new Exp(f);
        public static Term Ln(Term f) => new Ln(f);
        public static Term Sin(Term f) => new Sin(f);
        public static Term Cos(Term f) => new Cos(f);
        public static Term Pow(Term b, Term e) => new Pow(b, e);

        public static Term operator* (Term left, Term right)
        {
            return Product.Build(left, right);
        }

        public static Term operator +(Term left, Term right)
        {
            return Add.Build(left, right);
        }

        public static Term Sum(Term[] terms) => new Sum(terms);

        public static Term operator -(Term left, Term right)
        {
            return Add.Build(left, -1*right);
        }

        public static Term operator /(Term left, Term right)
        {
            return Divide.Build(left, right);
        }

        public string GradString(int dx)
        {

            var vect = Expression.Parameter(typeof(double[]), "X");
            var expr = GradExpr(vect, dx);

            return expr.ToString();
        }

        public abstract string GradientString(int dx);
        public Func<double[], double> Grad(int dx, ExpressionOptimizerFlags optimizationFlags= ExpressionOptimizerFlags.Default)
        {

            var vect = Expression.Parameter(typeof(double[]), "X");
            var expr = GradExpr(vect, dx);
            if (!optimizationFlags.HasFlag(ExpressionOptimizerFlags.DisableAll)) expr = new ExpressionOptimizer().OptimizeExpression(expr, typeof(double));
            return Expression.Lambda<Func<double[], double>>(expr, vect).Compile();
        }

        public Func<double[], double[]> Gradient(IList<int> dxs, ExpressionOptimizerFlags optimizationFlags = ExpressionOptimizerFlags.Default)
        {
            ParameterExpression result = Expression.Parameter(typeof(double[]));
            
            var X = Expression.Parameter(typeof(double[]), "X");
            List<Expression> blockExpression = new List<Expression>();
            
            blockExpression.Add(Expression.Assign(result, Expression.NewArrayBounds(typeof(double), Expression.Constant(dxs.Count)))); // Allocate results

            int idx = 0;
            foreach(var dx in dxs)
            {
                var gdx = GradExpr(X, dx);
                blockExpression.Add(
                    Expression.Assign(
                        Expression.ArrayAccess(result, Expression.Constant(idx)),
                        gdx
                    ));
                idx++;
            }

            blockExpression.Add(result);
            Expression expr = Expression.Block(
                new[] { result },
                blockExpression
                );

            if (!optimizationFlags.HasFlag(ExpressionOptimizerFlags.DisableAll)) expr = new ExpressionOptimizer().OptimizeExpression(expr, typeof(double[]), optimizationFlags);

            var lambda = Expression.Lambda<Func<double[], double[]>>(expr, X);
            return lambda.Compile();
        }

        public abstract double EvalGradient(double[] x, int dx);
        public abstract Expression Expr(Expression param);

        public abstract Expression GradExpr(Expression param, int dx);

        public abstract double Eval(double[] x);

        public  Func<double[], double> Compile(ExpressionOptimizerFlags optimizationFlags = ExpressionOptimizerFlags.Default)
        {
            var vect = Expression.Parameter(typeof(double[]), "X");
            
            var expr = Expr(vect);
            if (!optimizationFlags.HasFlag(ExpressionOptimizerFlags.DisableAll)) expr = new ExpressionOptimizer().OptimizeExpression(expr, typeof(double));

            return Expression.Lambda<Func<double[], double>>(expr, vect).Compile();
        }
    }
}
