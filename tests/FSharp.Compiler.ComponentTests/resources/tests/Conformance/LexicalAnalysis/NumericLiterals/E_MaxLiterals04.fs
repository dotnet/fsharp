// #Regression #Conformance #LexicalAnalysis #Constants 


// Verify compile error for signed literals which are MaxSize + 1, MaxSize - 1
// All of these should cause compiler errors

//<Expects id="FS1149" status="error">This number is outside the allowable range for 64-bit signed integers</Expects>
//<Expects id="FS1149" status="error">This number is outside the allowable range for 64-bit signed integers</Expects>

let int64Bin65bits   = 0b1_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000L
let int64Octal65bits = 0o2_000_000_000_000_000_000_000L
