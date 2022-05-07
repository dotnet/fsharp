// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace UnitTests.TestLib.Utils

open System
open System.IO
open NUnit.Framework
open Microsoft.VisualStudio

module Asserts =
    (* Asserts ----------------------------------------------------------------------------- *)
    let AssertEqualMsg expected actual failureMsg =
        if expected<>actual then 
            let message = sprintf "Expected\n%A\nbut got\n%A\n%s" expected actual failureMsg
            printfn "%s" message
            Assert.Fail(message)
    let AssertEqual expected actual =
        if expected<>actual then 
            let message = sprintf "Expected\n%A\nbut got\n%A" expected actual
            printfn "%s" message
            Assert.Fail(message)
    let AssertNotEqual expected actual =
        if expected=actual then 
            let message = "Expected not equal, but were equal"
            printfn "%s" message
            Assert.Fail(message)
    let AssertContains (s:string) c =
        if not (s.Contains(c)) then
            let message = sprintf "Expected '%s' to contain '%s'." s c
            printfn "%s" message
            Assert.Fail(message)
    let ValidateOK (i:int) =
        if not (i = VSConstants.S_OK) then
            let message = sprintf "Expected S_OK"
            printfn "%s" message
            Assert.Fail(message)
    let Throws<'T when 'T:> Exception> f =
        let error =
            try
                f ()
                Some(sprintf "No exception occurred, expected %O" typeof<'T>)
            with
            | :? 'T -> None
            | e -> Some(sprintf "Wrong exception type occurred. Got %O, expecting %O" (e.GetType()) typeof<'T>)
        match error with
        | Some(msg) -> Assert.Fail(msg)
        | None -> ()

    let AssertBuildSuccessful (result: Microsoft.VisualStudio.FSharp.ProjectSystem.BuildResult) =
        Assert.IsTrue(result.IsSuccessful, "Expected build to succeed")

module UIStuff =
    let SetupSynchronizationContext() =
        Microsoft.VisualStudio.FSharp.ProjectSystem.UIThread.InitUnitTestingMode()
        Microsoft.VisualStudio.FSharp.LanguageService.UIThread.InitUnitTestingMode()
        Microsoft.VisualStudio.FSharp.ProjectSystem.UIThread.InitUnitTestingMode()

module FilesystemHelpers =
    let pid = System.Diagnostics.Process.GetCurrentProcess().Id 

    /// Create a new temporary directory.
    let rec NewTempDirectory (prefixName : String) = 
        let tick = Environment.TickCount 
        let dir = Path.Combine(Path.GetTempPath(), sprintf "%s-%A-%d" prefixName tick pid)
        if Directory.Exists dir then NewTempDirectory prefixName
        else 
            let _ = Directory.CreateDirectory(dir)
            dir
       
    /// Create a temporary file name, invoke callback with that fileName, then clean up temp file.
    let DoWithTempFile (fileName : string) (f : string (*filePath*) -> 'a) = 
        let dir = NewTempDirectory "fsc-tests"
        let filePath = Path.Combine(dir, fileName)
        let r = f filePath
        let rec DeleteAll dir =
            for f in Directory.GetFiles(dir) do
                File.Delete(f)
            for d in Directory.GetDirectories(dir) do
                DeleteAll(d)
            try
                Directory.Delete(dir)
            with e ->
                printfn "failed to delete temp directory %s" dir
                printfn "  error was %s" e.Message
                printfn "  ignoring"
        DeleteAll(dir)
        r

module Spawn = 
    open System
    open System.IO
    open System.Diagnostics
    open System.Text

    /// Set this flag to true to see spawned commands.
    let mutable showSpawnedCommands = false

    type public ProcessResults = {
        PeakPagedMemorySize:int64
        PeakVirtualMemorySize:int64
        PeakWorkingSet:int64
        PrivilegedProcessorTime:float // milliseconds
        UserProcessorTime:float // milliseconds
        TotalProcessorTime:float // milliseconds
        } with
        static member internal CreateFromProcess(proc:Process) =
            try
                { PeakPagedMemorySize=proc.PeakPagedMemorySize64
                  PeakVirtualMemorySize=proc.PeakVirtualMemorySize64
                  PeakWorkingSet=proc.PeakWorkingSet64
                  PrivilegedProcessorTime=proc.PrivilegedProcessorTime.TotalMilliseconds
                  UserProcessorTime=proc.UserProcessorTime.TotalMilliseconds
                  TotalProcessorTime=proc.TotalProcessorTime.TotalMilliseconds
                }    
            with :? InvalidOperationException as e ->
                // There is what appears to be an unresolvable race here. The process may exit while building the record.
                { PeakPagedMemorySize=0L
                  PeakVirtualMemorySize=0L
                  PeakWorkingSet=0L
                  PrivilegedProcessorTime=0.0
                  UserProcessorTime=0.0
                  TotalProcessorTime=0.0
                }               
        static member internal SampleProcess(proc:Process,original) = 
            try
                { PeakPagedMemorySize=max proc.PeakPagedMemorySize64 original.PeakPagedMemorySize
                  PeakVirtualMemorySize=max proc.PeakVirtualMemorySize64 original.PeakVirtualMemorySize
                  PeakWorkingSet=max proc.PeakWorkingSet64 original.PeakWorkingSet
                  PrivilegedProcessorTime=max proc.PrivilegedProcessorTime.TotalMilliseconds original.PrivilegedProcessorTime
                  UserProcessorTime=max proc.UserProcessorTime.TotalMilliseconds original.UserProcessorTime
                  TotalProcessorTime=max proc.TotalProcessorTime.TotalMilliseconds original.TotalProcessorTime
                }    
            with :? InvalidOperationException as e ->
                // There is what appears to be an unresolvable race here. The process may exit while building the record.
                original

    let private spawnDetailed logOutputTo logErrorTo exitWith command fmt =
        let spawn (arguments:string) = 
            if showSpawnedCommands then
                printfn "%s %s" command arguments
            let pi = ProcessStartInfo(command,arguments)
            pi.WindowStyle <- ProcessWindowStyle.Hidden
            pi.CreateNoWindow <- true
            pi.UseShellExecute <- false
            pi.WorkingDirectory <- Directory.GetCurrentDirectory()
            pi.ErrorDialog <- false
            pi.RedirectStandardError<-true
            pi.RedirectStandardOutput<-true
            use proc = new Process()
            proc.StartInfo <- pi
            proc.OutputDataReceived.Add(logOutputTo)
            proc.ErrorDataReceived.Add(logErrorTo)
            match proc.Start() with
            | false -> 
                failwith(sprintf "Could not start process: %s %s " command arguments)
            | true ->
                proc.BeginOutputReadLine()
                proc.BeginErrorReadLine()
                let mutable stats = ProcessResults.CreateFromProcess(proc)
                while not(proc.WaitForExit(200)) do
                    stats <- ProcessResults.SampleProcess(proc,stats)
                exitWith command arguments proc.ExitCode stats

        Printf.ksprintf spawn fmt  

    let private expectCodeOrRaise expectedCode command arguments (exitCode:int) _ = 
        if expectedCode<>exitCode then 
            failwith(sprintf "%s %s exitted with code %d. Expected %d" command arguments exitCode expectedCode)
        ()

    let private expectCodeWithStatisticsOrExit expectedCode command arguments (exitCode:int) stats :ProcessResults = 
        if expectedCode<>exitCode then 
            failwith(sprintf "%s %s exitted with code %d. Expected %d" command arguments exitCode expectedCode)
        stats

    let private returnExitCode _ _ (exitCode:int) _= exitCode

    let ignoreDataReceived(_msg:DataReceivedEventArgs) = ()
  
    /// Execute a command
    let public Spawn command fmt = 
        spawnDetailed ignoreDataReceived ignoreDataReceived (expectCodeOrRaise 0) command fmt

    /// Execute a command and expect a particular result code
    let public SpawnExpectCode expectCode command fmt = 
        spawnDetailed ignoreDataReceived ignoreDataReceived (expectCodeOrRaise expectCode) command fmt

    /// Execute the command and return the exit code
    let public SpawnReturnExitCode command fmt = 
        spawnDetailed ignoreDataReceived ignoreDataReceived returnExitCode command fmt

    /// Execute the command a return an array of textlines for the output and error.
    let public SpawnToTextLines command fmt = 
        let outlock = obj()
        let captured = ref []
        let capture (msg:DataReceivedEventArgs) = 
            lock outlock (fun () -> captured := msg.Data :: !captured)

        let exitWithResult command arguments actualCode _ = 
            actualCode, (!captured)|>List.rev|>Array.ofList

        spawnDetailed capture capture exitWithResult command fmt

    /// Execute a command and expect a particular result code. Return the processor statistics.
    let public SpawnWithStatisticsExpectCode expectCode command fmt = 
        spawnDetailed ignoreDataReceived ignoreDataReceived (expectCodeWithStatisticsOrExit expectCode) command fmt

    let Batch batchText = 
        let outlock = obj()
        let mutable captured = []
        let capture (msg:DataReceivedEventArgs) = 
            lock outlock (fun () -> 
                captured <- msg.Data :: captured)

        let exitWithResult command arguments actualCode _ = 
            actualCode, captured|>List.rev|>Array.ofList

        FilesystemHelpers.DoWithTempFile
            "$$temp-batch.cmd"
            (fun fileName->
                File.WriteAllText(fileName,batchText)
                spawnDetailed capture capture exitWithResult fileName "")



    /// Zip some files
    let Zip archiveName (files: string[]) =
        Spawn "zip.exe" "%s %s" archiveName (String.Join(" ", files))

    /// Use robocopy to mirror a directory from one place to another
    /// NOTE: This command will delete files at the destination if they don't exist at the source
    let RoboCopyMirror source destination =
        let code = SpawnReturnExitCode "robocopy" "%s %s /mir" source destination
        match code with
        | 0 | 1 | 2 | 3 -> () // Success.
        | _ -> 
            printfn "Robocopy %s %s /mir exitted with code %d. Expected 0, 1, 2 or 3." source destination code
            exit code

    /// Submit a specific set of checked out files to Tfs.
    let TfsSubmitSpecificFiles (files:string[]) comment = 
        let files = String.Join(" ", files)

        // Submit the changes
        match SpawnToTextLines "tf_.exe" "submit %s /comment:\"%s\" /noprompt" files comment with
        | 0,_ -> 
            printfn "Submited files: %s" files
        | 1,_ ->  
            printfn "No changes detected in files: %s" files
        | errorCode,lines ->
            for line in lines do 
                printfn "%s" line
            eprintfn "tf submit returned error code %d" errorCode

[<AutoOpen>]
module Helpers = 
    type DummyType = A | B
    let PathRelativeToTestAssembly p = Path.Combine(Path.GetDirectoryName(Uri(typeof<DummyType>.Assembly.CodeBase).LocalPath), p)

namespace TestLibrary
  module LambdaCalculus =

    exception LambdaFailure of string

    module Syntax =
      type Binder = string

      type Expression = Variable of Binder
                      | Lambda   of (Binder * Expression)
                      | Apply    of (Expression * Expression)

      let rec stringOfExpression (e : Expression) : string =
        match e with
        | Variable x     -> x
        | Lambda (x, e)  -> "lambda " + x + " . " + stringOfExpression e
        | Apply (e1, e2) -> "(" + stringOfExpression e1 + ") @ (" + stringOfExpression e2 + ")"


    module Evaluation =

      open Syntax

      module Environment =
        type Env = Map<Binder, Expression>

        exception EnvironmentFailure of string

        let add (g : Env)(x : Binder)(e : Expression) = Map.add x e g

        let lookup (g : Env)(x : Binder) =
          try Map.find x g
              with _ -> raise (EnvironmentFailure <| "No binding for `" + (stringOfExpression <| Variable x) + "`.")

      open Environment

      exception EvalFailure = LambdaFailure

      let rec eval (g : Env)(e : Expression) : Expression =
        match e with
        | Variable x     -> lookup g x
        | Lambda _       -> e
        | Apply (e1, e2) -> match eval g e1 with
                            | Lambda (x, e) -> eval (add g x (eval g e2)) e1
                            | _             -> raise <| EvalFailure "Unexpected operator in application; need a lambda."

  module OtherTests =
    type Point = { x : int
                   y : int
                 }

    let showPoint (p : Point) = sprintf "(%A,%A)" p.x p.y 

    type Shape (initVertices : list<Point>) =
      let mutable vertices = initVertices

      let True _  = true
      let Id   x  = x

      // using this for everything is a bit artificial, but it ought to cover
      // quite a few patterns of members interacting
      member this.addFilterMap (pr : Point -> bool)(f : Point -> Point)(ps : list<Point>) : unit =
        match ps with
        | []      -> ()
        | p :: ps -> if pr p
                        then vertices <- (f p) :: vertices
                     this.addFilterMap pr f ps

      member this.getVertices () = vertices

      member this.clearVertices () = vertices <- []

      // new vertex
      member this.addVertex (p : Point) = this.addFilterMap True Id [p]

      // swallow another shape's vertices
      member this.subsume (s : Shape) = List.iter this.addVertex (s.getVertices ())

      member this.map (f : Point -> Point) =
        let ps = this.getVertices ()
        this.clearVertices ()
        this.addFilterMap True f ps

      member this.transpose () =
        let swap p =
          { x = p.y
            y = p.x
          }
        this.map swap

      // okay, this is silly; just to test mutual recursion of members
      member this.fold (f : 'a -> Point -> 'a)(acc : 'a) =
        match this.getVertices () with
        | []      -> acc
        | p :: ps -> f (this.refold f acc) p

      member this.refold (f : 'a -> Point -> 'a)(acc : 'a) =
        let ps = this.getVertices ()
        let set ps =
          this.clearVertices ()
          this.subsume (new Shape (ps))
        match ps with
        | []      -> ()
        | _ :: ps -> set ps
        let res = this.fold f acc
        set ps
        acc

      static member combine (s1 : Shape)(s2 : Shape) : Shape =
        let ps1 = s1.getVertices ()
        let ps2 = s2.getVertices ()
        new Shape (ps1 @ (ps2 |> List.filter (fun x -> not <| List.exists ((=) x) ps1)))



