// Copied from https://github.com/dungpa/fantomas and modified by Vasily Kirichenko

module FSharp.Compiler.Service.Tests.ServiceFormatting.FunctionDefinitionTests

open NUnit.Framework
open FsUnit
open TestHelper

[<Test>]
let ``recursive functions``() =
    formatSourceString false """
    let rec f x = g x
    and g x = x""" config
    |> prepend newline
    |> should equal """
let rec f x = g x

and g x = x
"""

[<Test>]
let ``recursive functions in type definition``() =
    formatSourceString false """
type C () = 
    let rec g x = h x
    and h x = g x

    member x.P = g 3""" config
    |> prepend newline
    |> should equal """
type C() =
    
    let rec g x = h x
    and h x = g x
    
    member x.P = g 3
"""

[<Test>]
let ``should keep mutually recursive functions``() =
    formatSourceString false """
let rec createJArray x = createJObject

and createJObject y = createJArray
    """ config
    |> should equal """let rec createJArray x = createJObject

and createJObject y = createJArray
"""

[<Test>]
let ``should keep mutually recursive functions in nested function``() =
    formatSourceString false """let f =
    let rec createJArray x = createJObject x

    and createJObject y = createJArray y
    createJArray
    """ config
    |> should equal """let f =
    let rec createJArray x = createJObject x
    and createJObject y = createJArray y
    createJArray
"""

[<Test>]
let ``should keep identifiers with whitespace in double backticks``() =
    formatSourceString false """let ``should keep identifiers in double backticks``() = x
    """ config
    |> should equal """let ``should keep identifiers in double backticks``() = x
"""

[<Test>]
let ``should remove backticks from shouldn't identifier``() =
    formatSourceString false """let ``shouldn't``() = x
    """ config
    |> should equal """let shouldn't() = x
"""

[<Test>]
let ``should keep identifiers with + in double backticks``() =
    formatSourceString false """let ``Foo+Bar``() = x
    """ config
    |> should equal """let ``Foo+Bar``() = x
"""

[<Test>]
let ``let bindings with return types``() =
    formatSourceString false """
       let divide x y =
           let stream : System.IO.FileStream = System.IO.File.Create("test.txt")
           let writer : System.IO.StreamWriter = new System.IO.StreamWriter(stream)
           try
              writer.WriteLine("test1");
              Some( x / y )
           finally
              writer.Flush()
              printfn "Closing stream"
              stream.Close()""" config
    |> prepend newline
    |> should equal """
let divide x y =
    let stream : System.IO.FileStream = System.IO.File.Create("test.txt")
    let writer : System.IO.StreamWriter = new System.IO.StreamWriter(stream)
    try 
        writer.WriteLine("test1")
        Some(x / y)
    finally
        writer.Flush()
        printfn "Closing stream"
        stream.Close()
"""

[<Test>]
let ``type constraints and inline``() =
    formatSourceString false """
let inline add(value1 : ^T when ^T : (static member (+) : ^T * ^T -> ^T), value2: ^T) =
    value1 + value2

let inline heterogenousAdd(value1 : ^T when (^T or ^U) : (static member (+) : ^T * ^U -> ^T), value2 : ^U) =
    value1 + value2""" config
    |> prepend newline
    |> should equal """
let inline add (value1 : ^T when ^T : (static member (+) : ^T * ^T -> ^T), 
                value2 : ^T) = value1 + value2
let inline heterogenousAdd (value1 : ^T when (^T or ^U) : (static member (+) : ^T * ^U
                                                                -> ^T), 
                            value2 : ^U) = value1 + value2
"""

[<Test>]
let ``should keep whitespace after function call``() =
    formatSourceString false """let relative = (toRelativePath fileName).TrimStart '.'
    """ config
    |> should equal """let relative = (toRelativePath fileName).TrimStart '.'
"""

[<Test>]
let ``should keep type annotations``() =
    formatSourceString false """let empty<'T> : LazyList<'T> = EmptyValue<'T>.Value""" config
    |> should equal """let empty<'T> : LazyList<'T> = EmptyValue<'T>.Value
"""

[<Test>]
let ``should add spaces between multiline nested let bindings``() =
    formatSourceString false """let f1 = 
    let f2 x = 
        let _ = ()
        x + 1
    let f3 y = 
        let _ = ()
        y + 1
    x + y""" config
    |> should equal """let f1 =
    let f2 x =
        let _ = ()
        x + 1
    
    let f3 y =
        let _ = ()
        y + 1
    
    x + y
"""

