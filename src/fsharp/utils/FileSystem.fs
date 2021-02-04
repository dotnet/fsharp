// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

namespace FSharp.Compiler.IO

open System
open System.IO
open System.Reflection

type IFileSystem = 

    /// A shim over File.ReadAllBytes
    abstract ReadAllBytesShim: fileName: string -> byte[] 

    /// A shim over FileStream with FileMode.Open, FileAccess.Read, FileShare.ReadWrite
    abstract FileStreamReadShim: fileName: string -> Stream

    /// A shim over FileStream with FileMode.Create, FileAccess.Write, FileShare.Read
    abstract FileStreamCreateShim: fileName: string -> Stream

    /// A shim over FileStream with FileMode.Open, FileAccess.Write, FileShare.Read
    abstract FileStreamWriteExistingShim: fileName: string -> Stream

    /// Take in a filename with an absolute path, and return the same filename
    /// but canonicalized with respect to extra path separators (e.g. C:\\\\foo.txt) 
    /// and '..' portions
    abstract GetFullPathShim: fileName: string -> string

    /// A shim over Path.IsPathRooted
    abstract IsPathRootedShim: path: string -> bool

    /// A shim over Path.IsInvalidPath
    abstract IsInvalidPathShim: filename: string -> bool

    /// A shim over Path.GetTempPath
    abstract GetTempPathShim : unit -> string

    /// Utc time of the last modification
    abstract GetLastWriteTimeShim: fileName: string -> DateTime

    /// A shim over File.Exists
    abstract SafeExists: fileName: string -> bool

    /// A shim over File.Delete
    abstract FileDelete: fileName: string -> unit

    /// Used to load type providers and located assemblies in F# Interactive
    abstract AssemblyLoadFrom: fileName: string -> Assembly 

    /// Used to load a dependency for F# Interactive and in an unused corner-case of type provider loading
    abstract AssemblyLoad: assemblyName: AssemblyName -> Assembly 

    /// Used to determine if a file will not be subject to deletion during the lifetime of a typical client process.
    abstract IsStableFileHeuristic: fileName: string -> bool


type DefaultFileSystem() =
    interface IFileSystem with

        member _.AssemblyLoadFrom(fileName: string) = 
            Assembly.UnsafeLoadFrom fileName

        member _.AssemblyLoad(assemblyName: AssemblyName) = 
            Assembly.Load assemblyName

        member _.ReadAllBytesShim (fileName: string) = File.ReadAllBytes fileName

        member _.FileStreamReadShim (fileName: string) = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)  :> Stream

        member _.FileStreamCreateShim (fileName: string) = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read, 0x1000, false) :> Stream

        member _.FileStreamWriteExistingShim (fileName: string) = new FileStream(fileName, FileMode.Open, FileAccess.Write, FileShare.Read, 0x1000, false) :> Stream

        member _.GetFullPathShim (fileName: string) = System.IO.Path.GetFullPath fileName

        member _.IsPathRootedShim (path: string) = Path.IsPathRooted path

        member _.IsInvalidPathShim(path: string) = 
            let isInvalidPath(p: string) = 
                String.IsNullOrEmpty p || p.IndexOfAny(Path.GetInvalidPathChars()) <> -1

            let isInvalidFilename(p: string) = 
                String.IsNullOrEmpty p || p.IndexOfAny(Path.GetInvalidFileNameChars()) <> -1

            let isInvalidDirectory(d: string) = 
                d=null || d.IndexOfAny(Path.GetInvalidPathChars()) <> -1

            isInvalidPath path || 
            let directory = Path.GetDirectoryName path
            let filename = Path.GetFileName path
            isInvalidDirectory directory || isInvalidFilename filename

        member _.GetTempPathShim() = Path.GetTempPath()

        member _.GetLastWriteTimeShim (fileName: string) = File.GetLastWriteTimeUtc fileName

        member _.SafeExists (fileName: string) = File.Exists fileName 

        member _.FileDelete (fileName: string) = File.Delete fileName

        member _.IsStableFileHeuristic (fileName: string) = 
            let directory = Path.GetDirectoryName fileName
            directory.Contains("Reference Assemblies/") || 
            directory.Contains("Reference Assemblies\\") || 
            directory.Contains("packages/") || 
            directory.Contains("packages\\") || 
            directory.Contains("lib/mono/")

[<AutoOpen>]
module FileSystemAutoOpens =

    let mutable FileSystem = DefaultFileSystem() :> IFileSystem

    // The choice of 60 retries times 50 ms is not arbitrary. The NTFS FILETIME structure 
    // uses 2 second resolution for LastWriteTime. We retry long enough to surpass this threshold 
    // plus 1 second. Once past the threshold the incremental builder will be able to retry asynchronously based
    // on plain old timestamp checking.
    //
    // The sleep time of 50ms is chosen so that we can respond to the user more quickly for Intellisense operations.
    //
    // This is not run on the UI thread for VS but it is on a thread that must be stopped before Intellisense
    // can return any result except for pending.
    let private retryDelayMilliseconds = 50
    let private numRetries = 60

    let private getReader (filename, codePage: int option, retryLocked: bool) =
        // Retry multiple times since other processes may be writing to this file.
        let rec getSource retryNumber =
          try 
            // Use the .NET functionality to auto-detect the unicode encoding
            let stream = FileSystem.FileStreamReadShim(filename) 
            match codePage with 
            | None -> new  StreamReader(stream,true)
            | Some n -> new  StreamReader(stream,System.Text.Encoding.GetEncoding(n))
          with 
              // We can get here if the file is locked--like when VS is saving a file--we don't have direct
              // access to the HRESULT to see that this is EONOACCESS.
              | :? System.IO.IOException as err when retryLocked && err.GetType() = typeof<System.IO.IOException> -> 
                   // This second check is to make sure the exception is exactly IOException and none of these for example:
                   //   DirectoryNotFoundException 
                   //   EndOfStreamException 
                   //   FileNotFoundException 
                   //   FileLoadException 
                   //   PathTooLongException
                   if retryNumber < numRetries then 
                       System.Threading.Thread.Sleep (retryDelayMilliseconds)
                       getSource (retryNumber + 1)
                   else 
                       reraise()
        getSource 0

    type File with 

        static member ReadBinaryChunk (fileName, start, len) = 
            use stream = FileSystem.FileStreamReadShim fileName
            stream.Seek(int64 start, SeekOrigin.Begin) |> ignore
            let buffer = Array.zeroCreate len 
            let mutable n = 0
            while n < len do 
                n <- n + stream.Read(buffer, n, len-n)
            buffer

        static member OpenReaderAndRetry (filename, codepage, retryLocked)  =
            getReader (filename, codepage, retryLocked)

