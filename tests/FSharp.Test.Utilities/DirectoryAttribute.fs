namespace FSharp.Test

open System
open System.IO
open System.Reflection

open Xunit
open Xunit.Sdk

open FSharp.Compiler.IO
open FSharp.Test.Compiler
open FSharp.Test.Utilities
open TestFramework

/// Attribute to use with Xunit's TheoryAttribute.
/// Takes a directory, relative to current test suite's root.
/// Returns a CompilationUnit with encapsulated source code, error baseline and IL baseline (if any).
/// 
/// CRITICAL BLOCKER FOR XUNIT3 MIGRATION:
/// F# compiler cannot resolve DataAttribute or IDataAttribute from xunit.v3.core.dll despite:
/// - Explicit package reference to xunit.v3.extensibility.core
/// - Direct DLL reference to xunit.v3.core.dll  
/// - Types confirmed to exist via ILSpy inspection
/// 
/// This appears to be an F# compiler/xUnit3 compatibility issue.
/// Workaround attempts tried:
/// 1. Inherit from DataAttribute directly - F# can't find type
/// 2. Implement IDataAttribute interface - F# can't find interface
/// 3. Inherit from InlineDataAttribute - sealed type
/// 4. Create C# helper class - too complex for F# project
/// 5. Use MemberDataAttribute - incompatible pattern
/// 
/// TODO: This requires either:
/// - Fix in F# compiler to properly resolve types from xunit.v3.core.dll
/// - Convert all tests using DirectoryAttribute to use ClassData/MemberData patterns (~100 tests)
/// - Wait for xUnit3/F# compatibility improvements
///
/// Temporarily disabled to unblock other xUnit3 migration work.
(*
[<AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)>]
[<NoComparison; NoEquality>]
type DirectoryAttribute(dir: string) =
    inherit DataAttribute()
    do if String.IsNullOrWhiteSpace(dir) then
            invalidArg "dir" "Directory cannot be null, empty or whitespace only."

    let directoryPath = normalizePathSeparator (Path.GetFullPath(dir))
    let mutable baselineSuffix = ""
    let mutable includes = Array.empty<string>
    
    new([<ParamArray>] dirs: string[]) = DirectoryAttribute(Path.Combine(dirs) : string)
    
    member _.BaselineSuffix with get() = baselineSuffix and set v = baselineSuffix <- v
    member _.Includes with get() = includes and set v = includes <- v

    override _.GetData _ = createCompilationUnitForFiles baselineSuffix directoryPath includes
*)
