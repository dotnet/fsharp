// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.DotNet.DependencyManager

open System
open System.IO
open System.Reflection
open System.Runtime.InteropServices
open Internal.Utilities.FSharpEnvironment
open Microsoft.FSharp.Reflection

[<AutoOpen>]
module ReflectionHelper =
    let dependencyManagerPattern = "*DependencyManager*.dll"

    let dependencyManagerAttributeName= "DependencyManagerAttribute"

    let resolveDependenciesMethodName = "ResolveDependencies"

    let namePropertyName = "Name"

    let keyPropertyName = "Key"

    let helpMessagesPropertyName = "HelpMessages"

    let arrEmpty = Array.empty<string>

    let seqEmpty = Seq.empty<string>

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
            let property = theType.GetProperty(propertyName, BindingFlags.Public ||| BindingFlags.NonPublic ||| BindingFlags.Instance, Unchecked.defaultof<Binder>, typeof<'treturn>, Array.empty, Array.empty)
            if isNull property then
                None
            else
                let getMethod = property.GetGetMethod()
                if not (isNull getMethod) && not (getMethod.IsStatic) then
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
open RidHelpers

/// Indicate the type of error to report
[<RequireQualifiedAccess>]
type ErrorReportType =
    | Warning
    | Error


type ResolvingErrorReport = delegate of ErrorReportType * int * string -> unit


(* Shape of Dependency Manager contract, resolved using reflection *)
/// The results of ResolveDependencies
type IResolveDependenciesResult =

    /// Succeded?
    abstract Success: bool

    /// The resolution output log
    abstract StdOut: string array

    /// The resolution error log (* process stderror *)
    abstract StdError: string array

    /// The resolution paths
    abstract Resolutions: string seq

    /// The source code file paths
    abstract SourceFiles: string seq

    /// The roots to package directories
    abstract Roots: string seq


[<AllowNullLiteralAttribute>]
type IDependencyManagerProvider =
    abstract Name: string
    abstract Key: string
    abstract HelpMessages: string[]
    abstract ResolveDependencies: scriptDir: string * mainScriptName: string * scriptName: string * scriptExt: string * packageManagerTextLines: string seq * tfm: string * rid: string -> IResolveDependenciesResult

type ReflectionDependencyManagerProvider(theType: Type, 
        nameProperty: PropertyInfo,
        keyProperty: PropertyInfo,
        helpMessagesProperty: PropertyInfo option,
        resolveDeps: MethodInfo option,
        resolveDepsEx: MethodInfo option,
        outputDir: string option) =

    let instance = Activator.CreateInstance(theType, [| outputDir :> obj |])
    let nameProperty = nameProperty.GetValue >> string
    let keyProperty = keyProperty.GetValue >> string
    let helpMessagesProperty =
        let toStringArray(o:obj) = o :?> string[]
        match helpMessagesProperty with
        | Some helpMessagesProperty -> helpMessagesProperty.GetValue >> toStringArray
        | None -> fun _ -> Array.empty<string>

    static member InstanceMaker (theType: System.Type, outputDir: string option) =
        match getAttributeNamed theType dependencyManagerAttributeName,
              getInstanceProperty<string> theType namePropertyName,
              getInstanceProperty<string> theType keyPropertyName,
              getInstanceProperty<string[]> theType helpMessagesPropertyName
              with
        | None, _, _, _
        | _, None, _, _
        | _, _, None, _ -> None
        | Some _, Some nameProperty, Some keyProperty, None ->
            let resolveMethod =   getInstanceMethod<bool * string list * string list> theType [| typeof<string>; typeof<string>; typeof<string>; typeof<string seq>; typeof<string> |] resolveDependenciesMethodName
            let resolveMethodEx = getInstanceMethod<bool * string list * string list> theType [| typeof<string>; typeof<string seq>; typeof<string>; typeof<string> |] resolveDependenciesMethodName
            Some (fun () -> new ReflectionDependencyManagerProvider(theType, nameProperty, keyProperty, None, resolveMethod, resolveMethodEx, outputDir) :> IDependencyManagerProvider)
        | Some _, Some nameProperty, Some keyProperty, Some helpMessagesProperty ->
            let resolveMethod =   getInstanceMethod<bool * string list * string list> theType [| typeof<string>; typeof<string>; typeof<string>; typeof<string seq>; typeof<string> |] resolveDependenciesMethodName
            let resolveMethodEx = getInstanceMethod<bool * string list * string list> theType [| typeof<string>; typeof<string seq>; typeof<string>; typeof<string> |] resolveDependenciesMethodName
            Some (fun () -> new ReflectionDependencyManagerProvider(theType, nameProperty, keyProperty, Some helpMessagesProperty, resolveMethod, resolveMethodEx, outputDir) :> IDependencyManagerProvider)

    static member MakeResultFromObject(result: obj) = {
        new IResolveDependenciesResult with
            /// Succeded?
            member _.Success =
                match getInstanceProperty<bool> (result.GetType()) "Success" with
                | None -> false
                | Some p -> p.GetValue(result) :?> bool

            /// The resolution output log
            member _.StdOut =
                match getInstanceProperty<string array> (result.GetType()) "StdOut" with
                | None -> Array.empty<string>
                | Some p -> p.GetValue(result) :?> string array

            /// The resolution error log (* process stderror *)
            member _.StdError =
                match getInstanceProperty<string array> (result.GetType()) "StdError" with
                | None -> Array.empty<string>
                | Some p -> p.GetValue(result) :?> string array

            /// The resolution paths
            member _.Resolutions =
                match getInstanceProperty<string seq> (result.GetType()) "Resolutions" with
                | None -> Seq.empty<string>
                | Some p -> p.GetValue(result) :?> string seq

            /// The source code file paths
            member _.SourceFiles =
                match getInstanceProperty<string seq> (result.GetType()) "SourceFiles" with
                | None -> Seq.empty<string>
                | Some p -> p.GetValue(result) :?> string seq

            /// The roots to package directories
            member _.Roots =
                match getInstanceProperty<string seq> (result.GetType()) "Roots" with
                | None -> Seq.empty<string>
                | Some p -> p.GetValue(result) :?> string seq
        }

    static member MakeResultFromFields(success: bool, stdOut: string array, stdError: string array, resolutions: string seq, sourceFiles: string seq, roots: string seq) = {
        new IResolveDependenciesResult with
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
        }


    interface IDependencyManagerProvider with

        /// Name of dependency Manager
        member _.Name = instance |> nameProperty

        /// Key of dependency Manager: used for #r "key: ... "   E.g nuget
        member _.Key = instance |> keyProperty

        /// Key of dependency Manager: used for #help
        member _.HelpMessages = instance |> helpMessagesProperty

        /// Resolve the dependencies for the given arguments
        member this.ResolveDependencies(scriptDir, mainScriptName, scriptName, scriptExt, packageManagerTextLines, tfm, rid): IResolveDependenciesResult =

            // The ResolveDependencies method, has two signatures, the original signaature in the variable resolveDeps and the updated signature resolveDepsEx
            // the resolve method can return values in two different tuples:
            //     (bool * string list * string list * string list)
            //     (bool * string list * string list)
            // We use reflection to get the correct method and to determine what we got back.
            let method, arguments =
                if resolveDepsEx.IsSome then
                    resolveDepsEx, [| box scriptExt; box packageManagerTextLines; box tfm; box rid |]
                elif resolveDeps.IsSome then
                    resolveDeps, [| box scriptDir; box mainScriptName; box scriptName; box packageManagerTextLines; box tfm |]
                else
                    None, [||]

            match method with
            | Some m ->
                let result = m.Invoke(instance, arguments)

                // Verify the number of arguments returned in the tuple returned by resolvedependencies, it can be:
                //     1 - object with properties
                //     3 - (bool * string list * string list)
                // Support legacy api return shape (bool, string seq, string seq) --- original paket packagemanager
                if Microsoft.FSharp.Reflection.FSharpType.IsTuple (result.GetType()) then
                    // Verify the number of arguments returned in the tuple returned by resolvedependencies, it can be:
                    //     3 - (bool * string list * string list)
                    let success, sourceFiles, packageRoots =
                        let tupleFields = result |> FSharpValue.GetTupleFields
                        match tupleFields |> Array.length with
                        | 3 -> tupleFields.[0] :?> bool, tupleFields.[1] :?> string list  |> List.toSeq, tupleFields.[2] :?> string list |> List.toSeq
                        | _ -> false, seqEmpty, seqEmpty
                    ReflectionDependencyManagerProvider.MakeResultFromFields(success, Array.empty, Array.empty, Seq.empty, sourceFiles, packageRoots)
                else
                    ReflectionDependencyManagerProvider.MakeResultFromObject(result)

            | None ->
                ReflectionDependencyManagerProvider.MakeResultFromFields(false, Array.empty, Array.empty, Seq.empty, Seq.empty, Seq.empty)


/// Provides DependencyManagement functions.
/// Class is IDisposable
type DependencyProvider (assemblyProbingPaths: AssemblyResolutionProbe, nativeProbingRoots: NativeResolutionProbe) =

    let dllResolveHandler = new NativeDllResolveHandler(nativeProbingRoots) :> IDisposable

    let assemblyResolveHandler = new AssemblyResolveHandler(assemblyProbingPaths) :> IDisposable

    // Resolution Path = Location of FSharp.Compiler.Private.dll
    let assemblySearchPaths = lazy (
        [
            let assemblyLocation = typeof<IDependencyManagerProvider>.GetTypeInfo().Assembly.Location
            yield Path.GetDirectoryName assemblyLocation
            yield AppDomain.CurrentDomain.BaseDirectory
        ])

    let enumerateDependencyManagerAssemblies compilerTools (reportError: ResolvingErrorReport) =
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
                let n, m = DependencyManager.SR.couldNotLoadDependencyManagerExtension(path,e.Message)
                reportError.Invoke(ErrorReportType.Warning, n, m)
                None)
        |> Seq.filter (fun a -> assemblyHasAttribute a dependencyManagerAttributeName)

    let mutable registeredDependencyManagers: Map<string, IDependencyManagerProvider> option= None

    let RegisteredDependencyManagers (compilerTools: string seq) (outputDir: string option) (reportError: ResolvingErrorReport) =
        match registeredDependencyManagers with
        | Some managers ->
            managers
        | None ->
            let managers =
                let defaultProviders = []

                let loadedProviders =
                    enumerateDependencyManagerAssemblies compilerTools reportError
                    |> Seq.collect (fun a -> a.GetTypes())
                    |> Seq.choose (fun t -> ReflectionDependencyManagerProvider.InstanceMaker(t, outputDir))
                    |> Seq.map (fun maker -> maker ())

                defaultProviders
                |> Seq.append loadedProviders
                |> Seq.map (fun pm -> pm.Key, pm)
                |> Map.ofSeq

            registeredDependencyManagers <-
                if managers.Count > 0 then
                    Some managers
                else
                    None
            managers

    /// Returns a formatted error message for the host to presentconstruct with just nativeProbing handler
    new (nativeProbingRoots: NativeResolutionProbe) =
        new DependencyProvider(Unchecked.defaultof<AssemblyResolutionProbe>, nativeProbingRoots)

    /// Returns a formatted help messages for registered dependencymanagers for the host to present
    member _.GetRegisteredDependencyManagerHelpText (compilerTools, outputDir, errorReport) = [|
            let managers = RegisteredDependencyManagers compilerTools (Option.ofString outputDir) errorReport
            for kvp in managers do
                let dm = kvp.Value
                yield! dm.HelpMessages
        |]
    /// Returns a formatted error message for the host to present
    member _.CreatePackageManagerUnknownError (compilerTools: string seq, outputDir: string, packageManagerKey: string, reportError: ResolvingErrorReport) =
        let registeredKeys = String.Join(", ", RegisteredDependencyManagers compilerTools (Option.ofString outputDir) reportError |> Seq.map (fun kv -> kv.Value.Key))
        let searchPaths = assemblySearchPaths.Force()
        DependencyManager.SR.packageManagerUnknown(packageManagerKey, String.Join(", ", searchPaths, compilerTools), registeredKeys)

    /// Fetch a dependencymanager that supports a specific key
    member this.TryFindDependencyManagerInPath (compilerTools: string seq, outputDir: string, reportError: ResolvingErrorReport, path: string): string * IDependencyManagerProvider =
        try
            if path.Contains ":" && not (Path.IsPathRooted path) then
                let managers = RegisteredDependencyManagers compilerTools (Option.ofString outputDir) reportError

                match managers |> Seq.tryFind (fun kv -> path.StartsWith(kv.Value.Key + ":" )) with
                | None ->
                    let err, msg = this.CreatePackageManagerUnknownError(compilerTools, outputDir, (path.Split(':').[0]), reportError)
                    reportError.Invoke(ErrorReportType.Error, err, msg)
                    null, Unchecked.defaultof<IDependencyManagerProvider>

                | Some kv -> path, kv.Value
            else
                path, Unchecked.defaultof<IDependencyManagerProvider>
        with 
        | e ->
            let e = stripTieWrapper e
            let err, msg = DependencyManager.SR.packageManagerError(e.Message)
            reportError.Invoke(ErrorReportType.Error, err, msg)
            null, Unchecked.defaultof<IDependencyManagerProvider>

    /// Remove the dependency mager with the specified key
    member _.RemoveDependencyManagerKey(packageManagerKey:string, path:string): string =

        path.Substring(packageManagerKey.Length + 1).Trim()

    /// Fetch a dependencymanager that supports a specific key
    member _.TryFindDependencyManagerByKey (compilerTools: string seq, outputDir: string, reportError: ResolvingErrorReport, key: string): IDependencyManagerProvider =

        try
            RegisteredDependencyManagers compilerTools (Option.ofString outputDir) reportError
            |> Map.tryFind key
            |> Option.defaultValue Unchecked.defaultof<IDependencyManagerProvider>

        with
        | e ->
            let e = stripTieWrapper e
            let err, msg = DependencyManager.SR.packageManagerError(e.Message)
            reportError.Invoke(ErrorReportType.Error, err, msg)
            Unchecked.defaultof<IDependencyManagerProvider>

    /// Resolve reference for a list of package manager lines
    member _.Resolve (packageManager:IDependencyManagerProvider,
                       scriptExt: string,
                       packageManagerTextLines: string seq,
                       reportError: ResolvingErrorReport,
                       executionTfm: string,
                       [<Optional;DefaultParameterValue(null:string)>]executionRid: string,
                       [<Optional;DefaultParameterValue("")>]implicitIncludeDir: string,
                       [<Optional;DefaultParameterValue("")>]mainScriptName: string,
                       [<Optional;DefaultParameterValue("")>]fileName: string): IResolveDependenciesResult =

        try
            let executionRid =
                if isNull executionRid then
                    RidHelpers.platformRid
                else
                    executionRid
            packageManager.ResolveDependencies(implicitIncludeDir, mainScriptName, fileName, scriptExt, packageManagerTextLines, executionTfm, executionRid)

        with e ->
            let e = stripTieWrapper e
            let err, msg = (DependencyManager.SR.packageManagerError(e.Message))
            reportError.Invoke(ErrorReportType.Error, err, msg)
            ReflectionDependencyManagerProvider.MakeResultFromFields(false, arrEmpty, arrEmpty, seqEmpty, seqEmpty, seqEmpty)

    interface IDisposable with

        member _.Dispose() =

            // Unregister everything
            registeredDependencyManagers <- None
            dllResolveHandler.Dispose()
            assemblyResolveHandler.Dispose()
