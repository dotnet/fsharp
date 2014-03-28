namespace MLang.Test

open System
open System.IO
open System.Net
open System.Net.Sockets
open System.Text
open System.Threading
open System.Linq
open FSharp.Compiler.Hosted

module Log =
    /// simple logger
    let log msg = printfn "%O - %s" (DateTime.Now.ToString("HH:mm:ss.fff")) msg
open Log

module CompilerHelpers =
    let MessageDelimiter = "|||"
    let fscCompiler = FSharp.Compiler.Hosted.FscCompiler()

    /// splits a provided command line string into argv array
    /// currently handles quotes, but not escaped quotes
    let parseCommandLine (commandLine : string) =
        let folder (inQuote : bool, currArg : string, argLst : string list) ch =
            match (ch, inQuote) with
            | ('"', _) ->
                (not inQuote, currArg, argLst)
            | (' ', false) ->
                if currArg.Length > 0 then (inQuote, "", currArg :: argLst)
                else (inQuote, "", argLst)
            | _ ->
                (inQuote, currArg + (string ch), argLst)

        seq { yield! commandLine.ToCharArray(); yield ' ' }
        |> Seq.fold folder (false, "", [])
        |> (fun (_, _, args) -> args)
        |> List.rev
        |> Array.ofList

    /// runs in-proc fsc compilation, returns array consisting of exit code, then compiler output
    let fscCompile directory args =
        // in-proc compiler still prints banner to console, so need this to capture it
        let origOut = Console.Out
        let sw = new StringWriter()
        Console.SetOut(sw)
        try
            try
                Environment.CurrentDirectory <- directory
                let (exitCode, output) = fscCompiler.Compile(args)
                let consoleOut = sw.ToString().Split([|'\r'; '\n'|], StringSplitOptions.RemoveEmptyEntries)
                [| yield exitCode.ToString(); yield! consoleOut; yield! output |]
            with e ->
                [| "1"; "Internal compiler error"; e.ToString().Replace('\n', ' ').Replace('\r', ' ') |]
        finally
            Console.SetOut(origOut)
    
    // logic for processing raw message string into intended purpose       
    let (|FscCompile|Unknown|) (message : string) =
        match message.Split([|MessageDelimiter|], StringSplitOptions.RemoveEmptyEntries) with
        | [|directory; commandLine|] ->
            let args = parseCommandLine commandLine
            log <| sprintf "Args parsed as [%s]" (String.Join("] [", args))
            log <| sprintf "Dir  parsed as [%s]" directory

            // pass back pointer to function that does the work
            FscCompile(fun () -> fscCompile directory args)
        | _ ->
            Unknown()

open CompilerHelpers

/// TCP server which listens for message from test code in other processes,
/// and runs hosted compilers in response
type HostedCompilerServer(port) =
    let Backlog = 10

    /// initializes sockets, sets up listener
    let setup () =
        let ipAddress = IPAddress.Loopback
        let localEndpoint = IPEndPoint(ipAddress, port)

        let listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
        listener.Bind(localEndpoint)
        listener.Listen(Backlog)
        listener

    /// reads bytes from the socket and returns string representation
    let receiveMessage (handler : Socket) =
        let rec page (data : StringBuilder) =
            let buffer = Array.zeroCreate<byte> 1024
            let numBytes = handler.Receive(buffer)
            data.Append(Encoding.ASCII.GetString(buffer, 0, numBytes)) |> ignore
            if numBytes < 1024 then data.ToString()
            else page data

        page (StringBuilder())

    /// accepts message string sent to the server, returns collection of strings to send back
    let processMessage message =
        log <| sprintf "Raw message: %s" message
        match message with
        | FscCompile(doCompile) ->
            doCompile()
        | _ ->
            [|sprintf "Unknown how to process message [%s]" message|]       

    /// sends one response line through the socket
    let sendMessage (handler : Socket) message =
        log <| sprintf "Sending data: %s" message
        let s = (if message = null then "" else message) + "\n"
        let bytes = Encoding.ASCII.GetBytes(s)
        handler.Send(bytes) |> ignore
     
    /// sets up and runs the infinite listen/respond loop  
    member this.Run() =
        use listener = setup ()
        log <| sprintf "Hosted compiler started, listening on port %d" port

        let rec loop _ =
            log ""
            log "Waiting for a connection..."

            let handler = listener.Accept()
            let message = receiveMessage handler
            let response =
                try
                    processMessage message
                with e ->
                    log "*** Error in host ***"
                    log (e.ToString())

                    [|"Compiler host server error"|]

            response |> Array.iter (sendMessage handler)
            handler.Shutdown(SocketShutdown.Both)
            handler.Close()

            loop ()

        try
            loop ()
        with e ->
            log <| sprintf "%s" (e.ToString())

module Program =
    [<EntryPoint>]
    let main argv =
        let port =
            if argv.Length = 1 then
                match Int32.TryParse(argv.[0]) with
                | (true, p) -> p
                | _ -> 11000
            else 11000
        let server = HostedCompilerServer(port)
        server.Run()
        0
