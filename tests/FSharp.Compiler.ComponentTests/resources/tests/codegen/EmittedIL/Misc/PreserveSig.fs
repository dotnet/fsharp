namespace Foo

open System.Runtime.InteropServices

type Bar =
  [<PreserveSig>]
  abstract MyCall: unit -> int32