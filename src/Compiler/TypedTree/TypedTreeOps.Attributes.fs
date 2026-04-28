// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

/// TypedTreeOps.Attributes: IL extensions, attribute helpers, and debug printing.
namespace FSharp.Compiler.TypedTreeOps

open System
open System.CodeDom.Compiler
open System.Collections.Generic
open System.Collections.Immutable
open Internal.Utilities
open Internal.Utilities.Collections
open Internal.Utilities.Library
open Internal.Utilities.Library.Extras
open Internal.Utilities.Rational
open FSharp.Compiler
open FSharp.Compiler.IO
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.CompilerGlobalState
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.Features
open FSharp.Compiler.Syntax
open FSharp.Compiler.Syntax.PrettyNaming
open FSharp.Compiler.SyntaxTreeOps
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Text
open FSharp.Compiler.Text.Range
open FSharp.Compiler.Text.Layout
open FSharp.Compiler.Text.LayoutRender
open FSharp.Compiler.Text.TaggedText
open FSharp.Compiler.Xml
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeBasics
#if !NO_TYPEPROVIDERS
open FSharp.Compiler.TypeProviders
#endif

[<AutoOpen>]
module internal ILExtensions =

    //----------------------------------------------------------------------------
    // Detect attributes
    //----------------------------------------------------------------------------

    // AbsIL view of attributes (we read these from .NET binaries)
    let isILAttribByName (tencl: string list, tname: string) (attr: ILAttribute) =
        (attr.Method.DeclaringType.TypeSpec.Name = tname)
        && (attr.Method.DeclaringType.TypeSpec.Enclosing = tencl)

    // AbsIL view of attributes (we read these from .NET binaries). The comparison is done by name.
    let isILAttrib (tref: ILTypeRef) (attr: ILAttribute) =
        isILAttribByName (tref.Enclosing, tref.Name) attr

    // REVIEW: consider supporting querying on Abstract IL custom attributes.
    // These linear iterations cost us a fair bit when there are lots of attributes
    // on imported types. However this is fairly rare and can also be solved by caching the
    // results of attribute lookups in the TAST
    let HasILAttribute tref (attrs: ILAttributes) =
        attrs.AsArray() |> Array.exists (isILAttrib tref)

    let TryDecodeILAttribute tref (attrs: ILAttributes) =
        attrs.AsArray()
        |> Array.tryPick (fun x ->
            if isILAttrib tref x then
                Some(decodeILAttribData x)
            else
                None)

    // F# view of attributes (these get converted to AbsIL attributes in ilxgen)
    let IsMatchingFSharpAttribute g (AttribInfo(_, tcref)) (Attrib(tcref2, _, _, _, _, _, _)) = tyconRefEq g tcref tcref2

    let HasFSharpAttribute g tref attrs =
        List.exists (IsMatchingFSharpAttribute g tref) attrs

    let TryFindFSharpAttribute g tref attrs =
        List.tryFind (IsMatchingFSharpAttribute g tref) attrs

    [<return: Struct>]
    let (|ExtractAttribNamedArg|_|) nm args =
        args
        |> List.tryPick (function
            | AttribNamedArg(nm2, _, _, v) when nm = nm2 -> Some v
            | _ -> None)
        |> ValueOption.ofOption

    [<return: Struct>]
    let (|ExtractILAttributeNamedArg|_|) nm (args: ILAttributeNamedArg list) =
        args
        |> List.tryPick (function
            | nm2, _, _, v when nm = nm2 -> Some v
            | _ -> None)
        |> ValueOption.ofOption

    [<return: Struct>]
    let (|StringExpr|_|) =
        function
        | Expr.Const(Const.String n, _, _) -> ValueSome n
        | _ -> ValueNone

    [<return: Struct>]
    let (|AttribInt32Arg|_|) =
        function
        | AttribExpr(_, Expr.Const(Const.Int32 n, _, _)) -> ValueSome n
        | _ -> ValueNone

    [<return: Struct>]
    let (|AttribInt16Arg|_|) =
        function
        | AttribExpr(_, Expr.Const(Const.Int16 n, _, _)) -> ValueSome n
        | _ -> ValueNone

    [<return: Struct>]
    let (|AttribBoolArg|_|) =
        function
        | AttribExpr(_, Expr.Const(Const.Bool n, _, _)) -> ValueSome n
        | _ -> ValueNone

    [<return: Struct>]
    let (|AttribStringArg|_|) =
        function
        | AttribExpr(_, Expr.Const(Const.String n, _, _)) -> ValueSome n
        | _ -> ValueNone

    let (|AttribElemStringArg|_|) =
        function
        | ILAttribElem.String(n) -> n
        | _ -> None

    let TryFindILAttribute (AttribInfo(atref, _)) attrs = HasILAttribute atref attrs

    let IsILAttrib (AttribInfo(builtInAttrRef, _)) attr = isILAttrib builtInAttrRef attr

    let inline hasFlag (flags: ^F) (flag: ^F) : bool when ^F: enum<uint64> =
        let f = LanguagePrimitives.EnumToValue flags
        let v = LanguagePrimitives.EnumToValue flag
        f &&& v <> 0uL

    /// Compute well-known attribute flags for an ILAttributes collection.
    /// Classify a single IL attribute, returning its well-known flag (or None).
    let classifyILAttrib (attr: ILAttribute) : WellKnownILAttributes =
        let atref = attr.Method.DeclaringType.TypeSpec.TypeRef

        if not atref.Enclosing.IsEmpty then
            WellKnownILAttributes.None
        else
            let name = atref.Name

            if name.StartsWith("System.Runtime.CompilerServices.") then
                match name with
                | "System.Runtime.CompilerServices.IsReadOnlyAttribute" -> WellKnownILAttributes.IsReadOnlyAttribute
                | "System.Runtime.CompilerServices.IsUnmanagedAttribute" -> WellKnownILAttributes.IsUnmanagedAttribute
                | "System.Runtime.CompilerServices.ExtensionAttribute" -> WellKnownILAttributes.ExtensionAttribute
                | "System.Runtime.CompilerServices.IsByRefLikeAttribute" -> WellKnownILAttributes.IsByRefLikeAttribute
                | "System.Runtime.CompilerServices.InternalsVisibleToAttribute" -> WellKnownILAttributes.InternalsVisibleToAttribute
                | "System.Runtime.CompilerServices.CallerMemberNameAttribute" -> WellKnownILAttributes.CallerMemberNameAttribute
                | "System.Runtime.CompilerServices.CallerFilePathAttribute" -> WellKnownILAttributes.CallerFilePathAttribute
                | "System.Runtime.CompilerServices.CallerLineNumberAttribute" -> WellKnownILAttributes.CallerLineNumberAttribute
                | "System.Runtime.CompilerServices.RequiresLocationAttribute" -> WellKnownILAttributes.RequiresLocationAttribute
                | "System.Runtime.CompilerServices.NullableAttribute" -> WellKnownILAttributes.NullableAttribute
                | "System.Runtime.CompilerServices.NullableContextAttribute" -> WellKnownILAttributes.NullableContextAttribute
                | "System.Runtime.CompilerServices.IDispatchConstantAttribute" -> WellKnownILAttributes.IDispatchConstantAttribute
                | "System.Runtime.CompilerServices.IUnknownConstantAttribute" -> WellKnownILAttributes.IUnknownConstantAttribute
                | "System.Runtime.CompilerServices.SetsRequiredMembersAttribute" -> WellKnownILAttributes.SetsRequiredMembersAttribute
                | "System.Runtime.CompilerServices.CompilerFeatureRequiredAttribute" ->
                    WellKnownILAttributes.CompilerFeatureRequiredAttribute
                | "System.Runtime.CompilerServices.RequiredMemberAttribute" -> WellKnownILAttributes.RequiredMemberAttribute
                | _ -> WellKnownILAttributes.None

            elif name.StartsWith("Microsoft.FSharp.Core.") then
                match name with
                | "Microsoft.FSharp.Core.AllowNullLiteralAttribute" -> WellKnownILAttributes.AllowNullLiteralAttribute
                | "Microsoft.FSharp.Core.ReflectedDefinitionAttribute" -> WellKnownILAttributes.ReflectedDefinitionAttribute
                | "Microsoft.FSharp.Core.AutoOpenAttribute" -> WellKnownILAttributes.AutoOpenAttribute
                | "Microsoft.FSharp.Core.CompilerServices.NoEagerConstraintApplicationAttribute" ->
                    WellKnownILAttributes.NoEagerConstraintApplicationAttribute
                | _ -> WellKnownILAttributes.None

            else
                match name with
                | "System.ParamArrayAttribute" -> WellKnownILAttributes.ParamArrayAttribute
                | "System.Reflection.DefaultMemberAttribute" -> WellKnownILAttributes.DefaultMemberAttribute
                | "System.Diagnostics.CodeAnalysis.SetsRequiredMembersAttribute" ->
                    // Also at System.Runtime.CompilerServices (line above); .NET defines it in both namespaces
                    WellKnownILAttributes.SetsRequiredMembersAttribute
                | "System.ObsoleteAttribute" -> WellKnownILAttributes.ObsoleteAttribute
                | "System.Diagnostics.CodeAnalysis.ExperimentalAttribute" -> WellKnownILAttributes.ExperimentalAttribute
                | "System.AttributeUsageAttribute" -> WellKnownILAttributes.AttributeUsageAttribute
                | _ -> WellKnownILAttributes.None

    /// Compute well-known attribute flags for an ILAttributes collection.
    let computeILWellKnownFlags (_g: TcGlobals) (attrs: ILAttributes) : WellKnownILAttributes =
        let mutable flags = WellKnownILAttributes.None

        for attr in attrs.AsArray() do
            flags <- flags ||| classifyILAttrib attr

        flags

    /// Find the first IL attribute matching a specific well-known flag and decode it.
    let tryFindILAttribByFlag (flag: WellKnownILAttributes) (cattrs: ILAttributes) =
        cattrs.AsArray()
        |> Array.tryPick (fun attr ->
            if classifyILAttrib attr &&& flag <> WellKnownILAttributes.None then
                Some(decodeILAttribData attr)
            else
                None)

    /// Active pattern: find and decode a well-known IL attribute.
    /// Returns decoded (ILAttribElem list * ILAttributeNamedArg list).
    [<return: Struct>]
    let (|ILAttribDecoded|_|) (flag: WellKnownILAttributes) (cattrs: ILAttributes) =
        tryFindILAttribByFlag flag cattrs |> ValueOption.ofOption

    type ILAttributesStored with

        member x.HasWellKnownAttribute(g: TcGlobals, flag: WellKnownILAttributes) =
            x.HasWellKnownAttribute(flag, computeILWellKnownFlags g)

    type ILTypeDef with

        member x.HasWellKnownAttribute(g: TcGlobals, flag: WellKnownILAttributes) =
            x.CustomAttrsStored.HasWellKnownAttribute(g, flag)

    type ILMethodDef with

        member x.HasWellKnownAttribute(g: TcGlobals, flag: WellKnownILAttributes) =
            x.CustomAttrsStored.HasWellKnownAttribute(g, flag)

    type ILFieldDef with

        member x.HasWellKnownAttribute(g: TcGlobals, flag: WellKnownILAttributes) =
            x.CustomAttrsStored.HasWellKnownAttribute(g, flag)

    type ILAttributes with

        /// Non-caching (unlike ILAttributesStored.HasWellKnownAttribute which caches).
        member x.HasWellKnownAttribute(flag: WellKnownILAttributes) =
            x.AsArray()
            |> Array.exists (fun attr -> classifyILAttrib attr &&& flag <> WellKnownILAttributes.None)

