// #Regression #Conformance #TypesAndModules #GeneratedEqualityAndHashing #Attributes 
//<Expects status="error" span="(15,13-15,15)" id="FS0001">The type 'R' does not support the 'comparison' constraint because it has the 'NoComparison' attribute$</Expects>

module M22 = 
  (* [<ReferenceEquality(true)>] *)
  [<NoComparison>]
  [<StructuralEquality>]
  type R  = { X : int }
  let r1  = { X = 10}
  let r2  = { X = 11}
  let r2b = { X = 11}
  let v1 = not (r1 = r2)  // expected true
  let v2 = r2 = r2b       // expected true
  let v3 = try
            r1 < r2 |> ignore       // expected true
            false
           with
            | _ -> true
  printfn "v1=%b" v1
  printfn "v2=%b" v2
  printfn "v3=%b" v3
  if not (v1 && v2 && v3) then failwith "Failed: 1"
