namespace NUnit.Framework

module Assert =

    let inline fail message = Assert.Fail message
    
    let inline failf fmt = Printf.kprintf fail fmt

    let inline areEqual (expected: ^T) (actual: ^T) =
        Assert.shouldBe expected actual
  
module StringAssert =

    let inline contains expected actual = StringAssert.Contains(expected, actual)
