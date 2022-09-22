using Snow.Core.Extensions;

namespace Snow.Core;

public abstract class Expression
{
    public abstract Value Evaluate(Dictionary<string, Expression> env);
}

public abstract class Value : Expression
{
    protected object? Underlying { get; init; }

    public Value(object? underlying)
    {
        Underlying = underlying;
    }

    protected Value(){}

    public T GetValue<T>() => (T) Underlying!;

    public override Value Evaluate(Dictionary<string, Expression> env) => this;
}

public class Bool : Value
{
    public Bool(bool b) : base(b)
    {
    }
}

public class Number : Value
{
    public Number(double x) : base(x)
    {
    }
}

public class Call : Expression
{
    public Expression Body { get; }

    public List<Expression> Arguments { get; }

    public Call(Expression body, List<Expression> arguments)
    {
        Body = body;
        Arguments = arguments;
    }

    public override Value Evaluate(Dictionary<string, Expression> env)
    {
        var closure = Body.Evaluate(env).GetValue<Closure>();
        var runtime = Arguments.Select(x => x.Evaluate(env));

        foreach (var pair in closure.Lambda.Parameters.Zip(runtime, (p, arg) => new {p, arg}))
        {
            closure.Env[pair.p] = pair.arg;
        }

        return closure.Lambda.Body.Evaluate(closure.Env);
    }
}

public class Define : Expression
{
    public string Name { get; }

    public Expression Expression { get; }

    public Define(string name, Expression expression)
    {
        Name = name;
        Expression = expression;
    }

    public override Value Evaluate(Dictionary<string, Expression> env)
    {
        env[Name] = Expression;

        return null;
    }
}

public class If : Expression
{
    public Expression Test { get; }

    public Expression TrueBranch { get; }

    public Expression ElseBranch { get; }

    public If(Expression test, Expression trueBranch, Expression elseBranch)
    {
        Test = test;
        TrueBranch = trueBranch;
        ElseBranch = elseBranch;
    }

    public override Value Evaluate(Dictionary<string, Expression> env) =>
        Test.Evaluate(env).GetValue<bool>()
            ? TrueBranch.Evaluate(env)
            : ElseBranch.Evaluate(env);
}

public class Lambda : Expression
{
    public Expression Body { get; }

    public List<string> Parameters { get; }

    public Lambda(Expression body, List<string> parameters)
    {
        Body = body;
        Parameters = parameters;
    }

    public override Value Evaluate(Dictionary<string, Expression> env) => new Closure(this, env);
}

public class ListFunction : Expression
{
    public string Operator { get; }

    public List<Expression> Arguments { get; }

    public ListFunction(string op, List<Expression> arguments)
    {
        Operator = op;
        Arguments = arguments;
    }

    public override Value Evaluate(Dictionary<string, Expression> env) => Operator switch
    {
        "+" => ReduceNumberListToNumber(env, Arguments, (acc, x) => acc + x),
        "-" => ReduceNumberListToNumber(env, Arguments, (acc, x) => acc - x),
        "/" => ReduceNumberListToNumber(env, Arguments, (acc, x) => acc / x),
        "*" => ReduceNumberListToNumber(env, Arguments, (acc, x) => acc * x),
        "<" => ReduceNumberListToBool(env, Arguments, (x, y) => x < y),
        ">" => ReduceNumberListToBool(env, Arguments, (x, y) => x > y),
        "<=" => ReduceNumberListToBool(env, Arguments, (x, y) => x <= y),
        ">=" => ReduceNumberListToBool(env, Arguments, (x, y) => x >= y),
        "=" => Arguments[0] is Bool
            ? ReduceBoolListToBool(env, Arguments, (x, y) => x == y)
            : ReduceNumberListToBool(env, Arguments, (x, y) => Math.Abs(x - y) < 0.0001),
        "!=" => Arguments[0] is Bool
            ? ReduceBoolListToBool(env, Arguments, (x, y) => x != y)
            : ReduceNumberListToBool(env, Arguments, (x, y) => Math.Abs(x - y) > 0.0001),
        _ => throw new Exception($"Operator {Operator} not implemented")
    };

    private static Number ReduceNumberListToNumber(Dictionary<string, Expression> env, List<Expression> arguments,
        Func<double, double, double> reducer) =>
        new Number(arguments.Select(x => x.Evaluate(env).GetValue<double>()).Aggregate(reducer));

    private static Bool ReduceNumberListToBool(Dictionary<string, Expression> env, List<Expression> arguments,
        Func<double, double, bool> check)
    {
        var args = arguments.Select(x => x.Evaluate(env).GetValue<double>()).ToList();

        return new Bool(args.SlidingWindow(2).All(window => check(window[0], window[1])));
    }

    private static Bool ReduceBoolListToBool(Dictionary<string, Expression> env, List<Expression> arguments,
        Func<bool, bool, bool> check)
    {
        var args = arguments.Select(x => x.Evaluate(env).GetValue<bool>()).ToList();

        return new Bool(args.SlidingWindow(2).All(window => check(window[0], window[1])));
    }
}

public class Variable : Expression
{
    public string Name { get; }

    public Variable(string name)
    {
        Name = name;
    }

    public override Value Evaluate(Dictionary<string, Expression> env) => env[Name].Evaluate(env);
}

public class Closure : Value
{
    public Lambda Lambda { get; }

    public Dictionary<string, Expression> Env { get; }

    public Closure(Lambda lambda, Dictionary<string, Expression> env)
    {
        Lambda = lambda;
        Env = new Dictionary<string, Expression>(env);

        Underlying = this;
    }
}
