module FSharp.Compiler.Interactive.FsiHelp

[<assembly: System.Runtime.InteropServices.ComVisible(false)>]
[<assembly: System.CLSCompliant(true)>]
do ()

open System
open System.Collections.Generic
open System.IO
open System.Text
open System.Reflection
open FSharp.Compiler

module Parser =

    open System.Xml

    type Help =
        {
            Summary: string
            Remarks: string option
            Parameters: (string * string) list
            Returns: string option
            Exceptions: (string * string) list
            Examples: (string * string) list
            FullName: string
            Assembly: string
        }

        member this.ToDisplayString() =
            let sb = StringBuilder()

            let parameters =
                this.Parameters
                |> List.map (fun (name, description) -> sprintf "- %s: %s" name description)
                |> String.concat "\n"

            sb.AppendLine($"\nDescription:\n%s{this.Summary}") |> ignore

            match this.Remarks with
            | Some r -> sb.AppendLine $"\nRemarks:\n%s{r}" |> ignore
            | None -> ()

            if not (String.IsNullOrWhiteSpace(parameters)) then
                sb.AppendLine $"\nParameters:\n%s{parameters}" |> ignore

            match this.Returns with
            | Some r -> sb.AppendLine $"Returns:\n%s{r}" |> ignore
            | None -> ()

            if not this.Exceptions.IsEmpty then
                sb.AppendLine "\nExceptions:" |> ignore

                for (exType, exDesc) in this.Exceptions do
                    sb.AppendLine $"%s{exType}: %s{exDesc}" |> ignore

            if not this.Examples.IsEmpty then
                sb.AppendLine "\nExamples:" |> ignore

                for example, desc in this.Examples do
                    sb.AppendLine example |> ignore

                    if not (String.IsNullOrWhiteSpace(desc)) then
                        sb.AppendLine $"""// {desc.Replace("\n", "\n// ")}""" |> ignore

                    sb.AppendLine "" |> ignore

            sb.AppendLine $"Full name: %s{this.FullName}" |> ignore
            sb.AppendLine $"Assembly: %s{this.Assembly}\n" |> ignore

            sb.ToString()

    let cleanupXmlContent (s: string) = s.Replace("\n ", "\n").Trim() // some stray whitespace from the XML

    // remove any leading `X:` and trailing `N
    let trimDotNet (s: string) =
        let s = if s.Length > 2 && s[1] = ':' then s.Substring(2) else s
        let idx = s.IndexOf('`')
        let s = if idx > 0 then s.Substring(0, idx) else s
        s

    let xmlDocCache = Dictionary<string, string>()

    let getXmlDocument xmlPath =
        match xmlDocCache.TryGetValue(xmlPath) with
        | true, value ->
            let xmlDocument = XmlDocument()
            xmlDocument.LoadXml(value)
            xmlDocument
        | _ ->
            let rawXml = File.ReadAllText(xmlPath)
            let xmlDocument = XmlDocument()
            xmlDocument.LoadXml(rawXml)
            xmlDocCache.Add(xmlPath, rawXml)
            xmlDocument

    let getTexts (node: Xml.XmlNode) =
        seq {
            for child in node.ChildNodes do
                if child.Name = "#text" then
                    yield child.Value

                if child.Name = "c" then
                    yield child.InnerText

                if child.Name = "see" then
                    let cref = child.Attributes.GetNamedItem("cref")

                    if not (isNull cref) then
                        yield cref.Value |> trimDotNet
        }
        |> String.concat ""

    let helpText (xmlPath: string) (assembly: string) (modName: string) (implName: string) (sourceName: string) =
        let sourceName = sourceName.Replace('.', '#') // for .ctor
        let implName = implName.Replace('.', '#') // for .ctor
        let xmlName = $"{modName}.{implName}"
        let xmlDocument = getXmlDocument xmlPath

        let node =
            let toTry =
                [
                    $"/doc/members/member[contains(@name, ':{xmlName}`')]"
                    $"/doc/members/member[contains(@name, ':{xmlName}(')]"
                    $"/doc/members/member[contains(@name, ':{xmlName}')]"
                ]

            seq {
                for t in toTry do
                    let node = xmlDocument.SelectSingleNode(t)
                    if not (isNull node) then Some node else None
            }
            |> Seq.tryPick id

        match node with
        | None -> None
        | Some n ->
            let summary =
                n.SelectSingleNode("summary")
                |> Option.ofObj
                |> Option.map getTexts
                |> Option.map cleanupXmlContent

            let remarks =
                n.SelectSingleNode("remarks")
                |> Option.ofObj
                |> Option.map getTexts
                |> Option.map cleanupXmlContent

            let parameters =
                n.SelectNodes("param")
                |> Seq.cast<XmlNode>
                |> Seq.map (fun n -> n.Attributes.GetNamedItem("name").Value.Trim(), n.InnerText.Trim())
                |> List.ofSeq

            let returns =
                n.SelectSingleNode("returns")
                |> Option.ofObj
                |> Option.map (fun n -> getTexts(n).Trim())

            let exceptions =
                n.SelectNodes("exception")
                |> Seq.cast<XmlNode>
                |> Seq.map (fun n ->
                    let exType = n.Attributes.GetNamedItem("cref").Value
                    let idx = exType.IndexOf(':')
                    let exType = if idx >= 0 then exType.Substring(idx + 1) else exType
                    exType.Trim(), n.InnerText.Trim())
                |> List.ofSeq

            let examples =
                n.SelectNodes("example")
                |> Seq.cast<XmlNode>
                |> Seq.map (fun n ->
                    let codeNode = n.SelectSingleNode("code")

                    let code =
                        if isNull codeNode then
                            ""
                        else
                            n.RemoveChild(codeNode) |> ignore
                            cleanupXmlContent codeNode.InnerText

                    code, cleanupXmlContent n.InnerText)
                |> List.ofSeq

            match summary with
            | Some s ->
                {
                    Summary = s
                    Remarks = remarks
                    Parameters = parameters
                    Returns = returns
                    Exceptions = exceptions
                    Examples = examples
                    FullName = $"{modName}.{sourceName}" // the long ident as users see it
                    Assembly = assembly
                }
                |> Some
            | None -> None

