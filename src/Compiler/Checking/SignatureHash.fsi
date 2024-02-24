module internal Fsharp.Compiler.SignatureHash

open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.TypedTree
open FSharp.Compiler.CheckDeclarations

type ObserverVisibility =
    | PublicOnly
    | PublicAndInternal

val calculateHashOfImpliedSignature:
    g: TcGlobals -> observer: ObserverVisibility -> expr: ModuleOrNamespaceContents -> int

val calculateSignatureHashOfFiles: files: CheckedImplFile list -> g: TcGlobals -> observer: ObserverVisibility -> int
val calculateHashOfAssemblyTopAttributes: attrs: TopAttribs -> platform: ILPlatform option -> int
