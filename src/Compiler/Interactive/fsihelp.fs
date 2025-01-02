module FSharp.Compiler.Interactive.FsiHelp

open System
open System.Collections.Generic
open System.IO
open System.Text
open System.Reflection
open FSharp.Compiler.IO

// 3261 Is the nullness warning. I really tried to properly check all accesses, but the chosen xml API has nulls everywhere and is not a good fit for compiler nullness checking.
// Even basic constructs like `n.Attributes.GetNamedItem("name").Value` have `| null| on every single dot access.
#nowarn "3261"

module Parser =

    open System.Xml
    open System.Collections.Concurrent

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

            sb.AppendLine().AppendLine("Description:").AppendLine(this.Summary) |> ignore

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
            sb.AppendLine $"Assembly: %s{this.Assembly}" |> ignore

            sb.ToString()

    let cleanupXmlContent (s: string) = s.Replace("\n ", "\n").Trim() // some stray whitespace from the XML

    // remove any leading `X:` and trailing `N
    let trimDotNet (s: string) =
        let s = if s.Length > 2 && s[1] = ':' then s.Substring(2) else s
        let idx = s.IndexOf('`')
        let s = if idx > 0 then s.Substring(0, idx) else s
        s

    let xmlDocCache = ConcurrentDictionary<string, Lazy<XmlDocument>>()

    let tryGetXmlDocument xmlPath =
        let valueFactory xmlPath =
            lazy
                use stream = FileSystem.OpenFileForReadShim(xmlPath)
                let rawXml = stream.ReadAllText()
                let xmlDocument = XmlDocument()
                xmlDocument.LoadXml(rawXml)
                xmlDocument

        try
            Some(xmlDocCache.GetOrAdd(xmlPath, valueFactory).Value)
        with _ ->
            None

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

    let tryMkHelp (xmlDocument: XmlDocument option) (assembly: string) (modName: string) (implName: string) (sourceName: string) =
        let sourceName = sourceName.Replace('.', '#') // for .ctor
        let implName = implName.Replace('.', '#') // for .ctor
        let xmlName = $"{modName}.{implName}"

        let toTry =
            [
                $"""/doc/members/member[contains(@name, ":{xmlName}`")]"""
                $"""/doc/members/member[contains(@name, ":{xmlName}(")]"""
                $"""/doc/members/member[contains(@name, ":{xmlName}")]"""
            ]

        xmlDocument
        |> Option.bind (fun xmlDocument ->
            seq {
                for t in toTry do
                    let node = xmlDocument.SelectSingleNode(t)
                    if not (isNull node) then Some node else None
            }
            |> Seq.tryPick id)
        |> function
            | None -> ValueNone
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
                    |> ValueSome
                | None -> ValueNone

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
        let xmlDoc = Parser.tryGetXmlDocument xmlPath
        let assembly = Path.GetFileName(declaringType.Assembly.Location)

        // for FullName cases like Microsoft.FSharp.Core.FSharpOption`1[System.Object]
        let fullName =
            let idx = declaringType.FullName.IndexOf('[')

            if idx >= 0 then
                declaringType.FullName.Substring(0, idx)
            else
                declaringType.FullName

        let fullName = fullName.Replace('+', '.') // for FullName cases like Microsoft.FSharp.Collections.ArrayModule+Parallel

        (xmlDoc, assembly, fullName, implName, sourceName |> Option.defaultValue implName)

    let rec exprNames expr =
        match expr with
        | Call(exprOpt, methodInfo, _exprList) ->
            match exprOpt with
            | Some _ -> None
            | None ->
                let sourceName = tryGetSourceName methodInfo
                getInfos methodInfo.DeclaringType sourceName methodInfo.Name |> Some
        | Lambda(_param, body) -> exprNames body
        | Let(_, _, body) -> exprNames body
        | Value(_o, t) -> getInfos t (Some t.Name) t.Name |> Some
        | DefaultValue t -> getInfos t (Some t.Name) t.Name |> Some
        | PropertyGet(_o, info, _) -> getInfos info.DeclaringType (Some info.Name) info.Name |> Some
        | NewUnionCase(info, _exprList) -> getInfos info.DeclaringType (Some info.Name) info.Name |> Some
        | NewObject(ctorInfo, _e) -> getInfos ctorInfo.DeclaringType (Some ctorInfo.Name) ctorInfo.Name |> Some
        | NewArray(t, _exprs) -> getInfos t (Some t.Name) t.Name |> Some
        | NewTuple _ ->
            let ty = typeof<_ * _>
            getInfos ty (Some ty.Name) ty.Name |> Some
        | NewStructTuple _ ->
            let ty = typeof<struct (_ * _)>
            getInfos ty (Some ty.Name) ty.Name |> Some
        | _ -> None

module Logic =

    open Expr
    open Parser

    module Quoted =
        let tryGetHelp (expr: Quotations.Expr) =
            match exprNames expr with
            | Some(xmlDocument, assembly, modName, implName, sourceName) -> tryMkHelp xmlDocument assembly modName implName sourceName
            | _ -> ValueNone

        let h (expr: Quotations.Expr) =
            match tryGetHelp expr with
            | ValueNone -> "unable to get documentation\n"
            | ValueSome d -> d.ToDisplayString()
