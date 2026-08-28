module internal Fsharp.Compiler.SignatureHash

open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.TypedTree
open FSharp.Compiler.CheckDeclarations

open Internal.Utilities.TypeHashing

val calculateHashOfImpliedSignature:
    g: TcGlobals -> observer: ObserverVisibility -> expr: ModuleOrNamespaceContents -> int64

val calculateSignatureHashOfFiles: files: CheckedImplFile list -> g: TcGlobals -> observer: ObserverVisibility -> int64
val calculateHashOfAssemblyTopAttributes: attrs: TopAttribs -> platform: ILPlatform option -> int64
