// #Regression #Conformance #LexicalAnalysis 
// Regression test for FSharp1.0:3820 - structs should not allow 'do'
//<Expects id="FS0035" status="error">This construct is deprecated: Structs cannot contain 'do' bindings because the default constructor for structs would not execute these bindings$</Expects>

[<Struct>]
type S (def : int) =
  do System.Console.WriteLine("Structs cannot use 'do'!")

let s = new S(0)

exit 1
