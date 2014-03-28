// #Conformance #Quotations 
#light

// Sanity check raw quotations

open Microsoft.FSharp.Quotations

let typedQuote : Expr<int> = <@  42  @>
let rawQuote   : Expr      = <@@ 42 @@>

if rawQuote.Type  <> typeof<int> then exit 1
if typedQuote.Raw <> rawQuote    then exit 1

exit 0
        
        
