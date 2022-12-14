using System.Linq.Expressions;
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
            Cons c => EvalCons(c, env),
            Car car => EvalCar(car, env),
            Cdr cdr => EvalCdr(cdr, env),
            _ => throw new ArgumentOutOfRangeException(
                nameof(expression),
                expression,
                $"{nameof(AstEvaluationVisitor)} does not support {expression.GetType()}"),
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

        try
        {
            return values.First() switch
            {
                Number => ListOperations.NumberOperations[listFunction.Operator].EvalList(values),
                Bool => ListOperations.BooleanOperations[listFunction.Operator].EvalList(values),
                _ => throw new ArgumentOutOfRangeException(nameof(values), values.First(), null)
            };
        }
        catch (KeyNotFoundException _)
        {
            var type = values.First().GetType().ToString().Split(".").Last();

            throw new Exception($"Operator '{listFunction.Operator}' not found for {type} type");
        }
    }

    private static Value EvalCons(Cons cons, Environment env) =>
        new Pair(Eval(cons.Left, env)!, Eval(cons.Right, env)!);

    private static Value EvalCar(Car car, Environment env)
    {
        if (Eval(car.Expression, env)! is not Pair pair)
            throw new Exception("TODO");

        return pair.Left;
    }

    private static Value EvalCdr(Cdr cdr, Environment env)
    {
        if (Eval(cdr.Expression, env)! is not Pair pair)
            throw new Exception("TODO");

        return pair.Right;
    }
}
