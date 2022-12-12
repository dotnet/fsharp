// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.Interactive

open System
open System.Text
open System.Collections.Generic
open FSharp.Compiler.DiagnosticsLogger

type internal Style =
    | Prompt
    | Out
    | Error

/// Class managing the command History.
type internal History() =
    let list = new List<string>()
    let mutable current = 0

    member _.Count = list.Count

    member _.Current =
        if current >= 0 && current < list.Count then
            list.[current]
        else
            String.Empty

    member _.Clear() =
        list.Clear()
        current <- -1

    member _.Add line =
        match line with
        | null
        | "" -> ()
        | _ -> list.Add(line)

    member _.AddLast line =
        match line with
        | null
        | "" -> ()
        | _ ->
            list.Add(line)
            current <- list.Count

    // Dead code
    // member x.First() = current <- 0; x.Current
    // member x.Last() = current <- list.Count - 1; x.Current

    member x.Previous() =
        if (list.Count > 0) then
            current <- ((current - 1) + list.Count) % list.Count

        x.Current

    member x.Next() =
        if (list.Count > 0) then
            current <- (current + 1) % list.Count

        x.Current

/// List of available optionsCache
type internal Options() =
    inherit History()

    let mutable root = ""

    member _.Root
        with get () = root
        and set (v) = (root <- v)

/// Cursor position management

