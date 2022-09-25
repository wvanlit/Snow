namespace Snow.Core.Constants;

public static class Syntax
{
    public const string Plus = "+";
    public const string Minus = "-";
    public const string Divide = "/";
    public const string Multiply = "*";
    public const string EqualsTo = "=";
    public const string NotEqualsTo = "!=";
    public const string And = "&&";
    public const string Or = "||";
    public const string GreaterThan = ">";
    public const string GreaterThanOrEqual = ">=";
    public const string LesserThan = "<";
    public const string LesserThanOrEqual = "<=";

    public const string Lambda = "lambda";
    public const string IfOperator = "if";
    public const string Define = "define";

    public const string Cons = "cons";
    public const string Car = "car";
    public const string Cdr = "cdr";

    public const string True = "#t";
    public const string False = "#f";

    public static readonly string[] Operators =
    {
        Plus, Minus, Divide, Multiply,
        And, Or,
        EqualsTo, NotEqualsTo,
        GreaterThan, GreaterThanOrEqual,
        LesserThan, LesserThanOrEqual
    };
}
