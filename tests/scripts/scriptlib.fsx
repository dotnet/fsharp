// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
//
// Scripting utilities
//=========================================================================================

namespace global

open System
open System.IO
open System.Text
open System.Diagnostics

[<AutoOpen>]
module Scripting =

    let isNullOrEmpty s = String.IsNullOrEmpty s

    module ConsoleHost =
#if !NETCOREAPP
        open System.Runtime.InteropServices

        [<DllImport("kernel32.dll")>]
        extern IntPtr GetConsoleWindow()

        [<DllImport("kernel32.dll", SetLastError=true)>]
        extern bool AllocConsole()

        let ensureConsole () =
            if GetConsoleWindow() = IntPtr.Zero then
                AllocConsole() |> ignore
            // Set UTF-8 encoding for console input/output to ensure FSI receives UTF-8 data.
            // This is needed because on net472 ProcessStartInfo.StandardInputEncoding is unavailable,
            // so the spawned process inherits the console's encoding settings.
            Console.InputEncoding <- Text.UTF8Encoding(false)
            Console.OutputEncoding <- Text.UTF8Encoding(false)
#endif
#if NETCOREAPP
        let ensureConsole () = ()
#endif

    let executeProcess fileName arguments =
        let processWriteMessage (chan:TextWriter) (message:string) =
            if message <> null then 
                chan.WriteLine(message) 
        printfn "%s %s" fileName arguments
        let info = ProcessStartInfo(Arguments=arguments, UseShellExecute=false, 
                                    RedirectStandardOutput=true, RedirectStandardError=true,
                                    CreateNoWindow=true, FileName=fileName)
        use p = new Process(StartInfo=info)
        p.OutputDataReceived.Add(fun x -> processWriteMessage stdout x.Data)
        p.ErrorDataReceived.Add(fun x ->  processWriteMessage stderr x.Data)
        if p.Start() then
            p.BeginOutputReadLine()
            p.BeginErrorReadLine()
            p.WaitForExit()
            // Second WaitForExit ensures async output handlers complete
            p.WaitForExit()
            p.ExitCode
        else
            0

#if INTERACTIVE
    let argv = FSharp.Compiler.Interactive.Settings.fsi.CommandLineArgs |> Seq.skip 1 |> Seq.toArray

    let getCmdLineArgOptional (switchName: string) =
        argv |> Array.filter(fun t -> t.StartsWith(switchName)) |> Array.map(fun t -> t.Remove(0, switchName.Length).Trim()) |> Array.tryHead 

    let getCmdLineArg switchName defaultValue = 
        match getCmdLineArgOptional switchName with
        | Some file -> file
        | _ -> defaultValue

    let getCmdLineArgReqd switchName = 
        match getCmdLineArg switchName null with 
        | null -> failwith (sprintf "The argument %s is required" switchName)
        | x -> x

    let getCmdLineExtraArgs isSwitch = argv |> Array.skipWhile isSwitch 

