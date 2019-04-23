// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Type providers, validation of provided types, etc.

namespace FSharp.Compiler

#if !NO_EXTENSIONTYPING

module internal ExtensionTyping =
    open System
    open System.IO
    open System.Collections.Generic
    open System.Reflection
    open Microsoft.FSharp.Core.CompilerServices
    open FSharp.Compiler.ErrorLogger
    open FSharp.Compiler.Range
    open FSharp.Compiler.AbstractIL.IL
    open FSharp.Compiler.AbstractIL.Diagnostics // dprintfn
    open FSharp.Compiler.AbstractIL.Internal.Library // frontAndBack

    type TypeProviderDesignation = TypeProviderDesignation of string

    exception ProvidedTypeResolution of range * System.Exception 
    exception ProvidedTypeResolutionNoRange of System.Exception 

    /// Represents some of the configuration parameters passed to type provider components 
    type ResolutionEnvironment =
        { resolutionFolder          : string
          outputFile                : string option
          showResolutionMessages    : bool
          referencedAssemblies      : string[]
          temporaryFolder           : string } 


    // Specify the tooling-compatible fragments of a path such as:
    //     typeproviders/fsharp41/net461/MyProvider.DesignTime.dll
    //     tools/fsharp41/net461/MyProvider.DesignTime.dll
    // See https://github.com/Microsoft/visualfsharp/issues/3736

    // Represents the FF#-compiler <-> type provider protocol.
    // When the API or protocol updates, add a new version moniker to the front of the list here.
    let toolingCompatibleTypeProviderProtocolMonikers() = 
        [ "fsharp41" ] 

    // Detect the host tooling context
    let toolingCompatibleVersions() = 
        if typeof<obj>.Assembly.GetName().Name = "mscorlib" then 
            [ "net461"; "net452"; "net451"; "net45"; "netstandard2.0"]
        elif typeof<obj>.Assembly.GetName().Name = "System.Private.CoreLib" then 
            [ "netcoreapp2.0"; "netstandard2.0"]
        else
            System.Diagnostics.Debug.Assert(false, "Couldn't determine runtime tooling context, assuming it supports at least .NET Standard 2.0")
            [  "netstandard2.0"]


    let toolingCompatiblePaths() = 
        [ for protocol in toolingCompatibleTypeProviderProtocolMonikers() do
            for netRuntime in toolingCompatibleVersions() do 
                yield Path.Combine("typeproviders", protocol, netRuntime)
                yield Path.Combine("tools", protocol, netRuntime)
        ]

    /// Load a the design-time part of a type-provider into the host process, and look for types
    /// marked with the TypeProviderAttribute attribute.
    let GetTypeProviderImplementationTypes (runTimeAssemblyFileName, designTimeAssemblyNameString, m: range) =

        // Report an error, blaming the particular type provider component
        let raiseError (e: exn) =
            raise (TypeProviderError(FSComp.SR.etProviderHasWrongDesignerAssembly(typeof<TypeProviderAssemblyAttribute>.Name, designTimeAssemblyNameString, e.Message), runTimeAssemblyFileName, m))

        // Find and load the designer assembly for the type provider component.
        //
        // We look in the directories stepping up from the location of the runtime assembly.

        let loadFromLocation designTimeAssemblyPath =
            try
                Some (FileSystem.AssemblyLoadFrom designTimeAssemblyPath)
            with e ->
                raiseError e

        let rec searchParentDirChain dir designTimeAssemblyName = 
            seq { 
                for subdir in toolingCompatiblePaths() do
                    let designTimeAssemblyPath  = Path.Combine (dir, subdir, designTimeAssemblyName)
                    if FileSystem.SafeExists designTimeAssemblyPath then 
                        yield loadFromLocation designTimeAssemblyPath
                match Path.GetDirectoryName dir with
                | s when s = "" || s = null || Path.GetFileName dir = "packages" || s = dir -> ()
                | parentDir -> yield! searchParentDirChain parentDir designTimeAssemblyName 
            } 

        let loadFromParentDirRelativeToRuntimeAssemblyLocation designTimeAssemblyName = 
            let runTimeAssemblyPath = Path.GetDirectoryName runTimeAssemblyFileName
            searchParentDirChain runTimeAssemblyPath  designTimeAssemblyName
            |> Seq.tryHead
            |> function 
               | Some res -> res 
               | None -> 
                // The search failed, just load from the first location and report an error
                let runTimeAssemblyPath = Path.GetDirectoryName runTimeAssemblyFileName
                loadFromLocation (Path.Combine (runTimeAssemblyPath, designTimeAssemblyName))

        let designTimeAssemblyOpt = 

            if designTimeAssemblyNameString.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) then
                loadFromParentDirRelativeToRuntimeAssemblyLocation designTimeAssemblyNameString
            else
                // Cover the case where the ".dll" extension has been left off and no version etc. has been used in the assembly
                // string specification.  The Name=FullName comparison is particularly strange, and was there to support
                // design-time DLLs specified using "x.DesignTIme, Version= ..." long assembly names and GAC loads.
                // These kind of design-time assembly specifications are no longer used to our knowledge so that comparison is basically legacy
                // and will always succeed.  
                let name = System.Reflection.AssemblyName (Path.GetFileNameWithoutExtension designTimeAssemblyNameString)
                if name.Name.Equals(name.FullName, StringComparison.OrdinalIgnoreCase) then
                    let designTimeAssemblyName = designTimeAssemblyNameString+".dll"
                    loadFromParentDirRelativeToRuntimeAssemblyLocation designTimeAssemblyName
                else
                    // Load from the GAC using Assembly.Load.  This is legacy since type provider design-time components are
                    // never in the GAC these days and  "x.DesignTIme, Version= ..." specifications are never used.
                    try
                        let asmName = System.Reflection.AssemblyName designTimeAssemblyNameString
                        Some (FileSystem.AssemblyLoad asmName)
                    with e ->
                        raiseError e

        // If we've find a design-time assembly, look for the public types with TypeProviderAttribute
        match designTimeAssemblyOpt with
        | Some loadedDesignTimeAssembly ->
            try
                let exportedTypes = loadedDesignTimeAssembly.GetExportedTypes() 
                let filtered = 
                    [ for t in exportedTypes do 
                          let ca = t.GetCustomAttributes(typeof<TypeProviderAttribute>, true)
                          if ca <> null && ca.Length > 0 then 
                              yield t ]
                filtered
            with e ->
                raiseError e
        | None -> []

    let StripException (e: exn) =
        match e with
#if !FX_REDUCED_EXCEPTIONS
        |   :? System.Reflection.TargetInvocationException as e -> e.InnerException
