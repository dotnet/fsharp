// Knownfail.lst rewriter.
// Takes a Knownfail.txt file and rewrites it according to the current configuration
// Exercise caution when making changes here because you may end up escluding tests without noticing

// NOTE: Execute as a 32bit process

#nowarn "9"

module OsInfo =

    open System.Runtime.InteropServices

    [<StructLayout(LayoutKind.Sequential)>]
    type SYSTEM_INFO = 
        struct
            val wProcessorArchitecture : uint16
            val wReserved : uint16
            val dwPageSize : uint32
            val lpMinimumApplicationAddress : System.IntPtr
            val lpMaximumApplicationAddress : System.IntPtr
            val dwActiveProcessorMask : System.UIntPtr
            val dwNumberOfProcessors : uint32 
            val dwProcessorType : uint32 
            val dwAllocationGranularity : uint32 
            val wProcessorLevel : uint16
            val wProcessorRevision : uint16
        end

    [<DllImport("kernel32.dll")>]
    extern void GetNativeSystemInfo(SYSTEM_INFO& lpSystemInfo);

    let Is64bitOS() =
        try
            let mutable si = new SYSTEM_INFO()
            GetNativeSystemInfo(&si)
            if (si.wProcessorArchitecture = 9us || si.wProcessorArchitecture = 6us) then true else false
        with
        | _ -> false

    let Is32bitOS() =
        try
            let mutable si = new SYSTEM_INFO()
            GetNativeSystemInfo(&si)
            if si.wProcessorArchitecture = 0us then true else false
        with
        | _ -> false

module NetFxInfo =
    let IsNetFx20Installed () = 
        let o = Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\NET Framework Setup\NDP\v2.0.50727", "Install", 0)
        if o = null then false else (o :?> int) = 1
        
    let IsNetFx30Installed () = 
        let o = Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\NET Framework Setup\NDP\v3.0", "Install", 0)
        if o = null then false else (o :?> int) = 1

    let IsNetFx35Installed () = 
        let o = Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\NET Framework Setup\NDP\v3.5", "Install", 0)
        if o = null then false else (o :?> int) = 1

    let IsNetFx40ClientInstalled () = 
        let o = Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Client", "Install", 0)
        if o = null then false else (o :?> int) = 1

    let IsNetFx40FullInstalled () = 
        let o = Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full", "Install", 0)
        if o = null then false else (o :?> int) = 1

    let IsNetFx40Installed () = IsNetFx40ClientInstalled() || IsNetFx40FullInstalled()

module VSInfo =
    let IsFSharp2010Installed () = 
        let o = Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\VisualStudio\10.0\Setup\F#", "ProductDir", null)
        o <> null

    let IsFSharpDev11Installed () = 
        let o = Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\VisualStudio\11.0\Setup\F#", "ProductDir", null)
        o <> null
        
    let IsFSharpDev12Installed () = 
        let o = Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\VisualStudio\14.0\Setup\F#", "ProductDir", null)
        o <> null

    let IsFSharp2008Installed () = 
        let o = Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\VisualStudio\9.0\InstalledProducts\Microsoft Visual F#", "Package", null)
        o <> null

    let IsFSharpRuntime20Installed () = 
        let o = Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\.NetFramework\v2.0.50727\AssemblyFoldersEx\F# 3.0 Core Assemblies", null, null)
        o <> null

    let IsFSharpRuntime40Installed () = 
        let o = Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\.NetFramework\v4.0.30319\AssemblyFoldersEx\F# 3.0 Core Assemblies", null, null)
        o <> null

module FSharpInfo =
    let IsPowerPackInstalled () = 
        let o = Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\.NETFramework\AssemblyFolders\FSharp.PowerPack-2.0.0.0", "Description", null)
        (o <> null)

    // Are we running a CHK build?
    let IsFSharpCompilerDebug () = 
        let o = Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\FSharp\4.1\Runtime\v4.0", null, null)
        if o <> null then
            let path = System.IO.Path.Combine( o :?> string, "FSharp.Compiler.Service.dll")
            let asm = System.Reflection.Assembly.LoadFrom(path)

            match asm.GetCustomAttributes(typeof<System.Diagnostics.DebuggableAttribute>, false) with
            | null | [||] -> false
            | [|:? System.Diagnostics.DebuggableAttribute as a|] -> 
                (a.DebuggingFlags &&& System.Diagnostics.DebuggableAttribute.DebuggingModes.DisableOptimizations) = System.Diagnostics.DebuggableAttribute.DebuggingModes.DisableOptimizations
            | _ -> false
        else
            false   // very weird...

