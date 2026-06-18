// #Regression #Conformance #PatternMatching #ActivePatterns 
// Regression test for FSHARP1.0:5590
//<Expects status="error" span="(7,6-7,11)" id="FS1209">Active pattern '|A|B|' is not a function$</Expects>
//<Expects status="error" span="(8,6-8,9)" id="FS1209">Active pattern '|C|' is not a function$</Expects>
//<Expects status="error" span="(9,6-9,11)" id="FS1209">Active pattern '|D|_|' is not a function$</Expects>

let (|A|B|) = failwith "" : Choice<int,int>
let (|C|) = 3
let (|D|_|) = None
