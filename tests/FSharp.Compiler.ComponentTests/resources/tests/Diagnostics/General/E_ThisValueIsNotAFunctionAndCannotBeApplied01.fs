// #Regression #Diagnostics 
// Regression test for FSHARP1.0:1406
//<Expects id="FS0003" span="(7,26-7,29)" status="error"></Expects>



let foo (arr : int[,]) = arr[1,2]
