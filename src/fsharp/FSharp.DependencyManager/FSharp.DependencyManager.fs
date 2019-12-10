// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.DependencyManager

open System
open System.Collections.Concurrent
open System.Diagnostics
open System.IO
open FSharp.DependencyManager
open FSharp.DependencyManager.Utilities

module Attributes =
    [<assembly: DependencyManagerAttribute()>]
    do ()

type PackageReference = { Include:string; Version:string; RestoreSources:string; Script:string }

module FSharpDependencyManager =

    let private concat (s:string) (v:string) : string =
        match String.IsNullOrEmpty(s), String.IsNullOrEmpty(v) with
        | false, false -> s + ";" + v
        | false, true -> s
        | true, false -> v
        | _  -> ""

    let formatPackageReference p =
        let { Include=inc; Version=ver; RestoreSources=src; Script=script } = p
        seq {
            match not (String.IsNullOrEmpty(inc)), not (String.IsNullOrEmpty(ver)), not (String.IsNullOrEmpty(script)) with
            | true, true, false  -> yield sprintf @"  <ItemGroup><PackageReference Include='%s' Version='%s'><GeneratePathProperty>true</GeneratePathProperty></PackageReference></ItemGroup>" inc ver
            | true, true, true   -> yield sprintf @"  <ItemGroup><PackageReference Include='%s' Version='%s' Script='%s'><GeneratePathProperty>true</GeneratePathProperty></PackageReference></ItemGroup>" inc ver script
            | true, false, false -> yield sprintf @"  <ItemGroup><PackageReference Include='%s'><GeneratePathProperty>true</GeneratePathProperty></PackageReference></ItemGroup>" inc
            | true, false, true  -> yield sprintf @"  <ItemGroup><PackageReference Include='%s' Script='%s'><GeneratePathProperty>true</GeneratePathProperty></PackageReference></ItemGroup>" inc script
            | _ -> ()
            match not (String.IsNullOrEmpty(src)) with
            | true -> yield sprintf @"  <PropertyGroup><RestoreAdditionalProjectSources>%s</RestoreAdditionalProjectSources></PropertyGroup>" (concat "$(RestoreAdditionalProjectSources)" src)
            | _ -> ()
        }

    let parsePackageReference (lines: string list) =
        let mutable binLogPath = None
        let parsePackageReferenceOption (line: string) =
            let validatePackageName package packageName =
                if String.Compare(packageName, package, StringComparison.OrdinalIgnoreCase) = 0 then
                    raise (ArgumentException(sprintf "PackageManager can not reference the System Package '%s'" packageName))  // @@@@@@@@@@@@@@@@@@@@@@@ Globalize me please
            let rec parsePackageReferenceOption' (options: (string option * string option) list) (implicitArgumentCount: int) (packageReference: PackageReference option) =
                let current =
                    match packageReference with
                    | Some p -> p
                    | None -> { Include = ""; Version = "*"; RestoreSources = ""; Script = "" }
                match options with
                | [] -> packageReference
                | opt :: rest ->
                    let addInclude v =
                        validatePackageName v "mscorlib"
                        validatePackageName v "FSharp.Core"
                        validatePackageName v "System.ValueTuple"
                        validatePackageName v "NETStandard.Library"
                        Some { current with Include = v }
                    let setVersion v = Some { current with Version = v }
                    match opt with
                    | Some "include", Some v -> addInclude v |> parsePackageReferenceOption' rest implicitArgumentCount
                    | Some "include", None -> raise (ArgumentException(sprintf "%s requires a value" "Include"))               // @@@@@@@@@@@@@@@@@@@@@@@ Globalize me please
                    | Some "version", Some v -> setVersion v |> parsePackageReferenceOption' rest implicitArgumentCount
                    | Some "version", None -> raise (ArgumentException(sprintf "%s requires a value" "Version"))               // @@@@@@@@@@@@@@@@@@@@@@@ Globalize me please
                    | Some "restoresources", Some v -> Some { current with RestoreSources = concat current.RestoreSources v } |> parsePackageReferenceOption' rest implicitArgumentCount
                    | Some "restoresources", None -> raise (ArgumentException(sprintf "%s requires a value" "RestoreSources")) // @@@@@@@@@@@@@@@@@@@@@@@ Globalize me please
                    | Some "script", Some v -> Some { current with Script = v } |> parsePackageReferenceOption' rest implicitArgumentCount
                    | Some "bl", value ->
                        match value with
                        | Some v when v.ToLowerInvariant() = "true" -> binLogPath <- Some None // auto-generated logging location
                        | Some v when v.ToLowerInvariant() = "false" -> binLogPath <- None // no logging
                        | Some path -> binLogPath <- Some(Some path) // explicit logging location
                        | None ->
                            // parser shouldn't get here because unkeyed values follow a different path, but for the sake of completeness and keeping the compiler happy,
                            // this is fine
                            binLogPath <- Some None // auto-generated logging location
                        parsePackageReferenceOption' rest implicitArgumentCount packageReference
                    | None, Some v ->
                        match v.ToLowerInvariant() with
                        | "bl" ->
                            // a bare 'bl' enables binary logging and is NOT interpreted as one of the positional arguments.  On the off chance that the user actually wants
                            // to reference a package named 'bl' they still have the 'Include=bl' syntax as a fallback.
                            binLogPath <- Some None // auto-generated logging location
                            parsePackageReferenceOption' rest implicitArgumentCount packageReference
                        | _ ->
                            match implicitArgumentCount with
                            | 0 -> addInclude v
                            | 1 -> setVersion v
                            | _ -> raise (ArgumentException(sprintf "Unable to apply implicit argument number %d" (implicitArgumentCount + 1))) // @@@@@@@@@@@@@@@@@@@@@@@ Globalize me please
                            |> parsePackageReferenceOption' rest (implicitArgumentCount + 1)
                    | _ -> parsePackageReferenceOption' rest implicitArgumentCount packageReference
            let options = getOptions line
            parsePackageReferenceOption' options 0 None
        lines
        |> List.choose parsePackageReferenceOption
        |> List.distinct
        |> (fun l -> l, binLogPath)

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

    do if deleteAtExit then AppDomain.CurrentDomain.ProcessExit |> Event.add(fun _ -> deleteScripts () )

    member __.Name = name

    member __.Key = key

    member __.ResolveDependencies(_scriptDir:string, _mainScriptName:string, _scriptName:string, packageManagerTextLines:string seq, tfm: string) : bool * string list * string list =

        let packageReferences, binLogPath =
            packageManagerTextLines
            |> List.ofSeq
            |> FSharpDependencyManager.parsePackageReference
        let packageReferenceLines =
            packageReferences
            |> List.map FSharpDependencyManager.formatPackageReference
            |> Seq.concat
        let packageReferenceText = String.Join(Environment.NewLine, packageReferenceLines)

        // Generate a project files
        let generateAndBuildProjectArtifacts =
            let writeFile path body =
                if not (generatedScripts.ContainsKey(body.GetHashCode().ToString())) then
                    emitFile path  body

            let fsProjectPath = Path.Combine(scriptsPath, "Project.fsproj")

            let generateProjBody =
                generateProjectBody.Replace("$(TARGETFRAMEWORK)", tfm)
                                   .Replace("$(PACKAGEREFERENCES)", packageReferenceText)

            writeFile (Path.Combine(scriptsPath, "Library.fs")) generateLibrarySource
            writeFile fsProjectPath generateProjBody

            let succeeded, resultingFsx = buildProject fsProjectPath binLogPath
            let fsx =
                match resultingFsx with
                | Some fsx -> [fsx]
                | None -> []

            succeeded, fsx, List.empty<string>

        generateAndBuildProjectArtifacts
