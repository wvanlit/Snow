# Snow

A simple Scheme inspired Lisp dialect written in C#

```scheme
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

## Features ([ref](https://groups.csail.mit.edu/mac/ftpdir/scheme-7.4/doc-html/scheme_toc.html))

- [ ] Values
  - [x] Boolean `(|| #t #f)`
  - [x] Number `(* 1 1.4 -2.1)`
  - [x] Closure
  - [x] Pairs `(cons 1 2) => (1 . 2)`
  - [ ] Lists `(list 1 2 3) => (1 . (2 . 3))`
  - [ ] Strings
  - [ ] Vectors
- [ ] Special Forms
  - [x] Lambda
  - [x] List Function `(- 1 2 3)`
  - [x] Definitions
  - [x] Conditionals
  - [ ] Pair functions `(car (cons 1 2)) => 1` and `(cdr (cons 1 2)) => 2`
  - [ ] Quote
  - [ ] Sequencing
  - [ ] Iteration
- [ ] Ports
  - [ ] File Ports
  - [ ] String Ports
  - [ ] Input Ports
  - [ ] Output Ports
- [ ] Error system
  - [ ] Error/Warn
  - [ ] Restarts
- [ ] Macro's
