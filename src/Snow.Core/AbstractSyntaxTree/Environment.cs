namespace Snow.Core.AbstractSyntaxTree;

public class Environment : Dictionary<string, AstExpression>
{
    public Environment() : base()
    {
    }

    public Environment(Environment env) : base(env)
    {
    }
}
