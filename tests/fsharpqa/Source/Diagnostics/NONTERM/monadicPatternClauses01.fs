// #Regression #Diagnostics 
// Regression test for FSharp1.0:2106
//<Expects id="FS0010" span="(22,25-22,32)" status="error"></Expects>

#light 

open Microsoft.FSharp.Control

type internal msg = 
    | Increment of int
    | Fetch of IChannel<int>
    | Stop
   
type CountingAgent() = 
    let counter = 
        MailboxProcessor.Start( fun inbox -> 
            let rec loop(n) =
                async { let! msg = inbox.Receive()
                        match msg with
                        | Increment m -> return! loop(n+m)
                        |
                        return! loop(n+msg) }
            loop(0))

for i = 0 to 500 do
    counter.Post(1)
