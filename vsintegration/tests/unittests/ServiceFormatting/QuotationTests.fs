module FSharp.Compiler.Service.Tests.ServiceFormatting.QuotationTests

open NUnit.Framework
open FsUnit
open TestHelper

[<Test>]
let ``typed quotations``() =
    formatSourceString false """
    <@ 
        let f x = x + 10
        f 20
    @>""" config
    |> prepend newline
    |> should equal """
<@ let f x = x + 10
   f 20 @>
"""

[<Test>]
let ``untyped quotations``() =
    formatSourceString false "<@@ 2 + 3 @@>" config
    |> should equal """<@@ 2 + 3 @@>
"""

[<Test>]
let ``should preserve unit literal``() =
    formatSourceString false """
    let logger = Mock<ILogger>().Setup(fun log -> <@ log.Log(error) @>).Returns(()).Create()
    """ config
    |> prepend newline
    |> should equal """
let logger =
    Mock<ILogger>().Setup(fun log -> <@ log.Log(error) @>).Returns(()).Create()
"""