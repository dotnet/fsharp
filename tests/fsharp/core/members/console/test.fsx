// #Conformance #Interop #Events #MemberDefinitions 

type ControlEvent = CTRL_C | CTRL_BREAK | CTRL_CLOSE |CTRL_LOGOFF | CTRL_SHUTDOWN 
  with 
     member x.ToInt = 
       match x with 
       | CTRL_C -> 0
       | CTRL_BREAK -> 1
       | CTRL_CLOSE -> 2
       | CTRL_LOGOFF -> 3
       | CTRL_SHUTDOWN -> 4
     static member OfInt(n) =
       match n with
       | 0 -> CTRL_C 
       | 1 -> CTRL_BREAK 
       | 2 -> CTRL_CLOSE 
       | 3 -> CTRL_LOGOFF 
       | 4 -> CTRL_SHUTDOWN 
       |  _ -> invalid_arg "ControlEvent.ToInt"
  end


open System
open System.Runtime.InteropServices
open Idioms

type ControlEventHandler = delegate of int -> int

[<DllImport("kernel32.dll")>]
let SetConsoleCtrlHandler((callback:ControlEventHandler),(add: bool)) : unit = ()

/// Class to catch console control events (ie CTRL-C) in C#.
/// Calls SetConsoleCtrlHandler() in Win32 API
type ConsoleCtrl = class

    /// Handler to be called when a console event occurs.
    val listeners : Idioms.EventListeners<ControlEvent>
    member x.ControlEvent = x.listeners.Event

    // Save the callback to a private var so the GC doesn't collect it.
    val mutable eventHandler : ControlEventHandler option

    /// Create a new instance.
    new() as x = 
      let eh = new ControlEventHandler(fun i -> x.listeners.Fire(ControlEvent.OfInt(i)); 1) in 
      SetConsoleCtrlHandler(eh,true);
      { listeners = new EventListeners<_>();
        eventHandler = Some eh }

    /// Remove the event handler
    member x.Dispose(disposing)  = 
       match x.eventHandler with 
       | Some h -> 
          SetConsoleCtrlHandler(h, false);
          x.eventHandler <- None
       | None -> ()

    interface IDisposable with 
      member x.Dispose() = x.Dispose(true)
    end
    override x.Finalize() = x.Dispose(true)
end


let main() = 
  let cc = new ConsoleCtrl() in 
  cc.ControlEvent.Add(fun ce -> Console.WriteLine("Event: {0}", ce));
  Console.WriteLine("Enter 'E' to exit");
  while (true) do
    let s = Console.ReadLine() in 
    if (s == "E") then
      exit 1;
  done


do main()
