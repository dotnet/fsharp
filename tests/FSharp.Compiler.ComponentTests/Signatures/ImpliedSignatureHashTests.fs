module FSharp.Compiler.ComponentTests.Signatures.ImpliedSignatureHashTests

open Xunit
open FSharp.Test
open FSharp.Test.Compiler



[<Theory>]

[<InlineDataAttribute("PrivateModuleAdded",
(*BEFORE*)"""module MyTest
type MyRecord = {X:string}"""
(*AFTER*),"""module MyTest
type MyRecord = {X:string}
module private PrivateInnerModule = 
    let private add a b = a + b""")>]

[<InlineDataAttribute("NestedPrivateModuleAdded",
(*BEFORE*)"""module MyTest
type MyRecord = {X:string}
module InnerModule = 
    let xxx = 42"""
(*AFTER*),"""module MyTest
type MyRecord = {X:string}
module InnerModule = 
    let xxx = 42
    module private PrivateInnerMostModule = 
        let add a b = a + b""")>]

[<InlineDataAttribute("EmptyNonPrivateChildModule",
(*BEFORE*)"""module MyTest
type MyRecord = {X:string}"""
(*AFTER*),"""module MyTest
type MyRecord = {X:string}
module  PublicInnerModule = 
    let private add a b = a + b""")>]

[<InlineDataAttribute("OpenSystemAdded",
(*BEFORE*)"""module MyTest
type MyRecord = {X:System.IDisposable}"""
(*AFTER*),"""module MyTest
open System
type MyRecord = {X:IDisposable}""")>]

[<InlineDataAttribute("DoActionAdded",
(*BEFORE*)"""module MyTest
type MyRecord = {X:string}"""
(*AFTER*),"""module MyTest
type MyRecord = {X:string}
do printfn "Hello" """)>]


[<InlineDataAttribute("NothingChanged",
(*BEFORE*)"""module MyTest
type MyRecord = {X:string}"""
(*AFTER*),"""module MyTest
type MyRecord = {X:string}""")>]

[<InlineDataAttribute("TypeAliasUsed",
(*BEFORE*)"""module MyTest
type MyRecord = {X:string}"""
(*AFTER*),"""module MyTest
type MyRecord = {X:System.String}""")>]

[<InlineDataAttribute("CustomTypeAliasUsed",
(*BEFORE*)"""module MyTest
type MyString = string
type MyRecord = {X:string}"""
(*AFTER*),"""module MyTest
type MyString = string
type MyRecord = {X:MyString}""")>]


[<InlineDataAttribute("PrivateBindingAdded",
(*BEFORE*)"""module MyTest
type MyRecord = {X:string}"""
(*AFTER*),"""module MyTest
type MyRecord = {X:string}
let private getValue() = 42""")>]

[<InlineDataAttribute("FunctionAnnotated",
(*BEFORE*)"""module MyTest
type MyRecord = {X:string}
let processRecord myRec = myRec.X"""
(*AFTER*),"""module MyTest
type MyRecord = {X:string}
let processRecord (myRec:MyRecord) = myRec.X""")>]

[<InlineDataAttribute("EnumReordered",
(*BEFORE*)"""module MyTest
type MyEnum = 
    | A = 0
    | B = 1"""
(*AFTER*),"""module MyTest
type MyEnum = 
    | B = 1
    | A = 0""")>]

[<InlineDataAttribute("ValueOfBinding",
(*BEFORE*)"""module MyTest
let myVal = 42"""
(*AFTER*),"""module MyTest
let myVal = -1""")>]

[<InlineDataAttribute("SRTP_Reordered",
(*BEFORE*)"""module MyTest
let inline mySRTPFunc<'a when 'a:(static member Zero: unit -> int) and 'a:(static member One: unit -> int)> () = 'a.Zero() + 'a.One()"""
(*AFTER*),"""module MyTest
let inline mySRTPFunc<'a when 'a:(static member One: unit -> int) and 'a:(static member Zero: unit -> int)> () = 'a.Zero() + 'a.One()""")>]

