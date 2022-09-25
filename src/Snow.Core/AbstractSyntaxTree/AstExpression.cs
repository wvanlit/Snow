namespace Snow.Core.AbstractSyntaxTree;

public abstract record AstExpression;

public record Call(AstExpression Body, List<AstExpression> Arguments) : AstExpression;

public record Define(string Name, AstExpression AstExpression) : AstExpression;

public record If(AstExpression Test, AstExpression TrueBranch, AstExpression ElseBranch) : AstExpression;

public record Lambda(AstExpression Body, List<string> Parameters) : AstExpression;

public record ListFunction(string Operator, List<AstExpression> Arguments) : AstExpression;

public record Variable(string Name) : AstExpression;

public record Cons(AstExpression Left, AstExpression Right) : AstExpression;

public record Car(AstExpression Expression) : AstExpression;

public record Cdr(AstExpression Expression) : AstExpression;
