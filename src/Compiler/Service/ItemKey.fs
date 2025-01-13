// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.CodeAnalysis

open System
open System.IO
open System.IO.MemoryMappedFiles
open System.Reflection.Metadata
open System.Runtime.InteropServices
open Internal.Utilities.Library
open FSharp.NativeInterop
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.Infos
open FSharp.Compiler.NameResolution
open FSharp.Compiler.Text
open FSharp.Compiler.Text.Range
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeBasics
open FSharp.Compiler.Syntax.PrettyNaming
open FSharp.Compiler.TypedTreeOps
open FSharp.Compiler.TcGlobals

#nowarn "9"
#nowarn "51"

/// These tags are used to create unique item key strings to decrease possible key string collisions when the Items are actually completely different.
[<RequireQualifiedAccess>]
module ItemKeyTags =

    [<Literal>]
    let entityRef = "#E#"

    [<Literal>]
    let typeTuple = "#T#"

    [<Literal>]
    let typeAnonymousRecord = "#N#"

    [<Literal>]
    let typeFunction = "#F#"

    [<Literal>]
    let typeMeasure = "#M#"

    [<Literal>]
    let typeUnionCase = "#U#"

    [<Literal>]
    let typeMeasureVar = "#p#"

    [<Literal>]
    let typeMeasureCon = "#c#"

    [<Literal>]
    let itemValueMember = "m$"

    [<Literal>]
    let itemValue = "v$"

    [<Literal>]
    let itemUnionCase = "u$"

    [<Literal>]
    let itemActivePattern = "r$"

    [<Literal>]
    let itemExnCase = "e$"

    [<Literal>]
    let itemRecordField = "d$"

    [<Literal>]
    let itemAnonymousRecordField = "a$"

    [<Literal>]
    let itemNewDef = "n$"

    [<Literal>]
    let itemILField = "l$"

    [<Literal>]
    let itemEvent = "t$"

    [<Literal>]
    let itemProperty = "p$"

    [<Literal>]
    let itemTrait = "T$"

    [<Literal>]
    let itemTypeVar = "y$"

    [<Literal>]
    let itemModuleOrNamespace = "o$"

    [<Literal>]
    let itemDelegateCtor = "g$"

    [<Literal>]
    let parameters = "p$p$"

[<AutoOpen>]
module DebugKeyStore =

    /// A debugging tool to show what's being written into the ItemKeyStore in a more human readable way in the debugger.
    type DebugKeyStore() =

        let mutable debugCurrentItem = ResizeArray()

        member val Items = ResizeArray()

        member _.WriteRange(m: range) = debugCurrentItem.Add("range", $"{m}")

        member _.WriteEntityRef(eref: EntityRef) =
            debugCurrentItem.Add("EntityRef", $"{eref}")

        member _.WriteILType(ilTy: ILType) =
            debugCurrentItem.Add("ILType", $"%A{ilTy}")

        member _.WriteType isStandalone (ty: TType) =
            debugCurrentItem.Add("Type", $"{isStandalone} %A{ty}")

        member _.WriteMeasure isStandalone (ms: Measure) =
            debugCurrentItem.Add("Measure", $"{isStandalone} %A{ms}")

        member _.WriteTypar (isStandalone: bool) (typar: Typar) =
            debugCurrentItem.Add("Typar", $"{isStandalone} %A{typar}")

        member _.WriteValRef(vref: ValRef) =
            debugCurrentItem.Add("ValRef", $"{vref}")

        member _.WriteValue(vref: ValRef) =
            debugCurrentItem.Add("Value", $"{vref}")

        member _.WriteActivePatternCase (apInfo: ActivePatternInfo) index =
            debugCurrentItem.Add("ActivePatternCase", $"{apInfo} {index}")

        member this.FinishItem(item, length) =
            debugCurrentItem.Add("length", $"{length}")
            this.Items.Add(item, debugCurrentItem)
            let itemCount = this.Items.Count
            assert (itemCount > 0)
            debugCurrentItem <- ResizeArray()

        member _.New() = DebugKeyStore()

    /// A replacement for DebugKeyStore for when we're not debugging.
    type _DebugKeyStoreNoop() =

        member inline _.Items = Unchecked.defaultof<_>

        member inline _.WriteRange(_m: range) = ()

        member inline _.WriteEntityRef(_eref: EntityRef) = ()

        member inline _.WriteILType(_ilTy: ILType) = ()

        member inline _.WriteType _isStandalone (_ty: TType) = ()

        member inline _.WriteMeasure _isStandalone (_ms: Measure) = ()

        member inline _.WriteTypar (_isStandalone: bool) (_typar: Typar) = ()

        member inline _.WriteValRef(_vref: ValRef) = ()

        member inline _.WriteValue(_vref: ValRef) = ()

        member inline _.WriteActivePatternCase (_apInfo: ActivePatternInfo) _index = ()

        member inline _.FinishItem(_item, _length) = ()

        member inline this.New() = this

    let DebugKeyStoreNoop = _DebugKeyStoreNoop ()

