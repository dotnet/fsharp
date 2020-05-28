// The compiler should add an embedded default win32 manifest so that UAC is handled properly
//<Expects status="success"></Expects>

open System
open System.Text
open System.Reflection
open System.Runtime.InteropServices
open Microsoft.FSharp.NativeInterop
open FSharp.Test.DefaultManifest

module NativeMethods = 
    type EnumResourceNamesCallback = delegate of IntPtr * IntPtr * IntPtr * IntPtr -> bool

    [<DllImport("kernel32.dll")>]
    extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hReservedNull, uint32 dwFlags)

    [<DllImport("kernel32.dll")>]
    extern bool EnumResourceNames(IntPtr hModule, int dwID, EnumResourceNamesCallback lpEnumFunc, IntPtr lParam)

    [<DllImport("kernel32.dll")>]
    extern IntPtr FindResource(IntPtr hModule, IntPtr lpName, IntPtr lpType)

    [<DllImport("kernel32.dll")>]
    extern IntPtr LoadResource(IntPtr hModule, IntPtr hResInfo)

    [<DllImport("kernel32.dll")>]
    extern uint32 SizeofResource(IntPtr hModule, IntPtr hResInfo)

    [<DllImport("kernel32.dll")>]
    extern IntPtr LockResource(IntPtr hResData)

    // ID of the manifest win32 resource type
    let RT_MANIFEST = 24
    
    // dwFlags to indicate that we are only loading binaries to read data, not to execute
    let LOAD_LIBRARY_AS_DATAFILE = 2u

/// extracts the win32 manifest of the specified assembly
let getManifest (path:string) =
    let manifest : string option ref = ref None
    let callback =  
        NativeMethods.EnumResourceNamesCallback(fun (hModule:IntPtr) (lpszType:IntPtr) (lpszName:IntPtr) (lParam:IntPtr) ->
            let resourceAsString =
                let hResInfo = NativeMethods.FindResource(hModule, lpszName, lpszType)
                let hResData = NativeMethods.LoadResource(hModule, hResInfo)
                let resSize = NativeMethods.SizeofResource(hModule, hResInfo) |> int
                let hResMem = NativeMethods.LockResource(hResData)
        
                let buff = Array.zeroCreate<byte> resSize        
                Marshal.Copy(hResMem, buff, 0, resSize)
                String(buff |> Array.map char)

            manifest := Some(resourceAsString)
            true
        )

    let hLib = NativeMethods.LoadLibraryEx(path, IntPtr.Zero, NativeMethods.LOAD_LIBRARY_AS_DATAFILE)
    if hLib = IntPtr.Zero then
        printfn "Error loading library %s" path
        exit 1
    else
        NativeMethods.EnumResourceNames(hLib, NativeMethods.RT_MANIFEST, callback, IntPtr.Zero) |> ignore
    !manifest

let exePath = System.Reflection.Assembly.GetExecutingAssembly().Location
let dllPath = (TestType()).GetType().Assembly.Location

let manifests = [exePath; dllPath] |> List.map getManifest

match manifests with
| [Some(m); None] ->
    printfn "Found exe manifest and no DLL manifest. This is expected."
    printfn "Exe manifest content: %s" m
    if m.Contains("requestedExecutionLevel") then exit 0
    else
        printfn "Exe manifest does not contain expected content"
        exit 1
| [exeM; dllM] ->
    printfn "Unexpected manifest result."
    printfn "EXE manifest: %A" exeM
    printfn "DLL manifest: %A" dllM
    exit 1
| _ -> exit 1 // should never get here