// #Regression #Diagnostics 
// Regression test for FSHARP1.0:2099
// Regression test for FSHARP1.0:2670
//<Expects id="FS0010" span="(18,50-18,52)" status="error">Unexpected symbol '<-' in binding. Expected '=' or other token.</Expects>
//<Expects id="FS0588" span="(18,42-18,45)" status="error">The block following this 'let' is unfinished. Every code block is an expression and must have a result. 'let' cannot be the final code element in a block. Consider giving this block an explicit result.</Expects>
//<Expects status="notin">lambda</Expects>


open Microsoft.FSharp.Control

let mutable sem = 0

let counter = 
    MailboxProcessor.Start( fun inbox -> 
        let rec loop(n) =
            async { do printfn "n = %d, waiting..." n
                    let! msg = inbox.Receive()
                    if n+msg >= 500 then let sem <- 1
                    return! loop(n+msg) }
        loop(0))
        
for i = 0 to 500 do
    counter.Post(1)
