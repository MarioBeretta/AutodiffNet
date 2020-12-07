# Project Description

This library provides an implementation for automatic differentiation of mathematical functions.

# Usage example

```c#
using AutoDiffNet;
using System;

namespace AutoDiffNetConsole
{

    class Program
    {


        static void Main(string[] args)
        {
            Variable x = new Variable();
            Term f;
            f = Term.Ln(x[0]+2*x[1]);

            Func<double[], double> fx = f.Compile();
            Func<double[], double> grad = f.Grad(0);

            var x0 = new double[] { 1, 2 };
            System.Console.WriteLine(f.GradString(0));
            System.Console.WriteLine(grad(x0));

            System.Console.WriteLine(fx(x0));
            
            System.Console.ReadLine();
        }
    }
}
```

# Expression Optimizer
As part of the release 1.0.0 I'm including a new feature: the Expression Optimizer that as the name suggest it's providing the capabilities of optimize the generated Lambda (for both the Function and his gradient).

The Optimizer have several feature and they can be enabled or disabled using the flag: ExpressionOptimizerFlags

```C#
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
```

|Flags   | Description |Default Behavior |
|--------|---------------|----|
| MultiplyByZero | Any sub-expression in the form 0 * g(x) will be converted in 0 and g(x) will not be evaluated | Enabled |
| ReuseOfDuplicatedExpression | The optimizer will scan the Function f(x) and will evalute every sub-expression just 1 times; es: f(x) = g(x)*2+ Log(g(x)) then with this option g(x) will be evaluated only once and the result will be cached | Enabled |
| ZeroDivideBy | Any sub-expression in the form 0 / g(x)  will be conveted in 0 and g(x) will not be evaluated; condition where g(x) is zero need to be validated externally | Disabled |

## What's new 1.2.0
 - Adding Gradient Function, that evaluate all the Gradient vector at once

## What's new 1.0.0
 - New Expression optimizer feature (enabled by default)
 - Some Unit Tests
 

## What's new 0.7.4

 - Performance improvents
 - *Term.Sum* to add multiple terms
 - Tree based evaluation to cover cases where the compiled expression is to large


