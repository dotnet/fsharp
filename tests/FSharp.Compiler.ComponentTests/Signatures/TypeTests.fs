module Signatures.TypeTests

open FSharp.Compiler.Symbols
open Xunit
open FSharp.Test.Compiler
open Signatures.TestHelpers

[<Fact>]
let ``Recursive type with attribute`` () =
    FSharp
        """
namespace Foo.Types

open System.Collections.Generic

type FormatSelectionRequest =
    {
        SourceCode: string
        /// File path will be used to identify the .editorconfig options
        /// Unless the configuration is passed
        FilePath: string
        /// Overrides the found .editorconfig.
        Config: IReadOnlyDictionary<string, string> option
        /// Range follows the same semantics of the FSharp Compiler Range type.
        Range: FormatSelectionRange
    }

    member this.IsSignatureFile = this.FilePath.EndsWith(".fsi")

and FormatSelectionRange =
    struct
        val StartLine: int
        val StartColumn: int
        val EndLine: int
        val EndColumn: int

        new(startLine: int, startColumn: int, endLine: int, endColumn: int) =
            { StartLine = startLine
              StartColumn = startColumn
              EndLine = endLine
              EndColumn = endColumn }
    end
"""
    |> printSignatures
    |> prependNewline
    |> assertEqualIgnoreLineEnding
        """
namespace Foo.Types

  type FormatSelectionRequest =
    {
      SourceCode: string

      /// File path will be used to identify the .editorconfig options
      /// Unless the configuration is passed
      FilePath: string

      /// Overrides the found .editorconfig.
      Config: System.Collections.Generic.IReadOnlyDictionary<string,string> option

      /// Range follows the same semantics of the FSharp Compiler Range type.
      Range: FormatSelectionRange
    }

    member IsSignatureFile: bool

  and [<Struct>] FormatSelectionRange =

    new: startLine: int * startColumn: int * endLine: int * endColumn: int -> FormatSelectionRange

    val StartLine: int

    val StartColumn: int

    val EndLine: int

    val EndColumn: int"""

[<Fact>]
let ``Type extension uses type parameters names from source`` () =
    FSharp """
module Extensions

type List<'E> with

    member this.X = this.Head
"""
    |> printSignatures
    |> assertEqualIgnoreLineEnding
        """
module Extensions
type List<'E> with

  member X: 'E"""

[<Fact>]
let ``Type extension with constraints uses type parameters names from source`` () =
    FSharp """
module Extensions

type Map<'K, 'V when 'K: comparison> with

    member m.X (t: 'T) (k: 'K) = Some k, ({| n = [|k|] |}, 0)
"""
    |> printSignatures
    |> assertEqualIgnoreLineEnding
        """
module Extensions
type Map<'K,'V when 'K: comparison> with

  member X: t: 'T -> k: 'K -> 'K option * ({| n: 'K array |} * int) when 'K: comparison"""
 
[<Fact>]
let ``Type extension with lowercase type parameters names from source`` () =
    FSharp """
module Extensions

open System.Collections.Concurrent

type ConcurrentDictionary<'key, 'value> with

  member x.TryFind key =
    match x.TryGetValue key with
    | true, value -> Some value
    | _ -> None
"""
    |> printSignatures
    |> assertEqualIgnoreLineEnding
        """
module Extensions
type System.Collections.Concurrent.ConcurrentDictionary<'key,'value> with

  member TryFind: key: 'key -> 'value option"""

[<Fact>]
let ``Don't update typar name in type extension when TyparStaticReq doesn't match`` () =
    FSharp """        
module Extensions

type DataItem<'data> =
    { Identifier: string
      Label: string
      Data: 'data }

    static member Create<'data>(identifier: string, label: string, data: 'data) =
        { DataItem.Identifier = identifier
          DataItem.Label = label
          DataItem.Data = data }

#nowarn "957"

type DataItem< ^input> with

    static member inline Create(item: ^input) =
        let stringValue: string = (^input: (member get_StringValue: unit -> string) (item))

        let friendlyStringValue: string =
            (^input: (member get_FriendlyStringValue: unit -> string) (item))

        DataItem.Create< ^input>(stringValue, friendlyStringValue, item)
"""
    |> printSignatures
    |> assertEqualIgnoreLineEnding
        """
module Extensions

type DataItem<'data> =
  {
    Identifier: string
    Label: string
    Data: 'data
  }

  static member inline Create: item: ^input -> DataItem<^input> when ^input: (member get_StringValue: unit -> string) and ^input: (member get_FriendlyStringValue: unit -> string)

  static member Create<'data> : identifier: string * label: string * data: 'data -> DataItem<'data>"""

