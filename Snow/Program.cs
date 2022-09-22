using Snow.Core;
using Spectre.Console;

var env = new Dictionary<string, Expression>();

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

Value Eval(string code) => AST.From(Parser.Parse(code)).Evaluate(env);
