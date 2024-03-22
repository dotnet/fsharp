module M
[<System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)>]
let inline getUnit (f : unit -> unit) = f()