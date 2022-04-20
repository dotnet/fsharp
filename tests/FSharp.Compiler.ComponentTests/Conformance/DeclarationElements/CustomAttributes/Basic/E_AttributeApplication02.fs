// #Regression #Conformance #DeclarationElements #Attributes 
// Regression test for FSHARP1.0:1475
// F# attributes in prim-types.fs do not specify attribute targets
//<Expects id="FS0429" span="(6,15-6,22)" status="error">The attribute type 'MeasureAttribute' has 'AllowMultiple=false'\. Multiple instances of this attribute cannot be attached to a single language element\.$</Expects>

[<Measure>] [<Measure>] type m
