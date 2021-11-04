(*** hide ***)
#I "../../artifacts/bin/FSharp.Compiler.Service/Debug/netstandard2.0"
(**
Compiler Services: Editor services
==================================

This tutorial demonstrates how to use the editor services provided by the F# compiler.
This API is used to provide auto-complete, tool-tips, parameter info help, matching of
brackets and other functions in F# editors including Visual Studio, Xamarin Studio and Emacs
(see [fsharpbindings](https://github.com/fsharp/fsharpbinding) project for more information).
Similarly to [the tutorial on using untyped AST](untypedtree.html), we start by
getting the `InteractiveChecker` object.

> **NOTE:** The FSharp.Compiler.Service API is subject to change when later versions of the nuget package are published


Type checking sample source code
--------------------------------

As in the [previous tutorial (using untyped AST)](untypedtree.html), we start by referencing
`FSharp.Compiler.Service.dll`, opening the relevant namespace and creating an instance
of `InteractiveChecker`:

*)
// Reference F# compiler API
#r "FSharp.Compiler.Service.dll"

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Text
open FSharp.Compiler.Tokenization

// Create an interactive checker instance
let checker = FSharpChecker.Create()

(**

As [previously](untypedtree.html), we use `GetProjectOptionsFromScriptRoot` to get a context
where the specified input is the only file passed to the compiler (and it is treated as a
script file or stand-alone F# source code).

*)
// Sample input as a multi-line string
let input =
    """
  open System

  let foo() =
    let msg = String.Concat("Hello"," ","world")
    if true then
      printfn "%s" msg.
  """
// Split the input & define file name
let inputLines = input.Split('\n')
let file = "/home/user/Test.fsx"

let projOptions, _diagnostics =
    checker.GetProjectOptionsFromScript(file, SourceText.ofString input)
    |> Async.RunSynchronously

let parsingOptions, _diagnostics2 =
    checker.GetParsingOptionsFromProjectOptions(projOptions)

(**
To perform type checking, we first need to parse the input using
`ParseFile`, which gives us access to the [untyped AST](untypedtree.html). However,
then we need to call `CheckFileInProject` to perform the full type checking. This function
also requires the result of `ParseFileInProject`, so the two functions are often called
together.
*)
// Perform parsing

let parseFileResults =
    checker.ParseFile(file, SourceText.ofString input, parsingOptions)
    |> Async.RunSynchronously
(**
Before we look at the interesting operations provided by `TypeCheckResults`, we
need to run the type checker on a sample input. On F# code with errors, you would get some type checking
result (but it may contain incorrectly "guessed" results).
*)

// Perform type checking
let checkFileAnswer =
    checker.CheckFileInProject(parseFileResults, file, 0, SourceText.ofString input, projOptions)
    |> Async.RunSynchronously

(**
Alternatively you can use `ParseAndCheckFileInProject` to check both in one step:
*)

let parseResults2, checkFileAnswer2 =
    checker.ParseAndCheckFileInProject(file, 0, SourceText.ofString input, projOptions)
    |> Async.RunSynchronously

(**

The function returns both the untyped parse result (which we do not use in this
tutorial), but also a `CheckFileAnswer` value, which gives us access to all
the interesting functionality...
*)

let checkFileResults =
    match checkFileAnswer with
    | FSharpCheckFileAnswer.Succeeded (res) -> res
    | res -> failwithf "Parsing did not finish... (%A)" res

(**

Here, we type check a simple function that (conditionally) prints "Hello world".
On the last line, we leave an additional dot in `msg.` so that we can get the
completion list on the `msg` value (we expect to see various methods on the string
type there).


Using type checking results
---------------------------

Let's now look at some of the API that is exposed by the `TypeCheckResults` type. In general,
this is the type that lets you implement most of the interesting F# source code editor services.

### Getting a tool tip

To get a tool tip, you can use `GetToolTip` method. The method takes a line number and character
offset. Both of the numbers are zero-based. In the sample code, we want to get tooltip for the `foo`
function that is defined on line 3 (line 0 is blank) and the letter `f` starts at index 7 (the tooltip
would work anywhere inside the identifier).

In addition, the method takes a tag of token which is typically `IDENT`, when getting tooltip for an
identifier (the other option lets you get tooltip with full assembly location when using `#r "..."`).

*)
// Get tag of the IDENT token to be used as the last argument
let identToken = FSharpTokenTag.Identifier

// Get tool tip at the specified location
let tip =
    checkFileResults.GetToolTip(4, 7, inputLines.[1], [ "foo" ], identToken)

printfn "%A" tip

(**

*)

(**
Aside from the location and token kind, the function also requires the current contents of the line
(useful when the source code changes) and a `Names` value, which is a list of strings representing
the current long name. For example to get tooltip for the `Random` identifier in a long name
`System.Random`, you would use location somewhere in the string `Random` and you would pass
`["System"; "Random"]` as the `Names` value.

The returned value is of type `ToolTipText` which contains a discriminated union `ToolTipElement`.
The union represents different kinds of tool tips that you can get from the compiler.

### Getting auto-complete lists

The next method exposed by `TypeCheckResults` lets us perform auto-complete on a given location.
This can be called on any identifier or in any scope (in which case you get a list of names visible
in the scope) or immediately after `.` to get a list of members of some object. Here, we get a
list of members of the string value `msg`.

To do this, we call `GetDeclarationListInfo` with the location of the `.` symbol on the last line
(ending with `printfn "%s" msg.`). The offsets are one-based, so the location is `7, 23`.
We also need to specify a function that says that the text has not changed and the current identifier
where we need to perform the completion.
*)
// Get declarations (autocomplete) for a location
let decls =
    checkFileResults.GetDeclarationListInfo(
        Some parseFileResults,
        7,
        inputLines.[6],
        PartialLongName.Empty 23,
        (fun () -> [])
    )

// Print the names of available items
for item in decls.Items do
    printfn " - %s" item.Name

(**

> **NOTE:** `v` is an alternative name for the old `GetDeclarations`. The old `GetDeclarations` was
deprecated because it accepted zero-based line numbers.  At some point it will be removed, and  `GetDeclarationListInfo` will be renamed back to `GetDeclarations`.
*)

(**
When you run the code, you should get a list containing the usual string methods such as
`Substring`, `ToUpper`, `ToLower` etc. The fourth argument of `GetDeclarations`, here `([], "msg")`,
specifies the context for the auto-completion. Here, we want a completion on a complete name
`msg`, but you could for example use `(["System"; "Collections"], "Generic")` to get a completion list
for a fully qualified namespace.

### Getting parameter information

The next common feature of editors is to provide information about overloads of a method. In our
sample code, we use `String.Concat` which has a number of overloads. We can get the list using
`GetMethods` operation. As previously, this takes zero-indexed offset of the location that we are
interested in (here, right at the end of the `String.Concat` identifier) and we also need to provide
the identifier again (so that the compiler can provide up-to-date information when the source code
changes):

*)
// Get overloads of the String.Concat method
let methods =
    checkFileResults.GetMethods(5, 27, inputLines.[4], Some [ "String"; "Concat" ])

// Print concatenated parameter lists
for mi in methods.Methods do
    [ for p in mi.Parameters do
          for tt in p.Display do
              yield tt.Text ]
    |> String.concat ", "
    |> printfn "%s(%s)" methods.MethodName
(**
The code uses the `Display` property to get the annotation for each parameter. This returns information
such as `arg0: obj` or `params args: obj[]` or `str0: string, str1: string`. We concatenate the parameters
and print a type annotation with the method name.
*)

(**

## Asynchronous and immediate operations

You may have noticed that `CheckFileInProject` is an asynchronous operation.
This indicates that type checking of F# code can take some time.
The F# compiler performs the work in background (automatically) and when
we call `CheckFileInProject` method, it returns an asynchronous operation.

There is also the `CheckFileInProjectIfReady` method. This returns immediately if the
type checking operation can't be started immediately, e.g. if other files in the project
are not yet type-checked. In this case, a background worker might choose to do other
work in the meantime, or give up on type checking the file until the `FileTypeCheckStateIsDirty` event
is raised.

> The [fsharpbinding](https://github.com/fsharp/fsharpbinding) project has more advanced
example of handling the background work where all requests are sent through an F# agent.
This may be a more appropriate for implementing editor support.

*)


(**
Summary
-------

The `CheckFileAnswer` object contains other useful methods that were not covered in this tutorial. You
can use it to get location of a declaration for a given identifier, additional colorization information
(the F# 3.1 colorizes computation builder identifiers & query operators) and others.

Using the FSharpChecker component in multi-project, incremental and interactive editing situations may involve
knowledge of the [FSharpChecker operations queue](queue.html) and the [FSharpChecker caches](caches.html).


Finally, if you are implementing an editor support for an editor that cannot directly call .NET API,
you can call many of the methods discussed here via a command line interface that is available in the
[FSharp.AutoComplete](https://github.com/fsharp/fsharpbinding/tree/master/FSharp.AutoComplete) project.

*)
