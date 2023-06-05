module internal Fsharp.Compiler.SignatureHash

open FSharp.Compiler.TcGlobals
open FSharp.Compiler.TypedTree

type ObserverVisibility =
    | PublicOnly
    | PublicAndInternal

val calculateHashOfImpliedSignature: g:TcGlobals -> observer:ObserverVisibility ->  expr:ModuleOrNamespaceContents -> int
val calculateSignatureHashOfFiles: files:CheckedImplFile list -> g:TcGlobals -> observer:ObserverVisibility -> int