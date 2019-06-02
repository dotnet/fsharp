// #Regression #Conformance #LexicalAnalysis 
// Regression test for FSHARP1.0:2193
// Unicodegraph-long not in parity with C#
//<Expects status="success"></Expects>

#light
let some_unicode_char = '\u00D6' 
let another_unicode_char = '\U000007FF' 
exit 0
