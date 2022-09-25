using System.Collections.Generic;
using Snow.Core;
using Snow.Core.AbstractSyntaxTree;
using Snow.Core.Parser;
using Xunit;

namespace Snow.Tests.Unit;

public class ProgramTests
{
    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(2, 1)]
    [InlineData(3, 2)]
    [InlineData(4, 3)]
    [InlineData(5, 5)]
    [InlineData(6, 8)]
    [InlineData(7, 13)]
    [InlineData(8, 21)]
    [InlineData(9, 34)]
    [InlineData(10, 55)]
    public void GivenAFibonacciSequenceLambda_WhenEvaluating_ReturnsCorrectAnswer(double input, double expected)
    {
        var env = new Environment();
        _ = Eval(
            @"
(define fib (lambda (n) 
    (if (= n 0)
        0
        (if (< n 2) 
            1 
        (+ (fib (- n 1)) (fib (- n 2)))))
    )
)",
            env);

        var actual = Eval($"(fib {input})", env).GetValue<object>();

        Assert.Equal(expected, actual);
    }

    private static Value Eval(string code, Environment env) =>
        AstEvaluationVisitor.Eval(Ast.From(Parser.Parse(code)), env)!;
}
