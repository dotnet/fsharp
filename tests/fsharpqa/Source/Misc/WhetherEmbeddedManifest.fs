// #Regression #Misc #NoMono 
// Regression test for FSHARP1.0:1668
// Compiler should provide a way to embed manifest files
//<Expects status="success"></Expects>

open System.Reflection
let thisExe = System.Reflection.Assembly.GetExecutingAssembly()
let foo= thisExe.GetManifestResourceInfo("FSharpSignatureCompressedData.WhetherEmbeddedManifest")
if foo = null 
    then exit 1
    else exit 0