[<Sealed>]
type ItemKeyStore(mmf: MemoryMappedFile, length, tcGlobals, debugStore) =

    let rangeBuffer = Array.zeroCreate<byte> sizeof<range>

    let mutable isDisposed = false

    let checkDispose () =
        if isDisposed then
            raise (ObjectDisposedException("ItemKeyStore"))

    member _.DebugStore = debugStore

    member _.ReadRange(reader: byref<BlobReader>) =
        reader.ReadBytes(sizeof<range>, rangeBuffer, 0)
        MemoryMarshal.Cast<byte, range>(Span(rangeBuffer)).[0]

    member _.ReadKeyString(reader: byref<BlobReader>) =
        let size = reader.ReadInt32()

        let keyString =
            ReadOnlySpan<byte>(reader.CurrentPointer |> NativePtr.toVoidPtr, size)

        reader.Offset <- reader.Offset + size
        keyString

    member this.ReadFirstKeyString() =
        use view = mmf.CreateViewAccessor(0L, length)

        let mutable reader =
            BlobReader(view.SafeMemoryMappedViewHandle.DangerousGetHandle() |> NativePtr.ofNativeInt, int length)

        this.ReadRange &reader |> ignore
        let bytes = (this.ReadKeyString &reader).ToArray()
        ReadOnlySpan.op_Implicit bytes

    member this.FindAll(item: Item) =
        checkDispose ()

        let builder = ItemKeyStoreBuilder(tcGlobals)
        builder.Write(range0, item)

        match builder.TryBuildAndReset() with
        | None -> Seq.empty
        | Some(singleStore: ItemKeyStore) ->
            let keyString1 = singleStore.ReadFirstKeyString()
            (singleStore :> IDisposable).Dispose()

            let results = ResizeArray()

            use view = mmf.CreateViewAccessor(0L, length)

            let mutable reader =
                BlobReader(view.SafeMemoryMappedViewHandle.DangerousGetHandle() |> NativePtr.ofNativeInt, int length)

            reader.Offset <- 0

            while reader.Offset < reader.Length do
                let m = this.ReadRange &reader
                let keyString2 = this.ReadKeyString &reader

                if keyString1.SequenceEqual keyString2 then
                    results.Add m

            results :> range seq

    interface IDisposable with

        member _.Dispose() =
            isDisposed <- true
            mmf.Dispose()

