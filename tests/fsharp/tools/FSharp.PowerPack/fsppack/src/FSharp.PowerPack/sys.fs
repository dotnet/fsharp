module Microsoft.FSharp.Compatibility.OCaml.Sys

#nowarn "52" // defensive value copy warning, only with warning level 4

open System
open System.Reflection
open Microsoft.FSharp.Control

#if FX_NO_COMMAND_LINE_ARGS
#else
let argv = System.Environment.GetCommandLineArgs()
#endif
let file_exists  (s:string) = System.IO.File.Exists(s)
let remove (s:string) = System.IO.File.Delete(s)
let rename (s:string) (s2:string) = System.IO.File.Move(s,s2)

#if FX_NO_ENVIRONMENT
#else
let getenv (s:string) =
    match System.Environment.GetEnvironmentVariable(s) with 
    | null -> raise (System.Collections.Generic.KeyNotFoundException("the given environment variable was not found"))
    | s -> s
#endif

#if FX_NO_PROCESS_START
#else
let command (s:string) = 
    let psi = new System.Diagnostics.ProcessStartInfo("cmd","/c "^s) 
    psi.UseShellExecute <- false;
    let p = System.Diagnostics.Process.Start(psi) 
    p.WaitForExit();
    p.ExitCode
#endif

let chdir (s:string) = System.IO.Directory.SetCurrentDirectory(s)
let getcwd () = System.IO.Directory.GetCurrentDirectory()

let word_size = sizeof<int> * 8

#if FX_NO_PROCESS_DIAGNOSTICS
#else
// Sys.time only returns the process time from the main thread
// The documentation doesn't guarantee that thread 0 is the main thread, 
// but it always appears to be.  
let mainThread = 
    lazy 
      (let thisProcess = System.Diagnostics.Process.GetCurrentProcess() 
       let threads = thisProcess.Threads 
       threads.[0])


let time() = 
    try mainThread.Force().TotalProcessorTime.TotalSeconds
    with _ -> 
      // If the above failed, e.g. because main thread has exited, then do the following
      System.Diagnostics.Process.GetCurrentProcess().UserProcessorTime.TotalSeconds
#endif

#if FX_NO_APP_DOMAINS
#else
let executable_name = 
    System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory,
                           System.AppDomain.CurrentDomain.FriendlyName)  
#endif
