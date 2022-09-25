using Snow.Core.Constants;
using Snow.Core.Extensions;
using Snow.Core.Parser;
using static Snow.Core.Helpers.Requirements;

namespace Snow.Core.AbstractSyntaxTree;

public static class Ast
{
    public static AstExpression From(SExp exp) =>
        exp switch
        {
            SExpAtom atom => FromAtom(atom),
            SExpList list => FromList(list),
            _ => throw new ArgumentOutOfRangeException(nameof(exp), exp, null)
        };

    private static AstExpression FromAtom(SExpAtom atom)
    {
        if (atom.Token.TryParseDouble(out var num))
            return new Number(num);

        if (atom.Token == Syntax.True)
            return new Bool(true);

        if (atom.Token == Syntax.False)
            return new Bool(false);

        return new Variable(atom.Token);
    }

    private static AstExpression FromList(SExpList root)
    {
        var head = root.Contents.Pop();

        return head switch
        {
            SExpList list => AsCall(root, list),
            SExpAtom atom => atom.Token switch
            {
                string token when Syntax.Operators.Contains(token) => AsListFunction(root, token),
                Syntax.Lambda => AsLambda(root),
                Syntax.IfOperator => AsIf(root),
                Syntax.Define => AsDefine(root),
                Syntax.Cons => AsCons(root),
                _ => AsCall(root, head)
            },
            _ => throw new Exception($"No valid expression found for: {head}")
        };
    }

    private static ListFunction AsListFunction(SExpList root, string token) =>
        new(token, root.Contents.SelectToList(From));

    private static Call AsCall(SExpList root, SExp body) => new(From(body), root.Contents.SelectToList(From));

    private static Lambda AsLambda(SExpList root)
    {
        Require(root.Contents.First() is SExpList, "'lambda' statement should be followed by a list of parameters");

        var parameters = (SExpList) root.Contents.Pop();

        Require(
            parameters.Contents.All(x => x is SExpAtom),
            "'lambda' statement should be followed by a list of string parameters");

        var body = From(root.Contents.Pop());

        return new Lambda(body, parameters.Contents.SelectToList(x => (x as SExpAtom)!.Token));
    }

    private static If AsIf(SExpList root)
    {
        Require(root.Contents.Count == 3, "'if' statements should be followed by exactly 3 expressions");

        var parameters = root.Contents.SelectToList(From);

        return new If(parameters.Pop(), parameters.Pop(), parameters.Pop());
    }

    private static Define AsDefine(SExpList root)
    {
        Require(root.Contents.First() is SExpAtom, "'define' statements should be followed by identifier");

        var atom = (SExpAtom) root.Contents.Pop();

        return new Define(atom.Token, From(root.Contents.Pop()));
    }

    private static Cons AsCons(SExpList root)
    {
        Require(root.Contents.Count == 2, "'cons' statements should be followed by exactly 2 expressions");

        var parameters = root.Contents.SelectToList(From);
        return new Cons(parameters[0], parameters[1]);
    }
}
