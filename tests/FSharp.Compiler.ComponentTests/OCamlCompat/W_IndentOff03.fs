// #Regression #OCaml 
// Regression test for FSHARP1.0:5984
//<Expects status="warning" span="(4,1-4,14)" id="FS0062">This construct is for ML compatibility\. Consider using a file with extension '\.ml' or '\.mli' instead\. You can disable this warning by using '--mlcompatibility' or '--nowarn:62'\.$</Expects>
#indent "off"
printfn "Finished"
