// #Regression #Conformance #TypeInference #Recursion 
// Regression test for FSharp1.0:3187
// Title: better inference for mutually recursrive generic classes
// Descr: Verify types are inferred correctly for generic classes

//<Expects status="error" id="FS0001" span="(15,14-15,20)">This expression was expected to have type</Expects>

// In the negative case, this tests that eager generalization is based on a transitive (greatest-fixed-point) computation
module OO_Example_GreatestFixedPoint = 
    type C() = 
        member x.M1() = x.M2() |> fst
        member x.M2() = failwith "", x.M3()
        member x.Mbbb() = 
            (x.M1() : int) |> ignore 
            (x.M2() : string) |> ignore   // M1 should not be generalized at this point
        member x.M3() = 1
