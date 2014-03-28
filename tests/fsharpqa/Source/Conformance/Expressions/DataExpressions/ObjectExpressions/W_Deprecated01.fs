// #Regression #Conformance #DataExpressions #ObjectConstructors 
// Regression test for FSharp1.0:3389 - Deprecate older object expression syntax
//<Expects id="FS0035" span="(10,18-10,21)" status="error">This construct is deprecated: This form of object expression is not used in F#\. Use 'member this\.MemberName \.\.\. = \.\.\.' to define member implementations in object expressions\.</Expects>

#light
type X = 
    abstract M : unit -> 'a
    
let v = 
    { new X with M() = failwith "" }
exit 0
