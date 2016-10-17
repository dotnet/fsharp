// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information,
//
// Scripting utilities
//=========================================================================================

namespace global

open System
open System.IO
open System.Diagnostics

[<AutoOpen>]
module Scripting =

    let isNullOrEmpty s = String.IsNullOrEmpty s

    let executeProcess filename arguments =
        let processWriteMessage (chan:TextWriter) (message:string) =
            if message <> null then 
                chan.WriteLine(message) 
        printfn "%s %s" filename arguments
        let info = ProcessStartInfo(Arguments=arguments, UseShellExecute=false, 
                                    RedirectStandardOutput=true, RedirectStandardError=true,
                                    CreateNoWindow=true, FileName=filename)
        let p = new Process(StartInfo=info)
        p.OutputDataReceived.Add(fun x -> processWriteMessage stdout x.Data)
        p.ErrorDataReceived.Add(fun x ->  processWriteMessage stderr x.Data)
        if p.Start() then
            p.BeginOutputReadLine()
            p.BeginErrorReadLine()
            p.WaitForExit()
            p.ExitCode
        else
            0

    let argv = fsi.CommandLineArgs |> Seq.skip 1

    let getCmdLineArg switchName defaultValue = 
        match argv |> Seq.filter(fun t -> t.StartsWith(switchName)) |> Seq.map(fun t -> t.Remove(0, switchName.Length).Trim()) |> Seq.tryHead with
        | Some(file) -> if file.Length <> 0 then file else defaultValue
        | _ -> defaultValue

    let makeDirectory output =
        if not (Directory.Exists(output)) then 
            Directory.CreateDirectory(output) |> ignore

    let (++) a b = Path.Combine(a,b)

    let getBasename a = Path.GetFileNameWithoutExtension a
    let getFullPath a = Path.GetFullPath a
    let getFilename a = Path.GetFileName a
    let getDirectoryName a = Path.GetDirectoryName a
            
    let copyFile source dir =
        let dest = 
            if not (Directory.Exists dir) then Directory.CreateDirectory dir |>ignore
            let result = Path.Combine(dir, Path.GetFileName source)
            result
        //printfn "Copy %s --> %s" source dest
        File.Copy(source, dest, true)

    let deleteDirectory output =
        if Directory.Exists output then 
            Directory.Delete(output, true) 

