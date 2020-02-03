// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.SourceCodeServices

open System
open System.IO
open System.IO.MemoryMappedFiles
open System.Reflection.Metadata
open FSharp.NativeInterop
open FSharp.Compiler
open FSharp.Compiler.Range
open FSharp.Compiler.Tast
open FSharp.Compiler.Infos
open FSharp.Compiler.NameResolution
open FSharp.Compiler.AbstractIL.IL

#nowarn "9"

[<Sealed>]
type ItemKeyStore(mmf: MemoryMappedFile, length) =

    let mutable isDisposed = false
    let checkDispose() =
        if isDisposed then
            raise (ObjectDisposedException("ItemKeyReader"))

    let viewAccessor = mmf.CreateViewAccessor()

    // This has to be mutable because BlobReader is a struct and we have to mutate its contents.
    let mutable reader = BlobReader(viewAccessor.SafeMemoryMappedViewHandle.DangerousGetHandle() |> NativePtr.ofNativeInt, int length)

    member _.ReadRange() =
        let startLine = reader.ReadInt32()
        let startColumn = reader.ReadInt32()
        let endLine = reader.ReadInt32()
        let endColumn = reader.ReadInt32()
        let fileIndex = reader.ReadInt32()

        let posStart = mkPos startLine startColumn
        let posEnd = mkPos endLine endColumn
        mkFileIndexRange fileIndex posStart posEnd

    member _.ReadKeyString() =
        let size = reader.ReadInt32()
        let keyString = ReadOnlySpan<byte>(reader.CurrentPointer |> NativePtr.toVoidPtr, size)
        reader.Offset <- reader.Offset + size
        keyString

    member this.FindAll(item: Item) =
        checkDispose ()

        let builder = ItemKeyStoreBuilder()
        builder.Write(Range.range0, item)
        match builder.TryBuildAndReset() with
        | None -> Seq.empty
        | Some(singleStore : ItemKeyStore) ->
            singleStore.ReadRange() |> ignore
            let keyString1 = singleStore.ReadKeyString()

            let results = ResizeArray()

            reader.Offset <- 0
            while reader.Offset < reader.Length do
                let m = this.ReadRange()
                let keyString2 = this.ReadKeyString()
                if keyString1.SequenceEqual keyString2 then
                    results.Add m

            (singleStore :> IDisposable).Dispose()

            results :> range seq

    interface IDisposable with

        member _.Dispose() =
            isDisposed <- true
            viewAccessor.Dispose()
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
        writeString "#E#"
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
            writeString "#T#"
            tinst |> List.iter writeType
        | TType_anon (anonInfo, tinst) ->
            writeString "#N#"
            writeString anonInfo.ILTypeRef.BasicQualifiedName
            tinst |> List.iter writeType
        | TType_fun (d, r) ->
            writeString "#F#"
            writeType d
            writeType r
        | TType_measure ms -> 
            writeString "#M#"
            writeMeasure ms
        | TType_var tp ->
            writeTypar tp
        | TType_ucase (uc, _) ->
            match uc with
            | UnionCaseRef.UCRef(tcref, nm) ->
                writeString "#U#"
                writeEntityRef tcref
                writeString nm

    and writeMeasure (ms: Measure) =
        match ms with
        | Measure.Var typar -> 
            writeString "#p#"
            writeTypar typar
        | Measure.Con tcref -> 
            writeString "#c#"
            writeEntityRef tcref
        | Measure.Prod(ms1, ms2) ->
            writeString "#r#"
            writeMeasure ms1
            writeMeasure ms2
        | Measure.Inv ms ->
            writeString "#i#"
            writeMeasure ms
        | Measure.One ->
            writeString "#1#"
        | Measure.RationalPower _ ->
            writeString "#z#"

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
                writeString "m$"
                writeEntityRef memberInfo.ApparentEnclosingEntity
                writeString vref.LogicalName
                writeType vref.Type
            | _ ->
                writeString "v$"
                writeValRef vref

        | Item.UnionCase(info, _) -> 
            writeString "u$"
            writeEntityRef info.TyconRef
            
        | Item.ActivePatternResult(info, ty, _, _) ->
            writeString "r$"
            info.ActiveTagsWithRanges
            |> List.iter (fun (nm, _) ->
                writeString nm)
            writeType ty

        | Item.ActivePatternCase elemRef ->
            writeString "c$"
            writeValRef elemRef.ActivePatternVal
            elemRef.ActivePatternInfo.ActiveTagsWithRanges
            |> List.iter (fun (nm, _) -> writeString nm)

        | Item.ExnCase tcref ->
            writeString "e$"
            writeEntityRef tcref

        | Item.RecdField info ->
            writeString "d$"
            writeEntityRef info.TyconRef
            writeType info.FieldType

        | Item.AnonRecdField(info, tys, i, _) ->
            writeString "a$"
            writeString info.ILTypeRef.BasicQualifiedName
            tys |> List.iter writeType
            writeInt32 i

        | Item.NewDef ident ->
            writeString "n$"
            writeString ident.idText

        | Item.ILField info ->
            writeString "l$"
            writeString info.ILTypeRef.BasicQualifiedName
            writeString info.FieldName

        | Item.Event info ->
            writeString "t$"
            writeString info.EventName
            writeEntityRef info.DeclaringTyconRef

        | Item.Property(nm, infos) ->
            writeString "p$"
            writeString nm
            infos
            |> List.iter (fun info -> writeEntityRef info.DeclaringTyconRef)

        | Item.TypeVar(nm, typar) ->
            writeString "y$"
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
            writeString "o$"
            x.CompilationPath.DemangledPath
            |> List.iter (fun x -> 
                writeString x
                writeString ".")
            writeString x.LogicalName

        | Item.DelegateCtor ty ->
            writeString "g$"
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