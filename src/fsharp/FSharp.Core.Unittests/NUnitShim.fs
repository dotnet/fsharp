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

    static member AreEqual<'T when 'T : equality> (expected: 'T, actual: 'T, userMessage) =
        if expected <> actual then
            Assert.Fail userMessage

    static member AreEqual (expected: string, actual: string, userMessage: string) =
        let maxCharsBefore = 33
        let displayChars = 64
        let lineLength = 78

        if expected <> actual then
            let truncate startIndex length (s: string) = if s.Length <= length then s else s.Substring (startIndex, length)
            let replace (oldValue: string) (newValue: string) (str:string) = str.Replace(oldValue, newValue)

            let escapeChars =
                let escapeChar c =
                    match c with
                    | '\r' -> "\\r"
                    | '\n' -> "\\n"
                    | '\t' -> "\\t"
                    | c -> c.ToString()
                String.collect escapeChar

            let differenceIndex = Seq.zip (expected.ToCharArray ())
                                          (actual.ToCharArray ())
                                  |> Seq.takeWhile (fun (a,b) -> a = b)
                                  |> Seq.length

            let differenceMessage =
                let startIndex = differenceIndex - maxCharsBefore |> max 0

                (expected |> truncate startIndex displayChars |> escapeChars,
                 actual |> truncate startIndex displayChars |> escapeChars,
                 String ('-' , differenceIndex - startIndex + 11))

                |||> sprintf "  Expected: %A\n  But was:  %A\n  %s^"

            let message = if expected.Length <> actual.Length then
                              sprintf "Expected string length %d but was %d. Strings differ at index %d.\n%s" expected.Length actual.Length differenceIndex differenceMessage
                          else
                              sprintf "String lengths are both %d. Strings differ at index %d.\n%s" expected.Length differenceIndex differenceMessage

            Assert.Fail message

    static member AreNotEqual<'T> (expected: 'T, actual: 'T) =
        Assert.NotEqual (expected, actual)

    static member IsNull (o: obj) =
        Assert.Null o
