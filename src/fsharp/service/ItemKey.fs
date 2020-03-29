// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.SourceCodeServices

open System
open System.IO
open System.IO.MemoryMappedFiles
open System.Reflection.Metadata

open FSharp.NativeInterop

open FSharp.Compiler
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.Infos
open FSharp.Compiler.NameResolution
open FSharp.Compiler.Range
open FSharp.Compiler.TypedTree

#nowarn "9"

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
    let typeMeasureProd = "#r#"

    [<Literal>]
    let typeMeasureInv = "#i#"

    [<Literal>]
    let typeMeasureOne = "#1#"

    [<Literal>]
    let typeMeasureRationalPower = "#z#"

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
    let itemTypeVar = "y$"

    [<Literal>]
    let itemModuleOrNamespace = "o$"

    [<Literal>]
    let itemDelegateCtor = "g$"

[<Sealed>]
type ItemKeyStore(mmf: MemoryMappedFile, length) =

    let mutable isDisposed = false
    let checkDispose() =
        if isDisposed then
            raise (ObjectDisposedException("ItemKeyStore"))

    member _.ReadRange(reader: byref<BlobReader>) =
        let startLine = reader.ReadInt32()
        let startColumn = reader.ReadInt32()
        let endLine = reader.ReadInt32()
        let endColumn = reader.ReadInt32()
        let fileIndex = reader.ReadInt32()

        let posStart = mkPos startLine startColumn
        let posEnd = mkPos endLine endColumn
        mkFileIndexRange fileIndex posStart posEnd

    member _.ReadKeyString(reader: byref<BlobReader>) =
        let size = reader.ReadInt32()
        let keyString = ReadOnlySpan<byte>(reader.CurrentPointer |> NativePtr.toVoidPtr, size)
        reader.Offset <- reader.Offset + size
        keyString

    member this.ReadFirstKeyString() =
        use view = mmf.CreateViewAccessor(0L, length)
        let mutable reader = BlobReader(view.SafeMemoryMappedViewHandle.DangerousGetHandle() |> NativePtr.ofNativeInt, int length)
        this.ReadRange &reader |> ignore
        let bytes = (this.ReadKeyString &reader).ToArray()
        ReadOnlySpan.op_Implicit bytes

    member this.FindAll(item: Item) =
        checkDispose ()

        let builder = ItemKeyStoreBuilder()
        builder.Write(Range.range0, item)
        match builder.TryBuildAndReset() with
        | None -> Seq.empty
        | Some(singleStore : ItemKeyStore) ->
            let keyString1 = singleStore.ReadFirstKeyString()
            (singleStore :> IDisposable).Dispose()

            let results = ResizeArray()

            use view = mmf.CreateViewAccessor(0L, length)
            let mutable reader = BlobReader(view.SafeMemoryMappedViewHandle.DangerousGetHandle() |> NativePtr.ofNativeInt, int length)

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

