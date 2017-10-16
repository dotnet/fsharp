// Copied from https://github.com/dungpa/fantomas and modified by Vasily Kirichenko

module FSharp.Compiler.Service.Tests.ServiceFormatting.RecordTests

open NUnit.Framework
open FsUnit
open TestHelper

[<Test>]
let ``record declaration``() =
    formatSourceString false "type AParameters = { a : int }" config
    |> prepend newline
    |> should equal """
type AParameters =
    { a : int }
"""

[<Test>]
let ``record declaration with implementation visibility attribute``() =
    formatSourceString false "type AParameters = private { a : int; b: float }" config
    |> prepend newline
    |> should equal """
type AParameters =
    private { a : int
              b : float }
"""

[<Test>]
let ``record signatures``() =
    formatSourceString true """
module RecordSignature
/// Represents simple XML elements.
type Element =
    {
        /// The attribute collection.
        Attributes : IDictionary<Name,string>

        /// The children collection.
        Children : seq<INode>

        /// The qualified name.
        Name : Name
    }

    interface INode

    /// Constructs an new empty Element.
    static member Create : name: string * ?uri: string -> Element

    /// Replaces the children.
    static member WithChildren : children: #seq<#INode> -> self: Element -> Element

    /// Replaces the children.
    static member ( - ) : self: Element * children: #seq<#INode> -> Element

    /// Replaces the attributes.
    static member WithAttributes : attrs: #seq<string*string> -> self: Element -> Element

    /// Replaces the attributes.
    static member ( + ) : self: Element * attrs: #seq<string*string> -> Element

    /// Replaces the children with a single text node.
    static member WithText : text: string -> self: Element-> Element

    /// Replaces the children with a single text node.
    static member ( -- ) : self: Element * text: string -> Element""" { config with SemicolonAtEndOfLine = true }
    |> prepend newline
    |> should equal """
module RecordSignature

/// Represents simple XML elements.
type Element =
    { /// The attribute collection.
      Attributes : IDictionary<Name, string>;
      /// The children collection.
      Children : seq<INode>;
      /// The qualified name.
      Name : Name }
    interface INode
    /// Constructs an new empty Element.
    static member Create : name:string * ?uri:string -> Element
    /// Replaces the children.
    static member WithChildren : children:#seq<#INode>
         -> self:Element -> Element
    /// Replaces the children.
    static member (-) : self:Element * children:#seq<#INode> -> Element
    /// Replaces the attributes.
    static member WithAttributes : attrs:#seq<string * string>
         -> self:Element -> Element
    /// Replaces the attributes.
    static member (+) : self:Element * attrs:#seq<string * string> -> Element
    /// Replaces the children with a single text node.
    static member WithText : text:string -> self:Element -> Element
    /// Replaces the children with a single text node.
    static member (--) : self:Element * text:string -> Element
"""

[<Test>]
let ``records with update``() =
    formatSourceString false """
type Car = {
    Make : string
    Model : string
    mutable Odometer : int
    }

let myRecord3 = { myRecord2 with Y = 100; Z = 2 }""" config
    |> prepend newline
    |> should equal """
type Car =
    { Make : string
      Model : string
      mutable Odometer : int }

let myRecord3 =
    { myRecord2 with Y = 100
                     Z = 2 }
"""

// the current behavior results in a compile error since the if is not aligned properly
[<Test>]
let ``should not break inside of if statements in records``() =
    formatSourceString false """let XpkgDefaults() =
    {
        ToolPath = "./tools/xpkg/xpkg.exe"
        WorkingDir = "./";
        TimeOut = TimeSpan.FromMinutes 5.
        Package = null
        Version = if not isLocalBuild then buildVersion else "0.1.0.0"
        OutputPath = "./xpkg"
        Project = null
        Summary = null
        Publisher = null
        Website = null
        Details = "Details.md"
        License = "License.md"
        GettingStarted = "GettingStarted.md"
        Icons = []
        Libraries = []
        Samples = [];
    }

    """ { config with SemicolonAtEndOfLine = true }
    |> should equal """let XpkgDefaults() =
    { ToolPath = "./tools/xpkg/xpkg.exe";
      WorkingDir = "./";
      TimeOut = TimeSpan.FromMinutes 5.;
      Package = null;
      Version =
          if not isLocalBuild then buildVersion
          else "0.1.0.0";
      OutputPath = "./xpkg";
      Project = null;
      Summary = null;
      Publisher = null;
      Website = null;
      Details = "Details.md";
      License = "License.md";
      GettingStarted = "GettingStarted.md";
      Icons = [];
      Libraries = [];
      Samples = [] }
"""

[<Test>]
let ``should not add redundant newlines when using a record in a DU``() =
    formatSourceString false """
let rec make item depth = 
    if depth > 0 then 
        Tree({ Left = make (2 * item - 1) (depth - 1)
               Right = make (2 * item) (depth - 1) }, item)
    else Tree(defaultof<_>, item)""" config
  |> prepend newline
  |> should equal """
let rec make item depth =
    if depth > 0 then 
        Tree({ Left = make (2 * item - 1) (depth - 1)
               Right = make (2 * item) (depth - 1) }, item)
    else Tree(defaultof<_>, item)
"""

[<Test>]
let ``should keep unit of measures in record and DU declaration``() =
    formatSourceString false """
type rate = {Rate:float<GBP*SGD/USD>}
type rate2 = Rate of float<GBP/SGD*USD>
"""  config
  |> prepend newline
  |> should equal """
type rate =
    { Rate : float<GBP * SGD / USD> }

type rate2 =
    | Rate of float<GBP / SGD * USD>
"""

[<Test>]
let ``should keep comments on records``() =
    formatSourceString false """
let newDocument = //somecomment
    { program = Encoding.Default.GetBytes(document.Program) |> Encoding.UTF8.GetString
      content = Encoding.Default.GetBytes(document.Content) |> Encoding.UTF8.GetString
      created = document.Created.ToLocalTime() }
    |> JsonConvert.SerializeObject
"""  config
  |> prepend newline
  |> should equal """
let newDocument = //somecomment
    { program =
          Encoding.Default.GetBytes(document.Program) |> Encoding.UTF8.GetString
      content =
          Encoding.Default.GetBytes(document.Content) |> Encoding.UTF8.GetString
      created = document.Created.ToLocalTime() }
    |> JsonConvert.SerializeObject
"""

[<Test>]
let ``should preserve inherit parts in records``() =
    formatSourceString false """
type MyExc =
    inherit Exception
    new(msg) = {inherit Exception(msg)}
"""  config
  |> prepend newline
  |> should equal """
type MyExc =
    inherit Exception
    new(msg) = { inherit Exception(msg) }
"""