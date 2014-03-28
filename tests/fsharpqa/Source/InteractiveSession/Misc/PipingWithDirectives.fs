// #Regression #NoMT #FSI 
#light

// Regression test for FSHARP1.0:1564 - Parsing difference in FSI between redirection and loading with #directives

let failures = ref false
let report_failure () = 
  printfn " NO"; failures.contents <- true
let test s b = printfn s;  if b then printfn " OK" else report_failure()

#nowarn "0026"
#nowarn "0025"
#time
#types
#r "System.Windows.Forms.dll"

let test2 x = 
  match x with
  | 1 -> true

let test3 x =
  match x with
  | x -> Some(x)
  | _ -> None

#quit
