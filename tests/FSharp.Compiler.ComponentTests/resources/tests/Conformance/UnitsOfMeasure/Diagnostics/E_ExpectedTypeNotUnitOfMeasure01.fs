// #Regression #Conformance #UnitsOfMeasure #Diagnostics 
// Regression test for FSHARP1.0:2345
//<Expects id="FS0033" span="(11,10-11,19)" status="error">The non-generic type 'Microsoft\.FSharp\.Core\.string' does not expect any type arguments, but here is given 1 type argument\(s\)</Expects>
//<Expects id="FS0704" span="(12,11-12,12)" status="error">Expected type, not unit-of-measure</Expects>
//<Expects id="FS0704" span="(13,17-13,18)" status="error">Expected type, not unit-of-measure</Expects>
//<Expects id="FS0704" span="(14,49-14,50)" status="error">Expected type, not unit-of-measure</Expects>
#light
 
[<Measure>] type m

let e1 : string<m> = "aa"
let (e2 : m list) = () 
let e3 = typeof<m>
let e4 : System.Collections.Generic.IEnumerable<m> = []

