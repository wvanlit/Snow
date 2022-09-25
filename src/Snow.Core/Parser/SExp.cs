namespace Snow.Core.Parser;

public abstract record SExp;

public record SExpAtom(string Token) : SExp;

public record SExpList(List<SExp>? Contents = null) : SExp
{
    public List<SExp> Contents { get; init; } = Contents ?? new List<SExp>();
}
