#if INTERACTIVE
#r "../../artifacts/bin/fcs/net461/FSharp.Compiler.Service.dll" // note, build FSharp.Compiler.Service.Tests.fsproj to generate this, this DLL has a public API so can be used from F# Interactive
#r "../../artifacts/bin/fcs/net461/nunit.framework.dll"
#load "FsUnit.fs"
#load "Common.fs"
#else
module Tests.Service.FileSystemTests
#endif


open NUnit.Framework
open FsUnit
open System
open System.IO
open System.Collections.Generic
open System.Text
open FSharp.Compiler
open FSharp.Compiler.SourceCodeServices
open FSharp.Compiler.AbstractIL.Internal.Library
open FSharp.Compiler.Service.Tests.Common

let fileName1 = @"c:\mycode\test1.fs" // note, the path doesn' exist
let fileName2 = @"c:\mycode\test2.fs" // note, the path doesn' exist

type internal MyFileSystem(defaultFileSystem:IFileSystem) = 
    let file1 = """
module File1

let A = 1"""
    let file2 = """
module File2
let B = File1.A + File1.A"""
    let files = dict [(fileName1, file1); (fileName2, file2)]

    interface IFileSystem with
        // Implement the service to open files for reading and writing
        member __.FileStreamReadShim(fileName) = 
            match files.TryGetValue fileName with
            | true, text -> new MemoryStream(Encoding.UTF8.GetBytes(text)) :> Stream
            | _ -> defaultFileSystem.FileStreamReadShim(fileName)
            
        member __.FileStreamCreateShim(fileName) = 
            defaultFileSystem.FileStreamCreateShim(fileName)

        member __.IsStableFileHeuristic(fileName) = 
            defaultFileSystem.IsStableFileHeuristic(fileName)

        member __.FileStreamWriteExistingShim(fileName) = 
            defaultFileSystem.FileStreamWriteExistingShim(fileName)

        member __.ReadAllBytesShim(fileName) = 
            match files.TryGetValue fileName with
            | true, text -> Encoding.UTF8.GetBytes(text)
            | _ -> defaultFileSystem.ReadAllBytesShim(fileName)

        // Implement the service related to temporary paths and file time stamps
        member __.GetTempPathShim() = defaultFileSystem.GetTempPathShim()
        member __.GetLastWriteTimeShim(fileName) = defaultFileSystem.GetLastWriteTimeShim(fileName)
        member __.GetFullPathShim(fileName) = defaultFileSystem.GetFullPathShim(fileName)
        member __.IsInvalidPathShim(fileName) = defaultFileSystem.IsInvalidPathShim(fileName)
        member __.IsPathRootedShim(fileName) = defaultFileSystem.IsPathRootedShim(fileName)

        // Implement the service related to file existence and deletion
        member __.SafeExists(fileName) = files.ContainsKey(fileName) || defaultFileSystem.SafeExists(fileName)
        member __.FileDelete(fileName) = defaultFileSystem.FileDelete(fileName)

        // Implement the service related to assembly loading, used to load type providers
        // and for F# interactive.
        member __.AssemblyLoadFrom(fileName) = defaultFileSystem.AssemblyLoadFrom fileName
        member __.AssemblyLoad(assemblyName) = defaultFileSystem.AssemblyLoad assemblyName 

let UseMyFileSystem() = 
    let myFileSystem = MyFileSystem(Shim.FileSystem)
    Shim.FileSystem <- myFileSystem
    { new IDisposable with member x.Dispose() = Shim.FileSystem <- myFileSystem }


[<Test>]
#if NETCOREAPP2_0
[<Ignore("SKIPPED: need to check if these tests can be enabled for .NET Core testing of FSharp.Compiler.Service")>]
#endif
let ``FileSystem compilation test``() = 
  if System.Environment.OSVersion.Platform = System.PlatformID.Win32NT then // file references only valid on Windows 
    use myFileSystem =  UseMyFileSystem()
    let programFilesx86Folder = System.Environment.GetEnvironmentVariable("PROGRAMFILES(X86)")

    let projectOptions = 
        let allFlags = 
            [| yield "--simpleresolution"; 
               yield "--noframework"; 
               yield "--debug:full"; 
               yield "--define:DEBUG"; 
               yield "--optimize-"; 
               yield "--doc:test.xml"; 
               yield "--warn:3"; 
               yield "--fullpaths"; 
               yield "--flaterrors"; 
               yield "--target:library"; 
               for r in [ sysLib "mscorlib"; sysLib "System"; sysLib "System.Core"; fsCoreDefaultReference() ] do 
                   yield "-r:" + r |]
 
        { ProjectFileName = @"c:\mycode\compilation.fsproj" // Make a name that is unique in this directory.
          ProjectId = None
          SourceFiles = [| fileName1; fileName2 |]
          OtherOptions = allFlags 
          ReferencedProjects = [| |];
          IsIncompleteTypeCheckEnvironment = false
          UseScriptResolutionRules = true 
          LoadTime = System.DateTime.Now // Not 'now', we don't want to force reloading
          UnresolvedReferences = None 
          OriginalLoadReferences = []
          ExtraProjectInfo = None 
          Stamp = None }

    let results = checker.ParseAndCheckProject(projectOptions) |> Async.RunSynchronously

    results.Errors.Length |> shouldEqual 0
    results.AssemblySignature.Entities.Count |> shouldEqual 2
    results.AssemblySignature.Entities.[0].MembersFunctionsAndValues.Count |> shouldEqual 1
    results.AssemblySignature.Entities.[0].MembersFunctionsAndValues.[0].DisplayName |> shouldEqual "B"

