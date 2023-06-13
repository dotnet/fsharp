// #Regression #Conformance #DeclarationElements #Attributes 
// FSB 3573: Add unveriofiability warning for StructLayout: ICE when you set Explicit struct layout but don't provide offsets for each field
//<Expects id="FS0009" span="(12,6-12,7)" status="warning">Uses of this construct may result in the generation of unverifiable \.NET IL code\. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'</Expects>



open System.Collections.Generic
open System.Diagnostics
open System.Runtime.InteropServices;
 
[<StructLayout(LayoutKind.Explicit)>]
type A =
    new() = A() 
    [<DefaultValue>]
    [<FieldOffset(0)>]
    static val mutable private x : int
