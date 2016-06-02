// #Regression #Diagnostics #Async 
// Regression tests for FSHARP1.0:4394
//<Expects status="error" span="(4,9-4,10)" id="FS0020">This expression has a value of type 'int' that is implicitly ignored\. Use the 'ignore' function to discard this value explicitly, e\.g\. 'expr \|> ignore', or bind it to a name to refer to it later, e\.g\. 'let result = expr'\.$</Expects>
async { 1;
        return 2 }  |> ignore
