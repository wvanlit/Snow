using System.Globalization;
using Snow.Core.Extensions;

namespace Snow.Core;

public static class AST
{
    public static Expression From(SExp exp) =>
        exp switch
        {
            SExpAtom atom => From(atom),
            SExpList list => From(list),
        };

    private static Expression From(SExpAtom atom)
    {
        if (double.TryParse(atom.Token, NumberStyles.Any, CultureInfo.InvariantCulture, out var num))
            return new Number(num);

        if (atom.Token == "#t")
            return new Bool(true);

        if (atom.Token == "#f")
            return new Bool(false);

        return new Variable(atom.Token);
    }

    private static Expression From(SExpList root)
    {
        var head = root.Contents.PopAt(0);

        return head switch
        {
            SExpList list => new Call(From(list), root.Contents.Select(From).ToList()),
            SExpAtom atom => atom.Token switch
            {
                "+" or "-" or "/" or "*" or "=" or "!=" or "&&" or "||" or "<" or "<=" or ">" or ">=" =>
                    new ListFunction(atom.Token, root.Contents.Select(From).ToList()),
                "lambda" => ToLambda(root),
                "if" => ToIf(root),
                "define" => ToDefine(root),
                _ => new Call(From(head), root.Contents.Select(From).ToList())
            },
            _ => throw new Exception($"No valid expression found for: {head}")
        };
    }

    private static Lambda ToLambda(SExpList root)
    {
        Require(root.Contents.First() is SExpList, "\'lambda\' statement should be followed by a list of parameters");

        var parameters = (SExpList) root.Contents.PopAt(0);

        Require(
            parameters.Contents.All(x => x is SExpAtom),
            "\'lambda\' statement should be followed by a list of string parameters");

        var body = From(root.Contents.PopAt(0));

        return new Lambda(body, parameters.Contents.Select(x => (x as SExpAtom)!.Token).ToList());
    }

    private static If ToIf(SExpList root)
    {
        Require(root.Contents.Count == 3, "\'if\' statements should be followed by 3 expressions");
        var parameters = root.Contents.Select(From).ToList();

        return new If(parameters.PopAt(0), parameters.PopAt(0), parameters.PopAt(0));
    }

    private static Define ToDefine(SExpList root)
    {
        Require(root.Contents.First() is SExpAtom, "\'define\' statements should be followed by identifier");
        var atom = (SExpAtom) root.Contents.PopAt(0);

        return new Define(atom.Token, From(root.Contents.PopAt(0)));
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new Exception(message);
    }
}
