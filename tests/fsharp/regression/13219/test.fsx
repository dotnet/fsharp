#r "nuget: FSharp.Data, 4.2.10"

open FSharp.Data

[<Literal>]
let url = "https://en.wikipedia.org/wiki/F_Sharp_(programming_language)"

// Works
let html = new HtmlProvider<url>()

type System.Object with

    // Works
    member x.Html1 = new HtmlProvider<"https://en.wikipedia.org/wiki/F_Sharp_(programming_language)">()

    // Error: FS0267 This is not a valid constant expression or custom attribute value
    member x.Html2 = new HtmlProvider<url>()
