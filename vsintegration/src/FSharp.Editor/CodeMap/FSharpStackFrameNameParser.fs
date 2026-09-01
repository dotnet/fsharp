// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Text.RegularExpressions

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
        Path: FramePathSegment list
        Member: FrameMember
        MethodGenericArity: int
    }

/// Turns the name a debug engine reports for a stack frame back into the F# construct it came from.
[<RequireQualifiedAccess>]
module internal FSharpStackFrameNameParser =

    let private closureSuffix =
        Regex(@"^(?<name>.+)@(?<line>\d+)(?:-(?<ordinal>\d+))?$", RegexOptions.Compiled ||| RegexOptions.CultureInvariant)

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

    let private parseSegment (segment: string) =
        match segment.IndexOf('`') with
        | -1 -> { Name = segment; GenericArity = 0 }
        | i ->
            let arity =
                match Int32.TryParse(segment.Substring(i + 1)) with
                | true, n -> n
                | _ -> 0

            {
                Name = segment.Substring(0, i)
                GenericArity = arity
            }

    let private parseClosureOrigin (segment: string) =
        let m = closureSuffix.Match segment

        if m.Success then
            ValueSome
                {
                    EnclosingName = m.Groups.["name"].Value
                    Line = Int32.Parse(m.Groups.["line"].Value)
                    Ordinal =
                        if m.Groups.["ordinal"].Success then
                            ValueSome(Int32.Parse(m.Groups.["ordinal"].Value))
                        else
                            ValueNone
                }
        else
            ValueNone

    let private isActivePattern (name: string) =
        name.Length > 2
        && name.StartsWith("|", StringComparison.Ordinal)
        && name.EndsWith("|", StringComparison.Ordinal)

    let private activePatternCases (name: string) =
        name.Trim('|').Split('|')
        |> Array.filter (fun case -> not (String.IsNullOrEmpty case))
        |> List.ofArray

    let private classifyMember (segment: string) =
        if isActivePattern segment then
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

            let segments =
                name.Split([| '.'; '+' |])
                |> Array.filter (fun segment -> not (String.IsNullOrEmpty segment))
                |> List.ofArray

            match segments with
            | [] -> ValueNone
            | segments ->
                let frame path frameMember =
                    ValueSome
                        {
                            Path = path |> List.map parseSegment
                            Member = frameMember
                            MethodGenericArity = methodGenericArity
                        }

                let isStartupCode =
                    segments
                    |> List.exists (fun segment -> segment.StartsWith(startupCodePrefix, StringComparison.Ordinal))

                let closureIndex =
                    segments
                    |> List.tryFindIndexBack (fun segment -> (parseClosureOrigin segment).IsSome)

                match constructorMember, isStartupCode, closureIndex with
                | ValueSome constructorMember, _, _ -> frame segments constructorMember
                | ValueNone, true, _ ->
                    let declaringModule =
                        segments
                        |> List.filter (fun segment -> not (segment.StartsWith(startupCodePrefix, StringComparison.Ordinal)))
                        |> List.map _.TrimStart('$')
                        |> List.filter (fun segment ->
                            not (String.IsNullOrEmpty segment)
                            && not (segment.EndsWith("@", StringComparison.Ordinal)))

                    frame declaringModule FrameStartupCode
                | ValueNone, false, Some i ->
                    let origin = (parseClosureOrigin segments.[i]).Value
                    frame (List.truncate i segments) (FrameClosureBody origin)
                | ValueNone, false, None -> frame (List.truncate (segments.Length - 1) segments) (classifyMember (List.last segments))
