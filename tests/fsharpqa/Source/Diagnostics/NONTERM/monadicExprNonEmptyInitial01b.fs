// #Regression #Diagnostics 
// Regression test for FSharp1.0:2104
//<Expects status="notin">NONTERM</Expects>
//<Expects status="error" span="(15,1)" id="FS0528">Unexpected end of input$</Expects>
#light 

open Microsoft.FSharp.Control

let counter = 
    MailboxProcessor.Start( fun inbox -> 
        let rec loop(n) =
            async { do printfn "n = %d, waiting..." n
                    let! msg = inbox.Receive()
                    return
