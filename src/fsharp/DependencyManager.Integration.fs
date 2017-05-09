// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

/// Helper members to integrate DependencyManagers into F# codebase
module internal Microsoft.FSharp.Compiler.DependencyManagerIntegration

open System
open System.Reflection
open System.IO
open Microsoft.FSharp.Compiler.ErrorLogger

#if FX_RESHAPED_REFLECTION
open Microsoft.FSharp.Core.ReflectionAdapters
#endif

// NOTE: this contains mostly members whose intents are :
// * to keep ReferenceLoading.PaketHandler usable outside of F# (so it can be used in scriptcs & others)
// * to minimize footprint of integration in fsi/CompileOps

/// hardcoded to net461 as we don't have fsi on netcore
let targetFramework = "net461"

/// reflection helpers
module ReflectionHelper =
    let assemblyHasAttributeNamed (theAssembly: Assembly) attributeName =
        try
            theAssembly.GetCustomAttributes false
            |> Seq.tryFind (fun a -> a.GetType().Name = attributeName)
            |> function | Some _ -> true | _ -> false
        with | _ -> false

    let getAttributeNamed (theType: Type) attributeName =
        try
            theType.GetCustomAttributes false
            |> Seq.tryFind (fun a -> a.GetType().Name = attributeName)
        with | _ -> None

    let getInstanceProperty<'treturn> (theType: Type) indexParameterTypes propertyName =
        try
            let property = theType.GetProperty(propertyName, typeof<'treturn>)
            if isNull property then
                None
            elif not (property.GetGetMethod().IsStatic)
                 && property.GetIndexParameters() = indexParameterTypes
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

    let implements<'timplemented> (theType: Type) =
        typeof<'timplemented>.IsAssignableFrom(theType)

/// Contract for dependency anager provider.  This is a loose contract for now, just to define the shape, 
/// it is resolved through reflection (ReflectionDependencyManagerProvider)
type internal IDependencyManagerProvider =
    inherit System.IDisposable
    abstract Name : string
    abstract ToolName: string
    abstract Key: string
    abstract ResolveDependencies : targetFramework: string * scriptDir: string * scriptName: string * packageManagerTextLines: string seq -> string option * string list

/// Reference
[<RequireQualifiedAccess>]
type ReferenceType =
| RegisteredDependencyManager of IDependencyManagerProvider
| Library of string
| UnknownType

/// Dependency Manager Provider using dotnet reflection
type ReflectionDependencyManagerProvider(theType: Type, nameProperty: PropertyInfo, toolNameProperty: PropertyInfo, keyProperty: PropertyInfo, resolveDeps: MethodInfo) =
    let instance = Activator.CreateInstance(theType) :?> IDisposable
    let nameProperty     = nameProperty.GetValue >> string
    let toolNameProperty = toolNameProperty.GetValue >> string
    let keyProperty      = keyProperty.GetValue >> string
    static member InstanceMaker (theType: System.Type) =
        if not (ReflectionHelper.implements<IDisposable> theType) then None
        else
            match ReflectionHelper.getAttributeNamed theType "FSharpDependencyManagerAttribute" with
            | None -> None
            | Some _ ->
                match ReflectionHelper.getInstanceProperty<string> theType Array.empty "Name",
                      ReflectionHelper.getInstanceProperty<string> theType Array.empty "ToolName",
                      ReflectionHelper.getInstanceProperty<string> theType Array.empty "Key",
                      ReflectionHelper.getInstanceMethod<string * string list> theType [|typeof<string>;typeof<string>;typeof<string>;typeof<string seq>;|] "ResolveDependencies"
                      with
                | Some nameProperty, Some toolNameProperty, Some keyProperty, Some resolveDependenciesMethod ->
                    Some (fun () -> new ReflectionDependencyManagerProvider(theType, nameProperty, toolNameProperty, keyProperty, resolveDependenciesMethod) :> IDependencyManagerProvider)
                | _ -> None

    interface IDependencyManagerProvider with
        member __.Name     = instance |> nameProperty
        member __.ToolName = instance |> toolNameProperty
        member __.Key      = instance |> keyProperty
        member __.ResolveDependencies(targetFramework, scriptDir, scriptName, packageManagerTextLines) =
            let arguments = [|box targetFramework; box scriptDir; box scriptName; box packageManagerTextLines|]
            resolveDeps.Invoke(instance, arguments) :?> _

    interface IDisposable with
        member __.Dispose () = instance.Dispose()


let assemblySearchLocations (additionalIncludePaths:string list) =
    additionalIncludePaths @
    [
        yield Path.GetDirectoryName typeof<IDependencyManagerProvider>.Assembly.Location
#if FX_NO_APP_DOMAINS
        yield AppContext.BaseDirectory
#else
        yield AppDomain.CurrentDomain.BaseDirectory
#endif
    ] |> List.distinct

let enumerateDependencyManagerAssembliesFromCurrentAssemblyLocation (additionalIncludePaths:string list) =
    /// Where to search for providers
    /// Algorithm TBD
    ///     1. Directory containing FSharp.Compiler.dll
    ///     2. AppContext (AppDomain on desktop) Base directory
    ///     3. directories supplied using --lib
    (assemblySearchLocations additionalIncludePaths)
    |> Seq.collect (fun path -> Directory.EnumerateFiles(path,"*DependencyManager*.dll"))
    |> Seq.choose (fun path -> try Assembly.LoadFrom path |> Some with | _ -> None)
    |> Seq.filter (fun a -> ReflectionHelper.assemblyHasAttributeNamed a "FSharpDependencyManagerAttribute")

/// TBD
type ProjectDependencyManager() =
    interface IDependencyManagerProvider with
        member __.Name = "Project loader"
        member __.ToolName = ""
        member __.Key = "project"
        member __.ResolveDependencies(_targetFramework:string, _scriptDir: string, _scriptName: string, _packageManagerTextLines: string seq) = 
            None,[]

    interface System.IDisposable with
        member __.Dispose() = ()

/// Contract for DependencyManager for #r assemblies
type RefDependencyManager() =
    interface IDependencyManagerProvider with
        member __.Name = "Library loader"
        member __.ToolName = ""
        member __.Key = "ref"
        member __.ResolveDependencies(_targetFramework:string, _scriptDir: string, _scriptName: string, _packageManagerTextLines: string seq) = 
            None,[]

    interface System.IDisposable with
        member __.Dispose() = ()

/// Get the list of registered DependencyManagers
let registeredDependencyManagers (additionalIncludePaths: string list) = 
    let dependencyManagers =
        lazy (
            let defaultProviders =
                [new ProjectDependencyManager() :> IDependencyManagerProvider
                 new RefDependencyManager() :> IDependencyManagerProvider ]

            let loadedProviders =
                enumerateDependencyManagerAssembliesFromCurrentAssemblyLocation(additionalIncludePaths)
                |> Seq.collect (fun a -> a.GetTypes())
                |> Seq.choose ReflectionDependencyManagerProvider.InstanceMaker
                |> Seq.map (fun maker -> maker ())

        defaultProviders
        |> Seq.append loadedProviders
        |> Seq.map (fun pm -> pm.Key, pm)
        |> Map.ofSeq
        )
    dependencyManagers.Force()

/// Issue PackageManner error
let createPackageManagerUnknownError packageManagerKey m (additionalIncludePaths: string list) =
    let registeredKeys = String.Join(", ", registeredDependencyManagers(additionalIncludePaths) |> Seq.map (fun kv -> kv.Value.Key))
    let searchPaths = assemblySearchLocations additionalIncludePaths
    Error(FSComp.SR.packageManagerUnknown(packageManagerKey, String.Join(", ", searchPaths), registeredKeys),m)

/// Issue Look for a packagemanager given a #r path.  (Path may contain a package manager moniker 'nuget', 'paket' followed by ':' 
/// or be a fully qualified Windows path 'C:...', a relative path or a UNC qualified path)
let tryFindDependencyManagerInPath m (path:string) (additionalIncludePaths: string list): ReferenceType =
    try
        match path.IndexOf(":") with
        | -1 | 1 ->
            ReferenceType.Library path
        | _ ->
            let managers = registeredDependencyManagers(additionalIncludePaths)
            match managers |> Seq.tryFind (fun kv -> path.StartsWith(kv.Value.Key + ":" )) with
            | None ->
                errorR(createPackageManagerUnknownError (path.Split(':').[0]) m additionalIncludePaths)
                ReferenceType.UnknownType
            | Some kv -> (ReferenceType.RegisteredDependencyManager kv.Value)
    with
    | e ->
        errorR(Error(FSComp.SR.packageManagerError(e.Message),m))
        ReferenceType.UnknownType

let removeDependencyManagerKey (packageManagerKey:string) (path:string) = path.Substring(packageManagerKey.Length + 1).Trim()

let tryFindDependencyManagerByKey m (key:string) (additionalIncludePaths: string list): IDependencyManagerProvider option =
    try
        registeredDependencyManagers(additionalIncludePaths) |> Map.tryFind key
    with 
    | e -> 
        errorR(Error(FSComp.SR.packageManagerError(e.Message),m))
        None

let resolve (packageManager:IDependencyManagerProvider) implicitIncludeDir fileName m packageManagerTextLines =
    try
        let loadScript,additionalIncludeFolders =
            packageManager.ResolveDependencies(
                targetFramework,
                implicitIncludeDir,
                fileName,
                packageManagerTextLines)

        Some(loadScript,additionalIncludeFolders)
    with e ->
        if e.InnerException <> null then
            errorR(Error(FSComp.SR.packageManagerError(e.InnerException.Message),m))
        else
            errorR(Error(FSComp.SR.packageManagerError(e.Message),m))
        None
