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
    open System.Collections.Concurrent
    open System.Reflection
    open Microsoft.FSharp.Core
    open Microsoft.FSharp.Core.Operators
    open Microsoft.FSharp.Collections
    open LanguagePrimitives.IntrinsicOperators
    
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
            InteropHoleDotNetFormat: string option
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

        let parseInteropHoleDotNetFormat typeChar (s: string) i =
            if typeChar = 'P' then 
                if i < s.Length && s.[i] = '(' then  
                     let i2 = s.IndexOf(")", i)
                     if i2 = -1 then 
                         None, i
                     else 
                         Some s.[i+1..i2-1], i2+1
                else
                    None, i
            else
                None, i

        // Skip %P() added for hole in "...%d{x}..."
        let skipInterpolationHole isInterpolatedString (s:string) i =
            if isInterpolatedString && i+3 < s.Length && 
               s.[i] = '%' &&
               s.[i+1] = 'P' &&
               s.[i+2] = '(' &&
               s.[i+3] = ')'  then i+4
            else i
    
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
    type PrintfEnv<'State, 'Residue, 'Result>(state: 'State) =
        member _.State = state

        abstract Finish: unit -> 'Result

        abstract Write: string -> unit
        
        /// Write a captured interpolation value
        abstract CaptureInterpoland: obj -> unit
        
        /// Write the result of a '%t' format.  If this is a string it is written. If it is a 'unit' value
        /// the side effect has already happened
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
    
    /// Type of results produced by specialization.
    ///
    /// This is a function that accepts a thunk to create PrintfEnv on demand (at the very last
    /// appliction of an argument) and returns a concrete instance of an appriate curried printer.
    ///
    /// After all arguments are collected, specialization obtains concrete PrintfEnv from the thunk
    /// and uses it to output collected data.
    type PrintfFactory<'Printer, 'State, 'Residue, 'Result> = (unit -> PrintfEnv<'State, 'Residue, 'Result>) -> 'Printer

    [<Literal>]
    let MaxArgumentsInSpecialization = 5

    /// Specializations are created via factory methods. These methods accepts 2 kinds of arguments
    /// - parts of format string that corresponds to raw text
    /// - functions that can transform collected values to strings
    /// basic shape of the signature of specialization
    /// <prefix-string> + <converter for arg1> + <suffix that comes after arg1> + ... <converter for arg-N> + <suffix that comes after arg-N>
    type Specializations<'State, 'Residue, 'Result> =
     
        /// <prefix-string> + <converter for arg1> + <suffix-string>
        static member FinalInterpoland1<'A>(s0, s1) =
            //Console.WriteLine("FinalInterpoland1 (build part 0)") // TODO remove me
            (fun (prev: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                //Console.WriteLine("FinalInterpoland1 (build part1)") // TODO remove me
                (fun (arg1: 'A) ->
                    let env = prev()
                    //Console.WriteLine("FinalInterpoland1 (execute): arg1 = {0}", arg1) // TODO remove me
                    env.Write(s0)
                    env.CaptureInterpoland(box arg1)
                    env.Write(s1)
                    env.Finish()
                )
            )
        /// <prefix-string> + <converter for arg1> + <suffix-string>
        static member Final1<'A>(s0, conv1, s1) =
            (fun (prev: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (arg1: 'A) ->
                    let env = prev()
                    Utils.Write(env, s0, conv1 arg1, s1)
                    env.Finish()
                )
            )

        /// <prefix-string> + <converter for arg1>
        static member FinalNoSuffix1<'A>(s0, conv1) =
            (fun (prev: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (arg1: 'A) ->
                    let env = prev()
                    Utils.Write(env, s0, conv1 arg1)
                    env.Finish()
                )
            )

        /// <converter for arg1> + <suffix-string>
        static member FinalNoPrefix1<'A>(conv1, s1) =
            (fun (prev: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (arg1: 'A) ->
                    let env = prev()
                    Utils.Write(env, conv1 arg1, s1)
                    env.Finish()
                )
            )

        static member FinalNoPrefixOrSuffix1<'A>(conv1) =
            (fun (prev: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (arg1: 'A) ->
                    let env = prev()
                    env.Write (conv1 arg1)
                    env.Finish()
                )
            )

        static member Final2<'A, 'B>(s0, conv1, s1, conv2, s2) =
            (fun (prev: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (a: 'A) (b: 'B) ->
                    let env = prev()
                    Utils.Write(env, s0, conv1 a, s1, conv2 b, s2)
                    env.Finish()
                )
            )

        static member FinalNoSuffix2<'A, 'B>(s0, conv1, s1, conv2) =
            (fun (prev: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (arg1: 'A) (arg2: 'B) ->
                    let env = prev()
                    Utils.Write(env, s0, conv1 arg1, s1, conv2 arg2)
                    env.Finish()
                )
            )

        static member FinalNoPrefix2<'A, 'B>(conv1, s1, conv2, s2) =
            (fun (prev: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (arg1: 'A) (arg2: 'B) ->
                    let env = prev()
                    Utils.Write(env, conv1 arg1, s1, conv2 arg2, s2)
                    env.Finish()
                )
            )

        static member FinalNoPrefixOrSuffix2<'A, 'B>(conv1, s1, conv2) =
            (fun (prev: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (a: 'A) (b: 'B) ->
                    let env = prev()
                    Utils.Write(env, conv1 a, s1, conv2 b)
                    env.Finish()
                )
            )

        static member Final3<'A, 'B, 'C>(s0, conv1, s1, conv2, s2, conv3, s3) =
            (fun (prev: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (arg1: 'A) (arg2: 'B) (arg3: 'C) ->
                    let env = prev()
                    Utils.Write(env, s0, conv1 arg1, s1, conv2 arg2, s2, conv3 arg3, s3)
                    env.Finish()
                )
            )

        static member FinalNoSuffix3<'A, 'B, 'C>(s0, conv1, s1, conv2, s2, conv3) =
            (fun (prev: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (arg1: 'A) (arg2: 'B) (arg3: 'C) ->
                    let env = prev()
                    Utils.Write(env, s0, conv1 arg1, s1, conv2 arg2, s2, conv3 arg3)
                    env.Finish()
                )
            )

        static member FinalNoPrefix3<'A, 'B, 'C>(conv1, s1, conv2, s2, conv3, s3) =
            (fun (prev: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (arg1: 'A) (arg2: 'B) (arg3: 'C) ->
                    let env = prev()
                    Utils.Write(env, conv1 arg1, s1, conv2 arg2, s2, conv3 arg3, s3)
                    env.Finish()
                )
            )

        static member FinalNoPrefixOrSuffix3<'A, 'B, 'C>(conv1, s1, conv2, s2, conv3) =
            (fun (prev: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (arg1: 'A) (arg2: 'B) (arg3: 'C) ->
                    let env = prev()
                    Utils.Write(env, conv1 arg1, s1, conv2 arg2, s2, conv3 arg3)
                    env.Finish()
                )
            )

        static member Final4<'A, 'B, 'C, 'D>(s0, conv1, s1, conv2, s2, conv3, s3, conv4, s4) =
            (fun (prev: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (arg1: 'A) (arg2: 'B) (arg3: 'C) (arg4: 'D)->
                    let env = prev()
                    Utils.Write(env, s0, conv1 arg1, s1, conv2 arg2, s2, conv3 arg3, s3, conv4 arg4, s4)
                    env.Finish()
                )
            )

        static member FinalNoSuffix4<'A, 'B, 'C, 'D>(s0, conv1, s1, conv2, s2, conv3, s3, conv4) =
            (fun (prev: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (arg1: 'A) (arg2: 'B) (arg3: 'C) (arg4: 'D)->
                    let env = prev()
                    Utils.Write(env, s0, conv1 arg1, s1, conv2 arg2, s2, conv3 arg3, s3, conv4 arg4)
                    env.Finish()
                )
            )

        static member FinalNoPrefix4<'A, 'B, 'C, 'D>(conv1, s1, conv2, s2, conv3, s3, conv4, s4) =
            (fun (prev: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (arg1: 'A) (arg2: 'B) (arg3: 'C) (arg4: 'D)->
                    let env = prev()
                    Utils.Write(env, conv1 arg1, s1, conv2 arg2, s2, conv3 arg3, s3, conv4 arg4, s4)
                    env.Finish()
                )
            )

        static member FinalNoPrefixOrSuffix4<'A, 'B, 'C, 'D>(conv1, s1, conv2, s2, conv3, s3, conv4) =
            (fun (prev: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (arg1: 'A) (arg2: 'B) (arg3: 'C) (arg4: 'D)->
                    let env = prev()
                    Utils.Write(env, conv1 arg1, s1, conv2 arg2, s2, conv3 arg3, s3, conv4 arg4)
                    env.Finish()
                )
            )

        static member Final5<'A, 'B, 'C, 'D, 'E>(s0, conv1, s1, conv2, s2, conv3, s3, conv4, s4, conv5, s5) =
            (fun (prev: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (arg1: 'A) (arg2: 'B) (arg3: 'C) (arg4: 'D) (arg5: 'E)->
                    let env = prev()
                    Utils.Write(env, s0, conv1 arg1, s1, conv2 arg2, s2, conv3 arg3, s3, conv4 arg4, s4, conv5 arg5, s5)
                    env.Finish()
                )
            )

        static member FinalNoSuffix5<'A, 'B, 'C, 'D, 'E>(s0, conv1, s1, conv2, s2, conv3, s3, conv4, s4, conv5) =
            (fun (prev: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (arg1: 'A) (arg2: 'B) (arg3: 'C) (arg4: 'D) (arg5: 'E)->
                    let env = prev()
                    Utils.Write(env, s0, conv1 arg1, s1, conv2 arg2, s2, conv3 arg3, s3, conv4 arg4, s4, conv5 arg5)
                    env.Finish()
                )
            )

        static member FinalNoPrefix5<'A, 'B, 'C, 'D, 'E>(conv1, s1, conv2, s2, conv3, s3, conv4, s4, conv5, s5) =
            (fun (prev: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (arg1: 'A) (arg2: 'B) (arg3: 'C) (arg4: 'D) (arg5: 'E)->
                    let env = prev()
                    Utils.Write(env, conv1 arg1, s1, conv2 arg2, s2, conv3 arg3, s3, conv4 arg4, s4, conv5 arg5, s5)
                    env.Finish()
                )
            )

        static member FinalNoPrefixOrSuffix5<'A, 'B, 'C, 'D, 'E>(conv1, s1, conv2, s2, conv3, s3, conv4, s4, conv5) =
            (fun (prev: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (arg1: 'A) (arg2: 'B) (arg3: 'C) (arg4: 'D) (arg5: 'E)->
                    let env = prev()
                    Utils.Write(env, conv1 arg1, s1, conv2 arg2, s2, conv3 arg3, s3, conv4 arg4, s4, conv5 arg5)
                    env.Finish()
                )
            )

        static member Chained1<'A, 'Tail>(s0, conv1, next) =
            (fun (prev: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (arg1: 'A) ->
                    let env() = 
                        let env = prev()
                        Utils.Write(env, s0, conv1 arg1)
                        env
                    next env : 'Tail
                )
            )

        static member ChainedInterpoland1<'A, 'Tail>(s0, next) =
            //Console.WriteLine("ChainedInterpoland1 (build part 0)") // TODO remove me
            (fun (prev: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                //Console.WriteLine("ChainedInterpoland1 (build part 1)") // TODO remove me
                (fun (arg1: 'A) ->
                    //Console.WriteLine("ChainedInterpoland1 (arg capture), arg1 = {0}", arg1) // TODO remove me
                    let env() = 
                        let env = prev()
                        //Console.WriteLine("ChainedInterpoland1 (execute), arg1 = {0}", arg1) // TODO remove me
                        env.Write(s0)
                        env.CaptureInterpoland(arg1)
                        env
                    next env : 'Tail
                )
            )

        static member ChainedNoPrefix1<'A, 'Tail>(conv1, next) =
            (fun (prev: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (arg1: 'A) ->
                    let curr() = 
                        let env = prev()
                        env.Write(conv1 arg1)
                        env
                    next curr : 'Tail
                )
            )

        static member Chained2<'A, 'B, 'Tail>(s0, conv1, s1, conv2, next) =
            (fun (prev: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (arg1: 'A) (arg2: 'B) ->
                    let curr() = 
                        let env = prev()
                        Utils.Write(env, s0, conv1 arg1, s1, conv2 arg2)
                        env
                    next curr : 'Tail
                )
            )

        static member ChainedNoPrefix2<'A, 'B, 'Tail>(conv1, s1, conv2, next) =
            (fun (prev: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (arg1: 'A) (arg2: 'B) ->
                    let curr() = 
                        let env = prev()
                        Utils.Write(env, conv1 arg1, s1, conv2 arg2)
                        env
                    next curr : 'Tail
                )
            )

        static member Chained3<'A, 'B, 'C, 'Tail> (s0, conv1, s1, conv2, s2, conv3, next) =
            (fun (prev: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (arg1: 'A) (arg2: 'B) (arg3: 'C) ->
                    let curr() = 
                        let env = prev()
                        Utils.Write(env, s0, conv1 arg1, s1, conv2 arg2, s2, conv3 arg3)
                        env
                    next curr : 'Tail
                )
            )

        static member ChainedNoPrefix3<'A, 'B, 'C, 'Tail> (conv1, s1, conv2, s2, conv3, next) =
            (fun (prev: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (arg1: 'A) (arg2: 'B) (arg3: 'C) ->
                    let curr() = 
                        let env = prev()
                        Utils.Write(env, conv1 arg1, s1, conv2 arg2, s2, conv3 arg3)
                        env
                    next curr : 'Tail
                )
            )

        static member Chained4<'A, 'B, 'C, 'D, 'Tail> (s0, conv1, s1, conv2, s2, conv3, s3, conv4, next) =
            (fun (prev: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (arg1: 'A) (arg2: 'B) (arg3: 'C) (arg4: 'D)->
                    let curr() = 
                        let env = prev()
                        Utils.Write(env, s0, conv1 arg1, s1, conv2 arg2, s2, conv3 arg3, s3, conv4 arg4)
                        env
                    next curr : 'Tail
                )
            )

        static member ChainedNoPrefix4<'A, 'B, 'C, 'D, 'Tail> (conv1, s1, conv2, s2, conv3, s3, conv4, next) =
            (fun (prev: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (arg1: 'A) (arg2: 'B) (arg3: 'C) (arg4: 'D)->
                    let curr() = 
                        let env = prev()
                        Utils.Write(env, conv1 arg1, s1, conv2 arg2, s2, conv3 arg3, s3, conv4 arg4)
                        env
                    next curr : 'Tail
                )
            )

        static member Chained5<'A, 'B, 'C, 'D, 'E, 'Tail> (s0, conv1, s1, conv2, s2, conv3, s3, conv4, s4, conv5, next) =
            (fun (prev: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (arg1: 'A) (arg2: 'B) (arg3: 'C) (arg4: 'D) (arg5: 'E)->
                    let curr() = 
                        let env = prev()
                        Utils.Write(env, s0, conv1 arg1, s1, conv2 arg2, s2, conv3 arg3, s3, conv4 arg4, s4, conv5 arg5)
                        env
                    next curr : 'Tail
                )
            )

        static member ChainedNoPrefix5<'A, 'B, 'C, 'D, 'E, 'Tail> (conv1, s1, conv2, s2, conv3, s3, conv4, s4, conv5, next) =
            (fun (prev: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (arg1: 'A) (arg2: 'B) (arg3: 'C) (arg4: 'D) (arg5: 'E)->
                    let env() = 
                        let env = prev()
                        Utils.Write(env, conv1 arg1, s1, conv2 arg2, s2, conv3 arg3, s3, conv4 arg4, s4, conv5 arg5)
                        env
                    next env : 'Tail
                )
            )

        static member TFinal(s1: string, s2: string) = 
            (fun (prev: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (f: 'State -> 'Residue) -> 
                    let env = prev()
                    env.Write s1
                    env.WriteT(f env.State)
                    env.Write s2
                    env.Finish()
                )
            )
        static member TChained<'Tail>(s1: string, next: PrintfFactory<'Tail, 'State, 'Residue, 'Result>) = 
            (fun (prev: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (f: 'State -> 'Residue) -> 
                    let curr() = 
                        let env = prev()
                        env.Write s1
                        env.WriteT(f env.State)
                        env
                    next curr: 'Tail
                )
            )

        static member LittleAFinal<'A>(s1: string, s2: string) = 
            (fun (prev: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (f: 'State -> 'A ->'Residue) (a: 'A) -> 
                    let env = prev()
                    env.Write s1
                    env.WriteT(f env.State a)
                    env.Write s2
                    env.Finish()
                )
            )
        static member LittleAChained<'A, 'Tail>(s1: string, next: PrintfFactory<'Tail, 'State, 'Residue, 'Result>) = 
            (fun (prev: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (f: 'State -> 'A ->'Residue) (a: 'A) -> 
                    let curr() = 
                        let env = prev()
                        env.Write s1
                        env.WriteT(f env.State a)
                        env
                    next curr: 'Tail
                )
            )

        static member StarFinal1<'A>(s1: string, conv, s2: string) = 
            (fun (prev: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (star1: int) (a: 'A) -> 
                    let env = prev()
                    env.Write s1
                    env.Write (conv a star1: string)
                    env.Write s2
                    env.Finish()
                )
            )   
       
        static member PercentStarFinal1(s1: string, s2: string) = 
            (fun (prev: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (_star1 : int) -> 
                    let env = prev()
                    env.Write s1
                    env.Write("%")
                    env.Write s2
                    env.Finish()
                )
            )

        static member StarFinal2<'A>(s1: string, conv, s2: string) = 
            (fun (prev: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (star1: int) (star2: int) (arg1: 'A) -> 
                    let env = prev()
                    env.Write s1
                    env.Write (conv arg1 star1 star2: string)
                    env.Write s2
                    env.Finish()
                )
            )

        /// Handles case when '%*.*%' is used at the end of string
        static member PercentStarFinal2(s1: string, s2: string) = 
            (fun (prev: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (_star1 : int) (_star2 : int) -> 
                    let env = prev()
                    env.Write s1
                    env.Write("%")
                    env.Write s2
                    env.Finish()
                )
            )

        static member StarChained1<'A, 'Tail>(s1: string, conv, next: PrintfFactory<'Tail, 'State, 'Residue, 'Result>) = 
            (fun (prev: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (star1: int) (arg1: 'A) -> 
                    let curr() =
                        let env = prev()
                        env.Write s1
                        env.Write(conv arg1 star1 : string)
                        env
                    next curr : 'Tail
                )
            )
        
        /// Handles case when '%*%' is used in the middle of the string so it needs to be chained to another printing block
        static member PercentStarChained1<'Tail>(s1: string, next: PrintfFactory<'Tail, 'State, 'Residue, 'Result>) = 
            (fun (prev: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (_star1 : int) -> 
                    let curr() =
                        let env = prev()
                        env.Write s1
                        env.Write("%")
                        env
                    next curr: 'Tail
                )
            )

        static member StarChained2<'A, 'Tail>(s1: string, conv, next: PrintfFactory<'Tail, 'State, 'Residue, 'Result>) = 
            (fun (prev: unit -> PrintfEnv<'State, 'Residue, 'Result>) ->
                (fun (star1: int) (star2: int) (arg1: 'A) -> 
                    let curr() =
                        let env = prev()
                        env.Write s1
                        env.Write(conv arg1 star1 star2 : string)
                        env
                    next curr : 'Tail
                )
            )
        
        /// Handles case when '%*.*%' is used in the middle of the string so it needs to be chained to another printing block
        static member PercentStarChained2<'Tail>(s1: string, next: PrintfFactory<'Tail, 'State, 'Residue, 'Result>) = 
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

    /// A wrapper struct used to slightly strengthen the types of "ValueConverter" objects produced during composition of
    /// the dynamic implementation.  These are always functions but sometimes they take one argument, sometimes two.
    [<Struct; NoEquality; NoComparison>]
    type ValueConverter private (f: obj) =
        member x.FuncObj = f

        static member inline Make (f: 'T1 -> string) = ValueConverter(box f)
        static member inline Make (f: 'T1 -> 'T2 -> string) = ValueConverter(box f)
        static member inline Make (f: 'T1 -> 'T2 -> 'T3 -> string) = ValueConverter(box f)

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
        let inline adaptPaddedFormatted (spec: FormatSpecifier) getFormat (basic: string -> 'T -> string) (pad: string -> int -> 'T -> string) : ValueConverter =
            if spec.IsStarWidth then
                if spec.IsStarPrecision then
                    // width=*, prec=*
                    ValueConverter.Make (fun v width prec -> 
                        let fmt = getFormat (normalizePrecision prec)
                        pad fmt width v)
                else 
                    // width=*, prec=?
                    let prec = if spec.IsPrecisionSpecified then normalizePrecision spec.Precision else DefaultPrecision
                    let fmt = getFormat prec
                    ValueConverter.Make (fun v width -> 
                        pad fmt width v)

            elif spec.IsStarPrecision then
                if spec.IsWidthSpecified then
                    // width=val, prec=*
                    ValueConverter.Make (fun v prec -> 
                        let fmt = getFormat prec
                        pad fmt spec.Width v)
                else
                    // width=X, prec=*
                    ValueConverter.Make (fun v prec -> 
                        let fmt = getFormat prec
                        basic fmt v)                        
            else
                let prec = if spec.IsPrecisionSpecified then normalizePrecision spec.Precision else DefaultPrecision
                let fmt = getFormat prec
                if spec.IsWidthSpecified then
                    // width=val, prec=*
                    ValueConverter.Make (fun v -> 
                        pad fmt spec.Width v)
                else
                    // width=X, prec=*
                    ValueConverter.Make (fun v -> 
                        basic fmt v)

        /// pad here is function that converts T to string with respect of justification
        /// basic - function that converts T to string without applying justification rules
        /// adaptPadded returns boxed function that has various number of arguments depending on if width flags has '*' value 
        let inline adaptPadded (spec: FormatSpecifier) (basic: 'T -> string) (pad: int -> 'T -> string) : ValueConverter = 
            if spec.IsStarWidth then
                // width=*, prec=?
                ValueConverter.Make (fun v width -> 
                    pad width v)
            else
                if spec.IsWidthSpecified then
                    // width=val, prec=*
                    ValueConverter.Make (fun v -> 
                        pad spec.Width v)
                else
                    // width=X, prec=*
                    ValueConverter.Make (fun v -> 
                        basic v)

        let inline withPaddingFormatted (spec: FormatSpecifier) getFormat  (defaultFormat: string) (f: string ->  'T -> string) left right : ValueConverter =
            if not (spec.IsWidthSpecified || spec.IsPrecisionSpecified) then
                ValueConverter.Make (f defaultFormat)
            else
                if isLeftJustify spec.Flags then
                    adaptPaddedFormatted spec getFormat f left
                else
                    adaptPaddedFormatted spec getFormat f right

        let inline withPadding (spec: FormatSpecifier) (f: 'T -> string) left right : ValueConverter =
            if not spec.IsWidthSpecified then
                ValueConverter.Make f
            else
                if isLeftJustify spec.Flags then
                    adaptPadded spec f left
                else
                    adaptPadded  spec f right

    let inline isNumber (x: ^T) =
        not (^T: (static member IsPositiveInfinity: 'T -> bool) x) &&
        not (^T: (static member IsNegativeInfinity: 'T -> bool) x) &&
        not (^T: (static member IsNaN: 'T -> bool) x)

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

    let inline decimalWithPadding (spec: FormatSpecifier) getFormat defaultFormat f : ValueConverter =
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

        static member ObjectToString<'T>(spec: FormatSpecifier) : ValueConverter = 
            basicWithPadding spec (fun (v: 'T) ->
                match box v with
                | null -> "<null>"
                | x -> x.ToString())
        
        /// Convert an interpoland to a string
        static member InterpolandToString<'T>(spec: FormatSpecifier) : ValueConverter = 
            let fmt = 
                match spec.InteropHoleDotNetFormat with 
                | None -> null
                | Some fmt -> "{0:" + fmt + "}"
            basicWithPadding spec (fun (v: 'T) ->
                match box v with
                | null -> ""
                | x -> 
                    match fmt with 
                    | null -> x.ToString()
                    | fmt -> String.Format(fmt, x))
        
        static member GenericToStringCore(v: 'T, opts: Microsoft.FSharp.Text.StructuredPrintfImpl.FormatOptions, bindingFlags) = 
            let vty = 
                match box v with
                | null -> typeof<'T>
                | _ -> v.GetType()
            Microsoft.FSharp.Text.StructuredPrintfImpl.Display.anyToStringForPrintf opts bindingFlags (v, vty)

        static member GenericToString<'T>(spec: FormatSpecifier) : ValueConverter = 
            let bindingFlags = 
                if isPlusForPositives spec.Flags then BindingFlags.Public ||| BindingFlags.NonPublic
                else BindingFlags.Public 

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
                ValueConverter.Make (fun (v: 'T) (width: int) (prec: int) ->
                    let opts = { opts with PrintSize = prec }
                    let opts  = if not useZeroWidth then { opts with PrintWidth = width} else opts
                    ObjectPrinter.GenericToStringCore(v, opts, bindingFlags)
                    )

            | true, false ->
                ValueConverter.Make (fun (v: 'T) (width: int) ->
                    let opts  = if not useZeroWidth then { opts with PrintWidth = width} else opts
                    ObjectPrinter.GenericToStringCore(v, opts, bindingFlags))

            | false, true ->
                ValueConverter.Make (fun (v: 'T) (prec: int) ->
                    let opts = { opts with PrintSize = prec }
                    ObjectPrinter.GenericToStringCore(v, opts, bindingFlags) )

            | false, false ->
                ValueConverter.Make (fun (v: 'T) ->
                    ObjectPrinter.GenericToStringCore(v, opts, bindingFlags))
        
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

    let mi_GenericToString = typeof<ObjectPrinter>.GetMethod("GenericToString", NonPublicStatics)
    let mi_ObjectToString = typeof<ObjectPrinter>.GetMethod("ObjectToString", NonPublicStatics)
    let mi_InterpolandToString = typeof<ObjectPrinter>.GetMethod("InterpolandToString", NonPublicStatics)

    let private getValueConverter (ty: Type) (spec: FormatSpecifier) : ValueConverter = 
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
            let mi = mi_GenericToString.MakeGenericMethod ty
            mi.Invoke(null, [| box spec |]) |> unbox
        | 'O' -> 
            let mi = mi_ObjectToString.MakeGenericMethod ty
            mi.Invoke(null, [| box spec |]) |> unbox
        | 'P' -> 
            let mi = mi_InterpolandToString.MakeGenericMethod ty
            mi.Invoke(null, [| box spec |]) |> unbox
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
        // Note this 'obj' is an untagged union of type "string | value converter function | continuation function" 
        let args = Stack<obj> 10  
        let types = Stack<Type> 5

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

        member __.PopContinuation() = 
            System.Diagnostics.Debug.Assert(args.Count = 1, "args.Count = 1")
            System.Diagnostics.Debug.Assert(types.Count = 1, "types.Count = 1")
            
            let cont = args.Pop()
            let contTy = types.Pop()

            cont, contTy

        member __.PopValueUnsafe() = args.Pop()

        member __.PushString(value: string) =
            args.Push (box value)

        member __.PushValueConverter(value: ValueConverter, ty) =
            args.Push value.FuncObj
            types.Push ty

        member this.PushContinuation (cont: obj, contTy: Type) = 
            System.Diagnostics.Debug.Assert(this.IsEmpty, "this.IsEmpty")
            System.Diagnostics.Debug.Assert(
                (
                    let _arg, retTy = Microsoft.FSharp.Reflection.FSharpType.GetFunctionElements(cont.GetType())
                    contTy.IsAssignableFrom retTy
                ),
                "incorrect type"
                )

            args.Push cont
            types.Push contTy

        member __.HasContinuationOnStack expectedNumberOfArguments = 
            (types.Count = expectedNumberOfArguments + 1)

        member __.IsEmpty = 
            System.Diagnostics.Debug.Assert(args.Count = types.Count, "args.Count = types.Count")
            (args.Count = 0)

    /// Type of element that is stored in cache. This is the residue of the parse of the format string.
    ///
    /// Pair: factory for the printer + number of text blocks that printer will produce (used to preallocate buffers)
    [<NoComparison; NoEquality>]
    type CachedItem<'Printer, 'State, 'Residue, 'Result> =
        {
          /// The format string, used to help identify the cache entry (the cache index types are taken
          /// into account as well).
          FormatString: string

          /// The factory function used to generate the result or the resulting function.  it is passed an
          /// environment-creating function.
          FunctionFactory: PrintfFactory<'Printer, 'State, 'Residue, 'Result> 

          /// The format for the FormattableString, if we're building one of those
          FormattableStringFormat: string

          /// Ther number of holes in the FormattableString, if we're building one of those
          FormattableStringHoleCount: int

          /// The maximum number of slots needed in the environment including string fragments and output strings from position holders
          BlockCount: int
        }

    /// Parses format string and creates result printer factory function.
    ///
    /// First it recursively consumes format string up to the end, then during unwinding builds printer using PrintfBuilderStack as storage for arguments.
    ///
    /// The idea of implementation is very simple: every step can either push argument to the stack (if current block of 5 format specifiers is not yet filled) 
    //  or grab the content of stack, build intermediate printer and push it back to stack (so it can later be consumed by as argument) 
    type private FormatParser<'Printer, 'State, 'Residue, 'Result>(fmt: string, isInterpolatedString, isFormattableString) =
    
        let mutable count = 0
        let mutable optimizedArgCount = 0

        // If we're building a formattable string, we build the resulting format string during the first pass
        let ffmtb = if isFormattableString then StringBuilder() else null
        let mutable ffmtCount = 0

#if DEBUG
        let verifyMethodInfoWasTaken (mi: System.Reflection.MemberInfo) =
            if isNull mi then 
                ignore (System.Diagnostics.Debugger.Launch())
#endif
            
        let addFormattableFormatStringPlaceholder (spec: FormatSpecifier) =
            ffmtb.Append "{" |> ignore
            ffmtb.Append (string ffmtCount) |> ignore
            match spec.InteropHoleDotNetFormat with 
            | None -> ()
            | Some txt -> 
                ffmtb.Append ":" |> ignore
                ffmtb.Append txt |> ignore
            ffmtb.Append "}" |> ignore
            ffmtCount <- ffmtCount + 1

        let buildSpecialChained(spec: FormatSpecifier, argTys: Type[], prefix: string, tail: obj, retTy) = 
            if isFormattableString then
                let mi = typeof<Specializations<'State, 'Residue, 'Result>>.GetMethod("ChainedInterpoland1", NonPublicStatics)
#if DEBUG                
                verifyMethodInfoWasTaken mi
#endif                
                ffmtb.Append prefix |> ignore
                addFormattableFormatStringPlaceholder spec

                let argTy = argTys.[0]
                //Console.WriteLine("buildSpecialChained: argTy = {0}", argTy) // TODO remove me
                let mi = mi.MakeGenericMethod ([| argTy;  retTy |])
                let args = [| box prefix |]
                mi.Invoke(null, args)

            elif spec.TypeChar = 'a' then
                let mi = typeof<Specializations<'State, 'Residue, 'Result>>.GetMethod("LittleAChained", NonPublicStatics)
#if DEBUG
                verifyMethodInfoWasTaken mi
#endif

                let mi = mi.MakeGenericMethod([| argTys.[1];  retTy |])
                let args = [| box prefix; tail   |]
                mi.Invoke(null, args)
            elif spec.TypeChar = 't' then
                let mi = typeof<Specializations<'State, 'Residue, 'Result>>.GetMethod("TChained", NonPublicStatics)
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
                    typeof<Specializations<'State, 'Residue, 'Result>>.GetMethod(name, NonPublicStatics)
#if DEBUG                
                verifyMethodInfoWasTaken mi
#endif                
                let argTypes, args =
                    if spec.TypeChar = '%' then
                        [| retTy |], [| box prefix; tail |]
                    else
                        let argTy = argTys.[argTys.Length - 2]
                        let conv = getValueConverter argTy spec 
                        [| argTy; retTy |], [| box prefix; conv.FuncObj; tail |]
                
                let mi = mi.MakeGenericMethod argTypes
                mi.Invoke(null, args)
            
        let buildSpecialFinal(spec: FormatSpecifier, argTys: Type[], prefix: string, suffix: string) =
            if isFormattableString then
                // Every hole in a formattable string captures the interpoland
                let mi = typeof<Specializations<'State, 'Residue, 'Result>>.GetMethod("FinalInterpoland1", NonPublicStatics)
#if DEBUG                
                verifyMethodInfoWasTaken mi
#endif                
                ffmtb.Append prefix |> ignore
                addFormattableFormatStringPlaceholder spec
                ffmtb.Append suffix |> ignore

                let argTy = argTys.[0]
                //Console.WriteLine("buildSpecialFinal: argTy = {0}", argTy) // TODO remove me
                let mi = mi.MakeGenericMethod [| argTy |]
                let args = [| box prefix; box suffix  |]
                mi.Invoke(null, args)

            elif spec.TypeChar = 'a' then
                let mi = typeof<Specializations<'State, 'Residue, 'Result>>.GetMethod("LittleAFinal", NonPublicStatics)
#if DEBUG
                verifyMethodInfoWasTaken mi
#endif
                let mi = mi.MakeGenericMethod(argTys.[1] : Type)
                let args = [| box prefix; box suffix |]
                mi.Invoke(null, args)
            elif spec.TypeChar = 't' then
                let mi = typeof<Specializations<'State, 'Residue, 'Result>>.GetMethod("TFinal", NonPublicStatics)
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
                    typeof<Specializations<'State, 'Residue, 'Result>>.GetMethod(name, NonPublicStatics)
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
                        mi, [| box prefix; conv.FuncObj; box suffix  |]

                mi.Invoke(null, args)

        let argIsEmptyString (arg: obj) = 
            match arg with 
            | :? string as s -> String.IsNullOrEmpty(s)
            | _ -> false

        let buildPlainFinal(args: obj[], argTypes: Type[]) =
            let argsCount = args.Length
            let methodName,args =
                // check if the prefix is empty
                if argsCount > 0 && argIsEmptyString args.[0] then
                    // check if both prefix and suffix are empty
                    if argsCount > 1 && argIsEmptyString args.[argsCount - 1] then
                        let args = Array.sub args 1 (argsCount - 2)
                        optimizedArgCount <- optimizedArgCount + 2
                        "FinalNoPrefixOrSuffix", args
                    else
                        optimizedArgCount <- optimizedArgCount + 1
                        "FinalNoPrefix", args |> Array.skip 1
                // check if suffix is empty
                elif argsCount > 0 && argIsEmptyString args.[argsCount - 1] then
                    let args = Array.sub args 0 (argsCount - 1)
                    optimizedArgCount <- optimizedArgCount + 1
                    "FinalNoSuffix", args
                else
                    "Final",args

            let mi = typeof<Specializations<'State, 'Residue, 'Result>>.GetMethod(methodName + string argTypes.Length, NonPublicStatics)
#if DEBUG
            verifyMethodInfoWasTaken mi
#endif
            let mi = mi.MakeGenericMethod argTypes
            mi.Invoke(null, args)
    
        let buildPlainChained(args: obj[], argTypes: Type[]) =
            let argsCount = args.Length
            let methodName,args =
                // check if the prefix is empty
                if argsCount > 0 && argIsEmptyString args.[0] then
                    optimizedArgCount <- optimizedArgCount + 1
                    "ChainedNoPrefix", args |> Array.skip 1
                else
                    "Chained", args

            let mi = typeof<Specializations<'State, 'Residue, 'Result>>.GetMethod(methodName + string (argTypes.Length - 1), NonPublicStatics)
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
                builderStack.GetArgumentAndTypesAsArrays(n + 1, 1, n, numberOfArgs + extra, 0, numberOfArgs)

            plainArgs.[0] <- box prefix

            if hasCont then
                let cont, contTy = builderStack.PopContinuation()
                plainArgs.[plainArgs.Length - 1] <- cont
                plainTypes.[plainTypes.Length - 1] <- contTy

                buildPlainChained(plainArgs, plainTypes)
            else
                buildPlainFinal(plainArgs, plainTypes)

        let rec parseFromFormatSpecifier (prefix: string) (s: string) (funcTy: Type) i: int = 
            
            if i >= s.Length then 0 else
            
            System.Diagnostics.Debug.Assert(s.[i] = '%', "s.[i] = '%'")
            count <- count + 1

            let flags, i = FormatString.parseFlags s (i + 1)
            let width, i = FormatString.parseWidth s i
            let precision, i = FormatString.parsePrecision s i
            let typeChar, i = FormatString.parseTypeChar s i
            let interpHoleDotnetFormat, i = FormatString.parseInteropHoleDotNetFormat typeChar s i

            // Skip %P insertion points added after %d{...} etc. in interpolated strings
            let i = FormatString.skipInterpolationHole isInterpolatedString s i

            let spec =
                { TypeChar = typeChar
                  Precision = precision
                  Flags = flags
                  Width = width
                  InteropHoleDotNetFormat = interpHoleDotnetFormat }
            
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

            if isFormattableString || spec.TypeChar = 'a' || spec.TypeChar = 't' || spec.IsStarWidth || spec.IsStarPrecision then
                // Every hole in a formattable string captures the interpoland
                if numberOfArgs = ContinuationOnStack then

                    let cont, contTy = builderStack.PopContinuation()
                    let currentCont = buildSpecialChained(spec, argTys, prefix, cont, contTy)
                    builderStack.PushContinuation(currentCont, funcTy)

                    ContinuationOnStack
                else
                    if numberOfArgs = 0 then
                        System.Diagnostics.Debug.Assert(builderStack.IsEmpty, "builderStack.IsEmpty")

                        let currentCont = buildSpecialFinal(spec, argTys, prefix, suffix)
                        builderStack.PushContinuation(currentCont, funcTy)
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
                                let nextCont, nextContTy = builderStack.PopContinuation()
                                plainArgs.[plainArgs.Length - 1] <- nextCont
                                plainTypes.[plainTypes.Length - 1] <- nextContTy
                                buildPlainChained(plainArgs, plainTypes)
                            else
                                buildPlainFinal(plainArgs, plainTypes)
                            
                        let next = buildSpecialChained(spec, argTys, prefix, next, retTy)
                        builderStack.PushContinuation(next, funcTy)

                        ContinuationOnStack
            else
                if numberOfArgs = ContinuationOnStack then
                    let idx = argTys.Length - 2
                    builderStack.PushString suffix
                    let conv = getValueConverter argTys.[idx] spec
                    builderStack.PushValueConverter(conv, argTys.[idx])
                    1
                else
                    builderStack.PushString suffix
                    let conv = getValueConverter argTys.[0] spec
                    builderStack.PushValueConverter(conv, argTys.[0])
                    
                    if numberOfArgs = MaxArgumentsInSpecialization - 1 then
                        let cont = buildPlain (numberOfArgs + 1) prefix
                        builderStack.PushContinuation(cont, funcTy)
                        ContinuationOnStack
                    else 
                        numberOfArgs + 1

        let funcTy = typeof<'Printer>

        let factoryObj = 

            // Find the format specifier
            let prefixPos, prefix = FormatString.findNextFormatSpecifier fmt 0
            
            if prefixPos = fmt.Length then 
                if isFormattableString then 
                    ffmtb.Append prefix |> ignore

                // If there are not format specifiers then take a simple path
                box (fun (env: unit -> PrintfEnv<'State, 'Residue, 'Result>) -> 
                    let env = env()
                    env.Write prefix
                    env.Finish())
            else
                let n = parseFromFormatSpecifier prefix fmt funcTy prefixPos
                
                if n = ContinuationOnStack || n = 0 then
                    builderStack.PopValueUnsafe()
                else
                    buildPlain n prefix

        //do System.Console.WriteLine("factoryObj.GetType() = {0}", factoryObj.GetType())
        let result = 
            {
              FormatString = fmt
              FormattableStringFormat = (if isFormattableString then ffmtb.ToString() else null)
              FormattableStringHoleCount = ffmtCount
              FunctionFactory = factoryObj  :?> PrintfFactory<'Printer, 'State, 'Residue, 'Result>
              // second component is used in SprintfEnv as value for internal buffer
              BlockCount = (2 * count + 1) - optimizedArgCount 
            } 

        member _.Result = result

    /// 2-level cache.
    ///
    /// We can use the same caches for both interpolated and non-interpolated strings
    /// since interpolated strings contain %P and don't overlap with non-interpolation strings, and if an interpolated
    /// string doesn't contain %P then the processing of the format strings is semantically identical.
    type Cache<'Printer, 'State, 'Residue, 'Result>() =

        /// 1st level cache (type-indexed). Stores last value that was consumed by the current
        /// thread in thread-static field thus providing shortcuts for scenarios when printf is
        /// called in tight loop.
        [<DefaultValue; ThreadStatic>]
        static val mutable private mostRecent: CachedItem<'Printer, 'State, 'Residue, 'Result>
    
        // 2nd level cache (type-indexed). Dictionary that maps format string to the corresponding cache entry
        static let mutable dict : ConcurrentDictionary<string, CachedItem<'Printer, 'State, 'Residue, 'Result>> = null

        static member Get(format: Format<'Printer, 'State, 'Residue, 'Result>, isInterpolatedString, isFormattableString) =
            let cacheEntry = Cache<'Printer, 'State, 'Residue, 'Result>.mostRecent
            let fmt = format.Value
            if not (cacheEntry === null) && fmt.Equals cacheEntry.FormatString then 
                cacheEntry
            else
                // Initialize the 2nd level cache if necessary.  Note there's a race condition but it doesn't
                // matter if we initialize these values twice (and lose one entry)
                if isNull dict then 
                    dict <- ConcurrentDictionary<_,_>()

                let v = 
                    match dict.TryGetValue(fmt) with 
                    | true, res -> res
                    | _ -> 
                        let parser = FormatParser<'Printer, 'State, 'Residue, 'Result>(fmt, isInterpolatedString, isFormattableString)
                        let result = parser.Result
                        // Note there's a race condition but it doesn't matter if lose one entry
                        dict.TryAdd(fmt, result) |> ignore
                        result
                Cache<'Printer, 'State, 'Residue, 'Result>.mostRecent <- v
                v

    type LargeStringPrintfEnv<'Result>(k, n) = 
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

        override __.CaptureInterpoland(_s) = failwith "no interpolands expected" 

    type SmallStringPrintfEnv() = 
        inherit PrintfEnv<unit, string, string>(())
        let mutable c = null

        override __.Finish() : string = c
        override __.Write(s: string) = if isNull c then c <- s else c <- c + s
        override __.WriteT s = if isNull c then c <- s else c <- c + s
        override __.CaptureInterpoland(_s) = failwith "no interpolands expected" 

    let StringPrintfEnv n = 
        if n <= 2 then
            SmallStringPrintfEnv() :> PrintfEnv<_,_,_>
        else
            LargeStringPrintfEnv(id, n) :> PrintfEnv<_,_,_>

#if NETSTANDARD
    let FormattableStringPrintfEnv(ffmt: string, n) = 
        let args: obj[] = Array.zeroCreate n
        let mutable ptr = 0

        { new PrintfEnv<unit, string, FormattableString>(()) with

            override __.Finish() : FormattableString =
                //Console.WriteLine("FormattableStringPrintfEnv - fmt = {0}", ffmt)
                System.Runtime.CompilerServices.FormattableStringFactory.Create(ffmt, args)

            override __.Write(s: string) = ()

            override __.WriteT s = failwith "no %t formats  in FormattableString"

            override __.CaptureInterpoland (value) =
                //Console.WriteLine("FormattableStringPrintfEnv - CaptureInterpoland({0})", value)
                args.[ptr] <- value
                ptr <- ptr + 1
        }
#endif

    let StringBuilderPrintfEnv<'Result>(k, buf) = 
        { new PrintfEnv<Text.StringBuilder, unit, 'Result>(buf) with
            override __.Finish() : 'Result = k ()
            override __.Write(s: string) = ignore(buf.Append s)
            override __.WriteT(()) = ()
            override __.CaptureInterpoland(_s) = failwith "no interpolands expected" }

    let TextWriterPrintfEnv<'Result>(k, tw: IO.TextWriter) =
        { new PrintfEnv<IO.TextWriter, unit, 'Result>(tw) with 
            override __.Finish() : 'Result = k()
            override __.Write(s: string) = tw.Write s
            override __.WriteT(()) = ()
            override __.CaptureInterpoland(_s) = failwith "no interpolands expected" }

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Printf =

    open System
    open System.IO
    open System.Text
    open PrintfImpl

    type BuilderFormat<'T,'Result> = Format<'T, StringBuilder, unit, 'Result>
    type StringFormat<'T,'Result> = Format<'T, unit, string, 'Result>
    type TextWriterFormat<'T,'Result> = Format<'T, TextWriter, unit, 'Result>
    type BuilderFormat<'T> = BuilderFormat<'T,unit>
#if NETSTANDARD
    type FormattableStringFormat<'T> = StringFormat<'T,FormattableString>
#endif
    type StringFormat<'T> = StringFormat<'T,string>
    type TextWriterFormat<'T>  = TextWriterFormat<'T,unit>

    [<CompiledName("PrintFormatToStringThen")>]
    let ksprintf continuation (format: StringFormat<'T, 'Result>) : 'T = 
        let cacheItem = Cache.Get (format, false, false)
        let initial() = LargeStringPrintfEnv (continuation, cacheItem.BlockCount) :> PrintfEnv<_,_,_>
        cacheItem.FunctionFactory initial

    [<CompiledName("PrintFormatToStringThen")>]
    let sprintf (format: StringFormat<'T>) =
        let cacheItem = Cache.Get (format, false, false)
        let initial() = StringPrintfEnv cacheItem.BlockCount
        cacheItem.FunctionFactory initial

    [<CompiledName("InterpolatedPrintFormatToStringThen")>]
    [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
    let isprintf (format: StringFormat<'T>) = 
        let cacheItem = Cache.Get (format, true, false)
        let initial() = StringPrintfEnv cacheItem.BlockCount
        cacheItem.FunctionFactory initial

#if NETSTANDARD
    [<CompiledName("InterpolatedPrintFormatToFormattableStringThen")>]
    [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
    let ifsprintf (format: FormattableStringFormat<'T>) =
        let cacheItem = Cache.Get (format, true, true)
        let initial() = FormattableStringPrintfEnv (cacheItem.FormattableStringFormat, cacheItem.FormattableStringHoleCount)
        cacheItem.FunctionFactory initial
#endif

    [<CompiledName("PrintFormatThen")>]
    let kprintf continuation format = ksprintf continuation format

    [<CompiledName("PrintFormatToStringBuilderThen")>]
    let kbprintf continuation (builder: StringBuilder) format = 
        let cacheItem = Cache.Get (format, false, false)
        let initial() = StringBuilderPrintfEnv(continuation, builder)
        cacheItem.FunctionFactory initial
    
    [<CompiledName("PrintFormatToTextWriterThen")>]
    let kfprintf continuation textWriter format =
        let cacheItem = Cache.Get (format, false, false)
        let initial() = TextWriterPrintfEnv(continuation, textWriter)
        cacheItem.FunctionFactory initial

    [<CompiledName("PrintFormatToStringBuilder")>]
    let bprintf builder format =
        kbprintf ignore builder format 

    [<CompiledName("PrintFormatToTextWriter")>]
    let fprintf (textWriter: TextWriter) format =
        kfprintf ignore textWriter format 

    [<CompiledName("PrintFormatLineToTextWriter")>]
    let fprintfn (textWriter: TextWriter) format =
        kfprintf (fun _ -> textWriter.WriteLine()) textWriter format

    [<CompiledName("PrintFormatToStringThenFail")>]
    let failwithf format =
        ksprintf failwith format

    [<CompiledName("PrintFormat")>]
    let printf format =
        fprintf Console.Out format

    [<CompiledName("PrintFormatToError")>]
    let eprintf format =
        fprintf Console.Error format

    [<CompiledName("PrintFormatLine")>]
    let printfn format =
        fprintfn Console.Out format

    [<CompiledName("PrintFormatLineToError")>]
    let eprintfn format =
        fprintfn Console.Error format
