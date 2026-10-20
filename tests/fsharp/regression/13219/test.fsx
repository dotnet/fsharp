#r "nuget: FSharp.Data, 4.2.10"

open FSharp.Data

[<Literal>]
let sample = "<table><tr><th>A</th></tr><tr><td>1</td></tr></table>"

// Works
let html = new HtmlProvider<sample>()

type System.Object with

    // Works
    member x.Html1 = new HtmlProvider<"<table><tr><th>A</th></tr><tr><td>1</td></tr></table>">()

    // Error: FS0267 This is not a valid constant expression or custom attribute value
    member x.Html2 = new HtmlProvider<sample>()

// This is a compilation test, not a lot actually happens in the test
do (System.Console.Out.WriteLine "Test Passed"; 
    printf "TEST PASSED OK" ; 
    exit 0)