module internal Utils =

    let guard (f) =
        try
            f ()
        with e ->
            warning (
                Failure(
                    sprintf
                        "Note: an unexpected exception in fsi.exe readline console support. Consider starting fsi.exe with the --no-readline option and report the stack trace below to the .NET or Mono implementors\n%s\n%s\n"
                        e.Message
                        e.StackTrace
                )
            )

    let rec previousWordFromIdx (line: string) (idx, isInWord) =
        if idx < 0 then
            0
        else
            match line.Chars(idx), isInWord with
            | ' ', true -> idx + 1
            | ' ', false -> previousWordFromIdx line (idx - 1, false)
            | _, _ -> previousWordFromIdx line (idx - 1, true)

    let rec nextWordFromIdx (line: string) (idx, isInWord) =
        if idx >= line.Length then
            line.Length - 1
        else
            match line.Chars(idx), isInWord with
            | ' ', true -> idx
            | ' ', false -> nextWordFromIdx line (idx + 1, false)
            | _, _ -> nextWordFromIdx line (idx + 1, true)

    /// An array stores ranges of full-width chars.
    ///
    /// The ranges are sorted by increasing order in the array, and each range are stored in the 2nth and 2n+1th
    /// position in the array (n is the ordinal number of the range)
    ///
    /// Array [| a; b; c; d |] represents range [a, b] or [c, d], means chars in these ranges are full-width.
    ///
    /// Definition: https://www.unicode.org/reports/tr11/
    ///
    /// Data source: https://www.unicode.org/Public/UCD/latest/ucd/EastAsianWidth.txt
    let private fullWidthCharRanges =
        Array.concat
            [|
                [| '\u1100'; '\u115f' |]
                [| '\u231a'; '\u231b' |]
                [| '\u2329'; '\u232a' |]
                [| '\u23e9'; '\u23ec' |]
                [| '\u23f0'; '\u23f0' |]
                [| '\u23f3'; '\u23f3' |]
                [| '\u25fd'; '\u25fe' |]
                [| '\u2614'; '\u2615' |]
                [| '\u2648'; '\u2653' |]
                [| '\u267f'; '\u267f' |]
                [| '\u2693'; '\u2693' |]
                [| '\u26a1'; '\u26a1' |]
                [| '\u26aa'; '\u26ab' |]
                [| '\u26bd'; '\u26be' |]
                [| '\u26c4'; '\u26c5' |]
                [| '\u26ce'; '\u26ce' |]
                [| '\u26d4'; '\u26d4' |]
                [| '\u26ea'; '\u26ea' |]
                [| '\u26f2'; '\u26f3' |]
                [| '\u26f5'; '\u26f5' |]
                [| '\u26fa'; '\u26fa' |]
                [| '\u26fd'; '\u26fd' |]
                [| '\u2705'; '\u2705' |]
                [| '\u270a'; '\u270b' |]
                [| '\u2728'; '\u2728' |]
                [| '\u274c'; '\u274c' |]
                [| '\u274e'; '\u274e' |]
                [| '\u2753'; '\u2755' |]
                [| '\u2757'; '\u2757' |]
                [| '\u2795'; '\u2797' |]
                [| '\u27b0'; '\u27b0' |]
                [| '\u27bf'; '\u27bf' |]
                [| '\u2b1b'; '\u2b1c' |]
                [| '\u2b50'; '\u2b50' |]
                [| '\u2b55'; '\u2b55' |]
                [| '\u2e80'; '\u303e' |]
                [| '\u3041'; '\u3096' |]
                [| '\u3099'; '\u30ff' |]
                [| '\u3105'; '\u312f' |]
                [| '\u3131'; '\u318e' |]
                [| '\u3190'; '\u3247' |]
                [| '\u3250'; '\u4dbf' |]
                [| '\u4e00'; '\ua4c6' |]
                [| '\ua960'; '\ua97c' |]
                [| '\uac00'; '\ud7a3' |]
                [| '\uf900'; '\ufaff' |]
                [| '\ufe10'; '\ufe1f' |]
                [| '\ufe30'; '\ufe6b' |]
                [| '\uff01'; '\uff60' |]
                [| '\uffe0'; '\uffe6' |]
            |]

    let isFullWidth (char) =
        // for array [| a; b; c; d |],
        // if a value is in (a, b) or (c, d), the result of Array.BinarySearch will be a negative even number
        // if a value is a, b, c, d, the result will be a positive number
        let n = Array.BinarySearch(fullWidthCharRanges, char)
        n >= 0 || n % 2 = 0

    /// Limits BufferWidth to make sure that full-width characters will not be print to wrong position.
    ///
    /// The return value is Console.BufferWidth - 2.
    ///
    /// When printing full-width characters to the screen (such as 一二三四五六七八九零),
    ///
    /// if BufferWidth = Console.BufferWidth, the output will be
    ///
    /// #> 一二三四五六七八九零一二三四五六七八九  # (零 is missing)
    ///
    /// #一二三四五六七八九零                      #
    ///
    /// if BufferWidth = Console.BufferWidth - 1, the output will be
    ///
    /// #> 一二三四五六七八九零一二三四五六七八九零# (零 is printed, but will not correctly cauculate cursor position)
    ///
    /// #一二三四五六七八九零                      # (cursor may appear in the middle of the character)
    ///
    /// if BufferWidth = Console.BufferWidth - 2, the output will be
    ///
    /// #> 一二三四五六七八九零一二三四五六七八九  #
    ///
    /// #零一二三四五六七八九零                    # (work correctly)
    let bufferWidth () = Console.BufferWidth - 2

[<Sealed>]
type internal Cursor =
    static member ResetTo(top, left) =
        Utils.guard (fun () ->
            Console.CursorTop <- min top (Console.BufferHeight - 1)
            Console.CursorLeft <- left)

    static member Move(delta) =
        let width = Utils.bufferWidth ()
        let position = Console.CursorTop * width + Console.CursorLeft + delta

        let top = position / width
        let left = position % width
        Cursor.ResetTo(top, left)

type internal Anchor =
    {
        top: int
        left: int
    }

    static member Current(inset) =
        {
            top = Console.CursorTop
            left = max inset Console.CursorLeft
        }

    static member Top(inset) = { top = 0; left = inset }

    member p.PlaceAt(inset, index) =
        //printf "p.top = %d, p.left = %d, inset = %d, index = %d\n" p.top p.left inset index
        let width = Utils.bufferWidth ()
        let index = inset + index

        let left = index % width
        let top = p.top + index / width
        Cursor.ResetTo(top, left)

