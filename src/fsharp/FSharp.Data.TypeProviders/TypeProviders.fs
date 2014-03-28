// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Data.TypeProviders.DesignTime
#nowarn "57"

open System
open System.IO
open System.Data
open System.Data.Sql
open System.Data.SqlClient
open System.Collections.Generic
open System.Reflection
open Microsoft.FSharp.Core.CompilerServices
open Microsoft.FSharp.Quotations
open Internal.Utilities.TypeProvider.Emit
open Microsoft.FSharp.Data.TypeProviders.Utility

// This is how the compiler decides the assembly has a type which extend the compiler.
[<assembly:TypeProviderAssembly>]
do()

type internal WatchSpec = 
    private 
    | WatchSpec of string * (string -> bool)
    static member File path = WatchSpec (path, fun _ -> true)
    static member FileAndFilter (path, filter) = WatchSpec (path, filter)
    member x.Path = let (WatchSpec(p, _)) = x in p
    member x.Filter = let (WatchSpec(_, f)) = x in f

/// Generate an assembly or cache an error. Do not retry the assembly generation until invalidation occurs.
type internal AssemblyOrPersistentErrorCache<'Key, 'Value when 'Key : equality>() =
        
    let cache = new Dictionary<(string list * 'Key), Choice<'Value,exn>>(HashIdentity.Structural) 
    member this.GetAssembly(typePath, moniker : 'Key, typeBuilder) =
        let res = 
            match cache.TryGetValue ((typePath,moniker)) with
            |   false, _ ->
                    let t = try Choice1Of2(typeBuilder()) with err -> Choice2Of2 err
                    cache.Add((typePath,moniker), t)
                    t
            |   true, t -> t
        match res with 
        | Choice1Of2 res -> res
        | Choice2Of2 err -> 
            // We continue to reraise errors until this type provider goes away.
            raise err

type internal NamespacePathComponent = string 
type internal TypesWithNamespacePaths = (NamespacePathComponent list * Type) list

type internal NameGenerator(prefix) = 
    let mutable n = 0
    member __.NextName() = 
       n <- n + 1
       prefix + string n

type internal NoData = | NoData

module internal Expr = 
    /// This helper makes working with Expr.Let a little easier and safer
    let LetVar(varName, expr:Expr, f) =  
        let var = Var(varName, expr.Type)
        Expr.Let(var, expr, f (Expr.Var var))
    

    let SimpleTryWith(i, body, catch) = 
        let filterVar = Var("_f" + i, typeof<System.ArgumentException>)
        let catchVar = Var("_f" + i, typeof<System.ArgumentException>)
        Expr.TryWith(
            body,
            filterVar,
            Expr.Value 1,
            catchVar,
            catch catchVar
            )

    // returns expression that raises given exception. Result expression will have specified with specified result type
    let Raise = 
        let m = typedefof<list<_>>.Assembly.GetType("Microsoft.FSharp.Core.Operators").GetMethod("Raise")
        fun (exnExpr, resultType) -> Expr.Call(m.MakeGenericMethod [|resultType|], [exnExpr])

/// helper type to isolate manipulation with paths
type internal PathResolutionUtils(baseResolutionFolder) = 

    /// if pathName is NullOrEmpty = returns pathName
    /// if pathName is absolute path - returns pathName
    /// else returns makes absolute path from resolutionFolderParam and pathName, using resolutionFolderParam as base part ()
    member this.MakeAbsolute(resolutionFolderParam, pathName) = 
        if String.IsNullOrWhiteSpace pathName then pathName 
        elif Path.IsPathRooted pathName then
            System.IO.Path.GetFullPath pathName
        else
            assert (Path.IsPathRooted resolutionFolderParam)
            System.IO.Path.GetFullPath(Path.Combine(resolutionFolderParam, pathName))

    // All file names and directory names are resolved relative to the ResolutionFolder.
    // This function should contain the only two direct uses of config.ResolutionFolder.
    member this.GetAbsoluteDesignTimeDirectory(resolutionFolderParam) =  
        if String.IsNullOrWhiteSpace resolutionFolderParam then 
            baseResolutionFolder
        else 
            this.MakeAbsolute (baseResolutionFolder, resolutionFolderParam)
    
    /// helper that combines getAbsoluteDesignTimeDirectory and makeAbsolute
    member this.MakeAbsoluteWithResolutionFolder (resolutionFolderParam, pathName) = 
        let absoluteDesignTimeDirectory = this.GetAbsoluteDesignTimeDirectory(resolutionFolderParam)
        this.MakeAbsolute(absoluteDesignTimeDirectory,pathName)

    // returns list that contains zero or one WatchSpec for configuration file
    // customConfigFile - can be either absolute or relative path. 
    // Relative paths are resolved using 'getAbsoluteDesignTimeDirectory resolutionFolder => makeAbsolute' pair
    // NOTE: here we can use option instead of list but list provides more convinient syntax on the calling side
    //      yield! r 
    // VS 
    //      match r with Some r -> yield r | None -> ()
    member this.GetConfigFileWatchSpec (resolutionFolder : string, customConfigFile : string, connectionStringName : string) =
        let designTimeDirectory = this.GetAbsoluteDesignTimeDirectory(resolutionFolder)
        assert (Path.IsPathRooted designTimeDirectory)

        let customConfigFile = this.MakeAbsolute(designTimeDirectory, customConfigFile)
        assert (String.IsNullOrEmpty(customConfigFile) || Path.IsPathRooted customConfigFile)

        if not (String.IsNullOrWhiteSpace connectionStringName) then
            let searchResult = ConfigFiles.findConfigFile (designTimeDirectory, customConfigFile)
            match searchResult with
            | ConfigFiles.StandardFound path
            | ConfigFiles.CustomFound path ->
                // configuration file exists
                // setup watcher and trigger invalidation only if changes in config file modify components of the connection string
                let currentConnString = ConfigFiles.tryReadConnectionString(path, connectionStringName)
                let filter = 
                    match currentConnString with
                    | ConfigFiles.ConnectionStringReadResult.Ok ccs ->
                        // configuration file contains valid value for connection string with specified name
                        let actualFilter p = 
                            let newConnString = ConfigFiles.tryReadConnectionString(p, connectionStringName)
                            match newConnString with
                            | ConfigFiles.ConnectionStringReadResult.Ok ncs ->
                                // connection string exists in both versions of config file
                                // compare components for two versions of connection strings to make final decision
                                ccs.ProviderName <> ncs.ProviderName
                                || ccs.Name <> ncs.Name
                                || ccs.ConnectionString <> ncs.ConnectionString
                            | ConfigFiles.ConnectionStringReadResult.NotFound -> 
                                // connection string disappeared in the second version of config file
                                // probably user has erased it - trigger invalidation
                                true 
                            | ConfigFiles.ConnectionStringReadResult.Error _ -> 
                                // error occured while reading config - exact reason is unknown so do not trigger invalidation
                                false

                        actualFilter
                    | _ ->
                        // configuration file either has incorrect structure or does not contain connection string with specified name
                        let actualFilter p = 
                            let newConnString = ConfigFiles.tryReadConnectionString(p, connectionStringName)
                            match newConnString with
                            | ConfigFiles.ConnectionStringReadResult.Ok _-> true // connection string appeared! - trigger invalidation
                            | _ -> 
                                // if we got here this means that connection string either not appeared or config file has error
                                // no reason to trigger invalidation
                                false 
                        actualFilter
                [WatchSpec.FileAndFilter(path, filter)]               
            | x ->
                // if user supplied file name for custom configuration file - use it as mask
                // otherwise use '*.config' to capture app.config or web.config
                let configFileMask = 
                    match x with
                    | ConfigFiles.CustomNotFound _ -> customConfigFile
                    | _ -> Path.Combine(designTimeDirectory, "*.config")

                // configuration file doesn't exists
                // setup watcher and trigger invalidation only if file contains connection string with given name
                // file can be created by
                // - copying it from another location - FSW will raise Created\Changed
                // - creating it from the scratch - FSW will raise Created and Changed on the first save
                // in both cases we will react on Changed
                let filter path =
                    let connString = ConfigFiles.tryReadConnectionString(path, connectionStringName)
                    match connString with
                    | ConfigFiles.ConnectionStringReadResult.Ok _ -> true
                    | _ -> false

                [WatchSpec.FileAndFilter (configFileMask, filter)]            
        else
            // connection string name not specified - no need to setup FileSystemWatcher for config file
            []

[<TypeProvider>]
/// The implementation of the built-in type providers.
type public DataProviders(config:TypeProviderConfig) =    
    let pathResolutionUtils = PathResolutionUtils(config.ResolutionFolder)

    let makeAbsolute resolutionFolderParam pathName = pathResolutionUtils.MakeAbsolute(resolutionFolderParam, pathName)

    // All file names and directory names are resolved relative to the ResolutionFolder.
    // This function should contain the only two direct uses of config.ResolutionFolder.
    let getAbsoluteDesignTimeDirectory resolutionFolderParam =  pathResolutionUtils.GetAbsoluteDesignTimeDirectory(resolutionFolderParam)
    
    /// helper that combines getAbsoluteDesignTimeDirectory and makeAbsolute
    let makeAbsoluteWithResolutionFolder resolutionFolderParam pathName = pathResolutionUtils.MakeAbsoluteWithResolutionFolder(resolutionFolderParam, pathName)

    let mkDefaultRuntimeResolutionFolderExpr resolutionFolderParam =  
        if config.IsHostedExecution then 
            Expr.Value (getAbsoluteDesignTimeDirectory resolutionFolderParam)
        else
            // the base directory always ends with a "\", remove it
            <@@ System.AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\', '/') @@>

    let mkAbsoluteFileNameExpr (resolutionFolderParam, staticFileName:string) =  
        if System.IO.Path.IsPathRooted staticFileName then 
            Expr.Value staticFileName
        else
            let baseDirectoryExpr = mkDefaultRuntimeResolutionFolderExpr resolutionFolderParam
            Expr.Call(typeof<System.IO.Path>.GetMethod("Combine",[| typeof<string>; typeof<string> |]), [ baseDirectoryExpr; Expr.Value staticFileName ])

    let theAssembly = typeof<DataProviders>.Assembly
    let namespaceName = "Microsoft.FSharp.Data.TypeProviders"

    // checks if target system runtime contains .net 4.5 specific type
    let isTargetingDotNet45 = lazy config.SystemRuntimeContainsType "System.Collections.Generic.IReadOnlyList`1"

    // The set of active file watchers
    let mutable disposals = ResizeArray<(unit -> unit)>()

    let entityNamespaceGenerator =  NameGenerator "SqlEntityConnection" 
    let serviceNamespaceGenerator =  NameGenerator "WsdlService" 

    // The invalidation signal.
    //
    // We trigger invalidation at most once. The host compiler should throw away the
    // provider instance once invalidation is signalled.
    let invalidation = new Event<System.EventHandler, _>()
    let mutable invalidationTriggered = 0
    let invalidate() = 
        // FSW can run callbacks in multiple threads - actual event should be raised at most once
        if System.Threading.Interlocked.CompareExchange(&invalidationTriggered, 1, 0) = 0 then
            invalidation.Trigger(null, EventArgs())

    
    // returns list that contains zero or one WatchSpec for configuration file
    // customConfigFile - can be either absolute or relative path. 
    // Relative paths are resolved using 'getAbsoluteDesignTimeDirectory resolutionFolder => makeAbsolute' pair
    // NOTE: here we can use option instead of list but list provides more convinient syntax on the calling side
    //      yield! r 
    // VS 
    //      match r with Some r -> yield r | None -> ()
    let getConfigFileWatchSpec (resolutionFolderParam : string, customConfigFile : string, connectionStringName : string) = 
        pathResolutionUtils.GetConfigFileWatchSpec(resolutionFolderParam, customConfigFile, connectionStringName)

    let watchPath (spec : WatchSpec) =

        let watcher = 
            let folder = Path.GetDirectoryName spec.Path
            let file = Path.GetFileName spec.Path
            new FileSystemWatcher (folder, file)
        
        watcher.Changed.Add (fun f -> 
            if spec.Filter(f.FullPath) then invalidate()
            )
        watcher.Deleted.Add(fun _ -> invalidate())
        watcher.Renamed.Add(fun _ -> invalidate())

        watcher.EnableRaisingEvents <- true
        disposals.Add(fun () -> watcher.Dispose()) 

    // Re-typecheck when the network becomes available
    let mutable watchingForNetworkUp = false
    let watchForNetworkUp () = 
        if not watchingForNetworkUp then 
            watchingForNetworkUp <- true
            let handler = System.Net.NetworkInformation.NetworkAvailabilityChangedEventHandler (fun _ evArgs -> if evArgs.IsAvailable then invalidate()) 
            System.Net.NetworkInformation.NetworkChange.NetworkAvailabilityChanged.AddHandler handler
            disposals.Add (fun () -> System.Net.NetworkInformation.NetworkChange.NetworkAvailabilityChanged.RemoveHandler handler)

    /// A utility function to bucket items hierarchically.
    static let bucketByPath (keyf  : NamespacePathComponent -> string) nodef tipf (items: TypesWithNamespacePaths) = 
        // Find all the items with an empty key list and call 'tipf' 
        let tips = 
            [ for (path,v) in items do 
                  match path with 
                  | [] -> yield tipf v
                  | _ -> () ]

        // Find all the items with a non-empty key list. Bucket them together by
        // the first key. For each bucket, call 'nodef' on that head key and the bucket.
        let nodes = 
            let buckets = new Dictionary<NamespacePathComponent,_>(10, HashIdentity.FromFunctions (fun k -> hash (keyf k)) (fun k1 k2 -> keyf k1 = keyf k2))
            for (path,v) in items do
                match path with 
                | [] -> ()
                | pathItem::rest -> 
                    buckets.[pathItem] <- (rest,v) :: (if buckets.ContainsKey pathItem then buckets.[pathItem] else []);

            [ for (KeyValue(key,items)) in buckets -> nodef key items ]

        tips @ nodes

    let assemblyBytesTable = Dictionary<System.Reflection.Assembly, byte[]>()
              
    // This major routine creates a lookaside table for generated assemblies based on 
    // particular configuration of one of the assembly generators.
    let containerTypeForGeneratedAssembly 
             (assemblyCache: AssemblyOrPersistentErrorCache<'ConfigInfo, (Assembly * 'ReorgInfo)>, 
              // Gets the file name(s) from the configuration info of a file to watch, if any              
              getFilesToWatch: 'ConfigInfo -> WatchSpec list,
              // Gets filename that will be used as global synchronization moniker
              getLockMonikerFileName : 'ConfigInfo -> string option,
              // Generates the assembly given the configuration info, and a unique name, and the file name
              generator : string -> 'ConfigInfo -> Util.FileResource * 'ReorgInfo, 
              // The XmlDoc to display for the container type
              xmlDocHelp : unit -> string, 
              // reorganizes the raw generated types from the assembly into nested types + some new GetDataContext methods
              reorganize : (string -> 'ReorgInfo -> 'ConfigInfo -> Type list -> Type list * MemberInfo list) option, 
              // Is type relocation being suppressed?
              suppressTypeRelocation, 
              // Get a unique name to use for generated bits and pieces
              getUniqueName : 'ConfigInfo -> string) =

        Util.MemoizationTable<string list * 'ConfigInfo, Type>(fun (typePath, itemSpec) -> 
            let uniqueName = getUniqueName itemSpec
            let typeName = List.nth typePath (List.length typePath - 1)
            let t = ProvidedTypeDefinition(theAssembly, uniqueName, typeName, baseType = Some typeof<obj>, IsErased=false)
            t.AddXmlDocDelayed (fun () -> xmlDocHelp())

            if suppressTypeRelocation then 
                t.SuppressRelocation <- true 
            let assembly, reorgInfo = 
                let lockMonikerFileNameOpt = getLockMonikerFileName itemSpec
                Util.WithExclusiveAccessToFile (defaultArg lockMonikerFileNameOpt null) <| fun () ->               
                    try
                        assemblyCache.GetAssembly(typePath, itemSpec, (fun () -> 
                            let assemblyFile, reorgInfo = generator uniqueName itemSpec
                            try 
                                let assemblyBytes = File.ReadAllBytes assemblyFile.Path
                                let assembly = Assembly.Load(assemblyBytes,null,System.Security.SecurityContextSource.CurrentAppDomain)
                                assemblyBytesTable.Add(assembly, assemblyBytes)
                                assembly, reorgInfo
                            finally
                                (try File.Delete assemblyFile.Path with _ -> ())
                            ))
                    finally
                        // set up FileSystemWatcher even if exception was raised 
                        // this allows to handle case when user edits LocalSchemaFile manually and intermediate result file has invalid structure
                        // we'll capture next change event and try to refresh TP
                        match getFilesToWatch itemSpec with 
                        | [] -> watchForNetworkUp()
                        | (watchSpecs: WatchSpec list) -> 
                            for watchSpec in watchSpecs do 
                                watchPath watchSpec

            let assemblyTypes : TypesWithNamespacePaths = 
                [ for ty in assembly.GetTypes() do 
                      if not ty.IsNested then
                            let namespaceParts = 
                                match ty.Namespace with 
                                | null -> [] 
                                | s ->  s.Split '.' 
                                        |> Array.toList
                            yield namespaceParts,  ty ]


            let rec loop types = 
                types 
                |> bucketByPath
                    (fun namespaceComponent -> namespaceComponent)
                    (fun namespaceComponent typesUnderNamespaceComponent -> 
                        let t = ProvidedTypeDefinition(namespaceComponent, baseType = Some typeof<obj>, IsErased=false)
                        //match help with None -> () | Some f -> t.AddXmlDocDelayed f
                        t.AddMembers (loop typesUnderNamespaceComponent)
                        (t :> Type))
                    (fun ty -> ty)
            let assemblyTypesAsNestedTypes = loop assemblyTypes

            let assemblyTypesReorganized, otherMembersAfterReorg = 
                match reorganize with 
                | None -> assemblyTypesAsNestedTypes, [] 
                | Some f -> f uniqueName reorgInfo itemSpec assemblyTypesAsNestedTypes

            t.AddMembers assemblyTypesReorganized
            t.AddMembers otherMembersAfterReorg
            let simplifyingAssemblyFile = Util.TemporaryFile "dll"
            t.ConvertToGenerated (simplifyingAssemblyFile.Path, (fun (assembly2,providedAssemblyRepr2) -> assemblyBytesTable.Add(assembly2, providedAssemblyRepr2)))
            (t :> Type))

    /// Reorganize an SQLMetal, EntityFramework, OData or WSDL set of generated types, hiding the "complex" generated API under "Types"
    /// and presenting a simpler set of types and properties.
    let dataSpaceReorg (// This determines is a type is a specific context type
                        isCtxtType, 
                        // gets the information for the GetDataContext static methods for each context type
                        getInfoOnContextType,
                        // the name to use for the single GetDataContext static method, if there is only one, normally "GetDataContext"
                        singluarStaticMethodNameOpt, 
                        // A string to put into XMLDoc help indicating the kind of the service, e.g. "OData Service"
                        serviceKind, 
                        // This predicate determines if we duplicte a method from the full context type to the simple context type
                        keepMethod, 
                        // This predicate determines if we duplicte a property from the full context type to the simple context type
                        keepProperty,
                        // This represents an additional set of properties to keep from the base type
                        propertiesToKeep:IDictionary<string,string>, 
                        // This is the set of types to reorganize
                        types: Type list,
                        // This indicates that the context type implements IDisposable and that the simplified context type should do that too
                        contextTypeImplementsIDisposable: bool) = 

        //for c in types do 
        //   printfn "c.Name = '%s', c.BaseType.FullName = '%s'" c.Name c.BaseType.FullName
        
        let contextTypes = types  |> List.filter isCtxtType
        //let contextBaseTypeShortName = dataContextTypeName.Split '.' |> Seq.last

        let simpleContextTypesContainer = 
            let t = ProvidedTypeDefinition("SimpleDataContextTypes", baseType = Some typeof<obj>, IsErased=false)
            t.AddXmlDocDelayed (fun () -> (FSData.SR.xmlDocContainsTheSimplifiedContextTypes(serviceKind)))
            t

        let fullServiceTypesType = 
            let help = (fun () -> 
                 let xmlHelpMaker = if propertiesToKeep.ContainsKey "Credentials" then FSData.SR.xmlDocFullServiceTypesAPI else FSData.SR.xmlDocFullServiceTypesAPINoCredentials
                 xmlHelpMaker(serviceKind,(String.concat ", " (contextTypes |> List.map (fun t -> "'"+t.Name+"'")))))
            //[ for (namespacePath, ty) in types -> (("ServiceTypes",help) :: namespacePath, ty) ] 
            let t = ProvidedTypeDefinition("ServiceTypes", baseType = Some typeof<obj>, IsErased=false)
            t.AddXmlDocDelayed help
            t.AddMembers types
            t.AddMember simpleContextTypesContainer
            t

        let simpleMembers = 
            [ for fullContextType in contextTypes do 
                  let storedContextType, revealedContextType, staticMethodsForContextType = getInfoOnContextType fullContextType 
                  let simpleContextType, simpleContextTypeCtor = 
                      let t = ProvidedTypeDefinition(fullContextType.Name, baseType = Some typeof<obj>, IsErased=false)
                      t.AddXmlDocDelayed (fun () -> (FSData.SR.xmlDocSimplifiedDataContext(serviceKind)))
                      t.HideObjectMethods <- true
                      // Generated provided types made using TypeProviderEmit can have one constructor, which implies the fields of the constructed type instance
                      let ctor = 
                          let code (_args: Expr list) = Expr.Value "unused" // For generated types, the constructor code is ignored.
                          let p = ProvidedConstructor([ ProvidedParameter("context", storedContextType) ], InvokeCode=code) 
                          p.AddXmlDocDelayed (fun () -> (FSData.SR.xmlDocConstructSimplifiedContext(serviceKind)))
                          p
                      if contextTypeImplementsIDisposable then 
                          let idisposeMeth = typeof<System.IDisposable>.GetMethod "Dispose"
                          let dispose = 
                              let code (_args: Expr list) = Expr.Call(Expr.Coerce( Expr.Var(Var.Global("context", storedContextType)), typeof<System.IDisposable>), idisposeMeth, [ ])
                              let m = ProvidedMethod("Dispose", [ ], typeof<System.Void>, InvokeCode=code) 
                              m.SetMethodAttrs(MethodAttributes.Virtual ||| MethodAttributes.HasSecurity ||| MethodAttributes.Final ||| MethodAttributes.NewSlot ||| MethodAttributes.Private)
                              m.AddXmlDocDelayed (fun () -> (FSData.SR.xmlDocDisposeSimplifiedContext()))
                              m
                          t.AddInterfaceImplementation typeof<System.IDisposable>
                          t.DefineMethodOverride(dispose, idisposeMeth)
                          t.AddMembers [ (dispose :> MemberInfo) ]

                      t.AddMember ctor 
                      let dataContextProperty = 
                          let code (_args: Expr list) = Expr.Coerce(Expr.Var(Var.Global("context", storedContextType)), revealedContextType)
                          let p = ProvidedProperty("DataContext", revealedContextType, GetterCode= code, IsStatic=false) 
                          p.AddXmlDocDelayed (fun () -> (FSData.SR.xmlDocGetFullContext(serviceKind)))
                          p
                      t.AddMember dataContextProperty 

                      t.AddMembers [ 
                          for serviceTypeMethod in storedContextType.GetMethods(BindingFlags.DeclaredOnly ||| BindingFlags.Instance ||| BindingFlags.Public) do 
                              if keepMethod serviceTypeMethod then 
                                  let delegatingMethod = 
                                      let code (args: Quotations.Expr[]) = 
                                          let obj = Quotations.Expr.Call(args.[0],dataContextProperty.GetGetMethod(),[ ])
                                          let objCoerced = Quotations.Expr.Coerce(obj,storedContextType) 
                                          Quotations.Expr.Call(objCoerced,serviceTypeMethod,Array.toList args.[1..]) // only instance methods
                                      let parameters = [ for p in serviceTypeMethod.GetParameters() -> ProvidedParameter(p.Name, p.ParameterType, isOut=p.IsOut) ]

                                      let m = ProvidedMethod(serviceTypeMethod.Name, parameters, serviceTypeMethod.ReturnType , InvokeCodeInternal = code, IsStaticMethod=false) 
                                      m.AddXmlDocDelayed (fun () -> (FSData.SR.xmlDocExecuteProcedure(m.Name)))
                                      m
                                  yield (delegatingMethod :> MemberInfo)

                          for serviceTypeProperty in storedContextType.GetProperties(BindingFlags.DeclaredOnly ||| BindingFlags.Instance ||| BindingFlags.Public) do 
                              if keepProperty serviceTypeProperty then 
                                  let delegatingProperty = 
                                      let code (args: Expr list) = 
                                          let obj = Expr.Call(args.[0],dataContextProperty.GetGetMethod(),[])
                                          let objCoerced = Expr.Coerce(obj,storedContextType)
                                          Expr.PropertyGet(objCoerced,serviceTypeProperty,[])  // only instance properties
                                      let p = ProvidedProperty(serviceTypeProperty.Name, serviceTypeProperty.PropertyType , GetterCode= code, IsStatic=false) 
                                      p.AddXmlDocDelayed (fun () -> (FSData.SR.xmlDocGetEntities(p.Name,serviceKind)))
                                      p
                                  yield (delegatingProperty :> MemberInfo)

                          for serviceTypeProperty in fullContextType.GetProperties(BindingFlags.Instance ||| BindingFlags.Public) do 
                              if propertiesToKeep.ContainsKey serviceTypeProperty.Name then 
                                  let keptProperty = 
                                      let p = ProvidedProperty(serviceTypeProperty.Name, serviceTypeProperty.PropertyType, IsStatic=false) 
                                      if serviceTypeProperty.CanRead then p.GetterCode <- (fun (args: Expr list) -> Expr.PropertyGet(Expr.Coerce(Expr.Call(args.[0],dataContextProperty.GetGetMethod(),[]),fullContextType),serviceTypeProperty,[])) 
                                      if serviceTypeProperty.CanWrite then p.SetterCode <- (fun (args: Expr list) -> Expr.PropertySet(Expr.Coerce(Expr.Call(args.[0],dataContextProperty.GetGetMethod(),[]),fullContextType),serviceTypeProperty,args.[1],[])) 
                                      p.AddXmlDocDelayed (fun () -> propertiesToKeep.[p.Name])
                                      p
                                  yield (keptProperty :> MemberInfo) ]


                      t, ctor

                  // Tuck this under the full service API 
                  simpleContextTypesContainer.AddMember simpleContextType 
                  let infoOnStaticMethods = 
                      match contextTypes,  singluarStaticMethodNameOpt with 
                      | [ _ ], Some singluarStaticMethodName -> [ for (_, argDescriptions, contextArgCode) in staticMethodsForContextType -> (singluarStaticMethodName, argDescriptions, contextArgCode) ]
                      | _ -> staticMethodsForContextType
                  for (methodName, argDescriptions, contextArgCode) in infoOnStaticMethods do // e.g. NetflixCatalog
                          let getContextMethod = 
                              let code args = Expr.NewObject(simpleContextTypeCtor, [ contextArgCode args ])
                              let p = ProvidedMethod(methodName, [ for (argName,argTy) in argDescriptions -> ProvidedParameter(argName, argTy)  ], simpleContextType, InvokeCode= code, IsStaticMethod=true)
                              p.AddXmlDocDelayed (fun () -> FSData.SR.xmlDocGetSimplifiedContext(serviceKind))
                              p
                          yield (getContextMethod :> MemberInfo) ] 

        [ (fullServiceTypesType :> Type) ], simpleMembers


    /// Define a provided static parameter with the given name, type, default value and xml documentation
    let staticParam (nm,ty,dflt: 'T option) = 
        let p = ProvidedStaticParameter(nm, ty, ?parameterDefaultValue=Option.map box dflt )
        p

    /// Define a provided type with the given name, type, default value and xml documentation
    let typeDefinition (nm, xml) = 
        let p = ProvidedTypeDefinition(theAssembly, namespaceName, nm, baseType = Some typeof<obj>, IsErased=false)
        p.AddXmlDocDelayed xml
        p


    let getRuntimeConnectionStringExpr (connectionStringParam, usingConfigFileInfo, configFileNameParam, dataDirectoryParam, resolutionFolderParam) = 
        let connectionStringExpr = 
            // Check if the ConfigFile parameter is being used
            match usingConfigFileInfo with 
            | Some (connectionStringName, configFileBaseNameOpt) -> 
                // If the ConfigFile parameter is being used, we are getting the connection 
                // string from a configuration file. In this case, we codegen the lookup of the
                // configuration file at runtime as the default configuration string, just in case the configuration 
                // has changed between compile-time and run-time.
                let configFileNameExpr = 
                    let configFileName = match configFileBaseNameOpt with Some x -> x | None -> configFileNameParam
                    mkAbsoluteFileNameExpr (resolutionFolderParam, configFileName)
                // PSEUDO: let text = System.IO.File.ReadAllText configFileName
                let textOfConfigFileExpr = Expr.Call(typeof<System.IO.File>.GetMethod("ReadAllText",[| typeof<string> |]), [ configFileNameExpr ])
                // PSEUDO: let tmpConfigFile = System.IO.Path.GetTempFileName()
                let tmpFileNameExpr = Expr.Call(typeof<System.IO.Path>.GetMethod("GetTempFileName",[| |]), [ ])
                Expr.LetVar("tmpConfigFile",tmpFileNameExpr,fun tmpFileNameExpr ->
                  Expr.Sequential(
                //  PSEUDO:  do System.IO.File.WriteAllText(tmpConfigFile.Path,text)
                    Expr.Call(typeof<System.IO.File>.GetMethod("WriteAllText",[| typeof<string>; typeof<string> |]),[ tmpFileNameExpr; textOfConfigFileExpr ]),
                    Expr.TryFinally(
                      // Read the config file as a System.Configuration.ExeConfigurationFileMap
                      // PSEUDO: let map = ExeConfigurationFileMap()
                      let mapExpr = Expr.NewObject(typeof<System.Configuration.ExeConfigurationFileMap>.GetConstructor([| |]), [ ])
                      Expr.LetVar("map", mapExpr,fun mapVarExpr ->
                        // Set the file name of the ExeConfigurationFileMap
                        // PSEUDO: do map.ExeConfigFilename <- tmpConfigFile
                        let action2 = Expr.PropertySet(mapVarExpr, typeof<System.Configuration.ExeConfigurationFileMap>.GetProperty "ExeConfigFilename", tmpFileNameExpr)
                        Expr.Sequential(action2,
                          // Open the config file as if it were a .exe.config. This also takes into account machine config on this machine.
                          // PSEUDO: let config = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
                          let configExpr = 
                              let meth = typeof<System.Configuration.ConfigurationManager>.GetMethod("OpenMappedExeConfiguration", [| typeof<System.Configuration.ExeConfigurationFileMap>; typeof<System.Configuration.ConfigurationUserLevel > |])
                              Expr.Call(meth, [ mapVarExpr; Expr.Value(System.Configuration.ConfigurationUserLevel.None, meth.GetParameters().[1].ParameterType) ])
                          Expr.LetVar("config", configExpr, fun configVarExpr ->
                              // PSEUDO: config.ConnectionStrings
                              let connStringsExpr1 = Expr.PropertyGet(configVarExpr, typeof<System.Configuration.Configuration>.GetProperty "ConnectionStrings", [])
                              // PSEUDO: config.ConnectionStrings.ConnectionStrings
                              let connStringsExpr2 = Expr.PropertyGet(connStringsExpr1, typeof<System.Configuration.ConnectionStringsSection>.GetProperty "ConnectionStrings", [])
                              // PSEUDO: config.ConnectionStrings.ConnectionStrings.[connectionStringName]
                              let connStringExpr1 = Expr.Call(connStringsExpr2, typeof<System.Configuration.ConnectionStringSettingsCollection>.GetMethod("get_Item", [| typeof<string> |]), [ Expr.Value connectionStringName ])
                              // PSEUDO: config.ConnectionStrings.ConnectionStrings.[connectionStringName].ConnectionString
                              let connStringExpr2 = Expr.PropertyGet(connStringExpr1, typeof<System.Configuration.ConnectionStringSettings>.GetProperty "ConnectionString", [])
                              connStringExpr2))),

                        Expr.Call(typeof<System.IO.File>.GetMethod("Delete",[| typeof<string> |]),[ tmpFileNameExpr ]))))
                                                   

            | None -> 
                // If the ConfigFile parameter is not being used, then just use the value of the connection string parameter
                Expr.Value connectionStringParam 
                                       
        let connectionStringExprAfterReplace = 
            // Get the value of the |DataDirectory| substitution
            let dataDirectoryExpr = 
                if String.IsNullOrWhiteSpace dataDirectoryParam then 
                    //PSEUDO: System.AppDomain.CurrentDomain.GetData("DataDirectory") ?? System.AppDomain.CurrentDomain.BaseDirectory
                    let currentDomainExpr = Expr.PropertyGet(typeof<System.AppDomain>.GetProperty "CurrentDomain",[])
                    let getDataExprAsObj = Expr.Call(currentDomainExpr,typeof<System.AppDomain>.GetMethod("GetData", [| typeof<string> |]),[ Expr.Value "DataDirectory" ])
                    let baseDirectoryExpr = mkDefaultRuntimeResolutionFolderExpr resolutionFolderParam
                    let getDataIsNotNullExpr = Expr.TypeTest(getDataExprAsObj,typeof<string>)
                    Expr.IfThenElse(getDataIsNotNullExpr, Expr.Coerce(getDataExprAsObj, typeof<string>), baseDirectoryExpr)
                else
                    //The value of dataDirectoryParam, relative to the runtime resolution folder
                    mkAbsoluteFileNameExpr (resolutionFolderParam, dataDirectoryParam)

            Expr.LetVar("dataDirectory", dataDirectoryExpr, fun dataDirectoryVarExpr ->
                // PSEUDO: connectionString.Replace("|DataDirectory|", dataDirectory)
                Expr.Call(connectionStringExpr, typeof<string>.GetMethod("Replace",[| typeof<string>; typeof<string> |] ), [ Expr.Value "|DataDirectory|"; dataDirectoryVarExpr ]))

        connectionStringExprAfterReplace


    let dbmlFileTypeHelp _ = FSData.SR.dbmlFileTypeHelp()
    let dbmlFileAssemblyOrPersistentErrorCache = new AssemblyOrPersistentErrorCache<_,_>()
    let dbmlFileTypeCache= 
        containerTypeForGeneratedAssembly 
           (dbmlFileAssemblyOrPersistentErrorCache, 
            (fun (fileName, resolutionFolderParam, _, _) -> 
                let absoluteFileName = makeAbsoluteWithResolutionFolder resolutionFolderParam  fileName
                [ WatchSpec.File absoluteFileName ]),  // Here, 'Some' indicates that there is a file name to be made absolute and to be watched
            (fun _ -> None), // no local schema file => no global lock moniker
            // The function used to generate the assembly
            (fun _ (fileName, resolutionFolderParam, contextTypeName, serializable) -> 
                let absoluteFileName = makeAbsoluteWithResolutionFolder resolutionFolderParam  fileName
                let dbmlFileContent = File.ReadAllText absoluteFileName
                let assembly = Dbml.buildType(dbmlFileContent, contextTypeName, serializable)
                assembly,NoData),
            dbmlFileTypeHelp,
            None,
            // don't suppress relocation of types, it works fine for types generated by SQL metal
            false, 
            // uniqueName
            (fun _ -> namespaceName))

    let dbmlFileType (typePath, itemSpec) = dbmlFileTypeCache.Apply (typePath, itemSpec) 
    let dbmlStaticParameters = 
        [| staticParam("File", typeof<string>, None)
           staticParam("ResolutionFolder", typeof<string>, Some "")
           staticParam("ContextTypeName", typeof<string>, Some "")
           staticParam("Serializable", typeof<bool>, Some false)
        |]

    let sqlDataConnectionReorg _ usingConfigFileInfo (connectionString, _connectionStringName, configFileNameParam, dataDirectoryParam, resolutionFolderParam, _localSchemaFile, _, _, _, _, _, _, _, _)  types = 
        dataSpaceReorg 
           ((fun ty -> ty.BaseType.FullName = "System.Data.Linq.DataContext"), 
            (fun contextType -> 
                let staticMethods = 
                    let methodName = "Get" + contextType.Name
                    [ (methodName, [], (fun _args -> 
                        let connectionStringExpr = getRuntimeConnectionStringExpr (connectionString, usingConfigFileInfo, configFileNameParam, dataDirectoryParam, resolutionFolderParam)
                        Expr.NewObject(contextType.GetConstructor [| typeof<string> |], [ connectionStringExpr ])))

                      (methodName, [("connectionString",typeof<string>)], (fun (args:Expr list) -> Expr.NewObject(contextType.GetConstructor [| typeof<string> |], [ args.[0] ]))) ]
                contextType, contextType.BaseType, staticMethods),
            Some "GetDataContext",
            (FSData.SR.sqlDataConnection()), 
            // only keep the query methods
            (fun m ->
                match m.ReturnType.Namespace, m.ReturnType.Name with
                // for SQL procedures
                | "System", "Int32"
                | "System.Data.Linq", ("ISingleResult`1"|"IMultipleResults") -> true
                // for SQL table-valued functions
                | "System.Linq", "IQueryable`1" -> true
                // for SQL scalar-valued functions
                | "System", ("Nullable`1"|"String") -> true
                | _ -> false),
            // only keep the table properties
            (fun p -> p.PropertyType.Namespace = "System.Data.Linq" && p.PropertyType.Name = "Table`1"), 
            dict [("Connection", (FSData.SR.sqlDataConnectionInfo()));
                  //("Log", "Specifies the destination to write the SQL query or command");
                  //("CommandTimeout", "Increases the time-out period for queries that would otherwise time out during the default timeout period")
                  ],
            types,
            true)

    let sqlDataConnectionTypeHelp _ = FSData.SR.sqlDataConnectionTypeHelp()
    let sqlDataConnectionAssemblyOrPersistentErrorCache = new AssemblyOrPersistentErrorCache<_,_>()
    let sqlDataConnectionTypeCache = 
        containerTypeForGeneratedAssembly 
           (sqlDataConnectionAssemblyOrPersistentErrorCache, 

            // The function used to compute which resources to watch
            (fun (_connectionString, connectionStringName, configFileNameParam, _dataDirectory, resolutionFolderParam, localSchemaFile, forceUpdate, _, _, _, _, _, _, _) -> 
                [ // Watch the local schema file for changes and make the filename absolute
                  if not (String.IsNullOrWhiteSpace localSchemaFile) && not forceUpdate then 
                      let absoluteLocalSchemaFile = makeAbsoluteWithResolutionFolder resolutionFolderParam localSchemaFile
                      yield WatchSpec.File absoluteLocalSchemaFile

                  // Watch the config file for changes 
                  yield! getConfigFileWatchSpec(resolutionFolderParam, configFileNameParam, connectionStringName) 
                ]), 
            (fun (_, _, _, _, resolutionFolderParam, localSchemaFile, _, _, _, _, _, _, _, _) -> Some (makeAbsoluteWithResolutionFolder resolutionFolderParam localSchemaFile)),
            // The function used to generate the assembly
            (fun _ (connectionString, connectionStringName, configFileNameParam, dataDirectoryParam, resolutionFolderParam, localSchemaFile, forceUpdate, flagPluralize, flagViews, flagFunctions, flagStoredProcs, flagTimeout, contextTypeName, serializable) -> 
                let absoluteDesignTimeDirectory = getAbsoluteDesignTimeDirectory resolutionFolderParam 
                let absoluteLocalSchemaFile = makeAbsolute absoluteDesignTimeDirectory localSchemaFile
                let absoluteConfigFileName = makeAbsolute absoluteDesignTimeDirectory configFileNameParam
                let absoluteDataDirectory = makeAbsolute absoluteDesignTimeDirectory dataDirectoryParam
                let assembly,info = SqlConnection.buildType (connectionString, connectionStringName, absoluteConfigFileName, absoluteDataDirectory, absoluteLocalSchemaFile, forceUpdate, flagPluralize, flagViews, flagFunctions, flagStoredProcs, flagTimeout, contextTypeName, serializable, absoluteDesignTimeDirectory)
                assembly,info),
            sqlDataConnectionTypeHelp,
            Some sqlDataConnectionReorg,
            // don't suppress relocation of types, it works fine for types generated by SQL metal
            false, 
            // unique name
            (fun _ -> namespaceName))

    let sqlDataConnectionStaticParameters = 
        [| staticParam("ConnectionString",                typeof<string>, Some "")
           staticParam("ConnectionStringName",            typeof<string>, Some "")
           staticParam("LocalSchemaFile",                 typeof<string>, Some "")
           staticParam("ForceUpdate",                     typeof<bool>,   Some true)
           staticParam("Pluralize",                       typeof<bool>,   Some false)
           staticParam("Views",                           typeof<bool>,   Some true )
           staticParam("Functions",                       typeof<bool>,   Some true )
           staticParam("ConfigFile",                      typeof<string>, Some "")
           staticParam("DataDirectory",                      typeof<string>, Some "")
           staticParam("ResolutionFolder",                typeof<string>, Some "")
           staticParam("StoredProcedures",                typeof<bool>,   Some true )
           staticParam("Timeout",                         typeof<int>,    Some 0    ) 
           staticParam("ContextTypeName",                 typeof<string>, Some ""   )
           staticParam("Serializable",                    typeof<bool>,   Some false)
        |]

    let sqlDataConnectionType (typePath, itemSpec) = sqlDataConnectionTypeCache.Apply (typePath, itemSpec) 


    let edmxFileTypeHelp _ = FSData.SR.edmxFileTypeHelp()
    let edmxFileAssemblyOrPersistentErrorCache = new AssemblyOrPersistentErrorCache<_,_>()
    let edmxFileTypeCache = 
        containerTypeForGeneratedAssembly 
           (edmxFileAssemblyOrPersistentErrorCache, 
            // The function used to compute which resources to watch
            (fun (fileName, resolutionFolderParam) -> 
                let absoluteFileName = makeAbsoluteWithResolutionFolder resolutionFolderParam fileName
                [ WatchSpec.File absoluteFileName ]),  
            (fun _ -> None), // no local schema file => no global lock moniker
            // The function used to generate the assembly
            (fun _ (fileName, resolutionFolderParam) ->
                let absoluteFileName = makeAbsoluteWithResolutionFolder resolutionFolderParam fileName
                let assembly = Edmx.buildTypeFromEdmxFile(absoluteFileName, isTargetingDotNet45.Value)
                assembly,NoData),
            edmxFileTypeHelp,
            None,
            // suppress relocation of types, a workaround for parts of bug 215150 where relocating entity framework types confuses EF
            true, 
            // unique name
            (fun (edmxFile, _) -> Path.GetFileNameWithoutExtension edmxFile))
    let edmxFileType (typePath, itemName) = edmxFileTypeCache.Apply (typePath, itemName) 
    let edmxFileStaticParameters = 
        [| staticParam("File", typeof<string>, None)
           staticParam("ResolutionFolder",                typeof<string>, Some "") |]


    // The function that reorganizes the types generated by EDMGEN.EXE, and adds the two GetDataContext methods that
    // compute the entity connection string from the relevant input pieces of information.
    let sqlEntityConnectionReorg 
           entityNamespaceName 
           // 'usingConfigFileInfo' is the info propagated from SqlEntityConnection.buildType
           usingConfigFileInfo 
           // These parameters are those given by the user. 
           (connectionStringParam, _connectionStringName, configFileNameParam, dataDirectoryParam, resolutionFolderParam, _localSchemaFile, provider, _entityContainer, _forceUpdate, _flagPluralize, _flagSuppressForeignKeyProperties) 
           // This is the set of non-nested types generated by EDMGEN.EXE
           (types: Type list)  = 

        // The EdmGen puts the context types in a namespace - undo this.
        let types = 
            [ for t in types do 
                 yield! t.GetNestedTypes()  ]

        let entityConnString = 
            sprintf "metadata=res://*/%s.csdl|res://*/%s.ssdl|res://*/%s.msl;provider=%s;provider connection string=\"Server=SQLEXPRESS\"" entityNamespaceName entityNamespaceName entityNamespaceName provider 

        let invalidConnectionStringMessage = FSData.SR.invalidConnectionString()
        let nonEquivalentConnectionStringMessage = FSData.SR.nonEquivalentConnectionString()


        dataSpaceReorg 
           ((fun ty -> ty.BaseType.FullName = "System.Data.Objects.ObjectContext"), 
            (fun contextType -> 

                // Build the expression for the entity connection string given the expression for the provider connection string.
                let mkRuntimeEntityConnectionStringExpr providerConnectionStringExpr = 
                    let contextCtor = contextType.GetConstructor [| typeof<string> |]
                    let entityConnStringBuilderType = 
                        let systemDataEntityClient = contextType.BaseType.Assembly
                        systemDataEntityClient.GetType("System.Data.EntityClient.EntityConnectionStringBuilder")
                                        
                    let ecsCtor = entityConnStringBuilderType.GetConstructor [| typeof<string> |]
                    let providerConnString = entityConnStringBuilderType.GetProperty "ProviderConnectionString"
                    let connString = entityConnStringBuilderType.GetProperty "ConnectionString"
                    let equivalentTo = entityConnStringBuilderType.GetMethod "EquivalentTo"
                                        
                    let dbProviderFactoriesType = 
                        let prop = contextType.GetProperty("Connection")
                        let systemData = prop.PropertyType.Assembly
                        systemData.GetType("System.Data.Common.DbProviderFactories")                        
                    let dbProviderGetFactoryMethod =  dbProviderFactoriesType.GetMethod("GetFactory", [| typeof<string> |])
                    let dbProviderCreateConnStringBuilderMethod = dbProviderGetFactoryMethod.ReturnType.GetMethod("CreateConnectionStringBuilder")
                    let dbConnStringBuilderConnStringProperty = dbProviderCreateConnStringBuilderMethod.ReturnType.GetProperty("ConnectionString")


                    let argExnString, argExnStringExn = 
                        let argExnType = typeof<System.ArgumentException>
                        let argExnString = argExnType.GetConstructor([|typeof<string>|])
                        let argExnStringExn = argExnType.GetConstructor([|typeof<string>; typeof<exn>|])
                        argExnString, argExnStringExn        
        

                    Expr.LetVar("provConnStrVar", providerConnectionStringExpr, fun providerConnectionStringE -> 
                      Expr.LetVar("newConnStrVar", Expr.NewObject(ecsCtor, [Expr.Value entityConnString]), fun newE -> 
                        Expr.Sequential(
                            Expr.SimpleTryWith(
                                "1",
                                Expr.LetVar("dbProviderFactory", Expr.Call(dbProviderGetFactoryMethod, [Expr.Value(provider)]), fun dbProviderFactory ->
                                    Expr.LetVar(
                                        "connStrBuilder", Expr.Call(dbProviderFactory, dbProviderCreateConnStringBuilderMethod, []), fun connStrBuilder ->
                                        Expr.Sequential(
                                            Expr.PropertySet(connStrBuilder, dbConnStringBuilderConnStringProperty, providerConnectionStringE),
                                            Expr.PropertySet(newE, providerConnString, providerConnectionStringE)
                                            )
                                        )
                                    ),
                                fun _ -> 
                                    Expr.LetVar(
                                        "intermediateConnStrVar", 
                                        Expr.SimpleTryWith(
                                            "2",
                                            Expr.NewObject(ecsCtor, [providerConnectionStringE]),
                                            fun e -> Expr.Raise(Expr.NewObject(argExnStringExn, [Expr.Value invalidConnectionStringMessage; Expr.Var e]), entityConnStringBuilderType)),
                                        fun tmpE -> 
                                          Expr.Sequential(
                                              Expr.PropertySet(newE, providerConnString, Expr.PropertyGet(tmpE, providerConnString)),
                                              Expr.IfThenElse(
                                                  Expr.Call(newE, equivalentTo, [tmpE]),
                                                  Expr.Value((), typeof<unit>),
                                                  Expr.Raise (Expr.NewObject(argExnString, [Expr.Value nonEquivalentConnectionStringMessage]), typeof<unit>)))) ),
                            Expr.NewObject(contextCtor, [ Expr.PropertyGet(newE, connString) ]))))


                let staticMethods = 
                    let methodName = "Get" + contextType.Name
                    [ (methodName, [], 
                        (fun _args -> 
                            // Compute the provider connection string from the configuration file, if necessary
                            let connectionStringExpr = getRuntimeConnectionStringExpr (connectionStringParam, usingConfigFileInfo, configFileNameParam, dataDirectoryParam, resolutionFolderParam)
                            mkRuntimeEntityConnectionStringExpr connectionStringExpr));

                      (methodName, [("providerConnectionString",typeof<string>)], 
                        (fun (args:Expr list) -> 
                            // Use the given provider configuration string
                            mkRuntimeEntityConnectionStringExpr args.[0]))
                    ]
                contextType, contextType.BaseType, staticMethods),

            Some "GetDataContext",
            (FSData.SR.sqlEntityConnection()), 
            // don't keep any methods
            (fun _m -> false),
            // only keep the query properties
            (fun p -> p.PropertyType.Namespace = "System.Data.Objects" && p.PropertyType.Name = "ObjectSet`1"), 
            dict [("Connection", (FSData.SR.connectionInfo()))],
            types,
            true)

    //let a = System.Reflection.Assembly.LoadFile @"C:\fsharp\vspro\devdiv\src\tests\fsharp\typecheck\sigs\tmp\Northwnd.ObjectLayer.dll"
    //let types = a.GetTypes()
    //types |> Array.map (fun s -> s.BaseType.FullName)

    let sqlEntityConnectionTypeHelp _ = FSData.SR.sqlEntityConnectionTypeHelp()
    let sqlEntityConnectionAssemblyOrPersistentErrorCache = new AssemblyOrPersistentErrorCache<_,_>()
    let sqlEntityConnectionTypeCache = 
        containerTypeForGeneratedAssembly 
           (sqlEntityConnectionAssemblyOrPersistentErrorCache, 

            // The function used to compute which resources to watch
            (fun (_connectionString, connectionStringName, configFileNameParam, _dataDirectory, resolutionFolderParam, localSchemaFile, _, _, forceUpdate, _, _) -> 
                [ if not (String.IsNullOrWhiteSpace localSchemaFile) && not forceUpdate then 
                      let absoluteLocalSchemaFile = makeAbsoluteWithResolutionFolder resolutionFolderParam localSchemaFile
                      yield WatchSpec.File absoluteLocalSchemaFile

                  yield! getConfigFileWatchSpec(resolutionFolderParam, configFileNameParam, connectionStringName)
                ] ), 
            (fun (_, _, _, _, resolutionFolderParam, localSchemaFile, _, _, _, _, _) -> Some (makeAbsoluteWithResolutionFolder resolutionFolderParam localSchemaFile)),
            // The function used to generate the assembly
            (fun entityNamespaceName (connectionString, connectionStringName, configFileNameParam, dataDirectoryParam, resolutionFolderParam, localSchemaFile, provider, entityContainer, forceUpdate, flagPluralize, flagSuppressForeignKeyProperties) ->
                let absoluteDesignTimeDirectory = getAbsoluteDesignTimeDirectory resolutionFolderParam 
                let absoluteLocalSchemaFile = makeAbsolute absoluteDesignTimeDirectory localSchemaFile
                let absoluteConfigFileName = makeAbsolute absoluteDesignTimeDirectory configFileNameParam
                let absoluteDataDirectory = makeAbsolute absoluteDesignTimeDirectory dataDirectoryParam
                SqlEntityConnection.buildType(entityNamespaceName, connectionString, connectionStringName, absoluteConfigFileName, absoluteDataDirectory, absoluteLocalSchemaFile, provider, entityContainer, forceUpdate, flagPluralize, flagSuppressForeignKeyProperties, absoluteDesignTimeDirectory, isTargetingDotNet45.Value)),
            sqlEntityConnectionTypeHelp,
            Some sqlEntityConnectionReorg,
            // suppress relocation of types, a workaround for parts of bug 215150 where relocating entity framework types confuses EF
            true, 
            // unique name
            (fun _ -> entityNamespaceGenerator.NextName()))
    let sqlEntityConnectionType (typePath, itemName) = sqlEntityConnectionTypeCache.Apply (typePath, itemName) 
    let sqlEntityConnectionStaticParameters = 
        [| staticParam("ConnectionString",                  typeof<string>, Some "")
           staticParam("ConnectionStringName",              typeof<string>, Some "")
           staticParam("LocalSchemaFile",                   typeof<string>, Some "")
           staticParam("Provider",                          typeof<string>, Some "System.Data.SqlClient")
           staticParam("EntityContainer",                   typeof<string>, Some "")
           staticParam("ConfigFile",                        typeof<string>, Some "")
           staticParam("DataDirectory",                        typeof<string>, Some "")
           staticParam("ResolutionFolder",                  typeof<string>, Some "")
           staticParam("ForceUpdate",                       typeof<bool>,   Some true)
           staticParam("Pluralize",                         typeof<bool>,   Some false)
           staticParam("SuppressForeignKeyProperties",      typeof<bool>,   Some false)
        |]



    let odataServiceTypeHelp _ = FSData.SR.odataServiceTypeHelp()
    let odataServiceAssemblyOrPersistentErrorCache = new AssemblyOrPersistentErrorCache<_,_>()


    let odataReorg _ NoData (serviceUri, _, _, _, _) (types: Type list) = 
        dataSpaceReorg 
           ((fun ty -> ty.BaseType.FullName = "System.Data.Services.Client.DataServiceContext"), 
            (fun contextType -> 
                let staticMethods = 
                    let methodName = "Get" + contextType.Name
                    [ (methodName, [], 
                        (fun _args -> 
                              let uri = Expr.NewObject(typeof<System.Uri>.GetConstructor [| typeof<string> |], [ Expr.Value serviceUri ]) 
                              Expr.NewObject(contextType.GetConstructor [| typeof<System.Uri> |], [ uri ]) ));
                      (methodName, [("uri",typeof<System.Uri>)], 
                        (fun (args:Expr list) ->
                              Expr.NewObject(contextType.GetConstructor [| typeof<System.Uri> |], [ args.[0] ]) )) ]
                contextType, contextType.BaseType, staticMethods ),
            Some "GetDataContext",
            "OData Service", 
            // don't keep any methods
            (fun _m -> false),
            // only keep the data source properties
            (fun p -> p.PropertyType.Namespace = "System.Data.Services.Client" && p.PropertyType.Name = "DataServiceQuery`1"),
            dict [("Credentials", (FSData.SR.odataServiceCredentialsInfo()))],
            types,
            false (* System.Data.Services.Client.DataServiceContext does not implement IDisposable - the joy of REST! *) )

    let odataServiceTypeCache =
        containerTypeForGeneratedAssembly 
           (odataServiceAssemblyOrPersistentErrorCache, 
            // The function used to compute which resources to watch
            (fun (_serviceUri, localSchemaFile, forceUpdate, resolutionFolderParam, _) -> 
                [ // Watch the local schema file for changes and make the filename absolute
                  if not (String.IsNullOrWhiteSpace localSchemaFile) && not forceUpdate then 
                      let absoluteLocalSchemaFile = makeAbsoluteWithResolutionFolder resolutionFolderParam localSchemaFile
                      yield WatchSpec.File absoluteLocalSchemaFile ]), 
            (fun (_, localSchemaFile, _, resolutionFolderParam, _) -> Some (makeAbsoluteWithResolutionFolder resolutionFolderParam localSchemaFile)),
            // The function used to generate the assembly
            (fun _ (serviceUri, localSchemaFile, forceUpdate, resolutionFolderParam, dataServiceCollection) -> 
                let absoluteLocalSchemaFile = makeAbsoluteWithResolutionFolder resolutionFolderParam localSchemaFile
                let assembly = DataSvcUtil.buildTypeFromMetadataUri (serviceUri, absoluteLocalSchemaFile, forceUpdate, dataServiceCollection)
                assembly,NoData),
            odataServiceTypeHelp,
            Some odataReorg,
            // don't suppress relocation of types, it works fine for types generated by SQL metal
            false, 
            // unique name
            (fun _ -> namespaceName))
    let odataServiceStaticParameters = 
        [| staticParam("ServiceUri",            typeof<string>, None) 
           staticParam("LocalSchemaFile",       typeof<string>, Some "")
           staticParam("ForceUpdate",           typeof<bool>,   Some true)
           staticParam("ResolutionFolder",      typeof<string>, Some "")
           staticParam("DataServiceCollection", typeof<bool>,   Some false)
        |]
    let odataServiceType (typePath, itemName) = odataServiceTypeCache.Apply (typePath, itemName)
        

    
    let wsdlReorg namespaceName endpointNames (serviceUri:string, _localSchemaDir, _, _, _, _, _, _, _) (types: Type list) = 
        /// Make the expression that creates the instance of the underlying full context type stored in the simplified context object.
        /// This function is used when no endpoint name is available (because either none were in the config file, or there was trouble getting
        /// or parsing the context file)
        let mkStoredContextWhenNoConfig (contextType:Type) uriStringExpr = 
            // call Service(binding:Binding, endpointAddress:EndpointAddress)
            // i.e. new ServiceTypeName(new System.ServiceModel.BasicHttpBinding(transportMode),System.ServiceModel.EndpointAddress(serviceUri))
            let ctor = contextType.GetConstructors() |> Array.find (fun c -> let ps = c.GetParameters() in ps.Length = 2 && ps.[0].ParameterType.Name = "Binding" &&  ps.[1].ParameterType.Name = "EndpointAddress" )
            let bindingType = ctor.GetParameters().[0].ParameterType
            let endpointType = ctor.GetParameters().[1].ParameterType
            let endpointTypeCtor = endpointType.GetConstructor [| typeof<string> |]
            let basicHttpBinding = bindingType.Assembly.GetType("System.ServiceModel.BasicHttpBinding")
            let basicHttpSecurityMode = bindingType.Assembly.GetType("System.ServiceModel.BasicHttpSecurityMode")
            let basicHttpBindingCtor = basicHttpBinding.GetConstructor [| basicHttpSecurityMode |]
            let transportMode = if (System.Uri(serviceUri).Scheme = "https") then System.ServiceModel.BasicHttpSecurityMode.Transport else System.ServiceModel.BasicHttpSecurityMode.None
            let arg1 = Expr.NewObject(basicHttpBindingCtor, [ Expr.Value(transportMode, basicHttpBindingCtor.GetParameters().[1].ParameterType )  ])
            let arg2 = Expr.NewObject(endpointTypeCtor, [ uriStringExpr ])
            let code = Expr.NewObject(ctor, [ arg1;arg2 ])
            code

        /// Make the expression that creates the instance of the underlying full context type stored in the simplified context object.
        /// This function is used when an endpoint name is available from the config file. endPointExprOpt optionally indicates
        /// a user parameter passed in to represent an explicit endpoint.
        let mkStoredContextWhenHaveConfig (storedContextType:Type) serviceInterfaceType (epName:string) endPointExprOpt = 
            // First get the config from the resource stored in the assembly.
            // PSEUDO: let assembly = typeof<storedContextType>.Assembly
            let assemblyExpr = 
               let typeOfExpr = Expr.Call(typedefof<list<_>>.Assembly.GetType("Microsoft.FSharp.Core.Operators").GetMethod("TypeOf").MakeGenericMethod [| storedContextType |], [ ])
               Expr.PropertyGet(typeOfExpr, typeOfExpr.Type.GetProperty "Assembly", [])
            //PSEUDO: typeof<System.Reflection.Assembly>.GetMethod("GetExecutingAssembly", [| |]), [])
            Expr.LetVar("assembly",assemblyExpr,fun assemblyVarExpr ->
              // Crack the config as a manifest resource.
              // PSEUDO: let stream = assembly.GetManifestResourceStream("WsdlService1.config")
              let streamExpr = Expr.Call(assemblyVarExpr, assemblyVarExpr.Type.GetMethod("GetManifestResourceStream", [| typeof<string> |]), [ Expr.Value (namespaceName+".config") ])
              Expr.LetVar("stream",streamExpr,fun streamVarExpr ->
                // Create a StreamReader for the config.
                // PSEUDO: let reader = new System.IO.StreamReader(stream)
                let readerExpr = Expr.NewObject(typeof<System.IO.StreamReader>.GetConstructor([| streamVarExpr.Type |]), [ streamVarExpr ])
                Expr.LetVar("reader",readerExpr,fun readerVarExpr ->
                  // Read the text of the config.
                  // PSEUDO: let text = reader.ReadToEnd()
                  let textExpr = Expr.Call(readerVarExpr, typeof<System.IO.StreamReader>.GetMethod("ReadToEnd", [| |]), [ ])
                  Expr.LetVar("text",textExpr,fun textVarExpr ->
                    // Create a temporary config file.
                    // PSEUDO: let tmpConfigFile = System.IO.Path.GetTempFileName()
                    let tmpConfigFileExpr = Expr.Call(typeof<System.IO.Path>.GetMethod("GetTempFileName", [| |]), [ ])
                    Expr.LetVar("tmpConfigFile",tmpConfigFileExpr,fun tmpConfigFileVarExpr ->
                      // Write the text to the temporary config file.
                      // PSEUDO: do System.IO.File.WriteAllText(tmpConfigFile,text)
                      let action1 = Expr.Call(typeof<System.IO.File>.GetMethod("WriteAllText", [| typeof<string>; typeof<string>|]), [ tmpConfigFileVarExpr; textVarExpr ])
                      Expr.Sequential(action1,
                        // Read the config file as a System.Configuration.ExeConfigurationFileMap
                        // PSEUDO: let map = ExeConfigurationFileMap()
                        let mapExpr = Expr.NewObject(typeof<System.Configuration.ExeConfigurationFileMap>.GetConstructor([| |]), [ ])
                        Expr.LetVar("map",mapExpr,fun mapVarExpr ->
                          // Set the file name of the ExeConfigurationFileMap
                          // PSEUDO: do map.ExeConfigFilename <- tmpConfigFile
                          let action2 = Expr.PropertySet(mapVarExpr, typeof<System.Configuration.ExeConfigurationFileMap>.GetProperty "ExeConfigFilename", tmpConfigFileVarExpr)
                          Expr.Sequential(action2,
                            // Open the config file as if it were a .exe.config. This also takes into account machine config on this machine.
                            // PSEUDO: let config = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
                            let configExpr = 
                               let meth = typeof<System.Configuration.ConfigurationManager>.GetMethod("OpenMappedExeConfiguration", [| typeof<System.Configuration.ExeConfigurationFileMap>; typeof<System.Configuration.ConfigurationUserLevel > |])
                               Expr.Call(meth, [ mapVarExpr; Expr.Value(System.Configuration.ConfigurationUserLevel.None, meth.GetParameters().[1].ParameterType) ])
                            Expr.LetVar("config", configExpr, fun configVarExpr ->
                              // Create a ConfigurationChannelFactory from the config. This can be used to create channels and endpoints.
                              // PSEUDO: let factory = new ConfigurationChannelFactory<Geocode.GeocodeService.ServiceTypes.IGeocodeService> ("CustomBinding_IGeocodeService", config, null);
                              let factoryExpr = 
                                  let ctor = typedefof<System.ServiceModel.Configuration.ConfigurationChannelFactory<_>>.MakeGenericType([| serviceInterfaceType |]).GetConstructors().[0]
                                  let epExpr = match endPointExprOpt with None -> Expr.Value(null, ctor.GetParameters().[2].ParameterType) | Some e -> e
                                  Expr.NewObject(ctor, [ Expr.Value epName; configVarExpr; epExpr ])
                              Expr.LetVar("factory", factoryExpr, fun factoryVarExpr ->
                                 // Get the binding and address from the endpoint of the factory.
                                 let resultExpr = 
                                     let ctor = storedContextType.GetConstructor [| typeof<System.ServiceModel.Channels.Binding>; typeof<System.ServiceModel.EndpointAddress > |]
                                     let endpointExpr = Expr.PropertyGet(factoryVarExpr, factoryVarExpr.Type.GetProperty "Endpoint",[])
                                     let bindingExpr = Expr.PropertyGet(endpointExpr, endpointExpr.Type.GetProperty "Binding", [])
                                     let addressExpr = Expr.PropertyGet(endpointExpr, endpointExpr.Type.GetProperty "Address", [])
                                     Expr.NewObject(ctor, [ bindingExpr; addressExpr ])
                                 Expr.LetVar("result", resultExpr, fun resultVarExpr ->
                                   // PSEUDO: System.IO.File.Delete(tmpConfigFile)
                                   let action3 = Expr.Call(typeof<System.IO.File>.GetMethod("Delete", [| typeof<string> |]), [ tmpConfigFileVarExpr ])
                                   Expr.Sequential(action3,
                                     resultVarExpr) )))))))))))


        dataSpaceReorg 
           ((fun ty -> ty.BaseType <> null && ty.BaseType.Namespace = "System.ServiceModel" && ty.BaseType.Name = "ClientBase`1"), 
            (fun contextType -> 
                match endpointNames with 
                | [] -> 
                    /// This indicates we either didn't get the config from the web service, or we found no entrypoint names in it
                    /// Resort to providing an HTTP endpoint
                    let staticMethods = 
                        let methodName = "GetHttp" + contextType.Name
                        [ (methodName, [], (fun _args -> mkStoredContextWhenNoConfig contextType (Expr.Value serviceUri)));
                          (methodName, [("uri", typeof<string>)], (fun (args:Expr list) -> mkStoredContextWhenNoConfig contextType args.[0])) ]
                    contextType, contextType.BaseType, staticMethods
                | _ -> 
                    let revealedContextType = contextType.BaseType 
                    let storedContextType = contextType 
                    let serviceInterfaceType = contextType.BaseType.GetGenericArguments().[0]
                    let staticMethods = 
                        [ for epName in endpointNames do
                            let methodName = "Get" + epName
                            yield (methodName, [], (fun _args -> mkStoredContextWhenHaveConfig storedContextType serviceInterfaceType epName None));
                            yield (methodName, [("remoteAddress", typeof<System.ServiceModel.EndpointAddress>)], (fun (args:Expr list) -> mkStoredContextWhenHaveConfig storedContextType serviceInterfaceType epName (Some args.[0]))) ]
                    storedContextType, revealedContextType, staticMethods),
            None,
            "WSDL Service", 
            // keep all methods except those for properties
            (fun m -> not (m.Name.StartsWith "get_") && not (m.Name.StartsWith "set_")),
            // don't keep any properties
            (fun _p -> false), 
            dict [ ],
            types,
            true)


    let wsdlServiceTypeHelp _ = FSData.SR.wsdlServiceTypeHelp()
    let wsdlServiceAssemblyOrPersistentErrorCache = new AssemblyOrPersistentErrorCache<_,_>()
    let wsdlServiceTypeCache =
        containerTypeForGeneratedAssembly 
           (wsdlServiceAssemblyOrPersistentErrorCache, 

            // The function used to compute which resources to watch
            (fun (_serviceUri, localSchemaFile, forceUpdate, resolutionFolderParam, _, _, _, _, _) -> 
                [ // Watch the local schema file for changes and make the filename absolute
                  if not (String.IsNullOrWhiteSpace localSchemaFile) && not forceUpdate then 
                      let absoluteLocalSchemaFile = makeAbsoluteWithResolutionFolder resolutionFolderParam localSchemaFile
                      yield WatchSpec.File absoluteLocalSchemaFile ]), 
            (fun (_, localSchemaFile, _, resolutionFolderParam, _, _, _, _, _) -> Some (makeAbsoluteWithResolutionFolder resolutionFolderParam localSchemaFile)),
            // The function used to generate the assembly
            (fun uniqueName (serviceUri, localSchemaFile, forceUpdate, resolutionFolderParam, messageContract, enableDataBinding, serializable, async, collectionType) -> 
                let absoluteLocalSchemaFile = makeAbsoluteWithResolutionFolder resolutionFolderParam localSchemaFile
                let assembly, endPointNames = SvcUtil.buildTypeFromWsdlUri (uniqueName, serviceUri, absoluteLocalSchemaFile, forceUpdate,  messageContract, enableDataBinding, serializable, async, collectionType)
                (assembly, endPointNames)),
            wsdlServiceTypeHelp,
            Some wsdlReorg,
            // don't suppress relocation of types, it works fine for types generated by SQL metal
            false, 
            // unique name
            (fun _ -> serviceNamespaceGenerator.NextName()))

    let wsdlServiceStaticParameters = 
        [| staticParam("ServiceUri",             typeof<string>, None) 
           staticParam("LocalSchemaFile",        typeof<string>, Some "")
           staticParam("ForceUpdate",            typeof<bool>,   Some true)
           staticParam("ResolutionFolder",       typeof<string>,   Some "")
           staticParam("MessageContract",        typeof<bool>,   Some false)
           staticParam("EnableDataBinding",      typeof<bool>,   Some false)
           staticParam("Serializable",           typeof<bool>,   Some false)
           staticParam("Async",                  typeof<bool>,   Some false)
           staticParam("CollectionType",         typeof<string>, Some "")
        |]

    let wsdlServiceType (typePath, itemName) = wsdlServiceTypeCache.Apply (typePath, itemName) 

    let wsdlServiceTypeUninstantiated   = typeDefinition("WsdlService",       wsdlServiceTypeHelp)
    let odataServiceTypeUninstantiated  = typeDefinition("ODataService",      odataServiceTypeHelp)
    let dbmlFileTypeUninstantiated      = typeDefinition("DbmlFile",          dbmlFileTypeHelp)
    let edmxFileTypeUninstantiated      = typeDefinition("EdmxFile",          edmxFileTypeHelp)
    let sqlEntityConnectionTypeUninstantiated      = typeDefinition("SqlEntityConnection",  sqlEntityConnectionTypeHelp)
    let sqlDataConnectionTypeUninstantiated = typeDefinition("SqlDataConnection", sqlDataConnectionTypeHelp)

    interface IDisposable with 
        member this.Dispose() = 
           let disposers = disposals |> Seq.toList
           disposals.Clear()
           for disposef in disposers do try disposef() with _ -> ()

    interface IProvidedNamespace with
        member this.GetNestedNamespaces() = [| |]
        member this.NamespaceName = namespaceName
        member this.GetTypes() = 
            [| sqlDataConnectionTypeUninstantiated
               edmxFileTypeUninstantiated
               sqlEntityConnectionTypeUninstantiated
               dbmlFileTypeUninstantiated
               odataServiceTypeUninstantiated
               wsdlServiceTypeUninstantiated |]
        
        member this.ResolveTypeName typeName = 
            match typeName with
            | "EdmxFile" -> edmxFileTypeUninstantiated :> Type
            | "SqlDataConnection" -> sqlDataConnectionTypeUninstantiated :> Type
            | "SqlEntityConnection" -> sqlEntityConnectionTypeUninstantiated :> Type
            | "DbmlFile" -> dbmlFileTypeUninstantiated :> Type
            | "ODataService" -> odataServiceTypeUninstantiated :> Type
            | "WsdlService" -> wsdlServiceTypeUninstantiated :> Type
            | _ -> null

    interface ITypeProvider with

        member this.GetNamespaces() = [| this |] 
        
        member this.GetStaticParameters(typeWithoutArguments) = 
            let parameters = 
                match typeWithoutArguments.Name with
                | "ODataService"  -> odataServiceStaticParameters
                | "WsdlService"   -> wsdlServiceStaticParameters 
                | "SqlDataConnection" -> sqlDataConnectionStaticParameters
                | "SqlEntityConnection" -> sqlEntityConnectionStaticParameters
                | "DbmlFile"      -> dbmlStaticParameters
                | "EdmxFile"      -> edmxFileStaticParameters
                | _               -> [| |]
            [| for p in parameters -> p :> System.Reflection.ParameterInfo |]

        member this.ApplyStaticArguments(typeWithoutArguments, typePathWithArguments, staticArguments) =

            let oneNamedParam (paramList:ProvidedStaticParameter[], name) : 'T = 
                match paramList |> Array.tryFindIndex (fun p -> p.Name = name) with
                | Some i when (i < staticArguments.Length && staticArguments.[i] :? 'T) -> unbox<'T>(staticArguments.[i])
                | _ -> failwith (FSData.SR.staticParameterNotFoundForType(name,typeWithoutArguments.Name))

            let typePath = Array.toList typePathWithArguments
            match typeWithoutArguments.Name with
            | "EdmxFile" -> 
                edmxFileType (typePath,
                             ((oneNamedParam (edmxFileStaticParameters, "File") : string),
                              (oneNamedParam (edmxFileStaticParameters, "ResolutionFolder") : string)))  
            | "SqlEntityConnection" -> 
                sqlEntityConnectionType 
                            (typePath,
                             ((oneNamedParam (sqlEntityConnectionStaticParameters, "ConnectionString") : string),
                              (oneNamedParam (sqlEntityConnectionStaticParameters, "ConnectionStringName") : string),
                              (oneNamedParam (sqlEntityConnectionStaticParameters, "ConfigFile") : string),
                              (oneNamedParam (sqlEntityConnectionStaticParameters, "DataDirectory") : string),
                              (oneNamedParam (sqlEntityConnectionStaticParameters, "ResolutionFolder") : string),
                              (oneNamedParam (sqlEntityConnectionStaticParameters, "LocalSchemaFile") : string),
                              (oneNamedParam (sqlEntityConnectionStaticParameters, "Provider") : string),
                              (oneNamedParam (sqlEntityConnectionStaticParameters, "EntityContainer") : string),
                              (oneNamedParam (sqlEntityConnectionStaticParameters, "ForceUpdate") : bool),
                              (oneNamedParam (sqlEntityConnectionStaticParameters, "Pluralize") : bool),
                              (oneNamedParam (sqlEntityConnectionStaticParameters, "SuppressForeignKeyProperties") : bool)))  
            | "SqlDataConnection" -> 
                sqlDataConnectionType (typePath,
                             ((oneNamedParam (sqlDataConnectionStaticParameters, "ConnectionString") : string),
                              (oneNamedParam (sqlDataConnectionStaticParameters, "ConnectionStringName") : string),
                              (oneNamedParam (sqlDataConnectionStaticParameters, "ConfigFile") : string),
                              (oneNamedParam (sqlDataConnectionStaticParameters, "DataDirectory") : string),
                              (oneNamedParam (sqlDataConnectionStaticParameters, "ResolutionFolder") : string),
                              (oneNamedParam (sqlDataConnectionStaticParameters, "LocalSchemaFile") : string),
                              (oneNamedParam (sqlDataConnectionStaticParameters, "ForceUpdate") : bool),
                              (oneNamedParam (sqlDataConnectionStaticParameters, "Pluralize") : bool),
                              (oneNamedParam (sqlDataConnectionStaticParameters, "Views") : bool),
                              (oneNamedParam (sqlDataConnectionStaticParameters, "Functions") : bool),
                              (oneNamedParam (sqlDataConnectionStaticParameters, "StoredProcedures") : bool),
                              (oneNamedParam (sqlDataConnectionStaticParameters, "Timeout") : int),
                              (oneNamedParam (sqlDataConnectionStaticParameters, "ContextTypeName") : string),
                              (oneNamedParam (sqlDataConnectionStaticParameters, "Serializable") : bool))) 
            | "DbmlFile" -> 
                dbmlFileType (typePath,
                             ((oneNamedParam (dbmlStaticParameters, "File") : string),
                              (oneNamedParam (dbmlStaticParameters, "ResolutionFolder") : string),
                              (oneNamedParam (dbmlStaticParameters, "ContextTypeName") : string),
                              (oneNamedParam (dbmlStaticParameters, "Serializable") : bool)))  

            | "WsdlService" -> 
                wsdlServiceType
                    (typePath,
                              ((oneNamedParam (wsdlServiceStaticParameters, "ServiceUri") : string),
                               (oneNamedParam (wsdlServiceStaticParameters, "LocalSchemaFile") : string),
                               (oneNamedParam (wsdlServiceStaticParameters, "ForceUpdate") : bool),
                               (oneNamedParam (wsdlServiceStaticParameters, "ResolutionFolder") : string),
                               (oneNamedParam (wsdlServiceStaticParameters, "MessageContract") : bool),
                               (oneNamedParam (wsdlServiceStaticParameters, "EnableDataBinding") : bool),
                               (oneNamedParam (wsdlServiceStaticParameters, "Serializable") : bool),
                               (oneNamedParam (wsdlServiceStaticParameters, "Async") : bool),
                               (oneNamedParam (wsdlServiceStaticParameters, "CollectionType") : string)))  

            | "ODataService"   -> 
                odataServiceType 
                    (typePath,
                              ((oneNamedParam (odataServiceStaticParameters, "ServiceUri") : string),
                               (oneNamedParam (odataServiceStaticParameters, "LocalSchemaFile") : string),
                               (oneNamedParam (odataServiceStaticParameters, "ForceUpdate") : bool),
                               (oneNamedParam (odataServiceStaticParameters, "ResolutionFolder") : string),
                               (oneNamedParam (odataServiceStaticParameters, "DataServiceCollection") : bool))) 

            | _ -> null


        member __.GetInvokerExpression(mbase, parameters) = 
            match mbase with
            | :? ProvidedMethod as m when (match mbase.DeclaringType with :? ProvidedTypeDefinition as pt -> pt.IsErased | _ -> true) -> 
                m.InvokeCodeInternal parameters
            | :? ProvidedConstructor as m when (match mbase.DeclaringType with :? ProvidedTypeDefinition as pt -> pt.IsErased | _ -> true) -> 
                m.InvokeCodeInternal parameters
            // These are the standard for generated types
            | :?  ConstructorInfo as cinfo ->  
                Quotations.Expr.NewObject(cinfo, Array.toList parameters) 
            | :? System.Reflection.MethodInfo as minfo ->  
                if minfo.IsStatic then 
                    Quotations.Expr.Call(minfo, Array.toList parameters) 
                else
                    Quotations.Expr.Call(parameters.[0], minfo, Array.toList parameters.[1..])
             | _ -> 
                 System.Diagnostics.Debug.Assert false
                 failwith (FSData.SR.unexpectedMethodBase())

        [<CLIEvent>]
        member x.Invalidate = invalidation.Publish

        member x.GetGeneratedAssemblyContents(assembly) = 
            System.Diagnostics.Debug.Assert(assemblyBytesTable.ContainsKey assembly, "unexpected missing assembly in assemblyBytesTable")
            assemblyBytesTable.[assembly]
