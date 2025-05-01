// #Regression #Conformance #TypesAndModules #GeneratedEqualityAndHashing #Attributes 
// Regression test for FSHARP1.0:4571
//<Expects status="error" span="(10,6-10,12)" id="FS0378">The 'NoEquality' attribute must be used in conjunction with the 'NoComparison' attribute$</Expects>
//<Expects status="warning" span="(10,6-10,12)" id="FS0346">The struct, record or union type 'MyType' has an explicit implementation of 'Object\.Equals'\. Consider implementing a matching override for 'Object\.GetHashCode\(\)'$</Expects>
//<Expects status="warning" span="(10,6-10,12)" id="FS0386">A type with attribute 'NoEquality' should not usually have an explicit implementation of 'Object\.Equals\(obj\)'\. Disable this warning if this is intentional for interoperability purposes$</Expects>

[<Struct>]
//[<StructuralComparison(false)>]
[<NoEqualityAttribute>]
type MyType =
    val a : int
    val b : int
    override this.Equals(x:obj) = false

