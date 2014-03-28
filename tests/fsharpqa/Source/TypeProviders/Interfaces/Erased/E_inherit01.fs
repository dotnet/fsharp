// #TypeProvider #Regression
// It is not possible to inherit from a synthetic interface
// This is the regression test for DevDiv:202027
//<Expects status="error" span="(6,13-6,25)" id="FS3063">Cannot inherit from erased provided type$</Expects>
type I = interface
            inherit N.I1
         end

let _ = typeof<N.I1>
