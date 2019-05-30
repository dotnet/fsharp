open System
open System.Collections.Generic
open System.IO
open System.Xml

type Expects = { status:string; id:string; span:string; pattern:string }

let stripFromFileExpectations source =

    let readExpect expect =
        let settings = XmlReaderSettings()
        settings.ValidationType <- ValidationType.None
        settings.CheckCharacters <- false
        settings.ConformanceLevel <- ConformanceLevel.Auto
        let rdr = XmlReader.Create(new StringReader(expect), settings)
        printfn "%s" expect
        let mutable element = {status="success";  id = ""; span = ""; pattern = ""}
        let mutable insideExpects = false
        let mutable foundOne = false
        try
            let rec loop () =
                if rdr.Read() then
                    match rdr.NodeType with
                    | XmlNodeType.Element when String.Compare(rdr.Name, "Expects", StringComparison.OrdinalIgnoreCase) = 0  ->
                        insideExpects <- true
                        if rdr.AttributeCount > 0 then
                            let status =rdr.GetAttribute("status")
                            let span = rdr.GetAttribute("span")
                            let id = rdr.GetAttribute("id")
                            element <- {element with status=status; id=id; span = span }
                            foundOne <- true
                    | XmlNodeType.Text -> if insideExpects then element <- { element with pattern = rdr.Value }
                    | XmlNodeType.EndElement when String.Compare(rdr.Name, "Expects", StringComparison.OrdinalIgnoreCase) = 0 ->
                        insideExpects <- false
                    | _ -> ()
                    loop ()
                else printfn "There"; ()
            loop ()
            if foundOne then Some element
            else None
            with | e ->
                printfn "%A" e
                None

    printfn "%s" source
    File.ReadAllLines(source)
    |> Seq.filter(fun line -> line.Trim().StartsWith(@"//"))
    |> Seq.map(fun line -> line.Trim().Substring(2).Trim())
    |> Seq.filter(fun line -> line .StartsWith(@"<Expects", StringComparison.OrdinalIgnoreCase))
    |> Seq.map(fun expect -> readExpect expect)
    |> Seq.filter(fun expect -> expect.IsSome)
    |> Seq.map(fun expect -> expect.Value)

let verifyResults path =
    let expectations = stripFromFileExpectations path

    // Print out discovered expects
    expectations |> Seq.iter(fun expects -> printfn "Expect: status:'%s' id:'%s' span:'%s' pattern:'%s'" expects.status expects.id expects.span expects.pattern)

let files = Directory.EnumerateFiles(@"c:\kevinransom\fsharp\tests\fsharpqa\Source", "*.fs*", SearchOption.AllDirectories)
//let files = [| @"C:\kevinransom\fsharp\tests\fsharpqa\Source\Conformance\BasicTypeAndModuleDefinitions\GeneratedEqualityHashingComparison\Attributes\Legacy\Test20.fs" |]
files |> Seq.iter(fun path -> (verifyResults path) |> ignore )


