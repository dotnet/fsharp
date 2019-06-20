// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Core

type PrintfFormat<'Printer,'State,'Residue,'Result>(value:string) =
        member x.Value = value

        override __.ToString() = value
    
type PrintfFormat<'Printer,'State,'Residue,'Result,'Tuple>(value:string) = 
    inherit PrintfFormat<'Printer,'State,'Residue,'Result>(value)

type Format<'Printer,'State,'Residue,'Result> = PrintfFormat<'Printer,'State,'Residue,'Result>
type Format<'Printer,'State,'Residue,'Result,'Tuple> = PrintfFormat<'Printer,'State,'Residue,'Result,'Tuple>

module internal PrintfImpl =

    /// Basic idea of implementation:
    /// Every Printf.* family should returns curried function that collects arguments and then somehow prints them.
    /// Idea - instead of building functions on fly argument by argument we instead introduce some predefined parts and then construct functions from these parts
    /// Parts include:
    /// Plain ones:
    /// 1. Final pieces (1..5) - set of functions with arguments number 1..5. 
    /// Primary characteristic - these functions produce final result of the *printf* operation
    /// 2. Chained pieces (1..5) - set of functions with arguments number 1..5. 
    /// Primary characteristic - these functions doesn not produce final result by itself, instead they tailed with some another piece (chained or final).
    /// Plain parts correspond to simple format specifiers (that are projected to just one parameter of the function, say %d or %s). However we also have 
    /// format specifiers that can be projected to more than one argument (i.e %a, %t or any simple format specified with * width or precision). 
    /// For them we add special cases (both chained and final to denote that they can either return value themselves or continue with some other piece)
    /// These primitives allow us to construct curried functions with arbitrary signatures.
    /// For example: 
    /// - function that corresponds to %s%s%s%s%s (string -> string -> string -> string -> string -> T) will be represented by one piece final 5.
    /// - function that has more that 5 arguments will include chained parts: %s%s%s%s%s%d%s  => chained2 -> final 5
    /// Primary benefits: 
    /// 1. creating specialized version of any part requires only one reflection call. This means that we can handle up to 5 simple format specifiers
    /// with just one reflection call
    /// 2. we can make combinable parts independent from particular printf implementation. Thus final result can be cached and shared. 
    /// i.e when first call to printf "%s %s" will trigger creation of the specialization. Subsequent calls will pick existing specialization
    open System
    open System.IO
    open System.Text

    open System.Collections.Generic
    open System.Reflection
    open Microsoft.FSharp.Core
    open Microsoft.FSharp.Core.Operators
    open Microsoft.FSharp.Collections
    open LanguagePrimitives.IntrinsicOperators

#if FX_RESHAPED_REFLECTION
    open Microsoft.FSharp.Core.PrimReflectionAdapters
    open Microsoft.FSharp.Core.ReflectionAdapters
