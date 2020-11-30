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
        public Func<double[], double> Grad(int dx)
        {

            var vect = Expression.Parameter(typeof(double[]), "X");
            var expr = GradExpr(vect, dx);
            return Expression.Lambda<Func<double[], double>>(expr, vect).Compile();
        }

        public abstract double EvalGradient(double[] x, int dx);
        public abstract Expression Expr(Expression param);

        public abstract Expression GradExpr(Expression param, int dx);

        public abstract double Eval(double[] x);

        public  Func<double[], double> Compile()
        {
            var vect = Expression.Parameter(typeof(double[]), "X");
            var expr = Expr(vect);
            return Expression.Lambda<Func<double[], double>>(expr, vect).Compile();
        }
        

    }
}
