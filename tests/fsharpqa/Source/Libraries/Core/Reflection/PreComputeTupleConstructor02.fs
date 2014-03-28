// #Regression #Libraries #Reflection 
// Regression test for FSHARP1.0:5113
// MT DCR: Reflection.FSharpValue.PreComputeTupleConstructor fails when executed for NetFx 2.0 by a Dev10 compiler

let test1 = try
               Reflection.FSharpValue.PreComputeTupleConstructor(typeof<int * string>) [| box "text"; box 12; |] |> ignore
               false
            with
            | _ -> true    // yes, we expect the call above to throw since the types are swapped!


(if test1 then 0 else 1) |> exit
