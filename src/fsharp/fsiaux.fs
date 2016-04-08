// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Compiler.Interactive

open System
open System.Diagnostics
open System.Threading

[<assembly: System.Runtime.InteropServices.ComVisible(false)>]
[<assembly: System.CLSCompliant(true)>]  
do()

type IEventLoop =
    abstract Run : unit -> bool
    abstract Invoke : (unit -> 'T) -> 'T 
    abstract ScheduleRestart : unit -> unit
    
// An implementation of IEventLoop suitable for the command-line console
[<AutoSerializable(false)>]
type internal SimpleEventLoop() = 
    let runSignal = new AutoResetEvent(false)
    let exitSignal = new AutoResetEvent(false)
    let doneSignal = new AutoResetEvent(false)
    let queue = ref ([] : (unit -> obj) list)
    let result = ref (None : obj option)
    let setSignal(signal : AutoResetEvent) = while not (signal.Set()) do Thread.Sleep(1); done
    let waitSignal signal = WaitHandle.WaitAll([| (signal :> WaitHandle) |]) |> ignore
    let waitSignal2 signal1 signal2 = 
        WaitHandle.WaitAny([| (signal1 :> WaitHandle); (signal2 :> WaitHandle) |])
    let running = ref false
    let restart = ref false
    interface IEventLoop with 
         member x.Run() =  
             running := true;
             let rec run() = 
                 match waitSignal2 runSignal exitSignal with 
                 | 0 -> 
                     !queue |> List.iter (fun f -> result := try Some(f()) with _ -> None); 
                     setSignal doneSignal;
                     run()
                 | 1 -> 
                     running := false;
                     !restart
                 | _ -> run()
             run();
         member x.Invoke(f : unit -> 'T) : 'T  = 
             queue := [f >> box];
             setSignal runSignal;
             waitSignal doneSignal
             !result |> Option.get |> unbox
         member x.ScheduleRestart() = 
             // nb. very minor race condition here on running here, but totally 
             // unproblematic as ScheduleRestart and Exit are almost never called.
             if !running then 
                 restart := true; 
                 setSignal exitSignal
    interface System.IDisposable with 
         member x.Dispose() =
                     runSignal.Dispose();
                     exitSignal.Dispose();
                     doneSignal.Dispose();
                     


[<Sealed>]
type InteractiveSession()  = 
    let mutable evLoop = (new SimpleEventLoop() :> IEventLoop)
    let mutable showIDictionary = true
    let mutable showDeclarationValues = true
    let mutable args = System.Environment.GetCommandLineArgs()         
    let mutable fpfmt = "g10"
    let mutable fp = (System.Globalization.CultureInfo.InvariantCulture :> System.IFormatProvider)
    let mutable printWidth = 78
    let mutable printDepth = 100
    let mutable printLength = 100
    let mutable printSize = 10000
    let mutable showIEnumerable = true
    let mutable showProperties = true
    let mutable addedPrinters = []

    member self.FloatingPointFormat with get() = fpfmt and set v = fpfmt <- v
    member self.FormatProvider with get() = fp and set v = fp <- v
    member self.PrintWidth  with get() = printWidth and set v = printWidth <- v
    member self.PrintDepth  with get() = printDepth and set v = printDepth <- v
    member self.PrintLength  with get() = printLength and set v = printLength <- v
    member self.PrintSize  with get() = printSize and set v = printSize <- v
    member self.ShowDeclarationValues with get() = showDeclarationValues and set v = showDeclarationValues <- v
    member self.ShowProperties  with get() = showProperties and set v = showProperties <- v
    member self.ShowIEnumerable with get() = showIEnumerable and set v = showIEnumerable <- v
    member self.ShowIDictionary with get() = showIDictionary and set v = showIDictionary <- v
    member self.AddedPrinters with get() = addedPrinters and set v = addedPrinters <- v

    [<CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")>]
    member self.CommandLineArgs
       with get() = args 
       and set v  = args <- v

    member self.AddPrinter(printer : 'T -> string) =
      addedPrinters <- Choice1Of2 (typeof<'T>, (fun (x:obj) -> printer (unbox x))) :: addedPrinters

    member self.EventLoop
       with get () = evLoop
       and set (x:IEventLoop)  = evLoop.ScheduleRestart(); evLoop <- x

    member self.AddPrintTransformer(printer : 'T -> obj) =
      addedPrinters <- Choice2Of2 (typeof<'T>, (fun (x:obj) -> printer (unbox x))) :: addedPrinters
    
[<assembly: CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly", Scope="member", Target="Microsoft.FSharp.Compiler.Interactive.InteractiveSession.#ThreadException")>]
do()
  
  
module Settings = 
    let fsi = new InteractiveSession()
   
    [<assembly: AutoOpen("Microsoft.FSharp.Compiler.Interactive.Settings")>]
    do()

module RuntimeHelpers = 
    open System
    open System.Reflection

    let internal savedIt = ref (typeof<int>,box 0)
    let SaveIt (x:'T) = (savedIt := (typeof<'T>, box x))
    let internal GetSavedIt () = snd !savedIt
    let internal GetSavedItType () = fst !savedIt