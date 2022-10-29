module FSharp.Compiler.Service.Tests.Types

open FSharp.Compiler.Service.Tests.Utils
type AST = FSharp.Compiler.Syntax.ParsedInput

/// Input from the compiler after parsing
[<CustomEquality; NoComparison>]
type SourceFile = 
    {
        Name : string
        Idx : FileIdx
        Code : string
        AST : AST
    }
    override this.Equals other =
        match other with
        | :? SourceFile as p -> p.Name.Equals this.Name
        | _ -> false
    override this.GetHashCode () = this.Name.GetHashCode()

type SourceFiles = SourceFile[]

[<CustomEquality; NoComparison>]
type File =
    {
        Name : string
        Idx : FileIdx
        Code : string
        AST : AST
        FsiBacked : bool
    }
    override this.Equals other =
        match other with
        | :? File as f -> f.Name.Equals this.Name
        | _ -> false
    override this.GetHashCode () = this.Name.GetHashCode()
    
type Files = File[]