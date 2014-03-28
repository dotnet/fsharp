// #Regression #Diagnostics 
//<Expects id="FS0807" span="(8,9-8,20)" status="error">Property 'X' is not readable$</Expects>
// See also FSHARP1.0:4963 ("property" should read "Property")
module M
type T() =
     member this.X
       with set (x:int) = ()
let _ = (new T()).X(1)      // Property X is not readable
