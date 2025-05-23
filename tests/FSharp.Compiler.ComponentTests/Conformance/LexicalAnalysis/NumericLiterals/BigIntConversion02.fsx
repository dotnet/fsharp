// #Regression #Conformance #LexicalAnalysis #Constants  
// Verify BigInt uses op_Implicit conversions
// Regression test for FSHARP1.0:4498
// Supported conversions
//<Expects status="success"></Expects>

if int32 10I <> 10 then ignore 1
if int64 10I <> 10L then ignore 1
if string 10I <> "10" then ignore 1
if float 10I <> 10. then ignore 1

ignore 0

