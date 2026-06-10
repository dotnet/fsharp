module internal FSharp.Compiler.HotReload.SymbolMatcher

open System
open System.Collections.Generic
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.HotReloadBaseline
open FSharp.Compiler.Syntax.PrettyNaming
open FSharp.Compiler.SynthesizedTypeMaps

type internal TypeMatch =
    { EnclosingTypes: ILTypeDef list
      TypeDef: ILTypeDef }

type internal MethodMatch =
    { EnclosingTypes: ILTypeDef list
      TypeDef: ILTypeDef
      MethodDef: ILMethodDef }

type FSharpSymbolMatcher =
    {
        TypeMatches: IReadOnlyDictionary<string, TypeMatch>
        MethodMatches: IReadOnlyDictionary<MethodDefinitionKey, MethodMatch>
    }

module FSharpSymbolMatcher =

    let private addMethodMatch
        (typeRef: ILTypeRef)
        (enclosing: ILTypeDef list)
        (typeDef: ILTypeDef)
        (methodDef: ILMethodDef)
        (destination: Dictionary<MethodDefinitionKey, MethodMatch>)
        =
        let key =
            { DeclaringType = typeRef.FullName
              Name = methodDef.Name
              GenericArity = methodDef.GenericParams.Length
              ParameterTypes = methodDef.ParameterTypes
              ReturnType = methodDef.Return.Type }

        destination[key] <-
            { EnclosingTypes = enclosing
              TypeDef = typeDef
              MethodDef = methodDef }

    [<Literal>]
    let private MaxNestedTypeDepth = 100

    let rec private addTypeMatches
        (synthesizedBuckets: Dictionary<string, string[]> option)
        (enclosing: ILTypeDef list)
        (types: Dictionary<string, TypeMatch>)
        (methods: Dictionary<MethodDefinitionKey, MethodMatch>)
        (depth: int)
        (typeDef: ILTypeDef)
        =
        if depth > MaxNestedTypeDepth then
            failwith $"Exceeded maximum nested type depth ({MaxNestedTypeDepth}) while processing type '{typeDef.Name}'. Possible malformed IL."
        let typeRef = mkRefForNestedILTypeDef ILScopeRef.Local (enclosing, typeDef)
        types[typeRef.FullName] <-
            { EnclosingTypes = enclosing
              TypeDef = typeDef }

        match synthesizedBuckets with
        | Some buckets when IsCompilerGeneratedName typeDef.Name ->
            let basicName = GetBasicNameOfPossibleCompilerGeneratedName typeDef.Name
            match buckets.TryGetValue basicName with
            | true, aliases when aliases.Length > 0 ->
                // Compute prefix directly from typeRef structure rather than string manipulation
                // on FullName, which can fail for generic types or mangled names
                let prefix =
                    match typeRef.Enclosing with
                    | [] ->
                        // Top-level type: Name may include namespace (e.g., "Namespace.TypeName")
                        match typeRef.Name.LastIndexOf('.') with
                        | -1 -> ""
                        | idx -> typeRef.Name.Substring(0, idx + 1)
                    | enclosing ->
                        // Nested type: prefix is enclosing path + "."
                        String.concat "." enclosing + "."

                for alias in aliases do
                    if alias <> typeDef.Name then
                        let aliasFullName = prefix + alias
                        if not (types.ContainsKey aliasFullName) then
                            types[aliasFullName] <-
                                { EnclosingTypes = enclosing
                                  TypeDef = typeDef }
            | _ -> ()
        | _ -> ()

        typeDef.Methods.AsList()
        |> List.iter (fun methodDef ->
            addMethodMatch typeRef enclosing typeDef methodDef methods)

        typeDef.NestedTypes.AsList()
        |> List.iter (fun nested -> addTypeMatches synthesizedBuckets (enclosing @ [ typeDef ]) types methods (depth + 1) nested)

    let private createInternal (moduleDef: ILModuleDef) (synthesized: Dictionary<string, string[]> option) : FSharpSymbolMatcher =
        let typeMatches = Dictionary<string, TypeMatch>()
        let methodMatches = Dictionary<MethodDefinitionKey, MethodMatch>()

        moduleDef.TypeDefs.AsList()
        |> List.iter (addTypeMatches synthesized [] typeMatches methodMatches 0)

        { TypeMatches = typeMatches :> IReadOnlyDictionary<string, TypeMatch>
          MethodMatches = methodMatches :> IReadOnlyDictionary<MethodDefinitionKey, MethodMatch> }

    let create (moduleDef: ILModuleDef) : FSharpSymbolMatcher =
        createInternal moduleDef None

    let createWithSynthesizedNames (moduleDef: ILModuleDef) (synthesizedMap: FSharpSynthesizedTypeMaps) : FSharpSymbolMatcher =
        let buckets = Dictionary<string, string[]>(StringComparer.Ordinal)
        for struct (basic, names) in synthesizedMap.Snapshot do
            buckets[basic] <- names

        createInternal moduleDef (Some buckets)

    let tryGetTypeDef (matcher: FSharpSymbolMatcher) (fullName: string) =
        match matcher.TypeMatches.TryGetValue fullName with
        | true, matchInfo -> Some(matchInfo.EnclosingTypes, matchInfo.TypeDef)
        | _ -> None

    let tryGetMethodDef (matcher: FSharpSymbolMatcher) (key: MethodDefinitionKey) =
        match matcher.MethodMatches.TryGetValue key with
        | true, matchInfo -> Some(matchInfo.EnclosingTypes, matchInfo.TypeDef, matchInfo.MethodDef)
        | _ -> None
