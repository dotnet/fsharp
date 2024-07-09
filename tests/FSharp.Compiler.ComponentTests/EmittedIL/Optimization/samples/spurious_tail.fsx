// <testmetadata>
// { "optimization": { "reported_in": "#6329", "reported_by": "@NinoFloris", "last_know_version_not_optimizing": "8", "first_known_version_optimizing": null } }
// </testmetadata>
open System.Runtime.CompilerServices

type LoggerLevel = A = 0 | B  = 1 

type Logger() =
    let messages = System.Collections.Generic.List<string>(10000);
    
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    let nop () = ()
    member this.Log(level : LoggerLevel, msg : string) =
        messages.Add(msg)
        nop()

    member this.Log(level : LoggerLevel, obj : 'a) =
        messages.Add(obj.ToString())
        nop()

type TailLogger() =
    let messages = System.Collections.Generic.List<string>(10000);
    
    member this.Log(level : LoggerLevel, msg : string) =
        messages.Add(msg)

    member this.Log(level : LoggerLevel, obj : 'a) =
        messages.Add(obj.ToString())

System.Console.WriteLine()