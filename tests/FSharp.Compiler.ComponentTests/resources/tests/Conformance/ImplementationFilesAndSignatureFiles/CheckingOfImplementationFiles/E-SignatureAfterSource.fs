// #Regression #Conformance #SignatureFiles 
//<Expects id="FS0238" status="error" span="(8,1)">An implementation of file or module 'E-SignatureAfterSource' has already been given\. Compilation order is significant in F# because of type inference\. You may need to adjust the order of your files to place the signature file before the implementation\. In Visual Studio files are type-checked in the order they appear in the project file, which can be edited manually or adjusted using the solution explorer</Expects>

module ``E-SignatureAfterSource``

type TypeInAnonSigFile() =
    member this.Value = 10


let test = new TypeInAnonSigFile()

