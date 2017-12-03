// Copied from https://github.com/dungpa/fantomas and modified by Vasily Kirichenko

module FSharp.Compiler.Service.Tests.ServiceFormatting.CommentTests

open NUnit.Framework
open FsUnit
open TestHelper

[<Test>]
let ``should keep sticky-to-the-left comments after nowarn directives``() =
    formatSourceString false """#nowarn "51" // address-of operator can occur in the code""" config
    |> should equal """#nowarn "51" // address-of operator can occur in the code
"""

[<Test>]
let ``should keep sticky-to-the-right comments before module definition``() =
    formatSourceString false """
// The original idea for this typeprovider is from Ivan Towlson
// some text
module FSharpx.TypeProviders.VectorTypeProvider

let x = 1""" config
    |> should equal """// The original idea for this typeprovider is from Ivan Towlson
// some text
module FSharpx.TypeProviders.VectorTypeProvider

let x = 1
"""

[<Test>]
let ``comments on local let bindings``() =
    formatSourceString false """
let print_30_permut() = 

    /// declare and initialize
    let permutation : int array = Array.init n (fun i -> Console.Write(i+1); i)
    permutation
    """ config
    |> prepend newline
    |> should equal """
let print_30_permut() =
    /// declare and initialize
    let permutation : int array =
        Array.init n (fun i -> 
            Console.Write(i + 1)
            i)
    permutation
"""

[<Test>]
let ``comments on local let bindings with desugared lambda``() =
    formatSourceString false """
let print_30_permut() = 

    /// declare and initialize
    let permutation : int array = Array.init n (fun (i,j) -> Console.Write(i+1); i)
    permutation
    """ config
    |> prepend newline
    |> should equal """
let print_30_permut() =
    /// declare and initialize
    let permutation : int array =
        Array.init n (fun (i, j) -> 
            Console.Write(i + 1)
            i)
    permutation
"""


[<Test>]
let ``xml documentation``() =
    formatSourceString false """
/// <summary>
/// Kill Weight Mud
/// </summary>
///<param name="sidpp">description</param>
///<param name="tvd">xdescription</param>
///<param name="omw">ydescription</param>
let kwm sidpp tvd omw =
    (sidpp / 0.052 / tvd) + omw

/// Kill Weight Mud
let kwm sidpp tvd omw = 1.0""" config
    |> prepend newline
    |> should equal """
/// <summary>
/// Kill Weight Mud
/// </summary>
///<param name="sidpp">description</param>
///<param name="tvd">xdescription</param>
///<param name="omw">ydescription</param>
let kwm sidpp tvd omw = (sidpp / 0.052 / tvd) + omw

/// Kill Weight Mud
let kwm sidpp tvd omw = 1.0
"""

[<Test>]
let ``should preserve comment-only source code``() =
    formatSourceString false """(*
  line1
  line2
*)
"""  config
    |> should equal """
(*
  line1
  line2
*)
"""

[<Test>]
let ``should keep sticky-to-the-right comments``() =
    formatSourceString false """
let f() = 
    // COMMENT
    x + x
"""  config
    |> prepend newline
    |> should equal """
let f() =
    // COMMENT
    x + x
"""

[<Test>]
let ``should keep sticky-to-the-left comments``() =
    formatSourceString false """
let f() = 
  let x = 1 // COMMENT
  x + x
"""   config
    |> prepend newline
    |> should equal """
let f() =
    let x = 1 // COMMENT
    x + x
""" 

[<Test>]
let ``should keep well-aligned comments``() =
    formatSourceString false """
/// XML COMMENT
// Other comment
let f() = 
    // COMMENT A
    let y = 1
    (* COMMENT B *)
    (* COMMENT C *)
    x + x + x

"""  config
    |> prepend newline
    |> should equal """
/// XML COMMENT
// Other comment
let f() =
    // COMMENT A
    let y = 1
    (* COMMENT B *)
    (* COMMENT C *)
    x + x + x
"""

[<Test; Ignore "ignored">]
let ``should align mis-aligned comments``() =
    formatSourceString false  """
   /// XML COMMENT A
     // Other comment
let f() = 
      // COMMENT A
    let y = 1
      /// XML COMMENT B
    let z = 1
  // COMMENT B
    x + x + x

"""  config
    |> prepend newline
    |> should equal """
/// XML COMMENT A
// Other comment
let f() =
    // COMMENT A
    let y = 1
    /// XML COMMENT B
    let z = 1
    // COMMENT B
    x + x + x
"""

[<Test>]
let ``should indent comments properly``() =
    formatSourceString false  """
/// Non-local information related to internals of code generation within an assembly
type IlxGenIntraAssemblyInfo = 
    { /// A table recording the generated name of the static backing fields for each mutable top level value where 
      /// we may need to take the address of that value, e.g. static mutable module-bound values which are structs. These are 
      /// only accessible intra-assembly. Across assemblies, taking the address of static mutable module-bound values is not permitted.
      /// The key to the table is the method ref for the property getter for the value, which is a stable name for the Val's
      /// that come from both the signature and the implementation.
      StaticFieldInfo : Dictionary<ILMethodRef, ILFieldSpec> }

"""  config
    |> prepend newline
    |> should equal """
/// Non-local information related to internals of code generation within an assembly
type IlxGenIntraAssemblyInfo =
    { /// A table recording the generated name of the static backing fields for each mutable top level value where 
      /// we may need to take the address of that value, e.g. static mutable module-bound values which are structs. These are 
      /// only accessible intra-assembly. Across assemblies, taking the address of static mutable module-bound values is not permitted.
      /// The key to the table is the method ref for the property getter for the value, which is a stable name for the Val's
      /// that come from both the signature and the implementation.
      StaticFieldInfo : Dictionary<ILMethodRef, ILFieldSpec> }
"""

