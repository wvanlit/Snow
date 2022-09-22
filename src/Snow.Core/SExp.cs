namespace Snow.Core;

public abstract class SExp
{

}

public class SExpAtom : SExp
{
    public SExpAtom(string token)
    {
        Token = token;
    }

    public string Token { get; }
}

public class SExpList : SExp
{
    public List<SExp> Contents { get; } = new();
}
