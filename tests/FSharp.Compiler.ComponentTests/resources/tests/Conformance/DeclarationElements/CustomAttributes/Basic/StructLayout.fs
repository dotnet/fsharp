// #Regression #Conformance #DeclarationElements #Attributes 
// Regression tests for FSHARP1.0:5931

module M

open System.Runtime.InteropServices

// vanilla - will build
[<Struct>]
type S =
    [<DefaultValue>]
    val v1 : int
    [<DefaultValue>]
    val v2 : int
    
// Explicit layout - will build
[<Struct>]
[<StructLayout(LayoutKind.Explicit)>]
type SExplicit =
    [<DefaultValue>]
    [<FieldOffset(0)>]
    val v1 : int
    [<DefaultValue>]
    [<FieldOffset(1)>]
    val v2 : int    
    
// Sequential layout - will build
[<Struct>]
[<StructLayout(LayoutKind.Sequential)>]
type SSequential =
    [<DefaultValue>]
    val v1 : int
    [<DefaultValue>]
    val v2 : int
