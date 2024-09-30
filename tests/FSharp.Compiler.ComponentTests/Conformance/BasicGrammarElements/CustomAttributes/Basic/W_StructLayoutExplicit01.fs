// #Regression #Conformance #DeclarationElements #Attributes 
// FSB 3573: Add unverifiability warning for StructLayout: ICE when you set Explicit struct layout but don't provide offsets for each field




open System.Collections.Generic
open System.Diagnostics
open System.Runtime.InteropServices;
 
[<StructLayout(LayoutKind.Explicit)>]
type A =
    new() = A() 
    [<DefaultValue>]
    [<FieldOffset(0)>]
    static val mutable private x : int
