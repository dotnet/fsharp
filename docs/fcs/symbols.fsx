(*** hide ***)
#I "../../artifacts/bin/FSharp.Compiler.Service/Debug/netstandard2.0"
(**
Compiler Services: Working with symbols
============================================

This tutorial demonstrates how to work with symbols provided by the F# compiler. See also [project wide analysis](project.html)
for information on symbol references.

> **NOTE:** The FSharp.Compiler.Service API is subject to change when later versions of the nuget package are published.

As usual we start by referencing `FSharp.Compiler.Service.dll`, opening the relevant namespace and creating an instance
of `FSharpChecker`:

*)
// Reference F# compiler API
#r "FSharp.Compiler.Service.dll"

open System
open System.IO
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Symbols
open FSharp.Compiler.Text

// Create an interactive checker instance 
let checker = FSharpChecker.Create()

(**

We now perform type checking on the specified input:

*)

let parseAndTypeCheckSingleFile (file, input) = 
    // Get context representing a stand-alone (script) file
    let projOptions, errors = 
        checker.GetProjectOptionsFromScript(file, input)
        |> Async.RunSynchronously

    let parseFileResults, checkFileResults = 
        checker.ParseAndCheckFileInProject(file, 0, input, projOptions) 
        |> Async.RunSynchronously

    // Wait until type checking succeeds (or 100 attempts)
    match checkFileResults with
    | FSharpCheckFileAnswer.Succeeded(res) -> parseFileResults, res
    | res -> failwithf "Parsing did not finish... (%A)" res

let file = "/home/user/Test.fsx"

(**
## Getting resolved signature information about the file

After type checking a file, you can access the inferred signature of a project up to and including the
checking of the given file through the `PartialAssemblySignature` property of the `TypeCheckResults`.

The full signature information is available for modules, types, attributes, members, values, functions, 
union cases, record types, units of measure and other F# language constructs.

The typed expression trees are also available, see [typed tree tutorial](typedtree.html).

*)

let input2 = 
      """
[<System.CLSCompliant(true)>]
let foo(x, y) = 
    let msg = String.Concat("Hello"," ","world")
    if true then 
        printfn "x = %d, y = %d" x y 
        printfn "%s" msg

type C() = 
    member x.P = 1
      """
let parseFileResults, checkFileResults = 
    parseAndTypeCheckSingleFile(file, SourceText.ofString input2)

(**
Now get the partial assembly signature for the code:
*)
let partialAssemblySignature = checkFileResults.PartialAssemblySignature
    
partialAssemblySignature.Entities.Count = 1  // one entity
    

(**
Now get the entity that corresponds to the module containing the code:
*)
let moduleEntity = partialAssemblySignature.Entities.[0]

moduleEntity.DisplayName = "Test"

(**
Now get the entity that corresponds to the type definition in the code:
*)
let classEntity = moduleEntity.NestedEntities.[0]

(**
Now get the value that corresponds to the function defined in the code:
*)
let fnVal = moduleEntity.MembersFunctionsAndValues.[0]

(**
Now look around at the properties describing the function value. All of the following evaluate to `true`:
*)
fnVal.Attributes.Count = 1
fnVal.CurriedParameterGroups.Count // 1
fnVal.CurriedParameterGroups.[0].Count // 2
fnVal.CurriedParameterGroups.[0].[0].Name // "x"
fnVal.CurriedParameterGroups.[0].[1].Name // "y"
fnVal.DeclarationLocation.StartLine // 3
fnVal.DisplayName // "foo"
fnVal.DeclaringEntity.Value.DisplayName // "Test"
fnVal.DeclaringEntity.Value.DeclarationLocation.StartLine // 1
fnVal.GenericParameters.Count // 0
fnVal.InlineAnnotation // FSharpInlineAnnotation.OptionalInline
fnVal.IsActivePattern // false
fnVal.IsCompilerGenerated // false
fnVal.IsDispatchSlot // false
fnVal.IsExtensionMember // false
fnVal.IsPropertyGetterMethod // false
fnVal.IsImplicitConstructor // false
fnVal.IsInstanceMember // false
fnVal.IsMember // false
fnVal.IsModuleValueOrMember // true
fnVal.IsMutable // false
fnVal.IsPropertySetterMethod // false
fnVal.IsTypeFunction // false

