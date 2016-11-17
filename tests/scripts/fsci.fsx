#load @"../../src/scripts/scriptlib.fsx"
#load "crackProjectJson.fsx"

open System
open System.IO
open System.Diagnostics

let root = Path.GetFullPath(__SOURCE_DIRECTORY__ ++ ".." ++ "..")
let Platform = getCmdLineArg "--platform:"    "win7-x64"
let ProjectJsonLock    = getCmdLineArg "--projectJsonLock:"    (root ++ "tests" ++ "fsharp" ++ "project.lock.json")
let PackagesDir        = getCmdLineArg "--packagesDir:"        (root ++ "packages")
let FrameworkName      = getCmdLineArg "--framework:"      ".NETCoreApp,Version=v1.0"
let Verbosity          = getCmdLineArg "--v:"                  "quiet"
let CompilerPathOpt    = getCmdLineArgOptional "--compilerPath:"       
let Flavour            = getCmdLineArg "--flavour:"       "release"
let ExtraArgs          = getCmdLineExtraArgs (fun x -> x.StartsWith "--")

let CompilerPath       = defaultArg CompilerPathOpt (root ++ "tests" ++ "testbin" ++ Flavour ++ "coreclr" ++ "fsc" ++ Platform)
let Win32Manifest = CompilerPath ++ "default.win32manifest"

let isVerbose = Verbosity = "verbose"

let dependencies = CrackProjectJson.collectReferences (isVerbose, PackagesDir, FrameworkName + "/" + Platform, ProjectJsonLock, false) |> Seq.toArray

let executeProcessNoRedirect filename arguments =
    if isVerbose then 
       printfn "%s %s" filename arguments
    let info = ProcessStartInfo(Arguments=arguments, UseShellExecute=false, 
                                RedirectStandardOutput=true, RedirectStandardError=true,RedirectStandardInput=true,
                                CreateNoWindow=true, FileName=filename)
    let p = new Process(StartInfo=info)
    if p.Start() then

        async { try 
                  let buffer = Array.zeroCreate 4096
                  while not p.StandardOutput.EndOfStream do 
                    let n = p.StandardOutput.Read(buffer, 0, buffer.Length)
                    if n > 0 then System.Console.Out.Write(buffer, 0, n)
                with _ -> () } |> Async.Start
        async { try 
                  let buffer = Array.zeroCreate 4096
                  while not p.StandardError.EndOfStream do 
                    let n = p.StandardError.Read(buffer, 0, buffer.Length)
                    if n > 0 then System.Console.Error.Write(buffer, 0, n)
                with _ -> () } |> Async.Start
        async { try 
                  while true do 
                    let c = System.Console.In.ReadLine()
                    p.StandardInput.WriteLine(c)
                with _ -> () } |> Async.Start
        p.WaitForExit()
        p.ExitCode
    else
        0

let executeCompiler references =
    let Win32manifest=Path.Combine(CompilerPath, "default.win32manifest")
    let listToPrefixedSpaceSeperatedString prefix list = list |> Seq.fold(fun a t -> sprintf "%s %s%s" a prefix t) ""
    let listToSpaceSeperatedString list = list |> Seq.fold(fun a t -> sprintf "%s %s" a t) ""
    let addReferenceSwitch list = list |> Seq.map(fun i -> sprintf "--reference:%s" i)
    let arguments = sprintf @"%s --noframework %s -r:%s %s"
                            (CompilerPath ++ "fsi.exe")
                            (listToSpaceSeperatedString (addReferenceSwitch references)) 
                            (CompilerPath ++ "FSharp.Core.dll")
                            (String.concat " " ExtraArgs)

    printfn "%s %s" (CompilerPath ++ "CoreRun.exe") arguments
    executeProcessNoRedirect (CompilerPath ++ "CoreRun.exe") arguments

exit (executeCompiler dependencies)

