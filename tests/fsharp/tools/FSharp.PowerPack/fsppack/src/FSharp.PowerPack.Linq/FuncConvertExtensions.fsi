namespace Microsoft.FSharp.Core
    open System

    [<Sealed; AbstractClass>]
    type FuncConvertExtensions =

        static member  ToFSharpFunc       : Func<'U>     -> (unit -> 'U)
        
        static member  ToFSharpFunc       : Func<'T1,'U>     -> ('T1 -> 'U)
        
        static member  ToFSharpFunc       : Action<'T1,'T2>           -> ('T1 -> 'T2 -> unit)
        
        static member  ToFSharpFunc       : Func<'T1,'T2,'U>     -> ('T1 -> 'T2 -> 'U)
        
        static member  ToFSharpFunc       : Action<'T1,'T2,'T3>       -> ('T1 -> 'T2 -> 'T3 -> unit)
        
        static member  ToFSharpFunc       : Func<'T1,'T2,'T3,'U> -> ('T1 -> 'T2 -> 'T3 -> 'U)

        static member  ToFSharpFunc       : Action<'T1,'T2,'T3,'T4>       -> ('T1 -> 'T2 -> 'T3 -> 'T4 -> unit)
        
        static member  ToFSharpFunc       : Func<'T1,'T2,'T3,'T4,'U> -> ('T1 -> 'T2 -> 'T3 -> 'T4 -> 'U)

        static member  ToTupledFSharpFunc : Func<'U>     -> (unit -> 'U)
        
        static member  ToTupledFSharpFunc : Func<'T1,'U>     -> ('T1 -> 'U)
        
        static member  ToTupledFSharpFunc : Action<'T1,'T2>           -> ('T1 * 'T2 -> unit)
        
        static member  ToTupledFSharpFunc : Func<'T1,'T2,'U>     -> ('T1 * 'T2 -> 'U)
        
        static member  ToTupledFSharpFunc : Action<'T1,'T2,'T3>       -> ('T1 * 'T2 * 'T3 -> unit)
        
        static member  ToTupledFSharpFunc : Func<'T1,'T2,'T3,'U> -> ('T1 * 'T2 * 'T3 -> 'U)

        static member  ToTupledFSharpFunc : Action<'T1,'T2,'T3,'T4>       -> ('T1 * 'T2 * 'T3 * 'T4 -> unit)
        
        static member  ToTupledFSharpFunc : Func<'T1,'T2,'T3,'T4,'U> -> ('T1 * 'T2 * 'T3 * 'T4 -> 'U)

