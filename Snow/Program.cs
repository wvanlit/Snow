using Snow.Core;
using Snow.Core.AbstractSyntaxTree;
using Snow.Core.Parser;
using Spectre.Console;
using Environment = Snow.Core.AbstractSyntaxTree.Environment;

var env = new Environment();

_ = Eval(
    @"
(define fib (lambda (n) 
    (if (= n 0)
        0
        (if (< n 2) 
            1 
        (+ (fib (- n 1)) (fib (- n 2)))))
    )
)"
);

AnsiConsole.WriteLine($"> {Eval("(fib 25)").GetValue<object>()}");

Value Eval(string code) => AstEvaluationVisitor.Eval(Ast.From(Parser.Parse(code)), env)!;
