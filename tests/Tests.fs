module Tests

open System
open Xunit

[<Fact>]
let ``My test 1`` () =
    Assert.True(true)

[<Fact>]
let ``My test 2`` () =
    Assert.True(true)

[<Fact>]
let ``My test 3`` () =
    Assert.True(true)

[<Fact>]
let ``My test 4`` () =
    Assert.True(true)


[<InlineData(2)>]
[<InlineData(3)>]
[<InlineData(4)>]
[<InlineData(5)>]
[<InlineData(6)>]
[<InlineData(7)>]
[<InlineData(8)>]
[<InlineData(9)>]
[<InlineData(10)>]
[<InlineData(11)>]
[<InlineData(12)>]
[<InlineData(13)>]
[<InlineData(14)>]
[<InlineData(15)>]
[<InlineData(16)>]
[<InlineData(17)>]
[<InlineData(18)>]
[<InlineData(19)>]
[<InlineData(20)>]
[<InlineData(21)>]
[<InlineData(22)>]
[<InlineData(23)>]
[<InlineData(24)>]
[<InlineData(25)>]
[<InlineData(26)>]
[<InlineData(27)>]
[<InlineData(28)>]
[<InlineData(29)>]
[<InlineData(30)>]
[<InlineData(31)>]
[<InlineData(32)>]
[<InlineData(33)>]
[<InlineData(34)>]
[<InlineData(35)>]
[<InlineData(36)>]
[<InlineData(37)>]
[<InlineData(38)>]
[<InlineData(39)>]
[<InlineData(40)>]
[<InlineData(41)>]
[<InlineData(42)>]
[<InlineData(43)>]
[<InlineData(44)>]
[<InlineData(45)>]
[<InlineData(46)>]
[<InlineData(47)>]
[<InlineData(48)>]
[<InlineData(49)>]
[<InlineData(50)>]
[<Theory>]
let ``My test with arg`` (arg:int) =
    System.Threading.Thread.Sleep(arg*100)
    Assert.True(true)