[<Fact>]
let ``ValText for C# abstract member override`` () =
    let csharp = CSharp """
namespace CSharp.Library
{
    using System;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Diagnostics.CodeAnalysis;
    
    public abstract class JsonConverter
    {
        public abstract void WriteJson(object writer, object? value, object serializer);
        public abstract object? ReadJson(object reader, Type objectType, object? existingValue, object serializer);
        public abstract bool CanConvert(Type objectType);
        public virtual bool CanRead => true;
        public virtual bool CanWrite => true;
    }
}
"""

    FSharp """
module F    

open CSharp.Library

[<Sealed>]
type EConverter () =
    inherit JsonConverter ()

    override this.WriteJson(writer, value, serializer) = failwith "todo"
    override this.ReadJson(reader, objectType, existingValue, serializer) = failwith "todo"
    override this.CanConvert(objectType) = failwith "todo"
    override this.CanRead = true
"""
    |> withReferences [ csharp ]
    |> typecheckResults
    |> fun results ->
        let writeJsonSymbolUse = results.GetSymbolUseAtLocation(10, 27, "    override this.WriteJson(writer, value, serializer) = failwith \"todo\"", [ "WriteJson" ]).Value
        match writeJsonSymbolUse.Symbol with
        | :? FSharpMemberOrFunctionOrValue as mfv ->
            let valText = mfv.GetValSignatureText(writeJsonSymbolUse.DisplayContext, writeJsonSymbolUse.Range).Value
            Assert.Equal("override WriteJson: writer: obj * value: obj * serializer: obj -> unit", valText)
        | _ -> ()

        let canReadSymbolUse = results.GetSymbolUseAtLocation(13, 25, "    override this.CanRead = true", [ "CanRead" ]).Value
        match canReadSymbolUse.Symbol with
        | :? FSharpMemberOrFunctionOrValue as mfv ->
            let valText = mfv.GetValSignatureText(canReadSymbolUse.DisplayContext, canReadSymbolUse.Range).Value
            Assert.Equal("override CanRead: bool", valText)
        | _ -> ()

[<Fact>]
let ``Property with unit getter and regular setter`` () =
    FSharp """
module Lib

type Foo =
  member x.Bar with get () = 5 and set v = ignore<int> v
"""
    |> printSignatures
    |> assertEqualIgnoreLineEnding
        """
module Lib

[<Class>]
type Foo =

  member Bar: int with get, set"""

[<Fact>]
let ``Property with indexed getter and regular setter`` () =
    FSharp """
module Lib

type Foo =
  member x.Bar with get (a:int) = 5 and set (a:int) v = ignore<int> v
"""
    |> printSignatures
    |> assertEqualIgnoreLineEnding
        """
module Lib

[<Class>]
type Foo =

  member Bar: a: int -> int with get

  member Bar: a: int -> int with set"""

[<Fact>]
let ``get_Is* method has IsUnionCaseTester = true`` () =
    FSharp """
module Lib

type Foo =
    | Bar of int
    | Baz of string
    member this.IsP
      with get () = 42

let bar = Bar 5

let f = bar.get_IsBar
"""
    |> withLangVersion10
    |> typecheckResults
    |> fun results ->
        let isBarSymbolUse = results.GetSymbolUseAtLocation(12, 21, "let f = bar.get_IsBar", [ "get_IsBar" ]).Value
        match isBarSymbolUse.Symbol with
        | :? FSharpMemberOrFunctionOrValue as mfv ->
            Assert.True(mfv.IsUnionCaseTester, "IsUnionCaseTester returned true")
            Assert.True(mfv.IsMethod, "IsMethod returned true")
            Assert.False(mfv.IsProperty, "IsProptery returned true")
            Assert.True(mfv.IsPropertyGetterMethod, "IsPropertyGetterMethod returned false")
        | _ -> failwith "Expected FSharpMemberOrFunctionOrValue"

[<Fact>]
let ``IsUnionCaseTester does not throw for non-method non-property`` () =
    FSharp """
module Lib

type Foo() =
    member _.Bar x = x

let foo = Foo()
"""
    |> withLangVersion10
    |> typecheckResults
    |> fun results ->
        let isBarSymbolUse = results.GetSymbolUseAtLocation(7, 13, "let foo = Foo()", [ "Foo" ]).Value
        match isBarSymbolUse.Symbol with
        | :? FSharpMemberOrFunctionOrValue as mfv ->
            Assert.False(mfv.IsUnionCaseTester, "IsUnionCaseTester returned true")
            Assert.True(mfv.IsConstructor)
        | _ -> failwith "Expected FSharpMemberOrFunctionOrValue"



