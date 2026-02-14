// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Various tests for:
// Microsoft.FSharp.Core.ExtraTopLevelOperators.print
// Microsoft.FSharp.Core.ExtraTopLevelOperators.println

namespace FSharp.Core.UnitTests

open System
open System.IO
open Xunit

[<Collection(nameof FSharp.Test.NotThreadSafeResourceCollection)>]
type PrintTests() =

    let captureConsoleOut (f: unit -> unit) =
        let oldOut = Console.Out
        use sw = new StringWriter()
        Console.SetOut(sw)
        try
            f ()
            sw.ToString()
        finally
            Console.SetOut(oldOut)

    [<Fact>]
    member _.``print writes string value``() =
        let result = captureConsoleOut (fun () -> print "hello")
        Assert.Equal("hello", result)

    [<Fact>]
    member _.``print writes integer value``() =
        let result = captureConsoleOut (fun () -> print 42)
        Assert.Equal("42", result)

    [<Fact>]
    member _.``print writes float with InvariantCulture``() =
        let result = captureConsoleOut (fun () -> print 3.14)
        Assert.Equal("3.14", result)

    [<Fact>]
    member _.``print writes bool value``() =
        let result = captureConsoleOut (fun () -> print true)
        Assert.Equal("True", result)

    [<Fact>]
    member _.``print writes Some value``() =
        let result = captureConsoleOut (fun () -> print (Some 42))
        Assert.Equal("Some(42)", result)

    [<Fact>]
    member _.``print writes None value``() =
        let result = captureConsoleOut (fun () -> print None)
        Assert.Equal("", result)

    [<Fact>]
    member _.``print writes list value``() =
        let result = captureConsoleOut (fun () -> print [1; 2; 3])
        Assert.Equal("[1; 2; 3]", result)

    [<Fact>]
    member _.``println writes value followed by newline``() =
        let result = captureConsoleOut (fun () -> println "hello")
        Assert.Equal("hello" + Environment.NewLine, result)

    [<Fact>]
    member _.``multiple prints concatenate``() =
        let result = captureConsoleOut (fun () ->
            print "Hello, "
            print "World!")
        Assert.Equal("Hello, World!", result)

    [<Fact>]
    member _.``println writes integer with newline``() =
        let result = captureConsoleOut (fun () -> println 42)
        Assert.Equal("42" + Environment.NewLine, result)
