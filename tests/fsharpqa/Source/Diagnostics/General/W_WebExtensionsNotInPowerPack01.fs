// #Regression #Diagnostics #RequiresPowerPack 
// Regression for FSHARP1.0:5879
//<Expects status="warning" id="FS0044" span="(9,9-9,46)">This construct is deprecated. The extension method now resides in the 'WebExtensions' module in the F# core library\. Please add 'open Microsoft\.FSharp\.Control\.WebExtensions' to access this method</Expects>
//<Expects status="warning" id="FS0044" span="(12,10-12,31)">This construct is deprecated. The extension method now resides in the 'WebExtensions' module in the F# core library\. Please add 'open Microsoft\.FSharp\.Control\.WebExtensions' to access this method</Expects>

module T

let x = new System.Net.WebClient()
let r = x.AsyncDownloadString(System.Uri(""))

let x2 = System.Net.WebRequest.Create(System.Uri(""))
let r2 = x2.AsyncGetResponse()

open Microsoft.FSharp.Control.WebExtensions

let x3 = new System.Net.WebClient()
let r3 = x.AsyncDownloadString(System.Uri(""))

let x4 = System.Net.WebRequest.Create(System.Uri(""))
let r4 = x2.AsyncGetResponse()

exit 0
