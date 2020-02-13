// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Interactive.DependencyManager

open System
open System.IO
open System.Reflection
open FSharp.Reflection
open Internal.Utilities.FSharpEnvironment

module ReflectionHelper =
    let dependencyManagerPattern = "*DependencyManager*.dll"
    let dependencyManagerAttributeName= "DependencyManagerAttribute"
    let resolveDependenciesMethodName = "ResolveDependencies"
    let namePropertyName = "Name"
    let keyPropertyName = "Key"

    let assemblyHasAttribute (theAssembly: Assembly) attributeName =
        try
            CustomAttributeExtensions.GetCustomAttributes(theAssembly)
            |> Seq.exists (fun a -> a.GetType().Name = attributeName)
        with | _ -> false

    let getAttributeNamed (theType: Type) attributeName =
        try
            theType.GetTypeInfo().GetCustomAttributes false
            |> Seq.tryFind (fun a -> a.GetType().Name = attributeName)
        with | _ -> None

    let getInstanceProperty<'treturn> (theType: Type) propertyName =
        try
            let property = theType.GetProperty(propertyName, typeof<'treturn>)
            if isNull property then
                None
            elif not (property.GetGetMethod().IsStatic)
                 && property.GetIndexParameters() = Array.empty
            then
                Some property
            else
                None
        with | _ -> None

    let getInstanceMethod<'treturn> (theType: Type) (parameterTypes: Type array) methodName =
        try
            let theMethod = theType.GetMethod(methodName, parameterTypes)
            if isNull theMethod then
                None
            else
                Some theMethod
        with | _ -> None

    let stripTieWrapper (e:Exception) =
        match e with
        | :? TargetInvocationException as e->
            e.InnerException
        | _ -> e

open ReflectionHelper

(* Shape of Dependency Manager contract, resolved using reflection *)
type IDependencyManagerProvider =
    abstract Name: string
    abstract Key: string
    abstract ResolveDependencies: scriptDir: string * mainScriptName: string * scriptName: string * scriptExt: string * packageManagerTextLines: string seq * tfm: string -> bool * string seq * string seq

[<RequireQualifiedAccess>]
type ReferenceType =
| RegisteredDependencyManager of IDependencyManagerProvider
| Library of string
| UnknownType

[<RequireQualifiedAccess>]
type ErrorReportType =
| Warning
| Error

type ReflectionDependencyManagerProvider(theType: Type, nameProperty: PropertyInfo, keyProperty: PropertyInfo, resolveDeps: MethodInfo option, resolveDepsEx: MethodInfo option,outputDir: string option) =
    let instance = Activator.CreateInstance(theType, [|outputDir :> obj|])
    let nameProperty     = nameProperty.GetValue >> string
    let keyProperty      = keyProperty.GetValue >> string

    static member InstanceMaker (theType: System.Type, outputDir: string option) =
        match getAttributeNamed theType dependencyManagerAttributeName,
              getInstanceProperty<string> theType namePropertyName,
              getInstanceProperty<string> theType keyPropertyName
              with
        | None, _, _
        | _, None, _
        | _, _, None -> None
        | Some _, Some nameProperty, Some keyProperty ->
            let resolveMethod = getInstanceMethod<bool * string list * string list> theType [| typeof<string>; typeof<string>; typeof<string>; typeof<string seq>; typeof<string> |] resolveDependenciesMethodName
            let resolveMethodEx = getInstanceMethod<bool * string list * string list> theType [| typeof<string>; typeof<string seq>; typeof<string> |] resolveDependenciesMethodName
            Some (fun () -> new ReflectionDependencyManagerProvider(theType, nameProperty, keyProperty, resolveMethod, resolveMethodEx, outputDir) :> IDependencyManagerProvider)

    interface IDependencyManagerProvider with
        member __.Name     = instance |> nameProperty
        member __.Key      = instance |> keyProperty
        member this.ResolveDependencies(scriptDir, mainScriptName, scriptName, scriptExt, packageManagerTextLines, tfm) =

//            let key = (this :> IDependencyManagerProvider).Key
//            let triggerEvent (evt: Event<string * string>) =
//                for prLine in packageManagerTextLines do
//                    evt.Trigger(key, prLine)
//            triggerEvent dependencyAddingEvent


            // The ResolveDependencies method, has two signatures, the original signaature in the variable resolveDeps and the updated signature resoveDepsEx
            // The resolve method can return values in two different tuples:
            //     (bool * string list * string list * string list)
            //     (bool * string list * string list)
            //
            // We use reflection to get the correct method and to determine what we got back.
            //
            let method, arguments =
                if resolveDepsEx.IsSome then
                    resolveDepsEx, [| box scriptExt; box packageManagerTextLines; box tfm |]
                elif resolveDeps.IsSome then
                    resolveDeps, [| box scriptDir; box mainScriptName; box scriptName; box packageManagerTextLines; box tfm |]
                else
                    None, [||]

            let succeeded, _references, generatedScripts, additionalIncludeFolders =
                let empty = Seq.empty<string>
                let result =
                    match method with
                    | Some m -> m.Invoke(instance, arguments) :?> _
                    | None -> false, empty, empty, empty

                // Verify the number of arguments returned in the tuple returned by resolvedependencies, it can be:
                //     4 - (bool * string list * string list * string list)
                //     3 - (bool * string list * string list)
                let tupleFields = result |> FSharpValue.GetTupleFields
                match tupleFields |> Array.length with
                | 4 -> tupleFields.[0] :?> bool, tupleFields.[1] :?> string seq, tupleFields.[2] :?> string seq, tupleFields.[3] :?> string seq
                | 3 -> tupleFields.[0] :?> bool, empty, tupleFields.[1] :?> string list  |> List.toSeq, tupleFields.[2] :?> string list |> List.toSeq
                | _ -> false, empty, empty, empty

//            for prLine in packageManagerTextLines do
//                if succeeded then
//                    dependencyAddedEvent.Trigger(key, prLine, references |> List.toSeq, generatedScripts |> List.toSeq, additionalIncludeFolders |> List.toSeq)
//                else
//                    dependencyFailedEvent.Trigger(key, prLine)

            succeeded, generatedScripts, additionalIncludeFolders


type DependencyProvider () =
    // Resolution Path = Location of FSharp.Compiler.Private.dll
    let assemblySearchPaths = lazy (
        [
            let assemblyLocation = typeof<IDependencyManagerProvider>.GetTypeInfo().Assembly.Location
            yield Path.GetDirectoryName assemblyLocation
            yield AppDomain.CurrentDomain.BaseDirectory
        ])

    let enumerateDependencyManagerAssemblies compilerTools reportError =
        getCompilerToolsDesignTimeAssemblyPaths compilerTools
        |> Seq.append (assemblySearchPaths.Force())
        |> Seq.collect (fun path ->
            try
                if Directory.Exists(path) then Directory.EnumerateFiles(path, dependencyManagerPattern)
                else Seq.empty
            with _ -> Seq.empty)
        |> Seq.choose (fun path -> 
            try
                Some(Assembly.LoadFrom path)
            with
            | e ->
                let e = stripTieWrapper e
                reportError ErrorReportType.Warning (InteractiveDependencyManager.SR.couldNotLoadDependencyManagerExtension(path,e.Message))
                None)
        |> Seq.filter (fun a -> assemblyHasAttribute a dependencyManagerAttributeName)

    let mutable registeredDependencyManagers = None

    let RegisteredDependencyManagers (compilerTools: string seq) (outputDir: string option) (reportError: ErrorReportType -> int * string -> unit) =
        match registeredDependencyManagers with
        | Some managers -> managers
        | None ->
            let defaultProviders =[]

            let loadedProviders =
                enumerateDependencyManagerAssemblies compilerTools reportError
                |> Seq.collect (fun a -> a.GetTypes())
                |> Seq.choose (fun t -> ReflectionDependencyManagerProvider.InstanceMaker(t, outputDir))
                |> Seq.map (fun maker -> maker ())

            defaultProviders
            |> Seq.append loadedProviders
            |> Seq.map (fun pm -> pm.Key, pm)
            |> Map.ofSeq

    member _.CreatePackageManagerUnknownError(compilerTools: string seq, outputDir: string option, packageManagerKey: string, reportError: ErrorReportType -> int * string -> unit) =

        let registeredKeys = String.Join(", ", RegisteredDependencyManagers compilerTools outputDir reportError |> Seq.map (fun kv -> kv.Value.Key))
        let searchPaths = assemblySearchPaths.Force()
        InteractiveDependencyManager.SR.packageManagerUnknown(packageManagerKey, String.Join(", ", searchPaths, compilerTools), registeredKeys)

    member this.TryFindDependencyManagerInPath (compilerTools: string seq, outputDir: string option, reportError: ErrorReportType -> int * string -> unit, path: string): ReferenceType =

        try
            if path.Contains ":" && not (System.IO.Path.IsPathRooted path) then
                let managers = RegisteredDependencyManagers compilerTools outputDir reportError

                match managers |> Seq.tryFind (fun kv -> path.StartsWith(kv.Value.Key + ":" )) with
                | None ->
                    reportError ErrorReportType.Error (this.CreatePackageManagerUnknownError(compilerTools, outputDir, (path.Split(':').[0]), reportError))
                    ReferenceType.UnknownType

                | Some kv -> ReferenceType.RegisteredDependencyManager kv.Value
            else
                ReferenceType.Library path
        with 
        | e ->
            let e = stripTieWrapper e
            reportError ErrorReportType.Error (InteractiveDependencyManager.SR.packageManagerError(e.Message))
            ReferenceType.UnknownType

    member _.RemoveDependencyManagerKey(packageManagerKey:string, path:string): string =

        path.Substring(packageManagerKey.Length + 1).Trim()

    member _.TryFindDependencyManagerByKey (compilerTools: string seq, outputDir: string option, reportError: ErrorReportType -> int * string -> unit, key: string): IDependencyManagerProvider option =

        try
            RegisteredDependencyManagers compilerTools outputDir reportError |> Map.tryFind key

        with
        | e ->
            let e = stripTieWrapper e
            reportError ErrorReportType.Error (InteractiveDependencyManager.SR.packageManagerError(e.Message))
            None

    member _.Resolve (packageManager:IDependencyManagerProvider, implicitIncludeDir: string, mainScriptName: string, fileName: string, scriptExt: string, packageManagerTextLines: string seq, reportError: ErrorReportType -> int * string -> unit, executionTfm: string) =

        try
            Some(packageManager.ResolveDependencies(implicitIncludeDir, mainScriptName, fileName, scriptExt, packageManagerTextLines, executionTfm))

        with e ->
            let e = stripTieWrapper e
            reportError ErrorReportType.Error (InteractiveDependencyManager.SR.packageManagerError(e.Message))
            None
