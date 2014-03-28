namespace Microsoft.FSharp.Math.Experimental

// NOTE: Abstractable code.
//   This file is a Service/Provider model cache with pre-checks that supporting DLLs are available.
//   This is not LAPACK specific, and is more generally useful.
//   It has been abstracted over the underlying service type (ILapack becomes 'a).
//   The providers could have other pre-conditions, e.g. machine architecture, OS, etc.

open Microsoft.FSharp.Collections
open System.IO
open System.Collections.Generic
 
/// Generic provider with unmanaged DLL dependencies.
type Provider<'a>(name:string,requiredDLLs:string[],provide:unit -> 'a) = 
    // NOTE: The dependencies could be extended to include architecture.
    member this.Name         = name 
    member this.RequiredDLLs = requiredDLLs
    member this.Provide()    = provide()   
  
module Locals =     
    let mkProvider (name,requireDLLs,provide) = Provider(name,requireDLLs,provide) 
    
    let noRepeats xs = xs |> Set.ofList |> Set.toList
    let noLaterRepeats xs =
      let collect (soFar,revXs) x =
        if Set.contains x soFar then (soFar,revXs) else (Set.add x soFar,x::revXs)
      let (_,revXs) = List.fold collect (Set.empty,[]) xs
      List.rev revXs
    
    let searchPathsForDLLImports() =         
      // The providers have DLL dependencies via their platform-invoke DLLImport attributes.
      // The DLLs search procedure for those DLLs is described below. (follow links).
      // Here, we compute those paths (a superset of them).
      // 
      // From: VC++ Programming Guide: Search Path Used by Windows to Locate a DLL
      //   http://msdn2.microsoft.com/en-us/library/7d83bc18(VS.80).aspx
      //
      // Search order:
      //   The directory where the executable module for the current process is located.
      //   The current directory.
      //   The Windows system directory. The GetSystemDirectory function retrieves the path of this directory.
      //   The Windows directory. The GetWindowsDirectory function retrieves the path of this directory.
      //   The directories listed in the PATH environment variable.
      //
      // From: Development Impacts of Security Changes in Windows Server 2003
      //   http://msdn2.microsoft.com/en-us/library/ms972822.aspx
      // This reports that system directories are now searched first.
      //
      let windowsSystemDir = System.Environment.SystemDirectory
      let windowsDir       = System.IO.Path.GetDirectoryName(windowsSystemDir)
      let currentExeDirs   =
        // This includes EXE directory, and loaded DLL directories.
        // It may be an over-estimate of the search path.
        let proc = System.Diagnostics.Process.GetCurrentProcess()
        let mods  : System.Diagnostics.ProcessModule list = proc.Modules |> Seq.cast |> Seq.toList
        mods     |> List.map (fun m -> Path.GetDirectoryName m.FileName) |> noRepeats
      let currentDir = System.Environment.CurrentDirectory 
      let pathDirs =
        match System.Environment.GetEnvironmentVariable("PATH") with
        | null  -> []
        | paths -> paths.Split([|';'|]) |> Array.toList      
      let orderedSearchPaths = windowsSystemDir :: windowsDir :: (currentExeDirs @ [currentDir] @ pathDirs)
      noLaterRepeats orderedSearchPaths
    
    let pathDLLs (path:string) = if not (Directory.Exists path) then [||] else Directory.GetFiles(path,"*.DLL")       
    
    let dllFilename (dll:string) = (Path.GetFileName dll).ToLower() // normalizes filename
        
    let loadableDLLPaths() =
      // Makes a table of (DLL,availablePaths)
      // This is reusable code...
      let paths = searchPathsForDLLImports()
      let dllPaths = new Dictionary<string,ResizeArray<string>>()
      let add path dll = 
        let dll = dllFilename dll
        if not (dllPaths.ContainsKey(dll)) then dllPaths.[dll] <- new ResizeArray<_>()
        dllPaths.[dll].Add(path)
      List.iter (fun path -> Array.iter (add path) (pathDLLs path)) paths
      dllPaths

    let loadableProvider (dllPaths:Dictionary<_,_>) (provider:Provider<'a>) = 
      let dllAvailable    (dll:string) = dllPaths.ContainsKey(dllFilename dll)
      let availableReason (dll:string) = let quote s = "'" + s + "'"
                                         let paths = dllPaths.[dllFilename dll] |> ResizeArray.toList
                                         let pathNote path = sprintf "Required %s seen in %s" dll path
                                         System.String.Join("\n", Array.ofList(List.map pathNote paths))
      if Array.forall dllAvailable provider.RequiredDLLs then
        let justification = System.String.Join("\n", Array.map availableReason provider.RequiredDLLs)
        let justification = "Provider: " ^ provider.Name ^ "\n" ^ justification
        Some (provider,justification)
      else
        None
        
    let checkProvider p = 
      match loadableProvider (loadableDLLPaths()) p with
      | None                   -> "Provider is not loadable"
      | Some (p,justification) -> justification
        
    // This type is internal to Service (motivates nested types).
    type 'a ServiceState =
    | ServiceDisabled                          // service disabled, do not look for it.
    | ServiceEnabledUninitialised              // service enabled, but no search made yet.
    | ServiceEnabledOK     of 'a * string      // service enabled, and justification string for diagnostics.
    | ServiceEnabledFailed                     // service enabled, but DLLs not found, or load failed.

open Locals
  
type Provider<'a> with
    member this.Check() = checkProvider this

type Service<'a>(providers:Provider<'a> seq) =
    let mutable providers = Seq.toArray providers                // possible providers configuration state
    let mutable state = ServiceEnabledUninitialised               // service state
    
    /// Service Providers
    member this.Providers with get()  = providers
                           and set(x) = providers <- x
    
    /// Disable the service.
    member this.Stop()       = state <- ServiceDisabled

    /// Use the LAPACK service from the given provider.
    /// If the supporting DLLs are not available, this may fail (now or later).
    member this.StartWith(p:Provider<'a>) =
      let justification = 
          match loadableProvider (loadableDLLPaths()) p with
          | None                   -> "The provider DLLs did not appear to be present, the service may fail"
          | Some (p,justification) -> justification      
      state <- ServiceEnabledOK (p.Provide(),justification)

    /// Start the service with the first provider that looks loadable.     
    member this.Start() =
      let candidates = Array.choose (loadableProvider (loadableDLLPaths())) providers
      if candidates.Length=0 then                                     // guard
        state <- ServiceEnabledFailed
        false
      else
        let provider,justification = candidates.[0]
        state <- ServiceEnabledOK (provider.Provide(),justification)  // index covered by guard above
        true

    member this.Service() = 
      match state with
      | ServiceEnabledUninitialised -> this.Start() |> ignore
      | _ -> ()
      match state with
        | ServiceDisabled                          
        | ServiceEnabledUninitialised  // (The above initialisation call must have failed)
        | ServiceEnabledFailed                     -> None
        | ServiceEnabledOK (service,justification) -> Some service
    
    member this.Available() =         
      match state with
        | ServiceDisabled            
        | ServiceEnabledFailed        
        | ServiceEnabledUninitialised              -> false
        | ServiceEnabledOK (service,justification) -> true
                        
    member this.Status() =         
      match state with
        | ServiceDisabled                          -> "Disabled"           
        | ServiceEnabledFailed                     -> "Failed to start"
        | ServiceEnabledUninitialised              -> "Will auto enable on demand"
        | ServiceEnabledOK (service,justification) -> "Enabled\n" ^ justification
