module FSharp.Compiler.TypeTreeSerialization

open System.CodeDom.Compiler
open FSharp.Compiler.Text
open Internal.Utilities.Library

open FSharp.Compiler.IO
open FSharp.Compiler.TypedTree

type TypedTreeNode =
    {
        Kind: string
        Name: string
        Children: TypedTreeNode list
        Flags: int64 option
        Range: range option
        CompilationPath: CompilationPath option
    }

let rec visitEntity (entity: Entity) : TypedTreeNode =
    let kind =
        if entity.IsModule then "module"
        elif entity.IsNamespace then "namespace"
        elif entity.IsUnionTycon then "union"
        elif entity.IsRecordTycon then "record"
        elif entity.IsFSharpClassTycon then "class"
        elif entity.IsErased then "erased"
        elif entity.IsEnumTycon then "enum"
        elif entity.IsTypeAbbrev then "abbreviation"
        elif entity.IsFSharpObjectModelTycon then "objectModel"
        elif entity.IsFSharpException then "exception"
        elif entity.IsFSharpDelegateTycon then "delegate"
        elif entity.IsFSharpInterfaceTycon then "interface"
        else "other"

    let children =
        seq {
            if entity.IsModuleOrNamespace then
                yield! Seq.map visitEntity entity.ModuleOrNamespaceType.AllEntities

            yield! Seq.map visitVal entity.ModuleOrNamespaceType.AllValsAndMembers
            yield! visitAttributes entity.Attribs
        }

    {
        Kind = kind
        Name = entity.CompiledName
        Children = Seq.toList children
        Flags = Some entity.entity_flags.PickledBits
        Range = Some entity.Range
        CompilationPath = Some entity.CompilationPath
    }

and visitVal (v: Val) : TypedTreeNode =
    let children =
        seq {
            yield! visitAttributes v.Attribs

            match v.ValReprInfo with
            | None -> ()
            | Some (ValReprInfo (_, args, result)) ->
                yield! args |> Seq.collect id |> Seq.map visitArgReprInfo
                yield visitArgReprInfo result

            yield!
                v.Typars
                |> Seq.map (fun typar ->
                    {
                        Name = typar.Name
                        Kind = "typar"
                        Children = []
                        Flags = Some typar.typar_flags.PickledBits
                        Range = Some typar.Range
                        CompilationPath = None
                    })
        }

    {
        Name = v.CompiledName None
        Kind = "val"
        Children = Seq.toList children
        Flags = Some v.val_flags.PickledBits
        Range = Some v.Range
        CompilationPath = None
    }

and visitAttribute (a: Attrib) : TypedTreeNode =
    {
        Kind = "attribute"
        Name = a.TyconRef.CompiledName
        Children = List.empty
        Flags = None
        Range = Some a.Range
        // I don't think the tycon ComplicationPath is relevant here.
        CompilationPath = None
    }

and visitAttributes (attribs: Attribs) : TypedTreeNode seq = List.map visitAttribute attribs

and visitArgReprInfo (argReprInfo: ArgReprInfo) =
    {
        Name = argReprInfo.Name |> Option.map (fun i -> i.idText) |> Option.defaultValue ""
        Kind = "ArgInfo"
        Children = []
        Flags = None
        Range = None
        CompilationPath = None
    }

let write (writer: IndentedTextWriter) key value =
    writer.WriteLine($"\"%s{key}\": \"{value}\",")

let rec serializeNode (writer: IndentedTextWriter) (addTrailingComma: bool) (node: TypedTreeNode) =
    writer.WriteLine("{")
    // Add indent after opening {
    writer.Indent <- writer.Indent + 1

    write writer "name" node.Name
    write writer "kind" node.Kind

    node.Flags |> Option.iter (write writer "flags")
    node.Range |> Option.iter (write writer "range")

    node.CompilationPath
    |> Option.iter (fun cp -> cp.MangledPath |> String.concat "," |> write writer "compilationPath")

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

let rec serializeEntity path (entity: Entity) =
    let root = visitEntity entity
    use sw = new System.IO.StringWriter()
    use writer = new IndentedTextWriter(sw)
    serializeNode writer false root
    writer.Flush()
    let json = sw.ToString()

    use out =
        FileSystem.OpenFileForWriteShim(path, fileMode = System.IO.FileMode.Create)

    out.WriteAllText(json)
