using Snow.Core.Extensions;

namespace Snow.Core.Parser;

public static class Parser
{
    public static SExp Parse(string input)
    {
        var tokens = input
            .Replace("(", " ( ")
            .Replace(")", " ) ")
            .Split()
            .Where(s => s != string.Empty)
            .ToList();

        return Next(tokens);
    }

    private static SExp Next(List<string> tokens)
    {
        var token = tokens.Pop();
        switch (token)
        {
            case ")":
                throw new Exception("Unexpected ')' character");
            case "(":
                var list = new SExpList();
                while (tokens.First() != ")")
                {
                    list.Contents.Add(Next(tokens));
                }

                tokens.Pop();

                return list;
            default:
                return new SExpAtom(token);
        }
    }
}
