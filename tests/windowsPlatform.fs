[<RequireQualifiedAccess>]
module WindowsPlatform

open PlatformHelpers

// REM == Find out path to native 'Program Files 32bit', no matter what
// REM == architecture we are running on and no matter what command
// REM == prompt we came from.
// IF /I "%OSARCH%"=="x86"   set X86_PROGRAMFILES=%ProgramFiles%
// IF /I "%OSARCH%"=="IA64"  set X86_PROGRAMFILES=%ProgramFiles(x86)%
// IF /I "%OSARCH%"=="AMD64" set X86_PROGRAMFILES=%ProgramFiles(x86)%
let x86ProgramFilesDirectory envVars osArch = 
    match osArch with
    | X86 -> envVars |> Map.find "ProgramFiles"
    | IA64 -> envVars |> Map.find "ProgramFiles(x86)"
    | AMD64 -> envVars |> Map.find "ProgramFiles(x86)"
    | Unknown os -> failwithf "OSARCH '%s' not supported" os

let private parseProcessorArchitecture (s : string) = 
    match s.ToUpper() with
    | "X86" -> X86
    | "IA64" -> IA64
    | "AMD64" -> AMD64
    | arc -> Unknown s

/// <summary>
/// Return current process architecture, using PROCESSOR_ARCHITECTURE environment variable
/// </summary>
let processorArchitecture envVars =
    match envVars |> Map.tryFind "PROCESSOR_ARCHITECTURE" |> Option.map parseProcessorArchitecture with
    | Some x -> x 
    | None -> failwithf "environment variable '%s' required " "PROCESSOR_ARCHITECTURE"

///<summary>
/// Return real processor architecture (ignore WOW64)
/// more info: http://blogs.msdn.com/b/david.wang/archive/2006/03/26/howto-detect-process-bitness.aspx
/// use PROCESSOR_ARCHITEW6432 and PROCESSOR_ARCHITECTURE environment variables
///</summary>
let osArch envVars =
    // SET OSARCH=%PROCESSOR_ARCHITECTURE%
    // IF NOT "%PROCESSOR_ARCHITEW6432%"=="" SET OSARCH=%PROCESSOR_ARCHITEW6432%
    match envVars |> Map.tryFind "PROCESSOR_ARCHITEW6432" |> Option.map parseProcessorArchitecture with
    | Some arc -> arc
    | None -> processorArchitecture envVars


//  %~i	    -   expands %i removing any surrounding quotes (")
//  %~fi	-   expands %i to a fully qualified path name
//  %~di	-   expands %i to a drive letter only
//  %~pi	-   expands %i to a path only
//  %~ni	-   expands %i to a file name only
//  %~xi	-   expands %i to a file extension only
//  %~si	-   expanded path contains short names only

open Microsoft.Win32

let regQuery path value (baseKey: RegistryKey) =
    use regKey  = baseKey.OpenSubKey(path, false)
   
    if (regKey = null) then None
    else 
        match regKey.GetValue(value) with
        | null -> None
        | x -> Some x

open System.Text.RegularExpressions
open FSharpTestSuiteTypes

let visualStudioVersion () =

    let hklm32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32)
    let subkey = 
        match hklm32.OpenSubKey(@"Software\Microsoft\VisualStudio\15.0\Setup") with 
        | null -> 
            match hklm32.OpenSubKey(@"Software\Microsoft\VisualStudio\14.0\Setup")  with
            | null -> None
            | t -> Some t
        | t -> Some t

    let keys = 
        match subkey with 
        | None -> [| |]
        | Some t -> t.GetSubKeyNames()

    let findstr r = keys |> Array.exists (fun t -> Regex.IsMatch(t, r)) 

    // reg query "%REG_SOFTWARE%\Microsoft\VisualStudio\15.0\Setup" | findstr /r /c:"Express .* for Windows Desktop" > NUL
    // if NOT ERRORLEVEL 1 (
    //     set INSTALL_SKU=DESKTOP_EXPRESS
    //     goto :done_SKU
    // )
    if findstr "Express .* for Windows Desktop" then 
        Some INSTALL_SKU.DesktopExpress
    
    // reg query "%REG_SOFTWARE%\Microsoft\VisualStudio\15.0\Setup" | findstr /r /c:"Express .* for Web" > NUL
    // if NOT ERRORLEVEL 1 (
    //     set INSTALL_SKU=WEB_EXPRESS
    //     goto :done_SKU
    // )
    elif findstr "Express .* for Web" then 
        Some INSTALL_SKU.WebExpress

    // reg query "%REG_SOFTWARE%\Microsoft\VisualStudio\15.0\Setup" | findstr /r /c:"Ultimate" > NUL
    // if NOT ERRORLEVEL 1 (
    //     set INSTALL_SKU=ULTIMATE
    //     goto :done_SKU
    // )
    elif findstr "Ultimate" then 
        Some INSTALL_SKU.Ultimate
    
    // set INSTALL_SKU=CLEAN
    // :done_SKU
    else Some INSTALL_SKU.Clean
