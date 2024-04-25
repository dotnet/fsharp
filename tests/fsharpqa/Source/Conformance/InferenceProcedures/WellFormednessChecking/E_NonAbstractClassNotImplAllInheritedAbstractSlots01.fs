// #Regression #Conformance #TypeInference 
// Regression test for FSHARP1.0:6123
//<Expects status="error" span="(6,6-6,15)" id="FS0365">No implementation was given for 'Derived\.get_Foo\(\) : int'$</Expects>
//<Expects status="error" span="(6,6-6,15)" id="FS0054">Non-abstract classes cannot contain abstract members\. Either provide a default member implementation or add the '\[<AbstractClass>\]' attribute to your type\.$</Expects>

type FSDerived() = 
    inherit Derived()

let x = new FSDerived()
