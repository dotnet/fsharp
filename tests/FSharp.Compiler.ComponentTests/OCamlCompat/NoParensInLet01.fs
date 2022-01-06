// #Regression #OCaml 
#light

// FSHARP1.0:1117, Bindings that look like function definitions are interpreted as pattern matches without warning. Add a warning for this case to allow for later language design change here.
// FSHARP1.0:2552, name scoping bug

//<Expects status="success"></Expects>

type t = F of int * int

let F(x, y) = F(1, 2)       // this is now a function definition
                            // > F(10,20);;
                            // val it : t = F (1,2)

let (F(a, b)) = F(1, 2)     // this is a pattern match
                            // > a;; b;;
                            // val it : int = 1
                            // val it : int = 2         
                               
(if F(10,20) = F(1,2) && a=1 && b=2 then 0 else 1) |>ignore
()
