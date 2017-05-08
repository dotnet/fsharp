// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

module internal Microsoft.VisualStudio.FSharp.Interactive.Session

open Microsoft.FSharp.Compiler
open System
open System.IO
open System.Text
open System.Diagnostics
open System.Runtime.Remoting
open System.Runtime.Remoting.Lifetime
open System.Windows.Forms
open Internal.Utilities

#nowarn "52" //  The value has been copied to ensure the original is not mutated by this operation

// Can not be DEBUG only since it is used by tests  
let mutable timeoutAppShowMessageOnTimeOut = true

open Microsoft.FSharp.Control
// Wrapper around ManualResetEvent which will ignore Sets on disposed object
type internal EventWrapper() =
    let waitHandle = new System.Threading.ManualResetEvent(false)
    let guard = new obj()
    
    let mutable disposed = false
    
    member this.Set() =
        lock guard (fun () -> if not disposed then waitHandle.Set() |> ignore)
    
    member this.Dispose() =
        lock guard (fun () -> disposed <- true; (waitHandle :> IDisposable).Dispose())
    
    member this.WaitOne(timeout : int) =
        waitHandle.WaitOne(timeout, true)
        
    interface IDisposable with
        member this.Dispose() = this.Dispose()
    

/// Run function application return Some (f x) or None if execution exceeds timeout (in ms).
/// Exceptions raised by f x are caught and reported in DEBUG mode.
let timeoutApp descr timeoutMS (f : 'a -> 'b) (arg:'a) =
    use ev = new EventWrapper()
    let r : 'b option ref = ref None
    System.Threading.ThreadPool.QueueUserWorkItem(fun _ ->
        r := 
            try
                f arg |> Some
            with
            | e -> 
#if DEBUG
                if timeoutAppShowMessageOnTimeOut then
                    System.Windows.Forms.MessageBox.Show("[This message is DEBUG build only]\n\n" +
                                                         "timeoutApp: an exception was thrown.\n" +
                                                         "fsi.exe starts the remoting server at the end of it's initialisation sequence.\n" +
                                                         "The initialisation sequence takes an observable time (e.g. 2 seconds).\n" + 
                                                         "Remoting exceptions are to be expected on interupt/intelisense calls made before that point.\n" +
                                                         "Context: " + descr + "\n" +
                                                         "Exception: " + e.ToString()) |> ignore
#endif             
                None
        ev.Set() 
    ) |> ignore
    ev.WaitOne(timeoutMS) |> ignore
    !r

module SessionsProperties = 
    let mutable useAnyCpuVersion = false
    let mutable fsiArgs = "--optimize"
    let mutable fsiShadowCopy = true
    let mutable fsiDebugMode = false

// This code pre-dates the events/object system.
// Later: Tidy up.
exception SessionError of string

/// Buffer messages up into a list of messages.
/// Allow upto timeMS to pass.
let bufferEvent timeMS (w : #IObservable<_>) =
    let bufferedW,bufferedE = 
        let e = new Event<_>() in e.Trigger, e.Publish

    let batchSize = 2000
    // 'FIFO' buffer for messages
    let buffer = System.Collections.Generic.LinkedList<ResizeArray<_>>() // queue of lists - every list contains up to 2000 elements - max size of batch to be processed 

    // drops existing content of the buffer
    let flushBuffer() = 
        lock buffer <| fun () -> 
            buffer.Clear()
    
    // adds item to one of slots
    // every slot can contain max 'batchSize' elements
    let addToBuffer item = 
        lock buffer <| fun() ->
            let slot = 
                if buffer.Count = 0 || buffer.First.Value.Count = batchSize then 
                    let slot = ResizeArray(batchSize)
                    buffer.AddFirst slot |> ignore
                    slot
                else 
                    buffer.First.Value
            slot.Add item
    
    // run the processing of the accumulated data in one slot
    let processBatch() = 
        let messages = 
            lock buffer <| fun() ->
                if buffer.Count = 0 then None
                else 
                    let batch = buffer.Last
                    buffer.RemoveLast()
                    Some batch.Value
        match messages with
        | None -> ()
        | Some batch -> bufferedW (List.ofSeq batch)

    let timer = new System.Windows.Forms.Timer() 
    timer.Interval <- timeMS
    timer.Tick.Add(fun _ -> timer.Stop(); processBatch(); timer.Start())
    timer.Start()
    w.Add(addToBuffer)
    flushBuffer,bufferedE

// Sessions...

let catchAll trigger x = 
    try trigger x  
    with err -> System.Windows.Forms.MessageBox.Show(err.ToString()) |> ignore

let fsiExeName () = 
    if SessionsProperties.useAnyCpuVersion then "fsiAnyCpu.exe" else "fsi.exe"

// Use the VS-extension-installed development path if available, relative to the location of this assembly
let determineFsiRelativePath1 () =
    let thisAssemblyDirectory = typeof<EventWrapper>.Assembly.Location |> Path.GetDirectoryName
    Path.Combine(thisAssemblyDirectory,fsiExeName() )

// This path is relative to the location of "FSharp.Compiler.Interactive.Settings.dll"
let determineFsiRelativePath2 () =
    let thisAssembly : System.Reflection.Assembly = typeof<Microsoft.FSharp.Compiler.Server.Shared.FSharpInteractiveServer>.Assembly
    let thisAssemblyDirectory = thisAssembly.Location |> Path.GetDirectoryName
    // Use the quick-development path if available    
    Path.Combine(thisAssemblyDirectory,fsiExeName() )

let determineFsiPath () =    
    // Choose VS extension path, if it exists (for developers)
    let fsiRelativePath1 = determineFsiRelativePath1()
    if  File.Exists fsiRelativePath1 then fsiRelativePath1 else

    // Choose relative path, if it exists (for developers), otherwise, the installed path.    
    let fsiRelativePath2 = determineFsiRelativePath2()
    if  File.Exists fsiRelativePath2 then fsiRelativePath2 else

    // Try the registry key
    let fsbin = match Internal.Utilities.FSharpEnvironment.BinFolderOfDefaultFSharpCompiler(None) with Some(s) -> s | None -> ""
    let fsiRegistryPath = Path.Combine(fsbin, fsiExeName() )
    if File.Exists(fsiRegistryPath) then fsiRegistryPath else

    // Otherwise give up
    raise (SessionError (VFSIstrings.SR.couldNotFindFsiExe fsiRegistryPath))

let readLinesAsync (reader: StreamReader) trigger =
    let buffer = StringBuilder(1024)
    let byteBuffer = Array.zeroCreate 128
    let encoding = Encoding.UTF8
    let decoder = encoding.GetDecoder()
    let async0 = async.Return 0
    let charBuffer = 
        let maxCharsInBuffer = encoding.GetMaxCharCount byteBuffer.Length
        Array.zeroCreate maxCharsInBuffer

    let rec findLinesInBuffer pos =
        if pos >= buffer.Length then max (buffer.Length - 1) 0 // exit and point to the last char
        else
        let c = buffer.[pos]
        let deletePos = match c with
                        | '\r' when (pos + 1) < buffer.Length && buffer.[pos + 1] = '\n' -> Some(pos + 2)
                        | '\r' when (pos + 1) = buffer.Length -> None
                        | '\r' -> Some(pos + 1)
                        | '\n' -> Some(pos + 1)
                        | _  ->  None

        match deletePos with
        | Some deletePos ->
            let line = buffer.ToString(0, pos)
            trigger line
            buffer.Remove(0, deletePos) |> ignore
            findLinesInBuffer 0
        | None ->  findLinesInBuffer (pos + 1)

    let rec read pos = 
        async {
            let! bytesRead = 
                try 
                    reader.BaseStream.AsyncRead(byteBuffer, 0, byteBuffer.Length)
                with
                    | :? IOException -> async0
            if bytesRead <> 0 then
                let charsRead = decoder.GetChars(byteBuffer, 0, bytesRead, charBuffer, 0)
                buffer.Append(charBuffer, 0, charsRead) |> ignore
                let newPos = findLinesInBuffer pos
                return! read newPos
        }
    Async.StartImmediate (read 0)

let fsiStartInfo channelName =
    let procInfo = new ProcessStartInfo()
    let fsiPath  = determineFsiPath () 

    procInfo.FileName  <- fsiPath
    // Mismatched encoding on I/O streams between VS addin and it's FSI session.
    // Fix: pin down the input/output encodings precisely (force I/O to use UTF8 regardless).
    // Send codepage preferences to the FSI.
    // We also need to send fsi.exe the locale of the VS process
    let inCP,outCP = Encoding.UTF8.CodePage,Encoding.UTF8.CodePage

    let addBoolOption name value args = sprintf "%s --%s%s" args name (if value then "+" else "-")
    let addStringOption name value args = sprintf "%s --%s:%O" args name value
    
    let procArgs =
        ""
        |> addStringOption "fsi-server-output-codepage" outCP
        |> addStringOption "fsi-server-input-codepage" inCP
        |> addStringOption "fsi-server-lcid" System.Threading.Thread.CurrentThread.CurrentUICulture.LCID
        |> addStringOption "fsi-server" channelName
        |> (fun s -> s +  sprintf " %s" SessionsProperties.fsiArgs)
        |> addBoolOption "shadowcopyreferences" SessionsProperties.fsiShadowCopy
        |> (fun args -> if SessionsProperties.fsiDebugMode then
                            // for best debug experience, need optimizations OFF and debug info ON
                            // tack these on the the end, they will override whatever comes earlier
                            args |> addBoolOption "optimize" false |> addBoolOption "debug" true
                        else args)

    procInfo.Arguments <- procArgs
    procInfo.CreateNoWindow <- true
    procInfo.UseShellExecute <- false
    procInfo.RedirectStandardError <- true
    procInfo.RedirectStandardInput <- true
    procInfo.RedirectStandardOutput <- true
    procInfo.StandardOutputEncoding <- Encoding.UTF8
    procInfo.StandardErrorEncoding <- Encoding.UTF8
    let tmpPath = Path.GetTempPath()
    if Directory.Exists(tmpPath) then
        procInfo.WorkingDirectory <- tmpPath
    procInfo


let nonNull = function null -> false | (s:string) -> true

/// Represents an active F# Interactive process to which Visual Studio is connected via stdin/stdout/stderr and a remoting channel
type FsiSession() = 
    let randomSalt = System.Random()
    let channelName = 
        let pid  = System.Diagnostics.Process.GetCurrentProcess().Id
        let tick = System.Environment.TickCount
        let salt = randomSalt.Next()
        sprintf "FSIChannel_%d_%d_%d" pid tick salt

    let procInfo = fsiStartInfo channelName
    let cmdProcess = new Process(StartInfo=procInfo)
    let fsiOutput = Event<_>()
    let fsiError = Event<_>()

    do cmdProcess.Start() |> ignore

    // hook up stdout\stderr data events
    do readLinesAsync cmdProcess.StandardOutput (catchAll fsiOutput.Trigger)
    do readLinesAsync cmdProcess.StandardError  (catchAll fsiError.Trigger)

    let inputQueue = 
        // Write the input asynchronously, freeing up the IDE thread to contrinue doing work
        // Fix 982: Force input to be written in UTF8 regardless of the apparent encoding.
        let inputWriter = new StreamWriter(cmdProcess.StandardInput.BaseStream, new UTF8Encoding(encoderShouldEmitUTF8Identifier=false), AutoFlush = false)
        MailboxProcessor<string>.Start(fun inbox -> 
            async { 
                try 
                  while not cmdProcess.HasExited do 
                    let! textToWrite = inbox.Receive() 
                    inputWriter.WriteLine(textToWrite) 
                    inputWriter.Flush()
                with _ -> () // if writing or flushing fails then just give up on this F# Interactive session
            })

    do cmdProcess.EnableRaisingEvents <- true

    let client   = 
        try Microsoft.FSharp.Compiler.Server.Shared.FSharpInteractiveServer.StartClient(channelName)
        with e -> raise (SessionError (VFSIstrings.SR.exceptionRaisedWhenCreatingRemotingClient(e.ToString())))

    /// interrupt timeout in miliseconds 
    let interruptTimeoutMS   = 1000 

#if FSI_SERVER_INTELLISENSE

    // timeout in miliseconds.
    // This timeout is to catch any issue with remoting becoming unresponsive.
    // On it's duration, it is better from user POV to wait a few seconds and see,
    // than to abort an intelisense request that would return,
    // since an abort request has no useful information at all.
    // 2 seconds seems to slow. (which was surprising, maybe
    // the tcEnv were still being computed).
    let completionsTimeoutMS = 3000
       
#endif

    let checkLeaseStatus myService =
        if false then
            let myLease = RemotingServices.GetLifetimeService(myService) :?> ILease
            match myLease with 
            | null -> printf "Cannot get lease.\n"
            | _ -> 
              ignore (System.Windows.Forms.MessageBox.Show
                        (sprintf "Initial lease time is %s\n"  (myLease.InitialLeaseTime  .ToString()) +
                         sprintf "Current lease time is %s\n"  (myLease.CurrentLeaseTime  .ToString()) +
                         sprintf "Renew on call time is %s\n"  (myLease.RenewOnCallTime   .ToString()) +
                         sprintf "Sponsorship timeout is %s\n" (myLease.SponsorshipTimeout.ToString()) +
                         sprintf "Current lease state is %s\n" (myLease.CurrentState      .ToString())))


    // Create session object 
    member x.Interrupt() = 
       checkLeaseStatus client
       match timeoutApp "VFSI interrupt" interruptTimeoutMS (fun () -> client.Interrupt()) () with
       | Some () -> true
       | None    -> false

    member x.SendInput (str: string) = inputQueue.Post(str)

    member x.Output      = Observable.filter nonNull fsiOutput.Publish

    member x.Error       = Observable.filter nonNull fsiError.Publish

    member x.Exited      = (cmdProcess.Exited |> Observable.map id)

    member x.Alive       = not cmdProcess.HasExited

    member x.ProcessID   = cmdProcess.Id

    member x.ProcessArgs = procInfo.Arguments

    member x.Kill()         = 
        let verboseSession = false
        try 
            if verboseSession then fsiOutput.Trigger ("Kill process " + cmdProcess.Id.ToString())
            cmdProcess.Kill()
            if verboseSession then cmdProcess.Exited.Add(fun _ -> fsiOutput.Trigger (cmdProcess.Id.ToString()))
        with e -> 
            fsiOutput.Trigger (VFSIstrings.SR.killingProcessRaisedException (e.ToString()))

#if FSI_SERVER_INTELLISENSE
    member x.Completions(s:string) = 
       checkLeaseStatus client
       match timeoutApp "VFSI completions" completionsTimeoutMS (fun () -> client.Completions(s)) () with
       | Some names -> names
       | None       -> [| |]

    member x.GetDeclarations(s: string, plid: string[]) = 
       checkLeaseStatus client       
       match timeoutApp "VFSI intelisense" completionsTimeoutMS (fun () -> client.GetDeclarations(s,plid)) () with
       | Some results -> results
       | None         -> [| |]
#endif

//-------------------------------------------------------------------------
// sessions
//-------------------------------------------------------------------------

/// Represents a container for all the active F# Interactive sessions
/// Currently there is either 0 or 1 
type FsiSessions() =
    // state: the most recent (if any) session object
    let mutable sessionR : FsiSession option  = None 

    let isCurrentSession session = 
        (sessionR = Some session)
    
    let fsiOut = Event<string>()
    let fsiError = Event<string>()
    let fsiExited = Event<EventArgs>()
  
    let kill() =
      sessionR |> Option.iter (fun session -> 
          sessionR <- None
          // clearing sessionR before kill() means session.Exited is ignored below
          session.Kill())
     
    let restart() =
        kill()
        try 
            let session = FsiSession()
            sessionR <- Some session          
            // All response callbacks are guarded by checks that "session" is still THE ACTIVE session.
            session.Output.Add(fun s -> if isCurrentSession session then fsiOut.Trigger s)
            session.Error.Add(fun s -> if isCurrentSession session then fsiError.Trigger s)
            session.Exited.Add(fun x -> if isCurrentSession session then (sessionR <- None; fsiExited.Trigger x))
        with
            SessionError text -> fsiError.Trigger text
   
    member x.Interrupt()    = 
        sessionR |> Option.forall (fun session -> session.Interrupt())

    member x.SendInput s    = 
        sessionR |> Option.iter (fun session -> session.SendInput s)

    member x.Output         = fsiOut.Publish

    member x.Error          = fsiError.Publish

    member x.Alive          = 
        sessionR |> Option.exists (fun session -> session.Alive)

    member x.ProcessID      = 
        match sessionR with
        | None -> -1 (* -1 assumed to never be a valid process ID *)
        | Some session -> session.ProcessID

    member x.ProcessArgs    = 
        match sessionR with
        | None -> ""
        | Some session -> session.ProcessArgs

    member x.Kill()         = kill()
    member x.Restart()      = restart()
    member x.Exited         = fsiExited.Publish

#if FSI_SERVER_INTELLISENSE
    member x.Completions(s) = 
        match sessionR with
        | None -> [| |]
        | Some session -> session.Completions(s)

    member x.GetDeclarations(s,plid) = 
        match sessionR with
        | None -> [| |]
        | Some session -> session.GetDeclarations(s,plid)

    member x.GetDeclarationInfos (str:string) = 
        // RPC to the session to get the completions based on the latest state.                        
        let plid = Microsoft.VisualStudio.FSharp.Interactive.QuickParse.GetPartialLongName(str,str.Length-1) // Subtract one to convert to zero-relative            
        let project = function (name,None) -> [name] | (name,Some residue) -> [name;residue]
        let plid = plid |> List.collect project |> List.toArray
        // diagnostics.Log(sprintf "RPC GetDeclarations with str =[[%s]]" str)
        // diagnostics.Log(sprintf "RPC GetDeclarations with plid=[[%s]]" (String.concat "." plid))
        let declInfos = x.GetDeclarations(str,plid)        
        declInfos

#endif