[<Fact>]
let ``Type extension type parameters do not overwrite the type being extended`` () =
    let one =
        FSharpWithFileName "One.fs" """
module One

type GenericType<'ActualType> = {
    Value: 'ActualType
}
"""

    let two =
        FsSourceWithFileName "Two.fs" """
module Two
type One.GenericType<'U> with
    member x.Print () = printfn "%A" x.Value
"""

    let three =
        FsSourceWithFileName "Three.fs" """
module Three

open One

type GenericType<'X> with
    member _.Nothing() = ignore ()
"""

    one |> withAdditionalSourceFiles [ two; three ]
    |> compile
    |> verifyILContains [ ".Print<ActualType>" ]

// https://github.com/dotnet/fsharp/issues/14310
[<Fact>]
let ``Signature generation via --sig includes private field for struct type`` () =
    let implSource =
        """
module StructPrivateField

[<Struct; NoComparison; NoEquality>]
type C =
    [<DefaultValue>]
    val mutable private goo : byte array
    member this.P with set(x) = this.goo <- x
"""

    let sigSource =
        """
module StructPrivateField

[<NoComparison; NoEquality; Struct>]
type C =

    [<DefaultValue>]
    val mutable private goo: byte array

    member P: byte array with set
"""

    Fsi sigSource
    |> withAdditionalSourceFile (FsSource implSource)
    |> withOptions [ "--warnaserror:64" ]
    |> ignoreWarnings
    |> compile
    |> shouldSucceed
    |> ignore

// https://github.com/dotnet/fsharp/issues/14308
[<Fact>]
let ``Signature generation via --sig includes Sealed, AbstractClass, Interface and Class attributes`` () =
    let implSource =
        """
module AttrTest

[<Sealed>]
type SealedClass() =
    member _.X = 1

[<AbstractClass>]
type AbstractClass() =
    abstract member Y : int

[<Interface>]
type IMyInterface =
    abstract member Z : int

[<Class>]
type ExplicitClass() =
    member _.W = 2
"""

    let sigSource =
        """
module AttrTest

[<Sealed>]
type SealedClass =

    new: unit -> SealedClass

    member X: int

[<AbstractClass>]
type AbstractClass =

    new: unit -> AbstractClass

    abstract Y: int

[<Interface>]
type IMyInterface =

    abstract Z: int

[<Class>]
type ExplicitClass =

    new: unit -> ExplicitClass

    member W: int
"""

    Fsi sigSource
    |> withAdditionalSourceFile (FsSource implSource)
    |> withOptions [ "--warnaserror:64" ]
    |> ignoreWarnings
    |> compile
    |> shouldSucceed
    |> ignore

// https://github.com/dotnet/fsharp/issues/15339
[<Fact>]
let ``Struct with non-comparable field includes NoComparison in signature`` () =
    let implSource =
        """
namespace FSInteractive

open System

[<Struct>]
type LocalReadWriteLockCookie(locker: obj) =
    interface IDisposable with
        member _.Dispose () = ()
"""

    let generatedSignature =
        FSharp implSource |> printSignatures

    Assert.Contains("NoComparison", generatedSignature)

    Fsi generatedSignature
    |> withAdditionalSourceFile (FsSource implSource)
    |> withOptions [ "--warnaserror:64" ]
    |> ignoreWarnings
    |> compile
    |> shouldSucceed
    |> ignore

// https://github.com/dotnet/fsharp/issues/15339
[<Fact>]
let ``Struct with explicit NoComparison does not get duplicate attribute`` () =
    let generatedSignature =
        FSharp
            """
namespace TestNs

[<Struct; NoComparison>]
type AlreadyMarked =
    val X: obj
    new(x) = { X = x }
"""
        |> printSignatures

    // Should NOT duplicate [<NoComparison>] — the explicit one is sufficient
    let count = System.Text.RegularExpressions.Regex.Matches(generatedSignature, "NoComparison").Count
    Assert.Equal(1, count)

// https://github.com/dotnet/fsharp/issues/15339
[<Fact>]
let ``Struct with CustomComparison does not get NoComparison`` () =
    let generatedSignature =
        FSharp
            """
namespace TestNs

open System

[<Struct; CustomComparison; NoEquality>]
type WithCustomCompare =
    val X: obj
    new(x) = { X = x }
    interface IComparable with
        member _.CompareTo(_) = 0
"""
        |> printSignatures

    Assert.DoesNotContain("NoComparison", generatedSignature)

