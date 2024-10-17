// #Regression #Conformance #PatternMatching #ActivePatterns 
// Regression test for FSHARP1.0:5590
//<Expects status="error" span="(5,6-5,11)" id="FS1209">Active pattern '|A|B|' is not a function$</Expects>

let (|A|B|) = failwith "" : Choice<int,int>
