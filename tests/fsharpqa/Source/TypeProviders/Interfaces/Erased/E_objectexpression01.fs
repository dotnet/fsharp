// #TypeProvider #Regression
// We cannot use object expressions since we cannont inherit from provided type
// This is regression test for DevDiv:202027
//<Expects status="error" span="(6,18-6,22)" id="FS3063">Cannot inherit from erased provided type$</Expects>

let i () = { new N.I1 with
                member __.M() = [||] }
