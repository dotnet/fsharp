namespace Microsoft.FSharp.Core
    open System
    open System.Linq

    [<Sealed; AbstractClass>]
    type FuncConvertExtensions = 

        static member  ToFSharpFunc( f : Action<_,_>) = (fun a1 a2 -> f.Invoke(a1,a2))
        static member  ToFSharpFunc( f : Func<_>) = (fun () -> f.Invoke())
        static member  ToFSharpFunc( f : Func<_,_>) = (fun a1 -> f.Invoke(a1))
        static member  ToFSharpFunc( f : Func<_,_,_>) = (fun a1 a2 -> f.Invoke(a1,a2))
        static member  ToFSharpFunc( f : Action<_,_,_>) = (fun a1 a2 a3 -> f.Invoke(a1,a2,a3))
        static member  ToFSharpFunc( f : Func<_,_,_,_>) = (fun a1 a2 a3 -> f.Invoke(a1,a2,a3))
        static member  ToFSharpFunc( f : Action<_,_,_,_>) = (fun a1 a2 a3 a4 -> f.Invoke(a1,a2,a3,a4))
        static member  ToFSharpFunc( f : Func<_,_,_,_,_>) = (fun a1 a2 a3 a4 -> f.Invoke(a1,a2,a3,a4))
        static member  ToTupledFSharpFunc( f : Func<_>) = (fun () -> f.Invoke())
        static member  ToTupledFSharpFunc( f : Func<_,_>) = (fun a1 -> f.Invoke(a1))
        static member  ToTupledFSharpFunc( f : Action<_,_>) = (fun (a1,a2) -> f.Invoke(a1,a2))
        static member  ToTupledFSharpFunc( f : Func<_,_,_>) = (fun (a1,a2) -> f.Invoke(a1,a2))
        static member  ToTupledFSharpFunc( f : Action<_,_,_>) = (fun (a1,a2,a3) -> f.Invoke(a1,a2,a3))
        static member  ToTupledFSharpFunc( f : Func<_,_,_,_>) = (fun (a1,a2,a3) -> f.Invoke(a1,a2,a3))
        static member  ToTupledFSharpFunc( f : Action<_,_,_,_>) = (fun (a1,a2,a3,a4) -> f.Invoke(a1,a2,a3,a4))
        static member  ToTupledFSharpFunc( f : Func<_,_,_,_,_>) = (fun (a1,a2,a3,a4) -> f.Invoke(a1,a2,a3,a4))


