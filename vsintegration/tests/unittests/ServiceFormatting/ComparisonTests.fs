module FSharp.Compiler.Service.Tests.ServiceFormatting.ComparisonTests

open NUnit.Framework
open FsUnit
open TestHelper

// the current behavior results in a compile error since the = is moved to the next line and not correctly indented
[<Test>]
let ``should keep the = on the same line in record def``() =
    formatSourceString false """type UnionTypeConverter() = 
    inherit JsonConverter()
    let doRead(reader : JsonReader) = reader.Read() |> ignore
    override x.CanConvert(typ : Type) = 
        let result = 
            ((typ.GetInterface(typeof<System.Collections.IEnumerable>.FullName) = null) && FSharpType.IsUnion typ)
        result
    """ config
    |> should equal """type UnionTypeConverter() =
    inherit JsonConverter()
    let doRead (reader : JsonReader) = reader.Read() |> ignore
    override x.CanConvert(typ : Type) =
        let result =
            ((typ.GetInterface(typeof<System.Collections.IEnumerable>.FullName) = null) 
             && FSharpType.IsUnion typ)
        result
"""

// the current behavior results in a compile error since the = is moved to the next line and not correctly indented
[<Test>]
let ``should keep the = on the same line``() =
    formatSourceString false """trimSpecialChars(controller.ServerName.ToUpper()) = trimSpecialChars(serverFilter.ToUpper())
    """ config
    |> should equal """trimSpecialChars (controller.ServerName.ToUpper()) = trimSpecialChars 
                                                         (serverFilter.ToUpper())
"""
