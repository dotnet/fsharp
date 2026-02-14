// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace EmittedIL

open Xunit
open FSharp.Test.Compiler

module PrintFunction =

    [<Fact>]
    let ``print with int specializes to Int32 ToString with InvariantCulture``() =
        FSharp """
module PrintInt

let printInt () = print 42
         """
         |> withOptimize
         |> compile
         |> shouldSucceed
         |> verifyIL [
             """call       class [netstandard]System.IO.TextWriter [netstandard]System.Console::get_Out()"""
             """call       class [netstandard]System.Globalization.CultureInfo [netstandard]System.Globalization.CultureInfo::get_InvariantCulture()"""
             """call       instance string [netstandard]System.Int32::ToString(string,"""
             """callvirt   instance void [netstandard]System.IO.TextWriter::Write(string)"""]

    [<Fact>]
    let ``println with int specializes to Int32 ToString with InvariantCulture``() =
        FSharp """
module PrintlnInt

let printlnInt () = println 42
         """
         |> withOptimize
         |> compile
         |> shouldSucceed
         |> verifyIL [
             """call       class [netstandard]System.IO.TextWriter [netstandard]System.Console::get_Out()"""
             """call       class [netstandard]System.Globalization.CultureInfo [netstandard]System.Globalization.CultureInfo::get_InvariantCulture()"""
             """call       instance string [netstandard]System.Int32::ToString(string,"""
             """callvirt   instance void [netstandard]System.IO.TextWriter::WriteLine(string)"""]

    [<Fact>]
    let ``print with string writes to Console Out``() =
        FSharp """
module PrintString

let printStr () = print "hello"
         """
         |> withOptimize
         |> compile
         |> shouldSucceed
         |> verifyIL [
             """call       class [netstandard]System.IO.TextWriter [netstandard]System.Console::get_Out()"""
             """callvirt   instance void [netstandard]System.IO.TextWriter::Write(string)"""]

    [<Fact>]
    let ``println with float specializes to Double ToString with InvariantCulture``() =
        FSharp """
module PrintlnFloat

let printlnFloat () = println 3.14
         """
         |> withOptimize
         |> compile
         |> shouldSucceed
         |> verifyIL [
             """call       class [netstandard]System.IO.TextWriter [netstandard]System.Console::get_Out()"""
             """call       class [netstandard]System.Globalization.CultureInfo [netstandard]System.Globalization.CultureInfo::get_InvariantCulture()"""
             """call       instance string [netstandard]System.Double::ToString(string,"""
             """callvirt   instance void [netstandard]System.IO.TextWriter::WriteLine(string)"""]
