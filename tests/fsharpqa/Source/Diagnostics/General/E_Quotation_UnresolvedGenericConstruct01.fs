// #Regression #Diagnostics 
// Regression test for FSHARP1.0:1278
// Unresolved generic constructs in quotations should error instead of warn.
//<Expects id="FS0331" span="(14,13-14,20)" status="error">The implicit instantiation of a generic construct at or near this point could not be resolved because it could resolve to multiple unrelated types, e\.g\. 'IB' and 'IA'\. Consider using type annotations to resolve the ambiguity</Expects>
//<Expects id="FS0071" span="(14,13-14,20)" status="error">Type constraint mismatch when applying the default type 'IA' for a type inference variable\. The type 'IA' is not compatible with the type 'IB' Consider adding further type constraints</Expects>


type IA = abstract member Foo : unit -> unit
type IB = abstract member Foo : unit -> unit
type T() = 
  member x.Foo(v:'t when 't :> IA and 't :> IB) : unit = 
    failwith "!"

let x = <@@ T().Foo @@> 
