// #Regression #Conformance #TypesAndModules #GeneratedEqualityAndHashing #Attributes 
//<Expects status="error" span="(13,17-13,19)" id="FS0001">The type 'R' does not support the 'equality' constraint because it has the 'NoEquality' attribute$</Expects>
//<Expects status="error" span="(14,12-14,14)" id="FS0001">The type 'R' does not support the 'equality' constraint because it has the 'NoEquality' attribute$</Expects>

module M08 = 
  [<ReferenceEquality>]
  (* [<StructuralComparison(true)>] *)
  [<NoEquality>]
  type R  = { X : int }
  let r1  = { X = 10}
  let r2  = { X = 11}
  let r2b = { X = 11}
  let v1 = not (r1 = r2)
  let v2 = r2 = r2b
  // let v3 = r1 < r2
  printfn "v1=%b" v1
  printfn "v2=%b" v2
  //printfn "v3=%b" v3
  if v1 && v2 (*&& v3*) then failwith "Failed: 1"