and [<Sealed>] ItemKeyStoreBuilder(tcGlobals: TcGlobals) =

    let b = BlobBuilder()

    // Change this to DebugKeyStore() for debugging (DebugStore will be available on ItemKeyStore)
    let mutable debug = DebugKeyStoreNoop

    let writeChar (c: char) = b.WriteUInt16(uint16 c)

    let writeUInt16 (i: uint16) = b.WriteUInt16 i

    let writeInt32 (i: int) = b.WriteInt32 i

    let writeInt64 (i: int64) = b.WriteInt64 i

    let writeString (str: string) = b.WriteUTF16 str

    let writeRange (m: range) =
        debug.WriteRange m
        let mutable m = m
        let ptr = &&m |> NativePtr.toNativeInt |> NativePtr.ofNativeInt<byte>
        b.WriteBytes(ptr, sizeof<range>)

    let writeEntityRef (eref: EntityRef) =
        debug.WriteEntityRef eref
        writeString ItemKeyTags.entityRef
        writeString eref.CompiledName
        eref.CompilationPath.MangledPath |> List.iter writeString

    let rec writeILType (ilTy: ILType) =
        debug.WriteILType ilTy

        match ilTy with
        | ILType.TypeVar n ->
            writeString "!"
            writeUInt16 n

        | ILType.Modified(_, _, ty2) -> writeILType ty2

        | ILType.Array(ILArrayShape s, ty) ->
            writeILType ty
            writeString "["
            writeInt32 (s.Length - 1)
            writeString "]"

        | ILType.Value tr
        | ILType.Boxed tr ->
            tr.TypeRef.Enclosing
            |> List.iter (fun x ->
                writeString x
                writeChar '.')

            writeChar '.'
            writeString tr.TypeRef.Name

        | ILType.Void -> writeString "void"

        | ILType.Ptr ty ->
            writeString "ptr<"
            writeILType ty
            writeChar '>'

        | ILType.Byref ty ->
            writeString "byref<"
            writeILType ty
            writeChar '>'

        | ILType.FunctionPointer mref ->
            mref.ArgTypes |> List.iter writeILType
            writeILType mref.ReturnType

    let rec writeType isStandalone (ty: TType) =
        debug.WriteType isStandalone ty

        match stripTyparEqns ty with
        | TType_forall(_, ty) -> writeType false ty

        | TType_app(tcref, _, _) ->
            match isStandalone, tcref.TypeAbbrev with
            | false, Some ty -> writeType false ty
            | _ -> writeEntityRef tcref

        | TType_tuple(_, tinst) ->
            writeString ItemKeyTags.typeTuple
            tinst |> List.iter (writeType false)

        | TType_anon(anonInfo, tinst) ->
            writeString ItemKeyTags.typeAnonymousRecord
            writeString anonInfo.ILTypeRef.BasicQualifiedName
            tinst |> List.iter (writeType false)

        | TType_fun(domainTy, rangeTy, _) ->
            writeString ItemKeyTags.typeFunction
            writeType false domainTy
            writeType false rangeTy

        | TType_measure ms ->
            if isStandalone then
                writeString ItemKeyTags.typeMeasure
                writeMeasure isStandalone ms

        | TType_var(tp, _) -> writeTypar isStandalone tp

        | TType_ucase(uc, _) ->
            match uc with
            | UnionCaseRef.UnionCaseRef(tcref, nm) ->
                writeString ItemKeyTags.typeUnionCase
                writeEntityRef tcref
                writeString nm

    and writeMeasure isStandalone (ms: Measure) =
        debug.WriteMeasure isStandalone ms

        match ms with
        | Measure.Var typar ->
            writeString ItemKeyTags.typeMeasureVar
            writeTypar isStandalone typar
        | Measure.Const(tyconRef = tcref) ->
            writeString ItemKeyTags.typeMeasureCon
            writeEntityRef tcref
        | _ -> ()

    and writeTypar (isStandalone: bool) (typar: Typar) =
        debug.WriteTypar isStandalone typar

        match typar.Solution with
        | Some ty -> writeType isStandalone ty
        | _ ->
            if isStandalone then
                writeInt64 typar.Stamp

    let writeValRef (vref: ValRef) =
        debug.WriteValRef vref

        match vref.MemberInfo with
        | Some memberInfo ->
            writeString ItemKeyTags.itemValueMember

            match vref.IsOverrideOrExplicitImpl, vref.MemberInfo with
            | true,
              Some {
                       ImplementedSlotSigs = slotSig :: _tail
                   } -> slotSig.DeclaringType |> writeType false
            | _ -> writeEntityRef memberInfo.ApparentEnclosingEntity

            writeString vref.LogicalName
            writeString ItemKeyTags.parameters

            match vref.IsInstanceMember, tryDestFunTy tcGlobals vref.Type with
            // In case of an instance member, we will skip the type of "this" because it will differ
            // between the definition and overrides. Also it's not needed to uniquely identify the reference.
            | true, ValueSome(_thisTy, funTy) -> funTy
            | _ -> vref.Type
            |> writeType false

        | _ ->
            writeString ItemKeyTags.itemValue
            writeString vref.LogicalName
            writeString ItemKeyTags.parameters
            writeType false vref.Type

            match vref.Deref.ArgReprInfoForDisplay with
            | Some({ OtherRange = Some(r) }) -> writeRange r
            | _ -> ()

            match vref.TryDeclaringEntity with
            | ParentNone -> writeChar '%'
            | Parent eref -> writeEntityRef eref

    let writeValue (vref: ValRef) =
        debug.WriteValue vref

        if vref.IsPropertyGetterMethod || vref.IsPropertySetterMethod then
            writeString ItemKeyTags.itemProperty
            writeString vref.PropertyName

            match vref.IsOverrideOrExplicitImpl, vref.MemberInfo with
            | true, Some { ImplementedSlotSigs = slotSig :: _ } -> slotSig.DeclaringType |> writeType false
            | _ ->

                match vref.TryDeclaringEntity with
                | ParentRef.Parent parent -> writeEntityRef parent
                | _ -> ()
        else
            writeValRef vref

    let writeActivePatternCase (apInfo: ActivePatternInfo) index =
        debug.WriteActivePatternCase apInfo index

        writeString ItemKeyTags.itemActivePattern

        match apInfo.ActiveTagsWithRanges with
        | (_, m) :: _ -> m.FileName |> Path.GetFileNameWithoutExtension |> (!!) |> writeString
        | _ -> ()

        for tag in apInfo.ActiveTags do
            writeChar '|'
            writeString tag

        writeInt32 index

    member _.Write(m: range, item: Item) =
        writeRange m

        let fixup = b.ReserveBytes 4 |> BlobWriter

        let preCount = b.Count

        match item with
        | Item.Value vref -> writeValue vref

        | Item.UnionCase(info, _) ->
            writeString ItemKeyTags.typeUnionCase
            writeEntityRef info.TyconRef
            writeString info.LogicalName

        | Item.ActivePatternResult(info, _, index, _) -> writeActivePatternCase info index

        | Item.ActivePatternCase elemRef -> writeActivePatternCase elemRef.ActivePatternInfo elemRef.CaseIndex

        | Item.ExnCase tcref ->
            writeString ItemKeyTags.itemExnCase
            writeEntityRef tcref

        | Item.RecdField info ->
            writeString ItemKeyTags.itemRecordField
            writeEntityRef info.TyconRef
            writeString info.LogicalName
            writeType false info.FieldType

        | Item.UnionCaseField(info, fieldIndex) ->
            writeString ItemKeyTags.typeUnionCase
            writeEntityRef info.TyconRef
            writeString info.LogicalName
            writeInt32 fieldIndex

        | Item.AnonRecdField(info, tys, i, _) ->
            writeString ItemKeyTags.itemAnonymousRecordField
            writeString info.ILTypeRef.BasicQualifiedName
            tys |> List.iter (writeType false)
            writeInt32 i

        | Item.NewDef ident ->
            writeString ItemKeyTags.itemNewDef
            writeString ident.idText

        | Item.ILField info ->
            writeString ItemKeyTags.itemILField
            writeString info.ILTypeRef.BasicQualifiedName
            writeString info.FieldName

        | Item.Event info ->
            writeString ItemKeyTags.itemEvent
            writeString info.EventName
            writeEntityRef info.DeclaringTyconRef

        | Item.Property(nm, infos, _) ->
            writeString ItemKeyTags.itemProperty
            writeString nm

            match infos |> List.tryHead with
            | Some info -> writeEntityRef info.DeclaringTyconRef
            | _ -> ()

        | Item.Trait(info) ->
            writeString ItemKeyTags.itemTrait
            writeString info.MemberLogicalName
            info.SupportTypes |> List.iter (writeType false)
            info.CompiledObjectAndArgumentTypes |> List.iter (writeType false)
            info.CompiledReturnType |> Option.iter (writeType false)

        | Item.TypeVar(_, typar) -> writeTypar true typar

        | Item.Types(_, [ ty ]) -> writeType true ty

        | Item.UnqualifiedType [ tcref ] -> writeEntityRef tcref

        | Item.MethodGroup(_, [ info ], _)
        | Item.CtorGroup(_, [ info ]) ->
            match info with
            | FSMeth(_, ty, vref, _) when vref.IsConstructor -> writeType true ty
            | FSMeth(_, _, vref, _) -> writeValue vref
            | ILMeth(_, ilMethInfo, _) when info.IsConstructor -> writeType true ilMethInfo.ApparentEnclosingType
            | ILMeth(_, ilMethInfo, _) ->
                ilMethInfo.ILMethodRef.ArgTypes |> List.iter writeILType
                writeILType ilMethInfo.ILMethodRef.ReturnType
                writeString ilMethInfo.ILName
                writeType false ilMethInfo.ApparentEnclosingType
            | _ ->
                writeString ItemKeyTags.itemValueMember
                writeEntityRef info.DeclaringTyconRef
                writeString info.LogicalName

        | Item.ModuleOrNamespaces [ x ] ->
            writeString ItemKeyTags.itemModuleOrNamespace

            x.CompilationPath.DemangledPath
            |> List.iter (fun x ->
                writeString x
                writeString ".")

            writeString x.LogicalName

        | Item.DelegateCtor ty ->
            writeString ItemKeyTags.itemDelegateCtor
            writeType false ty

        // Named argument in a signature
        | Item.OtherName(ident = Some(ident); argType = ty; argInfo = Some _) ->
            writeString ItemKeyTags.itemValue
            writeString ident.idText
            writeString ItemKeyTags.parameters
            writeType false ty
            writeRange ident.idRange
            writeChar '%'

        // We should consider writing ItemKey for each of these
        | Item.OtherName _ -> ()
        | Item.CustomOperation _ -> ()
        | Item.CustomBuilder _ -> ()
        | Item.ImplicitOp _ -> ()
        | Item.SetterArg _ -> ()

        // Empty lists do not occur
        | Item.Types(_, []) -> ()
        | Item.UnqualifiedType [] -> ()
        | Item.MethodGroup(_, [], _) -> ()
        | Item.CtorGroup(_, []) -> ()
        | Item.ModuleOrNamespaces [] -> ()

        // Items are flattened so multiples are not expected
        | Item.Types(_, _ :: _ :: _) -> ()
        | Item.UnqualifiedType(_ :: _ :: _) -> ()
        | Item.MethodGroup(_, (_ :: _ :: _), _) -> ()
        | Item.CtorGroup(_, (_ :: _ :: _)) -> ()
        | Item.ModuleOrNamespaces(_ :: _ :: _) -> ()

        let postCount = b.Count

        fixup.WriteInt32(postCount - preCount)
        debug.FinishItem(item, postCount - preCount)

    member _.TryBuildAndReset() =
        if b.Count > 0 then
            let length = int64 b.Count

            let mmf =
                let mmf =
                    MemoryMappedFile.CreateNew(
                        null,
                        length,
                        MemoryMappedFileAccess.ReadWrite,
                        MemoryMappedFileOptions.None,
                        HandleInheritability.None
                    )

                use stream = mmf.CreateViewStream(0L, length, MemoryMappedFileAccess.ReadWrite)
                b.WriteContentTo stream
                mmf

            b.Clear()

            let result = Some(new ItemKeyStore(mmf, length, tcGlobals, debug.Items))
            debug <- debug.New()
            result
        else
            b.Clear()
            debug <- debug.New()
            None