// https://github.com/dotnet/fsharp/issues/15339
[<Fact>]
let ``Struct with non-equatable field includes NoEquality in signature`` () =
    let implSource =
        """
namespace TestNs

[<Struct>]
type StructWithFunc =
    val Fn: int -> int
    new(f) = { Fn = f }
"""

    let generatedSignature =
        FSharp implSource |> printSignatures

    Assert.Contains("NoComparison", generatedSignature)
    Assert.Contains("NoEquality", generatedSignature)

    Fsi generatedSignature
    |> withAdditionalSourceFile (FsSource implSource)
    |> withOptions [ "--warnaserror:64" ]
    |> ignoreWarnings
    |> compile
    |> shouldSucceed
    |> ignore

// https://github.com/dotnet/fsharp/issues/15560
[<Fact>]
let ``Private type abbreviation with prefix type parameter`` () =
    let signatures =
        FSharp
            """
module Foo

type P<'a> = class end

[<RequireQualifiedAccess>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module R =     
    type private 'a P = unit -> 'a
"""
        |> printSignatures

    Assert.Contains("type private 'a P", signatures)
    Assert.DoesNotContain("type 'a private P", signatures)

// https://github.com/dotnet/fsharp/issues/16531
[<Fact>]
let ``Private constructor class with static members gets Class attribute in signature`` () =
    let implSource =
        """
module Telplin

type A private () = 
    static member Foo () = ()
"""

    let generatedSignature =
        FSharp implSource
        |> printSignatures

    // Static members alone are not enough for the compiler to infer class
    Assert.Contains("Class", generatedSignature)

    // Roundtrip: the generated signature must compile with the implementation
    Fsi generatedSignature
    |> withAdditionalSourceFile (FsSource implSource)
    |> withOptions [ "--warnaserror:64" ]
    |> ignoreWarnings
    |> compile
    |> shouldSucceed
    |> ignore

// https://github.com/dotnet/fsharp/issues/16531
[<Fact>]
let ``Private constructor class with instance members gets Class attribute in signature`` () =
    let implSource =
        """
module Telplin

type A private () = 
    member a.Foo () = ()
"""

    let generatedSignature =
        FSharp implSource
        |> printSignatures

    // Private ctor is not visible in signature, so [<Class>] is needed
    Assert.Contains("Class", generatedSignature)

    Fsi generatedSignature
    |> withAdditionalSourceFile (FsSource implSource)
    |> withOptions [ "--warnaserror:64" ]
    |> ignoreWarnings
    |> compile
    |> shouldSucceed
    |> ignore

// https://github.com/dotnet/fsharp/issues/16531
[<Fact>]
let ``Public constructor class does not need Class attribute in signature`` () =
    let implSource =
        """
module Telplin

type B() = 
    member b.Bar () = ()
"""

    let generatedSignature =
        FSharp implSource
        |> printSignatures

    // Public constructor is visible, so [<Class>] should not be needed
    Fsi generatedSignature
    |> withAdditionalSourceFile (FsSource implSource)
    |> withOptions [ "--warnaserror:64" ]
    |> ignoreWarnings
    |> compile
    |> shouldSucceed
    |> ignore

// https://github.com/dotnet/fsharp/issues/16531
[<Fact>]
let ``Empty class with private constructor uses class end`` () =
    let implSource =
        """
module Telplin

type Empty private () = class end
"""

    let generatedSignature =
        FSharp implSource
        |> printSignatures

    // Empty class should use 'class end' repr, not [<Class>] attribute
    Assert.Contains("class end", generatedSignature)

    Fsi generatedSignature
    |> withAdditionalSourceFile (FsSource implSource)
    |> ignoreWarnings
    |> compile
    |> shouldSucceed
    |> ignore

// https://github.com/dotnet/fsharp/issues/16531
[<Fact>]
let ``AbstractClass with private constructor roundtrips`` () =
    let implSource =
        """
module Telplin

[<AbstractClass>]
type A private () =
    abstract member M: unit -> unit
"""

    let generatedSignature =
        FSharp implSource
        |> printSignatures

    // The generated signature should compile with the implementation
    Fsi generatedSignature
    |> withAdditionalSourceFile (FsSource implSource)
    |> ignoreWarnings
    |> compile
    |> shouldSucceed
    |> ignore
