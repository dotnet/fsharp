// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System

open FSharp.Compiler.Syntax

/// A compiler-synthesized closure class such as `outer@47-1` names the binding it was lifted out of
/// and the line that binding was written on.
[<Struct>]
type internal ClosureOrigin =
    {
        EnclosingName: string
        Line: int
        Ordinal: int voption
    }

[<Struct>]
type internal FramePathSegment = { Name: string; GenericArity: int }

type internal FrameMember =
    | FrameMethod of name: string
    | FrameConstructor
    | FrameStaticConstructor
    | FramePropertyGetter of getter: string
    | FramePropertySetter of setter: string
    | FrameActivePattern of cases: string list
    | FrameClosureBody of origin: ClosureOrigin
    | FrameStartupCode

type internal ParsedFrame =
    {
        /// Declaring path, outermost first. Namespace, module and nested type segments are
        /// indistinguishable in a frame name, so consumers must try several splits.
        Path: FramePathSegment array
        Member: FrameMember
        MethodGenericArity: int
        /// The file and line the debugger resolved for this frame, which the provider fills in - the
        /// name alone locates nothing for module initialization (`$Demo.$Demo` names the file) and
        /// lies for a state machine's closure, which the compiler numbers from line 1.
        SourcePosition: struct (string * int) voption
    }

