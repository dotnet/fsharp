// #Regression #OCaml 
#light

// FSB 1117, Bindings that look like function definitions are interpreted as pattern matches without warning. Add a warning for this case to allow for later language design change here.


type t = F of int * int

let F(x, y) = F(1, 2)

printfn "Finished"