#endif
        |   :? TypeInitializationException as e -> e.InnerException
        |   _ -> e

    /// Create an instance of a type provider from the implementation type for the type provider in the
    /// design-time assembly by using reflection-invoke on a constructor for the type provider.
    let CreateTypeProvider (typeProviderImplementationType: System.Type, 
                            runtimeAssemblyPath, 
                            resolutionEnvironment: ResolutionEnvironment, 
                            isInvalidationSupported: bool, 
                            isInteractive: bool, 
                            systemRuntimeContainsType, 
                            systemRuntimeAssemblyVersion, 
                            m) =

        // Protect a .NET reflection call as we load the type provider component into the host process, 
        // reporting errors.
        let protect f =
            try 
                f ()
            with err ->
                let e = StripException (StripException err)
                raise (TypeProviderError(FSComp.SR.etTypeProviderConstructorException(e.Message), typeProviderImplementationType.FullName, m))

        if typeProviderImplementationType.GetConstructor([| typeof<TypeProviderConfig> |]) <> null then

            // Create the TypeProviderConfig to pass to the type provider constructor
            let e = TypeProviderConfig(systemRuntimeContainsType, 
                                       ResolutionFolder=resolutionEnvironment.resolutionFolder, 
                                       RuntimeAssembly=runtimeAssemblyPath, 
                                       ReferencedAssemblies=Array.copy resolutionEnvironment.referencedAssemblies, 
                                       TemporaryFolder=resolutionEnvironment.temporaryFolder, 
                                       IsInvalidationSupported=isInvalidationSupported, 
                                       IsHostedExecution= isInteractive, 
                                       SystemRuntimeAssemblyVersion = systemRuntimeAssemblyVersion)

            protect (fun () -> Activator.CreateInstance(typeProviderImplementationType, [| box e|]) :?> ITypeProvider )

        elif typeProviderImplementationType.GetConstructor [| |] <> null then 
            protect (fun () -> Activator.CreateInstance typeProviderImplementationType :?> ITypeProvider )

        else
            // No appropriate constructor found
            raise (TypeProviderError(FSComp.SR.etProviderDoesNotHaveValidConstructor(), typeProviderImplementationType.FullName, m))

    let GetTypeProvidersOfAssembly
            (runTimeAssemblyFileName: string, 
             ilScopeRefOfRuntimeAssembly: ILScopeRef, 
             designTimeAssemblyNameString: string, 
             resolutionEnvironment: ResolutionEnvironment, 
             isInvalidationSupported: bool, 
             isInteractive: bool, 
             systemRuntimeContainsType : string -> bool, 
             systemRuntimeAssemblyVersion : System.Version, 
             m: range) =         

        let providerSpecs = 
                try
                    let designTimeAssemblyName = 
                        try
                            if designTimeAssemblyNameString.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) then
                                Some (System.Reflection.AssemblyName (Path.GetFileNameWithoutExtension designTimeAssemblyNameString))
                            else
                                Some (System.Reflection.AssemblyName designTimeAssemblyNameString)
                        with :? ArgumentException ->
                            errorR(Error(FSComp.SR.etInvalidTypeProviderAssemblyName(runTimeAssemblyFileName, designTimeAssemblyNameString), m))
                            None

                    [ match designTimeAssemblyName, resolutionEnvironment.outputFile with
                      // Check if the attribute is pointing to the file being compiled, in which case ignore it
                      // This checks seems like legacy but is included for compat.
                      | Some designTimeAssemblyName, Some path when String.Compare(designTimeAssemblyName.Name, Path.GetFileNameWithoutExtension path, StringComparison.OrdinalIgnoreCase) = 0 ->
                          ()
                      | Some _, _ ->
                          for t in GetTypeProviderImplementationTypes (runTimeAssemblyFileName, designTimeAssemblyNameString, m) do
                            let resolver = CreateTypeProvider (t, runTimeAssemblyFileName, resolutionEnvironment, isInvalidationSupported, isInteractive, systemRuntimeContainsType, systemRuntimeAssemblyVersion, m)
                            match box resolver with 
                            | null -> ()
                            | _ -> yield (resolver, ilScopeRefOfRuntimeAssembly)
                      |   None, _ -> 
                          () ]

                with :? TypeProviderError as tpe ->
                    tpe.Iter(fun e -> errorR(NumberedError((e.Number, e.ContextualErrorMessage), m)) )                        
                    []

        let providers = Tainted<_>.CreateAll providerSpecs

        providers

    let unmarshal (t: Tainted<_>) = t.PUntaintNoFailure id

    /// Try to access a member on a provided type, catching and reporting errors
    let TryTypeMember(st: Tainted<_>, fullName, memberName, m, recover, f) =
        try
            st.PApply (f, m)
        with :? TypeProviderError as tpe -> 
            tpe.Iter (fun e -> errorR(Error(FSComp.SR.etUnexpectedExceptionFromProvidedTypeMember(fullName, memberName, e.ContextualErrorMessage), m)))
            st.PApplyNoFailure(fun _ -> recover)

    /// Try to access a member on a provided type, where the result is an array of values, catching and reporting errors
    let TryTypeMemberArray (st: Tainted<_>, fullName, memberName, m, f) =
        let result =
            try
                st.PApplyArray(f, memberName, m)
            with :? TypeProviderError as tpe ->
                tpe.Iter (fun e -> error(Error(FSComp.SR.etUnexpectedExceptionFromProvidedTypeMember(fullName, memberName, e.ContextualErrorMessage), m)))
                [||]

        match result with 
        | null -> error(Error(FSComp.SR.etUnexpectedNullFromProvidedTypeMember(fullName, memberName), m)); [||]
        | r -> r

    /// Try to access a member on a provided type, catching and reporting errors and checking the result is non-null, 
    let TryTypeMemberNonNull (st: Tainted<_>, fullName, memberName, m, recover, f) =
        match TryTypeMember(st, fullName, memberName, m, recover, f) with 
        | Tainted.Null -> 
            errorR(Error(FSComp.SR.etUnexpectedNullFromProvidedTypeMember(fullName, memberName), m)); 
            st.PApplyNoFailure(fun _ -> recover)
        | r -> r

    /// Try to access a property or method on a provided member, catching and reporting errors
    let TryMemberMember (mi: Tainted<_>, typeName, memberName, memberMemberName, m, recover, f) = 
        try
            mi.PApply (f, m)
        with :? TypeProviderError as tpe ->
            tpe.Iter (fun e -> errorR(Error(FSComp.SR.etUnexpectedExceptionFromProvidedMemberMember(memberMemberName, typeName, memberName, e.ContextualErrorMessage), m)))
            mi.PApplyNoFailure(fun _ -> recover)

    /// Get the string to show for the name of a type provider
    let DisplayNameOfTypeProvider(resolver: Tainted<ITypeProvider>, m: range) =
        resolver.PUntaint((fun tp -> tp.GetType().Name), m)

    /// Validate a provided namespace name
    let ValidateNamespaceName(name, typeProvider: Tainted<ITypeProvider>, m, nsp: string) =
        if nsp<>null then // Null namespace designates the global namespace.
            if String.IsNullOrWhiteSpace nsp then
                // Empty namespace is not allowed
                errorR(Error(FSComp.SR.etEmptyNamespaceOfTypeNotAllowed(name, typeProvider.PUntaint((fun tp -> tp.GetType().Name), m)), m))
            else
                for s in nsp.Split('.') do
                    match s.IndexOfAny(PrettyNaming.IllegalCharactersInTypeAndNamespaceNames) with
                    | -1 -> ()
                    | n -> errorR(Error(FSComp.SR.etIllegalCharactersInNamespaceName(string s.[n], s), m))  

    let bindingFlags =
        BindingFlags.DeclaredOnly |||
        BindingFlags.Static |||
        BindingFlags.Instance |||
        BindingFlags.Public

    // NOTE: for the purposes of remapping the closure of generated types, the FullName is sufficient.
    // We do _not_ rely on object identity or any other notion of equivalence provided by System.Type
    // itself. The mscorlib implementations of System.Type equality relations are not suitable: for
    // example RuntimeType overrides the equality relation to be reference equality for the Equals(object)
    // override, but the other subtypes of System.Type do not, making the relation non-reflective.
    //
    // Further, avoiding reliance on canonicalization (UnderlyingSystemType) or System.Type object identity means that 
    // providers can implement wrap-and-filter "views" over existing System.Type clusters without needing
    // to preserve object identity when presenting the types to the F# compiler.

    let providedSystemTypeComparer = 
        let key (ty: System.Type) = (ty.Assembly.FullName, ty.FullName)
        { new IEqualityComparer<Type> with 
            member __.GetHashCode(ty: Type) = hash (key ty)
            member __.Equals(ty1: Type, ty2: Type) = (key ty1 = key ty2) }

    /// The context used to interpret information in the closure of System.Type, System.MethodInfo and other 
    /// info objects coming from the type provider.
    ///
    /// This is the "Type --> Tycon" remapping context of the type. This is only present for generated provided types, and contains
    /// all the entries in the remappings for the generative declaration.
    ///
    /// Laziness is used "to prevent needless computation for every type during remapping". However it
    /// appears that the laziness likely serves no purpose and could be safely removed.
    type ProvidedTypeContext = 
        | NoEntries
        | Entries of Dictionary<System.Type, ILTypeRef> * Lazy<Dictionary<System.Type, obj>>

        static member Empty = NoEntries

        static member Create(d1, d2) = Entries(d1, notlazy d2)

        member ctxt.GetDictionaries()  = 
            match ctxt with
            | NoEntries -> 
                Dictionary<System.Type, ILTypeRef>(providedSystemTypeComparer), Dictionary<System.Type, obj>(providedSystemTypeComparer)
            | Entries (lookupILTR, lookupILTCR) ->
                lookupILTR, lookupILTCR.Force()

        member ctxt.TryGetILTypeRef st = 
            match ctxt with 
            | NoEntries -> None 
            | Entries(d, _) -> 
                match d.TryGetValue st with
                | true, res -> Some res
                | _ -> None

        member ctxt.TryGetTyconRef st = 
            match ctxt with 
            | NoEntries -> None 
            | Entries(_, d) -> 
                let d = d.Force()
                match d.TryGetValue st with
                | true, res -> Some res
                | _ -> None

        member ctxt.RemapTyconRefs (f: obj->obj) = 
            match ctxt with 
            | NoEntries -> NoEntries
            | Entries(d1, d2) ->
                Entries(d1, lazy (let dict = new Dictionary<System.Type, obj>(providedSystemTypeComparer)
                                  for KeyValue (st, tcref) in d2.Force() do dict.Add(st, f tcref)
                                  dict))

    type CustomAttributeData = System.Reflection.CustomAttributeData
    type CustomAttributeNamedArgument = System.Reflection.CustomAttributeNamedArgument
    type CustomAttributeTypedArgument = System.Reflection.CustomAttributeTypedArgument

    [<AllowNullLiteral; Sealed>]
    type ProvidedType (x: System.Type, ctxt: ProvidedTypeContext) =
        inherit ProvidedMemberInfo(x, ctxt)
        let provide () = ProvidedCustomAttributeProvider.Create (fun _provider -> x.CustomAttributes)
        interface IProvidedCustomAttributeProvider with 
            member __.GetHasTypeProviderEditorHideMethodsAttribute provider = provide().GetHasTypeProviderEditorHideMethodsAttribute provider
            member __.GetDefinitionLocationAttribute provider = provide().GetDefinitionLocationAttribute provider
            member __.GetXmlDocAttributes provider = provide().GetXmlDocAttributes provider
        
        // The type provider spec distinguishes between 
        //   - calls that can be made on provided types (i.e. types given by ReturnType, ParameterType, and generic argument types)
        //   - calls that can be made on provided type definitions (types returned by ResolveTypeName, GetTypes etc.)
        // Ideally we would enforce this decision structurally by having both ProvidedType and ProvidedTypeDefinition.
        // Alternatively we could use assertions to enforce this.

        // Suppress relocation of generated types
        member __.IsSuppressRelocate = (x.Attributes &&& enum (int32 TypeProviderTypeAttributes.SuppressRelocate)) <> enum 0  
        member __.IsErased = (x.Attributes &&& enum (int32 TypeProviderTypeAttributes.IsErased)) <> enum 0  
        member __.IsGenericType = x.IsGenericType
        member __.Namespace = x.Namespace
        member __.FullName = x.FullName
        member __.IsArray = x.IsArray
        member __.Assembly = x.Assembly |> ProvidedAssembly.Create ctxt
        member __.GetInterfaces() = x.GetInterfaces() |> ProvidedType.CreateArray ctxt
        member __.GetMethods() = x.GetMethods bindingFlags |> ProvidedMethodInfo.CreateArray ctxt
        member __.GetEvents() = x.GetEvents bindingFlags |> ProvidedEventInfo.CreateArray ctxt
        member __.GetEvent nm = x.GetEvent(nm, bindingFlags) |> ProvidedEventInfo.Create ctxt
        member __.GetProperties() = x.GetProperties bindingFlags |> ProvidedPropertyInfo.CreateArray ctxt
        member __.GetProperty nm = x.GetProperty(nm, bindingFlags) |> ProvidedPropertyInfo.Create ctxt
        member __.GetConstructors() = x.GetConstructors bindingFlags |> ProvidedConstructorInfo.CreateArray ctxt
        member __.GetFields() = x.GetFields bindingFlags |> ProvidedFieldInfo.CreateArray ctxt
        member __.GetField nm = x.GetField(nm, bindingFlags) |> ProvidedFieldInfo.Create ctxt
        member __.GetAllNestedTypes() = x.GetNestedTypes(bindingFlags ||| BindingFlags.NonPublic) |> ProvidedType.CreateArray ctxt
        member __.GetNestedTypes() = x.GetNestedTypes bindingFlags |> ProvidedType.CreateArray ctxt
        /// Type.GetNestedType(string) can return null if there is no nested type with given name
        member __.GetNestedType nm = x.GetNestedType (nm, bindingFlags) |> ProvidedType.Create ctxt
        /// Type.GetGenericTypeDefinition() either returns type or throws exception, null is not permitted
        member __.GetGenericTypeDefinition() = x.GetGenericTypeDefinition() |> ProvidedType.CreateWithNullCheck ctxt "GenericTypeDefinition"
        /// Type.BaseType can be null when Type is interface or object
        member __.BaseType = x.BaseType |> ProvidedType.Create ctxt
        member __.GetStaticParameters(provider: ITypeProvider) = provider.GetStaticParameters x |> ProvidedParameterInfo.CreateArray ctxt
        /// Type.GetElementType can be null if i.e. Type is not array\pointer\byref type
        member __.GetElementType() = x.GetElementType() |> ProvidedType.Create ctxt
        member __.GetGenericArguments() = x.GetGenericArguments() |> ProvidedType.CreateArray ctxt
        member __.ApplyStaticArguments(provider: ITypeProvider, fullTypePathAfterArguments, staticArgs: obj[]) = 
            provider.ApplyStaticArguments(x, fullTypePathAfterArguments,  staticArgs) |> ProvidedType.Create ctxt
        member __.IsVoid = (typeof<System.Void>.Equals x || (x.Namespace = "System" && x.Name = "Void"))
        member __.IsGenericParameter = x.IsGenericParameter
        member __.IsValueType = x.IsValueType
        member __.IsByRef = x.IsByRef
        member __.IsPointer = x.IsPointer
        member __.IsPublic = x.IsPublic
        member __.IsNestedPublic = x.IsNestedPublic
        member __.IsEnum = x.IsEnum
        member __.IsClass = x.IsClass
        member __.IsSealed = x.IsSealed
        member __.IsInterface = x.IsInterface
        member __.GetArrayRank() = x.GetArrayRank()
        member __.GenericParameterPosition = x.GenericParameterPosition
        member __.RawSystemType = x
        /// Type.GetEnumUnderlyingType either returns type or raises exception, null is not permitted
        member __.GetEnumUnderlyingType() = 
            x.GetEnumUnderlyingType() 
            |> ProvidedType.CreateWithNullCheck ctxt "EnumUnderlyingType"
        static member Create ctxt x = match x with null -> null | t -> ProvidedType (t, ctxt)
        static member CreateWithNullCheck ctxt name x = match x with null -> nullArg name | t -> ProvidedType (t, ctxt)
        static member CreateArray ctxt xs = match xs with null -> null | _ -> xs |> Array.map (ProvidedType.Create ctxt)
        static member CreateNoContext (x: Type) = ProvidedType.Create ProvidedTypeContext.Empty x
        static member Void = ProvidedType.CreateNoContext typeof<System.Void>
        member __.Handle = x
        override __.Equals y = assert false; match y with :? ProvidedType as y -> x.Equals y.Handle | _ -> false
        override __.GetHashCode() = assert false; x.GetHashCode()
        member __.TryGetILTypeRef() = ctxt.TryGetILTypeRef x
        member __.TryGetTyconRef() = ctxt.TryGetTyconRef x
        member __.Context = ctxt
        static member ApplyContext (pt: ProvidedType, ctxt) = ProvidedType(pt.Handle, ctxt)
        static member TaintedEquals (pt1: Tainted<ProvidedType>, pt2: Tainted<ProvidedType>) = 
           Tainted.EqTainted (pt1.PApplyNoFailure(fun st -> st.Handle)) (pt2.PApplyNoFailure(fun st -> st.Handle))

    and [<AllowNullLiteral>] 
        IProvidedCustomAttributeProvider =
        abstract GetDefinitionLocationAttribute : provider: ITypeProvider -> (string * int * int) option 
        abstract GetXmlDocAttributes : provider: ITypeProvider -> string[]
        abstract GetHasTypeProviderEditorHideMethodsAttribute : provider: ITypeProvider -> bool
        abstract GetAttributeConstructorArgs: provider: ITypeProvider * attribName: string -> (obj option list * (string * obj option) list) option

    and ProvidedCustomAttributeProvider =
        static member Create (attributes :(ITypeProvider -> seq<CustomAttributeData>)) : IProvidedCustomAttributeProvider = 
            let (|Member|_|) (s: string) (x: CustomAttributeNamedArgument) = if x.MemberName = s then Some x.TypedValue else None
            let (|Arg|_|) (x: CustomAttributeTypedArgument) = match x.Value with null -> None | v -> Some v
            let findAttribByName tyFullName (a: CustomAttributeData) = (a.Constructor.DeclaringType.FullName = tyFullName)  
            let findAttrib (ty: System.Type) a = findAttribByName ty.FullName a
            { new IProvidedCustomAttributeProvider with 
                  member __.GetAttributeConstructorArgs (provider, attribName) = 
                      attributes provider 
                        |> Seq.tryFind (findAttribByName  attribName)  
                        |> Option.map (fun a -> 
                            let ctorArgs = 
                                a.ConstructorArguments 
                                |> Seq.toList 
                                |> List.map (function Arg null -> None | Arg obj -> Some obj | _ -> None)
                            let namedArgs = 
                                a.NamedArguments 
                                |> Seq.toList 
                                |> List.map (fun arg -> arg.MemberName, match arg.TypedValue with Arg null -> None | Arg obj -> Some obj | _ -> None)
                            ctorArgs, namedArgs)

                  member __.GetHasTypeProviderEditorHideMethodsAttribute provider = 
                      attributes provider 
                        |> Seq.exists (findAttrib typeof<Microsoft.FSharp.Core.CompilerServices.TypeProviderEditorHideMethodsAttribute>) 

                  member __.GetDefinitionLocationAttribute provider = 
                      attributes provider 
                        |> Seq.tryFind (findAttrib  typeof<Microsoft.FSharp.Core.CompilerServices.TypeProviderDefinitionLocationAttribute>)  
                        |> Option.map (fun a -> 
                               (defaultArg (a.NamedArguments |> Seq.tryPick (function Member "FilePath" (Arg (:? string as v)) -> Some v | _ -> None)) null, 
                                defaultArg (a.NamedArguments |> Seq.tryPick (function Member "Line" (Arg (:? int as v)) -> Some v | _ -> None)) 0, 
                                defaultArg (a.NamedArguments |> Seq.tryPick (function Member "Column" (Arg (:? int as v)) -> Some v | _ -> None)) 0))

                  member __.GetXmlDocAttributes provider = 
                      attributes provider 
                        |> Seq.choose (fun a -> 
                             if findAttrib  typeof<Microsoft.FSharp.Core.CompilerServices.TypeProviderXmlDocAttribute> a then 
                                match a.ConstructorArguments |> Seq.toList with 
                                | [ Arg(:? string as s) ] -> Some s
                                | _ -> None
                             else 
                                None) 
                        |> Seq.toArray  }

    and [<AllowNullLiteral; AbstractClass>] 
        ProvidedMemberInfo (x: System.Reflection.MemberInfo, ctxt) = 
        let provide () = ProvidedCustomAttributeProvider.Create (fun _provider -> x.CustomAttributes)
        member __.Name = x.Name
        /// DeclaringType can be null if MemberInfo belongs to Module, not to Type
        member __.DeclaringType = ProvidedType.Create ctxt x.DeclaringType
        interface IProvidedCustomAttributeProvider with 
            member __.GetHasTypeProviderEditorHideMethodsAttribute provider = provide().GetHasTypeProviderEditorHideMethodsAttribute provider
            member __.GetDefinitionLocationAttribute provider = provide().GetDefinitionLocationAttribute provider
            member __.GetXmlDocAttributes provider = provide().GetXmlDocAttributes provider
            member __.GetAttributeConstructorArgs (provider, attribName) = provide().GetAttributeConstructorArgs (provider, attribName)

    and [<AllowNullLiteral; Sealed>] 
        ProvidedParameterInfo (x: System.Reflection.ParameterInfo, ctxt) = 
        let provide () = ProvidedCustomAttributeProvider.Create (fun _provider -> x.CustomAttributes)
        member __.Name = x.Name
        member __.IsOut = x.IsOut
        member __.IsIn = x.IsIn
        member __.IsOptional = x.IsOptional
        member __.RawDefaultValue = x.RawDefaultValue
        member __.HasDefaultValue = x.Attributes.HasFlag(System.Reflection.ParameterAttributes.HasDefault)
        /// ParameterInfo.ParameterType cannot be null
        member __.ParameterType = ProvidedType.CreateWithNullCheck ctxt "ParameterType" x.ParameterType 
        static member Create ctxt x = match x with null -> null | t -> ProvidedParameterInfo (t, ctxt)
        static member CreateArray ctxt xs = match xs with null -> null | _ -> xs |> Array.map (ProvidedParameterInfo.Create ctxt)  // TODO null wrong?
        interface IProvidedCustomAttributeProvider with 
            member __.GetHasTypeProviderEditorHideMethodsAttribute provider = provide().GetHasTypeProviderEditorHideMethodsAttribute provider
            member __.GetDefinitionLocationAttribute provider = provide().GetDefinitionLocationAttribute provider
            member __.GetXmlDocAttributes provider = provide().GetXmlDocAttributes provider
            member __.GetAttributeConstructorArgs (provider, attribName) = provide().GetAttributeConstructorArgs (provider, attribName)
        member __.Handle = x
        override __.Equals y = assert false; match y with :? ProvidedParameterInfo as y -> x.Equals y.Handle | _ -> false
        override __.GetHashCode() = assert false; x.GetHashCode()

    and [<AllowNullLiteral; Sealed>] 
        ProvidedAssembly (x: System.Reflection.Assembly, _ctxt) = 
        member __.GetName() = x.GetName()
        member __.FullName = x.FullName
        member __.GetManifestModuleContents(provider: ITypeProvider) = provider.GetGeneratedAssemblyContents x
        static member Create ctxt x = match x with null -> null | t -> ProvidedAssembly (t, ctxt)
        member __.Handle = x
        override __.Equals y = assert false; match y with :? ProvidedAssembly as y -> x.Equals y.Handle | _ -> false
        override __.GetHashCode() = assert false; x.GetHashCode()

    and [<AllowNullLiteral; AbstractClass>] 
        ProvidedMethodBase (x: System.Reflection.MethodBase, ctxt) = 
        inherit ProvidedMemberInfo(x, ctxt)
        member __.Context = ctxt
        member __.IsGenericMethod = x.IsGenericMethod
        member __.IsStatic  = x.IsStatic
        member __.IsFamily  = x.IsFamily
        member __.IsFamilyOrAssembly = x.IsFamilyOrAssembly
        member __.IsFamilyAndAssembly = x.IsFamilyAndAssembly
        member __.IsVirtual  = x.IsVirtual
        member __.IsFinal = x.IsFinal
        member __.IsPublic = x.IsPublic
        member __.IsAbstract  = x.IsAbstract
        member __.IsHideBySig = x.IsHideBySig
        member __.IsConstructor  = x.IsConstructor
        member __.GetParameters() = x.GetParameters() |> ProvidedParameterInfo.CreateArray ctxt 
        member __.GetGenericArguments() = x.GetGenericArguments() |> ProvidedType.CreateArray ctxt
        member __.Handle = x
        static member TaintedGetHashCode (x: Tainted<ProvidedMethodBase>) =            
           Tainted.GetHashCodeTainted (x.PApplyNoFailure(fun st -> (st.Name, st.DeclaringType.Assembly.FullName, st.DeclaringType.FullName))) 
        static member TaintedEquals (pt1: Tainted<ProvidedMethodBase>, pt2: Tainted<ProvidedMethodBase>) = 
           Tainted.EqTainted (pt1.PApplyNoFailure(fun st -> st.Handle)) (pt2.PApplyNoFailure(fun st -> st.Handle))

        member __.GetStaticParametersForMethod(provider: ITypeProvider) = 
            let bindingFlags = BindingFlags.Instance ||| BindingFlags.NonPublic ||| BindingFlags.Public 

            let staticParams = 
                match provider with 
                | :? ITypeProvider2 as itp2 -> 
                    itp2.GetStaticParametersForMethod x  
                | _ -> 
                    // To allow a type provider to depend only on FSharp.Core 4.3.0.0, it can alternatively implement an appropriate method called GetStaticParametersForMethod
                    let meth = provider.GetType().GetMethod( "GetStaticParametersForMethod", bindingFlags, null, [| typeof<System.Reflection.MethodBase> |], null)  
                    if isNull meth then [| |] else
                    let paramsAsObj = 
                        try meth.Invoke(provider, bindingFlags ||| BindingFlags.InvokeMethod, null, [| box x |], null) 
                        with err -> raise (StripException (StripException err))
                    paramsAsObj :?> System.Reflection.ParameterInfo[] 

            staticParams |> ProvidedParameterInfo.CreateArray ctxt

        member __.ApplyStaticArgumentsForMethod(provider: ITypeProvider, fullNameAfterArguments: string, staticArgs: obj[]) = 
            let bindingFlags = BindingFlags.Instance ||| BindingFlags.Public ||| BindingFlags.InvokeMethod

            let mb = 
                match provider with 
                | :? ITypeProvider2 as itp2 -> 
                    itp2.ApplyStaticArgumentsForMethod(x, fullNameAfterArguments, staticArgs)  
                | _ -> 
                    // To allow a type provider to depend only on FSharp.Core 4.3.0.0, it can alternatively implement a method called GetStaticParametersForMethod
                    let meth = provider.GetType().GetMethod( "ApplyStaticArgumentsForMethod", bindingFlags, null, [| typeof<System.Reflection.MethodBase>; typeof<string>; typeof<obj[]> |], null)  
                    match meth with 
                    | null -> failwith (FSComp.SR.estApplyStaticArgumentsForMethodNotImplemented())
                    | _ -> 
                    let mbAsObj = 
                       try meth.Invoke(provider, bindingFlags ||| BindingFlags.InvokeMethod, null, [| box x; box fullNameAfterArguments; box staticArgs  |], null) 
                       with err -> raise (StripException (StripException err))

                    match mbAsObj with 
                    | :? System.Reflection.MethodBase as mb -> mb
                    | _ -> failwith (FSComp.SR.estApplyStaticArgumentsForMethodNotImplemented())
            match mb with 
            | :? System.Reflection.MethodInfo as mi -> (mi |> ProvidedMethodInfo.Create ctxt : ProvidedMethodInfo) :> ProvidedMethodBase
            | :? System.Reflection.ConstructorInfo as ci -> (ci |> ProvidedConstructorInfo.Create ctxt : ProvidedConstructorInfo) :> ProvidedMethodBase
            | _ -> failwith (FSComp.SR.estApplyStaticArgumentsForMethodNotImplemented())


    and [<AllowNullLiteral; Sealed>] 
        ProvidedFieldInfo (x: System.Reflection.FieldInfo, ctxt) = 
        inherit ProvidedMemberInfo(x, ctxt)
        static member Create ctxt x = match x with null -> null | t -> ProvidedFieldInfo (t, ctxt)
        static member CreateArray ctxt xs = match xs with null -> null | _ -> xs |> Array.map (ProvidedFieldInfo.Create ctxt)
        member __.IsInitOnly = x.IsInitOnly
        member __.IsStatic = x.IsStatic
        member __.IsSpecialName = x.IsSpecialName
        member __.IsLiteral = x.IsLiteral
        member __.GetRawConstantValue() = x.GetRawConstantValue()
        /// FieldInfo.FieldType cannot be null
        member __.FieldType = x.FieldType |> ProvidedType.CreateWithNullCheck ctxt "FieldType" 
        member __.Handle = x
        member __.IsPublic = x.IsPublic
        member __.IsFamily = x.IsFamily
        member __.IsPrivate = x.IsPrivate
        member __.IsFamilyOrAssembly = x.IsFamilyOrAssembly
        member __.IsFamilyAndAssembly = x.IsFamilyAndAssembly
        override __.Equals y = assert false; match y with :? ProvidedFieldInfo as y -> x.Equals y.Handle | _ -> false
        override __.GetHashCode() = assert false; x.GetHashCode()
        static member TaintedEquals (pt1: Tainted<ProvidedFieldInfo>, pt2: Tainted<ProvidedFieldInfo>) = 
           Tainted.EqTainted (pt1.PApplyNoFailure(fun st -> st.Handle)) (pt2.PApplyNoFailure(fun st -> st.Handle))



    and [<AllowNullLiteral; Sealed>] 
        ProvidedMethodInfo (x: System.Reflection.MethodInfo, ctxt) = 
        inherit ProvidedMethodBase(x, ctxt)

        member __.ReturnType = x.ReturnType |> ProvidedType.CreateWithNullCheck ctxt "ReturnType"

        static member Create ctxt x = match x with null -> null | t -> ProvidedMethodInfo (t, ctxt)

        static member CreateArray ctxt xs = match xs with null -> null | _ -> xs |> Array.map (ProvidedMethodInfo.Create ctxt)
        member __.Handle = x
