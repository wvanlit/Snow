namespace Snow.Core.AbstractSyntaxTree;

public abstract record AstExpression;

public record Value : AstExpression
{
    protected object? Underlying { get; init; }

    public Value(object? underlying)
    {
        Underlying = underlying;
    }

    protected Value()
    {
    }

    public T GetValue<T>() => (T) Underlying!;

    public Value TryConvert() =>
        Underlying switch
        {
            bool b => new Bool(b),
            double d => new Number(d),
            _ => this
        };
}

public record Bool(bool B) : Value(B);

public record Number(double D) : Value(D);

public record Closure : Value
{
    public Lambda Lambda { get; }

    public Environment Env { get; }

    public Closure(Lambda lambda, Environment env)
    {
        Lambda = lambda;
        Env = new Environment(env);
        Underlying = this;
    }
}

public record Call(AstExpression Body, List<AstExpression> Arguments) : AstExpression;

public record Define(string Name, AstExpression AstExpression) : AstExpression;

public record If(AstExpression Test, AstExpression TrueBranch, AstExpression ElseBranch) : AstExpression;

public record Lambda(AstExpression Body, List<string> Parameters) : AstExpression;

public record ListFunction(string Operator, List<AstExpression> Arguments) : AstExpression;

public record Variable(string Name) : AstExpression;
