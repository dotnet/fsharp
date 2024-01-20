// #Regression #Conformance #TypesAndModules #GeneratedEqualityAndHashing #Attributes 

//<Expects status="error" span="(7,5-7,22)" id="FS0501">The object constructor 'ReferenceEqualityAttribute' takes 0 argument\(s\) but is here given 1\. The required signature is 'new: unit -> ReferenceEqualityAttribute'\.$</Expects>
//<Expects status="error" span="(8,5-8,25)" id="FS0501">The object constructor 'StructuralComparisonAttribute' takes 0 argument\(s\) but is here given 1\. The required signature is 'new: unit -> StructuralComparisonAttribute'\.$</Expects>

module M03 = 
  [<ReferenceEquality(true)>]
  [<StructuralComparison(true)>]
  (* [<StructuralEquality(true)>] *)
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
