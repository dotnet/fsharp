namespace FSharp.Compiler.Service.Tests

open System
open System.IO
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.EditorServices
open FSharp.Compiler.IO
open FSharp.Compiler.Symbols
open FSharp.Compiler.Tokenization
open FSharp.Test.Compiler.Assertions.TextBasedDiagnosticAsserts
open TestFramework

[<AutoOpen>]
module EditorServiceAsserts =
    let isIdentChar c =
        Char.IsLetterOrDigit c || c = '_' || c = '\''

    let private markAtOffset (offsetInMarker: string -> int) (source: string) (marker: string) =
        match source.IndexOf(marker, StringComparison.Ordinal) with
        | -1 -> failwithf "Marker %A not found in source" marker
        | i -> source.Insert(i + offsetInMarker marker, "{caret}")

    let markAtStartOfMarker = markAtOffset (fun _ -> 0)

    let markAtEndOfMarker = markAtOffset (fun marker -> marker.Length)

    let markCaretAfterLeadingIdent = markAtOffset (fun marker -> marker |> Seq.takeWhile isIdentChar |> Seq.length)

    let findCompletionItem (name: string) (completionInfo: DeclarationListInfo) =
        let norm = normalizeNewLines name

        match completionInfo.Items |> Array.tryFind (fun i -> normalizeNewLines i.NameInCode = norm || i.NameInList = name) with
        | Some item -> item
        | None ->
            let names = completionInfo.Items |> Array.map _.NameInCode |> String.concat ", "
            failwithf "Expected a completion item named %A but found none. Items: [%s]" name names

    let assertItemGlyph (name: string) (glyph: FSharpGlyph) (completionInfo: DeclarationListInfo) =
        let item = findCompletionItem name completionInfo

        if item.Glyph <> glyph then
            failwithf "Item %A has glyph %A but expected %A" name item.Glyph glyph

    let groupMainDescriptions (ToolTipText elements) =
        elements
        |> List.collect (fun e ->
            match e with
            | ToolTipElement.Group items -> items |> List.map (fun d -> taggedTextToString d.MainDescription)
            | _ -> [])

    let flattenItemDescription (tooltip: ToolTipText) =
        groupMainDescriptions tooltip |> String.concat "\n"

    let assertItemDescriptionOccurrences (expected: int) (itemName: string) (token: string) (completionInfo: DeclarationListInfo) =
        let item = findCompletionItem itemName completionInfo
        let descr = flattenItemDescription item.Description
        let occurrences = descr.Split([| token |], StringSplitOptions.None).Length - 1

        if occurrences <> expected then
            failwithf "Item %A: expected %d occurrence(s) of %A but found %d (description: %s)" itemName expected token occurrences descr

    let assertItemDescriptionContainsExactlyOnce itemName token completionInfo =
        assertItemDescriptionOccurrences 1 itemName token completionInfo

    let private itemsWithPrefix (prefix: string) (ignoreCase: bool) (completionInfo: DeclarationListInfo) =
        let cmp =
            if ignoreCase then StringComparison.OrdinalIgnoreCase else StringComparison.Ordinal

        completionInfo.Items
        |> Array.map _.NameInCode
        |> Array.filter (fun n -> n.StartsWith(prefix, cmp))

    let private assertPrefixUniqueness (unique: bool) (prefix: string) (ignoreCase: bool) (completionInfo: DeclarationListInfo) =
        let matches = itemsWithPrefix prefix ignoreCase completionInfo
        let ok = if unique then matches.Length = 1 else matches.Length >= 2

        if not ok then
            let expectation = if unique then "exactly ONE item" else "AT LEAST TWO items"

            failwithf "Expected %s whose NameInCode start(s) with %A (ignoreCase=%b) but found %d: [%s]"
                expectation prefix ignoreCase matches.Length (String.concat ", " matches)

    let assertPrefixIsUnique = assertPrefixUniqueness true

    let assertPrefixIsNotUnique = assertPrefixUniqueness false

    let private expectedLineOf (definitionLine: string) (sourceLines: string array) =
        match sourceLines |> Array.indexed |> Array.filter (fun (_, l) -> l.Contains definitionLine) with
        | [| (i, _) |] -> i + 1
        | [||] -> failwithf "Definition line containing %A was not found in the source" definitionLine
        | many ->
            failwithf
                "Definition line %A is AMBIGUOUS — it matches %d source lines (1-based: %A); use a more specific substring"
                definitionLine many.Length (many |> Array.map (fun (i, _) -> i + 1))

    let private assertLandedOnLine (landedPrefix: string) (definitionLine: string) (sourceLines: string array) (expectedLine: int) result =
        match result with
        | FindDeclResult.DeclFound range when range.StartLine = expectedLine -> ()
        | FindDeclResult.DeclFound range ->
            let landedText =
                if range.StartLine >= 1 && range.StartLine <= sourceLines.Length then
                    sourceLines.[range.StartLine - 1]
                else
                    "<line out of range>"

            failwithf "%s landed on line %d (%s) but expected line %d (containing %A)"
                landedPrefix range.StartLine landedText expectedLine definitionLine
        | other ->
            failwithf "Expected FindDeclResult.DeclFound on line %d (containing %A) but got %A"
                expectedLine definitionLine other

    let assertGoToDefinitionOnLine (definitionLine: string) (markedSource: string) =
        let context, checkResults = Checker.getCheckedResolveContext markedSource
        let result =
            checkResults.GetDeclarationLocation(context)

        let sourceLines = context.Source.Replace("\r\n", "\n").Split('\n')
        let expectedLine = expectedLineOf definitionLine sourceLines
        assertLandedOnLine "Goto-def" definitionLine sourceLines expectedLine result

    let assertGoToDefinitionFails (markedSource: string) =
        let context, checkResults = Checker.getCheckedResolveContext markedSource
        let result =
            checkResults.GetDeclarationLocation(context)

        match result with
        | FindDeclResult.DeclFound range ->
            failwithf "Expected goto-def to fail (not DeclFound), but it found a definition at %A" range
        | _ -> ()

    let assertGoToDefinitionIsExternal (markedSource: string) =
        let context, checkResults = Checker.getCheckedResolveContext markedSource
        let result =
            checkResults.GetDeclarationLocation(context)

        match result with
        | FindDeclResult.ExternalDecl _ -> ()
        | other ->
            failwithf "Expected FindDeclResult.ExternalDecl (resolved-but-external), but got %A" other

    let assertGoToDefinitionOperatorOnLine (definitionLine: string) (operatorName: string) (markedSource: string) =
        let context, checkResults = Checker.getCheckedResolveContext markedSource
        let result =
            checkResults.GetDeclarationLocation(context.Pos.Line, context.Pos.Column + 1, context.LineText, [ operatorName ])

        let sourceLines = context.Source.Replace("\r\n", "\n").Split('\n')
        let expectedLine = expectedLineOf definitionLine sourceLines
        assertLandedOnLine "Operator goto-def" definitionLine sourceLines expectedLine result

    let assertGoToDefinitionToExternalLine (definitionLine: string) (markedSource: string) =
        let context, checkResults = Checker.getCheckedResolveContext markedSource
        let result =
            checkResults.GetDeclarationLocation(context)

        match result with
        | FindDeclResult.DeclFound range when File.Exists range.FileName ->
            let landedLines =
                File.ReadAllText(range.FileName).Replace("\r\n", "\n").Split('\n')

            let landedText =
                if range.StartLine >= 1 && range.StartLine <= landedLines.Length then
                    landedLines.[range.StartLine - 1]
                else
                    "<line out of range>"

            if not (landedText.Contains definitionLine) then
                failwithf "Goto-def landed on %s:%d (%s) but expected a line containing %A"
                    range.FileName range.StartLine landedText definitionLine
        | FindDeclResult.DeclFound _ -> ()
        | other ->
            failwithf "Expected FindDeclResult.DeclFound on a line containing %A but got %A" definitionLine other

    let assertNoDiagnostics (results: FSharpCheckFileResults) =
        match dumpDiagnostics results with
        | [] -> ()
        | msgs ->
            failwithf "Expected no diagnostics, but got %d:\n%s" msgs.Length (String.concat "\n" msgs)

    let assertDiagnosticCount (expected: int) (results: FSharpCheckFileResults) =
        let msgs = dumpDiagnostics results |> List.distinct
        if msgs.Length <> expected then
            failwithf "Expected %d distinct diagnostic(s), but got %d:\n%s" expected msgs.Length (String.concat "\n" msgs)

    let assertDiagnosticsContain (expected: string) (results: FSharpCheckFileResults) =
        let messages = results.Diagnostics |> Array.map normalizeDiagnosticMessage
        if not (messages |> Array.exists (fun m -> m.Contains expected)) then
            let dump = dumpDiagnostics results
            failwithf "Expected a diagnostic message containing %A, but got %d:\n%s"
                expected dump.Length (String.concat "\n" dump)

    let assertSingleDiagnosticContainingAll (parts: string list) (results: FSharpCheckFileResults) =
        let dump = dumpDiagnostics results |> List.distinct
        match dump with
        | [ single ] ->
            match parts |> List.filter (fun p -> not (single.Contains p)) with
            | [] -> ()
            | missing ->
                failwithf "Single diagnostic is missing expected part(s) %A:\n%s" missing single
        | _ ->
            failwithf "Expected exactly 1 distinct diagnostic, but got %d:\n%s"
                dump.Length (String.concat "\n" dump)

    let assertWarningCount (expected: int) (results: FSharpCheckFileResults) =
        let warnings = dumpDiagnosticsOfSeverity FSharpDiagnosticSeverity.Warning results |> List.distinct

        if warnings.Length <> expected then
            failwithf "Expected %d warning(s), but got %d:\n%s"
                expected warnings.Length (String.concat "\n" warnings)

    let checkAsFsFile (source: string) =
        let fileName, options = mkTestFileAndOptions [||]
        let _, checkResults = parseAndCheckFile fileName source options
        checkResults

    let getTooltipWithReferences (name: string) (references: string list) (markedSource: string) =
        let context = Checker.getResolveContext markedSource
        let fileName = name + ".fsx"

        let args =
            [| "--simpleresolution"
               "--noframework"
               "--debug:full"
               "--define:DEBUG"
               "--optimize-"
               "--out:" + name + ".dll"
               "--warn:3"
               "--fullpaths"
               "--flaterrors"
               "--target:library"
               yield! references |> List.map (fun r -> "-r:" + r) |]

        let options =
            { checker.GetProjectOptionsFromCommandLineArgs(name + ".fsproj", args) with
                SourceFiles = [| fileName |] }

        let _, checkResults = parseAndCheckFile fileName context.Source options
        checkResults.GetTooltip(context)

    let foldToolTip (ToolTipText items) =
        items
        |> List.collect (fun item ->
            match item with
            | ToolTipElement.Group elements ->
                elements
                |> List.collect (fun e ->
                    [ taggedTextToString e.MainDescription
                      match e.XmlDoc with
                      | FSharpXmlDoc.FromXmlText xmlDoc -> String.concat "\n" xmlDoc.UnprocessedLines
                      | _ -> ""
                      match e.Remarks with
                      | Some r -> taggedTextToString r
                      | None -> "" ])
            | ToolTipElement.CompositionError err -> [ err ]
            | ToolTipElement.None -> [])
        |> String.concat "\n"

    type TooltipSource =
        | Script
        | FsFile

    let foldedTooltip (mode: TooltipSource) (markedSource: string) : string =
        match mode with
        | Script -> foldToolTip (Checker.getTooltip markedSource)
        | FsFile ->
            let context = Checker.getResolveContext markedSource
            let checkResults = checkAsFsFile context.Source

            checkResults.GetTooltip(context)
            |> foldToolTip

    let private tooltipSourceLabel mode =
        match mode with
        | Script -> "tooltip"
        | FsFile -> ".fs-file tooltip"

    let assertFoldedTooltipContains (contains: bool) (label: string) (expected: string) (actual: string) =
        if actual.Contains expected <> contains then
            let relation = if contains then "to contain" else "NOT to contain"
            failwithf "Expected %s %s %A, but the actual tooltip was:\n%s" label relation expected actual

    let private assertTooltip (contains: bool) (mode: TooltipSource) (expected: string) (markedSource: string) =
        assertFoldedTooltipContains contains (tooltipSourceLabel mode) expected (foldedTooltip mode markedSource)

    let assertTooltipContains = assertTooltip true Script

    let walk (source: string) (initial: string) (ident: string) (expected: string) =
        let baseIndex = source.IndexOf(initial, StringComparison.Ordinal)

        for i in 0 .. ident.Length - 1 do
            let marked = source.Insert(baseIndex + initial.Length + i + 1, "{caret}")
            assertTooltipContains expected marked

    let assertTooltipDoesNotContain = assertTooltip false Script

    let assertIdentifierInTooltipExactlyOnce (ident: string) (markedSource: string) =
        let actual = foldToolTip (Checker.getTooltip markedSource)

        if not (actual.Contains ident) then
            failwithf "Expected tooltip to contain %A at least once (non-vacuity), but the actual tooltip was:\n%s" ident actual

        let count =
            actual.Split([| '='; '.'; ' '; '\t'; '('; ':'; ')'; '\n'; '\r' |])
            |> Array.filter ((=) ident)
            |> Array.length

        if count <> 1 then
            failwithf "Expected identifier %A to occur exactly once in the tooltip, but it occurred %d time(s):\n%s" ident count actual

    let assertStringContainsInOrder (parts: string list) (actual: string) =
        let mutable fromIndex = 0
        for part in parts do
            match actual.IndexOf(part, fromIndex, StringComparison.Ordinal) with
            | -1 ->
                failwithf "Expected tooltip to contain %A after index %d (in order), but the actual tooltip was:\n%s"
                    part fromIndex actual
            | index -> fromIndex <- index + part.Length

    let assertTooltipContainsInOrder (parts: string list) (markedSource: string) =
        let actual = foldToolTip (Checker.getTooltip markedSource)
        assertStringContainsInOrder parts actual

    let assertCompletionItemTooltipContainsInOrder (itemName: string) (parts: string list) (markedSource: string) =
        let item = findCompletionItem itemName (Checker.getCompletionInfo markedSource)
        assertStringContainsInOrder parts (foldToolTip item.Description)

    let assertTooltipContainsInFsFile = assertTooltip true FsFile

    let assertTooltipDoesNotContainInFsFile = assertTooltip false FsFile

    let fsTestLibCode = """namespace FSTestLib

                            /// DocComment: This is MyStruct type, represents a struct.
                            type MyPoint =
                                struct
                                    val mutable private m_X : float
                                    val mutable private m_Y : float
        
                                    new (x, y) = { m_X = x; m_Y = y }
        
                                    /// Gets and sets X
                                    member this.X with get () = this.m_X and set x = this.m_X <- x
        
                                    /// Gets and sets Y
                                    member this.Y with get () = this.m_Y and set y = this.m_Y <- y
        
                                    // Length of given Point
                                    member this.Len = sqrt ( this.X * this.X + this.Y * this.Y )
        
                                    static member (+) (p1 : MyPoint, p2 : MyPoint) = MyPoint(p1.X + p2.X, p1.Y + p2.Y)
        
                                end

                            [<NoComparison;NoEquality>]
                            /// DocComment: This is my record type.
                            type MyEmployee = 
                                { mutable Name  : string;
                                  mutable Age   : int;
                                  /// DocComment: Indicates whether the employee is full time or not
                                  mutable IsFTE : bool }
    
                                interface System.IComparable with
                                    member this.CompareTo (emp : obj) = 
                                        let r = emp :?> MyEmployee
                                        match r.IsFTE && this.IsFTE with
                                        | true -> this.Age - r.Age
                                        | _ -> System.Convert.ToInt32(this.IsFTE) - System.Convert.ToInt32(r.IsFTE)
    
                                override this.ToString() = sprintf "%s is %d." this.Name this.Age
    
                                /// DocComment: Method
                                static member MakeDummy () =
                                    { Name = System.String.Empty; Age = -1; IsFTE = false }
    
                                // TODO: Normally there's no DotCompletion after "this" here
                                override this.Equals(ob : obj) = 
                                    let r = ob :?> MyEmployee
                                    this.Name = r.Name && this.Age = r.Age && this.IsFTE = r.IsFTE

                            /// DocComment: This is my interface type
                            type IMyInterface = 
                                interface
                                    /// DocComment: abstract method in Interface
                                    abstract Represent : unit -> string        
                                end

                            // TODO: add formatable ToString()
                            /// DocComment: This is my discriminated union type
                            type MyDistance =
                                | Kilometers of float
                                | Miles of float
                                | NauticalMiles of float
    
    
                                /// DocComment: Static Method
                                static member toMiles x =
                                    Miles(
                                        match x with
                                        | Miles x -> x
                                        | Kilometers x -> x / 1.6
                                        | NauticalMiles x -> x * 1.15
                                    )

                                /// DocComment: Property        
                                member this.toNautical =
                                    NauticalMiles(
                                        match this with
                                        | Kilometers x -> x / 1.852
                                        | Miles x -> x / 1.15
                                        | NauticalMiles x -> x
                                    )

                                /// DocComment: Method        
                                member this.IncreaseBy dist = 
                                    match this with
                                    | Kilometers x -> Kilometers (x + dist)
                                    | Miles x -> Miles (x + dist)
                                    | NauticalMiles x -> NauticalMiles (x + dist)
        
                                /// DocComment: Event
                                static member Event = 
                                    let evnt = new Event<string>()
                                    evnt
        
                            /// DocComment: This is my enum type
                            type MyColors =
                                | /// DocComment: Field
                                  Red = 0
                                | Green = 1
                                | Blue = 2
    
                            /// DocComment: This is my class type
                            type MyCar( number: int, color:MyColors) =
                                /// DocComment: This is static field
                                static member Owner     = "MySelf"
                                /// DocComment: This is instance field
                                member this.Number      = number
                                member this.Color       = color
                                /// DocComment: This is static method
                                static member Run (number:int)      = printf "%s" (number.ToString()+"Running")
                                /// DocComment: This is instance method
                                member this.Repair  (expense:int)  = printf "%s" ("Spent " + expense.ToString() + " for repairing. ")
    
                            /// DocComment: This is my delegate type
                            type ControlEventHandler = delegate of int -> unit"""

    let foldedProjectTooltip (priorFiles: string list) (extraRefs: string list) (markedSource: string) =
        let context = Checker.getResolveContext markedSource
        let options = createProjectOptions (priorFiles @ [ context.Source ]) [ for r in extraRefs -> "-r:" + r ]
        let queriedPath = Array.last options.SourceFiles
        let _, checkResults = parseAndCheckFile queriedPath context.Source options
        checkResults.GetTooltip(context) |> foldToolTip

    let assertTooltipContainsWithFsTestLib (expected: string) (markedFile2: string) =
        foldedProjectTooltip [ fsTestLibCode ] [] markedFile2
        |> assertFoldedTooltipContains true "FSTestLib two-file tooltip" expected

    let assertCompleteIdentifierIslandWithTolerate (tolerate: bool) (expected: string option) (sourceWithCaretMarker: string) =
        let n = sourceWithCaretMarker.IndexOf '$'
        if n < 0 then failwith "source must contain the '$' caret marker"
        let line = sourceWithCaretMarker.Remove(n, 1)

        match QuickParse.GetCompleteIdentifierIsland tolerate line n, expected with
        | Some(island, _, _), Some exp ->
            if island <> exp then
                failwithf "tolerate=%b: GetCompleteIdentifierIsland returned island %A but expected %A (line=%A col=%d)" tolerate island exp line n
        | None, None -> ()
        | Some(island, _, _), None ->
            failwithf "tolerate=%b: expected NO island but got %A (line=%A col=%d)" tolerate island line n
        | None, Some exp ->
            failwithf "tolerate=%b: expected island %A but got None (line=%A col=%d)" tolerate exp line n

    let assertCompleteIdentifierIsland (expected: string option) (sourceWithCaretMarker: string) =
        assertCompleteIdentifierIslandWithTolerate true expected sourceWithCaretMarker
        assertCompleteIdentifierIslandWithTolerate false expected sourceWithCaretMarker

    let private getMethodGroup (markedSource: string) =
        let context, checkResults = Checker.getCheckedResolveContext markedSource
        checkResults.GetMethods(context.Pos.Line, context.Pos.Column, context.LineText, Some context.Names)

    let private paramDisplays (m: MethodGroupItem) =
        m.Parameters |> Array.map (fun p -> taggedTextToString p.Display) |> Array.toList

    let private describeMethodGroup (mg: MethodGroup) =
        if mg.Methods.Length = 0 then
            "  <no overloads>"
        else
            mg.Methods
            |> Array.mapi (fun i m -> sprintf "  [%d] %s" i (String.concat ", " (paramDisplays m)))
            |> String.concat "\n"

    let private displaysMatch (expected: string list) (displays: string list) =
        expected.Length = displays.Length
        && List.forall2 (fun (e: string) (d: string) -> d.Contains e) expected displays

    let assertParameterInfoOverloads (expected: string list list) (markedSource: string) =
        let mg = getMethodGroup markedSource
        if mg.Methods.Length <> expected.Length then
            failwithf "Expected %d overload(s) but got %d:\n%s" expected.Length mg.Methods.Length (describeMethodGroup mg)
        for m in mg.Methods do
            let displays = paramDisplays m
            let matched = expected |> List.exists (fun exp -> displaysMatch exp displays)
            if not matched then
                failwithf "Overload [%s] matched no expected set %A:\n%s" (String.concat ", " displays) expected (describeMethodGroup mg)

    let assertNoParameterInfo (markedSource: string) =
        let mg = getMethodGroup markedSource
        if mg.Methods.Length <> 0 then
            failwithf "Expected no parameter info but got %d overload(s):\n%s" mg.Methods.Length (describeMethodGroup mg)

    let assertParameterInfoContains (expected: string list) (markedSource: string) =
        let mg = getMethodGroup markedSource
        let matched =
            mg.Methods
            |> Array.exists (fun m -> displaysMatch expected (paramDisplays m))
        if not matched then
            failwithf "No overload matched expected %A:\n%s" expected (describeMethodGroup mg)

    let assertParameterInfoOverloadIndex (idx: int) (expected: string list) (markedSource: string) =
        let mg = getMethodGroup markedSource
        if idx < 0 || idx >= mg.Methods.Length then
            failwithf "No overload at index %d (have %d):\n%s" idx mg.Methods.Length (describeMethodGroup mg)
        let displays = paramDisplays mg.Methods[idx]
        if not (displaysMatch expected displays) then
            failwithf "Overload [%d] = [%s] did not match expected %A:\n%s" idx (String.concat ", " displays) expected (describeMethodGroup mg)

    let assertHasParameterInfo (markedSource: string) =
        let mg = getMethodGroup markedSource
        if mg.Methods.Length = 0 then
            failwith "Expected a method group with parameter info, but got none"

    let assertFirstReturnTypeText (expected: string) (markedSource: string) =
        let mg = getMethodGroup markedSource
        if mg.Methods.Length = 0 then
            failwithf "Expected a method group, but got none. Looking for return type %A" expected
        let actual = taggedTextToString mg.Methods[0].ReturnTypeText
        if actual <> expected then
            failwithf "Expected first overload return type %A but got %A:\n%s" expected actual (describeMethodGroup mg)
