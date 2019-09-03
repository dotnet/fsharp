// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.Scripting

open System.Collections.Generic
open System.IO
open System.Text

type internal CapturedTextReader() =
    inherit TextReader()
    let queue = Queue<char>()
    member __.ProvideInput(text: string) =
        for c in text.ToCharArray() do
            queue.Enqueue(c)
    override __.Peek() =
        if queue.Count > 0 then queue.Peek() |> int
        else -1
    override __.Read() =
        if queue.Count > 0 then queue.Dequeue() |> int
        else -1

type internal EventedTextWriter() =
    inherit TextWriter()
    let sb = StringBuilder()
    let lineWritten = Event<string>()
    member __.LineWritten = lineWritten.Publish
    override __.Encoding = Encoding.UTF8
    override __.Write(c: char) =
        if c = '\n' then
            let line =
                let v = sb.ToString()
                if v.EndsWith("\r") then v.Substring(0, v.Length - 1)
                else v
            sb.Clear() |> ignore
            lineWritten.Trigger(line)
        else sb.Append(c) |> ignore
