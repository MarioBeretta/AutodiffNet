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

        Dictionary<Expression, string> _cacheExpressionString = new Dictionary<Expression, string>();

        private static void Visitor(Expression e, StringBuilder sb, Dictionary<Expression, int> parametersId)
        {
            BinaryExpression binExpr = e as BinaryExpression;
            if (binExpr != null)
            {
                sb.Append(binExpr.NodeType);
                sb.Append("(");
                Visitor(binExpr.Left, sb, parametersId);
                sb.Append(",");
                Visitor(binExpr.Right, sb, parametersId);
                sb.Append(")");
                return;
            }
            
            UnaryExpression unaryExpression = e as UnaryExpression;
            if (unaryExpression != null)
            {
                sb.Append(unaryExpression.NodeType);
                sb.Append("(");
                Visitor(unaryExpression.Operand, sb, parametersId);
                sb.Append(")");
                return;
            }
            BlockExpression blockExpression = e as BlockExpression;
            if (blockExpression != null)
            {
                sb.Append(blockExpression.NodeType);
                sb.Append("{");
                foreach (var x in blockExpression.Expressions) { sb.Append("\n"); Visitor(x, sb, parametersId); }
                sb.Append("\n}");
                return;
            }
            MethodCallExpression callExpression = e as MethodCallExpression;
            if (callExpression!=null)
            {
                sb.Append(callExpression.NodeType);
                sb.Append("(");
                Visitor(callExpression.Arguments[0], sb, parametersId);
                sb.Append(")");
                return;
            }
            ConstantExpression constantExpression = e as ConstantExpression;
            if (constantExpression!=null)
            {
                sb.Append(constantExpression.Value);
                return;
            }
            ParameterExpression parameterExpression = e as ParameterExpression;
            if (parameterExpression!=null)
            {
                if (String.IsNullOrEmpty(parameterExpression.Name))
                {
                    int id;
                    if (!parametersId.TryGetValue(e, out id))
                    {
                        id = parametersId.Count;
                        parametersId.Add(e, id);
                    }
                    sb.Append("$$Var_" + id.ToString());
                }
                else
                    sb.Append(parameterExpression.Name);
                return;
            }
            IndexExpression indexExpression = e as IndexExpression;
            if (indexExpression!=null)
            {
                Visitor(indexExpression.Object, sb, parametersId);
                sb.Append("[");
                foreach (var x in indexExpression.Arguments)
                {
                    Visitor(x, sb, parametersId);
                    sb.Append(",");
                }
                sb.Append("]");
                
                return;
            }
            NewArrayExpression newArrayExpression = e as NewArrayExpression;
            if (newArrayExpression!=null)
            {
                sb.Append(newArrayExpression.ToString());
                return;
            }

            InvocationExpression invocationExpression = e as InvocationExpression;
            if (invocationExpression!=null)
            {
                sb.Append("(");
                foreach (var x in invocationExpression.Arguments)
                {
                    Visitor(x, sb, parametersId);
                    sb.Append(",");
                }

                sb.Append(" => ");
                Visitor(invocationExpression.Expression, sb, parametersId);
            }
            return;
        }



        public string GetFullString(Expression f)
        {
            
            

            string z;
            if (_cacheExpressionString.TryGetValue(f, out z))
                return z;
            
            StringBuilder sb = new StringBuilder();
            Visitor(f, sb, new Dictionary<Expression, int>());
            /*
            (e) =>
            {
                if (e == f) return false;
                string s;
                if (_cacheExpressionString.TryGetValue(e, out s))
                {
                    if (sb.Length < 0x2000)
                    {
                        sb.Append("(");
                        sb.Append(s);
                        sb.Append(")");
                    }
                    else
                    {
                        toBig = true;
                    }
                    return true;
                }
                else
                {
                    if (sb.Length < 0x2000)
                        sb.Append(e.NodeType);
                    else
                        toBig = true;

                    return false;
                }

            });
            if (toBig) return null;
            sb.Append("(");
            sb.Append(f.ToString());
            sb.Append(z);
            
            sb.Append(")");

            */
            z = sb.ToString();
            
            _cacheExpressionString.Add(f, z);
            return z;
        }




        private void Visit(Expression e, Dictionary<string, ExpressionStatistics> dict)
        {
            string s = GetFullString(e);

            BinaryExpression binExpr = e as BinaryExpression;
            if (binExpr != null)
            {
                
                Visit(binExpr.Left, dict);
                Visit(binExpr.Right, dict);
                if (e.NodeType == ExpressionType.Assign) return;
                if (e.NodeType == ExpressionType.AddAssign) return;
            }
            else
            {
                UnaryExpression unaryExpression = e as UnaryExpression;
                if (unaryExpression != null)
                {
                    Visit(unaryExpression.Operand, dict);
                    if (e.NodeType == ExpressionType.Parameter) return;
                    if (e.NodeType == ExpressionType.Constant) return;
                }
                else
                {
                    BlockExpression blockExpression = e as BlockExpression;
                    if (blockExpression != null)
                    {
                        foreach (var x in blockExpression.Expressions) Visit(x, dict);
                    }
                    else
                    {
                        MethodCallExpression callExpression = e as MethodCallExpression;
                        if (callExpression != null)
                        {
                            Visit(callExpression.Arguments[0], dict);
                        }
                        else
                            return;
                    }
                }

            }

            if (s != null)
            {
                ExpressionStatistics v;
                if (!dict.TryGetValue(s, out v))
                {
                    v = new ExpressionStatistics() { expression = e, Name = "Var_" + dict.Count.ToString(), Usage = 1 };
                    dict.Add(s, v);
                }
                else
                    v.Usage += 1;
            }
        }

        private Expression ReplaceExpression(Expression e, Dictionary<string, ExpressionStatistics> dict)
        {
            string s = GetFullString(e);
            if (s!=null)
            {
                if (dict.ContainsKey(s))
                {
                    var v = dict[s];
                    if (v.Usage > 1)
                    {
                        return v.PreComputeVariable;
                    }
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
                var body = ReplaceExpression(unaryExpression.Operand, dict);
                return unaryExpression.Update(body);
            }
            BlockExpression blockExpression = e as BlockExpression;
            if (blockExpression != null)
            {
                var elems = blockExpression.Expressions.Select(x => ReplaceExpression(x, dict)).Where(x=>!x.IsZero());
                return blockExpression.Update(blockExpression.Variables, elems);
                
            }

            MethodCallExpression callExpression = e as MethodCallExpression;
            if (callExpression != null)
            {
                var arg = ReplaceExpression(callExpression.Arguments[0], dict);
                return callExpression.Update(callExpression.Object, new[] { arg });

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



        public Expression OptimizeExpression(Expression expr, Type resultType, ExpressionOptimizerFlags flags=ExpressionOptimizerFlags.Default)
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

                    var result = Expression.Parameter(resultType);
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