[<InlineDataAttribute("ReturnTypeReplacedWithObjectExpression",
(*BEFORE*)"""module MyTest
let domeSomething() : System.IDisposable = failwith "TODO" """
(*AFTER*),"""module MyTest
let domeSomething() = { new System.IDisposable with member x.Dispose() = () }  """)>]

let ``Hash should be stable for`` (change:string,codeBefore:string,codeAfter:string) =    
    let hashBefore = Fs codeBefore |> getImpliedSignatureHash
    let hashAfter = Fs codeAfter |> getImpliedSignatureHash


    Assert.True((hashBefore = hashAfter), userMessage = change.ToString())




[<InlineDataAttribute("ChildModuleAdded",
(*BEFORE*)"""module MyTest
type MyRecord = {X:string}"""
(*AFTER*),"""module MyTest
type MyRecord = {X:string}
module  PrivateInnerModule = 
    let add a b = a + b""")>]

[<InlineDataAttribute("FunctionSpecialized",
(*BEFORE*)"""module MyTest
let inline add a b = a + b"""
(*AFTER*),"""module MyTest
let inline add (a:int) (b:int) = a + b""")>]

[<InlineDataAttribute("SRTP_Condition_Added",
(*BEFORE*)"""module MyTest
let inline mySRTPFunc<'a when 'a:(static member Zero: unit -> int)> () = 'a.Zero() + 'a.Zero()"""
(*AFTER*),"""module MyTest
let inline mySRTPFunc<'a when 'a:(static member One: unit -> int) and 'a:(static member Zero: unit -> int)> () = 'a.Zero() + 'a.One()""")>]

[<InlineDataAttribute("SRTP_Member_Renamed",
(*BEFORE*)"""module MyTest
let inline mySRTPFunc<'a when 'a:(static member Zero: unit -> int)> () = 'a.Zero() + 'a.Zero()"""
(*AFTER*),"""module MyTest
let inline mySRTPFunc<'a when 'a:(static member One: unit -> int)> () = 'a.One() + 'a.One()""")>]

[<InlineDataAttribute("SRTP_Type_Changed",
(*BEFORE*)"""module MyTest
let inline mySRTPFunc<'a when 'a:(static member Zero: unit -> int)> () = 'a.Zero() + 'a.Zero()"""
(*AFTER*),"""module MyTest
let inline mySRTPFunc<'a when 'a:(static member Zero: unit -> byte)> () = 'a.Zero() + 'a.Zero()""")>]

[<InlineDataAttribute("QuotationTypeChanged",
(*BEFORE*)"""module MyTest
let foo () = <@ 2 @>"""
(*AFTER*),"""module MyTest
let foo () = <@ false @>""")>]

[<InlineDataAttribute("QuotationExpressionChanged",
(*BEFORE*)"""module MyTest
let foo () = <@ 2 + 2 @>"""
(*AFTER*),"""module MyTest
let foo () = <@ 2 + 3 @>""")>]

[<InlineDataAttribute("UnionReordered",
(*BEFORE*)"""module MyTest
type MyDU = 
    | A 
    | B """
(*AFTER*),"""module MyTest
type MyDU = 
    | B 
    | A """)>]

[<InlineDataAttribute("UnionTurnedStruct",
(*BEFORE*)"""module MyTest
type MyDU = 
    | A 
    | B """
(*AFTER*),"""module MyTest
[<Struct>]
type MyDU = 
    | A 
    | B """)>]

//TODO add a lot more negative tests - in which cases should hash in fact change

[<Theory>]
let ``Hash should change when`` (change:string,codeBefore:string,codeAfter:string) =  
    let hashBefore = Fs codeBefore |> getImpliedSignatureHash
    let hashAfter = Fs codeAfter |> getImpliedSignatureHash


    Assert.False((hashBefore = hashAfter), userMessage = change.ToString())