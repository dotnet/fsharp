// #Regression #Conformance #TypesAndModules #GeneratedEqualityAndHashing #Attributes #Diagnostics 
// Regression test for FSHARP1.0:5406
// Scenario #2: I'm writing my own custom IComparable implementation - don't bother giving me one
//<Expects status="error" span="(7,3-7,23)" id="FS0501">The object constructor 'StructuralComparisonAttribute' takes 0 argument\(s\) but is here given 1\. The required signature is 'new: unit -> StructuralComparisonAttribute'\.$</Expects>

[<StructuralEquality>]
[<StructuralComparison(false)>]
type DU = 
    | A of int
    interface System.IComparable with
        override this.CompareTo(a:obj) = -1
