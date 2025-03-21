module FSharp.Compiler.Service.Tests.FileContentTests

open FSharp.Compiler.Text
open FSharp.Compiler.Text.Position
open FSharp.Compiler.Text.Range
open FSharp.Compiler.Text.FileContent
open FSharp.Test.Assert
open Xunit

let substring (input: string) (startLine, startCol) (endLine, endCol) =
    let range = mkFileIndexRange 0 (mkPos startLine startCol) (mkPos endLine endCol)
    substring input range
    
[<Fact>]
let ``substring tests`` () =
    substring "Hello, world!\r\nThis is a test.\r\n" (1, 7) (2, 4) |> shouldEqual "world!\r\nThis"
    substring "Hello, world!\r\nThis is a test.\r\n" (9, 7) (9, 4) |> shouldEqual ""
    substring "Hello, world!\r\nThis is a test.\r\ntest\n" (1, 7) (9, 4) |> shouldEqual "world!\r\nThis is a test.\r\ntest\n"
    substring "Hello, world!\r\nThis is a test.\r\n" (1, 7) (2, 100) |> shouldEqual "world!\r\nThis is a test.\r\n"
    substring "Hello, world!\r\nThis is a test.\r\n" (1, 7) (2, 7) |> shouldEqual "world!\r\nThis is"
    substring "Hello, world! This is a test.\r\na test" (2, 1) (2, 6) |> shouldEqual " test"
    substring "Hello, world! This is a test.\r\na test" (2, 7) (2, 8) |> shouldEqual ""
    
    substring "Hello, world!\r\nThis is a test.\r\n" (1, 0) (1, 4) |> shouldEqual "Hell"
    substring "Hello, world!\r\nThis is a test.\r\n" (1, 7) (1, 4) |> shouldEqual ""
    substring "Hello, world!\r\nThis is a test.\r\n" (1, 7) (1, 100) |> shouldEqual "world!\r\n"
    substring "Hello, world!\r\nThis is a test.\r\n" (1, 7) (1, 7) |> shouldEqual ""
    substring "Hello, world!\r\nThis is a test.\r\n" (1, 7) (1, 8) |> shouldEqual "w"
    substring "Hello, world!\r\nThis is a test.\r\n" (1, 0) (1, 8) |> shouldEqual "Hello, w"
    substring "Hello, world!\r\nThis is a test.\r\n" (1, 0) (1, 100) |> shouldEqual "Hello, world!\r\n"
