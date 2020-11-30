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

## What's new 0.7.4

 - Performance improvents
 - *Term.Sum* to add multiple terms
 - Tree based evaluation to cover cases where the compiled expression is to large
 - 

