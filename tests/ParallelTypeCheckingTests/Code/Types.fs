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

type ASTOrX =
    | AST of AST
    | X of string
    with
        member x.Name =
            match x with
            | AST ast -> ast.FileName
            | X fsi -> fsi + "x"

[<CustomEquality; CustomComparison>]
type File =
    {
        /// Order of the file in the project. Files with lower number cannot depend on files with higher number
        Idx : FileIdx
        Code : string
        AST : ASTOrX
        FsiBacked : bool
    }
    with
        member this.Name = this.AST.Name // TODO Use qualified name 
        member this.CodeSize = this.Code.Length
        override this.Equals other =
            match other with
            | :? File as f -> f.Name.Equals this.Name
            | _ -> false
        override this.GetHashCode () = this.Name.GetHashCode()
        override this.ToString() = System.IO.Path.GetFileName this.Name
        interface System.IComparable with 
            member x.CompareTo y =
                match y with
                | :? File as f -> x.Idx.Idx.CompareTo f.Idx.Idx
                | _ -> 0
        
        // TODO Please make this sane
        member this.IsFake = this.Code = ""
        static member FakeFs (idx : FileIdx) (fsi : string) : File =
            {
                Idx = idx
                Code = ""
                AST = ASTOrX.X fsi
                FsiBacked = false
            }
    
type Files = File[]