// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

/// Helper members to integrate DependencyManagers into F# codebase
module internal FSharp.Compiler.DependencyManagerIntegration

open System
open System.IO
open System.Reflection
open FSharp.Compiler.DotNetFrameworkDependencies
open FSharp.Compiler.ErrorLogger
open Internal.Utilities.FSharpEnvironment

// Contract strings
let dependencyManagerPattern = "*DependencyManager*.dll"
let dependencyManagerAttributeName= "DependencyManagerAttribute"
let resolveDependenciesMethodName = "ResolveDependencies"
let namePropertyName = "Name"
let keyPropertyName = "Key"

module ReflectionHelper =
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

(* Shape of Dependency Manager contract, resolved using reflection *)
type internal IDependencyManagerProvider =
    abstract Name: string
    abstract Key: string
    abstract ResolveDependencies: scriptDir: string * mainScriptName: string * scriptName: string * packageManagerTextLines: string seq * tfm: string -> bool * string list * string list
    abstract DependencyAdding: IEvent<string * string>
    abstract DependencyAdded: IEvent<string * string>
    abstract DependencyFailed: IEvent<string * string>

[<RequireQualifiedAccess>]
type ReferenceType =
| RegisteredDependencyManager of IDependencyManagerProvider
| Library of string
| UnknownType

type ReflectionDependencyManagerProvider(theType: Type, nameProperty: PropertyInfo, keyProperty: PropertyInfo, resolveDeps: MethodInfo, outputDir: string option) =
    let instance = Activator.CreateInstance(theType, [|outputDir :> obj|])
    let nameProperty     = nameProperty.GetValue >> string
    let keyProperty      = keyProperty.GetValue >> string

    let dependencyAddingEvent = Event<_>()
    let dependencyAddedEvent = Event<_>()
    let dependencyFailedEvent = Event<_>()

    static member InstanceMaker (theType: System.Type, outputDir: string option) =
        match ReflectionHelper.getAttributeNamed theType dependencyManagerAttributeName,
                ReflectionHelper.getInstanceProperty<string> theType namePropertyName,
                ReflectionHelper.getInstanceProperty<string> theType keyPropertyName,
                ReflectionHelper.getInstanceMethod<bool * string list * string list> theType [| typeof<string>; typeof<string>; typeof<string>; typeof<string seq>; typeof<string> |] resolveDependenciesMethodName
                with
        | None, _, _, _ -> None
        | _, None, _, _ -> None
        | _, _, None, _ -> None
        | _, _, _, None -> None
        | Some _, Some nameProperty, Some keyProperty, Some resolveDependenciesMethod ->
            Some (fun () -> new ReflectionDependencyManagerProvider(theType, nameProperty, keyProperty, resolveDependenciesMethod, outputDir) :> IDependencyManagerProvider)

    interface IDependencyManagerProvider with
        member __.Name     = instance |> nameProperty
        member __.Key      = instance |> keyProperty
        member this.ResolveDependencies(scriptDir, mainScriptName, scriptName, packageManagerTextLines, tfm) =
            let key = (this :> IDependencyManagerProvider).Key
            let triggerEvent (evt: Event<string * string>) =
                for prLine in packageManagerTextLines do
                    evt.Trigger(key, prLine)
            triggerEvent dependencyAddingEvent
            let arguments = [| box scriptDir; box mainScriptName; box scriptName; box packageManagerTextLines; box tfm |]
            let succeeded, generatedScripts, additionalIncludeFolders = resolveDeps.Invoke(instance, arguments) :?> _
            if succeeded then triggerEvent dependencyAddedEvent
            else triggerEvent dependencyFailedEvent
            succeeded, generatedScripts, additionalIncludeFolders
        member __.DependencyAdding = dependencyAddingEvent.Publish
        member __.DependencyAdded = dependencyAddedEvent.Publish
        member __.DependencyFailed = dependencyFailedEvent.Publish

// Resolution Path = Location of FSharp.Compiler.Private.dll
let assemblySearchPaths = lazy (
    [
        let assemblyLocation = typeof<IDependencyManagerProvider>.GetTypeInfo().Assembly.Location
        yield Path.GetDirectoryName assemblyLocation
        yield AppDomain.CurrentDomain.BaseDirectory
    ])