module AppInfo =
    let IsOfficeInstalled () =
        let x86key = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Office\"
        let x64key = @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Office\"
        let o12 = if OsInfo.Is64bitOS() then Microsoft.Win32.Registry.GetValue(x64key + "12.0", "Description", null) else Microsoft.Win32.Registry.GetValue(x86key + "12.0", "Description", null)
        let o14 = if OsInfo.Is64bitOS() then Microsoft.Win32.Registry.GetValue(x64key + "14.0", "Description", null) else Microsoft.Win32.Registry.GetValue(x86key + "14.0", "Description", null)
        (o12 <> null) || (o14 <> null)

type Tags = 
    | DEV10
    | DEV11
    | DEV12
    | VS2008
    | NETFX20
    | NETFX30
    | NETFX35
    | NETFX40
    | FSHARPRUNTIME20
    | FSHARPRUNTIME40
    | OS32BIT
    | OS64BIT
    | CHK
    | RET
    | POWERPACK
    | MT402040
    | MT402020
    | OPTIMIZEPLUS
    | STANDALONE
    | INDIRECTCALLARRAYMETHODS
    | OFFICE
    | CURRENTUICULTURE1033
    | UNKNOWN of string
    override x.ToString() =
        match x with
        | DEV10 -> "DEV10"
        | DEV11 -> "DEV11"
        | DEV12 -> "DEV12"
        | VS2008 -> "VS2008"
        | NETFX20 -> "NETFX20"
        | NETFX30 -> "NETFX30"
        | NETFX35 -> "NETFX35"
        | NETFX40 -> "NETFX40"
        | FSHARPRUNTIME20 -> "FSHARPRUNTIME20"
        | FSHARPRUNTIME40 -> "FSHARPRUNTIME40"
        | OS32BIT -> "OS32BIT"
        | OS64BIT -> "OS64BIT"
        | CHK -> "CHK"
        | RET -> "RET"
        | POWERPACK -> "POWERPACK"
        | MT402040 -> "MT402040"
        | MT402020 -> "MT402020"
        | OPTIMIZEPLUS -> "OPTIMIZEPLUS"
        | STANDALONE -> "STANDALONE"
        | INDIRECTCALLARRAYMETHODS -> "INDIRECTCALLARRAYMETHODS"
        | OFFICE -> "OFFICE"
        | CURRENTUICULTURE1033 -> "CURRENTUICULTURE1033"
        | UNKNOWN(x) -> x
    static member AllTags() = [ DEV10; DEV11; DEV12; VS2008; NETFX20; NETFX30; NETFX35; NETFX40; FSHARPRUNTIME20; FSHARPRUNTIME40; OS32BIT; OS64BIT; CHK; RET; POWERPACK; MT402040; MT402020; OPTIMIZEPLUS; STANDALONE; INDIRECTCALLARRAYMETHODS; OFFICE; CURRENTUICULTURE1033 ]
    static member ofString(x:string) = 
        let found = Tags.AllTags() |> List.tryFind (fun t -> t.ToString() = x)
        if found = None then UNKNOWN(x) else found.Value

    member x.Check() =
        match x with
        | DEV10 -> Some(VSInfo.IsFSharp2010Installed())
        | DEV11 -> Some(VSInfo.IsFSharpDev11Installed())
        | DEV12 -> Some(VSInfo.IsFSharpDev12Installed())
        | VS2008 -> Some(VSInfo.IsFSharp2008Installed())
        | NETFX20 -> Some(NetFxInfo.IsNetFx20Installed())
        | NETFX30 -> Some(NetFxInfo.IsNetFx30Installed())
        | NETFX35 -> Some(NetFxInfo.IsNetFx35Installed())
        | NETFX40 -> Some(NetFxInfo.IsNetFx40Installed())
        | FSHARPRUNTIME20 -> Some(VSInfo.IsFSharpRuntime20Installed())
        | FSHARPRUNTIME40 -> Some(VSInfo.IsFSharpRuntime40Installed())
        | OS32BIT -> Some(OsInfo.Is32bitOS())
        | OS64BIT -> Some(OsInfo.Is64bitOS())
        | CHK -> Some(FSharpInfo.IsFSharpCompilerDebug())
        | RET -> Some(not(FSharpInfo.IsFSharpCompilerDebug()))
        | POWERPACK -> Some(FSharpInfo.IsPowerPackInstalled())
        | MT402040 -> Some(System.Environment.GetEnvironmentVariable("MT402040") <> null)
        | MT402020 -> Some(System.Environment.GetEnvironmentVariable("MT402020") <> null)
        | OPTIMIZEPLUS -> Some(System.String.IsNullOrEmpty(System.Environment.GetEnvironmentVariable("ISCFLAGS")) ||
                               System.Environment.GetEnvironmentVariable("ISCFLAGS").Contains("--optimize+") <> false )
        | STANDALONE -> Some(System.String.IsNullOrEmpty(System.Environment.GetEnvironmentVariable("ISCFLAGS")) ||
                             System.Environment.GetEnvironmentVariable("ISCFLAGS").Contains("--standalone") <> false)
        | INDIRECTCALLARRAYMETHODS -> Some(System.String.IsNullOrEmpty(System.Environment.GetEnvironmentVariable("ISCFLAGS")) ||
                                           System.Environment.GetEnvironmentVariable("ISCFLAGS").Contains("--indirectCallArrayMethods") <> false)
        | OFFICE -> Some(AppInfo.IsOfficeInstalled())
        | CURRENTUICULTURE1033 -> Some(System.Globalization.CultureInfo.CurrentUICulture.LCID = 1033)

        | UNKNOWN(_) -> None
        
                                       
