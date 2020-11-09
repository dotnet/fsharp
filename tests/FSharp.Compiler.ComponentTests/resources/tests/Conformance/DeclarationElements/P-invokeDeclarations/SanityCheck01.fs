// #Conformance #DeclarationElements #PInvoke 
#light

// Sanity check PInvoke from F#
open System.Runtime.InteropServices

[<DllImport("msvcrt.dll", EntryPoint="puts", CallingConvention = CallingConvention.Cdecl)>]
extern int PInvoke_puts(string c);

[<DllImport("msvcrt.dll", EntryPoint="atof", CallingConvention = CallingConvention.Cdecl)>]
extern float PInvoke_atof(string c);


PInvoke_puts("This is totally going to call the VC Runtime Library's put method!")

let result = PInvoke_atof("3.14")
if result <> 3.14 then exit 1

exit 0


