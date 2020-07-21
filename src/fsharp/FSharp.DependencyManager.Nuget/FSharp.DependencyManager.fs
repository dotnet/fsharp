// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.DependencyManager.Nuget

open System
open System.Collections.Concurrent
open System.Diagnostics
open System.IO
open FSharp.DependencyManager.Nuget
open FSharp.DependencyManager.Nuget.Utilities
open FSharp.DependencyManager.Nuget.ProjectFile
open FSDependencyManager

module FSharpDependencyManager =

    [<assembly: DependencyManagerAttribute()>]
    do ()

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
            | true, true, false  -> yield sprintf @"  <ItemGroup><PackageReference Include='%s' Version='%s' /></ItemGroup>" inc ver
            | true, true, true   -> yield sprintf @"  <ItemGroup><PackageReference Include='%s' Version='%s' Script='%s' /></ItemGroup>" inc ver script
            | true, false, false -> yield sprintf @"  <ItemGroup><PackageReference Include='%s' /></ItemGroup>" inc
            | true, false, true  -> yield sprintf @"  <ItemGroup><PackageReference Include='%s' Script='%s' /></ItemGroup>" inc script
            | _ -> ()
            match not (String.IsNullOrEmpty(src)) with
            | true -> yield sprintf @"  <PropertyGroup><RestoreAdditionalProjectSources>%s</RestoreAdditionalProjectSources></PropertyGroup>" (concat "$(RestoreAdditionalProjectSources)" src)
            | _ -> ()
        }

    let parsePackageReference scriptExt (lines: string list) =
        let mutable binLogPath = None
        let parsePackageReferenceOption (line: string) =
            let validatePackageName package packageName =
                if String.Compare(packageName, package, StringComparison.OrdinalIgnoreCase) = 0 then
                    raise (ArgumentException(SR.cantReferenceSystemPackage(packageName)))
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
                        if scriptExt = fsxExt then
                            validatePackageName v "FSharp.Core"
                        validatePackageName v "System.ValueTuple"
                        validatePackageName v "NETStandard.Library"
                        Some { current with Include = v }
                    let setVersion v = Some { current with Version = v }
                    match opt with
                    | Some "include", Some v -> addInclude v |> parsePackageReferenceOption' rest implicitArgumentCount
                    | Some "include", None -> raise (ArgumentException(SR.requiresAValue("Include")))
                    | Some "version", Some v -> setVersion v |> parsePackageReferenceOption' rest implicitArgumentCount
                    | Some "version", None -> setVersion "*" |> parsePackageReferenceOption' rest implicitArgumentCount
                    | Some "restoresources", Some v -> Some { current with RestoreSources = concat current.RestoreSources v } |> parsePackageReferenceOption' rest implicitArgumentCount
                    | Some "restoresources", None -> raise (ArgumentException(SR.requiresAValue("RestoreSources")))
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
                            | _ -> raise (ArgumentException(SR.unableToApplyImplicitArgument(implicitArgumentCount + 1)))
                            |> parsePackageReferenceOption' rest (implicitArgumentCount + 1)
                    | _ -> parsePackageReferenceOption' rest implicitArgumentCount packageReference
            let options = getOptions line
            parsePackageReferenceOption' options 0 None
        lines
        |> List.choose parsePackageReferenceOption
        |> List.distinct
        |> (fun l -> l, binLogPath)


/// The results of ResolveDependencies
type ResolveDependenciesResult (success: bool, stdOut: string array, stdError: string array, resolutions: string seq, sourceFiles: string seq, roots: string seq) =

    /// Succeded?
    member _.Success = success

    /// The resolution output log
    member _.StdOut = stdOut

    /// The resolution error log (* process stderror *)
    member _.StdError = stdError

    /// The resolution paths
    member _.Resolutions = resolutions

    /// The source code file paths
    member _.SourceFiles = sourceFiles

    /// The roots to package directories
    member _.Roots = roots

[<DependencyManagerAttribute>] 
type FSharpDependencyManager (outputDir:string option) =

    let key = "nuget"
    let name = "MsBuild Nuget DependencyManager"
    let scriptsPath =
        let path = Path.Combine(Path.GetTempPath(), key, Process.GetCurrentProcess().Id.ToString() + "--"+ Guid.NewGuid().ToString())
        match outputDir with
        | None -> path
        | Some v -> Path.Combine(path, v)

    let generatedScripts = new ConcurrentDictionary<string,string>()

    let deleteScripts () =
        try
#if !Debug
            if Directory.Exists(scriptsPath) then
                Directory.Delete(scriptsPath, true)
#else
            ()
#endif
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

    member _.Name = name

    member _.Key = key

    member _.HelpMessages = [|
        sprintf """    #r "nuget:FSharp.Data, 3.1.2";;   // %s 'FSharp.Data' %s '3.1.2'""" (SR.loadNugetPackage()) (SR.version())
        sprintf """    #r "nuget:FSharp.Data";;          // %s 'FSharp.Data' %s""" (SR.loadNugetPackage()) (SR.highestVersion())
        |]

    member _.ResolveDependencies(scriptExt:string, packageManagerTextLines:string seq, tfm: string, rid: string) : obj =

        let scriptExt, poundRprefix  =
            match scriptExt with
            | ".csx" -> csxExt, "#r \"" 
            | _ -> fsxExt, "#r @\"" 

        let packageReferences, binLogPath =
            packageManagerTextLines
            |> List.ofSeq
            |> FSharpDependencyManager.parsePackageReference scriptExt

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

            let projectPath = Path.Combine(scriptsPath, "Project.fsproj")

            let generateProjBody =
                generateProjectBody.Replace("$(TARGETFRAMEWORK)", tfm)
                                   .Replace("$(RUNTIMEIDENTIFIER)", rid)
                                   .Replace("$(PACKAGEREFERENCES)", packageReferenceText)
                                   .Replace("$(SCRIPTEXTENSION)", scriptExt)

            writeFile projectPath generateProjBody

            let result, stdOut, stdErr,  resolutionsFile = buildProject projectPath binLogPath
            match resolutionsFile with
            | Some file ->
                let resolutions = getResolutionsFromFile file
                let references = (findReferencesFromResolutions resolutions) |> Array.toSeq
                let scripts =
                    let scriptPath = projectPath + scriptExt
                    let scriptBody =  makeScriptFromReferences references poundRprefix
                    emitFile scriptPath scriptBody
                    let loads = (findLoadsFromResolutions resolutions) |> Array.toList
                    List.concat [ [scriptPath]; loads] |> List.toSeq
                let includes = (findIncludesFromResolutions resolutions) |> Array.toSeq

                ResolveDependenciesResult(result, stdOut, stdErr, references, scripts, includes)

            | None ->
                let empty = Seq.empty<string>
                ResolveDependenciesResult(result, stdOut, stdErr, empty, empty, empty)

        generateAndBuildProjectArtifacts :> obj
