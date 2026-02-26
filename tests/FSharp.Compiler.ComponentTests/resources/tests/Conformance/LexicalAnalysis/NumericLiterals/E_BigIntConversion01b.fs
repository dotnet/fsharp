// #Regression #Conformance #LexicalAnalysis #Constants  #ReqNOMT 
// Verify BigInt uses op_Implicit conversions
// Regression test for FSHARP1.0:4498
// Unsupported conversions - on Dev10/NetFx4.0
//<Expects status="error" span="(8,14-8,17)" id="FS0001">The type 'System\.Numerics\.BigInteger' does not support a conversion to the type 'char'$</Expects>

// Char and string
let _ = char 10I
