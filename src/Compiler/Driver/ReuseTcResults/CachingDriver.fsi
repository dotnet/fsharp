module internal FSharp.Compiler.ReuseTcResults

open FSharp.Compiler.CompilerConfig
open FSharp.Compiler.Syntax

[<Sealed>]
type CachingDriver =

    new: tcConfig: TcConfig -> CachingDriver

    member TryReuseTcResults: inputs: ParsedInput seq -> unit
