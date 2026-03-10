// #Regression #Diagnostics #ReqNOMT 
// Regression test for FSHARP1.0:2681
// To repro: fsi.exe < foo.fs, otherwise it won't repro!
// This test used to emit an error, now (after fix for 2193, it works fine)
//<Expects status="notin">\^\^\^\^\^</Expects>
//<Expects status="success"></Expects>
'\U00002620';;
exit 0;;
