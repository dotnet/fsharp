module Signatures.TypeTests

open Xunit
open FsUnit
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
    |> should equal
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
    |> should equal
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
    |> should equal
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
    |> should equal
        """
module Extensions
type System.Collections.Concurrent.ConcurrentDictionary<'key,'value> with

  member TryFind: key: 'key -> 'value option"""
  