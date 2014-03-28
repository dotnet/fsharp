// #Regression #Diagnostics #ReqNOMT 
// Regression test for FSHARP1.0:2681
// To repro: fsi.exe < foo.fs, otherwise it won't repro!
//<Expects id="FS1159" span="(5,1)" status="error">This Unicode encoding is only valid in string literals</Expects>
'\UFFFF2620';;
exit 1;;
