module FSharp.Compiler.Service.Tests.ServiceFormatting.SignatureTests

open NUnit.Framework
open FsUnit
open TestHelper

// the current behavior results in a compile error since "(string * string) list" is converted to "string * string list"
[<Test>]
let ``should keep the (string * string) list type signature in records``() =
    formatSourceString false """type MSBuildParams = 
    { Targets : string list
      Properties : (string * string) list
      MaxCpuCount : int option option
      ToolsVersion : string option
      Verbosity : MSBuildVerbosity option
      FileLoggers : MSBuildFileLoggerConfig list option }

    """ config
    |> should equal """type MSBuildParams =
    { Targets : string list
      Properties : (string * string) list
      MaxCpuCount : int option option
      ToolsVersion : string option
      Verbosity : MSBuildVerbosity option
      FileLoggers : MSBuildFileLoggerConfig list option }
"""

[<Test>]
let ``should keep the (string * string) list type signature in functions``() =
    formatSourceString false """let MSBuildWithProjectProperties outputPath (targets : string) 
    (properties : string -> (string * string) list) projects = doingsomstuff

    """ config
    |> should equal """let MSBuildWithProjectProperties outputPath (targets : string) 
    (properties : string -> (string * string) list) projects = doingsomstuff
"""


[<Test>]
let ``should keep the string * string list type signature in functions``() =
    formatSourceString false """let MSBuildWithProjectProperties outputPath (targets : string) 
    (properties : (string -> string) * string list) projects = doingsomstuff

    """ config
    |> should equal """let MSBuildWithProjectProperties outputPath (targets : string) 
    (properties : (string -> string) * string list) projects = doingsomstuff
"""

[<Test>]
let ``should not add parens in signature``() =
    formatSourceString false """type Route = 
    { Verb : string
      Path : string
      Handler : Map<string, string> -> HttpListenerContext -> string }
    override x.ToString() = sprintf "%s %s" x.Verb x.Path

    """ config
    |> should equal """type Route =
    { Verb : string
      Path : string
      Handler : Map<string, string> -> HttpListenerContext -> string }
    override x.ToString() = sprintf "%s %s" x.Verb x.Path
"""

[<Test>]
let ``should keep the string * string * string option type signature``() =
    formatSourceString false """type DGML = 
    | Node of string
    | Link of string * string * (string option)

    """ config
    |> should equal """type DGML =
    | Node of string
    | Link of string * string * string option
"""

[<Test>]
let ``should keep the (string option * Node) list type signature``() =
    formatSourceString false """type Node = 
    { Name : string;
      NextNodes : (string option * Node) list }

    """ { config with SemicolonAtEndOfLine = true }
    |> should equal """type Node =
    { Name : string;
      NextNodes : (string option * Node) list }
"""

[<Test>]
let ``should keep parentheses on the left of type signatures``() =
    formatSourceString false """type IA =
    abstract F: (unit -> Option<'T>) -> Option<'T>

type A () =
    interface IA with
        member x.F (f: unit -> _) = f ()
    """ config
    |> should equal """type IA =
    abstract F : (unit -> Option<'T>) -> Option<'T>

type A() =
    interface IA with
        member x.F(f : unit -> _) = f()
"""

[<Test>]
let ``should not add parentheses around bare tuples``() =
    formatSourceString true """
namespace TupleType
type C =
    member P1 : int * string
    /// def
    member P2 : int
"""  config
    |> prepend newline
    |> should equal """
namespace TupleType

type C =
    member P1 : int * string
    /// def
    member P2 : int
"""

[<Test>]
let ``should keep global constraints in type signature``() =
    formatSourceString true """
module Tainted
val GetHashCodeTainted : (Tainted<'T> -> int) when 'T : equality
"""  config
    |> prepend newline
    |> should equal """
module Tainted

val GetHashCodeTainted : Tainted<'T> -> int when 'T : equality
"""
