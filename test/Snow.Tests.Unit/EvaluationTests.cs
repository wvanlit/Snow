using System.Collections.Generic;
using Snow.Core;
using Snow.Core.AbstractSyntaxTree;
using Snow.Core.Parser;
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
        var env = new Environment();
        var expression = Parser.Parse(code);
        var val = AstEvaluationVisitor.Eval(Ast.From(expression), env).GetValue<double>();

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
        var val = ParseAndEvaluate(code, new Environment()).GetValue<bool>();
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
        var env = new Environment();
        var result = ParseAndEvaluate($"(define x {code})", env);

        Assert.True(result is null);

        result = AstEvaluationVisitor.Eval(env["x"], env)!;

        Assert.Equal(value, result.GetValue<object>());
    }

    [Theory]
    [InlineData("(lambda () #t)", "", true)]
    [InlineData("(lambda () #f)", "", false)]
    [InlineData("(lambda (y) (= #f y))", "#t", false)]
    [InlineData("(lambda (y) (= #f y))", "#f", true)]
    public void GivenLambda_WhenEvaluatingCall_ReturnsCorrectValue(string lambda, string args, object value)
    {
        var env = new Environment();

        var result = ParseAndEvaluate($"( define x {lambda})", env);
        Assert.True(result is null);

        result = ParseAndEvaluate($"(x {args})", env);
        Assert.Equal(value, result.GetValue<object>());
    }

    [Theory]
    [InlineData("(define λ 2)", "λ", 2.0)]
    [InlineData("(define μα #f)", "μα", false)]
    [InlineData("(define 💀 #t)", "💀", true)]
    public void GivenUnicode_WhenEvaluating_EvaluatesCorrectly(string define, string eval, object expected)
    {
        var env = new Environment();
        _ = ParseAndEvaluate(define, env);
        var result = ParseAndEvaluate(eval, env);

        Assert.Equal(expected, result.GetValue<object>());
    }

    private static Value ParseAndEvaluate(string code, Environment env) =>
        AstEvaluationVisitor.Eval(Ast.From(Parser.Parse(code)), env)!;
}
