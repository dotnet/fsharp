// #Regression #Libraries #Reflection 
// Regression test for FSHARP1.0:5113
// MT DCR: Reflection.FSharpValue.PreComputeTupleConstructor fails when executed for NetFx 2.0 by a Dev10 compiler

let test1 = try
               Reflection.FSharpValue.PreComputeTupleConstructor(typeof<int * string>) [| box 12; box "text" |] |> ignore
               true
            with
            | _ -> false


(if test1 then 0 else 1) |> exit
