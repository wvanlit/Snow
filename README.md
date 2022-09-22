# Snow

A simple lisp dialect written in C#

```lisp
(define fib (lambda (n) 
     (if (= n 0)
         0
         (if (< n 2) 
             1 
         (+ (fib (- n 1)) (fib (- n 2)))))
     )
 )

> (fib 25)
75025
```
