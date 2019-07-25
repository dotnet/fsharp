// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.DependencyManager

open System
open System.Collections.Concurrent
open System.Diagnostics
open System.IO
open System.Reflection

open FSharp.DependencyManager
open FSharp.DependencyManager.Utilities

module Attributes =
    [<assembly: DependencyManagerAttribute()>]
    do ()


type PackageReference = { Include:string; Version:string; RestoreSources:string; Script:string }

type [<DependencyManagerAttribute>] FSharpDependencyManager (outputDir:string option) =

    let key = "nuget"
    let name = "MsBuild Nuget DependencyManager"
    let scriptsPath =
        let path = Path.Combine(Path.GetTempPath(), key, Process.GetCurrentProcess().Id.ToString())
        match outputDir with
        | None -> path
        | Some v -> Path.Combine(path, v)
    let generatedScripts = new ConcurrentDictionary<string,string>()
    let deleteScripts () =
        try
            if Directory.Exists(scriptsPath) then
                () //Directory.Delete(scriptsPath, true)
        with | _ -> ()

    let deleteAtExit =
        try
            if not (File.Exists(scriptsPath)) then
                Directory.CreateDirectory(scriptsPath) |> ignore
            true
        with | _ -> false

    let emitFile filename (body:string) =
        try
            // Create a file to write to
            use sw = File.CreateText(filename)
            sw.WriteLine(body)
        with | _ -> ()

    let concat (s:string) (v:string) : string =
        match String.IsNullOrEmpty(s), String.IsNullOrEmpty(v) with
        | false, false -> s + ";" + v
        | false, true -> s
        | true, false -> v
        | _  -> ""

    do if deleteAtExit then AppDomain.CurrentDomain.ProcessExit |> Event.add(fun _ -> deleteScripts () )

    let formatPackageReference p =
        let { Include=inc; Version=ver; RestoreSources=src; Script=script } = p
        seq {
            match not (String.IsNullOrEmpty(inc)), not (String.IsNullOrEmpty(ver)),  not (String.IsNullOrEmpty(script)) with
            | true, true, false  -> yield sprintf @"  <ItemGroup><PackageReference Include = '%s' Version = '%s'><GeneratePathProperty>true</GeneratePathProperty></PackageReference></ItemGroup>" inc ver
            | true, true, true   -> yield sprintf @"  <ItemGroup><PackageReference Include = '%s' Version = '%s' Script = '%s'><GeneratePathProperty>true</GeneratePathProperty></PackageReference></ItemGroup>" inc ver script
            | true, false, false -> yield sprintf @"  <ItemGroup><PackageReference Include = '%s'><GeneratePathProperty>true</GeneratePathProperty></PackageReference></ItemGroup>" inc 
            | true, false, true  -> yield sprintf @"  <ItemGroup><PackageReference Include = '%s' Script = '%s'><GeneratePathProperty>true</GeneratePathProperty></PackageReference></ItemGroup>" inc script
            | _ -> ()
            match not (String.IsNullOrEmpty(src)) with
            | true -> yield sprintf @"  <PropertyGroup><RestoreAdditionalProjectSources>%s</RestoreAdditionalProjectSources></PropertyGroup>" (concat "$(RestoreAdditionalProjectSources)" src)
            | _ -> ()
        }

    member __.Name = name

    member __.Key = key

    member __.ResolveDependencies(_scriptDir:string, _mainScriptName:string, _scriptName:string, packageManagerTextLines:string seq, tfm: string) : bool * string list * string list =

        let packageReferences, binLogging =
            let validatePackageName package packageName =
                if String.Compare(packageName, package, StringComparison.OrdinalIgnoreCase) = 0 then
                    raise (ArgumentException(sprintf "PackageManager can not reference the System Package '%s'" packageName))   // @@@@@@@@@@@@@@@@@@@@@@@ Globalize me please

            let mutable binLogging = false
            let references = [
                for line in packageManagerTextLines do
                    let options = getOptions line
                    let mutable found = false
                    let mutable packageReference = { Include = ""; Version = "*"; RestoreSources = ""; Script = "" }
                    for opt in options do
                        let addInclude v =
                            // TODO:  Consider a comprehensive list of dotnet framework packages that are disallowed
                            validatePackageName v "mscorlib"
                            validatePackageName v "FSharp.Core"
                            validatePackageName v "System.ValueTuple"
                            validatePackageName v "NETStandard.Library"
                            found <- true
                            packageReference <- { packageReference with Include = v }

                        let addScript v = packageReference <- { packageReference with Script = v }
                        match opt with
                        | Some "version", Some v -> 
                            found <- true
                            packageReference <- { packageReference with Version = v }
                        | Some "restoresources", Some v ->
                            found <- true
                            packageReference <- { packageReference with RestoreSources = concat packageReference.RestoreSources v }
                        | Some "bl", Some v ->
                            binLogging <-
                                match v.ToLowerInvariant() with
                                | "true" -> true
                                | _ -> false
                        | Some "include", None ->
                            raise (ArgumentException(sprintf "%s requires a value" "Include"))                              // @@@@@@@@@@@@@@@@@@@@@@@ Globalize me please
                        | Some "version", None -> 
                            raise (ArgumentException(sprintf "%s requires a value" "Version"))                              // @@@@@@@@@@@@@@@@@@@@@@@ Globalize me please
                        | Some "restoresources", None ->
                            raise (ArgumentException(sprintf "%s requires a value" "RestoreSources"))                       // @@@@@@@@@@@@@@@@@@@@@@@ Globalize me please
                        | Some "include", Some v -> addInclude v
                        | Some "script", Some v -> addScript v
                        | None, Some v -> addInclude v
                        | _ -> ()
                    if found then yield! formatPackageReference packageReference
            ]
            references |> List.distinct |>String.concat Environment.NewLine, binLogging

        // Generate a project files
        let generateAndBuildProjectArtifacts =
            let writeFile path body =
                if not (generatedScripts.ContainsKey(body.GetHashCode().ToString())) then
                    emitFile path  body

            let fsProjectPath = Path.Combine(scriptsPath, "Project.fsproj")

            let generateProjBody =
                generateProjectBody.Replace("$(TARGETFRAMEWORK)", tfm)
                                   .Replace("$(PACKAGEREFERENCES)", packageReferences)

            writeFile (Path.Combine(scriptsPath, "Library.fs")) generateLibrarySource
            writeFile fsProjectPath generateProjBody

            let succeeded, resultingFsx = buildProject fsProjectPath binLogging
            let fsx =
                match resultingFsx with
                | Some fsx -> [fsx]
                | None -> []

            succeeded, fsx, List.empty<string>

        generateAndBuildProjectArtifacts
