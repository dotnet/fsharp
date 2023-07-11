// #Regression #Conformance #TypesAndModules #GeneratedEqualityAndHashing #Attributes 
//<Expects status="error" span="(14,12-14,14)" id="FS0001">The type 'R' does not support the 'comparison' constraint\. For example, it does not support the 'System\.IComparable' interface$</Expects>

module M25 = 
  (* [<ReferenceEquality(true)>] *)
  (* [<StructuralComparison(true)>] *)
  [<StructuralEquality>]
  type R  = { X : int }
  let r1  = { X = 10}
  let r2  = { X = 11}
  let r2b = { X = 11}
  let v1 = not (r1 = r2)  // expected true
  let v2 = r2 = r2b       // expected true
  let v3 = r1 < r2        // expected true
  printfn "v1=%b" v1
  printfn "v2=%b" v2
  printfn "v3=%b" v3
  if not (v1 && v2 && v3) then failwith "Failed: 1"
