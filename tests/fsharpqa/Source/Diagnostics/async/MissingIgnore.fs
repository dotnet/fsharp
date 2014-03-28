// #Regression #Diagnostics #Async 
// Regression tests for FSHARP1.0:4394
//<Expects status="error" span="(4,9-4,10)" id="FS0020">This expression should have type 'unit', but has type 'int'\. Use 'ignore' to discard the result of the expression, or 'let' to bind the result to a name\.$</Expects>
async { 1;
        return 2 }  |> ignore
