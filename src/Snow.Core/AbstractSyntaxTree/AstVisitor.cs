using Snow.Core.Extensions;

namespace Snow.Core.AbstractSyntaxTree;

public static class AstEvaluationVisitor
{
    public static Value? Eval(AstExpression expression, Environment env) =>
        expression switch
        {
            Value v => v,
            Variable v => EvalVariable(v, env),
            Define def => EvalDefine(def, env),
            If i => EvalIf(i, env),
            Call c => EvalCall(c, env),
            Lambda l => EvalLambda(l, env),
            ListFunction l => EvalListFunction(l, env),
            _ => throw new ArgumentOutOfRangeException(nameof(expression), expression, null)
        };

    private static Closure EvalLambda(Lambda l, Environment env) => new(l, env);

    private static Value EvalVariable(Variable v, Environment env) => Eval(env[v.Name], env)!;

    private static Value EvalIf(If i, Environment env)
    {
        var isTestTrue = Eval(i.Test, env)!.GetValue<bool>();
        var branch = isTestTrue ? i.TrueBranch : i.ElseBranch;

        return Eval(branch, env)!;
    }

    private static Value? EvalDefine(Define define, Environment env)
    {
        env[define.Name] = define.AstExpression;

        return null;
    }

    private static Value EvalCall(Call call, Environment env)
    {
        var closure = Eval(call.Body, env)!.GetValue<Closure>();
        var argumentValues = call.Arguments.SelectToList(a => Eval(a, env)!);

        foreach (var (parameter, value) in closure.Lambda.Parameters.Zip(argumentValues))
            closure.Env[parameter] = value;

        return Eval(closure.Lambda.Body, closure.Env)!;
    }

    private static Value EvalListFunction(ListFunction listFunction, Environment env)
    {
        var values = listFunction.Arguments.SelectToList(arg => Eval(arg, env)!);

        return values.First() switch
        {
            Number => ListOperations.NumberOperations[listFunction.Operator].EvalList(values),
            Bool => ListOperations.BooleanOperations[listFunction.Operator].EvalList(values),
            _ => throw new ArgumentOutOfRangeException(nameof(values), values.First(), null)
        };
    }
}
