module FSharp.Compiler.Service.Tests.Types

open FSharp.Compiler.Service.Tests.Utils
open FSharp.Compiler.Syntax
type AST = FSharp.Compiler.Syntax.ParsedInput

type FileType =
    | Sig
    | Impl
    
module AST =
    let getType (ast : AST) =
        match ast with
        | ParsedInput.SigFile _ -> FileType.Sig
        | ParsedInput.ImplFile _ -> FileType.Impl

/// Input from the compiler after parsing
[<CustomEquality; NoComparison>]
type SourceFile = 
    {
        Idx : FileIdx
        AST : AST
    }
    override this.Equals other =
        match other with
        | :? SourceFile as p -> p.Idx.Equals this.Idx
        | _ -> false
    override this.GetHashCode () = this.Idx.GetHashCode()
    override this.ToString() = this.Idx.ToString()
    member this.QualifiedName = this.AST.FileName

type SourceFiles = SourceFile[]

[<CustomEquality; NoComparison>]
type File =
    {
        /// Order of the file in the project. Files with lower number cannot depend on files with higher number
        Idx : FileIdx
        Code : string
        AST : AST
        FsiBacked : bool
    }
    with
        member this.Name = this.AST.FileName // TODO Use qualified name 
        member this.CodeSize = this.Code.Length
        override this.Equals other =
            match other with
            | :? File as f -> f.Name.Equals this.Name
            | _ -> false
        override this.GetHashCode () = this.Name.GetHashCode()
        override this.ToString() = this.Name
    
type Files = File[]