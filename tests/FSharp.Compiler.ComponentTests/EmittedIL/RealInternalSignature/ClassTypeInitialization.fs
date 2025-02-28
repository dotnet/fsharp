// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace EmittedIL.RealInternalSignature

open Xunit
open FSharp.Test
open FSharp.Test.Compiler
open System.IO

module ClassTypeInitialization =

    let withRealInternalSignature realSig compilation =
        compilation
        |> withOptions [if realSig then "--realsig+" else "--realsig-" ]

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``Simple types in namespace`` (realSig) =

        FSharp """
namespace MyLibrary
type MyFirstType =
    static let x1 = 1100 + System.Random().Next(0)
    static let _ = printfn "Hello, World from MyLibrary.MyFirstType"

type MySecondType =
    static let x2 = 2100 + System.Random().Next(0)
    static let _ = printfn "Hello, World from MyLibrary.MySecondType"

module MyModule =
    [<EntryPoint>]
    let main args =
        printfn "Hello from main method"
        0
        """
        |> withName "SimpleTypesInNamespace"
        |> withRealInternalSignature realSig
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContainsAllInOrder [
            "Hello, World from MyLibrary.MyFirstType"
            "Hello, World from MyLibrary.MySecondType"
            "Hello from main method"
        ]
        |> verifyPEFileWithSystemDlls
        |> withOutputContainsAllInOrderWithWildcards [
            "All Classes and Methods in*SimpleTypesInNamespace.exe Verified."
            ]



    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``Simple types in implicit main`` (realSig) =

        FSharp """
type MyFirstType =
    static let x1 = 1100 + System.Random().Next(0)
    static let _ = printfn "Hello, World from MyProgram.MyFirstType"

type MySecondType =
    static let x2 = 2100 + System.Random().Next(0)
    static let _ = printfn "Hello, World from MyProgram.MySecondType"

printfn "Hello from implicit main method"
        """
        |> withName "SimpleTypesInImplicitMain"
        |> withRealInternalSignature realSig
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContainsAllInOrder [
            "Hello, World from MyProgram.MyFirstType"
            "Hello, World from MyProgram.MySecondType"
            "Hello from implicit main method"
        ]
        |> verifyPEFileWithSystemDlls
        |> withOutputContainsAllInOrderWithWildcards [
            "All Classes and Methods in*SimpleTypesInImplicitMain.exe Verified."
            ]


    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``Simple TypeOne and TypeTwo in namespace`` (realSig) =

        FSharp """
namespace MyLibrary
type MyFirstType =
    static let x1 = 1100 + System.Random().Next(0)
    static let _ = printfn "Hello, World from MyLibrary.MyFirstType"

and MySecondType =
    static let x2 = 2100 + System.Random().Next(0)
    static let _ = printfn "Hello, World from MyLibrary.MySecondType"

module MyModule =
    [<EntryPoint>]
    let main args =
        printfn "Hello from main method"
        0
        """
        |> withName "SimpleTypeOneAndTypeTwoInNamespace"
        |> withRealInternalSignature realSig
        |> compileExeAndRun
        |> withStdOutContainsAllInOrder [
            "Hello, World from MyLibrary.MyFirstType"
            "Hello, World from MyLibrary.MySecondType"
            "Hello from main method"
        ]
        |> shouldSucceed
        |> verifyPEFileWithSystemDlls
        |> withOutputContainsAllInOrderWithWildcards [
            "All Classes and Methods in*SimpleTypeOneAndTypeTwoInNamespace.exe Verified."
            ]

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``Public type - public ctor`` (realSig) =

        FSharp """
namespace FSharp.Compiler.CodeAnalysis

open System
open System.IO


[<AbstractClass>]
type FSharpSource () =
    abstract FilePath: string

type FSharpSourceFromFile public (filePath: string) =
    inherit FSharpSource()

    override _.FilePath = filePath

type FSharpSource with

    static member CreateFromFile (filePath: string) =
        ()  //FSharpSourceFromFile(filePath) :> FSharpSource

module doit =
    FSharpSource.CreateFromFile("Hello") |> ignore
    printfn "Main program"
        """
        |> withName "PublicTypePublicCtor"
        |> withRealInternalSignature realSig
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContainsAllInOrder [
            "Main program"
        ]
        |> verifyPEFileWithSystemDlls
        |> withOutputContainsAllInOrderWithWildcards [
            "All Classes and Methods in*PublicTypePublicCtor.exe Verified."
            ]


    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``Public type - internal ctor`` (realSig) =

        FSharp """
namespace FSharp.Compiler.CodeAnalysis

open System
open System.IO


[<AbstractClass>]
type FSharpSource () =
    abstract FilePath: string

type FSharpSourceFromFile internal (filePath: string) =
    inherit FSharpSource()

    override _.FilePath = filePath

type FSharpSource with

    static member CreateFromFile(filePath: string) =
        FSharpSourceFromFile(filePath) :> FSharpSource

module doit =
    FSharpSource.CreateFromFile("Hello") |> ignore
    printfn "Main program"
        """
        |> withName "PublicTypeInternalCtor"
        |> withRealInternalSignature realSig
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContainsAllInOrder [
            "Main program"
        ]
        |> verifyPEFileWithSystemDlls
        |> withOutputContainsAllInOrderWithWildcards [
            "All Classes and Methods in*PublicTypeInternalCtor.exe Verified."
            ]


    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``Public type - private ctor`` (realSig) =

        FSharp """
namespace FSharp.Compiler.CodeAnalysis

open System
open System.IO


[<AbstractClass>]
type FSharpSource () =
    abstract FilePath: string

type FSharpSourceFromFile private (filePath: string) =
    inherit FSharpSource()

    override _.FilePath = filePath

type FSharpSource with

    static member CreateFromFile (filePath: string) =
        ()  //FSharpSourceFromFile(filePath) :> FSharpSource

module doit =
    FSharpSource.CreateFromFile("Hello") |> ignore
    printfn "Main program"
        """
        |> withName "PublicTypePrivateCtor"
        |> withRealInternalSignature realSig
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContainsAllInOrder [
            "Main program"
        ]
        |> verifyPEFileWithSystemDlls
        |> withOutputContainsAllInOrderWithWildcards [
            "All Classes and Methods in*PublicTypePrivateCtor.exe Verified."
            ]

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``Public type - unspecified ctor`` (realSig) =

        FSharp """
namespace FSharp.Compiler.CodeAnalysis

open System
open System.IO


[<AbstractClass>]
type FSharpSource () =
    abstract FilePath: string

type FSharpSourceFromFile (filePath: string) =
    inherit FSharpSource()

    override _.FilePath = filePath

type FSharpSource with

    static member CreateFromFile (filePath: string) =
        ()  //FSharpSourceFromFile(filePath) :> FSharpSource

module doit =
    FSharpSource.CreateFromFile("Hello") |> ignore
    printfn "Main program"
        """
        |> withName "PublicTypeUnspecifiedCtor"
        |> withRealInternalSignature realSig
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContainsAllInOrder [
            "Main program"
        ]
        |> verifyPEFileWithSystemDlls
        |> withOutputContainsAllInOrderWithWildcards [
            "All Classes and Methods in*PublicTypeUnspecifiedCtor.exe Verified."
            ]

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``Private type - public ctor`` (realSig) =

        FSharp """
namespace FSharp.Compiler.CodeAnalysis

open System
open System.IO


[<AbstractClass>]
type FSharpSource () =
    abstract FilePath: string

type private FSharpSourceFromFile public (filePath: string) =
    inherit FSharpSource()

    override _.FilePath = filePath

type FSharpSource with

    static member CreateFromFile (filePath: string) =
        ()  //FSharpSourceFromFile(filePath) :> FSharpSource

module doit =
    FSharpSource.CreateFromFile("Hello") |> ignore
    printfn "Main program"
        """
        |> withName "PrivateTypePublicCtor"
        |> withRealInternalSignature realSig
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContainsAllInOrder [
            "Main program"
        ]
        |> verifyPEFileWithSystemDlls
        |> withOutputContainsAllInOrderWithWildcards [
            "All Classes and Methods in*PrivateTypePublicCtor.exe Verified."
            ]

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``Private type - internal ctor`` (realSig) =

        FSharp """
namespace FSharp.Compiler.CodeAnalysis

open System
open System.IO


[<AbstractClass>]
type FSharpSource () =
    abstract FilePath: string

type private FSharpSourceFromFile internal (filePath: string) =
    inherit FSharpSource()

    override _.FilePath = filePath

type FSharpSource with

    static member CreateFromFile(filePath: string) =
        FSharpSourceFromFile(filePath) :> FSharpSource

module doit =
    FSharpSource.CreateFromFile("Hello") |> ignore
    printfn "Main program"
        """
        |> withName "PrivateTypeInternalCtor"
        |> withRealInternalSignature realSig
        |> compileExeAndRun
        |> withStdOutContainsAllInOrder [
            "Main program"
        ]
        |> shouldSucceed
        |> verifyPEFileWithSystemDlls
        |> withOutputContainsAllInOrderWithWildcards [
            "All Classes and Methods in*PrivateTypeInternalCtor.exe Verified."
            ]

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``Private type - private ctor`` (realSig) =

        FSharp """
namespace FSharp.Compiler.CodeAnalysis

open System
open System.IO


[<AbstractClass>]
type public FSharpSourcePublic () =
    abstract PublicFilePath: string

[<AbstractClass>]
type internal FSharpSource () =
    inherit FSharpSourcePublic()
    abstract InternalFilePath: string

type private FSharpSourceFromFile private (filePath: string) =
    inherit FSharpSource()

    override _.PublicFilePath = filePath
    override _.InternalFilePath = filePath
    member   _.PrivateFilePath = filePath

type FSharpSource with

    static member public PublicCreateFromFile (filePath: string) = ()
    static member internal InternalCreateFromFile (filePath: string) = ()
    static member private PrivateCreateFromFile (filePath: string) = ()

module doit =
    printfn "Main program"
        """
        |> withName "PrivateTypePrivateCtor"
        |> withRealInternalSignature realSig
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContainsAllInOrder [
            "Main program"
        ]
        |> verifyPEFileWithSystemDlls
        |> withOutputContainsAllInOrderWithWildcards [
            "All Classes and Methods in*PrivateTypePrivateCtor.exe Verified."
            ]

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``Private type - unspecified ctor`` (realSig) =

        FSharp """
namespace FSharp.Compiler.CodeAnalysis

open System
open System.IO


[<AbstractClass>]
type FSharpSource () =
    abstract FilePath: string

type private FSharpSourceFromFile (filePath: string) =
    inherit FSharpSource()

    override _.FilePath = filePath

type FSharpSource with

    static member CreateFromFile (filePath: string) =
        FSharpSourceFromFile(filePath) :> FSharpSource

module doit =
    FSharpSource.CreateFromFile("Hello") |> ignore
    printfn "Main program"
        """
        |> withName "PrivateTypeUnspecifiedCtor"
        |> withRealInternalSignature realSig
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContainsAllInOrder [
            "Main program"
        ]
        |> verifyPEFileWithSystemDlls
        |> withOutputContainsAllInOrderWithWildcards [
            "All Classes and Methods in*PrivateTypeUnspecifiedCtor.exe Verified."
            ]

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``Static Initialization - don't inline when method includes a private field`` (realSig) =

        FSharp """
module FSharp.Compiler.CodeAnalysis

open System
open System.IO

[<AbstractClass>]
type FSharpSource () =
    abstract FilePath: string

type public FSharpSourceFromFile private (filePath: string) =
    inherit FSharpSource()

    static let mutable someValue = "not set yet"

    override _.FilePath = filePath

    static member private SetSomeValue (value: string): string =
        someValue <- value
        someValue

    static member public SetIt (value: string) : string =
        FSharpSourceFromFile.SetSomeValue (value)

let message = FSharpSourceFromFile.SetIt ("Here is something")

printfn $"{message}"
        """
        |> withName "StaticInitializationNoInlinePrivateField"
        |> withRealInternalSignature realSig
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContainsAllInOrder [
            "Here is something"
        ]
        |> verifyPEFileWithSystemDlls
        |> withOutputContainsAllInOrderWithWildcards [
            "All Classes and Methods in*StaticInitializationNoInlinePrivateField.exe Verified."
            ]

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``Computation expression - access private binding`` (realSig) =

        FSharp """
module MyMaybeStuff =
    type MaybeBuilder() =
        member this.Bind(x, f) =
            match x with
            | Some v -> f v
            | None -> None
        member this.Return(x) = Some x

    let maybe = new MaybeBuilder()

open MyMaybeStuff
type MyClass =
    static member private privateThing(someValue) = someValue

    static member result () =
        maybe {
            let mutable someValue = 0
            let! u = someValue <- 1; Some (MyClass.privateThing(someValue))
            let! v = someValue <- 2; Some (MyClass.privateThing(someValue))
            return u + v
        }

printfn "%A" (MyClass.result())
"""
        |> withName "ComputationExpressionAccessPrivateBinding"
        |> withRealInternalSignature realSig
        |> withNoOptimize
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContainsAllInOrder [
            "Some 3"
        ]
        |> verifyPEFileWithSystemDlls
        |> withOutputContainsAllInOrderWithWildcards [
            "All Classes and Methods in*ComputationExpressionAccessPrivateBinding.exe Verified."
            ]

    [<InlineData(true, true)>]          // RealSig Optimize
    [<InlineData(true, false)>]         // RealSig NoOptimize
    [<InlineData(false, true)>]         // Regular Optimize
    [<InlineData(false, false)>]        // Regular NoOptimize
    [<Theory>]
    let ``nested generic closure`` (realSig, optimize) =
        let path = __SOURCE_DIRECTORY__ ++ "nested_generic_closure.fs"
        let source = File.ReadAllText (path)

        FSharp source
        |> withName "NestedGenericClosure"
        |> asExe
        |> withRealInternalSignature realSig
        |> withOptimization optimize
        |> compileAndRun
        |> shouldSucceed
        |> verifyPEFileWithSystemDlls
        |> withOutputContainsAllInOrderWithWildcards [
            "All Classes and Methods in*NestedGenericClosure.exe Verified."
            ]

    let ``Generic nested class with closure - source`` =

        FSharp """
module RuntimeHelpers =
    type MyType<'A,'B when 'B :> seq<'A>>(sources: seq<'B>) =
        member x.MoveNext() =
            let rec takeInner c =
                if c.ToString() = "1" then failwith "Oops"
                sources
            takeInner 3

module doIt =
    open RuntimeHelpers

    let x = seq { seq { 1uy } }
    let enumerator = x |> MyType<_,_>
    enumerator.MoveNext() |> ignore
    """

    [<InlineData(true, false)>]         // RealSig NoOptimize
    [<InlineData(false, true)>]         // Regular Optimize
    [<InlineData(false, false)>]        // Regular NoOptimize
    [<Theory>]
    let ``Generic nested class with closure`` (realSig, optimize) =

        ``Generic nested class with closure - source``
        |> withName "GenericClassWithClosureWithConstraints"
        |> asExe
        |> withRealInternalSignature realSig
        |> withOptimization optimize
        |> compileAndRun
        |> shouldSucceed
        |> verifyPEFileWithSystemDlls
        |> withOutputContainsAllInOrderWithWildcards [
            "All Classes and Methods in*GenericClassWithClosureWithConstraints.exe Verified."
            ]

    [<InlineData(true, true)>]          // RealSig Optimize
    [<Theory>]
    let ``Generic nested class with closure optimized`` (realSig, optimize) =

        ``Generic nested class with closure - source``
        |> withName "GenericStructWithClosureWithConstraints"
        |> asExe
        |> withRealInternalSignature realSig
        |> withOptimization optimize
        |> compileAndRun
        |> shouldSucceed
        |> verifyPEFileWithSystemDlls
        |> withOutputContainsAllInOrderWithRegexPatterns [
            @"Verifying \[GenericStructWithClosureWithConstraints\]MyType`2\.MoveNext"
            @"\[IL\]: Error \[StackUnexpected\]: \[.*? : .*?::MoveNext\(\)\]\[offset 0x[0-9A-Fa-f]+\]\[found \w+\]\[expected value '.*?'\] Unexpected type on the stack\."
            @"1 Error\(s\) Verifying .*\\|/GenericStructWithClosureWithConstraints\.exe"
            ]

    let ``Generic nested class with interface implemented and closure - source`` =
        FSharp """
module RuntimeHelpers =
    open System
    open System.Collections
    open System.Collections.Generic

    type MyType<'A, 'B when 'B :> seq<'A>>(_sources: seq<'B>) =

        let mutable v:'B = Unchecked.defaultof<'B>

        interface IEnumerator<'B> with
             member _.Current = v

        interface IEnumerator with
            member _.Current = box v

            member _.MoveNext() =
                let rec takeInner c =
                    if c.ToString() = "1" then failwith "Oops"
                    true

                takeInner 3

            member _.Reset() = ()

        interface IDisposable with
            member _.Dispose() = ()

#nowarn 760
module doIt =
    open RuntimeHelpers
    open System.Collections.Generic

    let x = seq { seq { 1uy } }
    let enumerator = x |> MyType<_,_> :> IEnumerator<_>
    enumerator.MoveNext() |> ignore"""

    [<InlineData(true, false)>]         // RealSig NoOptimize
    [<InlineData(false, true)>]         // Regular Optimize
    [<InlineData(false, false)>]        // Regular NoOptimize
    [<Theory>]
    let ``Generic nested class with interface implemented and closure`` (realSig, optimize) =

        ``Generic nested class with interface implemented and closure - source``
        |> withName "GenericClassWithInterfaceAndClosure"
        |> asExe
        |> withRealInternalSignature realSig
        |> withOptimization optimize
        |> compileAndRun
        |> shouldSucceed
        |> verifyPEFileWithSystemDlls
        |> withOutputContainsAllInOrderWithWildcards [
            "All Classes and Methods in*GenericClassWithInterfaceAndClosure.exe Verified."
            ]

    [<InlineData(true, true)>]          // RealSig Optimize
    [<Theory>]
    let ``Generic nested class with interface implemented and closure optimized`` (realSig, optimize) =

        ``Generic nested class with interface implemented and closure - source``
        |> withName "GenericClassWithInterfaceAndClosure"
        |> asExe
        |> withRealInternalSignature realSig
        |> withOptimization optimize
        |> compileAndRun
        |> shouldSucceed
        |> verifyPEFileWithSystemDlls
        |> withOutputContainsAllInOrderWithRegexPatterns [
            @"Verifying \[GenericClassWithInterfaceAndClosure\]MyType`2\.System\.Collections\.IEnumerator\.MoveNext"
            @"\[IL\]: Error \[StackUnexpected\]: \[(?:[a-zA-Z]:\\|/)?[^:]+ : Test\+RuntimeHelpers\+MyType`2::System\.Collections\.IEnumerator\.MoveNext\(\)\]\[offset 0x00000001\]\[found Int32\]\[expected value 'A'\] Unexpected type on the stack\."
            @"Verifying \[GenericClassWithInterfaceAndClosure\]MyType`2\.System\.Collections\.IEnumerator\.Reset"
            @"1 Error\(s\) Verifying .*\\|/GenericClassWithInterfaceAndClosure\.exe"
            ]

    [<InlineData(true, false)>]         // RealSig NoOptimize
    [<InlineData(false, true)>]         // Regular Optimize
    [<InlineData(false, false)>]        // Regular NoOptimize
    [<Theory>]
    let ``AgedLookup`` (realSig, optimize) =

        FSharp """
namespace Internal.Utilities.Collections

open System

[<StructuralEquality; NoComparison>]
type internal ValueStrength<'T when 'T: not struct> =
    | Strong of 'T
    | Weak of WeakReference<'T>

type internal AgedLookup<'Token, 'Key, 'Value when 'Value: not struct>(keepStrongly: int, areSimilar, ?requiredToKeep, ?keepMax: int) =
    /// The list of items stored. Youngest is at the end of the list.
    /// The choice of order is somewhat arbitrary. If the other way then adding
    /// items would be O(1) and removing O(N).
    let mutable refs: ('Key * ValueStrength<'Value>) list = []

    let mutable keepStrongly = keepStrongly

    // The 75 here determines how long the list should be passed the end of strongly held
    // references. Some operations are O(N) and we don't want to let things get out of
    // hand.
    let keepMax = defaultArg keepMax 75
    let mutable keepMax = max keepStrongly keepMax
    let requiredToKeep = defaultArg requiredToKeep (fun _ -> false)

    /// Look up the given key, return <c>None</c> if not found.
    let TryPeekKeyValueImpl (data, key) =
        let rec Lookup key =
            function
            // Treat a list of key-value pairs as a lookup collection.
            // This function returns true if two keys are the same according to the predicate
            // function passed in.
            | [] -> None
            | (similarKey, value) :: t ->
                if areSimilar (key, similarKey) then
                    Some(similarKey, value)
                else
                    Lookup key t

        Lookup key data

    /// Determines whether a particular key exists.
    let Exists (data, key) = TryPeekKeyValueImpl(data, key).IsSome

    /// Set a particular key's value.
    let Add (data, key, value) = data @ [ key, value ]

    /// Promote a particular key value.
    let Promote (data, key, value) =
        (data |> List.filter (fun (similarKey, _) -> not (areSimilar (key, similarKey))))
        @ [ (key, value) ]

    /// Remove a particular key value.
    let RemoveImpl (data, key) =
        let keep =
            data |> List.filter (fun (similarKey, _) -> not (areSimilar (key, similarKey)))

        keep

    let TryGetKeyValueImpl (data, key) =
        match TryPeekKeyValueImpl(data, key) with
        | Some(similarKey, value) as result ->
            // If the result existed, move it to the end of the list (more likely to keep it)
            result, Promote(data, similarKey, value)
        | None -> None, data

    /// Remove weak entries from the list that have been collected.
    let FilterAndHold (tok: 'Token) =
        ignore tok // reading 'refs' requires a token

        [
            for key, value in refs do
                match value with
                | Strong(value) -> yield (key, value)
                | Weak(weakReference) ->
                    match weakReference.TryGetTarget() with
                    | false, _ -> ()
                    | true, value -> yield key, value
        ]

    let AssignWithStrength (tok, newData) =
        let actualLength = List.length newData
        let tossThreshold = max 0 (actualLength - keepMax) // Delete everything less than this threshold
        let weakThreshold = max 0 (actualLength - keepStrongly) // Weaken everything less than this threshold

        let newData = newData |> List.mapi (fun n kv -> n, kv) // Place the index.

        let newData =
            newData
            |> List.filter (fun (n: int, v) -> n >= tossThreshold || requiredToKeep (snd v))

        let newData =
            newData
            |> List.map (fun (n: int, (k, v)) ->
                let handle =
                    if n < weakThreshold && not (requiredToKeep v) then
                        Weak(WeakReference<_>(v))
                    else
                        Strong(v)

                k, handle)

        ignore tok // Updating refs requires tok
        refs <- newData

    member al.TryPeekKeyValue(tok, key) =
        // Returns the original key value as well since it may be different depending on equality test.
        let data = FilterAndHold(tok)
        TryPeekKeyValueImpl(data, key)

    member al.TryGetKeyValue(tok, key) =
        let data = FilterAndHold(tok)
        let result, newData = TryGetKeyValueImpl(data, key)
        AssignWithStrength(tok, newData)
        result

    member al.TryGet(tok, key) =
        let data = FilterAndHold(tok)
        let result, newData = TryGetKeyValueImpl(data, key)
        AssignWithStrength(tok, newData)

        match result with
        | Some(_, value) -> Some(value)
        | None -> None

    member al.Put(tok, key, value) =
        let data = FilterAndHold(tok)

        let data = if Exists(data, key) then RemoveImpl(data, key) else data

        let data = Add(data, key, value)
        AssignWithStrength(tok, data) // This will remove extras

    member al.Remove(tok, key) =
        let data = FilterAndHold(tok)
        let newData = RemoveImpl(data, key)
        AssignWithStrength(tok, newData)

    member al.Clear(tok) =
        let _discards = FilterAndHold(tok)
        AssignWithStrength(tok, [])

    member al.Resize(tok, newKeepStrongly, ?newKeepMax) =
        let newKeepMax = defaultArg newKeepMax 75
        keepStrongly <- newKeepStrongly
        keepMax <- max newKeepStrongly newKeepMax
        let keep = FilterAndHold(tok)
        AssignWithStrength(tok, keep)
    """
        |> withName "AgedLookup"
        |> asLibrary
        |> withRealInternalSignature realSig
        |> withOptimization optimize
        |> compile
        |> shouldSucceed
        |> verifyPEFileWithSystemDlls
        |> withOutputContainsAllInOrderWithRegexPatterns [@"All Classes and Methods in .*AgedLookup\.dll Verified."]

    [<InlineData(true, true)>]          // RealSig Optimize
    [<Theory>]
    let ``AgedLookupEx`` (realSig, optimize) =

        FSharp """
namespace Internal.Utilities.Collections

open System

[<StructuralEquality; NoComparison>]
type internal ValueStrength<'T when 'T: not struct> =
    | Strong of 'T
    | Weak of WeakReference<'T>

type internal AgedLookup<'Token, 'Key, 'Value when 'Value: not struct>(keepStrongly: int, areSimilar, ?requiredToKeep, ?keepMax: int) =
    /// The list of items stored. Youngest is at the end of the list.
    /// The choice of order is somewhat arbitrary. If the other way then adding
    /// items would be O(1) and removing O(N).
    let mutable refs: ('Key * ValueStrength<'Value>) list = []

    let mutable keepStrongly = keepStrongly

    // The 75 here determines how long the list should be passed the end of strongly held
    // references. Some operations are O(N) and we don't want to let things get out of
    // hand.
    let keepMax = defaultArg keepMax 75
    let mutable keepMax = max keepStrongly keepMax
    let requiredToKeep = defaultArg requiredToKeep (fun _ -> false)

    /// Look up the given key, return <c>None</c> if not found.
    let TryPeekKeyValueImpl (data, key) =
        let rec Lookup key =
            function
            // Treat a list of key-value pairs as a lookup collection.
            // This function returns true if two keys are the same according to the predicate
            // function passed in.
            | [] -> None
            | (similarKey, value) :: t ->
                if areSimilar (key, similarKey) then
                    Some(similarKey, value)
                else
                    Lookup key t

        Lookup key data

    /// Determines whether a particular key exists.
    let Exists (data, key) = TryPeekKeyValueImpl(data, key).IsSome

    /// Set a particular key's value.
    let Add (data, key, value) = data @ [ key, value ]

    /// Promote a particular key value.
    let Promote (data, key, value) =
        (data |> List.filter (fun (similarKey, _) -> not (areSimilar (key, similarKey))))
        @ [ (key, value) ]

    /// Remove a particular key value.
    let RemoveImpl (data, key) =
        let keep =
            data |> List.filter (fun (similarKey, _) -> not (areSimilar (key, similarKey)))

        keep

    let TryGetKeyValueImpl (data, key) =
        match TryPeekKeyValueImpl(data, key) with
        | Some(similarKey, value) as result ->
            // If the result existed, move it to the end of the list (more likely to keep it)
            result, Promote(data, similarKey, value)
        | None -> None, data

    /// Remove weak entries from the list that have been collected.
    let FilterAndHold (tok: 'Token) =
        ignore tok // reading 'refs' requires a token

        [
            for key, value in refs do
                match value with
                | Strong(value) -> yield (key, value)
                | Weak(weakReference) ->
                    match weakReference.TryGetTarget() with
                    | false, _ -> ()
                    | true, value -> yield key, value
        ]

    let AssignWithStrength (tok, newData) =
        let actualLength = List.length newData
        let tossThreshold = max 0 (actualLength - keepMax) // Delete everything less than this threshold
        let weakThreshold = max 0 (actualLength - keepStrongly) // Weaken everything less than this threshold

        let newData = newData |> List.mapi (fun n kv -> n, kv) // Place the index.

        let newData =
            newData
            |> List.filter (fun (n: int, v) -> n >= tossThreshold || requiredToKeep (snd v))

        let newData =
            newData
            |> List.map (fun (n: int, (k, v)) ->
                let handle =
                    if n < weakThreshold && not (requiredToKeep v) then
                        Weak(WeakReference<_>(v))
                    else
                        Strong(v)

                k, handle)

        ignore tok // Updating refs requires tok
        refs <- newData

    member al.TryPeekKeyValue(tok, key) =
        // Returns the original key value as well since it may be different depending on equality test.
        let data = FilterAndHold(tok)
        TryPeekKeyValueImpl(data, key)

    member al.TryGetKeyValue(tok, key) =
        let data = FilterAndHold(tok)
        let result, newData = TryGetKeyValueImpl(data, key)
        AssignWithStrength(tok, newData)
        result

    member al.TryGet(tok, key) =
        let data = FilterAndHold(tok)
        let result, newData = TryGetKeyValueImpl(data, key)
        AssignWithStrength(tok, newData)

        match result with
        | Some(_, value) -> Some(value)
        | None -> None

    member al.Put(tok, key, value) =
        let data = FilterAndHold(tok)

        let data = if Exists(data, key) then RemoveImpl(data, key) else data

        let data = Add(data, key, value)
        AssignWithStrength(tok, data) // This will remove extras

    member al.Remove(tok, key) =
        let data = FilterAndHold(tok)
        let newData = RemoveImpl(data, key)
        AssignWithStrength(tok, newData)

    member al.Clear(tok) =
        let _discards = FilterAndHold(tok)
        AssignWithStrength(tok, [])

    member al.Resize(tok, newKeepStrongly, ?newKeepMax) =
        let newKeepMax = defaultArg newKeepMax 75
        keepStrongly <- newKeepStrongly
        keepMax <- max newKeepStrongly newKeepMax
        let keep = FilterAndHold(tok)
        AssignWithStrength(tok, keep)
    """
        |> withName "AgedLookup"
        |> asLibrary
        |> withRealInternalSignature realSig
        |> withOptimization optimize
        |> compile
        |> shouldSucceed
        |> verifyPEFileWithSystemDlls
        |> withOutputContainsAllInOrderWithRegexPatterns [
            @"\[IL\]: Error \[StackUnexpected\]: \[.* : Internal\.Utilities\.Collections\.AgedLookup`3::TryPeekKeyValueImpl\(\[.*\].*\)\].* Unexpected type on the stack\."//\[offset 0x[0-9A-Fa-f]+\]\[found ref '\[.*\]'\]\[expected ref '\[.*\]'\] Unexpected type on the stack\."
            @"\[IL\]: Error \[StackUnexpected\]: \[.* : Internal\.Utilities\.Collections\.AgedLookup`3::TryPeekKeyValueImpl\(\[.*\].*\)\].* Unexpected type on the stack\."//\[offset 0x[0-9A-Fa-f]+\]\[found ref '\[.*\]'\]\[expected ref '\[.*\]'\] Unexpected type on the stack\."
            @"2 Error\(s\) Verifying .*\\|/AgedLookup\.dll"
            ]

    [<InlineData(true, true)>]          // RealSig Optimize
    [<InlineData(true, false)>]         // RealSig NoOptimize
    [<InlineData(false, true)>]         // Regular Optimize
    [<InlineData(false, false)>]        // Regular NoOptimize
    [<Theory>]
    let ``BigTuples`` (realSig, optimize) =

        FSharp """
namespace Equality

type BigGenericTuple<'a> = BigGenericTuple of int * 'a * byte * int * 'a * byte
    """
        |> withName "BigTuples"
        |> asLibrary
        |> withRealInternalSignature realSig
        |> withOptimization optimize
        |> compile
        |> shouldSucceed
        |> verifyPEFileWithSystemDlls
        |> withOutputContainsAllInOrderWithWildcards [
            "All Classes and Methods in*BigTuples.dll Verified."
            ]

    [<InlineData(true, true)>]          // RealSig Optimize
    [<InlineData(true, false)>]         // RealSig NoOptimize
    [<InlineData(false, true)>]         // Regular Optimize
    [<InlineData(false, false)>]        // Regular NoOptimize
    [<Theory>]
    let ``Array groupBy id`` (realSig, optimize) =

        FSharp """
module GroupByTest
let ``for _ in Array groupBy id [||] do ...`` () = [|for _ in Array.groupBy id [||] do 0|]
    """
        |> withName "ArrayGroupById"
        |> asLibrary
        |> withRealInternalSignature realSig
        |> withOptimization optimize
        |> compile
        |> shouldSucceed
        |> verifyPEFileWithSystemDlls
        |> withOutputContainsAllInOrderWithWildcards [
            "All Classes and Methods in*ArrayGroupById.dll Verified."
            ]

    let roundTripWithInterfaceGeneration(realsig, optimize, implementationFile, name) =

        let generatedSignature =
            Fs implementationFile
            |> withRealInternalSignature realsig
            |> withOptimization optimize
            |> printSignatures

        Fsi generatedSignature
        |> withName name
        |> asLibrary
        |> withAdditionalSourceFile (FsSource implementationFile)
        |> withRealInternalSignature realsig
        |> withOptimization optimize
        |> compile
        |> shouldSucceed

    [<InlineData(true, true)>]          // RealSig Optimize
    [<InlineData(true, false)>]         // RealSig NoOptimize
    [<InlineData(false, true)>]         // Regular Optimize
    [<InlineData(false, false)>]        // Regular NoOptimize
    [<Theory>]
    let ``generic-parameter-order-roundtrip`` (realsig, optimize) =

        let implementationFile = """
module OrderMatters

type IMonad<'a> =
    interface
        // Hash constraint leads to another type parameter
        abstract bind : #IMonad<'a> -> ('a -> #IMonad<'b>) -> IMonad<'b>
    end"""

        roundTripWithInterfaceGeneration(realsig, optimize, implementationFile, "GenericParameterOrderRoundtrip")
        |> verifyPEFileWithSystemDlls
        |> withOutputContainsAllInOrderWithWildcards [
            "All Classes and Methods in*GenericParameterOrderRoundtrip.dll Verified."
            ]

    [<InlineData(true, true)>]          // RealSig Optimize
    [<InlineData(true, false)>]         // RealSig NoOptimize
    [<InlineData(false, true)>]         // Regular Optimize
    [<InlineData(false, false)>]        // Regular NoOptimize
    [<Theory>]
    let ``members_basic-roundtrip`` (realsig, optimize) =

        let implementationFile = """
namespace GenericInterfaceTest

    type Foo<'a> =
      interface 
          abstract fun1 : 'a -> 'a
          abstract fun2 : int -> int
      end


    type Bar<'b> =
      class 
          val store : 'b
          interface Foo<'b> with
            member self.fun1(x) = x
            member self.fun2(x) = 1
          end
          new(x) = { store = x }
      end"""

        roundTripWithInterfaceGeneration(realsig, optimize, implementationFile, "MembersBasicRoundtrip")
        |> verifyPEFileWithSystemDlls
        |> withOutputContainsAllInOrderWithWildcards [
            "All Classes and Methods in*MembersBasicRoundtrip.dll Verified."
            ]

    [<InlineData(true, true)>]          // RealSig Optimize
    [<InlineData(true, false)>]         // RealSig NoOptimize
    [<InlineData(false, true)>]         // Regular Optimize
    [<InlineData(false, false)>]        // Regular NoOptimize
    [<Theory>]
    let ``UtilsStrings.SR`` (realSig, optimize) =

        FSharp """
// This is a generated file; the original input is 'Facilities\UtilsStrings.txt'
namespace UtilsStrings

open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators
open Microsoft.FSharp.Reflection
open System.Reflection

type internal SR private() =

    // BEGIN BOILERPLATE

    static let getCurrentAssembly () = System.Reflection.Assembly.GetExecutingAssembly()

    static let getTypeInfo (t: System.Type) = t

    static let resources = lazy (new System.Resources.ResourceManager("UtilsStrings", getCurrentAssembly()))

    static let GetString(name:string) =
        let s = resources.Value.GetString(name, System.Globalization.CultureInfo.CurrentUICulture)
        s

    static let mkFunctionValue (tys: System.Type[]) (impl:objnull->objnull) =
        FSharpValue.MakeFunction(FSharpType.MakeFunctionType(tys.[0],tys.[1]), impl)

    static let funTyC = typeof<(obj -> obj)>.GetGenericTypeDefinition()

    static let isNamedType(ty:System.Type) = not (ty.IsArray ||  ty.IsByRef ||  ty.IsPointer)
    static let isFunctionType (ty1:System.Type)  =
        isNamedType(ty1) && getTypeInfo(ty1).IsGenericType && System.Type.op_Equality(ty1.GetGenericTypeDefinition(), funTyC)

    static let rec destFunTy (ty:System.Type) =
        if isFunctionType ty then
            ty, ty.GetGenericArguments()
        else
            match getTypeInfo(ty).BaseType with
            | null -> failwith "destFunTy: not a function type"
            | b -> destFunTy b

    static let buildFunctionForOneArgPat (ty: System.Type) impl =
        let _,tys = destFunTy ty
        let rty = tys.[1]
        // PERF: this technique is a bit slow (e.g. in simple cases, like 'sprintf "%x"')
        mkFunctionValue tys (fun inp -> impl rty inp)

    static let capture1 (fmt:string) i args ty (go: objnull list -> System.Type -> int -> obj) : obj =
        match fmt.[i] with
        | '%' -> go args ty (i+1)
        | 'd'
        | 'f'
        | 's' -> buildFunctionForOneArgPat ty (fun rty n -> go (n :: args) rty (i+1))
        | _ -> failwith "bad format specifier"

    // newlines and tabs get converted to strings when read from a resource file
    // this will preserve their original intention
    static let postProcessString (s: string) =
        s.Replace("\\n","\n").Replace("\\t","\t").Replace("\\r","\r").Replace("\\\"", "\"")

    static let createMessageString (messageString: string) (fmt: Printf.StringFormat<'T>) : 'T =
        let fmt = fmt.Value // here, we use the actual error string, as opposed to the one stored as fmt
        let len = fmt.Length

        /// Function to capture the arguments and then run.
        let rec capture args ty i =
            if i >= len ||  (fmt.[i] = '%' && i+1 >= len) then
                let b = new System.Text.StringBuilder()
                b.AppendFormat(messageString, [| for x in List.rev args -> x |]) |> ignore
                box(b.ToString()) |> Unchecked.nonNull
            elif System.Char.IsSurrogatePair(fmt,i) then
                capture args ty (i+2)
            else
                match fmt.[i] with
                | '%' ->
                    let i = i+1
                    capture1 fmt i args ty capture
                | _ ->
                    capture args ty (i+1)

        (unbox (capture [] (typeof<'T>) 0) : 'T)
    // END BOILERPLATE
    """
        |> withName "UtilsStringsSR"
        |> asLibrary
        |> withRealInternalSignature realSig
        |> withOptimization optimize
        |> compile
        |> shouldSucceed
        |> verifyPEFileWithSystemDlls
        |> withOutputContainsAllInOrderWithWildcards [
            "All Classes and Methods in*UtilsStringsSR.dll Verified."
            ]