let enumerateDependencyManagerAssemblies compilerTools m =
    getCompilerToolsDesignTimeAssemblyPaths compilerTools
    |> Seq.append (assemblySearchPaths.Force())
    |> Seq.collect (fun path ->
        try
            if Directory.Exists(path) then Directory.EnumerateFiles(path, dependencyManagerPattern)
            else Seq.empty
        with _ -> Seq.empty)
    |> Seq.choose (fun path -> 
        try
            Some(AbstractIL.Internal.Library.Shim.FileSystem.AssemblyLoadFrom path)
        with 
        | e ->
            let e = ReflectionHelper.stripTieWrapper e
            warning(Error(FSComp.SR.couldNotLoadDependencyManagerExtension(path,e.Message),m))
            None)
    |> Seq.filter (fun a -> ReflectionHelper.assemblyHasAttribute a dependencyManagerAttributeName)

let registeredDependencyManagers = ref None

let RegisteredDependencyManagers (compilerTools: string list) (outputDir: string option) m =
    match !registeredDependencyManagers with
    | Some managers -> managers
    | None ->
        let defaultProviders =[]

        let loadedProviders =
            enumerateDependencyManagerAssemblies compilerTools m
            |> Seq.collect (fun a -> a.GetTypes())
            |> Seq.choose (fun t -> ReflectionDependencyManagerProvider.InstanceMaker(t, outputDir))
            |> Seq.map (fun maker -> maker ())

        defaultProviders
        |> Seq.append loadedProviders
        |> Seq.map (fun pm -> pm.Key, pm)
        |> Map.ofSeq

let createPackageManagerUnknownError (compilerTools: string list) (outputDir:string option) packageManagerKey m =
    let registeredKeys = String.Join(", ", RegisteredDependencyManagers compilerTools outputDir m |> Seq.map (fun kv -> kv.Value.Key))
    let searchPaths = assemblySearchPaths.Force()
    Error(FSComp.SR.packageManagerUnknown(packageManagerKey, String.Join(", ", searchPaths, compilerTools), registeredKeys),m)

let tryFindDependencyManagerInPath (compilerTools:string list) (outputDir:string option) m (path:string) : ReferenceType =
    try
        if path.Contains ":" && not (System.IO.Path.IsPathRooted path) then
            let managers = RegisteredDependencyManagers compilerTools outputDir m
            match managers |> Seq.tryFind (fun kv -> path.StartsWith(kv.Value.Key + ":" )) with
            | None ->
                errorR(createPackageManagerUnknownError compilerTools outputDir (path.Split(':').[0]) m)
                ReferenceType.UnknownType
            | Some kv -> ReferenceType.RegisteredDependencyManager kv.Value
        else
            ReferenceType.Library path
    with 
    | e ->
        let e = ReflectionHelper.stripTieWrapper e
        errorR(Error(FSComp.SR.packageManagerError(e.Message),m))
        ReferenceType.UnknownType

let removeDependencyManagerKey (packageManagerKey:string) (path:string) = path.Substring(packageManagerKey.Length + 1).Trim()

let tryFindDependencyManagerByKey (compilerTools: string list) (outputDir:string option) m (key:string) : IDependencyManagerProvider option =
    try
        RegisteredDependencyManagers compilerTools outputDir m |> Map.tryFind key
    with 
    | e -> 
        let e = ReflectionHelper.stripTieWrapper e
        errorR(Error(FSComp.SR.packageManagerError(e.Message), m))
        None

let resolve (packageManager:IDependencyManagerProvider) implicitIncludeDir mainScriptName fileName m packageManagerTextLines =
    try
        Some(packageManager.ResolveDependencies(implicitIncludeDir, mainScriptName, fileName, packageManagerTextLines, executionTfm))
    with e ->
        let e = ReflectionHelper.stripTieWrapper e
        errorR(Error(FSComp.SR.packageManagerError(e.Message), m))
        None
