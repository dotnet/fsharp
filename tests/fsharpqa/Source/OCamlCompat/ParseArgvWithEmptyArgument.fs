// #Regression #OCaml #RequiresPowerPack 
#light

// Regression test for FSharp1.0:3803 - argv processing assumes arguments are non-empty strings

let mutable res = ""
let collect s = res <- res ^ "[" ^ s ^ "]"

do  Microsoft.FSharp.Compatibility.OCaml.Arg.parse_argv (ref 0) [| "foo.exe"; ""; "foo"; ""; "bar" |] [] collect "Use this in a right way"

if res <> "[][foo][][bar]" then exit 1

exit 0
