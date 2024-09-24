open Xunit

let shouldEqual (x: 'a) (y: 'a) =
    Assert.Equal<'a>(x, y)