[<Test>]
let ``shouldn't break on one-line comment``() =
    formatSourceString false  """
1 + (* Comment *) 1""" config
    |> prepend newline
    |> should equal """
1 + (* Comment *) 1
"""

[<Test>]
let ``should keep comments on DU cases``() =
    formatSourceString false  """
/// XML comment
type X = 
   /// Hello
   A 
   /// Goodbye
   | B
"""  config
    |> prepend newline
    |> should equal """
/// XML comment
type X =
    /// Hello
    | A
    /// Goodbye
    | B
"""

[<Test>]
let ``should keep comments before attributes``() =
    formatSourceString false  """
[<NoEquality; NoComparison>]
type IlxGenOptions = 
    { fragName: string
      generateFilterBlocks: bool
      workAroundReflectionEmitBugs: bool
      emitConstantArraysUsingStaticDataBlobs: bool
      // If this is set, then the last module becomes the "main" module and its toplevel bindings are executed at startup 
      mainMethodInfo: Tast.Attribs option
      localOptimizationsAreOn: bool
      generateDebugSymbols: bool
      testFlagEmitFeeFeeAs100001: bool
      ilxBackend: IlxGenBackend
      /// Indicates the code is being generated in FSI.EXE and is executed immediately after code generation
      /// This includes all interactively compiled code, including #load, definitions, and expressions
      isInteractive: bool 
      // Indicates the code generated is an interactive 'it' expression. We generate a setter to allow clearing of the underlying
      // storage, even though 'it' is not logically mutable
      isInteractiveItExpr: bool
      // Indicates System.SerializableAttribute is available in the target framework
      netFxHasSerializableAttribute : bool
      /// Whenever possible, use callvirt instead of call
      alwaysCallVirt: bool}

"""   { config with SemicolonAtEndOfLine = true }
    |> prepend newline
    |> should equal """
[<NoEquality; NoComparison>]
type IlxGenOptions =
    { fragName : string;
      generateFilterBlocks : bool;
      workAroundReflectionEmitBugs : bool;
      emitConstantArraysUsingStaticDataBlobs : bool;
      // If this is set, then the last module becomes the "main" module and its toplevel bindings are executed at startup 
      mainMethodInfo : Tast.Attribs option;
      localOptimizationsAreOn : bool;
      generateDebugSymbols : bool;
      testFlagEmitFeeFeeAs100001 : bool;
      ilxBackend : IlxGenBackend;
      /// Indicates the code is being generated in FSI.EXE and is executed immediately after code generation
      /// This includes all interactively compiled code, including #load, definitions, and expressions
      isInteractive : bool;
      // Indicates the code generated is an interactive 'it' expression. We generate a setter to allow clearing of the underlying
      // storage, even though 'it' is not logically mutable
      isInteractiveItExpr : bool;
      // Indicates System.SerializableAttribute is available in the target framework
      netFxHasSerializableAttribute : bool;
      /// Whenever possible, use callvirt instead of call
      alwaysCallVirt : bool }
"""

[<Test; Ignore "ignored">]
let ``should keep comments on else if``() =
    formatSourceString false  """
if true then ()
else
    // Comment 1
    if true then ()
    // Comment 2
    else ()
"""  config
    |> prepend newline
    |> should equal """
if true then ()
else
    // Comment 1
    if true then ()
    // Comment 2
    else ()
"""

[<Test>]
let ``should keep comments on almost-equal identifiers``() =
    formatSourceString false  """
let zp = p1 lxor p2
// Comment 1
let b = zp land (zp)
(* Comment 2 *)
let p = p1 land (b - 1)
"""  config
    |> prepend newline
    |> should equal """
let zp = p1 ``lxor`` p2
// Comment 1
let b = zp ``land`` (zp)
(* Comment 2 *)
let p = p1 ``land`` (b - 1)
"""

[<Test>]
let ``should not write sticky-to-the-left comments in a new line``() =
    formatSourceString false  """
let moveFrom source =
  getAllFiles source
    |> Seq.filter (fun f -> Path.GetExtension(f).ToLower() <> ".db")  //exlcude the thumbs.db files
    |> move @"C:\_EXTERNAL_DRIVE\_Camera"
"""  config
    |> prepend newline
    |> should equal """
let moveFrom source =
    getAllFiles source
    |> Seq.filter (fun f -> Path.GetExtension(f).ToLower() <> ".db") //exlcude the thumbs.db files
    |> move @"C:\_EXTERNAL_DRIVE\_Camera"
"""

[<Test>]
let ``should handle comments at the end of file``() =
    formatSourceString false  """
let hello() = "hello world"

(* This is a comment. *)
"""  config
    |> prepend newline
    |> should equal """
let hello() = "hello world"
(* This is a comment. *)
"""

[<Test>]
let ``should keep comments inside unit``() =
    formatSourceString false """
let x =
    ((*comment*))
    printf "a"
    // another comment 1
    printf "b"
    // another comment 2
    printf "c"
"""  config
    |> prepend newline
    |> should equal """
let x =
    ((*comment*))
    printf "a"
    // another comment 1
    printf "b"
    // another comment 2
    printf "c"
""" 