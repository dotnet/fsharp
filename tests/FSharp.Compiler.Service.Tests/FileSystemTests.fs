// FileSystem is a global shared resource.
[<Xunit.Collection(nameof FSharp.Test.NotThreadSafeResourceCollection)>]
module FSharp.Compiler.Service.Tests.FileSystemTests

open Xunit
open FSharp.Test.Assert
open FSharp.Test
open System
open System.IO
open System.Text
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.IO
open FSharp.Compiler.Service.Tests.Common


let fileName1 = @"c:\mycode\test1.fs" // note, the path doesn' exist
let fileName2 = @"c:\mycode\test2.fs" // note, the path doesn' exist

#nowarn "57"

let file1 = """
module File1

let A = 1"""

let file2 = """
module File2
let B = File1.A + File1.A"""

type internal MyFileSystem() =
    inherit DefaultFileSystem()
        static member FilesCache = dict [(fileName1, file1); (fileName2, file2)]
        // Implement the service to open files for reading and writing
        override _.OpenFileForReadShim(filePath, ?useMemoryMappedFile: bool, ?shouldShadowCopy: bool) =
            let shouldShadowCopy = defaultArg shouldShadowCopy false
            let useMemoryMappedFile = defaultArg useMemoryMappedFile false
            match MyFileSystem.FilesCache.TryGetValue filePath with
            | true, text ->
                new MemoryStream(Encoding.UTF8.GetBytes(text)) :> Stream
            | _ -> base.OpenFileForReadShim(filePath, useMemoryMappedFile, shouldShadowCopy)
        override _.FileExistsShim(fileName) = MyFileSystem.FilesCache.ContainsKey(fileName) || base.FileExistsShim(fileName)

let useFileSystemShim (shim: IFileSystem) =
    let originalShim = FileSystem
    FileSystem <- shim
    { new IDisposable with member x.Dispose() = FileSystem <- originalShim }

// .NET Core SKIPPED: need to check if these tests can be enabled for .NET Core testing of FSharp.Compiler.Service"
[<FactForDESKTOP>]
let ``FileSystem compilation test``() =
  if Environment.OSVersion.Platform = PlatformID.Win32NT then // file references only valid on Windows
    use myFileSystem = useFileSystemShim (MyFileSystem())

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
          LoadTime = DateTime.Now // Not 'now', we don't want to force reloading
          UnresolvedReferences = None
          OriginalLoadReferences = []
          Stamp = None }

    let results = checker.ParseAndCheckProject(projectOptions) |> Async.RunImmediate

    results.Diagnostics.Length |> shouldEqual 0
    results.AssemblySignature.Entities.Count |> shouldEqual 2
    results.AssemblySignature.Entities[0].MembersFunctionsAndValues.Count |> shouldEqual 1
    results.AssemblySignature.Entities[0].MembersFunctionsAndValues[0].DisplayName |> shouldEqual "B"

let checkEmptyScriptWithFsShim () =
    let shim = DefaultFileSystem()
    let ref: WeakReference = WeakReference(shim)

    use _ = useFileSystemShim shim
    getParseAndCheckResults "" |> ignore

    ref

[<Fact>]
let ``File system shim should not leak`` () =
    let shimRef = checkEmptyScriptWithFsShim ()

    GC.Collect()
    GC.WaitForPendingFinalizers()

    Assert.False(shimRef.IsAlive)
