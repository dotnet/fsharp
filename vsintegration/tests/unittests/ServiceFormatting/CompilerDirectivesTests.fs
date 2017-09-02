module FSharp.Compiler.Service.Tests.ServiceFormatting.CompilerDirectiveTests

open NUnit.Framework
open FsUnit
open TestHelper

[<Test>]
let ``should use verbatim strings on some hash directives``() =
    formatSourceString false """
    #r @"C:\foo\bar.dll"
    """ config
    |> prepend newline
    |> should equal """
#r @"C:\foo\bar.dll"
"""

[<Test>]
let ``hash directives``() =
    formatSourceString false """
    #r "Fantomas.Tests.dll"
    #load "CodeFormatterTests.fs"
    """ config
    |> prepend newline
    |> should equal """
#r "Fantomas.Tests.dll"
#load "CodeFormatterTests.fs"
"""

[<Test>]
let ``should support load directive multiple arguments``() =
    formatSourceString false """
    #load "A.fs" "B.fs"
    #load "C.fs"
          "D.fs"
          "E.fs"
    """ config
    |> prepend newline
    |> should equal """
#load "A.fs" "B.fs"
#load "C.fs" "D.fs" "E.fs"
"""

[<Test>]
let ``should keep compiler directives``() =
    formatSourceString false """
#if INTERACTIVE
#load "../FSharpx.TypeProviders/SetupTesting.fsx"
SetupTesting.generateSetupScript __SOURCE_DIRECTORY__
#load "__setup__.fsx"
#endif
"""  config
    |> should equal """
#if INTERACTIVE
#load "../FSharpx.TypeProviders/SetupTesting.fsx"

SetupTesting.generateSetupScript __SOURCE_DIRECTORY__

#load "__setup__.fsx"
#endif
"""

[<Test>]
let ``should keep compiler directives 2``() =
    formatSourceString false """
#if INTERACTIVE
#else
#load "../FSharpx.TypeProviders/SetupTesting.fsx"
SetupTesting.generateSetupScript __SOURCE_DIRECTORY__
#load "__setup__.fsx"
#endif
"""  config
    |> should equal """
#if INTERACTIVE
#else
#load "../FSharpx.TypeProviders/SetupTesting.fsx"
SetupTesting.generateSetupScript __SOURCE_DIRECTORY__
#load "__setup__.fsx"
#endif
"""

[<Test>]
let ``line, file and path identifiers``() =
    formatSourceString false """
    let printSourceLocation() =
        printfn "Line: %s" __LINE__
        printfn "Source Directory: %s" __SOURCE_DIRECTORY__
        printfn "Source File: %s" __SOURCE_FILE__
    printSourceLocation()
    """ config
    |> prepend newline
    |> should equal """
let printSourceLocation() =
    printfn "Line: %s" __LINE__
    printfn "Source Directory: %s" __SOURCE_DIRECTORY__
    printfn "Source File: %s" __SOURCE_FILE__

printSourceLocation()
"""

[<Test>]
let ``should keep #if, #else and #endif on compiler directives``() =
    formatSourceString false """
let x = 1
#if SILVERLIGHT
let useHiddenInitCode = false
#else
let useHiddenInitCode = true
#endif
let y = 2
"""  config
    |> prepend newline
    |> should equal """
let x = 1
#if SILVERLIGHT
let useHiddenInitCode = false
#else
let useHiddenInitCode = true
#endif

let y = 2
"""

[<Test>]
let ``should handle nested compiler directives``() =
    formatSourceString false """
let [<Literal>] private assemblyConfig =
    #if DEBUG
    #if TRACE
    "DEBUG;TRACE"
    #else
    "DEBUG"
    #endif
    #else
    #if TRACE
    "TRACE"
    #else
    ""
    #endif
    #endif
"""  config
    |> prepend newline
    |> should equal """
[<Literal>]
let private assemblyConfig =
#if DEBUG
#if TRACE
    "DEBUG;TRACE"
#else
    "DEBUG"
#endif
#else
#if TRACE
    "TRACE"
#else
    ""
#endif
#endif
"""

[<Test>]
let ``should break lines before compiler directives``() =
    formatSourceString false """
let [<Literal>] private assemblyConfig() =
  #if TRACE
  let x = ""
  #else
  let x = "x"
  #endif
  x
"""  config
    |> prepend newline
    |> should equal """
[<Literal>]
let private assemblyConfig() =
#if TRACE
    let x = ""
#else
    let x = "x"
#endif
    
    x
"""

[<Test>]
let ``should break line after single directive``() =
    formatSourceString false """
#nowarn "47"
namespace Internal.Utilities.Text.Lexing"""  config
    |> prepend newline
    |> should equal """
#nowarn "47"
namespace Internal.Utilities.Text.Lexing

"""

[<Test>]
let ``should handle endif directives with no newline``() =
    formatSourceString false """
namespace Internal.Utilities.Diagnostic

#if EXTENSIBLE_DUMPER
#if DEBUG

type ExtensibleDumper = A | B

#endif  
#endif"""  config
    |> prepend newline
    |> should equal """
namespace Internal.Utilities.Diagnostic
#if EXTENSIBLE_DUMPER
#if DEBUG
type ExtensibleDumper = A | B
#endif

#endif


"""
