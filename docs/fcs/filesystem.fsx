(**
---
title: IFileSystem
category: FSharp.Compiler.Service
categoryindex: 300
index: 1200
---
*)
(*** hide ***)
#I "../../artifacts/bin/FSharp.Compiler.Service/Debug/netstandard2.0"
(**
Compiler Services: Virtualized File System
==========================================

The `FSharp.Compiler.Service` component has a global variable
representing the file system. By setting this variable you can host the compiler in situations where a file system
is not available.

> **NOTE:** The FSharp.Compiler.Service API is subject to change when later versions of the nuget package are published.


Setting the FileSystem
----------------------

In the example below, we set the file system to an implementation which reads from disk
*)
#r "FSharp.Compiler.Service.dll"

open System.IO
open System.Text
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.IO

let defaultFileSystem = FileSystem

let fileName1 = @"c:\mycode\test1.fs" // note, the path doesn't exist
let fileName2 = @"c:\mycode\test2.fs" // note, the path doesn't exist

type MyFileSystem() =
    let file1 =
        """
module File1

let A = 1"""

    let file2 =
        """
module File2
let B = File1.A + File1.A"""

    let files =
        dict [ (fileName1, file1)
               (fileName2, file2) ]

    interface IFileSystem with
        // Implement the service to open files for reading and writing
        member _.OpenFileForReadShim(fileName, ?useMemoryMappedFile: bool, ?shouldShadowCopy: bool) =
            match files.TryGetValue fileName with
            | true, text -> new MemoryStream(Encoding.UTF8.GetBytes(text)) :> Stream
            | _ ->
                defaultFileSystem.OpenFileForReadShim(
                    fileName,
                    ?useMemoryMappedFile = useMemoryMappedFile,
                    ?shouldShadowCopy = shouldShadowCopy
                )

        member _.OpenFileForWriteShim(fileName, ?fileMode: FileMode, ?fileAccess: FileAccess, ?fileShare: FileShare) =
            defaultFileSystem.OpenFileForWriteShim(
                fileName,
                ?fileMode = fileMode,
                ?fileAccess = fileAccess,
                ?fileShare = fileShare
            )

        // Implement the service related to file existence and deletion
        member _.FileExistsShim(fileName) =
            files.ContainsKey(fileName)
            || defaultFileSystem.FileExistsShim(fileName)

        // Implement the service related to temporary paths and file time stamps
        member _.GetTempPathShim() = defaultFileSystem.GetTempPathShim()

        member _.GetLastWriteTimeShim(fileName) =
            defaultFileSystem.GetLastWriteTimeShim(fileName)

        member _.GetFullPathShim(fileName) =
            defaultFileSystem.GetFullPathShim(fileName)

        member _.IsInvalidPathShim(fileName) =
            defaultFileSystem.IsInvalidPathShim(fileName)

        member _.IsPathRootedShim(fileName) =
            defaultFileSystem.IsPathRootedShim(fileName)

        member _.FileDeleteShim(fileName) =
            defaultFileSystem.FileDeleteShim(fileName)

        member _.AssemblyLoader = defaultFileSystem.AssemblyLoader

        member _.GetFullFilePathInDirectoryShim dir fileName =
            defaultFileSystem.GetFullFilePathInDirectoryShim dir fileName

        member _.NormalizePathShim(path) =
            defaultFileSystem.NormalizePathShim(path)

        member _.GetDirectoryNameShim(path) =
            defaultFileSystem.GetDirectoryNameShim(path)

        member _.GetCreationTimeShim(path) =
            defaultFileSystem.GetCreationTimeShim(path)

        member _.CopyShim(src, dest, overwrite) =
            defaultFileSystem.CopyShim(src, dest, overwrite)

        member _.DirectoryCreateShim(path) =
            defaultFileSystem.DirectoryCreateShim(path)

        member _.DirectoryExistsShim(path) =
            defaultFileSystem.DirectoryExistsShim(path)

        member _.DirectoryDeleteShim(path) =
            defaultFileSystem.DirectoryDeleteShim(path)

        member _.EnumerateFilesShim(path, pattern) =
            defaultFileSystem.EnumerateFilesShim(path, pattern)

        member _.EnumerateDirectoriesShim(path) =
            defaultFileSystem.EnumerateDirectoriesShim(path)

        member _.IsStableFileHeuristic(path) =
            defaultFileSystem.IsStableFileHeuristic(path)

