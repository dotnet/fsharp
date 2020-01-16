#if INTERACTIVE
#r "../../artifacts/bin/fcs/net461/FSharp.Compiler.Service.dll" // note, build FSharp.Compiler.Service.Tests.fsproj to generate this, this DLL has a public API so can be used from F# Interactive
#r "../../artifacts/bin/fcs/net461/nunit.framework.dll"
#load "FsUnit.fs"
#load "Common.fs"
#else
module FSharp.Compiler.Service.Tests.ExtensionTypingProvider
#endif

open System
open System.IO
open FsUnit
open NUnit.Framework
open FSharp.Compiler.ExtensionTyping
open FSharp.Compiler.Range
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AbstractIL.Internal.Library
open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler
open FSharp.Compiler.SourceCodeServices
  
module internal Project1A = 
    open FSharp.Compiler.SourceCodeServices
    let fileName1 = Path.ChangeExtension(Path.GetTempFileName(), ".fs")
    let baseName = Path.GetTempFileName()
    let dllName = Path.ChangeExtension(baseName, ".dll")
    let projFileName = Path.ChangeExtension(baseName, ".fsproj")
    let fileSource1 = """module Project1A"""
    File.WriteAllText(fileName1, fileSource1)

    let cleanFileName a = if a = fileName1 then "file1" else "??"

    let fileNames = [fileName1]
    let args = mkProjectCommandLineArgs (dllName, fileNames)
    let options = 
          {ProjectFileName = __SOURCE_DIRECTORY__ + @"/data/TypeProviderConsole/TypeProviderConsole.fsproj";
           ProjectId = None
           SourceFiles = [|__SOURCE_DIRECTORY__ + @"/data/TypeProviderConsole/Program.fs"|];
           Stamp = None
           OtherOptions =
            [|yield "--simpleresolution";
              yield "--noframework";
              yield "--out:" + __SOURCE_DIRECTORY__ + @"/data/TypeProviderConsole/bin/Debug/TypeProviderConsole.exe";
              yield "--doc:" + __SOURCE_DIRECTORY__ + @"/data/TypeProviderConsole/bin/Debug/TypeProviderConsole.xml";
              yield "--subsystemversion:6.00"; 
              yield "--highentropyva+"; 
              yield "--fullpaths";
              yield "--flaterrors"; 
              yield "--target:exe"; 
              yield "--define:DEBUG"; 
              yield "--define:TRACE";
              yield "--debug+"; 
              yield "--optimize-"; 
              yield "--tailcalls-"; 
              yield "--debug:full";
              yield "--platform:anycpu";
              for r in mkStandardProjectReferences () do
                  yield "-r:" + r
              yield "-r:" + __SOURCE_DIRECTORY__ + @"/data/TypeProviderLibrary/TypeProviderLibrary.dll"|];
           ReferencedProjects =
            [|(__SOURCE_DIRECTORY__ + @"/data/TypeProviderLibrary/TypeProviderLibrary.dll",
               {ProjectFileName = __SOURCE_DIRECTORY__ + @"/data/TypeProviderLibrary/TypeProviderLibrary.fsproj";
                ProjectId = None
                SourceFiles = [|__SOURCE_DIRECTORY__ + @"/data/TypeProviderLibrary/Library1.fs"|];
                Stamp = None
                OtherOptions =
                 [|yield "--simpleresolution"; 
                   yield "--noframework";
                   yield "--out:" + __SOURCE_DIRECTORY__ + @"/data/TypeProviderLibrary/TypeProviderLibrary.dll";
                   yield "--doc:" + __SOURCE_DIRECTORY__ + @"/data/TypeProviderLibrary/bin/Debug/TypeProviderLibrary.xml";
                   yield "--subsystemversion:6.00"; 
                   yield "--highentropyva+"; 
                   yield "--fullpaths";
                   yield "--flaterrors"; 
                   yield "--target:library"; 
                   yield "--define:DEBUG";
                   yield "--define:TRACE"; 
                   yield "--debug+"; 
                   yield "--optimize-"; 
                   yield "--tailcalls-";
                   yield "--debug:full"; 
                   yield "--platform:anycpu";
                   for r in mkStandardProjectReferences () do
                       yield "-r:" + r
                   yield "-r:" + __SOURCE_DIRECTORY__ + @"/data/TypeProviderLibrary/FSharp.Data.TypeProviders.dll"; 
                  |];
                ReferencedProjects = [||];
                IsIncompleteTypeCheckEnvironment = false;
                UseScriptResolutionRules = false;
                LoadTime = System.DateTime.Now
                UnresolvedReferences = None;
                OriginalLoadReferences = [];
                ExtraProjectInfo = None;})|];
           IsIncompleteTypeCheckEnvironment = false;
           UseScriptResolutionRules = false;
           LoadTime = System.DateTime.Now
           UnresolvedReferences = None;
           OriginalLoadReferences = [];
           ExtraProjectInfo = None;}

[<Test>]
let ``Extension typing shim gets requests`` () =
    let mutable gotRequest = false
    let extensionTypingProvider =
        { new IExtensionTypingProvider with
            member this.InstantiateTypeProvidersOfAssembly
                    (runTimeAssemblyFileName: string, 
                     ilScopeRefOfRuntimeAssembly: ILScopeRef, 
                     designTimeAssemblyNameString: string, 
                     resolutionEnvironment: ResolutionEnvironment, 
                     isInvalidationSupported: bool, 
                     isInteractive: bool, 
                     systemRuntimeContainsType : string -> bool, 
                     systemRuntimeAssemblyVersion : System.Version, 
                     compilerToolPaths: string list,
                     m:range) =
                gotRequest <- true
                []
                
          interface IDisposable with
            member this.Dispose() = ()
        }
        
    Shim.ExtensionTypingProvider <- extensionTypingProvider

    checker.ParseAndCheckProject(Project1A.options) |> Async.RunSynchronously |> ignore
    gotRequest |> should be True