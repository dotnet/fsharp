module NUnit.Framework

open System
open Xunit

type TestAttribute = FactAttribute

type TestFixtureAttribute() = inherit Attribute()

type TestContext() =
    static member CurrentContext = TestContext()
    member __.WorkDirectory = ""

type Assert() =
    static member IsFalse (condition: bool) =
        Assert.False condition

    static member IsFalse (condition: bool, userMessage) =
        Assert.False (condition, userMessage)

    static member IsTrue (condition: bool) =
        Assert.True condition

    static member IsTrue (condition: bool, userMessage) =
        Assert.True (condition, userMessage)

    static member Fail () =
        Assert.True false

    static member Fail userMessage =
        Assert.True (false, userMessage)

    static member Fail (userMessage, [<ParamArrayAttribute>] args: obj []) =
        Assert.True (false, String.Format(userMessage, args))

    static member AreEqual (expected: obj, actual: obj) =
        Assert.Equal<obj> (expected, actual)

    static member AreEqual<'T> (expected: 'T seq, actual: 'T seq) =
        Assert.Equal<'T> (expected, actual)

    static member AreEqual (expected, actual, userMessage) =
        if expected <> actual then
            Assert.Fail userMessage

    static member AreNotEqual<'T> (expected: 'T, actual: 'T) =
        Assert.NotEqual (expected, actual)

    static member IsNull (o: obj) =
        Assert.Null o