let myFileSystem = MyFileSystem()
FileSystem <- MyFileSystem()

(**

Doing a compilation with the FileSystem
---------------------------------------

*)

let checker = FSharpChecker.Create()

let projectOptions =
    let sysLib nm =
        if System.Environment.OSVersion.Platform = System.PlatformID.Win32NT then // file references only valid on Windows
            System.Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFilesX86)
            + @"\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0\"
            + nm
            + ".dll"
        else
            let sysDir =
                System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory()

            let (++) a b = System.IO.Path.Combine(a, b)
            sysDir ++ nm + ".dll"

    let fsCore4300 () =
        if System.Environment.OSVersion.Platform = System.PlatformID.Win32NT then // file references only valid on Windows
            System.Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFilesX86)
            + @"\Reference Assemblies\Microsoft\FSharp\.NETFramework\v4.0\4.3.0.0\FSharp.Core.dll"
        else
            sysLib "FSharp.Core"

    let allFlags =
        [| "--simpleresolution"
           "--noframework"
           "--debug:full"
           "--define:DEBUG"
           "--optimize-"
           "--doc:test.xml"
           "--warn:3"
           "--fullpaths"
           "--flaterrors"
           "--target:library"
           let references =
               [ sysLib "mscorlib"
                 sysLib "System"
                 sysLib "System.Core"
                 fsCore4300 () ]

           for r in references do
               "-r:" + r |]

    { ProjectFileName = @"c:\mycode\compilation.fsproj" // Make a name that is unique in this directory.
      ProjectId = None
      SourceFiles = [| fileName1; fileName2 |]
      OriginalLoadReferences = []
      Stamp = None
      OtherOptions = allFlags
      ReferencedProjects = [||]
      IsIncompleteTypeCheckEnvironment = false
      UseScriptResolutionRules = true
      LoadTime = System.DateTime.Now // Note using 'Now' forces reloading
      UnresolvedReferences = None }

let results =
    checker.ParseAndCheckProject(projectOptions)
    |> Async.RunSynchronously

results.Diagnostics
results.AssemblySignature.Entities.Count //2

results.AssemblySignature.Entities.[0]
    .MembersFunctionsAndValues
    .Count

results.AssemblySignature.Entities.[0]
    .MembersFunctionsAndValues.[0]
    .DisplayName

(**
Summary
-------
In this tutorial, we've seen how to globally customize the view of the file system used by the FSharp.Compiler.Service
component.

At the time of writing, the following System.IO operations are not considered part of the virtualized file system API.
Future iterations on the compiler service implementation may add these to the API.

  - Path.Combine
  - Path.DirectorySeparatorChar
  - Path.GetDirectoryName
  - Path.GetFileName
  - Path.GetFileNameWithoutExtension
  - Path.HasExtension
  - Path.GetRandomFileName (used only in generation compiled win32 resources in assemblies)

**NOTE:** Several operations in the `SourceCodeServices` API accept the contents of a file to parse
or check as a parameter, in addition to a file name. In these cases, the file name is only used for
error reporting.

**NOTE:** Type provider components do not use the virtualized file system.

**NOTE:** The compiler service may use MSBuild for assembly resolutions unless `--simpleresolution` is
provided. When using the `FileSystem` API you will normally want to specify `--simpleresolution` as one
of your compiler flags. Also specify `--noframework`.  You will need to supply explicit resolutions of all
referenced .NET assemblies.

*)