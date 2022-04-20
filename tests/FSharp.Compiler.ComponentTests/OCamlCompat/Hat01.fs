// #OCaml 
// Compile with --mlcompatibility --warnaserror ==> should compile clean and run clean
//<Expects status="success"></Expects>
let r = "a" ^ "b"
(if r = "a" + "b" then 0 else 1) |> exit
