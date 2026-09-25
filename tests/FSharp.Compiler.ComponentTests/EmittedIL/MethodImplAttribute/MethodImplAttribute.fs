namespace EmittedIL

open System.Reflection
open System.Reflection.Metadata
open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module MethodImplAttribute =

    let verifyCompilation compilation =
        compilation
        |> withOptions [ "--test:EmitFeeFeeAs100001" ]
        |> asExe
        |> withNoOptimize
        |> withEmbeddedPdb
        |> withEmbedAllSource
        |> ignoreWarnings
        |> compile
        |> verifyILBaseline

    // SOURCE=MethodImplAttribute.ForwardRef.fs        SCFLAGS="-a -g --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd MethodImplAttribute.ForwardRef.dll"	# MethodImplAttribute.ForwardRef.fs
    [<Theory; FileInlineData("MethodImplAttribute.ForwardRef.fs")>]
    let ``ForwardRef_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=MethodImplAttribute.InternalCall.fs      SCFLAGS="-a -g --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd MethodImplAttribute.InternalCall.dll"	# MethodImplAttribute.InternalCall.fs
    [<Theory; FileInlineData("MethodImplAttribute.InternalCall.fs")>]
    let ``InternalCall_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=MethodImplAttribute.NoInlining.fs        SCFLAGS="-a -g --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd MethodImplAttribute.NoInlining.dll"	# MethodImplAttribute.NoInlining.fs
    [<Theory; FileInlineData("MethodImplAttribute.NoInlining.fs")>]
    let ``NoInlining_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    [<Theory; FileInlineData("MethodImplAttribute.NoInlining_InlineKeyword.fs")>]
    let ``NoInlining_fs with inline keyword => should warn in preview version`` compilation =
        compilation
        |> getCompilation
        |> withLangVersion80
        |> typecheck
        |> withSingleDiagnostic (Warning 3151, Line 3, Col 12, Line 3, Col 19, "This member, function or value declaration may not be declared 'inline'")

    // SOURCE=MethodImplAttribute.AggressiveInlining.fs SCFLAGS="-a -g --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd MethodImplAttribute.AggressiveInlining.dll"	# MethodImplAttribute.AggressiveInlining.fs
    [<Theory; FileInlineData("MethodImplAttribute.AggressiveInlining.fs")>]
    let ``AggressiveInlining_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=MethodImplAttribute.NoOptimization.fs    SCFLAGS="-a -g --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd MethodImplAttribute.NoOptimization.dll"	# MethodImplAttribute.NoOptimization.fs
    [<Theory; FileInlineData("MethodImplAttribute.NoOptimization.fs")>]
    let ``NoOptimization_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=MethodImplAttribute.PreserveSig.fs       SCFLAGS="-a -g --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd MethodImplAttribute.PreserveSig.dll"	# MethodImplAttribute.PreserveSig.fs
    [<Theory; FileInlineData("MethodImplAttribute.PreserveSig.fs")>]
    let ``PreserveSig_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=MethodImplAttribute.Synchronized.fs      SCFLAGS="-a -g --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd MethodImplAttribute.Synchronized.dll"	# MethodImplAttribute.Synchronized.fs
    [<Theory; FileInlineData("MethodImplAttribute.Synchronized.fs")>]
    let ``Synchronized_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=MethodImplAttribute.Unmanaged.fs         SCFLAGS="-a -g --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd MethodImplAttribute.Unmanaged.dll"	# MethodImplAttribute.Unmanaged.fs
    [<Theory; FileInlineData("MethodImplAttribute.Unmanaged.fs")>]
    let ``Unmanaged_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    let private verifyMetadata expectedMethods expectedProperties compilation =
        compilation
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> withMetadataReader (fun reader ->
            let qualify ns name =
                if ns = "" then name else ns + "." + name

            let rec typeName (handle: EntityHandle) =
                match handle.Kind with
                | HandleKind.TypeDefinition ->
                    let def = reader.GetTypeDefinition(TypeDefinitionHandle.op_Explicit handle)
                    let name = reader.GetString def.Name
                    let parent = def.GetDeclaringType()

                    if parent.IsNil then
                        qualify (reader.GetString def.Namespace) name
                    else
                        typeName (TypeDefinitionHandle.op_Implicit parent) + "+" + name
                | HandleKind.TypeReference ->
                    let reference = reader.GetTypeReference(TypeReferenceHandle.op_Explicit handle)
                    qualify (reader.GetString reference.Namespace) (reader.GetString reference.Name)
                | kind -> failwithf "Unexpected attribute/type handle: %A" kind

            let attributes handles =
                [
                    for handle in handles do
                        let attribute = reader.GetCustomAttribute handle

                        let parent =
                            match attribute.Constructor.Kind with
                            | HandleKind.MemberReference ->
                                reader.GetMemberReference(MemberReferenceHandle.op_Explicit attribute.Constructor).Parent
                            | HandleKind.MethodDefinition ->
                                let ctor =
                                    reader.GetMethodDefinition(MethodDefinitionHandle.op_Explicit attribute.Constructor)

                                TypeDefinitionHandle.op_Implicit (ctor.GetDeclaringType())
                            | kind -> failwithf "Unexpected attribute constructor: %A" kind

                        yield typeName parent
                ]

            let exactlyOne identity items =
                match Seq.toList items with
                | [ item ] -> item
                | items -> failwithf "Expected exactly one %s; found %d" identity items.Length

            let findType name =
                reader.TypeDefinitions
                |> Seq.filter (fun handle -> typeName (TypeDefinitionHandle.op_Implicit handle) = name)
                |> exactlyOne name
                |> reader.GetTypeDefinition

            let markers =
                [
                    "Markers.PropertyMarkerAttribute"
                    "Markers.GetterMarkerAttribute"
                    "Markers.SetterMarkerAttribute"
                ]

            let markerAttributes =
                List.filter (fun name -> List.contains name markers) >> List.sort

            let pseudoAttributes =
                List.filter (fun name ->
                    name = "System.Runtime.CompilerServices.MethodImplAttribute"
                    || name = "System.Runtime.InteropServices.PreserveSigAttribute")
                >> List.sort

            let actualMethods =
                [
                    for declaringType, name, _, isStatic, _ in expectedMethods do
                        let identity = declaringType + "." + name

                        let methods =
                            (findType declaringType).GetMethods()
                            |> Seq.map reader.GetMethodDefinition
                            |> Seq.toList

                        let methodDef =
                            methods
                            |> List.filter (fun def -> reader.GetString def.Name = name)
                            |> exactlyOne (
                                sprintf "%s (available: %A)" identity (methods |> List.map (fun def -> reader.GetString def.Name))
                            )

                        Assert.False(methodDef.Attributes.HasFlag MethodAttributes.Abstract, identity)
                        Assert.True(methodDef.RelativeVirtualAddress <> 0, identity + " must have a body")
                        Assert.True(methodDef.Attributes.HasFlag MethodAttributes.Static = isStatic, identity + " static flag")
                        let attrs = attributes (methodDef.GetCustomAttributes())
                        yield identity, int methodDef.ImplAttributes, pseudoAttributes attrs, markerAttributes attrs
                ]

            let expectedMethods =
                [
                    for declaringType, name, flags, _, markers in expectedMethods do
                        yield declaringType + "." + name, flags, [], List.sort markers
                ]

            Assert.True(
                (expectedMethods = actualMethods),
                sprintf "Expected metadata:\n%A\nActual metadata:\n%A" expectedMethods actualMethods
            )

            for declaringType, name, expectedMarkers in expectedProperties do
                let identity = declaringType + "." + name

                let property =
                    (findType declaringType).GetProperties()
                    |> Seq.map reader.GetPropertyDefinition
                    |> Seq.filter (fun def -> reader.GetString def.Name = name)
                    |> exactlyOne identity

                let actualMarkers = attributes (property.GetCustomAttributes()) |> markerAttributes
                Assert.True(List.sort expectedMarkers = actualMarkers, sprintf "%s markers: %A" identity actualMarkers))

    let private markerSource =
        """
namespace Markers
open System
[<AttributeUsage(AttributeTargets.Property)>]
type PropertyMarkerAttribute() = inherit Attribute()
[<AttributeUsage(AttributeTargets.Method)>]
type GetterMarkerAttribute() = inherit Attribute()
[<AttributeUsage(AttributeTargets.Method)>]
type SetterMarkerAttribute() = inherit Attribute()
"""

    let private instanceLibrary =
        FSharp(
            markerSource
            + """
namespace InstanceLibrary
open Markers
open System.Runtime.CompilerServices
type C() =
    let mutable value = 1
    [<PropertyMarker>]
    member _.P
        with [<GetterMarker; MethodImpl(MethodImplOptions.AggressiveInlining)>] get () = value
        and [<SetterMarker; MethodImpl(MethodImplOptions.NoInlining ||| MethodImplOptions.Synchronized ||| MethodImplOptions.PreserveSig)>] set (v: int) = value <- v
        """
        )
        |> asLibrary
        |> withName "InstanceLibrary"

    let private interfaceLibrary =
        FSharp
            """
namespace InterfaceLibrary
open System.Runtime.CompilerServices
type I =
    abstract P: int with get, set
type C() =
    let mutable value = 2
    interface I with
        member _.P
            with [<MethodImpl(MethodImplOptions.NoInlining)>] get () = value
            and [<MethodImpl(MethodImplOptions.Synchronized)>] set (v: int) = value <- v
"""
        |> asLibrary
        |> withName "InterfaceLibrary"

    let private signatureLibrary =
        Fsi
            """
module SignatureLibrary
type C =
    new: unit -> C
    member P: int with get, set
"""
        |> withAdditionalSourceFile (
            FsSource
                """
module SignatureLibrary
open System.Runtime.CompilerServices
type C() =
    let mutable value = 3
    member _.P
        with [<MethodImpl(MethodImplOptions.NoInlining)>] get () = value
        and [<MethodImpl(MethodImplOptions.Synchronized ||| MethodImplOptions.PreserveSig)>] set (v: int) = value <- v
        """
        )
        |> asLibrary
        |> withName "SignatureLibrary"

    [<Theory>]
    [<InlineData("issue")>]
    [<InlineData("instance markers")>]
    [<InlineData("standalone PreserveSig")>]
    [<InlineData("explicit interface")>]
    [<InlineData("extrinsic extension")>]
    [<InlineData("neutral signature")>]
    let ``Accessor implementation flags are metadata, not custom attributes`` scenario =
        let library, methods, properties =
            match scenario with
            | "issue" ->
                FSharp
                    """
module P
open System.Runtime.CompilerServices

[<AbstractClass; Sealed>]
type A =
    static member P1 with [<MethodImpl(MethodImplOptions.AggressiveInlining)>] get () = 1

[<AbstractClass; Sealed>]
type B =
    static member P2 with [<MethodImpl(MethodImplOptions.NoInlining)>] set (v: int) = ignore v

[<AbstractClass; Sealed>]
type C =
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    static member M1() = 1
                """,
                [
                    "P+A", "get_P1", 0x100, true, []
                    "P+B", "set_P2", 0x8, true, []
                    "P+C", "M1", 0x100, true, []
                ],
                []
            | "instance markers" ->
                instanceLibrary,
                [
                    "InstanceLibrary.C", "get_P", 0x100, false, [ "Markers.GetterMarkerAttribute" ]
                    "InstanceLibrary.C", "set_P", 0xA8, false, [ "Markers.SetterMarkerAttribute" ]
                ],
                [ "InstanceLibrary.C", "P", [ "Markers.PropertyMarkerAttribute" ] ]
            | "standalone PreserveSig" ->
                FSharp
                    """
namespace Standalone
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
type C() =
    member _.P with [<PreserveSig; MethodImpl(MethodImplOptions.NoInlining)>] get () = 1
    [<PreserveSig; MethodImpl(MethodImplOptions.NoInlining)>]
    member _.M() = 1
                """,
                [
                    "Standalone.C", "get_P", 0x88, false, []
                    "Standalone.C", "M", 0x88, false, []
                ],
                []
            | "explicit interface" ->
                interfaceLibrary,
                [
                    "InterfaceLibrary.C", "InterfaceLibrary.I.get_P", 0x8, false, []
                    "InterfaceLibrary.C", "InterfaceLibrary.I.set_P", 0x20, false, []
                ],
                []
            | "extrinsic extension" ->
                FSharp(
                    markerSource
                    + """
module Extensions =
    open System.Runtime.CompilerServices
    type System.String with
        member s.P with [<GetterMarker; MethodImpl(MethodImplOptions.NoInlining ||| MethodImplOptions.AggressiveInlining)>] get () = s.Length
                """
                ),
                [
                    "Markers.Extensions", "String.get_P", 0x108, true, [ "Markers.GetterMarkerAttribute" ]
                ],
                []
            | "neutral signature" ->
                signatureLibrary,
                [
                    "SignatureLibrary+C", "get_P", 0x8, false, []
                    "SignatureLibrary+C", "set_P", 0xA0, false, []
                ],
                []
            | _ -> failwithf "Unknown accessor scenario: %s" scenario

        library |> verifyMetadata methods properties

    let implementationFlagCases =
        [
            for attribute, flags, ordinary in
                [
                    "MethodImpl(MethodImplOptions.NoInlining)", 0x8, false
                    "MethodImpl(MethodImplOptions.Synchronized)", 0x20, false
                    "MethodImpl(MethodImplOptions.PreserveSig)", 0x80, false
                    "MethodImpl(MethodImplOptions.AggressiveInlining)", 0x100, false
                    "PreserveSig", 0x80, true
                    "MethodImpl(MethodImplOptions.NoInlining ||| MethodImplOptions.AggressiveInlining)", 0x108, true
                    "MethodImpl(MethodImplOptions.ForwardRef)", 0, false
                    "MethodImpl(MethodImplOptions.InternalCall)", 0, false
                    "MethodImpl(MethodImplOptions.NoOptimization)", 0, false
                    "MethodImpl(MethodImplOptions.Unmanaged)", 0, false
                    "MethodImpl(enum<MethodImplOptions>(0x200))", 0, true // AggressiveOptimization is absent on net472.
                    "MethodImpl(MethodImplOptions.NoInlining, MethodCodeType = MethodCodeType.Native)", 0x8, true
                    "MethodImpl(8s)", 0, true
                    "", 0, false
                ] do
                for accessor in [ true; false ] do
                    if accessor || ordinary then
                        yield [| box attribute; box flags; box accessor |]
        ]

    [<Theory; MemberData(nameof implementationFlagCases)>]
    let ``Implementation flag compatibility`` attribute flags accessor =
        let annotation = if attribute = "" then "" else "[<" + attribute + ">]"

        let memberSource =
            if accessor then
                "member _.P with " + annotation + " get () = 1"
            else
                annotation + "\n    member _.M() = 1"

        FSharp(
            """
namespace Compatibility
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
type C() =
    """
            + memberSource
        )
        |> verifyMetadata [ "Compatibility.C", (if accessor then "get_P" else "M"), flags, false, [] ] []

    [<Theory; InlineData(false); InlineData(true)>]
    let ``Property-level MethodImpl remains warning FS0842 unless promoted`` promote =
        let compilation =
            FSharp
                """
module InvalidTarget
open System.Runtime.CompilerServices
type C() =
    [<MethodImpl(MethodImplOptions.NoInlining)>]
    member _.P = 1
"""
            |> asLibrary

        let result, severity =
            if promote then
                compilation |> withOptions [ "--warnaserror:842" ] |> typecheck |> shouldFail, Error 842
            else
                compilation |> ignoreWarnings |> typecheck |> shouldSucceed, Warning 842

        result
        |> withSingleDiagnostic (
            severity,
            Line 5,
            Col 7,
            Line 5,
            Col 47,
            "This attribute cannot be applied to property, event, return value. Valid targets are: constructor, method"
        )

    [<Fact>]
    let ``External callers can get and set attributed properties`` () =
        FSharp
            """
module Consumer
[<EntryPoint>]
let main _ =
    let normal = InstanceLibrary.C()
    if normal.P <> 1 then failwith "normal getter"
    normal.P <- 11
    if normal.P <> 11 then failwith "normal setter"
    let explicit = InterfaceLibrary.C() :> InterfaceLibrary.I
    if explicit.P <> 2 then failwith "interface getter"
    explicit.P <- 22
    if explicit.P <> 22 then failwith "interface setter"
    let constrained = SignatureLibrary.C()
    if constrained.P <> 3 then failwith "signature getter"
    constrained.P <- 33
    if constrained.P <> 33 then failwith "signature setter"
    0
"""
        |> withReferences [ instanceLibrary; interfaceLibrary; signatureLibrary ]
        |> asExe
        |> compileExeAndRun
        |> shouldSucceed