[<Test>]
let ``should indent fun blocks``() =
    formatSourceString false """let f =
    fun x ->
    let y = 1
    x""" config
    |> should equal """let f =
    fun x -> 
        let y = 1
        x
"""
[<Test>]
let ``should not add spaces into a series of function application``() =
    formatSourceString false """let f x = "d"
f(1).Contains("3")""" config
    |> should equal """let f x = "d"

f(1).Contains("3")
"""

[<Test>]
let ``should handle external functions``() =
    formatSourceString false """[<DllImport(@"__Internal", CallingConvention = CallingConvention.Cdecl)>]
extern ReturnCode  GetParent (System.IntPtr inRef, byref outParentRef)""" config
    |> prepend newline
    |> should equal """
[<DllImport(@"__Internal", CallingConvention = CallingConvention.Cdecl)>]
extern ReturnCode GetParent(System.IntPtr inRef, byref outParentRef)
"""

[<Test>]
let ``should handle simple external functions``() =
    formatSourceString false """module InteropWithNative =
        [<DllImport(@"__Internal", CallingConvention = CallingConvention.Cdecl)>]
        extern IntPtr setCallbridgeSupportTarget(IntPtr newTarget)""" config
    |> prepend newline
    |> should equal """
module InteropWithNative =
    [<DllImport(@"__Internal", CallingConvention = CallingConvention.Cdecl)>]
    extern IntPtr setCallbridgeSupportTarget(IntPtr newTarget)
"""

[<Test>]
let ``should handle external functions with fully-qualified attributes``() =
    formatSourceString false """[<System.Runtime.InteropServices.DllImport("user32.dll")>]
extern int GetWindowLong(System.IntPtr hwnd, int index)""" config
    |> prepend newline
    |> should equal """
[<System.Runtime.InteropServices.DllImport("user32.dll")>]
extern int GetWindowLong(System.IntPtr hwnd, int index)
"""

[<Test>]
let ``should handle external functions with special types``() =
    formatSourceString false """open System
open System.Runtime.InteropServices
open Accessibility

[<DllImport("oleacc.dll")>]
extern int AccessibleChildren(
    IAccessible paccContainer, 
    int iChildStart, 
    int cChildren, 
    [<Out()>] [<MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4s)>] System.Object [] rgvarChildren, 
    int* pcObtained)""" config
    |> prepend newline
    |> should equal """
open System
open System.Runtime.InteropServices
open Accessibility

[<DllImport("oleacc.dll")>]
extern int AccessibleChildren(IAccessible paccContainer, int iChildStart, int cChildren, [<Out; MarshalAs(UnmanagedType.LPArray, 
                                                                                                          SizeParamIndex = 4s)>] System.Object [] rgvarChildren, int* pcObtained)
"""

[<Test>]
let ``should handle desugared matches correctly``() =
    formatSourceString false """
type U = X of int
let f = fun x -> match x with X (x) -> x
"""  config
    |> prepend newline
    |> should equal """
type U =
    | X of int

let f =
    fun x -> 
        match x with
        | X(x) -> x
"""

[<Test>]
let ``should handle member constraints and generic params correctly``() =
    formatSourceString false """
let inline implicit< ^a,^b when ^a : (static member op_Implicit : ^b -> ^a)> arg =
        (^a : (static member op_Implicit : ^b -> ^a) arg)
"""  config
    |> prepend newline
    |> should equal """
let inline implicit< ^a, ^b when ^a : (static member op_Implicit : ^b -> ^a)> arg =
    (^a : (static member op_Implicit : ^b -> ^a) arg)
"""

[<Test>]
let ``don't add spaces for function application inside dot access``() =
    formatSourceString false """
let f x = "foo"
f(42).Length
"""  config
    |> prepend newline
    |> should equal """
let f x = "foo"

f(42).Length
"""

[<Test>]
let ``do add spaces for function application inside parentheses inside dot access``() =
    formatSourceString false """let inputBlah = "So, I was like, Visual Studio did wat!?"
let someBlahing = (Blah.TryCreate inputBlah).Value"""  config
    |> prepend newline
    |> should equal """
let inputBlah = "So, I was like, Visual Studio did wat!?"
let someBlahing = (Blah.TryCreate inputBlah).Value
"""

[<Test>]
let ``don't create redundant parentheses outside trait calls``() =
    formatSourceString false """let f (arg : 'T) = (^T : (member Value : string) arg)"""  config
    |> prepend newline
    |> should equal """
let f (arg : 'T) = (^T : (member Value : string) arg)
"""