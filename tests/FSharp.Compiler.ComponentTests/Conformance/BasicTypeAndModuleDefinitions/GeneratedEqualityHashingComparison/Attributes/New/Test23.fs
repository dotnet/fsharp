// #Regression #Conformance #TypesAndModules #GeneratedEqualityAndHashing #Attributes 
//<Expects status="error" span="(14,17-14,19)" id="FS0001">The type 'R' does not support the 'equality' constraint because it has the 'NoEquality' attribute$</Expects>
//<Expects status="error" span="(15,17-15,19)" id="FS0001">The type 'R' does not support the 'equality' constraint because it has the 'NoEquality' attribute$</Expects>
//<Expects status="error" span="(17,13-17,15)" id="FS0001">The type 'R' does not support the 'comparison' constraint because it has the 'NoComparison' attribute$</Expects>

module M23 = 
  (* [<ReferenceEquality(true)>] *)
  [<NoComparison>]
  [<NoEquality>]
  type R  = { X : int }
  let r1  = { X = 10}
  let r2  = { X = 11}
  let r2b = { X = 11}
  let v1 = not (r1 = r2)   // expected true
  let v2 = not (r2 = r2b)  // expected true
  let v3 = try
            r1 < r2 |> ignore       // expected true
            false
           with
            | _ -> true
  printfn "v1=%b" v1
  printfn "v2=%b" v2
  printfn "v3=%b" v3
  (if v1 && v2 && v3 then 0 else 1) |> exit