module Expr =

    open Microsoft.FSharp.Quotations.Patterns

    let tryGetSourceName (methodInfo: MethodInfo) =
        try
            let attr = methodInfo.GetCustomAttribute<CompilationSourceNameAttribute>()
            Some attr.SourceName
        with _ ->
            None

    let getInfos (declaringType: Type) (sourceName: string option) (implName: string) =
        let xmlPath = Path.ChangeExtension(declaringType.Assembly.Location, ".xml")
        let assembly = Path.GetFileName(declaringType.Assembly.Location)

        if File.Exists(xmlPath) then
            // for FullName cases like Microsoft.FSharp.Core.FSharpOption`1[System.Object]
            let fullName =
                let idx = declaringType.FullName.IndexOf('[')

                if idx >= 0 then
                    declaringType.FullName.Substring(0, idx)
                else
                    declaringType.FullName

            let fullName = fullName.Replace('+', '.') // for FullName cases like Microsoft.FSharp.Collections.ArrayModule+Parallel

            Some(xmlPath, assembly, fullName, implName, sourceName |> Option.defaultValue implName)
        else
            None

    let rec exprNames expr =
        match expr with
        | Call(exprOpt, methodInfo, _exprList) ->
            match exprOpt with
            | Some _ -> None
            | None ->
                let sourceName = tryGetSourceName methodInfo
                getInfos methodInfo.DeclaringType sourceName methodInfo.Name
        | Lambda(_param, body) -> exprNames body
        | Let(_, _, body) -> exprNames body
        | Value(_o, t) -> getInfos t (Some t.Name) t.Name
        | DefaultValue t -> getInfos t (Some t.Name) t.Name
        | PropertyGet(_o, info, _) -> getInfos info.DeclaringType (Some info.Name) info.Name
        | NewUnionCase(info, _exprList) -> getInfos info.DeclaringType (Some info.Name) info.Name
        | NewObject(ctorInfo, _e) -> getInfos ctorInfo.DeclaringType (Some ctorInfo.Name) ctorInfo.Name
        | NewArray(t, _exprs) -> getInfos t (Some t.Name) t.Name
        | NewTuple _ ->
            let x = (23, 42)
            let t = x.GetType()
            getInfos t (Some t.Name) t.Name
        | NewStructTuple _ ->
            let x = struct (23, 42)
            let t = x.GetType()
            getInfos t (Some t.Name) t.Name
        | _ -> None

module Logic =

    open Expr
    open Parser

    module Quoted =
        let tryGetDocumentation expr =
            match exprNames expr with
            | Some(xmlPath, assembly, modName, implName, sourceName) -> helpText xmlPath assembly modName implName sourceName
            | _ -> None

        let h (expr: Quotations.Expr) =
            match tryGetDocumentation expr with
            | None -> "unable to get documentation"
            | Some d -> d.ToDisplayString()
