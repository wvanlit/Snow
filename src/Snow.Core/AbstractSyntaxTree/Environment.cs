namespace Snow.Core.AbstractSyntaxTree;

public class Environment : Dictionary<string, AstExpression>
{
    public Environment() : base()
    {
    }

    public Environment(Environment env) : base(env)
    {
    }

    public new AstExpression this[string key]
    {
        get
        {
            try
            {
                return base[key];
            }
            catch (KeyNotFoundException keyNotFound)
            {
                throw new Exception($"'{key}' is not defined current scope");
            }
        }
        set => base[key] = value;
    }
}
