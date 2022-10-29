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
    override this.ToString() = this.Name

type SourceFiles = SourceFile[]

[<CustomEquality; NoComparison>]
type File =
    {
        Name : string
        /// Order of the file in the project. Files with lower number cannot depend on files with higher number
        Idx : FileIdx
        Code : string
        AST : AST
        FsiBacked : bool
    }
    with
        member this.CodeSize = this.Code.Length
        override this.Equals other =
            match other with
            | :? File as f -> f.Name.Equals this.Name
            | _ -> false
        override this.GetHashCode () = this.Name.GetHashCode()
        override this.ToString() = this.Name
    
type Files = File[]