module FSharp.Compiler.Service.Tests.CompletionObjectInitializersTests

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open Xunit

let private propPlain = """
type A() = 
   member val SettableProperty = 1 with get,set
   member val AnotherSettableProperty = 1 with get,set
   member val NonSettableProperty = 1
"""

let private propGeneric = """
type A<'a>() = 
   member val SettableProperty = 1 with get,set
   member val AnotherSettableProperty = 1 with get,set
   member val NonSettableProperty = 1
"""

let private propModule = """
module M =
   type A() = 
       member val SettableProperty = 1 with get,set
       member val AnotherSettableProperty = 1 with get,set
       member val NonSettableProperty = 1
"""

let private propModuleGeneric = """
module M =
   type A<'a, 'b>() = 
       member val SettableProperty = 1 with get,set
       member val AnotherSettableProperty = 1 with get,set
       member val NonSettableProperty = 1
"""

let propertyCases: obj[] seq =
    [
        [| box (propPlain + "A((**){caret})"); box [ "SettableProperty"; "AnotherSettableProperty" ]; box [ "NonSettableProperty" ] |]
        [| box (propPlain + "A(S{caret} = 1)"); box [ "SettableProperty" ]; box [ "NonSettableProperty" ] |]
        [| box (propPlain + "A(S = 1{caret})"); box ([]: string list); box [ "SettableProperty"; "NonSettableProperty" ] |]
        [| box (propPlain + "A(S = 1,{caret})"); box [ "AnotherSettableProperty" ]; box [ "NonSettableProperty" ] |]
        [| box (propPlain + "new A((**){caret})"); box [ "SettableProperty"; "AnotherSettableProperty" ]; box [ "NonSettableProperty" ] |]
        [| box (propPlain + "new A(S{caret} = 1)"); box [ "SettableProperty" ]; box [ "NonSettableProperty" ] |]
        [| box (propPlain + "new A(S = 1{caret})"); box ([]: string list); box [ "SettableProperty"; "NonSettableProperty" ] |]
        [| box (propPlain + "new A(S = 1,{caret})"); box [ "AnotherSettableProperty" ]; box [ "NonSettableProperty" ] |]

        [| box (propGeneric + "A((**){caret})"); box [ "SettableProperty"; "AnotherSettableProperty" ]; box [ "NonSettableProperty" ] |]
        [| box (propGeneric + "A(S{caret} = 1)"); box [ "SettableProperty" ]; box [ "NonSettableProperty" ] |]
        [| box (propGeneric + "A(S = 1{caret})"); box ([]: string list); box [ "SettableProperty"; "NonSettableProperty" ] |]
        [| box (propGeneric + "A(S = 1,{caret})"); box [ "AnotherSettableProperty" ]; box [ "NonSettableProperty" ] |]
        [| box (propGeneric + "new A<_>((**){caret})"); box [ "SettableProperty"; "AnotherSettableProperty" ]; box [ "NonSettableProperty" ] |]
        [| box (propGeneric + "new A<_>(S{caret} = 1)"); box [ "SettableProperty" ]; box [ "NonSettableProperty" ] |]
        [| box (propGeneric + "new A<_>(S = 1{caret})"); box ([]: string list); box [ "SettableProperty"; "NonSettableProperty" ] |]
        [| box (propGeneric + "new A<_>(S = 1,{caret})"); box [ "AnotherSettableProperty" ]; box [ "NonSettableProperty" ] |]

        [| box (propModule + "M.A((**){caret})"); box [ "SettableProperty"; "AnotherSettableProperty" ]; box [ "NonSettableProperty" ] |]
        [| box (propModule + "M.A(S{caret} = 1)"); box [ "SettableProperty" ]; box [ "NonSettableProperty" ] |]
        [| box (propModule + "M.A(S = 1{caret})"); box ([]: string list); box [ "NonSettableProperty"; "SettableProperty" ] |]
        [| box (propModule + "M.A(S = 1,{caret})"); box [ "AnotherSettableProperty" ]; box [ "NonSettableProperty" ] |]
        [| box (propModule + "new M.A((**){caret})"); box [ "SettableProperty"; "AnotherSettableProperty" ]; box [ "NonSettableProperty" ] |]
        [| box (propModule + "new M.A(S{caret} = 1)"); box [ "SettableProperty" ]; box [ "NonSettableProperty" ] |]
        [| box (propModule + "new M.A(S = 1{caret})"); box ([]: string list); box [ "NonSettableProperty"; "SettableProperty" ] |]

        [| box (propModuleGeneric + "M.A((**){caret})"); box [ "SettableProperty"; "AnotherSettableProperty" ]; box [ "NonSettableProperty" ] |]
        [| box (propModuleGeneric + "M.A(S{caret} = 1)"); box [ "SettableProperty" ]; box [ "NonSettableProperty" ] |]
        [| box (propModuleGeneric + "M.A(S = 1{caret})"); box ([]: string list); box [ "SettableProperty"; "NonSettableProperty" ] |]
        [| box (propModuleGeneric + "M.A(S = 1,{caret})"); box [ "AnotherSettableProperty" ]; box [ "NonSettableProperty" ] |]
        [| box (propModuleGeneric + "new M.A<_, _>((**){caret})"); box [ "SettableProperty"; "AnotherSettableProperty" ]; box [ "NonSettableProperty" ] |]
        [| box (propModuleGeneric + "new M.A<_, _>(S{caret} = 1)"); box [ "SettableProperty" ]; box [ "NonSettableProperty" ] |]
        [| box (propModuleGeneric + "new M.A<_, _>(S = 1{caret})"); box ([]: string list); box [ "NonSettableProperty"; "SettableProperty" ] |]
        [| box (propModuleGeneric + "new M.A<_, _>(S = 1,{caret})"); box [ "AnotherSettableProperty" ]; box [ "NonSettableProperty" ] |]
    ]

