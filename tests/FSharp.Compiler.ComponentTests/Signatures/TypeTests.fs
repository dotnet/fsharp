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
    |> withLangVersionPreview
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
    |> withLangVersionPreview
    |> typecheckResults
    |> fun results ->
        let isBarSymbolUse = results.GetSymbolUseAtLocation(7, 13, "let foo = Foo()", [ "Foo" ]).Value
        match isBarSymbolUse.Symbol with
        | :? FSharpMemberOrFunctionOrValue as mfv ->
            Assert.False(mfv.IsUnionCaseTester, "IsUnionCaseTester returned true")
            Assert.True(mfv.IsConstructor)
        | _ -> failwith "Expected FSharpMemberOrFunctionOrValue"