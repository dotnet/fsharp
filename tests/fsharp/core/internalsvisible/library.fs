module Library

open System.Runtime.CompilerServices
// This one would trigger the warning about an invalid assembly name
// It also makes peverify fail on main.exe
// [<assembly:InternalsVisibleTo(",,,,,,,,")>]
// This one exposes the library internals to the main.exe, note PublicKey obtained via:
// secutil -hex -s main.exe 
[<assembly:InternalsVisibleTo("main, PublicKey=0024000004800000940000000602000000240000525341310004000001000100DDA0D353F027AB6ADC878BFBFE4A07FB00FFD2EDD5F255A14C6474B7F4E561796822B6B3CF83D81716C6AFE9BE5D343D7F99EF98252EAD91E7C3C4DF043FDD71FD3130F6611C3C0F7D4F3E698491E9B74D4DE456042A737F4FC5443A98BF989B7377BEE0969C58B85C26B48EF94FFBC95E68E10545FB573243E249204921AFB8")>]
do()

module M =
  // Recursive functions be sure they will not be inlined.
  // If they could be inlined then the compiler could grant them accessibility and inline them,
  // and the peverify checks for valid accessibility would be bypassed.
  let rec private  privateF  (x:int) = if x>0 then x*2 else privateF  (x+1)
  let rec internal internalF (x:int) = if x>0 then x*3 else internalF (x+1)
  /// mark internal in signature file, but not at definition
  let rec signatureInternalF (x:int) = if x>0 then x*4 else signatureInternalF (x+1)   
  let rec public   publicF   (x:int) = if x>0 then x*5 else publicF (x+1)

module internal P = 
    type internal InternalClass(x:int) =
        member this.X = x

    let internal InternalObject = InternalClass(999)    