type internal ReadLineConsole() =
    let history = new History()

    let mutable complete: (string option * string -> seq<string>) =
        fun (_s1, _s2) -> Seq.empty

    member _.SetCompletionFunction f = complete <- f

    /// Inset all inputs by this amount
    member _.Prompt = "> "

    member _.Prompt2 = "- "

    member x.Inset = x.Prompt.Length

    member _.GetOptions(input: string) =
        /// Tab optionsCache available in current context
        let optionsCache = new Options()

        let rec look parenCount i =
            if i <= 0 then
                i
            else
                match input.Chars(i - 1) with
                | c when Char.IsLetterOrDigit(c) (* or Char.IsWhiteSpace(c) *)  -> look parenCount (i - 1)
                | '.'
                | '_' -> look parenCount (i - 1)
                | '}'
                | ')'
                | ']' -> look (parenCount + 1) (i - 1)
                | '('
                | '{'
                | '[' -> look (parenCount - 1) (i - 1)
                | _ when parenCount > 0 -> look parenCount (i - 1)
                | _ -> i

        let start = look 0 input.Length

        let name = input.Substring(start, input.Length - start)

        if name.Trim().Length > 0 then
            let lastDot = name.LastIndexOf('.')

            let attr, pref, root =
                if (lastDot < 0) then
                    None, name, input.Substring(0, start)
                else
                    Some(name.Substring(0, lastDot)), name.Substring(lastDot + 1), input.Substring(0, start + lastDot + 1)

            try
                complete (attr, pref)
                |> Seq.filter (fun option -> option.StartsWith(pref, StringComparison.Ordinal))
                |> Seq.iter (fun option -> optionsCache.Add(option))

                optionsCache.Root <- root
            with _ ->
                optionsCache.Clear()

            optionsCache, true
        else
            optionsCache, false

    member _.MapCharacter(c) : string =
        match c with
        | '\x1A' -> "^Z"
        | _ -> "^?"

    member x.GetCharacterSize(c) =
        if Char.IsControl(c) then x.MapCharacter(c).Length
        elif Utils.isFullWidth c then 2
        else 1

    static member TabSize = 4

    member x.ReadLine() =

        let checkLeftEdge prompt =
            let currLeft = Console.CursorLeft

            if currLeft < x.Inset then
                if currLeft = 0 then
                    Console.Write(if prompt then x.Prompt2 else String(' ', x.Inset))

                Utils.guard (fun () ->
                    Console.CursorTop <- min Console.CursorTop (Console.BufferHeight - 1)
                    Console.CursorLeft <- x.Inset)

        // The caller writes the primary prompt.  If we are reading the 2nd and subsequent lines of the
        // input we're responsible for writing the secondary prompt.
        checkLeftEdge true

        /// Cursor anchor - position of !anchor when the routine was called
        let mutable anchor = Anchor.Current x.Inset

        /// Length of the output currently rendered on screen.
        let mutable rendered = 0

        /// Input has changed, therefore options cache is invalidated.
        let mutable changed = false

        /// Cache of optionsCache
        let mutable optionsCache = Options()

        let moveCursorToNextLine c =
            let charSize = x.GetCharacterSize(c)

            if Console.CursorLeft + charSize > Utils.bufferWidth () then
                if Console.CursorTop + 1 = Console.BufferHeight then
                    Console.BufferHeight <- Console.BufferHeight + 1

                Cursor.Move(0)

        let writeBlank () =
            moveCursorToNextLine (' ')
            Console.Write(' ')

        let writeChar (c) =
            moveCursorToNextLine (c)

            if Char.IsControl(c) then
                let s = x.MapCharacter c
                Console.Write(s)
                rendered <- rendered + s.Length
            else
                Console.Write(c)
                rendered <- rendered + x.GetCharacterSize(c)

        /// The console input buffer.
        let input = new StringBuilder()

        /// Current position - index into the input buffer
        let mutable current = 0

        let render () =
            let curr = current
            anchor.PlaceAt(x.Inset, 0)

            let rec getLineWidth state i =
                if i = curr || i = input.Length then
                    state
                else
                    getLineWidth (state + x.GetCharacterSize(input.Chars i)) (i + 1)

            let position = getLineWidth 0 0

            // render the current text, computing a new value for "rendered"
            let old_rendered = rendered
            rendered <- 0

            for i = 0 to input.Length - 1 do
                writeChar (input.Chars(i))

            // blank out any dangling old text
            for i = rendered to old_rendered - 1 do
                writeBlank ()

            anchor.PlaceAt(x.Inset, position)

        render ()

        let insertChar (c: char) =
            if current = input.Length then
                current <- current + 1
                input.Append(c) |> ignore
                writeChar (c)
            else
                input.Insert(current, c) |> ignore
                current <- current + 1
                render ()

        let insertTab () =
            for i = ReadLineConsole.TabSize - (current % ReadLineConsole.TabSize) downto 1 do
                insertChar (' ')

        let moveLeft () =
            if current > 0 && (current - 1 < input.Length) then
                current <- current - 1
                let c = input.Chars(current)
                Cursor.Move(-x.GetCharacterSize c)

        let moveRight () =
            if current < input.Length then
                let c = input.Chars(current)
                current <- current + 1
                Cursor.Move(x.GetCharacterSize c)

        let moveWordLeft () =
            if current > 0 && (current - 1 < input.Length) then
                let line = input.ToString()
                current <- Utils.previousWordFromIdx line (current - 1, false)
                anchor.PlaceAt(x.Inset, current)

        let moveWordRight () =
            if current < input.Length then
                let line = input.ToString()
                let idxToMoveTo = Utils.nextWordFromIdx line (current + 1, false)

                // if has reached end of the last word
                if idxToMoveTo = current && current < line.Length then
                    current <- line.Length
                else
                    current <- idxToMoveTo

                anchor.PlaceAt(x.Inset, current)

        let setInput (line: string) =
            input.Length <- 0
            input.Append(line) |> ignore
            current <- input.Length
            render ()

        let tabPress (shift) =
            let opts, prefix =
                if changed then
                    changed <- false
                    x.GetOptions(input.ToString())
                else
                    optionsCache, false

            optionsCache <- opts

            if (opts.Count > 0) then
                let part = if shift then opts.Previous() else opts.Next()

                setInput (opts.Root + part)
            else if (prefix) then
                Console.Beep()
            else
                insertTab ()

        let delete () =
            if (input.Length > 0 && current < input.Length) then
                input.Remove(current, 1) |> ignore
                render ()

        let deleteFromStartOfLineToCursor () =
            if (input.Length > 0 && current > 0) then
                input.Remove(0, current) |> ignore
                current <- 0
                render ()

        let deleteWordLeadingToCursor () =
            if (input.Length > 0 && current > 0) then
                let line = input.ToString()
                let idx = Utils.previousWordFromIdx line (current - 1, false)
                input.Remove(idx, current - idx) |> ignore
                current <- idx
                render ()

        let deleteToEndOfLine () =
            if (current < input.Length) then
                input.Remove(current, input.Length - current) |> ignore
                render ()

        let insert (key: ConsoleKeyInfo) =
            // REVIEW: is this F6 rewrite required? 0x1A looks like Ctrl-Z.
            // REVIEW: the Ctrl-Z code is not recognised as EOF by the lexer.
            // REVIEW: looks like a relic of the port of readline, which is currently removable.
            let c = if (key.Key = ConsoleKey.F6) then '\x1A' else key.KeyChar

            insertChar (c)

        let backspace () =
            if (input.Length > 0 && current > 0) then
                input.Remove(current - 1, 1) |> ignore
                current <- current - 1
                render ()

        let enter () =
            Console.Write("\n")
            let line = input.ToString()

            if (line = "\x1A") then
                null
            else
                if (line.Length > 0) then
                    history.AddLast(line)

                line

        let clear () =
            current <- input.Length

            let setPrompt prompt =
                if prompt then // We only allow clearing if prompt is ">"
                    Console.Clear()
                    Console.Write(x.Prompt)
                    Console.Write(input.ToString())
                    anchor <- Anchor.Top(x.Inset)

            let previous = history.Previous()
            history.Next() |> ignore

            if previous = "" then
                setPrompt true
            else
                setPrompt (previous.EndsWith(";;"))

        let home () =
            current <- 0
            anchor.PlaceAt(x.Inset, 0)

        let rec read () =
            let key = Console.ReadKey true

            match key.Key with
            | ConsoleKey.Backspace ->
                backspace ()
                change ()
            | ConsoleKey.Delete ->
                delete ()
                change ()
            | ConsoleKey.Enter -> enter ()
            | ConsoleKey.Tab ->
                tabPress (key.Modifiers &&& ConsoleModifiers.Shift <> enum 0)
                read ()
            | ConsoleKey.UpArrow ->
                setInput (history.Previous())
                change ()
            | ConsoleKey.DownArrow ->
                setInput (history.Next())
                change ()
            | ConsoleKey.RightArrow when key.Modifiers &&& ConsoleModifiers.Control = enum 0 ->
                moveRight ()
                change ()
            | ConsoleKey.LeftArrow when key.Modifiers &&& ConsoleModifiers.Control = enum 0 ->
                moveLeft ()
                change ()
            | ConsoleKey.Escape ->
                setInput String.Empty
                change ()
            | ConsoleKey.Home ->
                home ()
                change ()
            | ConsoleKey.End ->
                current <- input.Length
                anchor.PlaceAt(x.Inset, rendered)
                change ()
            | _ ->

                match key.Modifiers, key.Key with
                | ConsoleModifiers.Control, ConsoleKey.A ->
                    home ()
                    change ()
                | ConsoleModifiers.Control, ConsoleKey.E ->
                    current <- input.Length
                    anchor.PlaceAt(x.Inset, rendered)
                    change ()
                | ConsoleModifiers.Control, ConsoleKey.B ->
                    moveLeft ()
                    change ()
                | ConsoleModifiers.Control, ConsoleKey.F ->
                    moveRight ()
                    change ()
                | ConsoleModifiers.Control, ConsoleKey.LeftArrow
                | ConsoleModifiers.Alt, ConsoleKey.B ->
                    moveWordLeft ()
                    change ()
                | ConsoleModifiers.Control, ConsoleKey.RightArrow
                | ConsoleModifiers.Alt, ConsoleKey.F ->
                    moveWordRight ()
                    change ()
                | ConsoleModifiers.Control, ConsoleKey.K ->
                    deleteToEndOfLine ()
                    change ()
                | ConsoleModifiers.Control, ConsoleKey.P ->
                    setInput (history.Previous())
                    change ()
                | ConsoleModifiers.Control, ConsoleKey.N ->
                    setInput (history.Next())
                    change ()
                | ConsoleModifiers.Control, ConsoleKey.D ->
                    if (input.Length = 0) then
                        exit 0 //quit
                    else
                        delete ()
                        change ()
                | ConsoleModifiers.Control, ConsoleKey.L ->
                    clear ()
                    change ()
                | ConsoleModifiers.Control, ConsoleKey.U ->
                    deleteFromStartOfLineToCursor ()
                    change ()
                | ConsoleModifiers.Control, ConsoleKey.W ->
                    deleteWordLeadingToCursor ()
                    change ()
                | _ ->
                    // Note: If KeyChar=0, the not a proper char, e.g. it could be part of a multi key-press character,
                    //       e.g. e-acute is ' and e with the French (Belgium) IME and US Intl KB.
                    // Here: skip KeyChar=0 (except for F6 which maps to 0x1A (ctrl-Z?)).
                    if key.KeyChar <> '\000' || key.Key = ConsoleKey.F6 then
                        insert (key)
                        change ()
                    else
                        // Skip and read again.
                        read ()

        and change () =
            changed <- true
            read ()

        read ()
