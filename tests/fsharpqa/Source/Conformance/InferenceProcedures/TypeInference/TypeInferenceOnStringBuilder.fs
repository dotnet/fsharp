//<Expects status="success"></Expects>

open System.Text
open System.Runtime.CompilerServices
open System.Runtime.InteropServices

[<Extension; Sealed>]
type T = 
    [<Extension>]static member Append (x:StringBuilder, y:StringBuilder) = StringBuilder().Append(x).Append(y)
    
exit 0