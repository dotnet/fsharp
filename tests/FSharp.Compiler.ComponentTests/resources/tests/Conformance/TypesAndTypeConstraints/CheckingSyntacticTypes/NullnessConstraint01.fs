// #Regression #Conformance #TypeConstraints 
// Regression for FSB 4678

let mkInput<'d when 'd:null>() () = ()

if mkInput<string>() () <> () then exit 1

exit 0
