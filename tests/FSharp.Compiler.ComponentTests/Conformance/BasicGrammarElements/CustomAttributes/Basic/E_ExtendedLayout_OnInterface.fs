// ExtendedLayoutAttribute on interface should fail
namespace Test

open System.Runtime.InteropServices

[<ExtendedLayout(ExtendedLayoutKind.CStruct)>]
type IInvalidInterface =
    abstract member DoSomething: unit -> unit
