// #Regression #Conformance #TypesAndModules #GeneratedEqualityAndHashing #Attributes 
//<Expects status="error" span="(8,8-8,9)" id="FS0385">A type with attribute 'CustomComparison' must have an explicit implementation of at least one of 'System\.IComparable' or 'System\.Collections\.IStructuralComparable'$</Expects>

module M13 = 
  //[<ReferenceEquality(false)>]
  [<CustomComparison>]
  [<StructuralEquality>]
  type R  = { X : int }
  let r1  = { X = 10}
  let r2  = { X = 11}
  let r2b = { X = 11}
  let v1 = not (r1 = r2)  // expected true
  let v2 = r2 = r2b       // expected true
//  let v3 = r1 < r2        // expected true
  printfn "v1=%b" v1
  printfn "v2=%b" v2
//  printfn "v3=%b" v3
//  (if v1 && v2 && v3 then 0 else 1) |> exit