#if !FX_NO_REFLECTION_METADATA_TOKENS
        member __.MetadataToken = x.MetadataToken
#endif
        override __.Equals y = assert false; match y with :? ProvidedMethodInfo as y -> x.Equals y.Handle | _ -> false
        override __.GetHashCode() = assert false; x.GetHashCode()

    and [<AllowNullLiteral; Sealed>] 
        ProvidedPropertyInfo (x: System.Reflection.PropertyInfo, ctxt) = 
        inherit ProvidedMemberInfo(x, ctxt)
        member __.GetGetMethod() = x.GetGetMethod() |> ProvidedMethodInfo.Create ctxt
        member __.GetSetMethod() = x.GetSetMethod() |> ProvidedMethodInfo.Create ctxt
        member __.CanRead = x.CanRead
        member __.CanWrite = x.CanWrite
        member __.GetIndexParameters() = x.GetIndexParameters() |> ProvidedParameterInfo.CreateArray ctxt
        /// PropertyInfo.PropertyType cannot be null
        member __.PropertyType = x.PropertyType |> ProvidedType.CreateWithNullCheck ctxt "PropertyType"
        static member Create ctxt x = match x with null -> null | t -> ProvidedPropertyInfo (t, ctxt)
        static member CreateArray ctxt xs = match xs with null -> null | _ -> xs |> Array.map (ProvidedPropertyInfo.Create ctxt)
        member __.Handle = x
        override __.Equals y = assert false; match y with :? ProvidedPropertyInfo as y -> x.Equals y.Handle | _ -> false
        override __.GetHashCode() = assert false; x.GetHashCode()
        static member TaintedGetHashCode (x: Tainted<ProvidedPropertyInfo>) = 
           Tainted.GetHashCodeTainted (x.PApplyNoFailure(fun st -> (st.Name, st.DeclaringType.Assembly.FullName, st.DeclaringType.FullName))) 
        static member TaintedEquals (pt1: Tainted<ProvidedPropertyInfo>, pt2: Tainted<ProvidedPropertyInfo>) = 
           Tainted.EqTainted (pt1.PApplyNoFailure(fun st -> st.Handle)) (pt2.PApplyNoFailure(fun st -> st.Handle))

    and [<AllowNullLiteral; Sealed>] 
        ProvidedEventInfo (x: System.Reflection.EventInfo, ctxt) = 
        inherit ProvidedMemberInfo(x, ctxt)
        member __.GetAddMethod() = x.GetAddMethod() |> ProvidedMethodInfo.Create  ctxt
        member __.GetRemoveMethod() = x.GetRemoveMethod() |> ProvidedMethodInfo.Create ctxt
        /// EventInfo.EventHandlerType cannot be null
        member __.EventHandlerType = x.EventHandlerType |> ProvidedType.CreateWithNullCheck ctxt "EventHandlerType"
        static member Create ctxt x = match x with null -> null | t -> ProvidedEventInfo (t, ctxt)
        static member CreateArray ctxt xs = match xs with null -> null | _ -> xs |> Array.map (ProvidedEventInfo.Create ctxt)
        member __.Handle = x
        override __.Equals y = assert false; match y with :? ProvidedEventInfo as y -> x.Equals y.Handle | _ -> false
        override __.GetHashCode() = assert false; x.GetHashCode()
        static member TaintedGetHashCode (x: Tainted<ProvidedEventInfo>) = 
           Tainted.GetHashCodeTainted (x.PApplyNoFailure(fun st -> (st.Name, st.DeclaringType.Assembly.FullName, st.DeclaringType.FullName))) 
        static member TaintedEquals (pt1: Tainted<ProvidedEventInfo>, pt2: Tainted<ProvidedEventInfo>) = 
           Tainted.EqTainted (pt1.PApplyNoFailure(fun st -> st.Handle)) (pt2.PApplyNoFailure(fun st -> st.Handle))

    and [<AllowNullLiteral; Sealed>] 
        ProvidedConstructorInfo (x: System.Reflection.ConstructorInfo, ctxt) = 
        inherit ProvidedMethodBase(x, ctxt)
        static member Create ctxt x = match x with null -> null | t -> ProvidedConstructorInfo (t, ctxt)
        static member CreateArray ctxt xs = match xs with null -> null | _ -> xs |> Array.map (ProvidedConstructorInfo.Create ctxt)
        member __.Handle = x
        override __.Equals y = assert false; match y with :? ProvidedConstructorInfo as y -> x.Equals y.Handle | _ -> false
        override __.GetHashCode() = assert false; x.GetHashCode()

    [<RequireQualifiedAccess; Class; AllowNullLiteral; Sealed>]
    type ProvidedExpr (x: Quotations.Expr, ctxt) =
        member __.Type = x.Type |> ProvidedType.Create ctxt
        member __.Handle = x
        member __.Context = ctxt
        member __.UnderlyingExpressionString = x.ToString()
        static member Create ctxt t = match box t with null -> null | _ -> ProvidedExpr (t, ctxt)
        static member CreateArray ctxt xs = match xs with null -> null | _ -> xs |> Array.map (ProvidedExpr.Create ctxt)
        override __.Equals y = match y with :? ProvidedExpr as y -> x.Equals y.Handle | _ -> false
        override __.GetHashCode() = x.GetHashCode()

    [<RequireQualifiedAccess; Class; AllowNullLiteral; Sealed>]
    type ProvidedVar (x: Quotations.Var, ctxt) =
        member __.Type = x.Type |> ProvidedType.Create ctxt
        member __.Name = x.Name
        member __.IsMutable = x.IsMutable
        member __.Handle = x
        member __.Context = ctxt
        static member Create ctxt t = match box t with null -> null | _ -> ProvidedVar (t, ctxt)
        static member Fresh (nm, ty: ProvidedType) = ProvidedVar.Create ty.Context (new Quotations.Var(nm, ty.Handle))
        static member CreateArray ctxt xs = match xs with null -> null | _ -> xs |> Array.map (ProvidedVar.Create ctxt)
        override __.Equals y = match y with :? ProvidedVar as y -> x.Equals y.Handle | _ -> false
        override __.GetHashCode() = x.GetHashCode()


    /// Detect a provided new-object expression 
    let (|ProvidedNewObjectExpr|_|) (x: ProvidedExpr) = 
        match x.Handle with 
        |  Quotations.Patterns.NewObject(ctor, args)  -> 
            Some (ProvidedConstructorInfo.Create x.Context ctor, [| for a in args -> ProvidedExpr.Create x.Context a |])
        | _ -> None

    /// Detect a provided while-loop expression 
    let (|ProvidedWhileLoopExpr|_|) (x: ProvidedExpr) = 
        match x.Handle with 
        |  Quotations.Patterns.WhileLoop(guardExpr, bodyExpr)  -> 
            Some (ProvidedExpr.Create x.Context guardExpr, ProvidedExpr.Create x.Context bodyExpr)
        | _ -> None

    /// Detect a provided new-delegate expression 
    let (|ProvidedNewDelegateExpr|_|) (x: ProvidedExpr) = 
        match x.Handle with 
        |  Quotations.Patterns.NewDelegate(ty, vs, expr)  -> 
            Some (ProvidedType.Create x.Context ty, ProvidedVar.CreateArray x.Context (List.toArray vs), ProvidedExpr.Create x.Context expr)
        | _ -> None

    /// Detect a provided call expression 
    let (|ProvidedCallExpr|_|) (x: ProvidedExpr) = 
        match x.Handle with 
        |  Quotations.Patterns.Call(objOpt, meth, args) -> 
            Some ((match objOpt with None -> None | Some obj -> Some (ProvidedExpr.Create  x.Context obj)), 
                  ProvidedMethodInfo.Create x.Context meth, 
                  [| for a in args -> ProvidedExpr.Create  x.Context a |])
        | _ -> None

    /// Detect a provided default-value expression 
    let (|ProvidedDefaultExpr|_|) (x: ProvidedExpr) = 
        match x.Handle with 
        |  Quotations.Patterns.DefaultValue ty   -> Some (ProvidedType.Create x.Context ty)
        | _ -> None

    /// Detect a provided constant expression 
    let (|ProvidedConstantExpr|_|) (x: ProvidedExpr) = 
        match x.Handle with 
        |  Quotations.Patterns.Value(obj, ty)   -> Some (obj, ProvidedType.Create x.Context ty)
        | _ -> None

    /// Detect a provided type-as expression 
    let (|ProvidedTypeAsExpr|_|) (x: ProvidedExpr) = 
        match x.Handle with 
        |  Quotations.Patterns.Coerce(arg, ty) -> Some (ProvidedExpr.Create x.Context arg, ProvidedType.Create  x.Context ty)
        | _ -> None

    /// Detect a provided new-tuple expression 
    let (|ProvidedNewTupleExpr|_|) (x: ProvidedExpr) = 
        match x.Handle with 
        |  Quotations.Patterns.NewTuple args -> Some (ProvidedExpr.CreateArray x.Context (Array.ofList args))
        | _ -> None

    /// Detect a provided tuple-get expression 
    let (|ProvidedTupleGetExpr|_|) (x: ProvidedExpr) = 
        match x.Handle with 
        |  Quotations.Patterns.TupleGet(arg, n) -> Some (ProvidedExpr.Create x.Context arg, n)
        | _ -> None

    /// Detect a provided new-array expression 
    let (|ProvidedNewArrayExpr|_|) (x: ProvidedExpr) = 
        match x.Handle with 
        |  Quotations.Patterns.NewArray(ty, args) -> Some (ProvidedType.Create  x.Context ty, ProvidedExpr.CreateArray x.Context (Array.ofList args))
        | _ -> None

    /// Detect a provided sequential expression 
    let (|ProvidedSequentialExpr|_|) (x: ProvidedExpr) = 
        match x.Handle with 
        |  Quotations.Patterns.Sequential(e1, e2) -> Some (ProvidedExpr.Create x.Context e1, ProvidedExpr.Create x.Context e2)
        | _ -> None

    /// Detect a provided lambda expression 
    let (|ProvidedLambdaExpr|_|) (x: ProvidedExpr) = 
        match x.Handle with 
        |  Quotations.Patterns.Lambda(v, body) -> Some (ProvidedVar.Create x.Context v,  ProvidedExpr.Create x.Context body)
        | _ -> None

    /// Detect a provided try/finally expression 
    let (|ProvidedTryFinallyExpr|_|) (x: ProvidedExpr) = 
        match x.Handle with 
        |  Quotations.Patterns.TryFinally(b1, b2) -> Some (ProvidedExpr.Create x.Context b1, ProvidedExpr.Create x.Context b2)
        | _ -> None

    /// Detect a provided try/with expression 
    let (|ProvidedTryWithExpr|_|) (x: ProvidedExpr) = 
        match x.Handle with 
        |  Quotations.Patterns.TryWith(b, v1, e1, v2, e2) -> Some (ProvidedExpr.Create x.Context b, ProvidedVar.Create x.Context v1, ProvidedExpr.Create x.Context e1, ProvidedVar.Create x.Context v2, ProvidedExpr.Create x.Context e2)
        | _ -> None

