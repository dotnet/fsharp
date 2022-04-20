// #Regression #Conformance #TypesAndModules #GeneratedEqualityAndHashing #Attributes 
// Regression test for FSHARP1.0:4571

//<Expects status="error" span="(7,3-7,21)" id="FS0501">The object constructor 'StructuralEqualityAttribute' takes 0 argument\(s\) but is here given 1\. The required signature is 'new: unit -> StructuralEqualityAttribute'\.$</Expects>
[<Struct>]
//[<StructuralComparison(false)>]
[<StructuralEquality(false)>]
type MyType =
    val a : int
    val b : int
    override this.Equals(x:obj) = false

