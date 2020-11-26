using AutoDiffNet.Terms;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace AutoDiffNet
{
    public abstract class Term
    {
        public static implicit operator Term(double value)
        {
            return Constant.Build(value);
        }

        public static Term Exp(Term f) => new Exp(f);
        public static Term Ln(Term f) => new Ln(f);

        public static Term operator* (Term left, Term right)
        {
            return Product.Build(left, right);
        }

        public static Term operator +(Term left, Term right)
        {
            return Sum.Build(left, right);
        }

        public static Term operator -(Term left, Term right)
        {
            return Sum.Build(left, -1*right);
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

        public Func<double[], double> Grad(int dx)
        {

            var vect = Expression.Parameter(typeof(double[]), "X");
            var expr = GradExpr(vect, dx);
            return Expression.Lambda<Func<double[], double>>(expr, vect).Compile();

        }

        public abstract Expression Expr(Expression param);

        public abstract Expression GradExpr(Expression param, int dx);

        public  Func<double[], double> Compile()
        {
            
            var vect = Expression.Parameter(typeof(double[]), "X");
            var expr = Expr(vect);
            return Expression.Lambda<Func<double[], double>>(expr, vect).Compile();
        }
        

    }
}