#if PROVIDED_ADDRESS_OF
    let (|ProvidedAddressOfExpr|_|) (x: ProvidedExpr) = 
        match x.Handle with 
        |  Quotations.Patterns.AddressOf e -> Some (ProvidedExpr.Create x.Context e)
        | _ -> None
#endif

    /// Detect a provided type-test expression 
    let (|ProvidedTypeTestExpr|_|) (x: ProvidedExpr) = 
        match x.Handle with 
        |  Quotations.Patterns.TypeTest(e, ty) -> Some (ProvidedExpr.Create x.Context e, ProvidedType.Create x.Context ty)
        | _ -> None

    /// Detect a provided 'let' expression 
    let (|ProvidedLetExpr|_|) (x: ProvidedExpr) = 
        match x.Handle with 
        |  Quotations.Patterns.Let(v, e, b) -> Some (ProvidedVar.Create x.Context v, ProvidedExpr.Create x.Context e, ProvidedExpr.Create x.Context b)
        | _ -> None


    /// Detect a provided expression which is a for-loop over integers
    let (|ProvidedForIntegerRangeLoopExpr|_|) (x: ProvidedExpr) = 
        match x.Handle with 
        |  Quotations.Patterns.ForIntegerRangeLoop (v, e1, e2, e3) -> 
            Some (ProvidedVar.Create x.Context v, 
                  ProvidedExpr.Create x.Context e1, 
                  ProvidedExpr.Create x.Context e2, 
                  ProvidedExpr.Create x.Context e3)
        | _ -> None

    /// Detect a provided 'set variable' expression 
    let (|ProvidedVarSetExpr|_|) (x: ProvidedExpr) = 
        match x.Handle with 
        |  Quotations.Patterns.VarSet(v, e) -> Some (ProvidedVar.Create x.Context v, ProvidedExpr.Create x.Context e)
        | _ -> None

    /// Detect a provided 'IfThenElse' expression 
    let (|ProvidedIfThenElseExpr|_|) (x: ProvidedExpr) = 
        match x.Handle with 
        |  Quotations.Patterns.IfThenElse(g, t, e) ->  Some (ProvidedExpr.Create x.Context g, ProvidedExpr.Create x.Context t, ProvidedExpr.Create x.Context e)
        | _ -> None

    /// Detect a provided 'Var' expression 
    let (|ProvidedVarExpr|_|) (x: ProvidedExpr) = 
        match x.Handle with 
        |  Quotations.Patterns.Var v  -> Some (ProvidedVar.Create x.Context v)
        | _ -> None

    /// Get the provided invoker expression for a particular use of a method.
    let GetInvokerExpression (provider: ITypeProvider, methodBase: ProvidedMethodBase, paramExprs: ProvidedVar[]) = 
        provider.GetInvokerExpression(methodBase.Handle, [| for p in paramExprs -> Quotations.Expr.Var (p.Handle) |]) |> ProvidedExpr.Create methodBase.Context

    /// Compute the Name or FullName property of a provided type, reporting appropriate errors
    let CheckAndComputeProvidedNameProperty(m, st: Tainted<ProvidedType>, proj, propertyString) =
        let name = 
            try st.PUntaint(proj, m) 
            with :? TypeProviderError as tpe -> 
                let newError = tpe.MapText((fun msg -> FSComp.SR.etProvidedTypeWithNameException(propertyString, msg)), st.TypeProviderDesignation, m)
                raise newError
        if String.IsNullOrEmpty name then
            raise (TypeProviderError(FSComp.SR.etProvidedTypeWithNullOrEmptyName propertyString, st.TypeProviderDesignation, m))
        name

    /// Verify that this type provider has supported attributes
    let ValidateAttributesOfProvidedType (m, st: Tainted<ProvidedType>) =         
        let fullName = CheckAndComputeProvidedNameProperty(m, st, (fun st -> st.FullName), "FullName")
        if TryTypeMember(st, fullName, "IsGenericType", m, false, fun st->st.IsGenericType) |> unmarshal then  
            errorR(Error(FSComp.SR.etMustNotBeGeneric fullName, m))  
        if TryTypeMember(st, fullName, "IsArray", m, false, fun st->st.IsArray) |> unmarshal then 
            errorR(Error(FSComp.SR.etMustNotBeAnArray fullName, m))  
        TryTypeMemberNonNull(st, fullName, "GetInterfaces", m, [||], fun st -> st.GetInterfaces()) |> ignore


    /// Verify that a provided type has the expected name
    let ValidateExpectedName m expectedPath expectedName (st : Tainted<ProvidedType>) =
        let name = CheckAndComputeProvidedNameProperty(m, st, (fun st -> st.Name), "Name")
        if name <> expectedName then
            raise (TypeProviderError(FSComp.SR.etProvidedTypeHasUnexpectedName(expectedName, name), st.TypeProviderDesignation, m))

        let namespaceName = TryTypeMember(st, name, "Namespace", m, "", fun st -> st.Namespace) |> unmarshal
        let rec declaringTypes (st: Tainted<ProvidedType>) accu =
            match TryTypeMember(st, name, "DeclaringType", m, null, fun st -> st.DeclaringType) with
            |   Tainted.Null -> accu
            |   dt -> declaringTypes dt (CheckAndComputeProvidedNameProperty(m, dt, (fun dt -> dt.Name), "Name") :: accu)
        let path = 
            [|  match namespaceName with 
                | null -> ()
                | _ -> yield! namespaceName.Split([|'.'|])
                yield! declaringTypes st [] |]
        
        if path <> expectedPath then
            let expectedPath = String.Join(".", expectedPath)
            let path = String.Join(".", path)
            errorR(Error(FSComp.SR.etProvidedTypeHasUnexpectedPath(expectedPath, path), m))

    /// Eagerly validate a range of conditions on a provided type, after static instantiation (if any) has occurred
    let ValidateProvidedTypeAfterStaticInstantiation(m, st: Tainted<ProvidedType>, expectedPath : string[], expectedName : string) = 
        // Do all the calling into st up front with recovery
        let fullName, namespaceName, usedMembers =
            let name = CheckAndComputeProvidedNameProperty(m, st, (fun st -> st.Name), "Name")
            let namespaceName = TryTypeMember(st, name, "Namespace", m, FSComp.SR.invalidNamespaceForProvidedType(), fun st -> st.Namespace) |> unmarshal
            let fullName = TryTypeMemberNonNull(st, name, "FullName", m, FSComp.SR.invalidFullNameForProvidedType(), fun st -> st.FullName) |> unmarshal
            ValidateExpectedName m expectedPath expectedName st
            // Must be able to call (GetMethods|GetEvents|GetPropeties|GetNestedTypes|GetConstructors)(bindingFlags).
            let usedMembers : Tainted<ProvidedMemberInfo>[] = 
                // These are the members the compiler will actually use
                [| for x in TryTypeMemberArray(st, fullName, "GetMethods", m, fun st -> st.GetMethods()) -> x.Coerce m
                   for x in TryTypeMemberArray(st, fullName, "GetEvents", m, fun st -> st.GetEvents()) -> x.Coerce m
                   for x in TryTypeMemberArray(st, fullName, "GetFields", m, fun st -> st.GetFields()) -> x.Coerce m
                   for x in TryTypeMemberArray(st, fullName, "GetProperties", m, fun st -> st.GetProperties()) -> x.Coerce m
                   // These will be validated on-demand
                   //for x in TryTypeMemberArray(st, fullName, "GetNestedTypes", m, fun st -> st.GetNestedTypes bindingFlags) -> x.Coerce()
                   for x in TryTypeMemberArray(st, fullName, "GetConstructors", m, fun st -> st.GetConstructors()) -> x.Coerce m |]
            fullName, namespaceName, usedMembers       

        // We scrutinize namespaces for invalid characters on open, but this provides better diagnostics
        ValidateNamespaceName(fullName, st.TypeProvider, m, namespaceName)

        ValidateAttributesOfProvidedType(m, st)

        // Those members must have this type.
        // This needs to be a *shallow* exploration. Otherwise, as in Freebase sample the entire database could be explored.
        for mi in usedMembers do
            match mi with 
            | Tainted.Null -> errorR(Error(FSComp.SR.etNullMember fullName, m))  
            | _ -> 
                let memberName = TryMemberMember(mi, fullName, "Name", "Name", m, "invalid provided type member name", fun mi -> mi.Name) |> unmarshal
                if String.IsNullOrEmpty memberName then 
                    errorR(Error(FSComp.SR.etNullOrEmptyMemberName fullName, m))  
                else 
                    let miDeclaringType = TryMemberMember(mi, fullName, memberName, "DeclaringType", m, ProvidedType.CreateNoContext(typeof<obj>), fun mi -> mi.DeclaringType)
                    match miDeclaringType with 
                        // Generated nested types may have null DeclaringType
                    | Tainted.Null when (mi.OfType<ProvidedType>().IsSome) -> ()
                    | Tainted.Null -> 
                        errorR(Error(FSComp.SR.etNullMemberDeclaringType(fullName, memberName), m))   
                    | _ ->     
                        let miDeclaringTypeFullName = 
                            TryMemberMember(miDeclaringType, fullName, memberName, "FullName", m, "invalid declaring type full name", fun miDeclaringType -> miDeclaringType.FullName)
                            |> unmarshal
                        if not (ProvidedType.TaintedEquals (st, miDeclaringType)) then 
                            errorR(Error(FSComp.SR.etNullMemberDeclaringTypeDifferentFromProvidedType(fullName, memberName, miDeclaringTypeFullName), m))   

                    match mi.OfType<ProvidedMethodInfo>() with
                    | Some mi ->
                        let isPublic = TryMemberMember(mi, fullName, memberName, "IsPublic", m, true, fun mi->mi.IsPublic) |> unmarshal
                        let isGenericMethod = TryMemberMember(mi, fullName, memberName, "IsGenericMethod", m, true, fun mi->mi.IsGenericMethod) |> unmarshal
                        if not isPublic || isGenericMethod then
                            errorR(Error(FSComp.SR.etMethodHasRequirements(fullName, memberName), m))   
                    |   None ->
                    match mi.OfType<ProvidedType>() with
                    |   Some subType -> ValidateAttributesOfProvidedType(m, subType)
                    |   None ->
                    match mi.OfType<ProvidedPropertyInfo>() with
                    | Some pi ->
                        // Property must have a getter or setter
                        // TODO: Property must be public etc.
                        let expectRead =
                             match TryMemberMember(pi, fullName, memberName, "GetGetMethod", m, null, fun pi -> pi.GetGetMethod()) with 
                             |  Tainted.Null -> false 
                             | _ -> true
                        let expectWrite = 
                            match TryMemberMember(pi, fullName, memberName, "GetSetMethod", m, null, fun pi-> pi.GetSetMethod()) with 
                            |   Tainted.Null -> false 
                            |   _ -> true
                        let canRead = TryMemberMember(pi, fullName, memberName, "CanRead", m, expectRead, fun pi-> pi.CanRead) |> unmarshal
                        let canWrite = TryMemberMember(pi, fullName, memberName, "CanWrite", m, expectWrite, fun pi-> pi.CanWrite) |> unmarshal
                        match expectRead, canRead with
                        | false, false | true, true-> ()
                        | false, true -> errorR(Error(FSComp.SR.etPropertyCanReadButHasNoGetter(memberName, fullName), m))   
                        | true, false -> errorR(Error(FSComp.SR.etPropertyHasGetterButNoCanRead(memberName, fullName), m))   
                        match expectWrite, canWrite with
                        | false, false | true, true-> ()
                        | false, true -> errorR(Error(FSComp.SR.etPropertyCanWriteButHasNoSetter(memberName, fullName), m))   
                        | true, false -> errorR(Error(FSComp.SR.etPropertyHasSetterButNoCanWrite(memberName, fullName), m))   
                        if not canRead && not canWrite then 
                            errorR(Error(FSComp.SR.etPropertyNeedsCanWriteOrCanRead(memberName, fullName), m))   

                    | None ->
                    match mi.OfType<ProvidedEventInfo>() with 
                    | Some ei ->
                        // Event must have adder and remover
                        // TODO: Event must be public etc.
                        let adder = TryMemberMember(ei, fullName, memberName, "GetAddMethod", m, null, fun ei-> ei.GetAddMethod())
                        let remover = TryMemberMember(ei, fullName, memberName, "GetRemoveMethod", m, null, fun ei-> ei.GetRemoveMethod())
                        match adder, remover with
                        | Tainted.Null, _ -> errorR(Error(FSComp.SR.etEventNoAdd(memberName, fullName), m))   
                        | _, Tainted.Null -> errorR(Error(FSComp.SR.etEventNoRemove(memberName, fullName), m))   
                        | _, _ -> ()
                    | None ->
                    match mi.OfType<ProvidedConstructorInfo>() with
                    | Some _  -> () // TODO: Constructors must be public etc.
                    | None ->
                    match mi.OfType<ProvidedFieldInfo>() with
                    | Some _ -> () // TODO: Fields must be public, literals must have a value etc.
                    | None ->
                        errorR(Error(FSComp.SR.etUnsupportedMemberKind(memberName, fullName), m))   

    let ValidateProvidedTypeDefinition(m, st: Tainted<ProvidedType>, expectedPath : string[], expectedName : string) = 

        // Validate the Name, Namespace and FullName properties
        let name = CheckAndComputeProvidedNameProperty(m, st, (fun st -> st.Name), "Name")
        let _namespaceName = TryTypeMember(st, name, "Namespace", m, FSComp.SR.invalidNamespaceForProvidedType(), fun st -> st.Namespace) |> unmarshal
        let _fullname = TryTypeMemberNonNull(st, name, "FullName", m, FSComp.SR.invalidFullNameForProvidedType(), fun st -> st.FullName)  |> unmarshal
        ValidateExpectedName m expectedPath expectedName st

        ValidateAttributesOfProvidedType(m, st)

        // This excludes, for example, types with '.' in them which would not be resolvable during name resolution.
        match expectedName.IndexOfAny(PrettyNaming.IllegalCharactersInTypeAndNamespaceNames) with
        | -1 -> ()
        | n -> errorR(Error(FSComp.SR.etIllegalCharactersInTypeName(string expectedName.[n], expectedName), m))  

        let staticParameters = st.PApplyWithProvider((fun (st, provider) -> st.GetStaticParameters provider), range=m) 
        if staticParameters.PUntaint((fun a -> a.Length), m)  = 0 then 
            ValidateProvidedTypeAfterStaticInstantiation(m, st, expectedPath, expectedName)


    /// Resolve a (non-nested) provided type given a full namespace name and a type name. 
    /// May throw an exception which will be turned into an error message by one of the 'Try' function below.
    /// If resolution is successful the type is then validated.
    let ResolveProvidedType (resolver: Tainted<ITypeProvider>, m, moduleOrNamespace: string[], typeName) =
        let displayName = String.Join(".", moduleOrNamespace)

        // Try to find the type in the given provided namespace
        let rec tryNamespace (providedNamespace: Tainted<IProvidedNamespace>) = 

            // Get the provided namespace name
            let providedNamespaceName = providedNamespace.PUntaint((fun providedNamespace -> providedNamespace.NamespaceName), range=m)

            // Check if the provided namespace name is an exact match of the required namespace name
            if displayName = providedNamespaceName then
                let resolvedType = providedNamespace.PApply((fun providedNamespace -> ProvidedType.CreateNoContext(providedNamespace.ResolveTypeName typeName)), range=m) 
                match resolvedType with
                |   Tainted.Null -> None
                |   result -> 
                    ValidateProvidedTypeDefinition(m, result, moduleOrNamespace, typeName)
                    Some result
            else
                // Note: This eagerly explores all provided namespaces even if there is no match of even a prefix in the
                // namespace names. 
                let providedNamespaces = providedNamespace.PApplyArray((fun providedNamespace -> providedNamespace.GetNestedNamespaces()), "GetNestedNamespaces", range=m)
                tryNamespaces providedNamespaces

        and tryNamespaces (providedNamespaces: Tainted<IProvidedNamespace>[]) = 
            providedNamespaces |> Array.tryPick tryNamespace

        let providedNamespaces = resolver.PApplyArray((fun resolver -> resolver.GetNamespaces()), "GetNamespaces", range=m)
        match tryNamespaces providedNamespaces with 
        | None -> resolver.PApply((fun _ -> null), m)
        | Some res -> res
                    
    /// Try to resolve a type against the given host with the given resolution environment.
    let TryResolveProvidedType(resolver: Tainted<ITypeProvider>, m, moduleOrNamespace, typeName) =
        try 
            match ResolveProvidedType(resolver, m, moduleOrNamespace, typeName) with
            | Tainted.Null -> None
            | ty -> Some ty
        with e -> 
            errorRecovery e m
            None

    let ILPathToProvidedType  (st: Tainted<ProvidedType>, m) = 
        let nameContrib (st: Tainted<ProvidedType>) = 
            let typeName = st.PUntaint((fun st -> st.Name), m)
            match st.PApply((fun st -> st.DeclaringType), m) with 
            | Tainted.Null -> 
               match st.PUntaint((fun st -> st.Namespace), m) with 
               | null -> typeName
               | ns -> ns + "." + typeName
            | _ -> typeName

        let rec encContrib (st: Tainted<ProvidedType>) = 
            match st.PApply((fun st ->st.DeclaringType), m) with 
            | Tainted.Null -> []
            | enc -> encContrib enc @ [ nameContrib enc ]

        encContrib st, nameContrib st

    let ComputeMangledNameForApplyStaticParameters(nm, staticArgs, staticParams: Tainted<ProvidedParameterInfo[]>, m) =
        let defaultArgValues = 
            staticParams.PApply((fun ps ->  ps |> Array.map (fun sp -> sp.Name, (if sp.IsOptional then Some (string sp.RawDefaultValue) else None ))), range=m)

        let defaultArgValues = defaultArgValues.PUntaint(id, m)
        PrettyNaming.computeMangledNameWithoutDefaultArgValues(nm, staticArgs, defaultArgValues)

    /// Apply the given provided method to the given static arguments (the arguments are assumed to have been sorted into application order)
    let TryApplyProvidedMethod(methBeforeArgs: Tainted<ProvidedMethodBase>, staticArgs: obj[], m: range) =
        if staticArgs.Length = 0 then 
            Some methBeforeArgs
        else
            let mangledName = 
                let nm = methBeforeArgs.PUntaint((fun x -> x.Name), m)
                let staticParams = methBeforeArgs.PApplyWithProvider((fun (mb, resolver) -> mb.GetStaticParametersForMethod resolver), range=m) 
                let mangledName = ComputeMangledNameForApplyStaticParameters(nm, staticArgs, staticParams, m)
                mangledName
 
            match methBeforeArgs.PApplyWithProvider((fun (mb, provider) -> mb.ApplyStaticArgumentsForMethod(provider, mangledName, staticArgs)), range=m) with 
            | Tainted.Null -> None
            | methWithArguments -> 
                let actualName = methWithArguments.PUntaint((fun x -> x.Name), m)
                if actualName <> mangledName then 
                    error(Error(FSComp.SR.etProvidedAppliedMethodHadWrongName(methWithArguments.TypeProviderDesignation, mangledName, actualName), m))
                Some methWithArguments


    /// Apply the given provided type to the given static arguments (the arguments are assumed to have been sorted into application order
    let TryApplyProvidedType(typeBeforeArguments: Tainted<ProvidedType>, optGeneratedTypePath: string list option, staticArgs: obj[], m: range) =
        if staticArgs.Length = 0 then 
            Some (typeBeforeArguments, (fun () -> ()))
        else 
            
            let fullTypePathAfterArguments = 
                // If there is a generated type name, then use that
                match optGeneratedTypePath with 
                | Some path -> path
                | None -> 
                    // Otherwise, use the full path of the erased type, including mangled arguments
                    let nm = typeBeforeArguments.PUntaint((fun x -> x.Name), m)
                    let enc, _ = ILPathToProvidedType (typeBeforeArguments, m)
                    let staticParams = typeBeforeArguments.PApplyWithProvider((fun (mb, resolver) -> mb.GetStaticParameters resolver), range=m) 
                    let mangledName = ComputeMangledNameForApplyStaticParameters(nm, staticArgs, staticParams, m)
                    enc @ [ mangledName ]
 
            match typeBeforeArguments.PApplyWithProvider((fun (typeBeforeArguments, provider) -> typeBeforeArguments.ApplyStaticArguments(provider, Array.ofList fullTypePathAfterArguments, staticArgs)), range=m) with 
            | Tainted.Null -> None
            | typeWithArguments -> 
                let actualName = typeWithArguments.PUntaint((fun x -> x.Name), m)
                let checkTypeName() = 
                    let expectedTypeNameAfterArguments = fullTypePathAfterArguments.[fullTypePathAfterArguments.Length-1]
                    if actualName <> expectedTypeNameAfterArguments then 
                        error(Error(FSComp.SR.etProvidedAppliedTypeHadWrongName(typeWithArguments.TypeProviderDesignation, expectedTypeNameAfterArguments, actualName), m))
                Some (typeWithArguments, checkTypeName)

    /// Given a mangled name reference to a non-nested provided type, resolve it.
    /// If necessary, demangle its static arguments before applying them.
    let TryLinkProvidedType(resolver: Tainted<ITypeProvider>, moduleOrNamespace: string[], typeLogicalName: string, m: range) =
        
        // Demangle the static parameters
        let typeName, argNamesAndValues = 
            try 
                PrettyNaming.demangleProvidedTypeName typeLogicalName 
            with PrettyNaming.InvalidMangledStaticArg piece -> 
                error(Error(FSComp.SR.etProvidedTypeReferenceInvalidText piece, range0)) 

        let argSpecsTable = dict argNamesAndValues
        let typeBeforeArguments = ResolveProvidedType(resolver, range0, moduleOrNamespace, typeName) 

        match typeBeforeArguments with 
        | Tainted.Null -> None
        | _ -> 
            // Take the static arguments (as strings, taken from the text in the reference we're relinking), 
            // and convert them to objects of the appropriate type, based on the expected kind.
            let staticParameters = typeBeforeArguments.PApplyWithProvider((fun (typeBeforeArguments, resolver) -> typeBeforeArguments.GetStaticParameters resolver), range=range0)

            let staticParameters = staticParameters.PApplyArray(id, "", m)
            
            let staticArgs = 
                staticParameters |> Array.map (fun sp -> 
                      let typeBeforeArgumentsName = typeBeforeArguments.PUntaint ((fun st -> st.Name), m)
                      let spName = sp.PUntaint ((fun sp -> sp.Name), m)
                      match argSpecsTable.TryGetValue spName with
                      | true, arg ->
                          /// Find the name of the representation type for the static parameter
                          let spReprTypeName = 
                              sp.PUntaint((fun sp -> 
                                  let pt = sp.ParameterType 
                                  let ut = pt.RawSystemType
                                  let uet = if pt.IsEnum then ut.GetEnumUnderlyingType() else ut
                                  uet.FullName), m)

                          match spReprTypeName with 
                          | "System.SByte" -> box (sbyte arg)
                          | "System.Int16" -> box (int16 arg)
                          | "System.Int32" -> box (int32 arg)
                          | "System.Int64" -> box (int64 arg)
                          | "System.Byte" -> box (byte arg)
                          | "System.UInt16" -> box (uint16 arg)
                          | "System.UInt32" -> box (uint32 arg)
                          | "System.UInt64" -> box (uint64 arg)
                          | "System.Decimal" -> box (decimal arg)
                          | "System.Single" -> box (single arg)
                          | "System.Double" -> box (double arg)
                          | "System.Char" -> box (char arg)
                          | "System.Boolean" -> box (arg = "True")
                          | "System.String" -> box (string arg)
                          | s -> error(Error(FSComp.SR.etUnknownStaticArgumentKind(s, typeLogicalName), range0))

                      | _ ->
                          if sp.PUntaint ((fun sp -> sp.IsOptional), m) then 
                              match sp.PUntaint((fun sp -> sp.RawDefaultValue), m) with
                              | null -> error (Error(FSComp.SR.etStaticParameterRequiresAValue (spName, typeBeforeArgumentsName, typeBeforeArgumentsName, spName), range0))
                              | v -> v
                          else
                              error(Error(FSComp.SR.etProvidedTypeReferenceMissingArgument spName, range0)))
                    

            match TryApplyProvidedType(typeBeforeArguments, None, staticArgs, range0) with 
            | Some (typeWithArguments, checkTypeName) -> 
                checkTypeName() 
                Some typeWithArguments
            | None -> None

    /// Get the parts of a .NET namespace. Special rules: null means global, empty is not allowed.
    let GetPartsOfNamespaceRecover(namespaceName: string) = 
        if namespaceName=null then []
        elif  namespaceName.Length = 0 then ["<NonExistentNamespace>"]
        else splitNamespace namespaceName

    /// Get the parts of a .NET namespace. Special rules: null means global, empty is not allowed.
    let GetProvidedNamespaceAsPath (m, resolver: Tainted<ITypeProvider>, namespaceName: string) = 
        if namespaceName<>null && namespaceName.Length = 0 then
            errorR(Error(FSComp.SR.etEmptyNamespaceNotAllowed(DisplayNameOfTypeProvider(resolver.TypeProvider, m)), m))  

        GetPartsOfNamespaceRecover namespaceName

    /// Get the parts of the name that encloses the .NET type including nested types. 
    let GetFSharpPathToProvidedType (st: Tainted<ProvidedType>, m) = 
        // Can't use st.Fullname because it may be like IEnumerable<Something>
        // We want [System;Collections;Generic]
        let namespaceParts = GetPartsOfNamespaceRecover(st.PUntaint((fun st -> st.Namespace), m))
        let rec walkUpNestedClasses(st: Tainted<ProvidedType>, soFar) =
            match st with
            | Tainted.Null -> soFar
            | st -> walkUpNestedClasses(st.PApply((fun st ->st.DeclaringType), m), soFar) @ [st.PUntaint((fun st -> st.Name), m)]

        walkUpNestedClasses(st.PApply((fun st ->st.DeclaringType), m), namespaceParts)


    /// Get the ILAssemblyRef for a provided assembly. Do not take into account
    /// any type relocations or static linking for generated types.
    let GetOriginalILAssemblyRefOfProvidedAssembly (assembly: Tainted<ProvidedAssembly>, m) =
        let aname = assembly.PUntaint((fun assembly -> assembly.GetName()), m)
        ILAssemblyRef.FromAssemblyName aname

    /// Get the ILTypeRef for the provided type (including for nested types). Do not take into account
    /// any type relocations or static linking for generated types.
    let GetOriginalILTypeRefOfProvidedType (st: Tainted<ProvidedType>, m) = 
        
        let aref = GetOriginalILAssemblyRefOfProvidedAssembly (st.PApply((fun st -> st.Assembly), m), m)
        let scoperef = ILScopeRef.Assembly aref
        let enc, nm = ILPathToProvidedType (st, m)
        let tref = ILTypeRef.Create(scoperef, enc, nm)
        tref

    /// Get the ILTypeRef for the provided type (including for nested types). Take into account
    /// any type relocations or static linking for generated types.
    let GetILTypeRefOfProvidedType (st: Tainted<ProvidedType>, m) = 
        match st.PUntaint((fun st -> st.TryGetILTypeRef()), m) with 
        | Some ilTypeRef -> ilTypeRef
        | None -> GetOriginalILTypeRefOfProvidedType (st, m)

    type ProviderGeneratedType = ProviderGeneratedType of (*ilOrigTyRef*)ILTypeRef * (*ilRenamedTyRef*)ILTypeRef * ProviderGeneratedType list

    /// The table of information recording remappings from type names in the provided assembly to type
    /// names in the statically linked, embedded assembly, plus what types are nested in side what types.
    type ProvidedAssemblyStaticLinkingMap = 
        { ILTypeMap: System.Collections.Generic.Dictionary<ILTypeRef, ILTypeRef> }
        static member CreateNew() = 
            { ILTypeMap = System.Collections.Generic.Dictionary() }

    /// Check if this is a direct reference to a non-embedded generated type. This is not permitted at any name resolution.
    /// We check by seeing if the type is absent from the remapping context.
    let IsGeneratedTypeDirectReference (st: Tainted<ProvidedType>, m) =
        st.PUntaint((fun st -> st.TryGetTyconRef() |> Option.isNone), m)

#endif
