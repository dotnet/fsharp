namespace Test
open System.Runtime.InteropServices
[<ExtendedLayout(ExtendedLayoutKind.CStruct)>]
exception MyExn of string
