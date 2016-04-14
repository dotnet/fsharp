// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Data.TypeProviders.Utility

module internal Util =
    open System
    open System.IO
    open System.Collections.Generic
    open System.Reflection

    type ProcessResult = { exitCode : int; stdout : string[] ; stderr : string[] }

    let executeProcess (workingDirectory,exe,cmdline) =
        try  
            // External tools (like edmgen.exe) run as part of default OS locale/codepage/LCID.  We need to ensure we translate their output
            // accordingly, by setting up the correct encoding on the stdout/stderr streams.
            let encodingToTranslateToolOutput = System.Text.Encoding.Default

            let psi = new System.Diagnostics.ProcessStartInfo(exe,cmdline) 
            psi.WorkingDirectory <- workingDirectory
            psi.UseShellExecute <- false
            psi.RedirectStandardOutput <- true
            psi.RedirectStandardError <- true
            psi.CreateNoWindow <- true        
            psi.StandardOutputEncoding <- encodingToTranslateToolOutput
            psi.StandardErrorEncoding <- encodingToTranslateToolOutput
            let p = System.Diagnostics.Process.Start(psi) 
            let output = ResizeArray()
            let error = ResizeArray()

            // nulls are skipped because they act as signal that redirected stream is closed and not as part of output data
            p.OutputDataReceived.Add(fun args -> if args.Data <> null then output.Add(args.Data))
            p.ErrorDataReceived.Add(fun args -> if args.Data <> null then error.Add(args.Data))
            p.BeginErrorReadLine()
            p.BeginOutputReadLine()
            p.WaitForExit()
            { exitCode = p.ExitCode; stdout = output.ToArray(); stderr = error.ToArray() }
        with 
        | :? System.IO.FileNotFoundException
        | :? System.ComponentModel.Win32Exception -> failwith (FSData.SR.requiredToolNotFound(exe))
        | _ -> reraise()

    let shell (workingDirectory,exe,cmdline,formatError) =
    //    printfn "execute: %s %s" exe cmdline
        let result = executeProcess(workingDirectory,exe,cmdline)
        if result.exitCode > 0 then 
            //failwithf "The command\n%s %s\n failed.\n\nError:%s\n\nOutput: %s\n" exe cmdline result.stderr result.stdout
            match formatError with 
            | None -> 
                let merge lines = String.concat Environment.NewLine lines
                failwith ((merge result.stderr)  + Environment.NewLine + (merge result.stdout))
            | Some f -> f result.stdout result.stderr
        else
    //        printfn "<--command-->\n%s %s\n<-- success --><-- stdout -->\n%s<-- stderr -->\n%s\n" exe cmdline result.stdout result.stderr
            ()

    let cscExe () = 
        let toolPath = 
            //if System.Runtime.InteropServices.RuntimeEnvironment.GetSystemVersion().StartsWith("v4.0") then
                System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory()
            //else
             //   Path.Combine(System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory(), @"..\v3.5")
        Path.Combine(toolPath, "csc.exe")


    let dataSvcUtilExe () = 
        let toolPath = 
            //if System.Runtime.InteropServices.RuntimeEnvironment.GetSystemVersion().StartsWith("v4.0") then
                System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory()
            //else
            //    Path.Combine(System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory(), @"..\v4.0.30319")
        Path.Combine(toolPath, "DataSvcUtil.exe")


    let edmGenExe () = 
        let toolPath = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory()
        Path.Combine(toolPath, "edmgen.exe")




    let sdkPath () =
    
        let tryResult (key: Microsoft.Win32.RegistryKey) = 
            match key with 
            | null -> None
            | _ -> 
                let installFolder = key.GetValue("InstallationFolder") :?> string
                if Directory.Exists installFolder then 
                    Some installFolder 
                else
                    None
        // Annoyingly, the F# 2.0 decoding of 'use key = ...' on a registry key doesn't use an null check on the
        // key before calling Dispose. THis is because the key type doesn't use the IDisposable pattern.
        let useKey keyName f =
                // Look for WinSDK reg keys under the 32bit view of the registry.
                let reg32view = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, Microsoft.Win32.RegistryView.Registry32)
                let key = reg32view.OpenSubKey keyName 
                try f key 
                finally 
                    match key with 
                    | null -> () 
                    | _ -> key.Dispose()
                    reg32view.Dispose()     // if reg32view were really null, we would not be here and the user would have more serious issues not being able to access HKLM

        let SDK_REGPATHS = [ @"Software\Microsoft\Microsoft SDKs\NETFXSDK\4.6\WinSDK-NetFx40Tools"
                             @"Software\Microsoft\Microsoft SDKs\Windows\v8.1A\WinSDK-NetFx40Tools"
                             @"Software\Microsoft\Microsoft SDKs\Windows\v8.0A\WinSDK-NetFx40Tools"
                             @"Software\Microsoft\Microsoft SDKs\Windows\v7.1\WinSDK-NetFx40Tools"
                             @"Software\Microsoft\Microsoft SDKs\Windows\v7.0A\WinSDK-NetFx40Tools" ]
    
        SDK_REGPATHS 
        |> Seq.tryPick (fun p -> useKey p tryResult) 
        |> function 
           | Some p -> p
           | _      -> raise <| System.NotSupportedException(FSData.SR.unsupportedFramework())

    let sdkUtil name = Path.Combine(sdkPath(), name)

    let svcUtilExe () = sdkUtil "svcutil.exe"
    let xsdExe () = sdkUtil "xsd.exe"

    type FileResource =
        abstract member Path : string
        inherit IDisposable

    let ExistingFile(fileName : string) =
        { new FileResource with 
              member this.Path = fileName
          interface IDisposable with
              member this.Dispose() = () }

    let TemporaryFile(extension : string) =
        let filename =
            let fs = Path.GetTempFileName()
            let fs' = Path.ChangeExtension(fs, extension)
            if fs <> fs' then
                File.Delete fs
            fs'
    
        { new FileResource with 
              member this.Path = filename
          interface IDisposable with
              member this.Dispose() = File.Delete(filename) }

    type DirectoryResource =
        abstract member Path : string
        inherit IDisposable

    let TemporaryDirectory() =
        let dirName =
            let fs = Path.GetTempFileName()
            File.Delete fs
            Directory.CreateDirectory fs |> ignore
            fs
    
        { new DirectoryResource with 
            member this.Path = dirName
          interface IDisposable with
                member this.Dispose() = 
                    for f in Directory.EnumerateFiles dirName do File.Delete f 
                    Directory.Delete dirName }

    let ExistingDirectory(dirName : string) =
        { new DirectoryResource with 
              member this.Path = dirName
          interface IDisposable with
              member this.Dispose() = () }

    type MemoizationTable<'T,'U when 'T : equality>(f) =
        let createdDB = new Dictionary<'T,'U>()
        member x.Contents = (createdDB :> IDictionary<_,_>)
        member x.Apply typeName =
            let found,result = createdDB.TryGetValue typeName
            if found then
                result 
            else
                let ty = f typeName
                createdDB.[typeName] <- ty
                ty 

    open System.Threading

    let WithExclusiveAccessToFile (fileName : string) f = 
        // file name is not specified - run function directly
        if String.IsNullOrWhiteSpace fileName then f()
        else
        // convert filename to the uniform representation: full path + no backslashes + upper case

        // MSDN:  In Silverlight for Windows Phone, private object namespaces are not supported. 
        // In the .NET Framework, object namespaces are supported and because of this the backslash (\) is considered a delimiter and is not supported in the name parameter. 
        // In Silverlight for Windows Phone you can use a backslash (\) in the name parameter.
        let fileName = Path.GetFullPath(fileName)
        let fileName = fileName.Replace('\\', '_')
        let fileName = fileName.ToUpper()
        
        // MSDN: On a server that is running Terminal Services, a named system mutex can have two levels of visibility. 
        // If its name begins with the prefix "Global\", the mutex is visible in all terminal server sessions.
        // Since we use mutex to protect access to file - use Global prefix to cover all terminal sessions
        let fileName = @"Global\" + fileName
        use mutex = 
            let mutable createdNew = false
            let mutex = new Mutex(initiallyOwned = true, name = fileName, createdNew = &createdNew)
            // createdNew = true - we created and immediately acquired mutex - can continue
            // createdNew = false - mutex already exists and we do not own it - must wait until it is released
            if createdNew then mutex
            else
            try
                mutex.WaitOne() |> ignore
                mutex
            with
                | :? AbandonedMutexException -> 
                    // if thread that owns mutex was terminated abnormally - then WaitOne will raise AbandonedMutexException
                    // MSDN: The next thread to request ownership of the mutex can handle this exception and proceed, 
                    // provided that the integrity of the data structures can be verified.
                    // Current thread still owns mutex so we can return it
                    mutex
                | _ -> 
                    mutex.Dispose()
                    reraise()
        try f()
        finally mutex.ReleaseMutex()

