// #Conformance #DeclarationElements #Attributes #Import #RequiresPowerPack 
// Ignore "Uses of this construct may result in the generation of unverifiable .NET IL code..."
#nowarn "9"

open System.Runtime.InteropServices
open Microsoft.FSharp.Math

[<Struct>]
[<StructLayout(LayoutKind.Explicit)>]        // As per FSHARP1.0:5931, LayoutKind.Sequential (=default) does not allow FieldOffset
type Align16 =
    [<FieldOffset(0)>]
    val mutable x0  : complex
    [<FieldOffset(8)>]
    val x1          : complex

// Verify no runtime asserts...
let mutable test1 = new Align16()
test1.x0 <- new complex()

exit 0