let w (s:string) = 
    s.Split([|','|])
    |> Array.filter (fun s -> s.[0] <> '!')
    |> Array.map (fun s -> Tags.ofString(s))

let w' (s:string) = 
    s.Split([|','|])
    |> Array.filter (fun s -> s.[0] = '!')
    |> Array.map (fun s -> Tags.ofString(s.Remove(0,1)))

// Read knownfail.txt in memory
let File2List (filename:string) = 
    use s = new System.IO.StreamReader(filename)
    let mutable l = []
    while not s.EndOfStream do
        let line = s.ReadLine()
        let isblank_or_comment = System.Text.RegularExpressions.Regex.IsMatch(line, @"^[ \t]*#") || System.Text.RegularExpressions.Regex.IsMatch(line, @"^[ \t]*$")
        if not isblank_or_comment then 
            let tt = line.Split([|'\t'|], System.StringSplitOptions.RemoveEmptyEntries)
            if tt.Length = 1 then
                l <- List.append l ( ([||],[||],tt.[0]) :: [])
            else
                
                l <- List.append l ( (w(tt.[0]),
                                      w'(tt.[0]), 
                                      tt |> Seq.skip 1 |> Seq.fold (fun x e -> e + x) "" ) :: [])
    l

let f(x : Tags[], y : Tags[], z : string) = 
    match (x,y) with
    | [||],[||] -> printfn "%s" z
    | _,_ -> 
                let o =  x |> Array.map (fun e -> e.Check()) |> Array.fold (fun s e -> (if e.IsNone then false else e.Value) && s) true
                let o' = y |> Array.map (fun e -> e.Check()) |> Array.fold (fun s e -> (if e.IsNone then true else e.Value) || s) false

                // Emit explanation why we are including/excluding this test
                printf "# "
                Seq.append x y
                |> Seq.iter (fun s -> let c = s.Check()
                                      printf "%s=%s;" (s.ToString()) ( if c.IsNone then "?" else c.Value.ToString() ))
                printfn ""
                
                if o && not o' then
                    printfn "%s" z
                else
                    printfn "# %s" z    

#if INTERACTIVE    
File2List fsi.CommandLineArgs.[1]       // typically "Knownfail.txt"
#else
File2List "Knowfail.lst"                // for testing purposes
#endif
|> Seq.iter f
