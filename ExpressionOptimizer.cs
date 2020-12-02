using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace AutoDiffNet
{
    [Flags]
    public enum ExpressionOptimizerFlags
    {
        DisableAll=1,
        ZeroDividedBy =2,
        ReuseOfDuplicatedExpression=4,
        MultiplyByZero=8,

        Default=MultiplyByZero|ReuseOfDuplicatedExpression,
        Aggressive = Default | ZeroDividedBy

    }

    class ExpressionOptimizer
    {
        class ExpressionStatistics
        {
            public Expression expression { get; set; }
            public int Usage { get; set; }

            public string Name { get; set; }
            public ParameterExpression PreComputeVariable { get; set; }

        }

        

        

        private void Visit(Expression e, Dictionary<string, ExpressionStatistics> dict)
        {
            string s = e.GetFullString();
            BinaryExpression binExpr = e as BinaryExpression;
            if (binExpr != null)
            {
                if (e.NodeType == ExpressionType.Assign) return;
                Visit(binExpr.Left, dict);
                Visit(binExpr.Right, dict);
                if (e.NodeType == ExpressionType.AddAssign) return;
            }
            else
            {
                UnaryExpression unaryExpression = e as UnaryExpression;
                if (unaryExpression != null)
                {
                    Visit(unaryExpression, dict);
                }
                else
                {
                    BlockExpression blockExpression = e as BlockExpression;
                    if (blockExpression != null)
                    {
                        foreach (var x in blockExpression.Expressions) Visit(x, dict);
                    }
                    else
                        return;
                }

            }

            ExpressionStatistics v;
            if (!dict.TryGetValue(s, out v))
            {
                v = new ExpressionStatistics() { expression = e, Name = "Var_" + dict.Count.ToString(), Usage = 1 };
                dict.Add(s, v);
            }
            else
                v.Usage += 1;
        }

        private Expression ReplaceExpression(Expression e, Dictionary<string, ExpressionStatistics> dict)
        {
            string s = e.GetFullString();
            if (dict.ContainsKey(s))
            {
                var v = dict[s];
                if (v.Usage > 1)
                {
                    return v.PreComputeVariable;
                }
            }

            

            BinaryExpression binExpr = e as BinaryExpression;
            if (binExpr != null)
            {
                var left = ReplaceExpression(binExpr.Left, dict);
                var right = ReplaceExpression(binExpr.Right, dict);
                if (e.NodeType== ExpressionType.Multiply)
                    if (left.IsZero() || right.IsZero()) return Expression.Constant(0.0);
                

                return binExpr.Update(left, binExpr.Conversion, right);
                
            }
            UnaryExpression unaryExpression = e as UnaryExpression;
            if (unaryExpression != null)
            {
                var body = ReplaceExpression(unaryExpression, dict);
                return unaryExpression.Update(body);
            }
            BlockExpression blockExpression = e as BlockExpression;
            if (blockExpression != null)
            {
                var elems = blockExpression.Expressions.Select(x => ReplaceExpression(x, dict)).Where(x=>!x.IsZero());
                return blockExpression.Update(blockExpression.Variables, elems);
                
            }
            return e;
        }

        private Expression OptimizeSimpleOperation(Expression e, ExpressionOptimizerFlags flags)
        {
            BinaryExpression binExpr = e as BinaryExpression;
            if (binExpr != null)
            {
                var left = OptimizeSimpleOperation(binExpr.Left, flags);
                var right = OptimizeSimpleOperation(binExpr.Right, flags);
                if (e.NodeType == ExpressionType.Multiply && flags.HasFlag(ExpressionOptimizerFlags.MultiplyByZero))
                    if (left.IsZero() || right.IsZero()) return Expression.Constant(0.0);

                if (e.NodeType == ExpressionType.Add || e.NodeType==ExpressionType.Subtract)
                {
                    if (left.IsZero()) return right;
                    if (right.IsZero()) return left;
                }

                if (e.NodeType == ExpressionType.Divide)
                    if (left.IsZero() && flags.HasFlag(ExpressionOptimizerFlags.ZeroDividedBy)) 
                        return Expression.Constant(0.0); // with this I'm assuming that 0/0 is ok to be 0 -> The user need to explicitly validate this condittion

                return binExpr.Update(left, binExpr.Conversion, right);

            }
            UnaryExpression unaryExpression = e as UnaryExpression;
            if (unaryExpression != null)
            {
                var body = OptimizeSimpleOperation(unaryExpression, flags);
                return unaryExpression.Update(body);
            }
            BlockExpression blockExpression = e as BlockExpression;
            if (blockExpression != null)
            {
                var elems = blockExpression.Expressions.Select(x => OptimizeSimpleOperation(x, flags));
                return blockExpression.Update(blockExpression.Variables, elems);
            }
            return e;
        }



        public Expression OptimizeExpression(Expression expr, ExpressionOptimizerFlags flags=ExpressionOptimizerFlags.Default)
        {
            Dictionary<string, ExpressionStatistics> expressions = new Dictionary<string, ExpressionStatistics>();
            expr = OptimizeSimpleOperation(expr, flags);
            if (flags.HasFlag(ExpressionOptimizerFlags.ReuseOfDuplicatedExpression))
            {
                Visit(expr, expressions);
                var variables = new List<ParameterExpression>();
                var preComputation = new List<Expression>();

                foreach (var v in expressions)
                {
                    if (v.Value.Usage > 1)
                    {
                        var variable = Expression.Variable(typeof(double), v.Value.Name);
                        v.Value.PreComputeVariable = variable;
                        variables.Add(variable);
                        preComputation.Add(Expression.Assign(variable, v.Value.expression));
                    }
                }

                var newExpression = ReplaceExpression(expr, expressions);
                if (preComputation.Count > 0)
                {

                    var result = Expression.Parameter(typeof(double));
                    variables.Add(result);
                    preComputation.Add(Expression.Assign(result, newExpression));
                    var res = Expression.Block(variables.ToArray(), preComputation.ToArray());
                    return res;
                }
            }

            return expr;
        }
    }
}