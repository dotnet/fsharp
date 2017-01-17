// #Regression #Conformance #ObjectOrientedTypes #TypeExtensions 
// Regression test for FSharp1.0:5249
// Title: Compiler exception and type-checker crash when using type extension.

//<Expects status="error" id="FS0039" span="(9,40-9,46)">The value or constructor 'Object' is not defined</Expects>
//<Expects status="error" id="FS0039" span="(14,33-14,34)">The value or constructor 'T' is not defined</Expects>
//<Expects status="error" id="FS0039" span="(17,49-17,57)">The value, namespace, type or module 'DateTime' is not defined</Expects>

type System.Object with member x.Foo = Object()

module Test =
    type T() = class end

type Test.T with member x.Bar = T()

module Test1 = 
    type System.DateTime with member x.FooBaz = DateTime.Now