#endif

    let makeDirectory output =
        if not (Directory.Exists(output)) then 
            Directory.CreateDirectory(output) |> ignore

    let (++) a b = Path.Combine(a,b)

    let getBasename (a:string) = Path.GetFileNameWithoutExtension a
    let getFullPath (a:string) = Path.GetFullPath a
    let getFilename (a:string) = Path.GetFileName a
    let getDirectoryName (a:string) = Path.GetDirectoryName a

    let copyFile (source:string) dir =
        let dest = 
            if not (Directory.Exists dir) then Directory.CreateDirectory dir |>ignore
            let result = Path.Combine(dir, getFilename source)
            result
        //printfn "Copy %s --> %s" source dest
        File.Copy(source, dest, true)

    let deleteDirectory output =
        if Directory.Exists output then 
            Directory.Delete(output, true)
            
    // Capture the original stdout for logging.
    let private originalOut = stdout
    // When used during test run, log will always output to the original stdout of the testhost, instead of the test output.
    let log format = fprintfn originalOut format

    type FilePath = string

    type CmdResult = 
        | Success of output: string
        | ErrorLevel of string * int

    type CmdArguments = 
        { RedirectOutput : (string -> unit) option
          RedirectError : (string -> unit) option
          RedirectInput : (StreamWriter -> unit) option }

    module Process =

        let processExePath baseDir (exe:string) =
            if Path.IsPathRooted(exe) then exe
            else 
                match getDirectoryName exe with
                | "" -> exe
                | _ -> Path.Combine(baseDir,exe) |> Path.GetFullPath

        let exec cmdArgs (workDir: FilePath) envs (path: FilePath) (arguments: string) =

            let exePath = path |> processExePath workDir
            let processInfo = new ProcessStartInfo(exePath, arguments)
            
            // Diagnostic logging to file - bypasses all console redirection
            let diagLogFile = Path.Combine(workDir, "fsi_stdin_diag.log")
            let diagLog msg = 
                let timestamp = DateTime.Now.ToString("HH:mm:ss.fff")
                try File.AppendAllText(diagLogFile, sprintf "[%s] %s%s" timestamp msg Environment.NewLine) with _ -> ()

            if cmdArgs.RedirectInput.IsSome then
                ConsoleHost.ensureConsole()
            
            diagLog (sprintf "=== Process.exec START === exe=%s args=%s" exePath arguments)
            diagLog (sprintf "RedirectInput=%b RedirectOutput=%b RedirectError=%b" 
                cmdArgs.RedirectInput.IsSome cmdArgs.RedirectOutput.IsSome cmdArgs.RedirectError.IsSome)
            
            // Log console state of test process (parent) - helps diagnose MTP console inheritance
            diagLog (sprintf "[CONSOLE] IsInputRedirected=%b IsOutputRedirected=%b IsErrorRedirected=%b" 
                Console.IsInputRedirected Console.IsOutputRedirected Console.IsErrorRedirected)
            
            processInfo.EnvironmentVariables.["DOTNET_ROLL_FORWARD"] <- "LatestMajor"
            processInfo.EnvironmentVariables.["DOTNET_ROLL_FORWARD_TO_PRERELEASE"] <- "1"
            
            processInfo.CreateNoWindow <- true
            processInfo.UseShellExecute <- false
            processInfo.WorkingDirectory <- workDir

            ignore envs  // work out what to do about this

            use p = new Process()
            p.EnableRaisingEvents <- true
            p.StartInfo <- processInfo
            let out = StringBuilder()
            let err = StringBuilder()

            cmdArgs.RedirectOutput|> Option.iter (fun f ->
                processInfo.RedirectStandardOutput <- true
                p.OutputDataReceived.Add (fun ea -> 
                    if ea.Data <> null then 
                        out.Append(ea.Data + Environment.NewLine) |> ignore
                        f ea.Data)
            )

            cmdArgs.RedirectError |> Option.iter (fun f ->
                processInfo.RedirectStandardError <- true
                p.ErrorDataReceived.Add (fun ea -> 
                    if ea.Data <> null then 
                        err.Append(ea.Data + Environment.NewLine) |> ignore
                        f ea.Data)
            )

            cmdArgs.RedirectInput
            |> Option.iter (fun _ -> p.StartInfo.RedirectStandardInput <- true)

            diagLog "Starting process..."
            p.Start() |> ignore
            diagLog (sprintf "Process started: PID=%d" p.Id)

            cmdArgs.RedirectOutput |> Option.iter (fun _ -> p.BeginOutputReadLine())
            cmdArgs.RedirectError |> Option.iter (fun _ -> p.BeginErrorReadLine())
            diagLog "Async output readers started"

            // HYPOTHESIS TEST: Write stdin SYNCHRONOUSLY, not with Async.Start
            // The original Async.Start was fire-and-forget and might not complete before WaitForExit
            cmdArgs.RedirectInput |> Option.iter (fun input -> 
                diagLog "Writing to stdin (synchronously)..."
                diagLog (sprintf "Process.HasExited=%b BEFORE stdin write" p.HasExited)
                let inputWriter = p.StandardInput
                diagLog (sprintf "StandardInput.BaseStream.CanWrite=%b" inputWriter.BaseStream.CanWrite)
                try
                    input inputWriter  // This is the callback that writes the actual content
                    diagLog "Input callback completed, flushing..."
                    inputWriter.Flush()
                    diagLog "Flush completed, disposing (sends EOF)..."
                    inputWriter.Dispose()
                    diagLog (sprintf "Stdin closed. Process.HasExited=%b AFTER stdin write" p.HasExited)
                with ex ->
                    diagLog (sprintf "EXCEPTION during stdin write: %s - %s" (ex.GetType().Name) ex.Message)
            )

            diagLog "Calling WaitForExit..."
            p.WaitForExit()
            diagLog (sprintf "WaitForExit returned. ExitCode=%d" p.ExitCode)
            
            // Second WaitForExit call ensures async output handlers (OutputDataReceived/ErrorDataReceived) complete.
            // See: https://learn.microsoft.com/dotnet/api/system.diagnostics.process.waitforexit
            p.WaitForExit()
            diagLog (sprintf "Second WaitForExit returned. stdout.Length=%d stderr.Length=%d" out.Length err.Length)

            // Log stdout content - useful for understanding what FSI actually outputs
            if out.Length > 0 && out.Length < 200 then
                let stdoutContent = string out
                diagLog (sprintf "[STDOUT] %s" (stdoutContent.Replace("\r", "\\r").Replace("\n", "\\n")))

            // Log stderr content if there is any - this may contain FSI error messages
            if err.Length > 0 then
                let stderrContent = string err
                let truncated = if stderrContent.Length > 500 then stderrContent.Substring(0, 500) + "..." else stderrContent
                diagLog (sprintf "[STDERR] %s" (truncated.Replace("\r", "\\r").Replace("\n", "\\n")))

            diagLog "=== Process.exec END ==="

            printf $"{string out}"
            eprintf $"{string err}"

            match p.ExitCode with
            | 0 ->
                Success(string out)
            | errCode ->
                let msg = sprintf "Error running command '%s' with args '%s' in directory '%s'" exePath arguments workDir
                ErrorLevel (msg, errCode)

    type OutPipe (writer: TextWriter) =
        member x.Post (msg:string) = lock writer (fun () -> writer.WriteLine(msg))
        interface System.IDisposable with 
           member _.Dispose() = writer.Flush()

    let redirectTo (writer: TextWriter) = new OutPipe (writer)

#if !NETCOREAPP
    let defaultPlatform = 
        match Environment.OSVersion.Platform, Environment.Is64BitOperatingSystem with 
        | PlatformID.MacOSX, true -> "osx.10.11-x64"
        | PlatformID.Unix,true -> "ubuntu.14.04-x64"
        | _, true -> "win7-x64"
        | _, false -> "win7-x86"
#endif

    let executeProcessNoRedirect fileName arguments =
        let info = ProcessStartInfo(Arguments=arguments, UseShellExecute=false, 
                                    RedirectStandardOutput=true, RedirectStandardError=true,RedirectStandardInput=true,
                                    CreateNoWindow=true, FileName=fileName)
        use p = new Process(StartInfo=info)
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