#endif

    open System.IO
    
    [<Flags>]
    type FormatFlags = 
        | None = 0
        | LeftJustify = 1
        | PadWithZeros = 2
        | PlusForPositives = 4
        | SpaceForPositives = 8

    let inline hasFlag flags (expected: FormatFlags) = (flags &&& expected) = expected
    let inline isLeftJustify flags = hasFlag flags FormatFlags.LeftJustify
    let inline isPadWithZeros flags = hasFlag flags FormatFlags.PadWithZeros
    let inline isPlusForPositives flags = hasFlag flags FormatFlags.PlusForPositives
    let inline isSpaceForPositives flags = hasFlag flags FormatFlags.SpaceForPositives

    /// Used for width and precision to denote that user has specified '*' flag
    [<Literal>]
    let StarValue = -1
    /// Used for width and precision to denote that corresponding value was omitted in format string
    [<Literal>]
    let NotSpecifiedValue = -2

    [<System.Diagnostics.DebuggerDisplayAttribute("{ToString()}")>]
    [<NoComparison; NoEquality>]
    type FormatSpecifier =
        {
            TypeChar: char
            Precision: int
            Width: int
            Flags: FormatFlags
        }
        member this.IsStarPrecision = this.Precision = StarValue
        member this.IsPrecisionSpecified = this.Precision <> NotSpecifiedValue
        member this.IsStarWidth = this.Width = StarValue
        member this.IsWidthSpecified = this.Width <> NotSpecifiedValue

        override this.ToString() = 
            let valueOf n = match n with StarValue -> "*" | NotSpecifiedValue -> "-" | n -> n.ToString()
            System.String.Format
                (
                    "'{0}', Precision={1}, Width={2}, Flags={3}", 
                    this.TypeChar, 
                    (valueOf this.Precision),
                    (valueOf this.Width), 
                    this.Flags
                )
    
    /// Set of helpers to parse format string
    module private FormatString =

        let intFromString (s: string) pos =
            let rec go acc i =
                if Char.IsDigit s.[i] then 
                    let n = int s.[i] - int '0'
                    go (acc * 10 + n) (i + 1)
                else acc, i
            go 0 pos

        let parseFlags (s: string) i = 
            let rec go flags i = 
                match s.[i] with
                | '0' -> go (flags ||| FormatFlags.PadWithZeros) (i + 1)
                | '+' -> go (flags ||| FormatFlags.PlusForPositives) (i + 1)
                | ' ' -> go (flags ||| FormatFlags.SpaceForPositives) (i + 1)
                | '-' -> go (flags ||| FormatFlags.LeftJustify) (i + 1)
                | _ -> flags, i
            go FormatFlags.None i

        let parseWidth (s: string) i = 
            if s.[i] = '*' then StarValue, (i + 1)
            elif Char.IsDigit s.[i] then intFromString s i
            else NotSpecifiedValue, i

        let parsePrecision (s: string) i = 
            if s.[i] = '.' then
                if s.[i + 1] = '*' then StarValue, i + 2
                elif Char.IsDigit s.[i + 1] then intFromString s (i + 1)
                else raise (ArgumentException("invalid precision value"))
            else NotSpecifiedValue, i
        
        let parseTypeChar (s: string) i = 
            s.[i], (i + 1)
    
        let findNextFormatSpecifier (s: string) i = 
            let rec go i (buf: Text.StringBuilder) =
                if i >= s.Length then 
                    s.Length, buf.ToString()
                else
                    let c = s.[i]
                    if c = '%' then
                        if i + 1 < s.Length then
                            let _, i1 = parseFlags s (i + 1)
                            let w, i2 = parseWidth s i1
                            let p, i3 = parsePrecision s i2
                            let typeChar, i4 = parseTypeChar s i3
                            // shortcut for the simpliest case
                            // if typeChar is not % or it has star as width\precision - resort to long path
                            if typeChar = '%' && not (w = StarValue || p = StarValue) then 
                                buf.Append('%') |> ignore
                                go i4 buf
                            else 
                                i, buf.ToString()
                        else
                            raise (ArgumentException("Missing format specifier"))
                    else 
                        buf.Append c |> ignore
                        go (i + 1) buf
            go i (Text.StringBuilder())

    /// Abstracts generated printer from the details of particular environment: how to write text, how to produce results etc...
    [<AbstractClass>]
    type PrintfEnv<'State, 'Residue, 'Result> =
        val State: 'State
        new(s: 'State) = { State = s }
        abstract Finish: unit -> 'Result
        abstract Write: string -> unit
        abstract WriteT: 'Residue -> unit
    
    type Utils =
        static member inline Write (env: PrintfEnv<_, _, _>, a, b) =
            env.Write a
            env.Write b
        static member inline Write (env: PrintfEnv<_, _, _>, a, b, c) =
            Utils.Write(env, a, b)
            env.Write c
        static member inline Write (env: PrintfEnv<_, _, _>, a, b, c, d) =
            Utils.Write(env, a, b)
            Utils.Write(env, c, d)
        static member inline Write (env: PrintfEnv<_, _, _>, a, b, c, d, e) =
            Utils.Write(env, a, b, c)
            Utils.Write(env, d, e)
        static member inline Write (env: PrintfEnv<_, _, _>, a, b, c, d, e, f) =
            Utils.Write(env, a, b, c, d)
            Utils.Write(env, e, f)
        static member inline Write (env: PrintfEnv<_, _, _>, a, b, c, d, e, f, g) =
            Utils.Write(env, a, b, c, d, e)
            Utils.Write(env, f, g)
        static member inline Write (env: PrintfEnv<_, _, _>, a, b, c, d, e, f, g, h) =
            Utils.Write(env, a, b, c, d, e, f)
            Utils.Write(env, g, h)
        static member inline Write (env: PrintfEnv<_, _, _>, a, b, c, d, e, f, g, h, i) =
            Utils.Write(env, a, b, c, d, e, f, g)
            Utils.Write(env, h, i)
        static member inline Write (env: PrintfEnv<_, _, _>, a, b, c, d, e, f, g, h, i, j) =
            Utils.Write(env, a, b, c, d, e, f, g, h)
            Utils.Write(env, i, j)
        static member inline Write (env: PrintfEnv<_, _, _>, a, b, c, d, e, f, g, h, i, j, k) =
            Utils.Write(env, a, b, c, d, e, f, g, h, i)
            Utils.Write(env, j, k)
        static member inline Write (env: PrintfEnv<_, _, _>, a, b, c, d, e, f, g, h, i, j, k, l, m) =
            Utils.Write(env, a, b, c, d, e, f, g, h, i, j, k)
            Utils.Write(env, l, m)
    
    /// Type of results produced by specialization
    /// This is function that accepts thunk to create PrintfEnv on demand and returns concrete instance of Printer (curried function)
    /// After all arguments is collected, specialization obtains concrete PrintfEnv from the thunk and use it to output collected data.
    type PrintfFactory<'State, 'Residue, 'Result, 'Printer> = (unit -> PrintfEnv<'State, 'Residue, 'Result>) -> 'Printer

    [<Literal>]
    let MaxArgumentsInSpecialization = 5

    /// Specializations are created via factory methods. These methods accepts 2 kinds of arguments
    /// - parts of format string that corresponds to raw text
    /// - functions that can transform collected values to strings
    /// basic shape of the signature of specialization
    /// <prefix-string> + <converter for arg1> + <suffix that comes after arg1> + ... <converter for arg-N> + <suffix that comes after arg-N>
    type Specializations<'State, 'Residue, 'Result> private ()=
     
        static member Final1<'A>
            (
                s0, conv1, s1
            ) =
            (fun (env: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (a: 'A) ->
                    let env = env()
                    Utils.Write(env, s0, (conv1 a), s1)
                    env.Finish()
                )
            )

        static member FinalFastEnd1<'A>
            (
                s0, conv1
            ) =
            (fun (env: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (a: 'A) ->
                    let env = env()
                    Utils.Write(env, s0, (conv1 a))
                    env.Finish()
                )
            )

        static member FinalFastStart1<'A>
            (
                conv1, s1
            ) =
            (fun (env: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (a: 'A) ->
                    let env = env()
                    Utils.Write(env, (conv1 a), s1)
                    env.Finish()
                )
            )

        static member FinalFast1<'A>
            (
                conv1
            ) =
            (fun (env: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (a: 'A) ->
                    let env = env()
                    env.Write (conv1 a)
                    env.Finish()
                )
            )

        static member Final2<'A, 'B>
            (
                s0, conv1, s1, conv2, s2
            ) =
            (fun (env: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (a: 'A) (b: 'B) ->
                    let env = env()
                    Utils.Write(env, s0, (conv1 a), s1, (conv2 b), s2)
                    env.Finish()
                )
            )

        static member FinalFastEnd2<'A, 'B>
            (
                s0, conv1, s1, conv2
            ) =
            (fun (env: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (a: 'A) (b: 'B) ->
                    let env = env()
                    Utils.Write(env, s0, (conv1 a), s1, (conv2 b))
                    env.Finish()
                )
            )

        static member FinalFastStart2<'A, 'B>
            (
                conv1, s1, conv2, s2
            ) =
            (fun (env: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (a: 'A) (b: 'B) ->
                    let env = env()
                    Utils.Write(env, (conv1 a), s1, (conv2 b), s2)
                    env.Finish()
                )
            )

        static member FinalFast2<'A, 'B>
            (
                conv1, s1, conv2
            ) =
            (fun (env: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (a: 'A) (b: 'B) ->
                    let env = env()
                    Utils.Write(env, (conv1 a), s1, (conv2 b))
                    env.Finish()
                )
            )

        static member Final3<'A, 'B, 'C>
            (
                s0, conv1, s1, conv2, s2, conv3, s3
            ) =
            (fun (env: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (a: 'A) (b: 'B) (c: 'C) ->
                    let env = env()
                    Utils.Write(env, s0, (conv1 a), s1, (conv2 b), s2, (conv3 c), s3)
                    env.Finish()
                )
            )

        static member FinalFastEnd3<'A, 'B, 'C>
            (
                s0, conv1, s1, conv2, s2, conv3
            ) =
            (fun (env: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (a: 'A) (b: 'B) (c: 'C) ->
                    let env = env()
                    Utils.Write(env, s0, (conv1 a), s1, (conv2 b), s2, (conv3 c))
                    env.Finish()
                )
            )

        static member FinalFastStart3<'A, 'B, 'C>
            (
                conv1, s1, conv2, s2, conv3, s3
            ) =
            (fun (env: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (a: 'A) (b: 'B) (c: 'C) ->
                    let env = env()
                    Utils.Write(env, (conv1 a), s1, (conv2 b), s2, (conv3 c), s3)
                    env.Finish()
                )
            )

        static member FinalFast3<'A, 'B, 'C>
            (
                conv1, s1, conv2, s2, conv3
            ) =
            (fun (env: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (a: 'A) (b: 'B) (c: 'C) ->
                    let env = env()
                    Utils.Write(env, (conv1 a), s1, (conv2 b), s2, (conv3 c))
                    env.Finish()
                )
            )

        static member Final4<'A, 'B, 'C, 'D>
            (
                s0, conv1, s1, conv2, s2, conv3, s3, conv4, s4
            ) =
            (fun (env: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (a: 'A) (b: 'B) (c: 'C) (d: 'D)->
                    let env = env()
                    Utils.Write(env, s0, (conv1 a), s1, (conv2 b), s2, (conv3 c), s3, (conv4 d), s4)
                    env.Finish()
                )
            )

        static member FinalFastEnd4<'A, 'B, 'C, 'D>
            (
                s0, conv1, s1, conv2, s2, conv3, s3, conv4
            ) =
            (fun (env: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (a: 'A) (b: 'B) (c: 'C) (d: 'D)->
                    let env = env()
                    Utils.Write(env, s0, (conv1 a), s1, (conv2 b), s2, (conv3 c), s3, (conv4 d))
                    env.Finish()
                )
            )

        static member FinalFastStart4<'A, 'B, 'C, 'D>
            (
                conv1, s1, conv2, s2, conv3, s3, conv4, s4
            ) =
            (fun (env: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (a: 'A) (b: 'B) (c: 'C) (d: 'D)->
                    let env = env()
                    Utils.Write(env, (conv1 a), s1, (conv2 b), s2, (conv3 c), s3, (conv4 d), s4)
                    env.Finish()
                )
            )

        static member FinalFast4<'A, 'B, 'C, 'D>
            (
                conv1, s1, conv2, s2, conv3, s3, conv4
            ) =
            (fun (env: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (a: 'A) (b: 'B) (c: 'C) (d: 'D)->
                    let env = env()
                    Utils.Write(env, (conv1 a), s1, (conv2 b), s2, (conv3 c), s3, (conv4 d))
                    env.Finish()
                )
            )

        static member Final5<'A, 'B, 'C, 'D, 'E>
            (
                s0, conv1, s1, conv2, s2, conv3, s3, conv4, s4, conv5, s5
            ) =
            (fun (env: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (a: 'A) (b: 'B) (c: 'C) (d: 'D) (e: 'E)->
                    let env = env()
                    Utils.Write(env, s0, (conv1 a), s1, (conv2 b), s2, (conv3 c), s3, (conv4 d), s4, (conv5 e), s5)
                    env.Finish()
                )
            )

        static member FinalFastEnd5<'A, 'B, 'C, 'D, 'E>
            (
                s0, conv1, s1, conv2, s2, conv3, s3, conv4, s4, conv5
            ) =
            (fun (env: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (a: 'A) (b: 'B) (c: 'C) (d: 'D) (e: 'E)->
                    let env = env()
                    Utils.Write(env, s0, (conv1 a), s1, (conv2 b), s2, (conv3 c), s3, (conv4 d), s4, (conv5 e))
                    env.Finish()
                )
            )

        static member FinalFastStart5<'A, 'B, 'C, 'D, 'E>
            (
                conv1, s1, conv2, s2, conv3, s3, conv4, s4, conv5, s5
            ) =
            (fun (env: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (a: 'A) (b: 'B) (c: 'C) (d: 'D) (e: 'E)->
                    let env = env()
                    Utils.Write(env, (conv1 a), s1, (conv2 b), s2, (conv3 c), s3, (conv4 d), s4, (conv5 e), s5)
                    env.Finish()
                )
            )

        static member FinalFast5<'A, 'B, 'C, 'D, 'E>
            (
                conv1, s1, conv2, s2, conv3, s3, conv4, s4, conv5
            ) =
            (fun (env: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (a: 'A) (b: 'B) (c: 'C) (d: 'D) (e: 'E)->
                    let env = env()
                    Utils.Write(env, (conv1 a), s1, (conv2 b), s2, (conv3 c), s3, (conv4 d), s4, (conv5 e))
                    env.Finish()
                )
            )

        static member Chained1<'A, 'Tail>
            (
                s0, conv1,
                next
            ) =
            (fun (env: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (a: 'A) ->
                    let env() = 
                        let env = env()
                        Utils.Write(env, s0, (conv1 a))
                        env
                    next env : 'Tail
                )
            )

         static member ChainedFastStart1<'A, 'Tail>
            (
                conv1,
                next
            ) =
            (fun (env: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (a: 'A) ->
                    let env() = 
                        let env = env()
                        env.Write(conv1 a)
                        env
                    next env : 'Tail
                )
            )

        static member Chained2<'A, 'B, 'Tail>
            (
                s0, conv1, s1, conv2,
                next
            ) =
            (fun (env: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (a: 'A) (b: 'B) ->
                    let env() = 
                        let env = env()
                        Utils.Write(env, s0, (conv1 a), s1, (conv2 b))
                        env
                    next env : 'Tail
                )
            )

        static member ChainedFastStart2<'A, 'B, 'Tail>
            (
                conv1, s1, conv2,
                next
            ) =
            (fun (env: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (a: 'A) (b: 'B) ->
                    let env() = 
                        let env = env()
                        Utils.Write(env, (conv1 a), s1, (conv2 b))
                        env
                    next env : 'Tail
                )
            )

        static member Chained3<'A, 'B, 'C, 'Tail>
            (
                s0, conv1, s1, conv2, s2, conv3,
                next
            ) =
            (fun (env: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (a: 'A) (b: 'B) (c: 'C) ->
                    let env() = 
                        let env = env()
                        Utils.Write(env, s0, (conv1 a), s1, (conv2 b), s2, (conv3 c))
                        env
                    next env : 'Tail
                )
            )

        static member ChainedFastStart3<'A, 'B, 'C, 'Tail>
            (
                conv1, s1, conv2, s2, conv3,
                next
            ) =
            (fun (env: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (a: 'A) (b: 'B) (c: 'C) ->
                    let env() = 
                        let env = env()
                        Utils.Write(env, (conv1 a), s1, (conv2 b), s2, (conv3 c))
                        env
                    next env : 'Tail
                )
            )

        static member Chained4<'A, 'B, 'C, 'D, 'Tail>
            (
                s0, conv1, s1, conv2, s2, conv3, s3, conv4,
                next
            ) =
            (fun (env: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (a: 'A) (b: 'B) (c: 'C) (d: 'D)->
                    let env() = 
                        let env = env()
                        Utils.Write(env, s0, (conv1 a), s1, (conv2 b), s2, (conv3 c), s3, (conv4 d))
                        env
                    next env : 'Tail
                )
            )

        static member ChainedFastStart4<'A, 'B, 'C, 'D, 'Tail>
            (
                conv1, s1, conv2, s2, conv3, s3, conv4,
                next
            ) =
            (fun (env: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (a: 'A) (b: 'B) (c: 'C) (d: 'D)->
                    let env() = 
                        let env = env()
                        Utils.Write(env, (conv1 a), s1, (conv2 b), s2, (conv3 c), s3, (conv4 d))
                        env
                    next env : 'Tail
                )
            )

        static member Chained5<'A, 'B, 'C, 'D, 'E, 'Tail>
            (
                s0, conv1, s1, conv2, s2, conv3, s3, conv4, s4, conv5,
                next
            ) =
            (fun (env: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (a: 'A) (b: 'B) (c: 'C) (d: 'D) (e: 'E)->
                    let env() = 
                        let env = env()
                        Utils.Write(env, s0, (conv1 a), s1, (conv2 b), s2, (conv3 c), s3, (conv4 d), s4, (conv5 e))
                        env
                    next env : 'Tail
                )
            )

        static member ChainedFastStart5<'A, 'B, 'C, 'D, 'E, 'Tail>
            (
                conv1, s1, conv2, s2, conv3, s3, conv4, s4, conv5,
                next
            ) =
            (fun (env: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (a: 'A) (b: 'B) (c: 'C) (d: 'D) (e: 'E)->
                    let env() = 
                        let env = env()
                        Utils.Write(env, (conv1 a), s1, (conv2 b), s2, (conv3 c), s3, (conv4 d), s4, (conv5 e))
                        env
                    next env : 'Tail
                )
            )

        static member TFinal(s1: string, s2: string) = 
            (fun (env: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (f: 'State -> 'Residue) -> 
                    let env = env()
                    env.Write s1
                    env.WriteT(f env.State)
                    env.Write s2
                    env.Finish()
                )
            )
        static member TChained<'Tail>(s1: string, next: PrintfFactory<'State, 'Residue, 'Result,'Tail>) = 
            (fun (env: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (f: 'State -> 'Residue) -> 
                    let env() = 
                        let env = env()
                        env.Write s1
                        env.WriteT(f env.State)
                        env
                    next env: 'Tail
                )
            )

        static member LittleAFinal<'A>(s1: string, s2: string) = 
            (fun (env: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (f: 'State -> 'A ->'Residue) (a: 'A) -> 
                    let env = env()
                    env.Write s1
                    env.WriteT(f env.State a)
                    env.Write s2
                    env.Finish()
                )
            )
        static member LittleAChained<'A, 'Tail>(s1: string, next: PrintfFactory<'State, 'Residue, 'Result,'Tail>) = 
            (fun (env: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (f: 'State -> 'A ->'Residue) (a: 'A) -> 
                    let env() = 
                        let env = env()
                        env.Write s1
                        env.WriteT(f env.State a)
                        env
                    next env: 'Tail
                )
            )

        static member StarFinal1<'A>(s1: string, conv, s2: string) = 
            (fun (env: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (star1: int) (a: 'A) -> 
                    let env = env()
                    env.Write s1
                    env.Write (conv a star1: string)
                    env.Write s2
                    env.Finish()
                )
            )   
       
        static member PercentStarFinal1(s1: string, s2: string) = 
            (fun (env: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (_star1 : int) -> 
                    let env = env()
                    env.Write s1
                    env.Write("%")
                    env.Write s2
                    env.Finish()
                )
            )

        static member StarFinal2<'A>(s1: string, conv, s2: string) = 
            (fun (env: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (star1: int) (star2: int) (a: 'A) -> 
                    let env = env()
                    env.Write s1
                    env.Write (conv a star1 star2: string)
                    env.Write s2
                    env.Finish()
                )
            )

        /// Handles case when '%*.*%' is used at the end of string
        static member PercentStarFinal2(s1: string, s2: string) = 
            (fun (env: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (_star1 : int) (_star2 : int) -> 
                    let env = env()
                    env.Write s1
                    env.Write("%")
                    env.Write s2
                    env.Finish()
                )
            )

        static member StarChained1<'A, 'Tail>(s1: string, conv, next: PrintfFactory<'State, 'Residue, 'Result,'Tail>) = 
            (fun (env: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (star1: int) (a: 'A) -> 
                    let env() =
                        let env = env()
                        env.Write s1
                        env.Write(conv a star1 : string)
                        env
                    next env : 'Tail
                )
            )
        
        /// Handles case when '%*%' is used in the middle of the string so it needs to be chained to another printing block
        static member PercentStarChained1<'Tail>(s1: string, next: PrintfFactory<'State, 'Residue, 'Result,'Tail>) = 
            (fun (env: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (_star1 : int) -> 
                    let env() =
                        let env = env()
                        env.Write s1
                        env.Write("%")
                        env
                    next env: 'Tail
                )
            )

        static member StarChained2<'A, 'Tail>(s1: string, conv, next: PrintfFactory<'State, 'Residue, 'Result,'Tail>) = 
            (fun (env: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (star1: int) (star2: int) (a: 'A) -> 
                    let env() =
                        let env = env()
                        env.Write s1
                        env.Write(conv a star1 star2 : string)
                        env
                    next env : 'Tail
                )
            )
        
        /// Handles case when '%*.*%' is used in the middle of the string so it needs to be chained to another printing block
        static member PercentStarChained2<'Tail>(s1: string, next: PrintfFactory<'State, 'Residue, 'Result,'Tail>) = 
            (fun (env: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (_star1 : int) (_star2 : int) -> 
                    let env() =
                        let env = env()
                        env.Write s1
                        env.Write("%")
                        env
                    next env : 'Tail
                )
            )
    
    let inline (===) a b = Object.ReferenceEquals(a, b)
    let invariantCulture = System.Globalization.CultureInfo.InvariantCulture 

    let inline boolToString v = if v then "true" else "false"
    let inline stringToSafeString v = 
        match v with
        | null -> ""
        | _ -> v

    [<Literal>]
    let DefaultPrecision = 6

    let getFormatForFloat (ch: char) (prec: int) = ch.ToString() +  prec.ToString()
    let normalizePrecision prec = min (max prec 0) 99

    /// Contains helpers to convert printer functions to functions that prints value with respect to specified justification
    /// There are two kinds to printers: 
    /// 'T -> string - converts value to string - used for strings, basic integers etc..
    /// string -> 'T -> string - converts value to string with given format string - used by numbers with floating point, typically precision is set via format string 
    /// To support both categories there are two entry points:
    /// - withPadding - adapts first category
    /// - withPaddingFormatted - adapts second category
    module Padding = 
        /// pad here is function that converts T to string with respect of justification
        /// basic - function that converts T to string without applying justification rules
        /// adaptPaddedFormatted returns boxed function that has various number of arguments depending on if width\precision flags has '*' value 
        let inline adaptPaddedFormatted (spec: FormatSpecifier) getFormat (basic: string -> 'T -> string) (pad: string -> int -> 'T -> string) = 
            if spec.IsStarWidth then
                if spec.IsStarPrecision then
                    // width=*, prec=*
                    box(fun v width prec -> 
                        let fmt = getFormat (normalizePrecision prec)
                        pad fmt width v)
                else 
                    // width=*, prec=?
                    let prec = if spec.IsPrecisionSpecified then normalizePrecision spec.Precision else DefaultPrecision
                    let fmt = getFormat prec
                    box(fun v width -> 
                        pad fmt width v)

            elif spec.IsStarPrecision then
                if spec.IsWidthSpecified then
                    // width=val, prec=*
                    box(fun v prec -> 
                        let fmt = getFormat prec
                        pad fmt spec.Width v)
                else
                    // width=X, prec=*
                    box(fun v prec -> 
                        let fmt = getFormat prec
                        basic fmt v)                        
            else
                let prec = if spec.IsPrecisionSpecified then normalizePrecision spec.Precision else DefaultPrecision
                let fmt = getFormat prec
                if spec.IsWidthSpecified then
                    // width=val, prec=*
                    box(fun v -> 
                        pad fmt spec.Width v)
                else
                    // width=X, prec=*
                    box(fun v -> 
                        basic fmt v)

        /// pad here is function that converts T to string with respect of justification
        /// basic - function that converts T to string without applying justification rules
        /// adaptPadded returns boxed function that has various number of arguments depending on if width flags has '*' value 
        let inline adaptPadded (spec: FormatSpecifier) (basic: 'T -> string) (pad: int -> 'T -> string) = 
            if spec.IsStarWidth then
                    // width=*, prec=?
                    box(fun v width -> 
                        pad width v)
            else
                if spec.IsWidthSpecified then
                    // width=val, prec=*
                    box(fun v -> 
                        pad spec.Width v)
                else
                    // width=X, prec=*
                    box(fun v -> 
                        basic v)

        let inline withPaddingFormatted (spec: FormatSpecifier) getFormat  (defaultFormat: string) (f: string ->  'T -> string) left right =
            if not (spec.IsWidthSpecified || spec.IsPrecisionSpecified) then
                box (f defaultFormat)
            else
                if isLeftJustify spec.Flags then
                    adaptPaddedFormatted spec getFormat f left
                else
                    adaptPaddedFormatted spec getFormat f right

        let inline withPadding (spec: FormatSpecifier) (f: 'T -> string) left right =
            if not spec.IsWidthSpecified then
                box f
            else
                if isLeftJustify spec.Flags then
                    adaptPadded spec f left
                else
                    adaptPadded  spec f right

    let inline isNumber (x: ^T) =
        not (^T: (static member IsPositiveInfinity: 'T -> bool) x) && not (^T: (static member IsNegativeInfinity: 'T -> bool) x) && not (^T: (static member IsNaN: 'T -> bool) x)

    let inline isInteger n = 
        n % LanguagePrimitives.GenericOne = LanguagePrimitives.GenericZero
    
    let inline isPositive n = 
        n >= LanguagePrimitives.GenericZero

    /// contains functions to handle left\right justifications for non-numeric types (strings\bools)
    module Basic =
        let inline leftJustify (f: 'T -> string) padChar = 
            fun (w: int) v -> 
                (f v).PadRight(w, padChar)
    
        let inline rightJustify (f: 'T -> string) padChar = 
            fun (w: int) v -> 
                (f v).PadLeft(w, padChar)
    
    /// contains functions to handle left\right and no justification case for numbers
    module GenericNumber =
        /// handles right justification when pad char = '0'
        /// this case can be tricky:
        /// - negative numbers, -7 should be printed as '-007', not '00-7'
        /// - positive numbers when prefix for positives is set: 7 should be '+007', not '00+7'
        let inline rightJustifyWithZeroAsPadChar (str: string) isNumber isPositive w (prefixForPositives: string) =
            System.Diagnostics.Debug.Assert(prefixForPositives.Length = 0 || prefixForPositives.Length = 1)
            if isNumber then
                if isPositive then
                    prefixForPositives + (if w = 0 then str else str.PadLeft(w - prefixForPositives.Length, '0')) // save space to 
                else
                    if str.[0] = '-' then
                        let str = str.Substring 1
                        "-" + (if w = 0 then str else str.PadLeft(w - 1, '0'))
                    else
                        str.PadLeft(w, '0')
            else
                str.PadLeft(w, ' ')
        
        /// handler right justification when pad char = ' '
        let inline rightJustifyWithSpaceAsPadChar (str: string) isNumber isPositive w (prefixForPositives: string) =
            System.Diagnostics.Debug.Assert(prefixForPositives.Length = 0 || prefixForPositives.Length = 1)
            (if isNumber && isPositive then prefixForPositives + str else str).PadLeft(w, ' ')
        
        /// handles left justification with formatting with 'G'\'g' - either for decimals or with 'g'\'G' is explicitly set 
        let inline leftJustifyWithGFormat (str: string) isNumber isInteger isPositive w (prefixForPositives: string) padChar  =
            if isNumber then
                let str = if isPositive then prefixForPositives + str else str
                // NOTE: difference - for 'g' format we use isInt check to detect situations when '5.0' is printed as '5'
                // in this case we need to override padding and always use ' ', otherwise we'll produce incorrect results
                if isInteger then
                    str.PadRight(w, ' ') // don't pad integer numbers with '0' when 'g' format is specified (may yield incorrect results)
                else
                    str.PadRight(w, padChar) // non-integer => string representation has point => can pad with any character
            else
                str.PadRight(w, ' ') // pad NaNs with ' '

        let inline leftJustifyWithNonGFormat (str: string) isNumber isPositive w (prefixForPositives: string) padChar  =
            if isNumber then
                let str = if isPositive then prefixForPositives + str else str
                str.PadRight(w, padChar)
            else
                str.PadRight(w, ' ') // pad NaNs with ' ' 
        
        /// processes given string based depending on values isNumber\isPositive
        let inline noJustificationCore (str: string) isNumber isPositive prefixForPositives = 
            if isNumber && isPositive then prefixForPositives + str
            else str
        
        /// noJustification handler for f: 'T -> string - basic integer types
        let inline noJustification f (prefix: string) isUnsigned =
            if isUnsigned then
                fun v -> noJustificationCore (f v) true true prefix
            else 
                fun v -> noJustificationCore (f v) true (isPositive v) prefix

        /// noJustification handler for f: string -> 'T -> string - floating point types
        let inline noJustificationWithFormat f (prefix: string) = 
            fun (fmt: string) v -> noJustificationCore (f fmt v) true (isPositive v) prefix

        /// leftJustify handler for f: 'T -> string - basic integer types
        let inline leftJustify isGFormat f (prefix: string) padChar isUnsigned = 
            if isUnsigned then
                if isGFormat then
                    fun (w: int) v ->
                        leftJustifyWithGFormat (f v) true (isInteger v) true w prefix padChar
                else
                    fun (w: int) v ->
                        leftJustifyWithNonGFormat (f v) true true w prefix padChar
            else
                if isGFormat then
                    fun (w: int) v ->
                        leftJustifyWithGFormat (f v) true (isInteger v) (isPositive v) w prefix padChar
                else
                    fun (w: int) v ->
                        leftJustifyWithNonGFormat (f v) true (isPositive v) w prefix padChar
        
        /// leftJustify handler for f: string -> 'T -> string - floating point types                    
        let inline leftJustifyWithFormat isGFormat f (prefix: string) padChar = 
            if isGFormat then
                fun (fmt: string) (w: int) v ->
                    leftJustifyWithGFormat (f fmt v) true (isInteger v) (isPositive v) w prefix padChar
            else
                fun (fmt: string) (w: int) v ->
                    leftJustifyWithNonGFormat (f fmt v) true (isPositive v) w prefix padChar    

        /// rightJustify handler for f: 'T -> string - basic integer types
        let inline rightJustify f (prefixForPositives: string) padChar isUnsigned =
            if isUnsigned then
                if padChar = '0' then
                    fun (w: int) v ->
                        rightJustifyWithZeroAsPadChar (f v) true true w prefixForPositives
                else
                    System.Diagnostics.Debug.Assert((padChar = ' '))
                    fun (w: int) v ->
                        rightJustifyWithSpaceAsPadChar (f v) true true w prefixForPositives
            else
                if padChar = '0' then
                    fun (w: int) v ->
                        rightJustifyWithZeroAsPadChar (f v) true (isPositive v) w prefixForPositives

                else
                    System.Diagnostics.Debug.Assert((padChar = ' '))
                    fun (w: int) v ->
                        rightJustifyWithSpaceAsPadChar (f v) true (isPositive v) w prefixForPositives

        /// rightJustify handler for f: string -> 'T -> string - floating point types                    
        let inline rightJustifyWithFormat f (prefixForPositives: string) padChar =
            if padChar = '0' then
                fun (fmt: string) (w: int) v ->
                    rightJustifyWithZeroAsPadChar (f fmt v) true (isPositive v) w prefixForPositives

            else
                System.Diagnostics.Debug.Assert((padChar = ' '))
                fun (fmt: string) (w: int) v ->
                    rightJustifyWithSpaceAsPadChar (f fmt v) true (isPositive v) w prefixForPositives
    module Float = 
        let inline noJustification f (prefixForPositives: string) = 
            fun (fmt: string) v -> 
                GenericNumber.noJustificationCore (f fmt v) (isNumber v) (isPositive v) prefixForPositives
    
        let inline leftJustify isGFormat f (prefix: string) padChar = 
            if isGFormat then
                fun (fmt: string) (w: int) v ->
                    GenericNumber.leftJustifyWithGFormat (f fmt v) (isNumber v) (isInteger v) (isPositive v) w prefix padChar
            else
                fun (fmt: string) (w: int) v ->
                    GenericNumber.leftJustifyWithNonGFormat (f fmt v) (isNumber v) (isPositive v) w prefix padChar  

        let inline rightJustify f (prefixForPositives: string) padChar =
            if padChar = '0' then
                fun (fmt: string) (w: int) v ->
                    GenericNumber.rightJustifyWithZeroAsPadChar (f fmt v) (isNumber v) (isPositive v) w prefixForPositives
            else
                System.Diagnostics.Debug.Assert((padChar = ' '))
                fun (fmt: string) (w: int) v ->
                    GenericNumber.rightJustifyWithSpaceAsPadChar (f fmt v) (isNumber v) (isPositive v) w prefixForPositives

    let isDecimalFormatSpecifier (spec: FormatSpecifier) = 
        spec.TypeChar = 'M'

    let getPadAndPrefix allowZeroPadding (spec: FormatSpecifier) = 
        let padChar = if allowZeroPadding && isPadWithZeros spec.Flags then '0' else ' ';
        let prefix = 
            if isPlusForPositives spec.Flags then "+" 
            elif isSpaceForPositives spec.Flags then " "
            else ""
        padChar, prefix    

    let isGFormat(spec: FormatSpecifier) = 
        isDecimalFormatSpecifier spec || System.Char.ToLower(spec.TypeChar) = 'g'

    let inline basicWithPadding (spec: FormatSpecifier) f =
        let padChar, _ = getPadAndPrefix false spec
        Padding.withPadding spec f (Basic.leftJustify f padChar) (Basic.rightJustify f padChar)
    
    let inline numWithPadding (spec: FormatSpecifier) isUnsigned f  =
        let allowZeroPadding = not (isLeftJustify spec.Flags) || isDecimalFormatSpecifier spec
        let padChar, prefix = getPadAndPrefix allowZeroPadding spec
        let isGFormat = isGFormat spec
        Padding.withPadding spec (GenericNumber.noJustification f prefix isUnsigned) (GenericNumber.leftJustify isGFormat f prefix padChar isUnsigned) (GenericNumber.rightJustify f prefix padChar isUnsigned)

    let inline decimalWithPadding (spec: FormatSpecifier) getFormat defaultFormat f =
        let padChar, prefix = getPadAndPrefix true spec
        let isGFormat = isGFormat spec
        Padding.withPaddingFormatted spec getFormat defaultFormat (GenericNumber.noJustificationWithFormat f prefix) (GenericNumber.leftJustifyWithFormat isGFormat f prefix padChar) (GenericNumber.rightJustifyWithFormat f prefix padChar)

    let inline floatWithPadding (spec: FormatSpecifier) getFormat defaultFormat f =
        let padChar, prefix = getPadAndPrefix true spec
        let isGFormat = isGFormat spec
        Padding.withPaddingFormatted spec getFormat defaultFormat (Float.noJustification f prefix) (Float.leftJustify isGFormat f prefix padChar) (Float.rightJustify f prefix padChar)
    
    let inline identity v =  v
    let inline toString  v =   (^T : (member ToString: IFormatProvider -> string)(v, invariantCulture))
    let inline toFormattedString fmt = fun (v: ^T) -> (^T: (member ToString: string * IFormatProvider -> string)(v, fmt, invariantCulture))

    let inline numberToString c spec alt unsignedConv  =
        if c = 'd' || c = 'i' then
            numWithPadding spec false (alt >> toString: ^T -> string)
        elif c = 'u' then
            numWithPadding spec true  (alt >> unsignedConv >> toString: ^T -> string) 
        elif c = 'x' then
            numWithPadding spec true (alt >> toFormattedString "x": ^T -> string)
        elif c = 'X' then
            numWithPadding spec true (alt >> toFormattedString "X": ^T -> string )
        elif c = 'o' then
            numWithPadding spec true (fun (v: ^T) -> Convert.ToString(int64(unsignedConv (alt v)), 8))
        else raise (ArgumentException())    
    
    type ObjectPrinter = 
        static member ObjectToString<'T>(spec: FormatSpecifier) = 
            basicWithPadding spec (fun (v: 'T) -> match box v with null -> "<null>" | x -> x.ToString())
        
        static member GenericToStringCore(v: 'T, opts: Microsoft.FSharp.Text.StructuredPrintfImpl.FormatOptions, bindingFlags) = 
            // printfn %0A is considered to mean 'print width zero'
            match box v with 
            | null -> "<null>" 
            | _ -> Microsoft.FSharp.Text.StructuredPrintfImpl.Display.anyToStringForPrintf opts bindingFlags (v, v.GetType())

        static member GenericToString<'T>(spec: FormatSpecifier) = 
            let bindingFlags = 
#if FX_RESHAPED_REFLECTION
                isPlusForPositives spec.Flags // true - show non-public
#else
                if isPlusForPositives spec.Flags then BindingFlags.Public ||| BindingFlags.NonPublic
                else BindingFlags.Public 
#endif

            let useZeroWidth = isPadWithZeros spec.Flags
            let opts = 
                let o = Microsoft.FSharp.Text.StructuredPrintfImpl.FormatOptions.Default
                let o =
                    if useZeroWidth then { o with PrintWidth = 0} 
                    elif spec.IsWidthSpecified then { o with PrintWidth = spec.Width}
                    else o
                if spec.IsPrecisionSpecified then { o with PrintSize = spec.Precision}
                else o
            match spec.IsStarWidth, spec.IsStarPrecision with
            | true, true ->
                box (fun (v: 'T) (width: int) (prec: int) ->
                    let opts = { opts with PrintSize = prec }
                    let opts  = if not useZeroWidth then { opts with PrintWidth = width} else opts
                    ObjectPrinter.GenericToStringCore(v, opts, bindingFlags)
                    )
            | true, false ->
                box (fun (v: 'T) (width: int) ->
                    let opts  = if not useZeroWidth then { opts with PrintWidth = width} else opts
                    ObjectPrinter.GenericToStringCore(v, opts, bindingFlags)
                    )
            | false, true ->
                box (fun (v: 'T) (prec: int) ->
                    let opts = { opts with PrintSize = prec }
                    ObjectPrinter.GenericToStringCore(v, opts, bindingFlags)
                    )
            | false, false ->
                box (fun (v: 'T) ->
                    ObjectPrinter.GenericToStringCore(v, opts, bindingFlags)
                    )
    
    let basicNumberToString (ty: Type) (spec: FormatSpecifier) =
        System.Diagnostics.Debug.Assert(not spec.IsPrecisionSpecified, "not spec.IsPrecisionSpecified")

        let ch = spec.TypeChar

        match Type.GetTypeCode ty with
        | TypeCode.Int32    -> numberToString ch spec identity (uint32: int -> uint32) 
        | TypeCode.Int64    -> numberToString ch spec identity (uint64: int64 -> uint64)
        | TypeCode.Byte     -> numberToString ch spec identity (byte: byte -> byte) 
        | TypeCode.SByte    -> numberToString ch spec identity (byte: sbyte -> byte)
        | TypeCode.Int16    -> numberToString ch spec identity (uint16: int16 -> uint16)
        | TypeCode.UInt16   -> numberToString ch spec identity (uint16: uint16 -> uint16)
        | TypeCode.UInt32   -> numberToString ch spec identity (uint32: uint32 -> uint32)
        | TypeCode.UInt64   -> numberToString ch spec identity (uint64: uint64 -> uint64)
        | _ ->
        if ty === typeof<nativeint> then 
            if IntPtr.Size = 4 then 
                numberToString ch spec (fun (v: IntPtr) -> v.ToInt32()) uint32
            else
                numberToString ch spec (fun (v: IntPtr) -> v.ToInt64()) uint64
        elif ty === typeof<unativeint> then 
            if IntPtr.Size = 4 then
                numberToString ch spec (fun (v: UIntPtr) -> v.ToUInt32()) uint32
            else
                numberToString ch spec (fun (v: UIntPtr) -> v.ToUInt64()) uint64

        else raise (ArgumentException(ty.Name + " not a basic integer type"))

    let basicFloatToString ty spec = 
        let defaultFormat = getFormatForFloat spec.TypeChar DefaultPrecision
        match Type.GetTypeCode ty with
        | TypeCode.Single   -> floatWithPadding spec (getFormatForFloat spec.TypeChar) defaultFormat (fun fmt (v: float32) -> toFormattedString fmt v)
        | TypeCode.Double   -> floatWithPadding spec (getFormatForFloat spec.TypeChar) defaultFormat (fun fmt (v: float) -> toFormattedString fmt v)
        | TypeCode.Decimal  -> decimalWithPadding spec (getFormatForFloat spec.TypeChar) defaultFormat (fun fmt (v: decimal) -> toFormattedString fmt v)
        | _ -> raise (ArgumentException(ty.Name + " not a basic floating point type"))

    let private NonPublicStatics = BindingFlags.NonPublic ||| BindingFlags.Static

    let private getValueConverter (ty: Type) (spec: FormatSpecifier) : obj = 
        match spec.TypeChar with
        | 'b' ->  
            System.Diagnostics.Debug.Assert(ty === typeof<bool>, "ty === typeof<bool>")
            basicWithPadding spec boolToString
        | 's' ->
            System.Diagnostics.Debug.Assert(ty === typeof<string>, "ty === typeof<string>")
            basicWithPadding spec stringToSafeString
        | 'c' ->
            System.Diagnostics.Debug.Assert(ty === typeof<char>, "ty === typeof<char>")
            basicWithPadding spec (fun (c: char) -> c.ToString())
        | 'M'  ->
            System.Diagnostics.Debug.Assert(ty === typeof<decimal>, "ty === typeof<decimal>")
            decimalWithPadding spec (fun _ -> "G") "G" (fun fmt (v: decimal) -> toFormattedString fmt v) // %M ignores precision
        | 'd' | 'i' | 'x' | 'X' | 'u' | 'o'-> 
            basicNumberToString ty spec
        | 'e' | 'E' 
        | 'f' | 'F' 
        | 'g' | 'G' -> 
            basicFloatToString ty spec
        | 'A' ->
            let mi = typeof<ObjectPrinter>.GetMethod("GenericToString", NonPublicStatics)
            let mi = mi.MakeGenericMethod ty
            mi.Invoke(null, [| box spec |])
        | 'O' -> 
            let mi = typeof<ObjectPrinter>.GetMethod("ObjectToString", NonPublicStatics)
            let mi = mi.MakeGenericMethod ty
            mi.Invoke(null, [| box spec |])
        | _ -> 
            raise (ArgumentException(SR.GetString(SR.printfBadFormatSpecifier)))
    
    let extractCurriedArguments (ty: Type) n = 
        System.Diagnostics.Debug.Assert(n = 1 || n = 2 || n = 3, "n = 1 || n = 2 || n = 3")
        let buf = Array.zeroCreate (n + 1)
        let rec go (ty: Type) i = 
            if i < n then
                match ty.GetGenericArguments() with
                | [| argTy; retTy|] ->
                    buf.[i] <- argTy
                    go retTy (i + 1)
                | _ -> failwith (String.Format("Expected function with {0} arguments", n))
            else 
                System.Diagnostics.Debug.Assert((i = n), "i = n")
                buf.[i] <- ty
                buf           
        go ty 0    
    
    type private PrintfBuilderStack() = 
        let args = Stack 10
        let types = Stack 5

        let stackToArray size start count (s: Stack<_>) = 
            let arr = Array.zeroCreate size
            for i = 0 to count - 1 do
                arr.[start + i] <- s.Pop()
            arr
        
        member __.GetArgumentAndTypesAsArrays
            (
                argsArraySize, argsArrayStartPos, argsArrayTotalCount, 
                typesArraySize, typesArrayStartPos, typesArrayTotalCount 
            ) = 
            let argsArray = stackToArray argsArraySize argsArrayStartPos argsArrayTotalCount args
            let typesArray = stackToArray typesArraySize typesArrayStartPos typesArrayTotalCount types
            argsArray, typesArray

        member __.PopContinuationWithType() = 
            System.Diagnostics.Debug.Assert(args.Count = 1, "args.Count = 1")
            System.Diagnostics.Debug.Assert(types.Count = 1, "types.Count = 1")
            
            let cont = args.Pop()
            let contTy = types.Pop()

            cont, contTy

        member __.PopValueUnsafe() = args.Pop()

        member this.PushContinuationWithType (cont: obj, contTy: Type) = 
            System.Diagnostics.Debug.Assert(this.IsEmpty, "this.IsEmpty")
            System.Diagnostics.Debug.Assert(
                (
                    let _arg, retTy = Microsoft.FSharp.Reflection.FSharpType.GetFunctionElements(cont.GetType())
                    contTy.IsAssignableFrom retTy
                ),
                "incorrect type"
                )

            this.PushArgumentWithType(cont, contTy)

        member __.PushArgument(value: obj) =
            args.Push value

        member __.PushArgumentWithType(value: obj, ty) =
            args.Push value
            types.Push ty

        member __.HasContinuationOnStack expectedNumberOfArguments = 
            types.Count = expectedNumberOfArguments + 1

        member __.IsEmpty = 
            System.Diagnostics.Debug.Assert(args.Count = types.Count, "args.Count = types.Count")
            args.Count = 0

    /// Parses format string and creates result printer function.
    /// First it recursively consumes format string up to the end, then during unwinding builds printer using PrintfBuilderStack as storage for arguments.
    /// idea of implementation is very simple: every step can either push argument to the stack (if current block of 5 format specifiers is not yet filled) 
    //  or grab the content of stack, build intermediate printer and push it back to stack (so it can later be consumed by as argument) 
    type private PrintfBuilder<'S, 'Re, 'Res>() =
    
        let mutable count = 0
        let mutable optimizedArgCount = 0
#if DEBUG
        let verifyMethodInfoWasTaken (mi: System.Reflection.MemberInfo) =
            if isNull mi then 
                ignore (System.Diagnostics.Debugger.Launch())
#endif
            
        let buildSpecialChained(spec: FormatSpecifier, argTys: Type[], prefix: string, tail: obj, retTy) = 
            if spec.TypeChar = 'a' then
                let mi = typeof<Specializations<'S, 'Re, 'Res>>.GetMethod("LittleAChained", NonPublicStatics)
#if DEBUG
                verifyMethodInfoWasTaken mi
#endif

                let mi = mi.MakeGenericMethod([| argTys.[1];  retTy |])
                let args = [| box prefix; tail   |]
                mi.Invoke(null, args)
            elif spec.TypeChar = 't' then
                let mi = typeof<Specializations<'S, 'Re, 'Res>>.GetMethod("TChained", NonPublicStatics)
#if DEBUG
                verifyMethodInfoWasTaken mi
#endif
                let mi = mi.MakeGenericMethod([| retTy |])
                let args = [| box prefix; tail |]
                mi.Invoke(null, args)
            else
                System.Diagnostics.Debug.Assert(spec.IsStarPrecision || spec.IsStarWidth, "spec.IsStarPrecision || spec.IsStarWidth ")

                let mi = 
                    let n = if spec.IsStarWidth = spec.IsStarPrecision then 2 else 1
                    let prefix = if spec.TypeChar = '%' then "PercentStarChained" else "StarChained"
                    let name = prefix + (string n)
                    typeof<Specializations<'S, 'Re, 'Res>>.GetMethod(name, NonPublicStatics)
#if DEBUG                
                verifyMethodInfoWasTaken mi
#endif                
                let argTypes, args =
                    if spec.TypeChar = '%' then
                        [| retTy |], [| box prefix; tail |]
                    else
                        let argTy = argTys.[argTys.Length - 2]
                        let conv = getValueConverter argTy spec 
                        [| argTy; retTy |], [| box prefix; box conv; tail |]
                
                let mi = mi.MakeGenericMethod argTypes
                mi.Invoke(null, args)
            
        let buildSpecialFinal(spec: FormatSpecifier, argTys: Type[], prefix: string, suffix: string) =
            if spec.TypeChar = 'a' then
                let mi = typeof<Specializations<'S, 'Re, 'Res>>.GetMethod("LittleAFinal", NonPublicStatics)
#if DEBUG
                verifyMethodInfoWasTaken mi
#endif
                let mi = mi.MakeGenericMethod(argTys.[1] : Type)
                let args = [| box prefix; box suffix |]
                mi.Invoke(null, args)
            elif spec.TypeChar = 't' then
                let mi = typeof<Specializations<'S, 'Re, 'Res>>.GetMethod("TFinal", NonPublicStatics)
#if DEBUG
                verifyMethodInfoWasTaken mi
#endif
                let args = [| box prefix; box suffix |]
                mi.Invoke(null, args)
            else
                System.Diagnostics.Debug.Assert(spec.IsStarPrecision || spec.IsStarWidth, "spec.IsStarPrecision || spec.IsStarWidth ")

                let mi = 
                    let n = if spec.IsStarWidth = spec.IsStarPrecision then 2 else 1
                    let prefix = if spec.TypeChar = '%' then "PercentStarFinal" else "StarFinal"
                    let name = prefix + (string n)
                    typeof<Specializations<'S, 'Re, 'Res>>.GetMethod(name, NonPublicStatics)
#if DEBUG
                verifyMethodInfoWasTaken mi
#endif

                let mi, args = 
                    if spec.TypeChar = '%' then 
                        mi, [| box prefix; box suffix  |]
                    else
                        let argTy = argTys.[argTys.Length - 2]
                        let mi = mi.MakeGenericMethod argTy
                        let conv = getValueConverter argTy spec 
                        mi, [| box prefix; box conv; box suffix  |]

                mi.Invoke(null, args)

        let buildPlainFinal(args: obj[], argTypes: Type[]) =
            let argsCount = args.Length
            let methodName,args =
                if argsCount > 0 && args.[0].ToString() = "" then
                    if argsCount > 1 && args.[argsCount - 1].ToString() = "" then
                        let args = Array.sub args 1 (argsCount - 2)
                        optimizedArgCount <- optimizedArgCount + 2
                        "FinalFast", args
                    else
                        optimizedArgCount <- optimizedArgCount + 1
                        "FinalFastStart", args |> Array.skip 1
                elif argsCount > 0 && args.[argsCount - 1].ToString() = "" then
                    let args = Array.sub args 0 (argsCount - 1)
                    optimizedArgCount <- optimizedArgCount + 1
                    "FinalFastEnd", args
                else
                    "Final",args

            let mi = typeof<Specializations<'S, 'Re, 'Res>>.GetMethod(methodName + argTypes.Length.ToString(), NonPublicStatics)
#if DEBUG
            verifyMethodInfoWasTaken mi
#endif
            let mi = mi.MakeGenericMethod argTypes
            mi.Invoke(null, args)
    
        let buildPlainChained(args: obj[], argTypes: Type[]) =
            let argsCount = args.Length
            let methodName,args =
                if argsCount > 0 && args.[0].ToString() = "" then
                    optimizedArgCount <- optimizedArgCount + 1
                    "ChainedFastStart", args |> Array.skip 1
                else
                    "Chained", args

            let mi = typeof<Specializations<'S, 'Re, 'Res>>.GetMethod(methodName + (argTypes.Length - 1).ToString(), NonPublicStatics)
#if DEBUG
            verifyMethodInfoWasTaken mi
#endif
            let mi = mi.MakeGenericMethod argTypes
            mi.Invoke(null, args)

        let builderStack = PrintfBuilderStack()

        let ContinuationOnStack = -1

        let buildPlain numberOfArgs prefix = 
            let n = numberOfArgs * 2
            let hasCont = builderStack.HasContinuationOnStack numberOfArgs

            let extra = if hasCont then 1 else 0
            let plainArgs, plainTypes = 
                builderStack.GetArgumentAndTypesAsArrays(n + 1, 1, n, (numberOfArgs + extra), 0, numberOfArgs)

            plainArgs.[0] <- box prefix

            if hasCont then
                let cont, contTy = builderStack.PopContinuationWithType()
                plainArgs.[plainArgs.Length - 1] <- cont
                plainTypes.[plainTypes.Length - 1] <- contTy

                buildPlainChained(plainArgs, plainTypes)
            else
                buildPlainFinal(plainArgs, plainTypes)

        let rec parseFromFormatSpecifier (prefix: string) (s: string) (funcTy: Type) i: int = 
            
            if i >= s.Length then 0
            else
            
            System.Diagnostics.Debug.Assert(s.[i] = '%', "s.[i] = '%'")
            count <- count + 1

            let flags, i = FormatString.parseFlags s (i + 1)
            let width, i = FormatString.parseWidth s i
            let precision, i = FormatString.parsePrecision s i
            let typeChar, i = FormatString.parseTypeChar s i
            let spec = { TypeChar = typeChar; Precision = precision; Flags = flags; Width = width}
            
            let next, suffix = FormatString.findNextFormatSpecifier s i

            let argTys = 
                let n = 
                    if spec.TypeChar = 'a' then 2 
                    elif spec.IsStarWidth || spec.IsStarPrecision then
                        if spec.IsStarWidth = spec.IsStarPrecision then 3 
                        else 2
                    else 1

                let n = if spec.TypeChar = '%' then n - 1 else n
                
                System.Diagnostics.Debug.Assert(n <> 0, "n <> 0")

                extractCurriedArguments funcTy n

            let retTy = argTys.[argTys.Length - 1]

            let numberOfArgs = parseFromFormatSpecifier suffix s retTy next

            if spec.TypeChar = 'a' || spec.TypeChar = 't' || spec.IsStarWidth || spec.IsStarPrecision then
                if numberOfArgs = ContinuationOnStack then

                    let cont, contTy = builderStack.PopContinuationWithType()
                    let currentCont = buildSpecialChained(spec, argTys, prefix, cont, contTy)
                    builderStack.PushContinuationWithType(currentCont, funcTy)

                    ContinuationOnStack
                else
                    if numberOfArgs = 0 then
                        System.Diagnostics.Debug.Assert(builderStack.IsEmpty, "builderStack.IsEmpty")

                        let currentCont = buildSpecialFinal(spec, argTys, prefix, suffix)
                        builderStack.PushContinuationWithType(currentCont, funcTy)
                        ContinuationOnStack
                    else
                        let hasCont = builderStack.HasContinuationOnStack numberOfArgs
                        
                        let expectedNumberOfItemsOnStack = numberOfArgs * 2
                        let sizeOfTypesArray = 
                            if hasCont then numberOfArgs + 1
                            else numberOfArgs
                                                
                        let plainArgs, plainTypes = 
                            builderStack.GetArgumentAndTypesAsArrays(expectedNumberOfItemsOnStack + 1, 1, expectedNumberOfItemsOnStack, sizeOfTypesArray, 0, numberOfArgs )

                        plainArgs.[0] <- box suffix

                        let next =
                            if hasCont then
                                let nextCont, nextContTy = builderStack.PopContinuationWithType()
                                plainArgs.[plainArgs.Length - 1] <- nextCont
                                plainTypes.[plainTypes.Length - 1] <- nextContTy
                                buildPlainChained(plainArgs, plainTypes)
                            else
                                buildPlainFinal(plainArgs, plainTypes)
                            
                        let next = buildSpecialChained(spec, argTys, prefix, next, retTy)
                        builderStack.PushContinuationWithType(next, funcTy)

                        ContinuationOnStack
            else
                if numberOfArgs = ContinuationOnStack then
                    let idx = argTys.Length - 2
                    builderStack.PushArgument suffix
                    builderStack.PushArgumentWithType((getValueConverter argTys.[idx] spec), argTys.[idx])
                    1
                else
                    builderStack.PushArgument suffix
                    builderStack.PushArgumentWithType((getValueConverter argTys.[0] spec), argTys.[0])
                    
                    if numberOfArgs = MaxArgumentsInSpecialization - 1 then
                        let cont = buildPlain (numberOfArgs + 1) prefix
                        builderStack.PushContinuationWithType(cont, funcTy)
                        ContinuationOnStack
                    else 
                        numberOfArgs + 1

        let parseFormatString (s: string) (funcTy: System.Type) : obj = 
            optimizedArgCount <- 0
            let prefixPos, prefix = FormatString.findNextFormatSpecifier s 0
            if prefixPos = s.Length then 
                box (fun (env: unit -> PrintfEnv<'S, 'Re, 'Res>) -> 
                    let env = env()
                    env.Write prefix
                    env.Finish()
                    )
            else
                let n = parseFromFormatSpecifier prefix s funcTy prefixPos
                
                if n = ContinuationOnStack || n = 0 then
                    builderStack.PopValueUnsafe()
                else
                    buildPlain n prefix

        member __.Build<'T>(s: string) : PrintfFactory<'S, 'Re, 'Res, 'T> * int = 
            parseFormatString s typeof<'T> :?> _, (2 * count + 1) - optimizedArgCount // second component is used in SprintfEnv as value for internal buffer

    /// Type of element that is stored in cache 
    /// Pair: factory for the printer + number of text blocks that printer will produce (used to preallocate buffers)
    type CachedItem<'T, 'State, 'Residue, 'Result> = PrintfFactory<'State, 'Residue, 'Result, 'T> * int

    /// 2-level cache.
    /// 1st-level stores last value that was consumed by the current thread in thread-static field thus providing shortcuts for scenarios when 
    /// printf is called in tight loop
    /// 2nd level is global dictionary that maps format string to the corresponding PrintfFactory
    type Cache<'T, 'State, 'Residue, 'Result>() =
        static let generate fmt = PrintfBuilder<'State, 'Residue, 'Result>().Build<'T>(fmt)        
        static let mutable map = System.Collections.Concurrent.ConcurrentDictionary<string, CachedItem<'T, 'State, 'Residue, 'Result>>()
        static let getOrAddFunc = Func<_, _>(generate)
        static let get (key: string) = map.GetOrAdd(key, getOrAddFunc)

        [<DefaultValue>]
        [<ThreadStatic>]
        static val mutable private last: string * CachedItem<'T, 'State, 'Residue, 'Result>
    
        static member Get(key: Format<'T, 'State, 'Residue, 'Result>) =
            if not (Cache<'T, 'State, 'Residue, 'Result>.last === null) 
                && key.Value.Equals (fst Cache<'T, 'State, 'Residue, 'Result>.last) then
                    snd Cache<'T, 'State, 'Residue, 'Result>.last
            else
                let v = get key.Value
                Cache<'T, 'State, 'Residue, 'Result>.last <- (key.Value, v)
                v

    type StringPrintfEnv<'Result>(k, n) = 
        inherit PrintfEnv<unit, string, 'Result>(())

        let buf: string[] = Array.zeroCreate n
        let mutable ptr = 0

        override __.Finish() : 'Result = k (String.Concat buf)
        override __.Write(s: string) = 
            buf.[ptr] <- s
            ptr <- ptr + 1
        override __.WriteT s =
            buf.[ptr] <- s
            ptr <- ptr + 1

    type SmallStringPrintfEnv<'Result>(k) = 
        inherit PrintfEnv<unit, string, 'Result>(())
        
        let mutable c = null

        override __.Finish() : 'Result = k c
        override __.Write(s: string) = if isNull c then c <- s else c <- c + s
        override __.WriteT s = if isNull c then c <- s else c <- c + s

    type StringBuilderPrintfEnv<'Result>(k, buf) = 
        inherit PrintfEnv<Text.StringBuilder, unit, 'Result>(buf)
        override __.Finish() : 'Result = k ()
        override __.Write(s: string) = ignore(buf.Append s)
        override __.WriteT(()) = ()

    type TextWriterPrintfEnv<'Result>(k, tw: IO.TextWriter) =
        inherit PrintfEnv<IO.TextWriter, unit, 'Result>(tw)
        override __.Finish() : 'Result = k()
        override __.Write(s: string) = tw.Write s
        override __.WriteT(()) = ()
    
    let inline doPrintf fmt f = 
        let formatter, n = Cache<_, _, _, _>.Get fmt
        let env() = f n
        formatter env

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Printf =

    open System
    open System.IO
    open System.Text
    open PrintfImpl

    type BuilderFormat<'T,'Result>    = Format<'T, StringBuilder, unit, 'Result>
    type StringFormat<'T,'Result>     = Format<'T, unit, string, 'Result>
    type TextWriterFormat<'T,'Result> = Format<'T, TextWriter, unit, 'Result>
    type BuilderFormat<'T>     = BuilderFormat<'T,unit>
    type StringFormat<'T>      = StringFormat<'T,string>
    type TextWriterFormat<'T>  = TextWriterFormat<'T,unit>

    [<CompiledName("PrintFormatToStringThen")>]
    let ksprintf continuation (format: StringFormat<'T, 'Result>) : 'T = 
        doPrintf format (fun n ->
            if n <= 2 then
                SmallStringPrintfEnv continuation :> PrintfEnv<_, _, _>
            else
                StringPrintfEnv(continuation, n) :> PrintfEnv<_, _, _>
        )

    [<CompiledName("PrintFormatToStringThen")>]
    let sprintf (format: StringFormat<'T>) =
        doPrintf format (fun n ->
            if n <= 2 then
                SmallStringPrintfEnv id :> PrintfEnv<_, _, _>
            else
                StringPrintfEnv(id, n) :> PrintfEnv<_, _, _>
        )

    [<CompiledName("PrintFormatThen")>]
    let kprintf continuation format = ksprintf continuation format

    [<CompiledName("PrintFormatToStringBuilderThen")>]
    let kbprintf continuation (builder: StringBuilder) format = 
        doPrintf format (fun _ -> 
            StringBuilderPrintfEnv(continuation, builder) :> PrintfEnv<_, _, _> 
        )
    
    [<CompiledName("PrintFormatToTextWriterThen")>]
    let kfprintf continuation textWriter format =
        doPrintf format (fun _ -> 
            TextWriterPrintfEnv(continuation, textWriter) :> PrintfEnv<_, _, _>
        )

    [<CompiledName("PrintFormatToStringBuilder")>]
    let bprintf builder format  = kbprintf ignore builder format 

    [<CompiledName("PrintFormatToTextWriter")>]
    let fprintf (textWriter: TextWriter) format  = kfprintf ignore textWriter format 

    [<CompiledName("PrintFormatLineToTextWriter")>]
    let fprintfn (textWriter: TextWriter) format  = kfprintf (fun _ -> textWriter.WriteLine()) textWriter format

    [<CompiledName("PrintFormatToStringThenFail")>]
    let failwithf format = ksprintf failwith format

#if !FX_NO_SYSTEM_CONSOLE
#if EXTRAS_FOR_SILVERLIGHT_COMPILER
    [<CompiledName("PrintFormat")>]
    let printf format = fprintf (!outWriter) format

    [<CompiledName("PrintFormatToError")>]
    let eprintf format = fprintf (!errorWriter) format

    [<CompiledName("PrintFormatLine")>]
    let printfn format = fprintfn (!outWriter) format

    [<CompiledName("PrintFormatLineToError")>]
    let eprintfn format = fprintfn (!errorWriter) format
#else
    [<CompiledName("PrintFormat")>]
    let printf format = fprintf Console.Out format

    [<CompiledName("PrintFormatToError")>]
    let eprintf format = fprintf Console.Error format

    [<CompiledName("PrintFormatLine")>]
    let printfn format = fprintfn Console.Out format

    [<CompiledName("PrintFormatLineToError")>]
    let eprintfn format = fprintfn Console.Error format
#endif
#endif 
