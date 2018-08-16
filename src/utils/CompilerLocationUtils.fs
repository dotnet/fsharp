// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Internal.Utilities
open System
open System.IO
open System.Reflection
open Microsoft.Win32
open System.Runtime.InteropServices

#nowarn "44" // ConfigurationSettings is obsolete but the new stuff is horribly complicated. 

module internal FSharpEnvironment =

    /// The F# version reported in the banner
    let FSharpBannerVersion = "10.2.3 for F# 4.5"

    let versionOf<'t> =
#if FX_RESHAPED_REFLECTION
        let aq = (typeof<'t>).AssemblyQualifiedName
        let version = 
            if aq <> null then 
                let x = aq.Split(',', ' ') |> Seq.filter(fun x -> x.StartsWith("Version=", StringComparison.OrdinalIgnoreCase)) |> Seq.tryHead
                match x with 
                | Some(x) -> x.Substring(8)
                | _ -> null
            else
                null
        version
#else
        typeof<'t>.Assembly.GetName().Version.ToString()
#endif

    let FSharpCoreLibRunningVersion = 
        try match versionOf<Unit> with
            | null -> None
            | "" -> None
            | s  -> Some(s)
        with _ -> None


    // The F# team version number. This version number is used for
    //     - the F# version number reported by the fsc.exe and fsi.exe banners in the CTP release
    //     - the F# version number printed in the HTML documentation generator
    //     - the .NET DLL version number for all VS2008 DLLs
    //     - the VS2008 registry key, written by the VS2008 installer
    //         HKEY_LOCAL_MACHINE\Software\Microsoft\.NETFramework\AssemblyFolders\Microsoft.FSharp-" + FSharpTeamVersionNumber
    // Also
    //     - for Beta2, the language revision number indicated on the F# language spec
    //
    // It is NOT the version number listed on FSharp.Core.dll
    let FSharpTeamVersionNumber = "2.0.0.0"

    // The F# binary format revision number. The first three digits of this form the significant part of the 
    // format revision number for F# binary signature and optimization metadata. The last digit is not significant.
    //
    // WARNING: Do not change this revision number unless you absolutely know what you're doing.
    let FSharpBinaryMetadataFormatRevision = "2.0.0.0"

#if FX_NO_WIN_REGISTRY
#else
    [<DllImport("Advapi32.dll", CharSet = CharSet.Unicode, BestFitMapping = false)>]
    extern uint32 RegOpenKeyExW(UIntPtr _hKey, string _lpSubKey, uint32 _ulOptions, int _samDesired, UIntPtr & _phkResult);

    [<DllImport("Advapi32.dll", CharSet = CharSet.Unicode, BestFitMapping = false)>]
    extern uint32 RegQueryValueExW(UIntPtr _hKey, string _lpValueName, uint32 _lpReserved, uint32 & _lpType, IntPtr _lpData, int & _lpchData);

    [<DllImport("Advapi32.dll")>]
    extern uint32 RegCloseKey(UIntPtr _hKey)
#endif
    module Option = 
        /// Convert string into Option string where null and String.Empty result in None
        let ofString s = 
            if String.IsNullOrEmpty(s) then None
            else Some(s)

    // MaxPath accounts for the null-terminating character, for example, the maximum path on the D drive is "D:\<256 chars>\0". 
    // See: ndp\clr\src\BCL\System\IO\Path.cs
    let maxPath = 260;
    let maxDataLength = (new System.Text.UTF32Encoding()).GetMaxByteCount(maxPath)
#if FX_NO_WIN_REGISTRY
#else
    let KEY_WOW64_DEFAULT = 0x0000
    let KEY_WOW64_32KEY = 0x0200
    let HKEY_LOCAL_MACHINE = UIntPtr(0x80000002u)
    let KEY_QUERY_VALUE = 0x1
    let REG_SZ = 1u

    let GetDefaultRegistryStringValueViaDotNet(subKey: string)  =
        Option.ofString
            (try
                downcast Microsoft.Win32.Registry.GetValue("HKEY_LOCAL_MACHINE\\"+subKey,null,null)
             with e->
                System.Diagnostics.Debug.Assert(false, sprintf "Failed in GetDefaultRegistryStringValueViaDotNet: %s" (e.ToString()))
                null)


    let Get32BitRegistryStringValueViaPInvoke(subKey:string) = 
        Option.ofString
            (try 
                // 64 bit flag is not available <= Win2k
                let options = 
                    let hasWow6432Node =
                        use x = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node")
                        x <> null
                    try
                        match hasWow6432Node with
                        | true  -> KEY_WOW64_32KEY
                        | false -> KEY_WOW64_DEFAULT
                    with
                    | _ -> KEY_WOW64_DEFAULT

                let mutable hkey = UIntPtr.Zero;
                let pathResult = Marshal.AllocCoTaskMem(maxDataLength);

                try
                    let res = RegOpenKeyExW(HKEY_LOCAL_MACHINE,subKey, 0u, KEY_QUERY_VALUE ||| options, & hkey)
                    if res = 0u then
                        let mutable uType = REG_SZ;
                        let mutable cbData = maxDataLength;

                        let res = RegQueryValueExW(hkey, null, 0u, &uType, pathResult, &cbData);

                        if (res = 0u && cbData > 0 && cbData <= maxDataLength) then
                            Marshal.PtrToStringUni(pathResult, (cbData - 2)/2);
                        else 
                            null
                    else
                        null
                finally
                    if hkey <> UIntPtr.Zero then
                        RegCloseKey(hkey) |> ignore
                
                    if pathResult <> IntPtr.Zero then
                        Marshal.FreeCoTaskMem(pathResult)
             with e->
                System.Diagnostics.Debug.Assert(false, sprintf "Failed in Get32BitRegistryStringValueViaPInvoke: %s" (e.ToString()))
                null)

    let is32Bit = IntPtr.Size = 4
    
    let runningOnMono = try System.Type.GetType("Mono.Runtime") <> null with e-> false

    let tryRegKey(subKey:string) = 

        //if we are runing on mono simply return None
        // GetDefaultRegistryStringValueViaDotNet will result in an access denied by default, 
        // and Get32BitRegistryStringValueViaPInvoke will fail due to Advapi32.dll not existing
        if runningOnMono then None else
        if is32Bit then
            let s = GetDefaultRegistryStringValueViaDotNet(subKey)
            // If we got here AND we're on a 32-bit OS then we can validate that Get32BitRegistryStringValueViaPInvoke(...) works
            // by comparing against the result from GetDefaultRegistryStringValueViaDotNet(...)
#if DEBUG
            let viaPinvoke = Get32BitRegistryStringValueViaPInvoke(subKey)
            System.Diagnostics.Debug.Assert((s = viaPinvoke), sprintf "32bit path: pi=%A def=%A" viaPinvoke s)
#endif
            s
        else
            Get32BitRegistryStringValueViaPInvoke(subKey) 
#endif

    let internal tryCurrentDomain() =
        let pathFromCurrentDomain = 
#if FX_NO_APP_DOMAINS
            System.AppContext.BaseDirectory
#else
            System.AppDomain.CurrentDomain.BaseDirectory
#endif
        if not(String.IsNullOrEmpty(pathFromCurrentDomain)) then 
            Some pathFromCurrentDomain
        else
            None

#if FX_NO_SYSTEM_CONFIGURATION
    let internal tryAppConfig (_appConfigKey:string) = None
#else
    let internal tryAppConfig (_appConfigKey:string) = 
        let locationFromAppConfig = System.Configuration.ConfigurationSettings.AppSettings.[_appConfigKey]
        System.Diagnostics.Debug.Print(sprintf "Considering _appConfigKey %s which has value '%s'" _appConfigKey locationFromAppConfig) 

        if String.IsNullOrEmpty(locationFromAppConfig) then 
            None
        else
            let exeAssemblyFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
            let locationFromAppConfig = locationFromAppConfig.Replace("{exepath}", exeAssemblyFolder)
            System.Diagnostics.Debug.Print(sprintf "Using path %s" locationFromAppConfig) 
            Some locationFromAppConfig
#endif

    // The default location of FSharp.Core.dll and fsc.exe based on the version of fsc.exe that is running
    // Used for
    //     - location of design-time copies of FSharp.Core.dll and FSharp.Compiler.Interactive.Settings.dll for the default assumed environment for scripts
    //     - default ToolPath in tasks in FSharp.Build.dll (for Fsc tasks, but note a probe location is given)
    //     - default F# binaries directory in service.fs (REVIEW: check this)
    //     - default location of fsi.exe in FSharp.VS.FSI.dll (REVIEW: check this)
    //     - default F# binaries directory in (project system) Project.fs
    let BinFolderOfDefaultFSharpCompiler(probePoint:string option) = 
#if FX_NO_WIN_REGISTRY
        ignore probePoint
#if FX_NO_APP_DOMAINS
        Some System.AppContext.BaseDirectory
#else
        Some System.AppDomain.CurrentDomain.BaseDirectory
#endif
#else
        // Check for an app.config setting to redirect the default compiler location
        // Like fsharp-compiler-location
        try 
            // FSharp.Compiler support setting an appkey for compiler location. I've never seen this used.
            let result = tryAppConfig "fsharp-compiler-location"
            match result with 
            | Some _ ->  result 
            | None ->

            let safeExists f = (try File.Exists(f) with _ -> false)
            // Look in the probePoint if given, e.g. look for a compiler alongside of FSharp.Build.dll
            match probePoint with 
            | Some p when safeExists (Path.Combine(p,"FSharp.Core.dll")) -> Some p 
            | _ -> 
            // We let you set FSHARP_COMPILER_BIN. I've rarely seen this used and its not documented in the install instructions.
            let result = System.Environment.GetEnvironmentVariable("FSHARP_COMPILER_BIN")
            if not (String.IsNullOrEmpty(result)) then
                Some result
            else
                // For the prototype compiler, we can just use the current domain
                tryCurrentDomain()
        with e -> 
            System.Diagnostics.Debug.Assert(false, "Error while determining default location of F# compiler")
            None


    // Apply the given function to the registry entry corresponding to the subkey.
    // The reg key is disposed at the end of the scope.
    let useKey subkey f =
        let key = Registry.LocalMachine.OpenSubKey subkey
        try f key 
        finally 
            match key with 
            | null -> () 
            | _ -> key.Dispose()

    // Check if the framework version 4.5 or above is installed at the given key entry 
    let IsNetFx45OrAboveInstalledAt subkey =
      try
        useKey subkey (fun regkey ->
            match regkey with
            | null -> false
            | _ -> regkey.GetValue("Release", 0) :?> int |> (fun s -> s >= 0x50000)) // 0x50000 implies 4.5.0
      with _ -> false
 
    // Check if the framework version 4.5 or above is installed
    let IsNetFx45OrAboveInstalled =
        IsNetFx45OrAboveInstalledAt @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Client" ||
        IsNetFx45OrAboveInstalledAt @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full" ||
        runningOnMono

    // Check if the running framework version is 4.5 or above.
    // Use the presence of v4.5.x in the registry to distinguish between 4.0 and 4.5
    let IsRunningOnNetFx45OrAbove =
            let version = new Version(versionOf<System.Int32>) 
            let major = version.Major
            major > 4 || (major = 4 && IsNetFx45OrAboveInstalled)

#endif
