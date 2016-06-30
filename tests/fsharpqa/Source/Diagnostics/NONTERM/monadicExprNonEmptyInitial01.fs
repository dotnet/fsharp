// #Regression #Diagnostics 
// Regression test for FSharp1.0:2104

//<Expects span="(19,19-19,20)" status="error" id="FS0604">Unmatched '\{'$</Expects>
//<Expects span="(22,1-22,1)" status="error" id="FS0528" >Unexpected end of input$</Expects>
//<Expects status="error" span="(18,9-18,12)" id="FS3118">Incomplete value or function definition\. If this is in an expression, the body of the expression must be indented to the same column as the 'let' keyword\.$</Expects>
//<Expects span="(18,9-18,12)" status="error" id="FS0588">The block following this 'let' is unfinished. Every code block is an expression and must have a result. 'let' cannot be the final code element in a block. Consider giving this block an explicit result.</Expects>
//<Expects span="(22,1-22,1)" status="error" id="FS0528">Unexpected end of input$</Expects>

//<Expects status="error" span="(17,29-17,32)" id="FS3110">Unexpected end of input in body of lambda expression\. Expected 'fun <pat> \.\.\. <pat> -> <expr>'\.$</Expects>
//<Expects status="error" span="(17,27-17,28)" id="FS0583">Unmatched '\('$</Expects>
//<Expects status="error" span="(16,1-16,4)" id="FS3118">Incomplete value or function definition\. If this is in an expression, the body of the expression must be indented to the same column as the 'let' keyword\.$</Expects>

open Microsoft.FSharp.Control

let counter = 
    MailboxProcessor.Start( fun inbox -> 
        let rec loop(n) =
            async { do printfn "n = %d, waiting..." n
                    let! msg = inbox.Receive()
                    return
