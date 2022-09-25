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
  - [ ] Strings
  - [ ] Lists
  - [ ] Vectors
- [ ] Special Forms
  - [x] Lambda
  - [x] List Function `(- 1 2 3)`
  - [x] Definitions
  - [x] Conditionals
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