(*
  [|"packages\Microsoft.CSharp\4.0.1\ref\netstandard1.0\Microsoft.CSharp.dll";
    "packages\Microsoft.DiaSymReader\1.0.8\lib\netstandard1.1\Microsoft.DiaSymReader.dll";
    "packages\Microsoft.DiaSymReader.PortablePdb\1.1.0\lib\netstandard1.1\Microsoft.DiaSymReader.PortablePdb.dll";
    "packages\Microsoft.VisualBasic\10.0.1\ref\netstandard1.1\Microsoft.VisualBasic.dll";
    "packages\Microsoft.Win32.Primitives\4.0.1\ref\netstandard1.3\Microsoft.Win32.Primitives.dll";
    "packages\System.AppContext\4.1.0\ref\netstandard1.3\System.AppContext.dll";
    "packages\System.Buffers\4.0.0\lib\netstandard1.1\System.Buffers.dll";
    "packages\System.Collections\4.0.11\ref\netstandard1.3\System.Collections.dll";
    "packages\System.Collections.Concurrent\4.0.12\ref\netstandard1.3\System.Collections.Concurrent.dll";
    "packages\System.Collections.Immutable\1.2.0\lib\netstandard1.0\System.Collections.Immutable.dll";
    "packages\System.ComponentModel\4.0.1\ref\netstandard1.0\System.ComponentModel.dll";
    "packages\System.ComponentModel.Annotations\4.1.0\ref\netstandard1.4\System.ComponentModel.Annotations.dll";
    "packages\System.Console\4.0.0\ref\netstandard1.3\System.Console.dll";
    "packages\System.Diagnostics.Debug\4.0.11\ref\netstandard1.3\System.Diagnostics.Debug.dll";
    "packages\System.Diagnostics.DiagnosticSource\4.0.0\lib\netstandard1.3\System.Diagnostics.DiagnosticSource.dll";
    "packages\System.Diagnostics.Process\4.1.0\ref\netstandard1.4\System.Diagnostics.Process.dll";
    "packages\System.Diagnostics.Tools\4.0.1\ref\netstandard1.0\System.Diagnostics.Tools.dll";
    "packages\System.Diagnostics.TraceSource\4.0.0\ref\netstandard1.3\System.Diagnostics.TraceSource.dll";
    "packages\System.Diagnostics.Tracing\4.1.0\ref\netstandard1.5\System.Diagnostics.Tracing.dll";
    "packages\System.Dynamic.Runtime\4.0.11\ref\netstandard1.3\System.Dynamic.Runtime.dll";
    "packages\System.Globalization\4.0.11\ref\netstandard1.3\System.Globalization.dll";
    "packages\System.Globalization.Calendars\4.0.1\ref\netstandard1.3\System.Globalization.Calendars.dll";
    "packages\System.Globalization.Extensions\4.0.1\ref\netstandard1.3\System.Globalization.Extensions.dll";
    "packages\System.IO\4.1.0\ref\netstandard1.5\System.IO.dll";
    "packages\System.IO.Compression\4.1.0\ref\netstandard1.3\System.IO.Compression.dll";
    "packages\System.IO.Compression.ZipFile\4.0.1\ref\netstandard1.3\System.IO.Compression.ZipFile.dll";
    "packages\System.IO.FileSystem\4.0.1\ref\netstandard1.3\System.IO.FileSystem.dll";
    "packages\System.IO.FileSystem.Primitives\4.0.1\ref\netstandard1.3\System.IO.FileSystem.Primitives.dll";
    "packages\System.IO.FileSystem.Watcher\4.0.0\ref\netstandard1.3\System.IO.FileSystem.Watcher.dll";
    "packages\System.IO.MemoryMappedFiles\4.0.0\ref\netstandard1.3\System.IO.MemoryMappedFiles.dll";
    "packages\System.IO.UnmanagedMemoryStream\4.0.1\ref\netstandard1.3\System.IO.UnmanagedMemoryStream.dll";
    "packages\System.Linq\4.1.0\ref\netstandard1.0\System.Linq.dll";
    "packages\System.Linq.Expressions\4.1.0\ref\netstandard1.3\System.Linq.Expressions.dll";
    "packages\System.Linq.Parallel\4.0.1\ref\netstandard1.1\System.Linq.Parallel.dll";
    "packages\System.Linq.Queryable\4.0.1\ref\netstandard1.0\System.Linq.Queryable.dll";
    "packages\System.Net.Http\4.1.0\ref\netstandard1.3\System.Net.Http.dll";
    "packages\System.Net.NameResolution\4.0.0\ref\netstandard1.3\System.Net.NameResolution.dll";
    "packages\System.Net.Primitives\4.0.11\ref\netstandard1.3\System.Net.Primitives.dll";
    "packages\System.Net.Requests\4.0.11\ref\netstandard1.3\System.Net.Requests.dll";
    "packages\System.Net.Security\4.0.0\ref\netstandard1.3\System.Net.Security.dll";
    "packages\System.Net.Sockets\4.1.0\ref\netstandard1.3\System.Net.Sockets.dll";
    "packages\System.Net.WebHeaderCollection\4.0.1\ref\netstandard1.3\System.Net.WebHeaderCollection.dll";
    "packages\System.Numerics.Vectors\4.1.1\ref\netstandard1.0\System.Numerics.Vectors.dll";
    "packages\System.ObjectModel\4.0.12\ref\netstandard1.3\System.ObjectModel.dll";
    "packages\System.Reflection\4.1.0\ref\netstandard1.5\System.Reflection.dll";
    "packages\System.Reflection.DispatchProxy\4.0.1\ref\netstandard1.3\System.Reflection.DispatchProxy.dll";
    "packages\System.Reflection.Emit\4.0.1\ref\netstandard1.1\System.Reflection.Emit.dll";
    "packages\System.Reflection.Emit.ILGeneration\4.0.1\ref\netstandard1.0\System.Reflection.Emit.ILGeneration.dll";
    "packages\System.Reflection.Extensions\4.0.1\ref\netstandard1.0\System.Reflection.Extensions.dll";
    "packages\System.Reflection.Metadata\1.4.1-beta-24227-04\lib\netstandard1.1\System.Reflection.Metadata.dll";
    "packages\System.Reflection.Primitives\4.0.1\ref\netstandard1.0\System.Reflection.Primitives.dll";
    "packages\System.Reflection.TypeExtensions\4.1.0\ref\netstandard1.5\System.Reflection.TypeExtensions.dll";
    "packages\System.Resources.Reader\4.0.0\lib\netstandard1.0\System.Resources.Reader.dll";
    "packages\System.Resources.ResourceManager\4.0.1\ref\netstandard1.0\System.Resources.ResourceManager.dll";
    "packages\System.Runtime\4.1.0\ref\netstandard1.5\System.Runtime.dll";
    "packages\System.Runtime.Extensions\4.1.0\ref\netstandard1.5\System.Runtime.Extensions.dll";
    "packages\System.Runtime.Handles\4.0.1\ref\netstandard1.3\System.Runtime.Handles.dll";
    "packages\System.Runtime.InteropServices\4.1.0\ref\netstandard1.5\System.Runtime.InteropServices.dll";
    "packages\System.Runtime.InteropServices.RuntimeInformation\4.0.0\ref\netstandard1.1\System.Runtime.InteropServices.RuntimeInformation.dll";
    "packages\System.Runtime.Loader\4.0.0\ref\netstandard1.5\System.Runtime.Loader.dll";
    "packages\System.Runtime.Numerics\4.0.1\ref\netstandard1.1\System.Runtime.Numerics.dll";
    "packages\System.Security.Claims\4.0.1\ref\netstandard1.3\System.Security.Claims.dll";
    "packages\System.Security.Cryptography.Algorithms\4.2.0\ref\netstandard1.4\System.Security.Cryptography.Algorithms.dll";
    "packages\System.Security.Cryptography.Encoding\4.0.0\ref\netstandard1.3\System.Security.Cryptography.Encoding.dll";
    "packages\System.Security.Cryptography.Primitives\4.0.0\ref\netstandard1.3\System.Security.Cryptography.Primitives.dll";
    "packages\System.Security.Cryptography.X509Certificates\4.1.0\ref\netstandard1.4\System.Security.Cryptography.X509Certificates.dll";
    "packages\System.Security.Principal\4.0.1\ref\netstandard1.0\System.Security.Principal.dll";
    "packages\System.Security.Principal.Windows\4.0.0\ref\netstandard1.3\System.Security.Principal.Windows.dll";
    "packages\System.Text.Encoding\4.0.11\ref\netstandard1.3\System.Text.Encoding.dll";
    "packages\System.Text.Encoding.Extensions\4.0.11\ref\netstandard1.3\System.Text.Encoding.Extensions.dll";
    "packages\System.Text.RegularExpressions\4.1.0\ref\netstandard1.3\System.Text.RegularExpressions.dll";
    "packages\System.Threading\4.0.11\ref\netstandard1.3\System.Threading.dll";
    "packages\System.Threading.Overlapped\4.0.1\ref\netstandard1.3\System.Threading.Overlapped.dll";
    "packages\System.Threading.Tasks\4.0.11\ref\netstandard1.3\System.Threading.Tasks.dll";
    "packages\System.Threading.Tasks.Dataflow\4.6.0\lib\netstandard1.1\System.Threading.Tasks.Dataflow.dll";
    "packages\System.Threading.Tasks.Extensions\4.0.0\lib\netstandard1.0\System.Threading.Tasks.Extensions.dll";
    "packages\System.Threading.Tasks.Parallel\4.0.1\ref\netstandard1.1\System.Threading.Tasks.Parallel.dll";
    "packages\System.Threading.Thread\4.0.0\ref\netstandard1.3\System.Threading.Thread.dll";
    "packages\System.Threading.ThreadPool\4.0.10\ref\netstandard1.3\System.Threading.ThreadPool.dll";
    "packages\System.Threading.Timer\4.0.1\ref\netstandard1.2\System.Threading.Timer.dll";
    "packages\System.ValueTuple\4.0.0-rc3-24212-01\lib\netstandard1.1\System.ValueTuple.dll";
    "packages\System.Xml.ReaderWriter\4.0.11\ref\netstandard1.3\System.Xml.ReaderWriter.dll";
    "packages\System.Xml.XDocument\4.0.11\ref\netstandard1.3\System.Xml.XDocument.dll"|]
    *)