(**
Now look at the type of the function if used as a first class value. (Aside: the `CurriedParameterGroups` property contains
more information like the names of the arguments.)
*)
fnVal.FullType // int * int -> unit
fnVal.FullType.IsFunctionType // int * int -> unit
fnVal.FullType.GenericArguments.[0] // int * int 
fnVal.FullType.GenericArguments.[0].IsTupleType // int * int 
let argTy1 = fnVal.FullType.GenericArguments.[0].GenericArguments.[0]

argTy1.TypeDefinition.DisplayName // int

(**
OK, so we got an object representation of the type `int * int -> unit`, and we have seen the first 'int'. We can find out more about the
type 'int' as follows, determining that it is a named type, which is an F# type abbreviation, `type int = int32`:
*)

argTy1.HasTypeDefinition
argTy1.TypeDefinition.IsFSharpAbbreviation // "int"

(**
We can now look at the right-hand-side of the type abbreviation, which is the type `int32`:
*)

let argTy1b = argTy1.TypeDefinition.AbbreviatedType
argTy1b.TypeDefinition.Namespace // Some "Microsoft.FSharp.Core" 
argTy1b.TypeDefinition.CompiledName // "int32" 

(**
Again we can now look through the type abbreviation `type int32 = System.Int32` to get the 
full information about the type:
*)
let argTy1c = argTy1b.TypeDefinition.AbbreviatedType
argTy1c.TypeDefinition.Namespace // Some "SystemCore" 
argTy1c.TypeDefinition.CompiledName // "Int32" 

(**
The type checking results for a file also contain information extracted from the project (or script) options
used in the compilation, called the `ProjectContext`:
*)
let projectContext = checkFileResults.ProjectContext
    
for assembly in projectContext.GetReferencedAssemblies() do
    match assembly.FileName with 
    | None -> printfn "compilation referenced an assembly without a file" 
    | Some s -> printfn "compilation references assembly '%s'" s
    

(**
**Notes:**

  - If incomplete code is present, some or all of the attributes may not be quite as expected.
  - If some assembly references are missing (which is actually very, very common), then 'IsUnresolved'  may
    be true on values, members and/or entities related to external assemblies.  You should be sure to make your
    code robust against IsUnresolved exceptions.

*)

(**

## Getting symbolic information about whole projects

To check whole projects, create a checker, then call `parseAndCheckScript`. In this case, we just check 
the project for a single script. By specifying a different "projOptions" you can create 
a specification of a larger project.
*)
let parseAndCheckScript (file, input) = 
    let projOptions, errors = 
        checker.GetProjectOptionsFromScript(file, input)
        |> Async.RunSynchronously

    checker.ParseAndCheckProject(projOptions) |> Async.RunSynchronously

(**
Now do it for a particular input:
*)

let tmpFile = Path.ChangeExtension(System.IO.Path.GetTempFileName() , "fs")
File.WriteAllText(tmpFile, input2)

let projectResults = parseAndCheckScript(tmpFile, SourceText.ofString input2)


(**
Now look at the results:
*)

let assemblySig = projectResults.AssemblySignature
    
assemblySig.Entities.Count = 1  // one entity
assemblySig.Entities.[0].Namespace  // one entity
assemblySig.Entities.[0].DisplayName // "Tmp28D0"
assemblySig.Entities.[0].MembersFunctionsAndValues.Count // 1 
assemblySig.Entities.[0].MembersFunctionsAndValues.[0].DisplayName // "foo" 
    
