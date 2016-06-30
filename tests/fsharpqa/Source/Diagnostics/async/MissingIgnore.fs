// #Regression #Diagnostics #Async 
// Regression tests for FSHARP1.0:4394
//<Expects status="error" span="(4,9-4,10)" id="FS0020">The result of this expression is implicitly ignored\. Consider using 'ignore' to discard this value explicitly, e\.g\. 'expr \|> ignore', or 'let' to bind the result to a name, e\.g\. 'let result = expr'.$</Expects>
async { 1;
        return 2 }  |> ignore
