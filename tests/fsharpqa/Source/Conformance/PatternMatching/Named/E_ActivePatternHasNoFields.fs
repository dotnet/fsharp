// #Regression #Conformance #PatternMatching #ActivePatterns 
// Regression test for https://github.com/Microsoft/visualfsharp/issues/5745
//<Expects status="error" id="FS3174">Active patterns do not have fields. This syntax is invalid\.</Expects>
open System.Text.RegularExpressions

let (|USZipPlus4Code|_|) s =
    let m = Regex.Match(s, @"^(\d{5})\-(\d{4})$")
    if m.Success then
        USZipPlus4Code(x=m.Groups.[1].Value, 
                       y=m.Groups.[2].Value)
        |> Some
    else
        None