and [<Sealed>] ItemKeyStoreBuilder() =

    let b = BlobBuilder()

    let writeChar (c: char) =
        b.WriteUInt16(uint16 c)

    let writeUInt16 (i: uint16) =
        b.WriteUInt16 i

    let writeInt32 (i: int) =
        b.WriteInt32 i

    let writeInt64 (i: int64) =
        b.WriteInt64 i

    let writeString (str: string) =
        b.WriteUTF16 str

    let writeRange (m: Range.range) =
        b.WriteInt32(m.StartLine)
        b.WriteInt32(m.StartColumn)
        b.WriteInt32(m.EndLine)
        b.WriteInt32(m.EndColumn)
        b.WriteInt32(m.FileIndex)

    let writeEntityRef (eref: EntityRef) =
        writeString ItemKeyTags.entityRef
        writeString eref.CompiledName
        eref.CompilationPath.MangledPath
        |> List.iter (fun str -> writeString str)

    let rec writeILType (ilty: ILType) =
        match ilty with
        | ILType.TypeVar n -> writeString "!"; writeUInt16 n
        | ILType.Modified (_, _, ty2) -> writeILType ty2
        | ILType.Array (ILArrayShape s, ty) -> 
            writeILType ty
            writeString "[" 
            writeInt32 (s.Length-1)
            writeString "]"
        | ILType.Value tr 
        | ILType.Boxed tr -> 
            tr.TypeRef.Enclosing
            |> List.iter (fun x ->
                writeString x
                writeChar '.')
            writeChar '.'
            writeString tr.TypeRef.Name
        | ILType.Void -> 
            writeString "void"
        | ILType.Ptr ty -> 
            writeString "ptr<"
            writeILType ty
            writeChar '>'
        | ILType.Byref ty ->
            writeString "byref<"
            writeILType ty
            writeChar '>'
        | ILType.FunctionPointer mref ->
            mref.ArgTypes
            |> List.iter (fun x ->
                writeILType x)
            writeILType mref.ReturnType

    let rec writeType (ty: TType) =
        match ty with
        | TType_forall (_, ty) ->
            writeType ty
        | TType_app (tcref, _) ->
            writeEntityRef tcref
        | TType_tuple (_, tinst) ->
            writeString ItemKeyTags.typeTuple
            tinst |> List.iter writeType
        | TType_anon (anonInfo, tinst) ->
            writeString ItemKeyTags.typeAnonymousRecord
            writeString anonInfo.ILTypeRef.BasicQualifiedName
            tinst |> List.iter writeType
        | TType_fun (d, r) ->
            writeString ItemKeyTags.typeFunction
            writeType d
            writeType r
        | TType_measure ms -> 
            writeString ItemKeyTags.typeMeasure
            writeMeasure ms
        | TType_var tp ->
            writeTypar tp
        | TType_ucase (uc, _) ->
            match uc with
            | UnionCaseRef.UnionCaseRef(tcref, nm) ->
                writeString ItemKeyTags.typeUnionCase
                writeEntityRef tcref
                writeString nm

    and writeMeasure (ms: Measure) =
        match ms with
        | Measure.Var typar -> 
            writeString ItemKeyTags.typeMeasureVar
            writeTypar typar
        | Measure.Con tcref -> 
            writeString ItemKeyTags.typeMeasureCon
            writeEntityRef tcref
        | Measure.Prod(ms1, ms2) ->
            writeString ItemKeyTags.typeMeasureProd
            writeMeasure ms1
            writeMeasure ms2
        | Measure.Inv ms ->
            writeString ItemKeyTags.typeMeasureInv
            writeMeasure ms
        | Measure.One ->
            writeString ItemKeyTags.typeMeasureOne
        | Measure.RationalPower _ ->
            writeString ItemKeyTags.typeMeasureRationalPower

    and writeTypar (typar: Typar) =
        match typar.Solution with
        | Some ty -> writeType ty
        | _ -> writeInt64 typar.Stamp

    let writeValRef (vref: ValRef) =
        match vref.MemberInfo with
        | Some memberInfo ->
            writeString "m$"
            writeEntityRef memberInfo.ApparentEnclosingEntity
            writeString vref.LogicalName
            writeType vref.Type
        | _ ->
            writeString "v$"
            writeString vref.LogicalName
            writeType vref.Type
            match vref.DeclaringEntity with
            | ParentNone -> writeChar '%'
            | Parent eref -> writeEntityRef eref

    member _.Write (m: Range.range, item: Item) =
        writeRange m

        let fixup = b.ReserveBytes 4 |> BlobWriter

        let preCount = b.Count

        match item with
        | Item.Value vref ->
            match vref.MemberInfo with
            | Some memberInfo ->
                writeString ItemKeyTags.itemValueMember
                writeEntityRef memberInfo.ApparentEnclosingEntity
                writeString vref.LogicalName
                writeType vref.Type
            | _ ->
                writeString ItemKeyTags.itemValue
                writeValRef vref

        | Item.UnionCase(info, _) -> 
            writeString ItemKeyTags.typeUnionCase
            writeEntityRef info.TyconRef
            writeString info.Name
            
        | Item.ActivePatternResult(info, _, _, _) ->
            writeString ItemKeyTags.itemActivePattern
            info.ActiveTagsWithRanges
            |> List.iter (fun (nm, _) ->
                writeString nm)

        | Item.ActivePatternCase elemRef ->
            writeString ItemKeyTags.itemActivePattern
            elemRef.ActivePatternInfo.ActiveTagsWithRanges
            |> List.iter (fun (nm, _) -> writeString nm)

        | Item.ExnCase tcref ->
            writeString ItemKeyTags.itemExnCase
            writeEntityRef tcref

        | Item.RecdField info ->
            writeString ItemKeyTags.itemRecordField
            writeEntityRef info.TyconRef
            writeString info.Name
            writeType info.FieldType

        | Item.UnionCaseField(info, fieldIndex) ->
            writeString ItemKeyTags.typeUnionCase
            writeEntityRef info.TyconRef
            writeString info.Name
            writeInt32 fieldIndex
        
        | Item.AnonRecdField(info, tys, i, _) ->
            writeString ItemKeyTags.itemAnonymousRecordField
            writeString info.ILTypeRef.BasicQualifiedName
            tys |> List.iter writeType
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

        | Item.Property(nm, infos) ->
            writeString ItemKeyTags.itemProperty
            writeString nm
            infos
            |> List.iter (fun info -> writeEntityRef info.DeclaringTyconRef)

        | Item.TypeVar(nm, typar) ->
            writeString ItemKeyTags.itemTypeVar
            writeString nm
            writeTypar typar

        | Item.Types(_, [ty]) ->
            writeType ty

        | Item.UnqualifiedType [tcref] ->
            writeEntityRef tcref

        | Item.MethodGroup(_, [info], _) 
        | Item.CtorGroup(_, [info]) ->
            match info with
            | FSMeth(_, _, vref, _) ->
                writeValRef vref
            | ILMeth(_, info, _) ->
                info.ILMethodRef.ArgTypes
                |> List.iter writeILType
                writeILType info.ILMethodRef.ReturnType
                writeString info.ILName
                writeType info.ApparentEnclosingType
            | _ ->
                writeString "m$"
                writeEntityRef info.DeclaringTyconRef
                writeString info.LogicalName

        | Item.ModuleOrNamespaces [x] ->
            writeString ItemKeyTags.itemModuleOrNamespace
            x.CompilationPath.DemangledPath
            |> List.iter (fun x -> 
                writeString x
                writeString ".")
            writeString x.LogicalName

        | Item.DelegateCtor ty ->
            writeString ItemKeyTags.itemDelegateCtor
            writeType ty

        | Item.MethodGroup _ -> ()
        | Item.CtorGroup _ -> ()
        | Item.FakeInterfaceCtor _ -> ()
        | Item.Types _ -> ()
        | Item.CustomOperation _ -> ()
        | Item.CustomBuilder _ -> ()
        | Item.ModuleOrNamespaces _ -> ()
        | Item.ImplicitOp _ -> ()
        | Item.ArgName _ -> ()
        | Item.SetterArg _ -> ()
        | Item.UnqualifiedType _ -> ()

        let postCount = b.Count

        fixup.WriteInt32(postCount - preCount)

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
                        HandleInheritability.None)
                use stream = mmf.CreateViewStream(0L, length, MemoryMappedFileAccess.ReadWrite)
                b.WriteContentTo stream
                mmf

            b.Clear()

            Some(new ItemKeyStore(mmf, length))       
        else
            b.Clear()
            None