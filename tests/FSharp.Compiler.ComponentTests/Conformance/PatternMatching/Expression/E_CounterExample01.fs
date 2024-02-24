// #Regression #Conformance #PatternMatching 
    // Regression test for FSHARP1.0:2034
//<Expects id="FS0025" span="(7,11-7,12)" status="warning">Incomplete pattern matches on this expression\. For example, the value '0' may indicate a case not covered by the pattern\(s\)</Expects>

module TestModule
let f x = 
    match x with        // warning FS0025: Incomplete pattern matches on this expression. For example, the value '0' may indicate a case not covered by the pattern(s)
    | 1 -> 7
    | 2 -> 49