[<AutoOpen>]
module internal AttributeHelpers =

    /// Resolve the FSharp.Core path for an attribute's type reference.
    /// Returns struct(bclPath, fsharpCorePath). Exactly one will be ValueSome, or both ValueNone.
    let inline resolveAttribPath (g: TcGlobals) (tcref: TyconRef) : struct (string[] voption * string[] voption) =
        if not tcref.IsLocalRef then
            let nlr = tcref.nlr

            if ccuEq nlr.Ccu g.fslibCcu then
                struct (ValueNone, ValueSome nlr.Path)
            else
                struct (ValueSome nlr.Path, ValueNone)
        elif g.compilingFSharpCore then
            match tcref.Deref.PublicPath with
            | Some(PubPath pp) -> struct (ValueNone, ValueSome pp)
            | None -> struct (ValueNone, ValueNone)
        else
            struct (ValueNone, ValueNone)

    /// Decode a bool-arg attribute and set the appropriate true/false flag.
    let inline decodeBoolAttribFlag (attrib: Attrib) trueFlag falseFlag defaultFlag =
        match attrib with
        | Attrib(_, _, [ AttribBoolArg b ], _, _, _, _) -> if b then trueFlag else falseFlag
        | _ -> defaultFlag

    /// Classify a single Entity-level attribute, returning its well-known flag (or None).
    let classifyEntityAttrib (g: TcGlobals) (attrib: Attrib) : WellKnownEntityAttributes =
        let (Attrib(tcref, _, _, _, _, _, _)) = attrib
        let struct (bclPath, fsharpCorePath) = resolveAttribPath g tcref

        match bclPath with
        | ValueSome path ->
            match path with
            | [| "System"; "Runtime"; "CompilerServices"; name |] ->
                match name with
                | "ExtensionAttribute" -> WellKnownEntityAttributes.ExtensionAttribute
                | "IsReadOnlyAttribute" -> WellKnownEntityAttributes.IsReadOnlyAttribute
                | "SkipLocalsInitAttribute" -> WellKnownEntityAttributes.SkipLocalsInitAttribute
                | "IsByRefLikeAttribute" -> WellKnownEntityAttributes.IsByRefLikeAttribute
                | _ -> WellKnownEntityAttributes.None

            | [| "System"; "Runtime"; "InteropServices"; name |] ->
                match name with
                | "StructLayoutAttribute" -> WellKnownEntityAttributes.StructLayoutAttribute
                | "DllImportAttribute" -> WellKnownEntityAttributes.DllImportAttribute
                | "ComVisibleAttribute" ->
                    decodeBoolAttribFlag
                        attrib
                        WellKnownEntityAttributes.ComVisibleAttribute_True
                        WellKnownEntityAttributes.ComVisibleAttribute_False
                        WellKnownEntityAttributes.ComVisibleAttribute_True
                | "ComImportAttribute" ->
                    decodeBoolAttribFlag
                        attrib
                        WellKnownEntityAttributes.ComImportAttribute_True
                        WellKnownEntityAttributes.None
                        WellKnownEntityAttributes.ComImportAttribute_True
                | _ -> WellKnownEntityAttributes.None

            | [| "System"; "Diagnostics"; name |] ->
                match name with
                | "DebuggerDisplayAttribute" -> WellKnownEntityAttributes.DebuggerDisplayAttribute
                | "DebuggerTypeProxyAttribute" -> WellKnownEntityAttributes.DebuggerTypeProxyAttribute
                | _ -> WellKnownEntityAttributes.None

            | [| "System"; "ComponentModel"; name |] ->
                match name with
                | "EditorBrowsableAttribute" -> WellKnownEntityAttributes.EditorBrowsableAttribute
                | _ -> WellKnownEntityAttributes.None

            | [| "System"; name |] ->
                match name with
                | "AttributeUsageAttribute" -> WellKnownEntityAttributes.AttributeUsageAttribute
                | "ObsoleteAttribute" -> WellKnownEntityAttributes.ObsoleteAttribute
                | _ -> WellKnownEntityAttributes.None

            | _ -> WellKnownEntityAttributes.None

        | ValueNone ->

            match fsharpCorePath with
            | ValueSome path ->
                match path with
                | [| "Microsoft"; "FSharp"; "Core"; name |] ->
                    match name with
                    | "SealedAttribute" ->
                        decodeBoolAttribFlag
                            attrib
                            WellKnownEntityAttributes.SealedAttribute_True
                            WellKnownEntityAttributes.SealedAttribute_False
                            WellKnownEntityAttributes.SealedAttribute_True
                    | "AbstractClassAttribute" -> WellKnownEntityAttributes.AbstractClassAttribute
                    | "RequireQualifiedAccessAttribute" -> WellKnownEntityAttributes.RequireQualifiedAccessAttribute
                    | "AutoOpenAttribute" -> WellKnownEntityAttributes.AutoOpenAttribute
                    | "NoEqualityAttribute" -> WellKnownEntityAttributes.NoEqualityAttribute
                    | "NoComparisonAttribute" -> WellKnownEntityAttributes.NoComparisonAttribute
                    | "StructuralEqualityAttribute" -> WellKnownEntityAttributes.StructuralEqualityAttribute
                    | "StructuralComparisonAttribute" -> WellKnownEntityAttributes.StructuralComparisonAttribute
                    | "CustomEqualityAttribute" -> WellKnownEntityAttributes.CustomEqualityAttribute
                    | "CustomComparisonAttribute" -> WellKnownEntityAttributes.CustomComparisonAttribute
                    | "ReferenceEqualityAttribute" -> WellKnownEntityAttributes.ReferenceEqualityAttribute
                    | "DefaultAugmentationAttribute" ->
                        decodeBoolAttribFlag
                            attrib
                            WellKnownEntityAttributes.DefaultAugmentationAttribute_True
                            WellKnownEntityAttributes.DefaultAugmentationAttribute_False
                            WellKnownEntityAttributes.DefaultAugmentationAttribute_True
                    | "CLIMutableAttribute" -> WellKnownEntityAttributes.CLIMutableAttribute
                    | "AutoSerializableAttribute" ->
                        decodeBoolAttribFlag
                            attrib
                            WellKnownEntityAttributes.AutoSerializableAttribute_True
                            WellKnownEntityAttributes.AutoSerializableAttribute_False
                            WellKnownEntityAttributes.AutoSerializableAttribute_True
                    | "ReflectedDefinitionAttribute" -> WellKnownEntityAttributes.ReflectedDefinitionAttribute
                    | "AllowNullLiteralAttribute" ->
                        decodeBoolAttribFlag
                            attrib
                            WellKnownEntityAttributes.AllowNullLiteralAttribute_True
                            WellKnownEntityAttributes.AllowNullLiteralAttribute_False
                            WellKnownEntityAttributes.AllowNullLiteralAttribute_True
                    | "WarnOnWithoutNullArgumentAttribute" -> WellKnownEntityAttributes.WarnOnWithoutNullArgumentAttribute
                    | "ClassAttribute" -> WellKnownEntityAttributes.ClassAttribute
                    | "InterfaceAttribute" -> WellKnownEntityAttributes.InterfaceAttribute
                    | "StructAttribute" -> WellKnownEntityAttributes.StructAttribute
                    | "MeasureAttribute" -> WellKnownEntityAttributes.MeasureAttribute
                    | "MeasureAnnotatedAbbreviationAttribute" -> WellKnownEntityAttributes.MeasureableAttribute
                    | "CLIEventAttribute" -> WellKnownEntityAttributes.CLIEventAttribute
                    | "CompilerMessageAttribute" -> WellKnownEntityAttributes.CompilerMessageAttribute
                    | "ExperimentalAttribute" -> WellKnownEntityAttributes.ExperimentalAttribute
                    | "UnverifiableAttribute" -> WellKnownEntityAttributes.UnverifiableAttribute
                    | "CompiledNameAttribute" -> WellKnownEntityAttributes.CompiledNameAttribute
                    | "CompilationRepresentationAttribute" ->
                        match attrib with
                        | Attrib(_, _, [ AttribInt32Arg v ], _, _, _, _) ->
                            let mutable flags = WellKnownEntityAttributes.None

                            if v &&& 0x01 <> 0 then
                                flags <- flags ||| WellKnownEntityAttributes.CompilationRepresentation_Static

                            if v &&& 0x02 <> 0 then
                                flags <- flags ||| WellKnownEntityAttributes.CompilationRepresentation_Instance

                            if v &&& 0x04 <> 0 then
                                flags <- flags ||| WellKnownEntityAttributes.CompilationRepresentation_ModuleSuffix

                            if v &&& 0x08 <> 0 then
                                flags <- flags ||| WellKnownEntityAttributes.CompilationRepresentation_PermitNull

                            flags
                        | _ -> WellKnownEntityAttributes.None
                    | _ -> WellKnownEntityAttributes.None
                | _ -> WellKnownEntityAttributes.None
            | ValueNone -> WellKnownEntityAttributes.None

    /// Classify a single assembly-level attribute, returning its well-known flag (or None).
    let classifyAssemblyAttrib (g: TcGlobals) (attrib: Attrib) : WellKnownAssemblyAttributes =
        let (Attrib(tcref, _, _, _, _, _, _)) = attrib
        let struct (bclPath, fsharpCorePath) = resolveAttribPath g tcref

        match bclPath with
        | ValueSome path ->
            match path with
            | [| "System"; "Runtime"; "CompilerServices"; name |] ->
                match name with
                | "InternalsVisibleToAttribute" -> WellKnownAssemblyAttributes.InternalsVisibleToAttribute
                | _ -> WellKnownAssemblyAttributes.None
            | [| "System"; "Reflection"; name |] ->
                match name with
                | "AssemblyCultureAttribute" -> WellKnownAssemblyAttributes.AssemblyCultureAttribute
                | "AssemblyVersionAttribute" -> WellKnownAssemblyAttributes.AssemblyVersionAttribute
                | _ -> WellKnownAssemblyAttributes.None
            | _ -> WellKnownAssemblyAttributes.None
        | ValueNone ->

            match fsharpCorePath with
            | ValueSome path ->
                match path with
                | [| "Microsoft"; "FSharp"; "Core"; name |] ->
                    match name with
                    | "AutoOpenAttribute" -> WellKnownAssemblyAttributes.AutoOpenAttribute
                    | _ -> WellKnownAssemblyAttributes.None
                | [| "Microsoft"; "FSharp"; "Core"; "CompilerServices"; name |] ->
                    match name with
                    | "TypeProviderAssemblyAttribute" -> WellKnownAssemblyAttributes.TypeProviderAssemblyAttribute
                    | _ -> WellKnownAssemblyAttributes.None
                | _ -> WellKnownAssemblyAttributes.None
            | ValueNone -> WellKnownAssemblyAttributes.None

    // ---------------------------------------------------------------
    // Well-Known Attribute APIs — Navigation Guide
    // ---------------------------------------------------------------
    //
    // This section provides O(1) cached lookups for well-known attributes.
    // Choose the right API based on what you have and what you need:
    //
    // EXISTENCE CHECKS (cached, O(1) after first call):
    //   EntityHasWellKnownAttribute  g flag entity  — Entity (type/module)
    //   ValHasWellKnownAttribute     g flag v       — Val (value/member)
    //   ArgReprInfoHasWellKnownAttribute g flag arg — ArgReprInfo (parameter)
    //
    // AD-HOC CHECKS (no cache, re-scans each call):
    //   attribsHaveEntityFlag  g flag attribs  — raw Attrib list, entity flags
    //   attribsHaveValFlag     g flag attribs  — raw Attrib list, val flags
    //
    // DATA EXTRACTION (active patterns):
    //   (|EntityAttrib|_|)       g flag attribs  — returns full Attrib
    //   (|ValAttrib|_|)          g flag attribs  — returns full Attrib
    //   (|EntityAttribInt|_|)    g flag attribs  — extracts int32 argument
    //   (|EntityAttribString|_|) g flag attribs  — extracts string argument
    //   (|ValAttribInt|_|)       g flag attribs  — extracts int32 argument
    //   (|ValAttribString|_|)    g flag attribs  — extracts string argument
    //
    // BOOL ATTRIBUTE QUERIES (three-state: Some true / Some false / None):
    //   EntityTryGetBoolAttribute  g trueFlag falseFlag entity
    //   ValTryGetBoolAttribute     g trueFlag falseFlag v
    //
    // IL-LEVEL (operates on ILAttribute / ILAttributes):
    //   classifyILAttrib           attr           — classify a single IL attr
    //   (|ILAttribDecoded|_|)      flag cattrs    — find & decode by flag
    //   ILAttributes.HasWellKnownAttribute(flag)  — existence check (no cache)
    //   ILAttributesStored.HasWellKnownAttribute(g, flag) — cached existence
    //
    // CROSS-METADATA (IL + F# + Provided type dispatch):
    //   TyconRefHasWellKnownAttribute  g flag tcref
    //   TyconRefAllowsNull             g tcref
    //
    // CROSS-METADATA (in AttributeChecking.fs):
    //   MethInfoHasWellKnownAttribute      g m ilFlag valFlag attribSpec minfo
    //   MethInfoHasWellKnownAttributeSpec  g m spec minfo  — convenience wrapper
    //
    // CLASSIFICATION (maps attribute → flag enum):
    //   classifyEntityAttrib  g attrib  — Attrib → WellKnownEntityAttributes
    //   classifyValAttrib     g attrib  — Attrib → WellKnownValAttributes
    //   classifyILAttrib      attr      — ILAttribute → WellKnownILAttributes
    // ---------------------------------------------------------------

    /// Shared combinator: find first attrib matching a flag via a classify function.
    let inline internal tryFindAttribByClassifier
        ([<InlineIfLambda>] classify: TcGlobals -> Attrib -> 'Flag)
        (none: 'Flag)
        (g: TcGlobals)
        (flag: 'Flag)
        (attribs: Attribs)
        : Attrib option =
        attribs |> List.tryFind (fun attrib -> classify g attrib &&& flag <> none)

    /// Shared combinator: check if any attrib in a list matches a flag via a classify function.
    let inline internal attribsHaveFlag
        ([<InlineIfLambda>] classify: TcGlobals -> Attrib -> 'Flag)
        (none: 'Flag)
        (g: TcGlobals)
        (flag: 'Flag)
        (attribs: Attribs)
        : bool =
        attribs |> List.exists (fun attrib -> classify g attrib &&& flag <> none)

    /// Compute well-known attribute flags for an Entity's Attrib list.
    let computeEntityWellKnownFlags (g: TcGlobals) (attribs: Attribs) : WellKnownEntityAttributes =
        let mutable flags = WellKnownEntityAttributes.None

        for attrib in attribs do
            flags <- flags ||| classifyEntityAttrib g attrib

        flags

    /// Find the first attribute matching a specific well-known entity flag.
    let tryFindEntityAttribByFlag g flag attribs =
        tryFindAttribByClassifier classifyEntityAttrib WellKnownEntityAttributes.None g flag attribs

    /// Active pattern: find a well-known entity attribute and return the full Attrib.
    [<return: Struct>]
    let (|EntityAttrib|_|) (g: TcGlobals) (flag: WellKnownEntityAttributes) (attribs: Attribs) =
        tryFindEntityAttribByFlag g flag attribs |> ValueOption.ofOption

    /// Active pattern: extract a single int32 argument from a well-known entity attribute.
    [<return: Struct>]
    let (|EntityAttribInt|_|) (g: TcGlobals) (flag: WellKnownEntityAttributes) (attribs: Attribs) =
        match attribs with
        | EntityAttrib g flag (Attrib(_, _, [ AttribInt32Arg v ], _, _, _, _)) -> ValueSome v
        | _ -> ValueNone

    /// Active pattern: extract a single string argument from a well-known entity attribute.
    [<return: Struct>]
    let (|EntityAttribString|_|) (g: TcGlobals) (flag: WellKnownEntityAttributes) (attribs: Attribs) =
        match attribs with
        | EntityAttrib g flag (Attrib(_, _, [ AttribStringArg s ], _, _, _, _)) -> ValueSome s
        | _ -> ValueNone

    /// Map a WellKnownILAttributes flag to its entity flag + provided-type AttribInfo equivalents.
    let mapILFlag (g: TcGlobals) (flag: WellKnownILAttributes) : struct (WellKnownEntityAttributes * BuiltinAttribInfo option) =
        match flag with
        | WellKnownILAttributes.IsReadOnlyAttribute ->
            struct (WellKnownEntityAttributes.IsReadOnlyAttribute, Some g.attrib_IsReadOnlyAttribute)
        | WellKnownILAttributes.IsByRefLikeAttribute ->
            struct (WellKnownEntityAttributes.IsByRefLikeAttribute, g.attrib_IsByRefLikeAttribute_opt)
        | WellKnownILAttributes.ExtensionAttribute ->
            struct (WellKnownEntityAttributes.ExtensionAttribute, Some g.attrib_ExtensionAttribute)
        | WellKnownILAttributes.AllowNullLiteralAttribute ->
            struct (WellKnownEntityAttributes.AllowNullLiteralAttribute_True, Some g.attrib_AllowNullLiteralAttribute)
        | WellKnownILAttributes.AutoOpenAttribute -> struct (WellKnownEntityAttributes.AutoOpenAttribute, Some g.attrib_AutoOpenAttribute)
        | WellKnownILAttributes.ReflectedDefinitionAttribute ->
            struct (WellKnownEntityAttributes.ReflectedDefinitionAttribute, Some g.attrib_ReflectedDefinitionAttribute)
        | WellKnownILAttributes.ObsoleteAttribute -> struct (WellKnownEntityAttributes.ObsoleteAttribute, None)
        | _ -> struct (WellKnownEntityAttributes.None, None)

    /// Check if a raw attribute list has a specific well-known entity flag (ad-hoc, non-caching).
    let attribsHaveEntityFlag g (flag: WellKnownEntityAttributes) (attribs: Attribs) =
        attribsHaveFlag classifyEntityAttrib WellKnownEntityAttributes.None g flag attribs

    /// Map a WellKnownILAttributes flag to its WellKnownValAttributes equivalent.
    /// Check if an Entity has a specific well-known attribute, computing and caching flags if needed.
    let EntityHasWellKnownAttribute (g: TcGlobals) (flag: WellKnownEntityAttributes) (entity: Entity) : bool =
        entity.HasWellKnownAttribute(flag, computeEntityWellKnownFlags g)

    /// Get the computed well-known attribute flags for an entity.
    let GetEntityWellKnownFlags (g: TcGlobals) (entity: Entity) : WellKnownEntityAttributes =
        entity.GetWellKnownEntityFlags(computeEntityWellKnownFlags g)

    /// Classify a single Val-level attribute, returning its well-known flag (or None).
    let classifyValAttrib (g: TcGlobals) (attrib: Attrib) : WellKnownValAttributes =
        let (Attrib(tcref, _, _, _, _, _, _)) = attrib
        let struct (bclPath, fsharpCorePath) = resolveAttribPath g tcref

        match bclPath with
        | ValueSome path ->
            match path with
            | [| "System"; "Runtime"; "CompilerServices"; name |] ->
                match name with
                | "SkipLocalsInitAttribute" -> WellKnownValAttributes.SkipLocalsInitAttribute
                | "ExtensionAttribute" -> WellKnownValAttributes.ExtensionAttribute
                | "CallerMemberNameAttribute" -> WellKnownValAttributes.CallerMemberNameAttribute
                | "CallerFilePathAttribute" -> WellKnownValAttributes.CallerFilePathAttribute
                | "CallerLineNumberAttribute" -> WellKnownValAttributes.CallerLineNumberAttribute
                | "MethodImplAttribute" -> WellKnownValAttributes.MethodImplAttribute
                | _ -> WellKnownValAttributes.None

            | [| "System"; "Runtime"; "InteropServices"; name |] ->
                match name with
                | "DllImportAttribute" -> WellKnownValAttributes.DllImportAttribute
                | "InAttribute" -> WellKnownValAttributes.InAttribute
                | "OutAttribute" -> WellKnownValAttributes.OutAttribute
                | "MarshalAsAttribute" -> WellKnownValAttributes.MarshalAsAttribute
                | "DefaultParameterValueAttribute" -> WellKnownValAttributes.DefaultParameterValueAttribute
                | "OptionalAttribute" -> WellKnownValAttributes.OptionalAttribute
                | "PreserveSigAttribute" -> WellKnownValAttributes.PreserveSigAttribute
                | "FieldOffsetAttribute" -> WellKnownValAttributes.FieldOffsetAttribute
                | _ -> WellKnownValAttributes.None

            | [| "System"; "Diagnostics"; name |] ->
                match name with
                | "ConditionalAttribute" -> WellKnownValAttributes.ConditionalAttribute
                | _ -> WellKnownValAttributes.None

            | [| "System"; name |] ->
                match name with
                | "ThreadStaticAttribute" -> WellKnownValAttributes.ThreadStaticAttribute
                | "ContextStaticAttribute" -> WellKnownValAttributes.ContextStaticAttribute
                | "ParamArrayAttribute" -> WellKnownValAttributes.ParamArrayAttribute
                | "NonSerializedAttribute" -> WellKnownValAttributes.NonSerializedAttribute
                | _ -> WellKnownValAttributes.None

            | _ -> WellKnownValAttributes.None

        | ValueNone ->

            match fsharpCorePath with
            | ValueSome path ->
                match path with
                | [| "Microsoft"; "FSharp"; "Core"; name |] ->
                    match name with
                    | "EntryPointAttribute" -> WellKnownValAttributes.EntryPointAttribute
                    | "LiteralAttribute" -> WellKnownValAttributes.LiteralAttribute
                    | "ReflectedDefinitionAttribute" ->
                        decodeBoolAttribFlag
                            attrib
                            WellKnownValAttributes.ReflectedDefinitionAttribute_True
                            WellKnownValAttributes.ReflectedDefinitionAttribute_False
                            WellKnownValAttributes.ReflectedDefinitionAttribute_False
                    | "RequiresExplicitTypeArgumentsAttribute" -> WellKnownValAttributes.RequiresExplicitTypeArgumentsAttribute
                    | "DefaultValueAttribute" ->
                        decodeBoolAttribFlag
                            attrib
                            WellKnownValAttributes.DefaultValueAttribute_True
                            WellKnownValAttributes.DefaultValueAttribute_False
                            WellKnownValAttributes.DefaultValueAttribute_True
                    | "VolatileFieldAttribute" -> WellKnownValAttributes.VolatileFieldAttribute
                    | "NoDynamicInvocationAttribute" ->
                        decodeBoolAttribFlag
                            attrib
                            WellKnownValAttributes.NoDynamicInvocationAttribute_True
                            WellKnownValAttributes.NoDynamicInvocationAttribute_False
                            WellKnownValAttributes.NoDynamicInvocationAttribute_False
                    | "OptionalArgumentAttribute" -> WellKnownValAttributes.OptionalArgumentAttribute
                    | "ProjectionParameterAttribute" -> WellKnownValAttributes.ProjectionParameterAttribute
                    | "InlineIfLambdaAttribute" -> WellKnownValAttributes.InlineIfLambdaAttribute
                    | "StructAttribute" -> WellKnownValAttributes.StructAttribute
                    | "NoCompilerInliningAttribute" -> WellKnownValAttributes.NoCompilerInliningAttribute
                    | "GeneralizableValueAttribute" -> WellKnownValAttributes.GeneralizableValueAttribute
                    | "CLIEventAttribute" -> WellKnownValAttributes.CLIEventAttribute
                    | "CompiledNameAttribute" -> WellKnownValAttributes.CompiledNameAttribute
                    | "WarnOnWithoutNullArgumentAttribute" -> WellKnownValAttributes.WarnOnWithoutNullArgumentAttribute
                    | "ValueAsStaticPropertyAttribute" -> WellKnownValAttributes.ValueAsStaticPropertyAttribute
                    | "TailCallAttribute" -> WellKnownValAttributes.TailCallAttribute
                    | _ -> WellKnownValAttributes.None
                | [| "Microsoft"; "FSharp"; "Core"; "CompilerServices"; name |] ->
                    match name with
                    | "NoEagerConstraintApplicationAttribute" -> WellKnownValAttributes.NoEagerConstraintApplicationAttribute
                    | _ -> WellKnownValAttributes.None
                | _ -> WellKnownValAttributes.None
            | ValueNone -> WellKnownValAttributes.None

    let computeValWellKnownFlags (g: TcGlobals) (attribs: Attribs) : WellKnownValAttributes =
        let mutable flags = WellKnownValAttributes.None

        for attrib in attribs do
            flags <- flags ||| classifyValAttrib g attrib

        flags

    /// Find the first attribute in a list that matches a specific well-known val flag.
    let tryFindValAttribByFlag g flag attribs =
        tryFindAttribByClassifier classifyValAttrib WellKnownValAttributes.None g flag attribs

    /// Active pattern: find a well-known val attribute and return the full Attrib.
    [<return: Struct>]
    let (|ValAttrib|_|) (g: TcGlobals) (flag: WellKnownValAttributes) (attribs: Attribs) =
        tryFindValAttribByFlag g flag attribs |> ValueOption.ofOption

    /// Active pattern: extract a single int32 argument from a well-known val attribute.
    [<return: Struct>]
    let (|ValAttribInt|_|) (g: TcGlobals) (flag: WellKnownValAttributes) (attribs: Attribs) =
        match attribs with
        | ValAttrib g flag (Attrib(_, _, [ AttribInt32Arg v ], _, _, _, _)) -> ValueSome v
        | _ -> ValueNone

    /// Active pattern: extract a single string argument from a well-known val attribute.
    [<return: Struct>]
    let (|ValAttribString|_|) (g: TcGlobals) (flag: WellKnownValAttributes) (attribs: Attribs) =
        match attribs with
        | ValAttrib g flag (Attrib(_, _, [ AttribStringArg s ], _, _, _, _)) -> ValueSome s
        | _ -> ValueNone

    /// Check if a raw attribute list has a specific well-known val flag (ad-hoc, non-caching).
    let attribsHaveValFlag g (flag: WellKnownValAttributes) (attribs: Attribs) =
        attribsHaveFlag classifyValAttrib WellKnownValAttributes.None g flag attribs

    /// Filter out well-known attributes from a list. Single-pass using classify functions.
    /// Attributes matching ANY set bit in entityMask or valMask are removed.
    let filterOutWellKnownAttribs
        (g: TcGlobals)
        (entityMask: WellKnownEntityAttributes)
        (valMask: WellKnownValAttributes)
        (attribs: Attribs)
        =
        attribs
        |> List.filter (fun attrib ->
            (entityMask = WellKnownEntityAttributes.None
             || classifyEntityAttrib g attrib &&& entityMask = WellKnownEntityAttributes.None)
            && (valMask = WellKnownValAttributes.None
                || classifyValAttrib g attrib &&& valMask = WellKnownValAttributes.None))

    /// Check if an ArgReprInfo has a specific well-known attribute, computing and caching flags if needed.
    let ArgReprInfoHasWellKnownAttribute (g: TcGlobals) (flag: WellKnownValAttributes) (argInfo: ArgReprInfo) : bool =
        let struct (result, waNew, changed) =
            argInfo.Attribs.CheckFlag(flag, computeValWellKnownFlags g)

        if changed then
            argInfo.Attribs <- waNew

        result

    /// Check if a Val has a specific well-known attribute, computing and caching flags if needed.
    let ValHasWellKnownAttribute (g: TcGlobals) (flag: WellKnownValAttributes) (v: Val) : bool =
        v.HasWellKnownAttribute(flag, computeValWellKnownFlags g)

    /// Query a three-state bool attribute on an entity. Returns bool option.
    let EntityTryGetBoolAttribute
        (g: TcGlobals)
        (trueFlag: WellKnownEntityAttributes)
        (falseFlag: WellKnownEntityAttributes)
        (entity: Entity)
        : bool option =
        if not (entity.HasWellKnownAttribute(trueFlag ||| falseFlag, computeEntityWellKnownFlags g)) then
            Option.None
        else
            let struct (hasTrue, _, _) =
                entity.EntityAttribs.CheckFlag(trueFlag, computeEntityWellKnownFlags g)

            if hasTrue then Some true else Some false

    /// Query a three-state bool attribute on a Val. Returns bool option.
    let ValTryGetBoolAttribute
        (g: TcGlobals)
        (trueFlag: WellKnownValAttributes)
        (falseFlag: WellKnownValAttributes)
        (v: Val)
        : bool option =
        if not (v.HasWellKnownAttribute(trueFlag ||| falseFlag, computeValWellKnownFlags g)) then
            Option.None
        else
            let struct (hasTrue, _, _) =
                v.ValAttribs.CheckFlag(trueFlag, computeValWellKnownFlags g)

            if hasTrue then Some true else Some false

    /// Shared core for binding attributes on type definitions, supporting an optional
    /// WellKnownILAttributes flag for O(1) early exit on the IL metadata path.
    let private tryBindTyconRefAttributeCore
        g
        (m: range)
        (ilFlag: WellKnownILAttributes voption)
        (AttribInfo(atref, _) as args)
        (tcref: TyconRef)
        f1
        f2
        (f3: obj option list * (string * obj option) list -> 'a option)
        : 'a option =
        ignore m
        ignore f3

        match metadataOfTycon tcref.Deref with
#if !NO_TYPEPROVIDERS
        | ProvidedTypeMetadata info ->
            let provAttribs =
                info.ProvidedType.PApply((fun a -> (a :> IProvidedCustomAttributeProvider)), m)

            match
                provAttribs.PUntaint(
                    (fun a -> a.GetAttributeConstructorArgs(provAttribs.TypeProvider.PUntaintNoFailure id, atref.FullName)),
                    m
                )
            with
            | Some args -> f3 args
            | None -> None
#endif
        | ILTypeMetadata(TILObjectReprData(_, _, tdef)) ->
            match ilFlag with
            | ValueSome flag when not (tdef.HasWellKnownAttribute(g, flag)) -> None
            | _ ->
                match TryDecodeILAttribute atref tdef.CustomAttrs with
                | Some attr -> f1 attr
                | _ -> None
        | FSharpOrArrayOrByrefOrTupleOrExnTypeMetadata ->
            match TryFindFSharpAttribute g args tcref.Attribs with
            | Some attr -> f2 attr
            | _ -> None

    /// Analyze three cases for attributes declared on type definitions: IL-declared attributes, F#-declared attributes and
    /// provided attributes.
    //
    // This is used for AttributeUsageAttribute, DefaultMemberAttribute and ConditionalAttribute (on attribute types)
    let TryBindTyconRefAttribute g (m: range) args (tcref: TyconRef) f1 f2 f3 : 'a option =
        tryBindTyconRefAttributeCore g m ValueNone args tcref f1 f2 f3

    let TryFindTyconRefBoolAttribute g m attribSpec tcref =
        TryBindTyconRefAttribute
            g
            m
            attribSpec
            tcref
            (function
            | [], _ -> Some true
            | [ ILAttribElem.Bool v ], _ -> Some v
            | _ -> None)
            (function
            | Attrib(_, _, [], _, _, _, _) -> Some true
            | Attrib(_, _, [ AttribBoolArg v ], _, _, _, _) -> Some v
            | _ -> None)
            (function
             | [], _ -> Some true
             | [ Some(:? bool as v: obj) ], _ -> Some v
             | _ -> None)

    /// Try to find the AllowMultiple value of the AttributeUsage attribute on a type definition.
    let TryFindAttributeUsageAttribute g m tcref =
        tryBindTyconRefAttributeCore
            g
            m
            (ValueSome WellKnownILAttributes.AttributeUsageAttribute)
            g.attrib_AttributeUsageAttribute
            tcref
            (fun (_, named) ->
                named
                |> List.tryPick (function
                    | "AllowMultiple", _, _, ILAttribElem.Bool res -> Some res
                    | _ -> None))
            (fun (Attrib(_, _, _, named, _, _, _)) ->
                named
                |> List.tryPick (function
                    | AttribNamedArg("AllowMultiple", _, _, AttribBoolArg res) -> Some res
                    | _ -> None))
            (fun (_, named) ->
                named
                |> List.tryPick (function
                    | "AllowMultiple", Some(:? bool as res: obj) -> Some res
                    | _ -> None))

    /// Try to find a specific attribute on a type definition, where the attribute accepts a string argument.
    ///
    /// This is used to detect the 'DefaultMemberAttribute' and 'ConditionalAttribute' attributes (on type definitions)
    let TryFindTyconRefStringAttribute g m attribSpec tcref =
        TryBindTyconRefAttribute
            g
            m
            attribSpec
            tcref
            (function
            | [ ILAttribElem.String(Some msg) ], _ -> Some msg
            | _ -> None)
            (function
            | Attrib(_, _, [ AttribStringArg msg ], _, _, _, _) -> Some msg
            | _ -> None)
            (function
             | [ Some(:? string as msg: obj) ], _ -> Some msg
             | _ -> None)

    /// Like TryBindTyconRefAttribute but with a fast-path flag check on the IL metadata path.
    /// Skips the full attribute scan if the cached flag indicates the attribute is absent.
    let TryBindTyconRefAttributeWithILFlag g (m: range) (ilFlag: WellKnownILAttributes) args (tcref: TyconRef) f1 f2 f3 : 'a option =
        tryBindTyconRefAttributeCore g m (ValueSome ilFlag) args tcref f1 f2 f3

    /// Like TryFindTyconRefStringAttribute but with a fast-path flag check on the IL path.
    /// Use this when the attribute has a corresponding WellKnownILAttributes flag for O(1) early exit.
    let TryFindTyconRefStringAttributeFast g m ilFlag attribSpec tcref =
        TryBindTyconRefAttributeWithILFlag
            g
            m
            ilFlag
            attribSpec
            tcref
            (function
            | [ ILAttribElem.String(Some msg) ], _ -> Some msg
            | _ -> None)
            (function
            | Attrib(_, _, [ AttribStringArg msg ], _, _, _, _) -> Some msg
            | _ -> None)
            (function
             | [ Some(:? string as msg: obj) ], _ -> Some msg
             | _ -> None)

    /// Check if a type definition has a specific attribute
    let TyconRefHasAttribute g m attribSpec tcref =
        TryBindTyconRefAttribute g m attribSpec tcref (fun _ -> Some()) (fun _ -> Some()) (fun _ -> Some())
        |> Option.isSome

    /// Check if a TyconRef has a well-known attribute, handling both IL and F# metadata.
    /// Uses O(1) flag tests on both paths.
    let TyconRefHasWellKnownAttribute (g: TcGlobals) (flag: WellKnownILAttributes) (tcref: TyconRef) : bool =
        match metadataOfTycon tcref.Deref with
#if !NO_TYPEPROVIDERS
        | ProvidedTypeMetadata _ ->
            let struct (_, attribInfoOpt) = mapILFlag g flag

            match attribInfoOpt with
            | Some attribInfo -> TyconRefHasAttribute g tcref.Range attribInfo tcref
            | None -> false
#endif
        | ILTypeMetadata(TILObjectReprData(_, _, tdef)) -> tdef.HasWellKnownAttribute(g, flag)
        | FSharpOrArrayOrByrefOrTupleOrExnTypeMetadata ->
            let struct (entityFlag, _) = mapILFlag g flag

            if entityFlag <> WellKnownEntityAttributes.None then
                EntityHasWellKnownAttribute g entityFlag tcref.Deref
            else
                false

    let HasDefaultAugmentationAttribute g (tcref: TyconRef) =
        match
            EntityTryGetBoolAttribute
                g
                WellKnownEntityAttributes.DefaultAugmentationAttribute_True
                WellKnownEntityAttributes.DefaultAugmentationAttribute_False
                tcref.Deref
        with
        | Some b -> b
        | None -> true

    /// Check if a TyconRef has AllowNullLiteralAttribute, returning Some true/Some false/None.
    let TyconRefAllowsNull (g: TcGlobals) (tcref: TyconRef) : bool option =
        match metadataOfTycon tcref.Deref with
#if !NO_TYPEPROVIDERS
        | ProvidedTypeMetadata _ -> TryFindTyconRefBoolAttribute g tcref.Range g.attrib_AllowNullLiteralAttribute tcref
#endif
        | ILTypeMetadata(TILObjectReprData(_, _, tdef)) ->
            if tdef.HasWellKnownAttribute(g, WellKnownILAttributes.AllowNullLiteralAttribute) then
                Some true
            else
                None
        | FSharpOrArrayOrByrefOrTupleOrExnTypeMetadata ->
            EntityTryGetBoolAttribute
                g
                WellKnownEntityAttributes.AllowNullLiteralAttribute_True
                WellKnownEntityAttributes.AllowNullLiteralAttribute_False
                tcref.Deref

    /// Check if a type definition has an attribute with a specific full name
    let TyconRefHasAttributeByName (m: range) attrFullName (tcref: TyconRef) =
        ignore m

        match metadataOfTycon tcref.Deref with
#if !NO_TYPEPROVIDERS
        | ProvidedTypeMetadata info ->
            let provAttribs =
                info.ProvidedType.PApply((fun a -> (a :> IProvidedCustomAttributeProvider)), m)

            provAttribs
                .PUntaint((fun a -> a.GetAttributeConstructorArgs(provAttribs.TypeProvider.PUntaintNoFailure id, attrFullName)), m)
                .IsSome
#endif
        | ILTypeMetadata(TILObjectReprData(_, _, tdef)) ->
            tdef.CustomAttrs.AsArray()
            |> Array.exists (fun attr -> isILAttribByName ([], attrFullName) attr)
        | FSharpOrArrayOrByrefOrTupleOrExnTypeMetadata ->
            tcref.Attribs
            |> List.exists (fun attr ->
                match attr.TyconRef.CompiledRepresentation with
                | CompiledTypeRepr.ILAsmNamed(typeRef, _, _) -> typeRef.Enclosing.IsEmpty && typeRef.Name = attrFullName
                | CompiledTypeRepr.ILAsmOpen _ -> false)

    type ValRef with
        member vref.IsDispatchSlot =
            match vref.MemberInfo with
            | Some membInfo -> membInfo.MemberFlags.IsDispatchSlot
            | None -> false

    [<return: Struct>]
    let (|UnopExpr|_|) (_g: TcGlobals) expr =
        match expr with
        | Expr.App(Expr.Val(vref, _, _), _, _, [ arg1 ], _) -> ValueSome(vref, arg1)
        | _ -> ValueNone

    [<return: Struct>]
    let (|BinopExpr|_|) (_g: TcGlobals) expr =
        match expr with
        | Expr.App(Expr.Val(vref, _, _), _, _, [ arg1; arg2 ], _) -> ValueSome(vref, arg1, arg2)
        | _ -> ValueNone

    [<return: Struct>]
    let (|SpecificUnopExpr|_|) g vrefReqd expr =
        match expr with
        | UnopExpr g (vref, arg1) when valRefEq g vref vrefReqd -> ValueSome arg1
        | _ -> ValueNone

    [<return: Struct>]
    let (|SignedConstExpr|_|) expr =
        match expr with
        | Expr.Const(Const.Int32 _, _, _)
        | Expr.Const(Const.SByte _, _, _)
        | Expr.Const(Const.Int16 _, _, _)
        | Expr.Const(Const.Int64 _, _, _)
        | Expr.Const(Const.Single _, _, _)
        | Expr.Const(Const.Double _, _, _) -> ValueSome()
        | _ -> ValueNone

    [<return: Struct>]
    let (|IntegerConstExpr|_|) expr =
        match expr with
        | Expr.Const(Const.Int32 _, _, _)
        | Expr.Const(Const.SByte _, _, _)
        | Expr.Const(Const.Int16 _, _, _)
        | Expr.Const(Const.Int64 _, _, _)
        | Expr.Const(Const.Byte _, _, _)
        | Expr.Const(Const.UInt16 _, _, _)
        | Expr.Const(Const.UInt32 _, _, _)
        | Expr.Const(Const.UInt64 _, _, _) -> ValueSome()
        | _ -> ValueNone

    [<return: Struct>]
    let (|FloatConstExpr|_|) expr =
        match expr with
        | Expr.Const(Const.Single _, _, _)
        | Expr.Const(Const.Double _, _, _) -> ValueSome()
        | _ -> ValueNone

    [<return: Struct>]
    let (|SpecificBinopExpr|_|) g vrefReqd expr =
        match expr with
        | BinopExpr g (vref, arg1, arg2) when valRefEq g vref vrefReqd -> ValueSome(arg1, arg2)
        | _ -> ValueNone

    [<return: Struct>]
    let (|EnumExpr|_|) g expr =
        match (|SpecificUnopExpr|_|) g g.enum_vref expr with
        | ValueNone -> (|SpecificUnopExpr|_|) g g.enumOfValue_vref expr
        | x -> x

    [<return: Struct>]
    let (|BitwiseOrExpr|_|) g expr =
        (|SpecificBinopExpr|_|) g g.bitwise_or_vref expr

    [<return: Struct>]
    let (|AttribBitwiseOrExpr|_|) g expr =
        match expr with
        | BitwiseOrExpr g (arg1, arg2) -> ValueSome(arg1, arg2)
        // Special workaround, only used when compiling FSharp.Core.dll. Uses of 'a ||| b' occur before the '|||' bitwise or operator
        // is defined. These get through type checking because enums implicitly support the '|||' operator through
        // the automatic resolution of undefined operators (see tc.fs, Item.ImplicitOp). This then compiles as an
        // application of a lambda to two arguments. We recognize this pattern here
        | Expr.App(Expr.Lambda _, _, _, [ arg1; arg2 ], _) when g.compilingFSharpCore -> ValueSome(arg1, arg2)
        | _ -> ValueNone

    let isUncheckedDefaultOfValRef g vref =
        valRefEq g vref g.unchecked_defaultof_vref
        // There is an internal version of typeof defined in prim-types.fs that needs to be detected
        || (g.compilingFSharpCore && vref.LogicalName = "defaultof")

    let isTypeOfValRef g vref =
        valRefEq g vref g.typeof_vref
        // There is an internal version of typeof defined in prim-types.fs that needs to be detected
        || (g.compilingFSharpCore && vref.LogicalName = "typeof")

    let isSizeOfValRef g vref =
        valRefEq g vref g.sizeof_vref
        // There is an internal version of typeof defined in prim-types.fs that needs to be detected
        || (g.compilingFSharpCore && vref.LogicalName = "sizeof")

    let isNameOfValRef g vref =
        valRefEq g vref g.nameof_vref
        // There is an internal version of nameof defined in prim-types.fs that needs to be detected
        || (g.compilingFSharpCore && vref.LogicalName = "nameof")

    let isTypeDefOfValRef g vref =
        valRefEq g vref g.typedefof_vref
        // There is an internal version of typedefof defined in prim-types.fs that needs to be detected
        || (g.compilingFSharpCore && vref.LogicalName = "typedefof")

    [<return: Struct>]
    let (|UncheckedDefaultOfExpr|_|) g expr =
        match expr with
        | Expr.App(Expr.Val(vref, _, _), _, [ ty ], [], _) when isUncheckedDefaultOfValRef g vref -> ValueSome ty
        | _ -> ValueNone

    [<return: Struct>]
    let (|TypeOfExpr|_|) g expr =
        match expr with
        | Expr.App(Expr.Val(vref, _, _), _, [ ty ], [], _) when isTypeOfValRef g vref -> ValueSome ty
        | _ -> ValueNone

    [<return: Struct>]
    let (|SizeOfExpr|_|) g expr =
        match expr with
        | Expr.App(Expr.Val(vref, _, _), _, [ ty ], [], _) when isSizeOfValRef g vref -> ValueSome ty
        | _ -> ValueNone

    [<return: Struct>]
    let (|TypeDefOfExpr|_|) g expr =
        match expr with
        | Expr.App(Expr.Val(vref, _, _), _, [ ty ], [], _) when isTypeDefOfValRef g vref -> ValueSome ty
        | _ -> ValueNone

    [<return: Struct>]
    let (|NameOfExpr|_|) g expr =
        match expr with
        | Expr.App(Expr.Val(vref, _, _), _, [ ty ], [], _) when isNameOfValRef g vref -> ValueSome ty
        | _ -> ValueNone

    [<return: Struct>]
    let (|SeqExpr|_|) g expr =
        match expr with
        | Expr.App(Expr.Val(vref, _, _), _, _, _, _) when valRefEq g vref g.seq_vref -> ValueSome()
        | _ -> ValueNone

    //----------------------------------------------------------------------------
    // CompilationMappingAttribute, SourceConstructFlags
    //----------------------------------------------------------------------------

    let tnameCompilationSourceNameAttr = Core + ".CompilationSourceNameAttribute"

    let tnameCompilationArgumentCountsAttr =
        Core + ".CompilationArgumentCountsAttribute"

    let tnameCompilationMappingAttr = Core + ".CompilationMappingAttribute"
    let tnameSourceConstructFlags = Core + ".SourceConstructFlags"

    let tref_CompilationArgumentCountsAttr (g: TcGlobals) =
        mkILTyRef (g.fslibCcu.ILScopeRef, tnameCompilationArgumentCountsAttr)

    let tref_CompilationMappingAttr (g: TcGlobals) =
        mkILTyRef (g.fslibCcu.ILScopeRef, tnameCompilationMappingAttr)

    let tref_CompilationSourceNameAttr (g: TcGlobals) =
        mkILTyRef (g.fslibCcu.ILScopeRef, tnameCompilationSourceNameAttr)

    let tref_SourceConstructFlags (g: TcGlobals) =
        mkILTyRef (g.fslibCcu.ILScopeRef, tnameSourceConstructFlags)

    let mkCompilationMappingAttrPrim (g: TcGlobals) k nums =
        mkILCustomAttribute (
            tref_CompilationMappingAttr g,
            ((mkILNonGenericValueTy (tref_SourceConstructFlags g))
             :: (nums |> List.map (fun _ -> g.ilg.typ_Int32))),
            ((k :: nums) |> List.map ILAttribElem.Int32),
            []
        )

    let mkCompilationMappingAttr g kind = mkCompilationMappingAttrPrim g kind []

    let mkCompilationMappingAttrWithSeqNum g kind seqNum =
        mkCompilationMappingAttrPrim g kind [ seqNum ]

    let mkCompilationMappingAttrWithVariantNumAndSeqNum g kind varNum seqNum =
        mkCompilationMappingAttrPrim g kind [ varNum; seqNum ]

    let mkCompilationArgumentCountsAttr (g: TcGlobals) nums =
        mkILCustomAttribute (
            tref_CompilationArgumentCountsAttr g,
            [ mkILArr1DTy g.ilg.typ_Int32 ],
            [ ILAttribElem.Array(g.ilg.typ_Int32, List.map ILAttribElem.Int32 nums) ],
            []
        )

    let mkCompilationSourceNameAttr (g: TcGlobals) n =
        mkILCustomAttribute (tref_CompilationSourceNameAttr g, [ g.ilg.typ_String ], [ ILAttribElem.String(Some n) ], [])

    let mkCompilationMappingAttrForQuotationResource (g: TcGlobals) (nm, tys: ILTypeRef list) =
        mkILCustomAttribute (
            tref_CompilationMappingAttr g,
            [ g.ilg.typ_String; mkILArr1DTy g.ilg.typ_Type ],
            [
                ILAttribElem.String(Some nm)
                ILAttribElem.Array(g.ilg.typ_Type, [ for ty in tys -> ILAttribElem.TypeRef(Some ty) ])
            ],
            []
        )

    //----------------------------------------------------------------------------
    // Decode extensible typing attributes
    //----------------------------------------------------------------------------

#if !NO_TYPEPROVIDERS

    let isTypeProviderAssemblyAttr (cattr: ILAttribute) =
        cattr.Method.DeclaringType.BasicQualifiedName = !!typeof<Microsoft.FSharp.Core.CompilerServices.TypeProviderAssemblyAttribute>
            .FullName

    let TryDecodeTypeProviderAssemblyAttr (cattr: ILAttribute) : (string | null) option =
        if isTypeProviderAssemblyAttr cattr then
            let params_, _args = decodeILAttribData cattr

            match params_ with // The first parameter to the attribute is the name of the assembly with the compiler extensions.
            | ILAttribElem.String(Some assemblyName) :: _ -> Some assemblyName
            | ILAttribElem.String None :: _ -> Some null
            | [] -> Some null
            | _ -> None
        else
            None

#endif

    //----------------------------------------------------------------------------
    // FSharpInterfaceDataVersionAttribute
    //----------------------------------------------------------------------------

    let tname_SignatureDataVersionAttr = Core + ".FSharpInterfaceDataVersionAttribute"

    let tref_SignatureDataVersionAttr fsharpCoreAssemblyScopeRef =
        mkILTyRef (fsharpCoreAssemblyScopeRef, tname_SignatureDataVersionAttr)

    let mkSignatureDataVersionAttr (g: TcGlobals) (version: ILVersionInfo) =
        mkILCustomAttribute (
            tref_SignatureDataVersionAttr g.ilg.fsharpCoreAssemblyScopeRef,
            [ g.ilg.typ_Int32; g.ilg.typ_Int32; g.ilg.typ_Int32 ],
            [
                ILAttribElem.Int32(int32 version.Major)
                ILAttribElem.Int32(int32 version.Minor)
                ILAttribElem.Int32(int32 version.Build)
            ],
            []
        )

    let IsSignatureDataVersionAttr cattr =
        isILAttribByName ([], tname_SignatureDataVersionAttr) cattr

    let TryFindAutoOpenAttr (cattr: ILAttribute) =
        if
            classifyILAttrib cattr &&& WellKnownILAttributes.AutoOpenAttribute
            <> WellKnownILAttributes.None
        then
            match decodeILAttribData cattr with
            | [ ILAttribElem.String s ], _ -> s
            | [], _ -> None
            | _ ->
                warning (Failure(FSComp.SR.tastUnexpectedDecodeOfAutoOpenAttribute ()))
                None
        else
            None

    let TryFindInternalsVisibleToAttr (cattr: ILAttribute) =
        if
            classifyILAttrib cattr &&& WellKnownILAttributes.InternalsVisibleToAttribute
            <> WellKnownILAttributes.None
        then
            match decodeILAttribData cattr with
            | [ ILAttribElem.String s ], _ -> s
            | [], _ -> None
            | _ ->
                warning (Failure(FSComp.SR.tastUnexpectedDecodeOfInternalsVisibleToAttribute ()))
                None
        else
            None

    let IsMatchingSignatureDataVersionAttr (version: ILVersionInfo) cattr =
        IsSignatureDataVersionAttr cattr
        && match decodeILAttribData cattr with
           | [ ILAttribElem.Int32 u1; ILAttribElem.Int32 u2; ILAttribElem.Int32 u3 ], _ ->
               (version.Major = uint16 u1)
               && (version.Minor = uint16 u2)
               && (version.Build = uint16 u3)
           | _ ->
               warning (Failure(FSComp.SR.tastUnexpectedDecodeOfInterfaceDataVersionAttribute ()))
               false

    let isSealedTy g ty =
        let ty = stripTyEqnsAndMeasureEqns g ty

        not (isRefTy g ty)
        || isUnitTy g ty
        || isArrayTy g ty
        ||

        match metadataOfTy g ty with
#if !NO_TYPEPROVIDERS
        | ProvidedTypeMetadata st -> st.IsSealed
#endif
        | ILTypeMetadata(TILObjectReprData(_, _, td)) -> td.IsSealed
        | FSharpOrArrayOrByrefOrTupleOrExnTypeMetadata ->
            if (isFSharpInterfaceTy g ty || isFSharpClassTy g ty) then
                let tcref = tcrefOfAppTy g ty
                EntityHasWellKnownAttribute g WellKnownEntityAttributes.SealedAttribute_True tcref.Deref
            else
                // All other F# types, array, byref, tuple types are sealed
                true

    //--------------------------------------------------------------------------
    // Some unions have null as representations
    //--------------------------------------------------------------------------

    let TyconHasUseNullAsTrueValueAttribute g (tycon: Tycon) =
        EntityHasWellKnownAttribute g WellKnownEntityAttributes.CompilationRepresentation_PermitNull tycon

    // WARNING: this must match optimizeAlternativeToNull in ilx/cu_erase.fs
    let CanHaveUseNullAsTrueValueAttribute (_g: TcGlobals) (tycon: Tycon) =
        (tycon.IsUnionTycon
         && let ucs = tycon.UnionCasesArray in

            (ucs.Length = 0
             || (ucs |> Array.existsOne (fun uc -> uc.IsNullary)
                 && ucs |> Array.exists (fun uc -> not uc.IsNullary))))

    // WARNING: this must match optimizeAlternativeToNull in ilx/cu_erase.fs
    let IsUnionTypeWithNullAsTrueValue (g: TcGlobals) (tycon: Tycon) =
        (tycon.IsUnionTycon
         && let ucs = tycon.UnionCasesArray in

            (ucs.Length = 0
             || (TyconHasUseNullAsTrueValueAttribute g tycon
                 && ucs |> Array.existsOne (fun uc -> uc.IsNullary)
                 && ucs |> Array.exists (fun uc -> not uc.IsNullary))))

    let TyconCompilesInstanceMembersAsStatic g tycon = IsUnionTypeWithNullAsTrueValue g tycon

    let TcrefCompilesInstanceMembersAsStatic g (tcref: TyconRef) =
        TyconCompilesInstanceMembersAsStatic g tcref.Deref

    let ModuleNameIsMangled g attrs =
        attribsHaveEntityFlag g WellKnownEntityAttributes.CompilationRepresentation_ModuleSuffix attrs

    let CompileAsEvent g attrs =
        attribsHaveValFlag g WellKnownValAttributes.CLIEventAttribute attrs

    let ValCompileAsEvent g (v: Val) =
        ValHasWellKnownAttribute g WellKnownValAttributes.CLIEventAttribute v

    let MemberIsCompiledAsInstance g parent isExtensionMember (membInfo: ValMemberInfo) attrs =
        // All extension members are compiled as static members
        if isExtensionMember then
            false
        // Abstract slots, overrides and interface impls are all true to IsInstance
        elif
            membInfo.MemberFlags.IsDispatchSlot
            || membInfo.MemberFlags.IsOverrideOrExplicitImpl
            || not (isNil membInfo.ImplementedSlotSigs)
        then
            membInfo.MemberFlags.IsInstance
        else
            // Otherwise check attributes to see if there is an explicit instance or explicit static flag
            let entityFlags = computeEntityWellKnownFlags g attrs

            let explicitInstance =
                hasFlag entityFlags WellKnownEntityAttributes.CompilationRepresentation_Instance

            let explicitStatic =
                hasFlag entityFlags WellKnownEntityAttributes.CompilationRepresentation_Static

            explicitInstance
            || (membInfo.MemberFlags.IsInstance
                && not explicitStatic
                && not (TcrefCompilesInstanceMembersAsStatic g parent))

    let ValSpecIsCompiledAsInstance g (v: Val) =
        match v.MemberInfo with
        | Some membInfo ->
            // Note it doesn't matter if we pass 'v.DeclaringEntity' or 'v.MemberApparentEntity' here.
            // These only differ if the value is an extension member, and in that case MemberIsCompiledAsInstance always returns
            // false anyway
            MemberIsCompiledAsInstance g v.MemberApparentEntity v.IsExtensionMember membInfo v.Attribs
        | _ -> false

    let ValRefIsCompiledAsInstanceMember g (vref: ValRef) =
        ValSpecIsCompiledAsInstance g vref.Deref

    let tryFindExtensionAttribute (g: TcGlobals) (attribs: Attrib list) : Attrib option =
        tryFindEntityAttribByFlag g WellKnownEntityAttributes.ExtensionAttribute attribs

    let tryAddExtensionAttributeIfNotAlreadyPresentForModule
        (g: TcGlobals)
        (tryFindExtensionAttributeIn: (Attrib list -> Attrib option) -> Attrib option)
        (moduleEntity: Entity)
        : Entity =
        if Option.isSome (tryFindExtensionAttribute g moduleEntity.Attribs) then
            moduleEntity
        else
            match tryFindExtensionAttributeIn (tryFindExtensionAttribute g) with
            | None -> moduleEntity
            | Some extensionAttrib ->
                { moduleEntity with
                    entity_attribs = moduleEntity.EntityAttribs.Add(extensionAttrib, WellKnownEntityAttributes.ExtensionAttribute)
                }

    let tryAddExtensionAttributeIfNotAlreadyPresentForType
        (g: TcGlobals)
        (tryFindExtensionAttributeIn: (Attrib list -> Attrib option) -> Attrib option)
        (moduleOrNamespaceTypeAccumulator: ModuleOrNamespaceType ref)
        (typeEntity: Entity)
        : Entity =
        if Option.isSome (tryFindExtensionAttribute g typeEntity.Attribs) then
            typeEntity
        else
            match tryFindExtensionAttributeIn (tryFindExtensionAttribute g) with
            | None -> typeEntity
            | Some extensionAttrib ->
                moduleOrNamespaceTypeAccumulator.Value.AllEntitiesByLogicalMangledName.TryFind(typeEntity.LogicalName)
                |> Option.iter (fun e ->
                    e.entity_attribs <- e.EntityAttribs.Add(extensionAttrib, WellKnownEntityAttributes.ExtensionAttribute))

                typeEntity

[<AutoOpen>]
module internal ByrefAndSpanHelpers =

    // See RFC FS-1053.md
    // Must use name-based matching (not type-identity) because user code can define
    // its own IsByRefLikeAttribute per RFC FS-1053.
    let isByrefLikeTyconRef (g: TcGlobals) m (tcref: TyconRef) =
        tcref.CanDeref
        && match tcref.TryIsByRefLike with
           | ValueSome res -> res
           | _ ->
               let res =
                   isByrefTyconRef g tcref
                   || (isStructTyconRef tcref
                       && TyconRefHasAttributeByName m tname_IsByRefLikeAttribute tcref)

               tcref.SetIsByRefLike res
               res

    let isSpanLikeTyconRef g m tcref =
        isByrefLikeTyconRef g m tcref && not (isByrefTyconRef g tcref)

    let isByrefLikeTy g m ty =
        ty
        |> stripTyEqns g
        |> (function
        | TType_app(tcref, _, _) -> isByrefLikeTyconRef g m tcref
        | _ -> false)

    let isSpanLikeTy g m ty =
        isByrefLikeTy g m ty && not (isByrefTy g ty)

    let isSpanTyconRef g m tcref =
        isByrefLikeTyconRef g m tcref
        && tcref.CompiledRepresentationForNamedType.BasicQualifiedName = "System.Span`1"

    let isSpanTy g m ty =
        ty
        |> stripTyEqns g
        |> (function
        | TType_app(tcref, _, _) -> isSpanTyconRef g m tcref
        | _ -> false)

    let tryDestSpanTy g m ty =
        match tryAppTy g ty with
        | ValueSome(tcref, [ ty ]) when isSpanTyconRef g m tcref -> Some(tcref, ty)
        | _ -> None

    let destSpanTy g m ty =
        match tryDestSpanTy g m ty with
        | Some(tcref, ty) -> (tcref, ty)
        | _ -> failwith "destSpanTy"

    let isReadOnlySpanTyconRef g m tcref =
        isByrefLikeTyconRef g m tcref
        && tcref.CompiledRepresentationForNamedType.BasicQualifiedName = "System.ReadOnlySpan`1"

    let isReadOnlySpanTy g m ty =
        ty
        |> stripTyEqns g
        |> (function
        | TType_app(tcref, _, _) -> isReadOnlySpanTyconRef g m tcref
        | _ -> false)

    let tryDestReadOnlySpanTy g m ty =
        match tryAppTy g ty with
        | ValueSome(tcref, [ ty ]) when isReadOnlySpanTyconRef g m tcref -> Some(tcref, ty)
        | _ -> None

    let destReadOnlySpanTy g m ty =
        match tryDestReadOnlySpanTy g m ty with
        | Some(tcref, ty) -> (tcref, ty)
        | _ -> failwith "destReadOnlySpanTy"

module internal DebugPrint =

    //--------------------------------------------------------------------------
    // DEBUG layout
    //---------------------------------------------------------------------------
    let mutable layoutRanges = false
    let mutable layoutTypes = false
    let mutable layoutStamps = false
    let mutable layoutValReprInfo = false

    let braceBarL l =
        leftL leftBraceBar ^^ l ^^ rightL rightBraceBar

    let intL (n: int) = wordL (tagNumericLiteral (string n))

    let qlistL f xmap =
        QueueList.foldBack (fun x z -> z @@ f x) xmap emptyL

    let bracketIfL b lyt = if b then bracketL lyt else lyt

    let lvalopL x =
        match x with
        | LAddrOf false -> wordL (tagText "&")
        | LAddrOf true -> wordL (tagText "&!")
        | LByrefGet -> wordL (tagText "*")
        | LSet -> wordL (tagText "LSet")
        | LByrefSet -> wordL (tagText "LByrefSet")

    let angleBracketL l =
        leftL (tagText "<") ^^ l ^^ rightL (tagText ">")

    let angleBracketListL l =
        angleBracketL (sepListL (sepL (tagText ",")) l)

#if DEBUG
    let layoutMemberFlags (memFlags: SynMemberFlags) =
        let stat =
            if memFlags.IsInstance || (memFlags.MemberKind = SynMemberKind.Constructor) then
                emptyL
            else
                wordL (tagText "static")

        let stat =
            if memFlags.IsDispatchSlot then
                stat ++ wordL (tagText "abstract")
            elif memFlags.IsOverrideOrExplicitImpl then
                stat ++ wordL (tagText "override")
            else
                stat

        stat
#endif

    let stampL (n: Stamp) w =
        if layoutStamps then
            w ^^ wordL (tagText ("#" + string n))
        else
            w

    let layoutTyconRef (tcref: TyconRef) =
        wordL (tagText tcref.DisplayNameWithStaticParameters) |> stampL tcref.Stamp

    let rec auxTypeL env ty = auxTypeWrapL env false ty

    and auxTypeAtomL env ty = auxTypeWrapL env true ty

    and auxTyparsL env tcL prefix tinst =
        match tinst with
        | [] -> tcL
        | [ t ] ->
            let tL = auxTypeAtomL env t
            if prefix then tcL ^^ angleBracketL tL else tL ^^ tcL
        | _ ->
            let tinstL = List.map (auxTypeL env) tinst

            if prefix then
                tcL ^^ angleBracketListL tinstL
            else
                tupleL tinstL ^^ tcL

    and auxAddNullness coreL (nullness: Nullness) =
        match nullness.Evaluate() with
        | NullnessInfo.WithNull -> coreL ^^ wordL (tagText "?")
        | NullnessInfo.WithoutNull -> coreL
        | NullnessInfo.AmbivalentToNull -> coreL //^^ wordL (tagText "%")

    and auxTypeWrapL env isAtomic ty =
        let wrap x = bracketIfL isAtomic x in // wrap iff require atomic expr

        match stripTyparEqns ty with
        | TType_forall(typars, bodyTy) -> (leftL (tagText "!") ^^ layoutTyparDecls typars --- auxTypeL env bodyTy) |> wrap

        | TType_ucase(UnionCaseRef(tcref, _), tinst) ->
            let prefix = tcref.IsPrefixDisplay
            let tcL = layoutTyconRef tcref
            auxTyparsL env tcL prefix tinst

        | TType_app(tcref, tinst, nullness) ->
            let prefix = tcref.IsPrefixDisplay
            let tcL = layoutTyconRef tcref
            let coreL = auxTyparsL env tcL prefix tinst
            auxAddNullness coreL nullness

        | TType_tuple(_tupInfo, tys) -> sepListL (wordL (tagText "*")) (List.map (auxTypeAtomL env) tys) |> wrap

        | TType_fun(domainTy, rangeTy, nullness) ->
            let coreL =
                ((auxTypeAtomL env domainTy ^^ wordL (tagText "->")) --- auxTypeL env rangeTy)
                |> wrap

            auxAddNullness coreL nullness

        | TType_var(typar, nullness) ->
            let coreL = auxTyparWrapL env isAtomic typar
            auxAddNullness coreL nullness

        | TType_anon(anonInfo, tys) ->
            braceBarL (
                sepListL
                    (wordL (tagText ";"))
                    (List.map2 (fun nm ty -> wordL (tagField nm) --- auxTypeAtomL env ty) (Array.toList anonInfo.SortedNames) tys)
            )

        | TType_measure unt ->
#if DEBUG
            leftL (tagText "{")
            ^^ (match global_g with
                | None -> wordL (tagText "<no global g>")
                | Some g ->
                    let sortVars (vs: (Typar * Rational) list) =
                        vs |> List.sortBy (fun (v, _) -> v.DisplayName)

                    let sortCons (cs: (TyconRef * Rational) list) =
                        cs |> List.sortBy (fun (c, _) -> c.DisplayName)

                    let negvs, posvs =
                        ListMeasureVarOccsWithNonZeroExponents unt
                        |> sortVars
                        |> List.partition (fun (_, e) -> SignRational e < 0)

                    let negcs, poscs =
                        ListMeasureConOccsWithNonZeroExponents g false unt
                        |> sortCons
                        |> List.partition (fun (_, e) -> SignRational e < 0)

                    let unparL (uv: Typar) = wordL (tagText ("'" + uv.DisplayName))
                    let unconL tcref = layoutTyconRef tcref
                    let rationalL e = wordL (tagText (RationalToString e))

                    let measureToPowerL x e =
                        if e = OneRational then
                            x
                        else
                            x -- wordL (tagText "^") -- rationalL e

                    let prefix =
                        spaceListL (
                            List.map (fun (v, e) -> measureToPowerL (unparL v) e) posvs
                            @ List.map (fun (c, e) -> measureToPowerL (unconL c) e) poscs
                        )

                    let postfix =
                        spaceListL (
                            List.map (fun (v, e) -> measureToPowerL (unparL v) (NegRational e)) negvs
                            @ List.map (fun (c, e) -> measureToPowerL (unconL c) (NegRational e)) negcs
                        )

                    match (negvs, negcs) with
                    | [], [] -> prefix
                    | _ -> prefix ^^ sepL (tagText "/") ^^ postfix)
            ^^ rightL (tagText "}")
#else
            unt |> ignore
            wordL (tagText "<measure>")
#endif

    and auxTyparWrapL (env: SimplifyTypes.TypeSimplificationInfo) isAtomic (typar: Typar) =

        let tpText =
            prefixOfStaticReq typar.StaticReq
            + prefixOfInferenceTypar typar
            + typar.DisplayName

        let tpL = wordL (tagText tpText)

        let varL = tpL |> stampL typar.Stamp

        // There are several cases for pprinting of typar.
        //
        //   'a - is multiple occurrence.
        //   #Type - inplace coercion constraint and singleton
        //   ('a :> Type) - inplace coercion constraint not singleton
        //   ('a.opM: S->T) - inplace operator constraint
        match Zmap.tryFind typar env.inplaceConstraints with
        | Some typarConstraintTy ->
            if Zset.contains typar env.singletons then
                leftL (tagText "#") ^^ auxTyparConstraintTypL env typarConstraintTy
            else
                (varL ^^ sepL (tagText ":>") ^^ auxTyparConstraintTypL env typarConstraintTy)
                |> bracketIfL isAtomic
        | _ -> varL

    and auxTypar2L env typar = auxTyparWrapL env false typar

    and auxTyparConstraintTypL env ty = auxTypeL env ty

    and auxTraitL env (ttrait: TraitConstraintInfo) =
#if DEBUG
        let (TTrait(tys, nm, memFlags, argTys, retTy, _, _)) = ttrait

        match global_g with
        | None -> wordL (tagText "<no global g>")
        | Some g ->
            let retTy = GetFSharpViewOfReturnType g retTy
            let stat = layoutMemberFlags memFlags
            let argsL = sepListL (wordL (tagText "*")) (List.map (auxTypeAtomL env) argTys)
            let resL = auxTypeL env retTy
            let methodTypeL = (argsL ^^ wordL (tagText "->")) ++ resL

            bracketL (
                stat
                ++ bracketL (sepListL (wordL (tagText "or")) (List.map (auxTypeAtomL env) tys))
                ++ wordL (tagText "member")
                --- (wordL (tagText nm) ^^ wordL (tagText ":") -- methodTypeL)
            )
#else
        ignore (env, ttrait)
        wordL (tagText "trait")
#endif

    and auxTyparConstraintL env (tp, tpc) =
        let constraintPrefix l =
            auxTypar2L env tp ^^ wordL (tagText ":") ^^ l

        match tpc with
        | TyparConstraint.CoercesTo(typarConstraintTy, _) ->
            auxTypar2L env tp
            ^^ wordL (tagText ":>") --- auxTyparConstraintTypL env typarConstraintTy
        | TyparConstraint.MayResolveMember(traitInfo, _) -> auxTypar2L env tp ^^ wordL (tagText ":") --- auxTraitL env traitInfo
        | TyparConstraint.DefaultsTo(_, ty, _) ->
            wordL (tagText "default")
            ^^ auxTypar2L env tp
            ^^ wordL (tagText ":")
            ^^ auxTypeL env ty
        | TyparConstraint.IsEnum(ty, _) -> auxTyparsL env (wordL (tagText "enum")) true [ ty ] |> constraintPrefix
        | TyparConstraint.IsDelegate(aty, bty, _) ->
            auxTyparsL env (wordL (tagText "delegate")) true [ aty; bty ]
            |> constraintPrefix
        | TyparConstraint.SupportsNull _ -> wordL (tagText "null") |> constraintPrefix
        | TyparConstraint.SupportsComparison _ -> wordL (tagText "comparison") |> constraintPrefix
        | TyparConstraint.SupportsEquality _ -> wordL (tagText "equality") |> constraintPrefix
        | TyparConstraint.IsNonNullableStruct _ -> wordL (tagText "struct") |> constraintPrefix
        | TyparConstraint.IsReferenceType _ -> wordL (tagText "not struct") |> constraintPrefix
        | TyparConstraint.NotSupportsNull _ -> wordL (tagText "not null") |> constraintPrefix
        | TyparConstraint.IsUnmanaged _ -> wordL (tagText "unmanaged") |> constraintPrefix
        | TyparConstraint.AllowsRefStruct _ -> wordL (tagText "allows ref struct") |> constraintPrefix
        | TyparConstraint.SimpleChoice(tys, _) ->
            bracketL (sepListL (sepL (tagText "|")) (List.map (auxTypeL env) tys))
            |> constraintPrefix
        | TyparConstraint.RequiresDefaultConstructor _ ->
            bracketL (wordL (tagText "new : unit -> ") ^^ (auxTypar2L env tp))
            |> constraintPrefix

    and auxTyparConstraintsL env x =
        match x with
        | [] -> emptyL
        | cxs -> wordL (tagText "when") --- aboveListL (List.map (auxTyparConstraintL env) cxs)

    and typarL tp =
        auxTypar2L SimplifyTypes.typeSimplificationInfo0 tp

    and typeAtomL tau =
        let tau, cxs = tau, []
        let env = SimplifyTypes.CollectInfo false [ tau ] cxs

        match env.postfixConstraints with
        | [] -> auxTypeAtomL env tau
        | _ -> bracketL (auxTypeL env tau --- auxTyparConstraintsL env env.postfixConstraints)

    and typeL tau =
        let tau, cxs = tau, []
        let env = SimplifyTypes.CollectInfo false [ tau ] cxs

        match env.postfixConstraints with
        | [] -> auxTypeL env tau
        | _ -> (auxTypeL env tau --- auxTyparConstraintsL env env.postfixConstraints)

    and typarDeclL tp =
        let tau, cxs = mkTyparTy tp, (List.map (fun x -> (tp, x)) tp.Constraints)
        let env = SimplifyTypes.CollectInfo false [ tau ] cxs

        match env.postfixConstraints with
        | [] -> auxTypeL env tau
        | _ -> (auxTypeL env tau --- auxTyparConstraintsL env env.postfixConstraints)

    and layoutTyparDecls tps =
        match tps with
        | [] -> emptyL
        | _ -> angleBracketListL (List.map typarDeclL tps)

    let rangeL m = wordL (tagText (stringOfRange m))

    let instL tyL tys =
        if layoutTypes then
            match tys with
            | [] -> emptyL
            | tys -> sepL (tagText "@[") ^^ commaListL (List.map tyL tys) ^^ rightL (tagText "]")
        else
            emptyL

    let valRefL (vr: ValRef) =
        wordL (tagText vr.LogicalName) |> stampL vr.Stamp

    let layoutAttrib (Attrib(_, k, _, _, _, _, _)) =
        leftL (tagText "[<")
        ^^ (match k with
            | ILAttrib ilmeth -> wordL (tagText ilmeth.Name)
            | FSAttrib vref -> valRefL vref)
        ^^ rightL (tagText ">]")

    let layoutAttribs attribs =
        aboveListL (List.map layoutAttrib attribs)

    let valReprInfoL (ValReprInfo(tpNames, _, _) as tvd) =
        let ns = tvd.AritiesOfArgs

        leftL (tagText "<")
        ^^ intL tpNames.Length
        ^^ sepL (tagText ">[")
        ^^ commaListL (List.map intL ns)
        ^^ rightL (tagText "]")

    let valL (v: Val) =
        let vsL =
            wordL (tagText (ConvertValLogicalNameToDisplayNameCore v.LogicalName))
            |> stampL v.Stamp

        let vsL = vsL -- layoutAttribs v.Attribs
        vsL

    let typeOfValL (v: Val) =
        valL v
        ^^ (if v.ShouldInline then wordL (tagText "inline ") else emptyL)
        ^^ (if v.IsMutable then wordL (tagText "mutable ") else emptyL)
        ^^ (if layoutTypes then
                wordL (tagText ":") ^^ typeL v.Type
            else
                emptyL)

#if DEBUG
    let tslotparamL (TSlotParam(nmOpt, ty, inFlag, outFlag, _, _)) =
        (optionL (tagText >> wordL) nmOpt)
        ^^ wordL (tagText ":")
        ^^ typeL ty
        ^^ (if inFlag then wordL (tagText "[in]") else emptyL)
        ^^ (if outFlag then wordL (tagText "[out]") else emptyL)
        ^^ (if inFlag then wordL (tagText "[opt]") else emptyL)
#endif

    let slotSigL (slotsig: SlotSig) =
#if DEBUG
        let (TSlotSig(nm, ty, tps1, tps2, pms, retTy)) = slotsig

        match global_g with
        | None -> wordL (tagText "<no global g>")
        | Some g ->
            let retTy = GetFSharpViewOfReturnType g retTy

            (wordL (tagText "slot") --- (wordL (tagText nm))
             ^^ wordL (tagText "@")
             ^^ typeL ty)
            -- (wordL (tagText "LAM") --- spaceListL (List.map typarL tps1)
                ^^ rightL (tagText "."))
            --- (wordL (tagText "LAM") --- spaceListL (List.map typarL tps2)
                 ^^ rightL (tagText "."))
            --- (commaListL (List.map (List.map tslotparamL >> tupleL) pms))
            ^^ wordL (tagText "-> ") --- (typeL retTy)
#else
        ignore slotsig
        wordL (tagText "slotsig")
#endif

    let valAtBindL v =
        let vL = valL v
        let vL = (if v.IsMutable then wordL (tagText "mutable") ++ vL else vL)

        let vL =
            if layoutTypes then
                vL ^^ wordL (tagText ":") ^^ typeL v.Type
            else
                vL

        let vL =
            match v.ValReprInfo with
            | Some info when layoutValReprInfo -> vL ^^ wordL (tagText "!") ^^ valReprInfoL info
            | _ -> vL

        vL

    let unionCaseRefL (ucr: UnionCaseRef) = wordL (tagText ucr.CaseName)

    let recdFieldRefL (rfref: RecdFieldRef) = wordL (tagText rfref.FieldName)

    // Note: We need nice printing of constants in order to print literals and attributes
    let constL c =
        let str =
            match c with
            | Const.Bool x -> if x then "true" else "false"
            | Const.SByte x -> (x |> string) + "y"
            | Const.Byte x -> (x |> string) + "uy"
            | Const.Int16 x -> (x |> string) + "s"
            | Const.UInt16 x -> (x |> string) + "us"
            | Const.Int32 x -> (x |> string)
            | Const.UInt32 x -> (x |> string) + "u"
            | Const.Int64 x -> (x |> string) + "L"
            | Const.UInt64 x -> (x |> string) + "UL"
            | Const.IntPtr x -> (x |> string) + "n"
            | Const.UIntPtr x -> (x |> string) + "un"
            | Const.Single d ->
                (let s = d.ToString("g12", System.Globalization.CultureInfo.InvariantCulture)

                 if String.forall (fun c -> Char.IsDigit c || c = '-') s then
                     s + ".0"
                 else
                     s)
                + "f"
            | Const.Double d ->
                let s = d.ToString("g12", System.Globalization.CultureInfo.InvariantCulture)

                if String.forall (fun c -> Char.IsDigit c || c = '-') s then
                    s + ".0"
                else
                    s
            | Const.Char c -> "'" + c.ToString() + "'"
            | Const.String bs -> "\"" + bs + "\""
            | Const.Unit -> "()"
            | Const.Decimal bs -> string bs + "M"
            | Const.Zero -> "default"

        wordL (tagText str)

    let layoutUnionCaseArgTypes argTys =
        sepListL (wordL (tagText "*")) (List.map typeL argTys)

    let ucaseL prefixL (ucase: UnionCase) =
        let nmL = wordL (tagText ucase.DisplayName)

        match ucase.RecdFields |> List.map (fun rfld -> rfld.FormalType) with
        | [] -> (prefixL ^^ nmL)
        | argTys -> (prefixL ^^ nmL ^^ wordL (tagText "of")) --- layoutUnionCaseArgTypes argTys

    let layoutUnionCases ucases =
        let prefixL =
            if not (isNilOrSingleton ucases) then
                wordL (tagText "|")
            else
                emptyL

        List.map (ucaseL prefixL) ucases

    let layoutRecdField (fld: RecdField) =
        let lhs = wordL (tagText fld.LogicalName)

        let lhs =
            if fld.IsMutable then
                wordL (tagText "mutable") --- lhs
            else
                lhs

        let lhs =
            if layoutTypes then
                lhs ^^ rightL (tagText ":") ^^ typeL fld.FormalType
            else
                lhs

        lhs

    let tyconReprL (repr, tycon: Tycon) =
        match repr with
        | TFSharpTyconRepr { fsobjmodel_kind = TFSharpUnion } -> tycon.UnionCasesAsList |> layoutUnionCases |> aboveListL
        | TFSharpTyconRepr r ->
            match r.fsobjmodel_kind with
            | TFSharpDelegate _ -> wordL (tagText "delegate ...")
            | _ ->
                let start =
                    match r.fsobjmodel_kind with
                    | TFSharpClass -> "class"
                    | TFSharpInterface -> "interface"
                    | TFSharpStruct -> "struct"
                    | TFSharpEnum -> "enum"
                    | _ -> failwith "???"

                let inherits =
                    match r.fsobjmodel_kind, tycon.TypeContents.tcaug_super with
                    | TFSharpClass, Some super -> [ wordL (tagText "inherit") ^^ (typeL super) ]
                    | TFSharpInterface, _ ->
                        tycon.ImmediateInterfacesOfFSharpTycon
                        |> List.filter (fun (_, compgen, _) -> not compgen)
                        |> List.map (fun (ity, _, _) -> wordL (tagText "inherit") ^^ (typeL ity))
                    | _ -> []

                let vsprs =
                    tycon.MembersOfFSharpTyconSorted
                    |> List.filter (fun v -> v.IsDispatchSlot)
                    |> List.map (fun vref -> valAtBindL vref.Deref)

                let vals =
                    tycon.TrueFieldsAsList
                    |> List.map (fun f ->
                        (if f.IsStatic then wordL (tagText "static") else emptyL)
                        ^^ wordL (tagText "val")
                        ^^ layoutRecdField f)

                let alldecls = inherits @ vsprs @ vals

                let emptyMeasure =
                    match tycon.TypeOrMeasureKind with
                    | TyparKind.Measure -> isNil alldecls
                    | _ -> false

                if emptyMeasure then
                    emptyL
                else
                    (wordL (tagText start) @@-- aboveListL alldecls) @@ wordL (tagText "end")

        | TAsmRepr _ -> wordL (tagText "(# ... #)")
        | TMeasureableRepr ty -> typeL ty
        | TILObjectRepr(TILObjectReprData(_, _, td)) -> wordL (tagText td.Name)
        | _ -> failwith "unreachable"

    let rec bindingL (TBind(v, repr, _)) =
        (valAtBindL v ^^ wordL (tagText "=")) @@-- exprL repr

    and exprL expr = exprWrapL false expr

    and atomL expr =
        // true means bracket if needed to be atomic expr
        exprWrapL true expr

    and letRecL binds bodyL =
        let eqnsL =
            binds
            |> List.mapHeadTail (fun bind -> wordL (tagText "rec") ^^ bindingL bind ^^ wordL (tagText "in")) (fun bind ->
                wordL (tagText "and") ^^ bindingL bind ^^ wordL (tagText "in"))

        (aboveListL eqnsL @@ bodyL)

    and letL bind bodyL =
        let eqnL = wordL (tagText "let") ^^ bindingL bind
        (eqnL @@ bodyL)

    and exprWrapL isAtomic expr =
        let wrap = bracketIfL isAtomic // wrap iff require atomic expr

        let lay =
            match expr with
            | Expr.Const(c, _, _) -> constL c

            | Expr.Val(v, flags, _) ->
                let xL = valL v.Deref

                let xL =
                    match flags with
                    | PossibleConstrainedCall _ -> xL ^^ rightL (tagText "<constrained>")
                    | CtorValUsedAsSelfInit -> xL ^^ rightL (tagText "<selfinit>")
                    | CtorValUsedAsSuperInit -> xL ^^ rightL (tagText "<superinit>")
                    | VSlotDirectCall -> xL ^^ rightL (tagText "<vdirect>")
                    | NormalValUse -> xL

                xL

            | Expr.Sequential(expr1, expr2, flag, _) ->
                aboveListL
                    [
                        exprL expr1
                        match flag with
                        | NormalSeq -> ()
                        | ThenDoSeq -> wordL (tagText "ThenDo")
                        exprL expr2
                    ]
                |> wrap

            | Expr.Lambda(_, _, baseValOpt, argvs, body, _, _) ->
                let formalsL = spaceListL (List.map valAtBindL argvs)

                let bindingL =
                    match baseValOpt with
                    | None -> wordL (tagText "fun") ^^ formalsL ^^ wordL (tagText "->")
                    | Some basev ->
                        wordL (tagText "fun")
                        ^^ (leftL (tagText "base=") ^^ valAtBindL basev) --- formalsL
                        ^^ wordL (tagText "->")

                (bindingL @@-- exprL body) |> wrap

            | Expr.TyLambda(_, tps, body, _, _) ->
                ((wordL (tagText "FUN") ^^ layoutTyparDecls tps ^^ wordL (tagText "->"))
                 ++ exprL body)
                |> wrap

            | Expr.TyChoose(tps, body, _) ->
                ((wordL (tagText "CHOOSE") ^^ layoutTyparDecls tps ^^ wordL (tagText "->"))
                 ++ exprL body)
                |> wrap

            | Expr.App(f, _, tys, argTys, _) ->
                let flayout = atomL f
                appL flayout tys argTys |> wrap

            | Expr.LetRec(binds, body, _, _) -> letRecL binds (exprL body) |> wrap

            | Expr.Let(bind, body, _, _) -> letL bind (exprL body) |> wrap

            | Expr.Link rX -> exprL rX.Value |> wrap

            | Expr.DebugPoint(DebugPointAtLeafExpr.Yes m, rX) ->
                aboveListL [ wordL (tagText "__debugPoint(") ^^ rangeL m ^^ wordL (tagText ")"); exprL rX ]
                |> wrap

            | Expr.Match(_, _, dtree, targets, _, _) ->
                leftL (tagText "[")
                ^^ (decisionTreeL dtree
                    @@ aboveListL (List.mapi targetL (targets |> Array.toList)) ^^ rightL (tagText "]"))

            | Expr.Op(TOp.UnionCase c, _, args, _) -> (unionCaseRefL c ++ spaceListL (List.map atomL args)) |> wrap

            | Expr.Op(TOp.ExnConstr ecref, _, args, _) -> wordL (tagText ecref.LogicalName) ^^ bracketL (commaListL (List.map atomL args))

            | Expr.Op(TOp.Tuple _, _, xs, _) -> tupleL (List.map exprL xs)

            | Expr.Op(TOp.Recd(ctor, tcref), _, xs, _) ->
                let fields = tcref.TrueInstanceFieldsAsList

                let lay fs x =
                    (wordL (tagText fs.rfield_id.idText) ^^ sepL (tagText "=")) --- (exprL x)

                let ctorL =
                    match ctor with
                    | RecdExpr -> emptyL
                    | RecdExprIsObjInit -> wordL (tagText "(new)")

                leftL (tagText "{")
                ^^ aboveListL (List.map2 lay fields xs)
                ^^ rightL (tagText "}")
                ^^ ctorL

            | Expr.Op(TOp.ValFieldSet rf, _, [ rx; x ], _) ->
                (atomL rx --- wordL (tagText "."))
                ^^ (recdFieldRefL rf ^^ wordL (tagText "<-") --- exprL x)

            | Expr.Op(TOp.ValFieldSet rf, _, [ x ], _) -> recdFieldRefL rf ^^ wordL (tagText "<-") --- exprL x

            | Expr.Op(TOp.ValFieldGet rf, _, [ rx ], _) -> atomL rx ^^ rightL (tagText ".#") ^^ recdFieldRefL rf

            | Expr.Op(TOp.ValFieldGet rf, _, [], _) -> recdFieldRefL rf

            | Expr.Op(TOp.ValFieldGetAddr(rf, _), _, [ rx ], _) ->
                leftL (tagText "&")
                ^^ bracketL (atomL rx ^^ rightL (tagText ".!") ^^ recdFieldRefL rf)

            | Expr.Op(TOp.ValFieldGetAddr(rf, _), _, [], _) -> leftL (tagText "&") ^^ (recdFieldRefL rf)

            | Expr.Op(TOp.UnionCaseTagGet tycr, _, [ x ], _) -> wordL (tagText (tycr.LogicalName + ".tag")) ^^ atomL x

            | Expr.Op(TOp.UnionCaseProof c, _, [ x ], _) -> wordL (tagText (c.CaseName + ".proof")) ^^ atomL x

            | Expr.Op(TOp.UnionCaseFieldGet(c, i), _, [ x ], _) -> wordL (tagText (c.CaseName + "." + string i)) --- atomL x

            | Expr.Op(TOp.UnionCaseFieldSet(c, i), _, [ x; y ], _) ->
                ((atomL x --- (rightL (tagText ("#" + c.CaseName + "." + string i))))
                 ^^ wordL (tagText ":="))
                --- exprL y

            | Expr.Op(TOp.TupleFieldGet(_, i), _, [ x ], _) -> wordL (tagText ("#" + string i)) --- atomL x

            | Expr.Op(TOp.Coerce, [ ty; _ ], [ x ], _) -> atomL x --- (wordL (tagText ":>") ^^ typeL ty)

            | Expr.Op(TOp.Reraise, [ _ ], [], _) -> wordL (tagText "Reraise")

            | Expr.Op(TOp.ILAsm(instrs, retTypes), tyargs, args, _) ->
                let instrs = instrs |> List.map (sprintf "%+A" >> tagText >> wordL) |> spaceListL // %+A has + since instrs are from an "internal" type
                let instrs = leftL (tagText "(#") ^^ instrs ^^ rightL (tagText "#)")
                let instrL = appL instrs tyargs args

                let instrL =
                    if layoutTypes then
                        instrL ^^ wordL (tagText ":") ^^ spaceListL (List.map typeAtomL retTypes)
                    else
                        instrL

                instrL |> wrap

            | Expr.Op(TOp.LValueOp(lvop, vr), _, args, _) ->
                (lvalopL lvop ^^ valRefL vr --- bracketL (commaListL (List.map atomL args)))
                |> wrap

            | Expr.Op(TOp.ILCall(_, _, _, _, _, _, _, ilMethRef, _enclTypeInst, _methInst, _), _tyargs, args, _) ->
                let meth = ilMethRef.Name

                (wordL (tagText ilMethRef.DeclaringTypeRef.FullName)
                 ^^ sepL (tagText ".")
                 ^^ wordL (tagText meth))
                ---- (if args.IsEmpty then
                          wordL (tagText "()")
                      else
                          listL exprL args)
                //if not enclTypeInst.IsEmpty then yield wordL(tagText "tinst ") --- listL typeL enclTypeInst
                //if not methInst.IsEmpty then yield wordL (tagText "minst ") --- listL typeL methInst
                //if not tyargs.IsEmpty then yield wordL (tagText "tyargs") --- listL typeL tyargs

                |> wrap

            | Expr.Op(TOp.Array, [ _ ], xs, _) -> leftL (tagText "[|") ^^ commaListL (List.map exprL xs) ^^ rightL (tagText "|]")

            | Expr.Op(TOp.While _, [], [ Expr.Lambda(_, _, _, [ _ ], x1, _, _); Expr.Lambda(_, _, _, [ _ ], x2, _, _) ], _) ->
                let headerL = wordL (tagText "while") ^^ exprL x1 ^^ wordL (tagText "do")
                headerL @@-- exprL x2

            | Expr.Op(TOp.IntegerForLoop _,
                      [],
                      [ Expr.Lambda(_, _, _, [ _ ], x1, _, _); Expr.Lambda(_, _, _, [ _ ], x2, _, _); Expr.Lambda(_, _, _, [ _ ], x3, _, _) ],
                      _) ->
                let headerL =
                    wordL (tagText "for")
                    ^^ exprL x1
                    ^^ wordL (tagText "to")
                    ^^ exprL x2
                    ^^ wordL (tagText "do")

                headerL @@-- exprL x3

            | Expr.Op(TOp.TryWith _,
                      [ _ ],
                      [ Expr.Lambda(_, _, _, [ _ ], x1, _, _); Expr.Lambda(_, _, _, [ _ ], xf, _, _); Expr.Lambda(_, _, _, [ _ ], xh, _, _) ],
                      _) ->
                (wordL (tagText "try") @@-- exprL x1)
                @@ (wordL (tagText "with-filter") @@-- exprL xf)
                @@ (wordL (tagText "with") @@-- exprL xh)

            | Expr.Op(TOp.TryFinally _, [ _ ], [ Expr.Lambda(_, _, _, [ _ ], x1, _, _); Expr.Lambda(_, _, _, [ _ ], x2, _, _) ], _) ->
                (wordL (tagText "try") @@-- exprL x1)
                @@ (wordL (tagText "finally") @@-- exprL x2)
            | Expr.Op(TOp.Bytes _, _, _, _) -> wordL (tagText "bytes++")

            | Expr.Op(TOp.UInt16s _, _, _, _) -> wordL (tagText "uint16++")
            | Expr.Op(TOp.RefAddrGet _, _tyargs, _args, _) -> wordL (tagText "GetRefLVal...")
            | Expr.Op(TOp.TraitCall _, _tyargs, _args, _) -> wordL (tagText "traitcall...")
            | Expr.Op(TOp.ExnFieldGet _, _tyargs, _args, _) -> wordL (tagText "TOp.ExnFieldGet...")
            | Expr.Op(TOp.ExnFieldSet _, _tyargs, _args, _) -> wordL (tagText "TOp.ExnFieldSet...")
            | Expr.Op(TOp.TryFinally _, _tyargs, args, _) -> wordL (tagText "unexpected-try-finally") ---- aboveListL (List.map atomL args)
            | Expr.Op(TOp.TryWith _, _tyargs, args, _) -> wordL (tagText "unexpected-try-with") ---- aboveListL (List.map atomL args)
            | Expr.Op(TOp.Goto l, _tys, args, _) ->
                wordL (tagText ("Expr.Goto " + string l))
                ^^ bracketL (commaListL (List.map atomL args))
            | Expr.Op(TOp.Label l, _tys, args, _) ->
                wordL (tagText ("Expr.Label " + string l))
                ^^ bracketL (commaListL (List.map atomL args))
            | Expr.Op(_, _tys, args, _) -> wordL (tagText "Expr.Op ...") ^^ bracketL (commaListL (List.map atomL args))
            | Expr.Quote(a, _, _, _, _) -> leftL (tagText "<@") ^^ atomL a ^^ rightL (tagText "@>")

            | Expr.Obj(_lambdaId, ty, basev, ccall, overrides, iimpls, _) ->
                (leftL (tagText "{")
                 @@-- ((wordL (tagText "new ") ++ typeL ty)
                       @@-- aboveListL
                           [
                               exprL ccall
                               match basev with
                               | None -> ()
                               | Some b -> valAtBindL b
                               yield! List.map tmethodL overrides
                               yield! List.map iimplL iimpls
                           ]))
                @@ rightL (tagText "}")

            | Expr.WitnessArg _ -> wordL (tagText "<witnessarg>")

            | Expr.StaticOptimization(_tcs, csx, x, _) ->
                (wordL (tagText "opt") @@- (exprL x))
                @@-- (wordL (tagText "|") ^^ exprL csx --- wordL (tagText "when..."))

        // For tracking ranges through expr rewrites
        if layoutRanges then
            aboveListL [ leftL (tagText "//") ^^ rangeL expr.Range; lay ]
        else
            lay

    and appL flayout tys args =
        let z = flayout
        let z = if isNil tys then z else z ^^ instL typeL tys

        let z =
            if isNil args then
                z
            else
                z --- spaceListL (List.map atomL args)

        z

    and decisionTreeL x =
        match x with
        | TDBind(bind, body) ->
            let bind = wordL (tagText "let") ^^ bindingL bind
            (bind @@ decisionTreeL body)
        | TDSuccess(args, n) ->
            wordL (tagText "Success")
            ^^ leftL (tagText "T")
            ^^ intL n
            ^^ tupleL (args |> List.map exprL)
        | TDSwitch(test, dcases, dflt, _) ->
            (wordL (tagText "Switch") --- exprL test)
            @@-- (aboveListL (List.map dcaseL dcases)
                  @@ match dflt with
                     | None -> emptyL
                     | Some dtree -> wordL (tagText "dflt:") --- decisionTreeL dtree)

    and dcaseL (TCase(test, dtree)) =
        (dtestL test ^^ wordL (tagText "//")) --- decisionTreeL dtree

    and dtestL x =
        match x with
        | DecisionTreeTest.UnionCase(c, tinst) -> wordL (tagText "is") ^^ unionCaseRefL c ^^ instL typeL tinst
        | DecisionTreeTest.ArrayLength(n, ty) -> wordL (tagText "length") ^^ intL n ^^ typeL ty
        | DecisionTreeTest.Const c -> wordL (tagText "is") ^^ constL c
        | DecisionTreeTest.IsNull -> wordL (tagText "isnull")
        | DecisionTreeTest.IsInst(_, ty) -> wordL (tagText "isinst") ^^ typeL ty
        | DecisionTreeTest.ActivePatternCase(exp, _, _, _, _, _) -> wordL (tagText "query") ^^ exprL exp
        | DecisionTreeTest.Error _ -> wordL (tagText "error recovery")

    and targetL i (TTarget(argvs, body, _)) =
        leftL (tagText "T")
        ^^ intL i
        ^^ tupleL (flatValsL argvs)
        ^^ rightL (tagText ":") --- exprL body

    and flatValsL vs = vs |> List.map valL

    and tmethodL (TObjExprMethod(TSlotSig(nm, _, _, _, _, _), _, tps, vs, e, _)) =
        (wordL (tagText "member")
         ^^ (wordL (tagText nm))
         ^^ layoutTyparDecls tps
         ^^ tupleL (List.map (List.map valAtBindL >> tupleL) vs)
         ^^ rightL (tagText "="))
        @@-- exprL e

    and iimplL (ty, tmeths) =
        wordL (tagText "impl") ^^ aboveListL (typeL ty :: List.map tmethodL tmeths)

    let rec tyconL (tycon: Tycon) =

        let lhsL =
            wordL (
                tagText (
                    match tycon.TypeOrMeasureKind with
                    | TyparKind.Measure -> "[<Measure>] type"
                    | TyparKind.Type -> "type"
                )
            )
            ^^ wordL (tagText tycon.DisplayName)
            ^^ layoutTyparDecls tycon.TyparsNoRange

        let lhsL = lhsL --- layoutAttribs tycon.Attribs

        let memberLs =
            let adhoc =
                tycon.MembersOfFSharpTyconSorted
                |> List.filter (fun v -> not v.IsDispatchSlot)
                |> List.filter (fun v -> not v.Deref.IsClassConstructor)
                // Don't print individual methods forming interface implementations - these are currently never exported
                |> List.filter (fun v -> isNil (Option.get v.MemberInfo).ImplementedSlotSigs)

            let iimpls =
                match tycon.TypeReprInfo with
                | TFSharpTyconRepr r when
                    (match r.fsobjmodel_kind with
                     | TFSharpInterface -> true
                     | _ -> false)
                    ->
                    []
                | _ -> tycon.ImmediateInterfacesOfFSharpTycon

            let iimpls = iimpls |> List.filter (fun (_, compgen, _) -> not compgen)
            // if TFSharpInterface, the iimpls should be printed as inherited interfaces
            if isNil adhoc && isNil iimpls then
                emptyL
            else
                let iimplsLs =
                    iimpls |> List.map (fun (ty, _, _) -> wordL (tagText "interface") --- typeL ty)

                let adhocLs = adhoc |> List.map (fun vref -> valAtBindL vref.Deref)

                (wordL (tagText "with") @@-- aboveListL (iimplsLs @ adhocLs))
                @@ wordL (tagText "end")

        let reprL =
            match tycon.TypeReprInfo with
#if !NO_TYPEPROVIDERS
            | TProvidedTypeRepr _
            | TProvidedNamespaceRepr _
#endif
            | TNoRepr ->
                match tycon.TypeAbbrev with
                | None -> lhsL @@-- memberLs
                | Some a -> (lhsL ^^ wordL (tagText "=")) --- (typeL a @@ memberLs)
            | a ->
                let rhsL = tyconReprL (a, tycon) @@ memberLs
                (lhsL ^^ wordL (tagText "=")) @@-- rhsL

        reprL

    and entityL (entity: Entity) =
        if entity.IsModuleOrNamespace then
            moduleOrNamespaceL entity
        else
            tyconL entity

    and mexprL mtyp defs =
        let resL = mdefL defs

        let resL =
            if layoutTypes then
                resL @@- (wordL (tagText ":") @@- moduleOrNamespaceTypeL mtyp)
            else
                resL

        resL

    and mdefsL defs =
        wordL (tagText "Module Defs") @@-- aboveListL (List.map mdefL defs)

    and mdefL x =
        match x with
        | TMDefRec(_, _, tycons, mbinds, _) -> aboveListL ((tycons |> List.map tyconL) @ (mbinds |> List.map mbindL))
        | TMDefLet(bind, _) -> letL bind emptyL
        | TMDefDo(e, _) -> exprL e
        | TMDefOpens _ -> wordL (tagText "open ... ")
        | TMDefs defs -> mdefsL defs

    and mbindL x =
        match x with
        | ModuleOrNamespaceBinding.Binding bind -> letL bind emptyL
        | ModuleOrNamespaceBinding.Module(mspec, rhs) ->
            let titleL =
                wordL (tagText (if mspec.IsNamespace then "namespace" else "module"))
                ^^ (wordL (tagText mspec.DemangledModuleOrNamespaceName) |> stampL mspec.Stamp)

            titleL @@-- mdefL rhs

    and moduleOrNamespaceTypeL (mtyp: ModuleOrNamespaceType) =
        aboveListL [ qlistL typeOfValL mtyp.AllValsAndMembers; qlistL tyconL mtyp.AllEntities ]

    and moduleOrNamespaceL (ms: ModuleOrNamespace) =
        let header =
            wordL (tagText "module")
            ^^ (wordL (tagText ms.DemangledModuleOrNamespaceName) |> stampL ms.Stamp)
            ^^ wordL (tagText ":")

        let footer = wordL (tagText "end")
        let body = moduleOrNamespaceTypeL ms.ModuleOrNamespaceType
        (header @@-- body) @@ footer

    let implFileL (CheckedImplFile(signature = implFileTy; contents = implFileContents)) =
        aboveListL
            [
                wordL (tagText "top implementation ") @@-- mexprL implFileTy implFileContents
            ]

    let implFilesL implFiles =
        aboveListL (List.map implFileL implFiles)

    let showType x = showL (typeL x)

    let showExpr x = showL (exprL x)

    let traitL x =
        auxTraitL SimplifyTypes.typeSimplificationInfo0 x

    let typarsL x = layoutTyparDecls x

    type TypedTreeNode =
        {
            Kind: string
            Name: string
            Children: TypedTreeNode list
        }

    let rec visitEntity (entity: Entity) : TypedTreeNode =
        let kind =
            if entity.IsModule then "module"
            elif entity.IsNamespace then "namespace"
            else "other"

        let children =
            if not entity.IsModuleOrNamespace then
                Seq.empty
            else
                seq {
                    yield! Seq.map visitEntity entity.ModuleOrNamespaceType.AllEntities
                    yield! Seq.map visitVal entity.ModuleOrNamespaceType.AllValsAndMembers
                }

        {
            Kind = kind
            Name = entity.CompiledName
            Children = Seq.toList children
        }

    and visitVal (v: Val) : TypedTreeNode =
        let children =
            seq {
                match v.ValReprInfo with
                | None -> ()
                | Some reprInfo ->
                    yield!
                        reprInfo.ArgInfos
                        |> Seq.collect (fun argInfos ->
                            argInfos
                            |> Seq.map (fun argInfo ->
                                {
                                    Name = argInfo.Name |> Option.map (fun i -> i.idText) |> Option.defaultValue ""
                                    Kind = "ArgInfo"
                                    Children = []
                                }))

                yield!
                    v.Typars
                    |> Seq.map (fun typar ->
                        {
                            Name = typar.Name
                            Kind = "Typar"
                            Children = []
                        })
            }

        {
            Name = v.CompiledName None
            Kind = "val"
            Children = Seq.toList children
        }

    let rec serializeNode (writer: IndentedTextWriter) (addTrailingComma: bool) (node: TypedTreeNode) =
        writer.WriteLine("{")
        // Add indent after opening {
        writer.Indent <- writer.Indent + 1

        writer.WriteLine($"\"name\": \"{node.Name}\",")
        writer.WriteLine($"\"kind\": \"{node.Kind}\",")

        if node.Children.IsEmpty then
            writer.WriteLine("\"children\": []")
        else
            writer.WriteLine("\"children\": [")

            // Add indent after opening [
            writer.Indent <- writer.Indent + 1

            node.Children
            |> List.iteri (fun idx -> serializeNode writer (idx + 1 < node.Children.Length))

            // Remove indent before closing ]
            writer.Indent <- writer.Indent - 1
            writer.WriteLine("]")

        // Remove indent before closing }
        writer.Indent <- writer.Indent - 1

        if addTrailingComma then
            writer.WriteLine("},")
        else
            writer.WriteLine("}")

    let serializeEntity path (entity: Entity) =
        let root = visitEntity entity
        use sw = new System.IO.StringWriter()
        use writer = new IndentedTextWriter(sw)
        serializeNode writer false root
        writer.Flush()
        let json = sw.ToString()

        use out =
            FileSystem.OpenFileForWriteShim(path, fileMode = System.IO.FileMode.Create)

        out.WriteAllText(json)
