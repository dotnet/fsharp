module HandleExpects

open System
open System.IO
open System.Text.RegularExpressions
open System.Xml

type Expects = { status:string; id:string; span:string; pattern:string; mutable matched:bool; line:string }
type ErrorMessage = { source:string; status:string; id:string; span:string; text:string; mutable matched:bool; line:string }
type Span = { startrow:int; startcol:int; endrow:int; endcol:int }

let tryParseSpan (span:string) =
    let s = span.Trim([| '('; ')' |]).Split(',')
    match s.Length with
    | 2 -> { startrow=Int32.Parse(s.[0]); startcol=Int32.Parse(s.[1]); endrow=Int32.MaxValue; endcol=Int32.MaxValue }
    | 4 -> { startrow=Int32.Parse(s.[0]); startcol=Int32.Parse(s.[1]); endrow=Int32.Parse(s.[2]); endcol=Int32.Parse(s.[3]) }
    | _ -> raise (InvalidDataException(sprintf "The span : '%s' is invalid" span));

let isStringEmpty s = String.IsNullOrWhiteSpace(s)
let isStringNotEmpty s = not (isStringEmpty s)
let stringToLower s = if isStringNotEmpty s then s.ToLower() else s
let areStringsEqual s1 s2 = String.Compare(s1, s2, StringComparison.OrdinalIgnoreCase) = 0 
let areSpansEqual s1 s2 =
    let span1 = tryParseSpan s1
    let span2 = tryParseSpan s2
    if span1.startrow <> span2.startrow then false
    elif span1.startcol <> span2.startcol then false
    elif span1.endrow <> span2.endrow then false
    elif span1.endcol <> span2.endcol then false
    else true

let stripFromFileExpectations source =
    let readExpect expect =
        let pattern = "(?<tagOpen><Expects{1}[^>]*>{1})(?<tagContent>.*)(?<tagClose></Expects>)"          //"(<!--((?!-->).)*-->|<\w*((?!\/<).)*\/>|<(?<Expects>\w+)[^>]*>(?>[^<]|(?R))*<\/\k<Expects>\s*>)"
        let rx = new Regex(pattern)
        let matched = rx.Match(expect)
        if matched.Success then
            // The content of the Expects group contains a lot of invalid Xml and the Xml reader fails when it sees it.
            // So we just save it away, remove it from the xml, then read the xml and put it back
            // Save away the contents of the element and strip it out of expect pattern
            let content = (matched.Groups.[2]).ToString()
            let nocontentxpect = 
                if isStringEmpty content then expect
                else expect.Replace(content, "")

            let rdr = XmlReader.Create(new StringReader(nocontentxpect))
            let mutable element = { status="success";  id = ""; span = ""; pattern = content; matched = false; line=expect }
            let mutable insideExpects = false
            let mutable foundOne = false
            try
                let rec loop () =
                    if rdr.Read() then
                        match rdr.NodeType with
                        | XmlNodeType.Element when String.Compare(rdr.Name, "Expects", StringComparison.OrdinalIgnoreCase) = 0  ->
                            insideExpects <- true
                            if rdr.AttributeCount > 0 then
                                let status = stringToLower (rdr.GetAttribute("status"))
                                let span = rdr.GetAttribute("span")
                                let id = stringToLower  (rdr.GetAttribute("id"))
                                element <- {element with status=status; id=id; span=span }
                                foundOne <- true
                        | XmlNodeType.EndElement when String.Compare(rdr.Name, "Expects", StringComparison.OrdinalIgnoreCase) = 0 ->
                            insideExpects <- false
                        | _ -> ()
                        loop ()
                    else ()
                loop ()
                if foundOne then Some element
                else None
                with | e -> printfn "Oops !!! %A" e; reraise()
        else None

    File.ReadAllLines(source)
    |> Array.filter(fun line -> line.Trim().StartsWith(@"//"))
    |> Array.map(fun line -> line.Trim().Substring(2).Trim())
    |> Array.filter(fun line -> line.StartsWith(@"<Expects", StringComparison.OrdinalIgnoreCase))
    |> Array.map(fun expect -> readExpect expect)
    |> Array.filter(fun expect -> expect.IsSome)
    |> Array.map(fun expect -> expect.Value)

