using System.Collections.Generic;
using Snow.Core;
using Xunit;

namespace Snow.Tests.Unit;

public class EvaluationTests
{
    [Theory]
    [InlineData("(+ 2 2)", 4.0)]
    [InlineData("(+ 2 2 3)", 7.0)]
    [InlineData("(+ 2 2 2 2 2 2 2 2 2 2)", 20.0)]
    [InlineData("(* 2 3)", 6.0)]
    [InlineData("(* 2 3 3)", 18.0)]
    [InlineData("(- 2 3)", -1.0)]
    [InlineData("(- 2 3 3)", -4.0)]
    [InlineData("(/ 8 2)", 4.0)]
    [InlineData("(/ 8 2 2)", 2.0)]
    public void GivenNumberOperatorExpression_WhenEvaluating_ReturnsRightNumber(string code, double expected)
    {
        var env = new Dictionary<string, Expression>();
        var expression = Parser.Parse(code);
        var val = AST.From(expression).Evaluate(env).GetValue<double>();

        Assert.Equal(expected, val);
    }

    [Theory]
    [InlineData("(< 2 3)", true)]
    [InlineData("(> 2 3)", false)]
    [InlineData("(<= 2 3)", true)]
    [InlineData("(>= 2 3)", false)]
    [InlineData("(< 4 3)", false)]
    [InlineData("(> 4 3)", true)]
    [InlineData("(<= 4 3)", false)]
    [InlineData("(>= 4 3)", true)]
    [InlineData("(= 3 3)", true)]
    [InlineData("(!= 3 3)", false)]
    [InlineData("(= 3 4)", false)]
    [InlineData("(!= 3 4)", true)]
    [InlineData("(= #t #t)", true)]
    [InlineData("(= #t #f)", false)]
    [InlineData("(!= #t #f)", true)]
    [InlineData("(!= #f #f)", false)]
    public void GivenEqualityOperatorExpression_WhenEvaluating_ReturnsRightBool(string code, bool expected)
    {
        var env = new Dictionary<string, Expression>();
        var expression = Parser.Parse(code);
        var val = AST.From(expression).Evaluate(env).GetValue<bool>();

        Assert.Equal(expected, val);
    }

    [Theory]
    [InlineData("#t", true)]
    [InlineData("#f", false)]
    [InlineData("6", 6d)]
    [InlineData("-10.5", -10.5)]
    [InlineData("(> 10.5 10)", true)]
    [InlineData("(!= 10.5 10)", true)]
    public void GivenDefinition_WhenEvaluating_AssignsValueCorrectly(string code, object value)
    {
        var env = new Dictionary<string, Expression>();
        var expression = Parser.Parse($"( define x {code})");
        var result = AST.From(expression).Evaluate(env);

        Assert.True(result is null);
        Assert.Equal(value, env["x"].Evaluate(env).GetValue<object>());
    }

    [Theory]
    [InlineData("(lambda () #t)", "", true)]
    [InlineData("(lambda () #f)", "", false)]
    [InlineData("(lambda (y) (= #f y))", "#t", false)]
    [InlineData("(lambda (y) (= #f y))", "#f", true)]
    public void GivenLambda_WhenEvaluatingCall_ReturnsCorrectValue(string lambda, string args, object value)
    {
        var env = new Dictionary<string, Expression>();
        var expression = Parser.Parse($"( define x {lambda} )");

        var result = AST.From(expression).Evaluate(env);
        Assert.True(result is null);

        expression = Parser.Parse($"(x {args})");
        result = AST.From(expression).Evaluate(env);
        Assert.Equal(value, result.GetValue<object>());
    }
}
