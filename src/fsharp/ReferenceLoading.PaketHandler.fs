// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

/// Coordinating compiler operations - configuration, loading initial context, reporting errors etc.
module internal Microsoft.FSharp.Compiler.ReferenceLoading.PaketHandler

type ReferenceLoadingResult =
| Solved of loadingScript: string
| PackageManagerNotFound of implicitIncludeDir: string * userProfile: string
| PackageResolutionFailed of toolPath: string * workingDir: string * msg : string

module Internals =
    open System
    open System.IO
    let PM_EXE = "paket.exe"
    let PM_DIR = ".paket"
    let PM_SPEC_FILE = "paket.dependencies"
    let PM_LOCK_FILE = "paket.lock"
    let PM_COMMAND = "install --generate-load-scripts"
    let PM_LOADSCRIPT = PM_DIR + "/load/main.group.fsx"

    let userProfile = 
        let res = Environment.GetEnvironmentVariable("USERPROFILE")
        if System.String.IsNullOrEmpty res then
            Environment.GetEnvironmentVariable("HOME")
        else res

    let getDiretoryAndAllParentDirectories (directory: DirectoryInfo) =
        let rec allParents (directory: DirectoryInfo) =
            seq {
                match directory.Parent with
                | null -> ()
                | parent -> 
                    yield parent
                    yield! allParents parent
            }

        seq {
            yield directory
            yield! allParents directory
        }

    /// walks up directory structure and tries to find .paket/paket.exe
    let findPaketExe (baseDir: string) =

        let getPaketAndExe (directory: DirectoryInfo) =
            let dir = directory.GetDirectories(PM_DIR)
            match dir with
            | [|dir|] -> 
              match dir.GetFiles(PM_EXE) with
              | [|exe|] -> Some exe.FullName
              | _ -> None
            | _ -> None

        let dir = DirectoryInfo baseDir
        let allDirs = getDiretoryAndAllParentDirectories dir
        
        allDirs
        |> Seq.choose getPaketAndExe
        |> Seq.tryHead

    let ResolvePackages alterToolPath (implicitIncludeDir: string, scriptName: string, packageManagerTextLines: string list) =
        printfn "OBJ %A" (implicitIncludeDir,scriptName)
        let workingDir = Path.Combine(Path.GetTempPath(),"fsx-packages", string(abs(hash (implicitIncludeDir,scriptName))))
        let workingDirSpecFile = FileInfo(Path.Combine(workingDir,PM_SPEC_FILE))
        if not (Directory.Exists workingDir) then
            Directory.CreateDirectory workingDir |> ignore

        let packageManagerTextLines = packageManagerTextLines |> List.filter (fun l -> not (String.IsNullOrWhiteSpace l))

        let rootDir,packageManagerTextLines =
            let rec findSpecFile dir =
                let fi = FileInfo(Path.Combine(dir,PM_SPEC_FILE))
                if fi.Exists then
                    let lockFile = FileInfo(Path.Combine(fi.Directory.FullName,PM_LOCK_FILE))
                    let depsFileLines = File.ReadAllLines fi.FullName
                    if lockFile.Exists then
                        let originalDepsFile = FileInfo(workingDirSpecFile.FullName + ".original")
                        if not originalDepsFile.Exists ||
                           File.ReadAllLines originalDepsFile.FullName <> depsFileLines
                        then
                            File.Copy(fi.FullName,originalDepsFile.FullName,true)
                            let targetLockFile = FileInfo(Path.Combine(workingDir,PM_LOCK_FILE))
                            File.Copy(lockFile.FullName,targetLockFile.FullName,true)
                    
                    let lines = 
                        if List.isEmpty packageManagerTextLines then 
                            Array.toList depsFileLines
                        else
                            (Array.toList depsFileLines) @ ("group Main" :: packageManagerTextLines)

                    fi.Directory.FullName, lines
                elif fi.Directory.Parent <> null then
                    findSpecFile fi.Directory.Parent.FullName
                else
                    workingDir, "framework: net461" :: "source https://nuget.org/api/v2" :: packageManagerTextLines
           
            findSpecFile implicitIncludeDir

        let toolPathOpt = 
            // we try to resolve .paket/paket.exe any place up in the folder structure from current script
            match findPaketExe implicitIncludeDir with
            | Some paketExe -> Some paketExe
            | None ->
                match findPaketExe rootDir with
                | Some paketExe -> Some paketExe
                | None ->
                  let profileExe = Path.Combine (userProfile, PM_DIR, PM_EXE)
                  if File.Exists profileExe then Some profileExe
                  else None

        match toolPathOpt with 
        | None -> 
            PackageManagerNotFound(implicitIncludeDir, userProfile)

        | Some toolPath ->
            let loadScript = Path.Combine(workingDir,PM_LOADSCRIPT)
            if workingDirSpecFile.Exists && 
               (File.ReadAllLines(workingDirSpecFile.FullName) |> Array.toList) = packageManagerTextLines && 
               File.Exists loadScript
            then 
                printfn "skipping running package resolution... already done that" 
                Solved loadScript
            else
                try File.Delete(loadScript) with _ -> ()
                let toolPath = alterToolPath toolPath
                File.WriteAllLines(workingDirSpecFile.FullName, packageManagerTextLines)
                printfn "running package resolution in '%s'..." workingDir
                let startInfo = 
                    System.Diagnostics.ProcessStartInfo(
                        FileName = toolPath,
                        WorkingDirectory = workingDir, 
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        Arguments = PM_COMMAND,
                        CreateNoWindow = true,
                        UseShellExecute = false)
                
                use p = new System.Diagnostics.Process()
                let errors = System.Collections.Generic.List<_>()
                let log = System.Collections.Generic.List<_>()
                p.StartInfo <- startInfo
                p.ErrorDataReceived.Add(fun d -> if d.Data <> null then errors.Add d.Data)
                p.OutputDataReceived.Add(fun d -> if d.Data <> null then log.Add d.Data)
                p.Start() |> ignore
                p.BeginErrorReadLine()
                p.BeginOutputReadLine()
                p.WaitForExit()

                printfn "done running package resolution..."
                if p.ExitCode <> 0 then
                    let msg = String.Join(Environment.NewLine, errors)
                    PackageResolutionFailed(toolPath, workingDir, msg)
                else
                    printfn "package resolution completed at %A" System.DateTimeOffset.UtcNow
                    Solved loadScript

