module MyTestModule

open System
open System.Collections.Generic


type DerivedWhichAllowsNull() =
   inherit ResizeArray<string | null>()
   member x.FirstItem = x[0]

type DerivedWithoutNull() =
   inherit ResizeArray<string>()
   member x.FirstItem = x[0]

type ICanGetAnything<'T> =
    abstract member Get : unit -> 'T

type MyClassImplementingTheSameInterface() =
    interface ICanGetAnything<string | null> with
        member x.Get() = null
    interface ICanGetAnything<ResizeArray<ResizeArray<string>>> with
        member x.Get() = new ResizeArray<_>()
    interface ICanGetAnything<ResizeArray<string | null> | null> with
        member x.Get() = null