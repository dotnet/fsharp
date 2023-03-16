module FSharp.Compiler.TypeTreeSerialization

open System.CodeDom.Compiler
open Internal.Utilities.Library

open FSharp.Compiler.IO
open FSharp.Compiler.TypedTree

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