/// Turns the name a debug engine reports for a stack frame back into the F# construct it came from.
[<RequireQualifiedAccess>]
module internal FSharpStackFrameNameParser =

    let private startupCodePrefix = "<StartupCode$"

    [<Literal>]
    let private ConstructorSuffix = "..ctor"

    [<Literal>]
    let private StaticConstructorSuffix = "..cctor"

    let private dropParameters (name: string) =
        match name.IndexOf('(') with
        | -1 -> name
        | i -> name.Substring(0, i)

    let private dropGenericArguments (name: string) =
        match name.IndexOf('[') with
        | -1 -> name, 0
        | i ->
            let arguments = name.Substring(i + 1).TrimEnd(']')

            let arity =
                if String.IsNullOrEmpty arguments then
                    0
                else
                    arguments.Split(',').Length

            name.Substring(0, i), arity

    /// Splits on `.` and `+`, but not inside `<...>` - the debug engine prints instantiations like
    /// `Box<System.String>.Unwrap`, where the inner dot is part of an argument, not the path.
    let private splitPath (name: string) =
        let segments = ResizeArray()
        let mutable start = 0
        let mutable depth = 0

        for i in 0 .. name.Length - 1 do
            match name.[i] with
            | '<' -> depth <- depth + 1
            | '>' -> depth <- depth - 1
            | '.'
            | '+' when depth = 0 ->
                if i > start then
                    segments.Add(name.Substring(start, i - start))

                start <- i + 1
            | _ -> ()

        if start < name.Length then
            segments.Add(name.Substring start)

        segments.ToArray()

    /// Takes `Box<int>` or `Box<Ns.Item, int>` apart; the argument count doubles as the arity
    /// because the engine prints instantiations, never open definitions.
    let private dropAngleArguments (segment: string) =
        let close = segment.Length - 1

        if close < 1 || segment.[close] <> '>' then
            segment, 0
        else
            let mutable depth = 0
            let mutable openIndex = -1
            let mutable arguments = 1
            let mutable i = close

            while openIndex < 0 && i >= 0 do
                match segment.[i] with
                | '>' -> depth <- depth + 1
                | '<' ->
                    depth <- depth - 1

                    if depth = 0 then
                        openIndex <- i
                | ',' when depth = 1 -> arguments <- arguments + 1
                | _ -> ()

                i <- i - 1

            if openIndex <= 0 then
                segment, 0
            else
                segment.Substring(0, openIndex), arguments

    let private parseSegment (segment: string) =
        let segment, angleArity = dropAngleArguments segment

        match segment.IndexOf('`') with
        | -1 ->
            {
                Name = segment
                GenericArity = angleArity
            }
        | i ->
            let arity =
                match Int32.TryParse(segment.Substring(i + 1)) with
                | true, n -> n
                | _ -> angleArity

            {
                Name = segment.Substring(0, i)
                GenericArity = arity
            }

    /// Reads `digits` spanning [start, stop), rejecting an empty or non-numeric run.
    let private digitsIn (segment: string) start stop =
        if start >= stop then
            ValueNone
        else

            let mutable value = 0
            let mutable i = start

            while i < stop && Char.IsDigit segment.[i] do
                value <- value * 10 + int segment.[i] - int '0'
                i <- i + 1

            if i = stop then ValueSome value else ValueNone

    /// Matches `enclosingName@line` and `enclosingName@line-ordinal`, the shapes the compiler gives
    /// the classes it lifts closures, local functions and computation-expression bodies into.
    /// A generic closure class carries a trailing `T` (`helperTwo@42T`), and the engine may print
    /// its instantiation (`helperTwo@42T<int>`).
    let private parseClosureOrigin (segment: string) =
        let segment, _ = dropAngleArguments segment

        let segment =
            if segment.EndsWith("T", StringComparison.Ordinal) then
                segment.Substring(0, segment.Length - 1)
            else
                segment

        let at = segment.LastIndexOf '@'

        if at <= 0 || at = segment.Length - 1 then
            ValueNone
        else

            let origin line ordinal =
                ValueSome
                    {
                        EnclosingName = segment.Substring(0, at)
                        Line = line
                        Ordinal = ordinal
                    }

            match segment.IndexOf('-', at + 1) with
            | -1 ->
                match digitsIn segment (at + 1) segment.Length with
                | ValueSome line -> origin line ValueNone
                | ValueNone -> ValueNone
            | dash ->
                match digitsIn segment (at + 1) dash, digitsIn segment (dash + 1) segment.Length with
                | ValueSome line, ValueSome ordinal -> origin line (ValueSome ordinal)
                | _ -> ValueNone

    let private activePatternCases (name: string) =
        name.Trim('|').Split('|')
        |> Seq.filter (fun case -> not (String.IsNullOrEmpty case))
        |> Seq.toList

    let private classifyMember (segment: string) =
        if PrettyNaming.IsActivePatternName segment then
            FrameActivePattern(activePatternCases segment)
        elif segment.StartsWith("get_", StringComparison.Ordinal) then
            FramePropertyGetter(segment.Substring 4)
        elif segment.StartsWith("set_", StringComparison.Ordinal) then
            FramePropertySetter(segment.Substring 4)
        else
            FrameMethod segment

    /// `name` is the raw frame name, e.g. `GateC.Library.outer@47-1.Invoke` or `Ns.Type..ctor`.
    let parse (frameName: string) : ParsedFrame voption =
        if String.IsNullOrWhiteSpace frameName then
            ValueNone
        else

            let name = (dropParameters frameName).Trim()
            let name, methodGenericArity = dropGenericArguments name

            let name, constructorMember =
                if name.EndsWith(StaticConstructorSuffix, StringComparison.Ordinal) then
                    name.Substring(0, name.Length - StaticConstructorSuffix.Length), ValueSome FrameStaticConstructor
                elif name.EndsWith(ConstructorSuffix, StringComparison.Ordinal) then
                    name.Substring(0, name.Length - ConstructorSuffix.Length), ValueSome FrameConstructor
                else
                    name, ValueNone

            let segments = splitPath name

            match segments with
            | [||] -> ValueNone
            | segments ->
                // Flexible so the branches below can pass either the segment array or a filtered seq.
                let frameWithArity (path: #seq<string>) frameMember memberArity =
                    ValueSome
                        {
                            Path = path |> Seq.map parseSegment |> Seq.toArray
                            Member = frameMember
                            MethodGenericArity = memberArity
                            SourcePosition = ValueNone
                        }

                let frame path frameMember =
                    frameWithArity path frameMember methodGenericArity

                let isStartupCode =
                    segments
                    |> Array.exists (fun segment -> segment.StartsWith(startupCodePrefix, StringComparison.Ordinal))

                let closureIndex =
                    segments
                    |> Array.tryFindIndexBackV (fun segment -> (parseClosureOrigin segment).IsSome)

                match constructorMember, isStartupCode, closureIndex with
                | ValueSome constructorMember, _, _ -> frame segments constructorMember
                | ValueNone, true, _ ->
                    let declaringModule =
                        segments
                        |> Seq.filter (fun segment -> not (segment.StartsWith(startupCodePrefix, StringComparison.Ordinal)))
                        |> Seq.map _.TrimStart('$')
                        |> Seq.filter (fun segment ->
                            not (String.IsNullOrEmpty segment)
                            && not (segment.EndsWith("@", StringComparison.Ordinal)))

                    frame declaringModule FrameStartupCode
                | ValueNone, false, ValueSome i ->
                    let origin = (parseClosureOrigin segments.[i]).Value
                    frame (Seq.truncate i segments) (FrameClosureBody origin)
                | ValueNone, false, ValueNone ->
                    let lastName, lastArity = dropAngleArguments (Array.last segments)
                    let memberArity = max methodGenericArity lastArity
                    let path = Seq.truncate (segments.Length - 1) segments

                    let previousName =
                        if segments.Length >= 2 then
                            (dropAngleArguments segments.[segments.Length - 2] |> fst)
                        else
                            ""

                    // The debug engine prints a constructor as `Type.Type` and an accessor as a
                    // trailing `.get`/`.set` segment, unlike the `..ctor`/`get_` metadata shapes.
                    if String.Equals(lastName, previousName, StringComparison.Ordinal) then
                        frameWithArity path FrameConstructor memberArity
                    elif lastName = "get" && segments.Length >= 2 then
                        frameWithArity (Seq.truncate (segments.Length - 2) segments) (FramePropertyGetter previousName) memberArity
                    elif lastName = "set" && segments.Length >= 2 then
                        frameWithArity (Seq.truncate (segments.Length - 2) segments) (FramePropertySetter previousName) memberArity
                    else
                        frameWithArity path (classifyMember lastName) memberArity
