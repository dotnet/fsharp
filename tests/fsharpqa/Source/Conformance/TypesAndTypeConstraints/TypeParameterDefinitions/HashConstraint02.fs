// #Regression #Conformance #TypeConstraints 
// Regression test for FSHARP1.0:1419
// Tokens beginning with # should not match greedily with directives
// The only case where we are still a bit confused is #light: for this reason the code
// below compiles just fine (it would not work if I replace #light with #r for example)
#light

type light_() = class
                end

let t = new light_()

let t5 (x : #light_) = x

let r1 = t5 1.0
let r2 = t5 t

exit 0
