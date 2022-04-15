// #Regression #Conformance #TypesAndModules #GeneratedEqualityAndHashing #Attributes 
//<Expects status="error" span="(14,17-14,19)" id="FS0001">The type 'R' does not support the 'equality' constraint because it has the 'NoEquality' attribute$</Expects>
//<Expects status="error" span="(15,12-15,14)" id="FS0001">The type 'R' does not support the 'equality' constraint because it has the 'NoEquality' attribute$</Expects>
//<Expects status="error" span="(16,12-16,14)" id="FS0001">The type 'R' does not support the 'comparison' constraint\. For example, it does not support the 'System\.IComparable' interface$</Expects>

module M26 = 
  (* [<ReferenceEquality(true)>] *)
  (* [<StructuralComparison(true)>] *)
  [<NoEquality>]
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
