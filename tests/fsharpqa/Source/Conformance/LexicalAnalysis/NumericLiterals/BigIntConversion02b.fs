// #Regression #Conformance #LexicalAnalysis #Constants #NoMono  #ReqNOMT 
// Verify BigInt uses op_Implicit conversions
// Regression test for FSHARP1.0:4498
// Supported conversions - on Dev10/NetFx4.0
//<Expects status="success"></Expects>

if int8 10I <> 10y then exit 1
if int16 10I <> 10s then exit 1
if int32 10I <> 10 then exit 1
if int64 10I <> 10L  then exit 1

if uint8 10I <> 10uy then exit 1
if uint16 10I <> 10us then exit 1
if uint32 10I <> 10u then exit 1
if uint64 10I <> 10UL  then exit 1

if string 10I <> "10" then exit 1

if float 10I <> 10. then exit 1
if float32 10I <> 10.f then exit 1
if decimal 10I <> 10M then exit 1

exit 0
