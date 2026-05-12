// #Conformance #DeclarationElements #Regression #PInvoke #LetBindings
// Regression for FSHARP1.0:6211, used to throw InvalidProgramException at runtime
// <Expects status="error" id="FS1221" span="(14,9-14,26)">DLLImport bindings must be static members in a class or function definitions in a module</Expects>
open System.Runtime.InteropServices

type Works() =
    [<DllImport("user32.dll")>]
    static let GetCaretBlinkTime() : int = failwith ""
    do
        printf "%d" (GetCaretBlinkTime())

type FailsAtRuntime() =
    [<DllImport("user32.dll")>]
    let GetCaretBlinkTime() : int = failwith ""
    do  
        printf "%d" (GetCaretBlinkTime())   // used to throw here
    
Works() |> ignore
FailsAtRuntime() |> ignore

exit 1