let readErrorMessagesFromOutput output =
    //Formats of error messages
    // Syntax error in code:
    // 1. filename(row,col): (sometext perhaps typecheck) error|warning ErrorNo: ErrorText
    //    e.g:  Program.fs(5,9): error ErrorNo: ErrorText
    // 2. Program.fs(5,3,5,20): (sometext perhaps typecheck) error FS0039: ErrorText
    //      e.g:
    //          Program.fs(5,3,5,20): (sometext perhaps typecheck) error FS0039: PicturePoint ...
    // 3. error ErrorNo: ErrorText
    //    e.g: error FS0207: No inputs specified
    let getErrorMessage line pattern =
        let rx = new Regex(pattern)
        let matched = rx.Match(line)
        let getMatchForName (name:string) = matched.Groups.[name].ToString()

        if matched.Success then Some {
            source = (getMatchForName "tagSourceFileName") 
            status = stringToLower  (getMatchForName "tagStatus")
            id = stringToLower (getMatchForName "tagErrorNo")
            span = (getMatchForName "tagSpan")
            text = (getMatchForName "tagText")
            matched = false
            line = line
            }
        else None

    let rgxTagSourceFileName = "(?<tagSourceFileName>[^(]{1,})(?:[(]{1})"
    let rgxTagSpan = "(?<tagSpan>[^):]{1,})(?:[)]{1})(?:[(\s:]*)"
    let rgxTagStatus = "(?<tagStatus>(error|typecheck error|warning|success|notin))"
    let rgxColonWhiteSpace = "(?:[\s:]*)"
    let rgxWhiteSpace = "(?:[\s]*)"
    let rgxTagErrorNo = "(?<tagErrorNo>\s*[^:\s]*)"
    let rgxTagText = "(?<tagText>.*)"
    let rgxTagTail = "(?<tagTail>\s\[.*\]$)"

    // E.g: Q:\version46\test.fs(25,13): error FS0010: Unexpected symbol '.' in member definition. Expected 'with', '=' or other token. [Q:\Temp\FSharp.Cambridge\vaw2t1vp.cai\f0bi0hny.wwx.fsproj]
    let rgxFull = rgxTagSourceFileName + rgxTagSpan + rgxColonWhiteSpace + rgxTagStatus + rgxWhiteSpace + rgxTagErrorNo + rgxColonWhiteSpace + rgxTagText + rgxWhiteSpace + rgxTagTail

    // E.g: FSC : error FS0010: Unexpected symbol '.' in member definition. Expected 'with', '=' or other token. [Q:\Temp\FSharp.Cambridge\vaw2t1vp.cai\f0bi0hny.wwx.fsproj]
    let rgxShort = rgxTagStatus + rgxTagErrorNo + rgxColonWhiteSpace + rgxTagText + rgxWhiteSpace + rgxTagTail
    [|
        for line in output do
            let errorMessage =
                getErrorMessage line rgxFull
                |> Option.orElse (getErrorMessage line rgxShort)
            match errorMessage with
            | Some e -> yield e
            | _ -> ()
    |]

let compareResults output (expectations:Expects array) (errorMessages:ErrorMessage array) =
    for expect in expectations do
        match expect.status with
        | "error"
        | "typecheck error"
        | "warning" ->
            // Check for this error/warning in found errors list
            for msg in errorMessages do
                let matched =
                    if isStringNotEmpty expect.id && not (areStringsEqual expect.id msg.id) then false
                    elif isStringNotEmpty expect.status && not (areStringsEqual expect.status msg.status) then false
                    elif isStringNotEmpty expect.span && not (areSpansEqual expect.span msg.span) then false
                    elif isStringNotEmpty expect.pattern then
                        let regex = new Regex(expect.pattern)
                        let matched = regex.Match(msg.text)
                        matched.Success
                    else true
                if matched then
                    expect.matched <- true
                    msg.matched <- true
        | "success" ->
            // In this case search for text in the page
            let regex = new Regex(expect.pattern)
            for line in output do
                let matched = regex.Match(line)
                if matched.Success then expect.matched <- true
        | "notin" ->
            // In this case search for text not appearing in the page
            let regex = new Regex(expect.pattern)
            let mutable found = false
            for line in output do
                let matched = regex.Match(line)
                if matched.Success then found <- true
            if not found then expect.matched <- true
        | _ -> ()

let verifyResults source outputPath =
    let output = File.ReadAllLines(outputPath)
    let expectations = stripFromFileExpectations source
    if expectations.Length > 0 then
        // There must be at least one <Expects></Expects> to do this testing
        let errorMessages = readErrorMessagesFromOutput output
        compareResults output expectations errorMessages

        // Print out discovered expects
        let verifiedexpectations =
            expectations
            |> Seq.fold(fun result expects ->
                if not (expects.matched) then
                    printfn "Failed to match expected result '%s'" expects.line
                    false
                else result
                ) true
        let verifiederrormessages =
                errorMessages
                |> Seq.fold(fun result msg ->
                    if not (msg.matched) then
                        printfn "Failed to match produced error message: '%s'" msg.line
                        false
                    else result
                    ) true

        if not (verifiedexpectations && verifiederrormessages) then
            failwith (sprintf "Failed validating error codes")

//HandleExpects.verifyResults @"C:\Users\kevinr\AppData\Local\Temp\FSharp.Cambridge\bcnyzkvb.ict\test.fs" @"C:\Users\kevinr\AppData\Local\Temp\FSharp.Cambridge\bcnyzkvb.ict\buildoutput.txt"