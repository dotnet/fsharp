// #Conformance #ComputationExpressions #Async 
#if Portable
module Core_controlChamenos
#endif
#light

#nowarn "40" // recursive references

let biggerThanTrampoliningLimit = 10000

let failuresFile =
   let f = System.Environment.GetEnvironmentVariable("CONTROL_FAILURES_LOG")
   match f with
   | "" | null -> "failures.log"
   | _ -> f

let log msg = 
  printfn "%s" msg
  System.IO.File.AppendAllText(failuresFile, sprintf "%A: %s\r\n" System.DateTime.Now msg)

let mutable failures = []
let syncObj = new obj()
let report_failure s = 
  stderr.WriteLine " NO"; 
  lock syncObj (fun () ->
     failures <- s :: failures;
     log (sprintf "FAILURE: %s failed" s)
  )

#if Portable
#else
System.AppDomain.CurrentDomain.UnhandledException.AddHandler(
       fun _ (args:System.UnhandledExceptionEventArgs) ->
          lock syncObj (fun () ->
                failures <- (args.ExceptionObject :?> System.Exception).ToString() :: failures
             )
)
#endif

let test s b = stderr.Write(s:string);  if b then stderr.WriteLine " OK" else report_failure s 

let checkQuiet s x1 x2 = 
    if x1 <> x2 then 
        (test s false; printfn "expected: %A, got %A" x2 x1)

let check s x1 x2 = 
    if x1 = x2 then test s true
    else (test s false; printfn "expected: %A, got %A" x2 x1)

#if NetCore
#else
let argv = System.Environment.GetCommandLineArgs() 
let SetCulture() = 
  if argv.Length > 2 && argv.[1] = "--culture" then  begin
    let cultureString = argv.[2] in 
    let culture = new System.Globalization.CultureInfo(cultureString) in 
    stdout.WriteLine ("Running under culture "+culture.ToString()+"...");
    System.Threading.Thread.CurrentThread.CurrentCulture <-  culture
  end 
  
do SetCulture()    
#endif

open Microsoft.FSharp.Control
open Microsoft.FSharp.Control.WebExtensions

type Color = Blue | Red | Yellow
let complement = function
    | (Red, Yellow) | (Yellow, Red) -> Blue
    | (Red, Blue) | (Blue, Red) -> Yellow
    | (Yellow, Blue) | (Blue, Yellow) -> Red
    | (Blue, Blue) -> Blue
    | (Red, Red) -> Red
    | (Yellow, Yellow) -> Yellow

type Message =  Color * AsyncReplyChannel<Color option>

let chameleon (meetingPlace : MailboxProcessor<Message>) initial = 
    let rec loop c meets = async  {
            let! replyMessage = meetingPlace.PostAndAsyncReply(fun reply -> c, reply)
            match replyMessage with     
            | Some(newColor) -> return! loop newColor (meets + 1)
            | None -> 
                return meets
        }
    loop initial 0
    

let meetingPlace chams n = MailboxProcessor.Start(fun (processor : MailboxProcessor<Message>)->
    let rec fadingLoop total = 
        async   {
            if total <> 0 then
                let! (_, reply) = processor.Receive()
                reply.Reply None
                return! fadingLoop (total - 1)
            else
                printfn "Done"
        }
    let rec mainLoop curr = 
        async   {
            if (curr > 0) then
                let! (color1, reply1) = processor.Receive()
                let! (color2, reply2) = processor.Receive()
                let newColor = complement (color1, color2)
                reply1.Reply <| Some(newColor)
                reply2.Reply <| Some(newColor)                
                return! mainLoop (curr - 1)
            else
                return! fadingLoop chams
        }
    mainLoop n
    ) 

open System
open System.Diagnostics


let () =
    let meetings = 100000
    
    let colors = [Blue; Red; Yellow; Blue]    
    let mp = meetingPlace (colors.Length) meetings
    let meets = 
            colors 
                |> List.map (chameleon mp) 
                |> Async.Parallel 
                |> Async.RunSynchronously 

    check "Chamenos" (Seq.sum meets) (meetings*2)

#if Portable
let aa = 
    if not failures.IsEmpty then exit 1
    else 
        stdout.WriteLine "Test Passed"
        exit 0
#else
let _ = 
  if not failures.IsEmpty then (stdout.WriteLine("Test Failed, failures = {0}", failures); exit 1) 
  else (stdout.WriteLine "Test Passed"; 
        System.IO.File.WriteAllText("test.ok","ok"); 
// debug: why is the fsi test failing?  is it because test.ok does not exist?
        if System.IO.File.Exists("test.ok") then
            stdout.WriteLine ("test.ok found at {0}", System.IO.FileInfo("test.ok").FullName)
        else
            stdout.WriteLine ("test.ok not found")
        exit 0)
#endif