[<Theory; MemberData(nameof propertyCases)>]
let ``ObjectInitializer.CompletionForProperties`` (source: string) (included: string list) (excluded: string list) =
    let info = Checker.getCompletionInfo source
    assertHasItemWithNames included info
    assertHasNoItemsWithNames excluded info

let private namedPlain = """
type A = 
   static member Run(xyz: int, zyx: string) = 1
"""

let private namedGeneric = """
type A = 
   static member Run<'T>(xyz: 'T, zyx: string) = 1
"""

let namedParamCases: obj[] seq =
    [
        [| box (namedPlain + "A.Run({caret})"); box [ "xyz"; "zyx" ] |]
        [| box (namedPlain + "A.Run(x{caret} = 1)"); box [ "xyz" ] |]
        [| box (namedPlain + "A.Run(x = 1,{caret})"); box [ "xyz"; "zyx" ] |]

        [| box (namedGeneric + "A.Run({caret})"); box [ "xyz"; "zyx" ] |]
        [| box (namedGeneric + "A.Run(x{caret} = 1)"); box [ "xyz" ] |]
        [| box (namedGeneric + "A.Run(x = 1,{caret})"); box [ "xyz"; "zyx" ] |]
        [| box (namedGeneric + "A.Run<_>({caret})"); box [ "xyz"; "zyx" ] |]
        [| box (namedGeneric + "A.Run<_>(x{caret} = 1)"); box [ "xyz" ] |]
        [| box (namedGeneric + "A.Run<_>(x = 1,{caret})"); box [ "xyz"; "zyx" ] |]
    ]

[<Theory; MemberData(nameof namedParamCases)>]
let ``ObjectInitializer.CompletionForNamedParameters`` (source: string) (expected: string list) =
    assertHasItemWithNames expected (Checker.getCompletionInfo source)

let private settablePlain = """
type A0() = member val Settable0 = 1 with get,set
type A() = 
   member val Settable = 1 with get,set
   member val NonSettable = 1
   static member Run(): A0 =  Unchecked.defaultof<_>
   static member Run(a: string): A =  Unchecked.defaultof<_>
"""

let private settableGeneric = """
type A0() = member val Settable0 = 1 with get,set
type A() = 
   member val Settable = 1 with get,set
   member val NonSettable = 1
   static member Run<'T>(): A0 = Unchecked.defaultof<_>
   static member Run(a: int): A = Unchecked.defaultof<_>
"""

let settableReturnCases: obj[] seq =
    [
        [| box (settablePlain + "A.Run({caret})"); box [ "Settable"; "Settable0" ] |]
        [| box (settablePlain + "A.Run(S{caret} = 1)"); box [ "Settable"; "Settable0" ] |]
        [| box (settablePlain + "A.Run(S = 1,{caret})"); box [ "Settable"; "Settable0" ] |]
        [| box (settablePlain + "A.Run(Settable = 1,{caret})"); box [ "Settable0" ] |]

        [| box (settableGeneric + "A.Run({caret})"); box [ "Settable"; "Settable0" ] |]
        [| box (settableGeneric + "A.Run(S{caret} = 1)"); box [ "Settable"; "Settable0" ] |]
        [| box (settableGeneric + "A.Run(S = 1,{caret})"); box [ "Settable"; "Settable0" ] |]
        [| box (settableGeneric + "A.Run(Settable = 1,{caret})"); box [ "Settable0" ] |]
        [| box (settableGeneric + "A.Run<_>({caret})"); box [ "Settable"; "Settable0" ] |]
        [| box (settableGeneric + "A.Run<_>(S{caret} = 1)"); box [ "Settable"; "Settable0" ] |]
        [| box (settableGeneric + "A.Run<_>(S = 1,{caret})"); box [ "Settable"; "Settable0" ] |]
        [| box (settableGeneric + "A.Run<_>(Settable = 1,{caret})"); box [ "Settable0" ] |]
    ]

[<Theory; MemberData(nameof settableReturnCases)>]
let ``ObjectInitializer.CompletionForSettablePropertiesInReturnValue`` (source: string) (included: string list) =
    let info = Checker.getCompletionInfo source
    assertHasItemWithNames included info
    assertHasNoItemsWithNames [ "NonSettable" ] info
