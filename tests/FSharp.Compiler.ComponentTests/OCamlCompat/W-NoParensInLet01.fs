// #Regression #OCaml 
#light

// FSB 1117, Bindings that look like function definitions are interpreted as pattern matches without warning. Add a warning for this case to allow for later language design change here.
//<Expects id="FS0191" status="warning">This construct is for compatibility with OCaml</Expects>

type t = F of int * int

let F(x, y) = F(1, 2)

exit 0
