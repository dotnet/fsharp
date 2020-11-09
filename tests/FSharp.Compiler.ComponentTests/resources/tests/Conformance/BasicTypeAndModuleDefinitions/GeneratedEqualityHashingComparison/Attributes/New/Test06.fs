// #Regression #Conformance #TypesAndModules #GeneratedEqualityAndHashing #Attributes 
//<Expects status="error" span="(9,8-9,9)" id="FS0377">This type uses an invalid mix of the attributes 'NoEquality', 'ReferenceEquality', 'StructuralEquality', 'NoComparison' and 'StructuralComparison'$</Expects>
//<Expects status="error" span="(9,8-9,9)" id="FS0385">A type with attribute 'CustomComparison' must have an explicit implementation of at least one of 'System\.IComparable' or 'System\.Collections\.IStructuralComparable'$</Expects>

module M06 = 
  [<ReferenceEquality>]
  [<CustomComparisonAttribute>]
  (* [<StructuralEquality(true)>] *)
  type R  = { X : int }
  let r1  = { X = 10}
  let r2  = { X = 11}
  let r2b = { X = 11}
  let v1 = not (r1 = r2)
  let v2 = r2 = r2b
  //let v3 = false // r1 < r2
  printfn "v1=%b" v1
  printfn "v2=%b" v2
  //printfn "v3=%b" v3
  (if v1 && v2 (*&& v3 *)then 0 else 1) |> exit
