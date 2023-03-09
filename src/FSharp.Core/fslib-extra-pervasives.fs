// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Core

module ExtraTopLevelOperators =

    open System
    open System.Collections.Generic
    open System.ComponentModel
    open System.IO
    open System.Diagnostics
    open Microsoft.FSharp
    open Microsoft.FSharp.Core
    open Microsoft.FSharp.Core.Operators
    open Microsoft.FSharp.Collections
    open Microsoft.FSharp.Control
    open Microsoft.FSharp.Primitives.Basics
    open Microsoft.FSharp.Core.CompilerServices

    [<CompiledName("CreateSet"); EditorBrowsableAttribute(EditorBrowsableState.Never)>]
    let set elements =
        Collections.Set.ofSeq elements

    [<CompiledName("CreateDictionary"); EditorBrowsableAttribute(EditorBrowsableState.Never)>]
    let dict (keyValuePairs: seq<'Key * 'T>) : IDictionary<'Key, 'T> =
        SetGlobalOperators.dict keyValuePairs

    [<CompiledName("CreateReadOnlyDictionary"); EditorBrowsableAttribute(EditorBrowsableState.Never)>]
    let readOnlyDict (keyValuePairs: seq<'Key * 'T>) : IReadOnlyDictionary<'Key, 'T> =
        SetGlobalOperators.readOnlyDict keyValuePairs

    [<CompiledName("CreateArray2D")>]
    let array2D (rows: seq<#seq<'T>>) =
        SeqGlobalOperators.array2D rows

    [<CompiledName("PrintFormatToString"); EditorBrowsableAttribute(EditorBrowsableState.Never)>]
    let sprintf format =
        Printf.sprintf format

    [<CompiledName("PrintFormatToStringThenFail"); EditorBrowsableAttribute(EditorBrowsableState.Never)>]
    let failwithf format =
        Printf.failwithf format

    [<CompiledName("PrintFormatToTextWriter"); EditorBrowsableAttribute(EditorBrowsableState.Never)>]
    let fprintf (textWriter: TextWriter) format =
        Printf.fprintf textWriter format

    [<CompiledName("PrintFormatLineToTextWriter"); EditorBrowsableAttribute(EditorBrowsableState.Never)>]
    let fprintfn (textWriter: TextWriter) format =
        Printf.fprintfn textWriter format

    [<CompiledName("PrintFormat"); EditorBrowsableAttribute(EditorBrowsableState.Never)>]
    let printf format =
        Printf.printf format

    [<CompiledName("PrintFormatToError"); EditorBrowsableAttribute(EditorBrowsableState.Never)>]
    let eprintf format =
        Printf.eprintf format

    [<CompiledName("PrintFormatLine"); EditorBrowsableAttribute(EditorBrowsableState.Never)>]
    let printfn format =
        Printf.printfn format

    [<CompiledName("PrintFormatLineToError"); EditorBrowsableAttribute(EditorBrowsableState.Never)>]
    let eprintfn format =
        Printf.eprintfn format

    [<CompiledName("DefaultAsyncBuilder"); EditorBrowsableAttribute(EditorBrowsableState.Never)>]
    let async = AsyncBuilder()

    [<CompiledName("ToSingle"); EditorBrowsableAttribute(EditorBrowsableState.Never)>]
    let inline single value =
        float32 value

    [<CompiledName("ToDouble"); EditorBrowsableAttribute(EditorBrowsableState.Never)>]
    let inline double value =
        float value

    [<CompiledName("ToByte"); EditorBrowsableAttribute(EditorBrowsableState.Never)>]
    let inline uint8 value =
        byte value

    [<CompiledName("ToSByte"); EditorBrowsableAttribute(EditorBrowsableState.Never)>]
    let inline int8 value =
        sbyte value

    module Checked =

        [<CompiledName("ToByte"); EditorBrowsableAttribute(EditorBrowsableState.Never)>]
        let inline uint8 value =
            Checked.byte value

        [<CompiledName("ToSByte"); EditorBrowsableAttribute(EditorBrowsableState.Never)>]
        let inline int8 value =
            Checked.sbyte value

    [<CompiledName("SpliceExpression")>]
    let (~%) (expression: Microsoft.FSharp.Quotations.Expr<'T>) : 'T =
        ignore expression
        raise (InvalidOperationException(SR.GetString(SR.firstClassUsesOfSplice)))

    [<CompiledName("SpliceUntypedExpression")>]
    let (~%%) (expression: Microsoft.FSharp.Quotations.Expr) : 'T =
        ignore expression
        raise (InvalidOperationException(SR.GetString(SR.firstClassUsesOfSplice)))

    [<CompiledName("LazyPattern"); EditorBrowsableAttribute(EditorBrowsableState.Never)>]
    let (|Lazy|) (input: Lazy<_>) =
        input.Force()

    [<EditorBrowsableAttribute(EditorBrowsableState.Never)>]
    let query = Microsoft.FSharp.Linq.QueryBuilder()

namespace Microsoft.FSharp.Core.CompilerServices

open System
open System.Reflection
open Microsoft.FSharp.Core
open Microsoft.FSharp.Control
open Microsoft.FSharp.Quotations

/// <summary>Represents the product of two measure expressions when returned as a generic argument of a provided type.</summary>
[<Sealed>]
type MeasureProduct<'Measure1, 'Measure2>() =
    class
    end

/// <summary>Represents the inverse of a measure expressions when returned as a generic argument of a provided type.</summary>
[<Sealed>]
type MeasureInverse<'Measure> =
    class
    end

/// <summary>Represents the '1' measure expression when returned as a generic argument of a provided type.</summary>
[<Sealed>]
type MeasureOne =
    class
    end

[<AttributeUsage(AttributeTargets.Class, AllowMultiple = false)>]
type TypeProviderAttribute() =
    inherit System.Attribute()

[<AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)>]
type TypeProviderAssemblyAttribute(assemblyName: string) =
    inherit System.Attribute()
    new() = TypeProviderAssemblyAttribute(null)

    member _.AssemblyName = assemblyName

[<AttributeUsage(AttributeTargets.All, AllowMultiple = false)>]
type TypeProviderXmlDocAttribute(commentText: string) =
    inherit System.Attribute()

    member _.CommentText = commentText

[<AttributeUsage(AttributeTargets.All, AllowMultiple = false)>]
type TypeProviderDefinitionLocationAttribute() =
    inherit System.Attribute()
    let mutable filePath: string = null
    let mutable line: int = 0
    let mutable column: int = 0

    member _.FilePath
        with get () = filePath
        and set v = filePath <- v

    member _.Line
        with get () = line
        and set v = line <- v

    member _.Column
        with get () = column
        and set v = column <- v

[<AttributeUsage(AttributeTargets.Class
                 ||| AttributeTargets.Interface
                 ||| AttributeTargets.Struct
                 ||| AttributeTargets.Delegate,
                 AllowMultiple = false)>]
type TypeProviderEditorHideMethodsAttribute() =
    inherit System.Attribute()

/// <summary>Additional type attribute flags related to provided types</summary>
type TypeProviderTypeAttributes =
    | SuppressRelocate = 0x80000000
    | IsErased = 0x40000000

type TypeProviderConfig
    (
        systemRuntimeContainsType: string -> bool,
        getReferencedAssembliesOption: (unit -> string array) option
    ) =
    let mutable resolutionFolder: string = null
    let mutable runtimeAssembly: string = null
    let mutable referencedAssemblies: string[] = null
    let mutable temporaryFolder: string = null
    let mutable isInvalidationSupported: bool = false
    let mutable useResolutionFolderAtRuntime: bool = false
    let mutable systemRuntimeAssemblyVersion: System.Version = null

    new(systemRuntimeContainsType) = TypeProviderConfig(systemRuntimeContainsType, getReferencedAssembliesOption = None)

    new(systemRuntimeContainsType, getReferencedAssemblies) =
        TypeProviderConfig(systemRuntimeContainsType, getReferencedAssembliesOption = Some getReferencedAssemblies)

    member _.ResolutionFolder
        with get () = resolutionFolder
        and set v = resolutionFolder <- v

    member _.RuntimeAssembly
        with get () = runtimeAssembly
        and set v = runtimeAssembly <- v

    member _.ReferencedAssemblies
        with get () =
            match getReferencedAssembliesOption with
            | None -> referencedAssemblies
            | Some f -> f ()

        and set v =
            match getReferencedAssembliesOption with
            | None -> referencedAssemblies <- v
            | Some _ -> raise (InvalidOperationException())

    member _.TemporaryFolder
        with get () = temporaryFolder
        and set v = temporaryFolder <- v

    member _.IsInvalidationSupported
        with get () = isInvalidationSupported
        and set v = isInvalidationSupported <- v

    member _.IsHostedExecution
        with get () = useResolutionFolderAtRuntime
        and set v = useResolutionFolderAtRuntime <- v

    member _.SystemRuntimeAssemblyVersion
        with get () = systemRuntimeAssemblyVersion
        and set v = systemRuntimeAssemblyVersion <- v

    member _.SystemRuntimeContainsType(typeName: string) =
        systemRuntimeContainsType typeName

type IProvidedNamespace =

    abstract NamespaceName: string

    abstract GetNestedNamespaces: unit -> IProvidedNamespace[]

    abstract GetTypes: unit -> Type[]

    abstract ResolveTypeName: typeName: string -> Type

type ITypeProvider =
    inherit System.IDisposable

    abstract GetNamespaces: unit -> IProvidedNamespace[]

    abstract GetStaticParameters: typeWithoutArguments: Type -> ParameterInfo[]

    abstract ApplyStaticArguments:
        typeWithoutArguments: Type * typePathWithArguments: string[] * staticArguments: obj[] -> Type

    abstract GetInvokerExpression: syntheticMethodBase: MethodBase * parameters: Expr[] -> Expr

    [<CLIEvent>]
    abstract Invalidate: IEvent<System.EventHandler, System.EventArgs>

    abstract GetGeneratedAssemblyContents: assembly: System.Reflection.Assembly -> byte[]

type ITypeProvider2 =
    abstract GetStaticParametersForMethod: methodWithoutArguments: MethodBase -> ParameterInfo[]

    abstract ApplyStaticArgumentsForMethod:
        methodWithoutArguments: MethodBase * methodNameWithArguments: string * staticArguments: obj[] -> MethodBase
