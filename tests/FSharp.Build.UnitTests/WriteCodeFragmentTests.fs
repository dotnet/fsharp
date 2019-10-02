// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Build.UnitTests

open Microsoft.Build.Framework
open Microsoft.Build.Utilities
open FSharp.Build
open NUnit.Framework

[<TestFixture>]
type WriteCodeFragmentFSharpTests() =

    let verifyAttribute (attributeName:string) (parameters:(string*string) list) (expectedAttributeText:string) =
        let taskItem = TaskItem(attributeName)
        parameters |> List.iter (fun (key, value) -> taskItem.SetMetadata(key, value))
        let actualAttributeText = WriteCodeFragment.GenerateAttribute (taskItem :> ITaskItem, "f#")
        let fullExpectedAttributeText = "[<assembly: " + expectedAttributeText + ">]"
        Assert.AreEqual(fullExpectedAttributeText, actualAttributeText)

    [<Test>]
    member this.``No parameters``() =
        verifyAttribute "SomeAttribute" [] "SomeAttribute()"

    [<Test>]
    member this.``Skipped and out of order positional parameters``() =
        verifyAttribute "SomeAttribute" [("_Parameter3", "3"); ("_Parameter5", "5"); ("_Parameter2", "2")] "SomeAttribute(null, \"2\", \"3\", null, \"5\")"

    [<Test>]
    member this.``Named parameters``() =
        verifyAttribute "SomeAttribute" [("One", "1"); ("Two", "2")] "SomeAttribute(One = \"1\", Two = \"2\")"

    [<Test>]
    member this.``Named and positional parameters``() =
        verifyAttribute "SomeAttribute" [("One", "1"); ("_Parameter2", "2.2"); ("Two", "2")] "SomeAttribute(null, \"2.2\", One = \"1\", Two = \"2\")"

    [<Test>]
    member this.``Escaped string parameters``() =
        verifyAttribute "SomeAttribute" [("_Parameter1", "\"uno\"")] "SomeAttribute(\"\\\"uno\\\"\")"
        //                                     this should look like: SomeAttribute("\"uno\"")


[<TestFixture>]
type WriteCodeFragmentCSharpTests() =

    let verifyAttribute (attributeName:string) (parameters:(string*string) list) (expectedAttributeText:string) =
        let taskItem = TaskItem(attributeName)
        parameters |> List.iter (fun (key, value) -> taskItem.SetMetadata(key, value))
        let actualAttributeText = WriteCodeFragment.GenerateAttribute (taskItem :> ITaskItem, "c#")
        let fullExpectedAttributeText = "[assembly: " + expectedAttributeText + "]"
        Assert.AreEqual(fullExpectedAttributeText, actualAttributeText)

    [<Test>]
    member this.``No parameters``() =
        verifyAttribute "SomeAttribute" [] "SomeAttribute()"

    [<Test>]
    member this.``Skipped and out of order positional parameters``() =
        verifyAttribute "SomeAttribute" [("_Parameter3", "3"); ("_Parameter5", "5"); ("_Parameter2", "2")] "SomeAttribute(null, \"2\", \"3\", null, \"5\")"

    [<Test>]
    member this.``Named parameters``() =
        verifyAttribute "SomeAttribute" [("One", "1"); ("Two", "2")] "SomeAttribute(One = \"1\", Two = \"2\")"

    [<Test>]
    member this.``Named and positional parameters``() =
        verifyAttribute "SomeAttribute" [("One", "1"); ("_Parameter2", "2.2"); ("Two", "2")] "SomeAttribute(null, \"2.2\", One = \"1\", Two = \"2\")"

    [<Test>]
    member this.``Escaped string parameters``() =
        verifyAttribute "SomeAttribute" [("_Parameter1", "\"uno\"")] "SomeAttribute(\"\\\"uno\\\"\")"
        //                                     this should look like: SomeAttribute("\"uno\"")


[<TestFixture>]
type WriteCodeFragmentVisualBasicTests() =

    let verifyAttribute (attributeName:string) (parameters:(string*string) list) (expectedAttributeText:string) =
        let taskItem = TaskItem(attributeName)
        parameters |> List.iter (fun (key, value) -> taskItem.SetMetadata(key, value))
        let actualAttributeText = WriteCodeFragment.GenerateAttribute (taskItem :> ITaskItem, "vb")
        let fullExpectedAttributeText = "<Assembly: " + expectedAttributeText + ">"
        Assert.AreEqual(fullExpectedAttributeText, actualAttributeText)

    [<Test>]
    member this.``No parameters``() =
        verifyAttribute "SomeAttribute" [] "SomeAttribute()"

    [<Test>]
    member this.``Skipped and out of order positional parameters``() =
        verifyAttribute "SomeAttribute" [("_Parameter3", "3"); ("_Parameter5", "5"); ("_Parameter2", "2")] "SomeAttribute(null, \"2\", \"3\", null, \"5\")"

    [<Test>]
    member this.``Named parameters``() =
        verifyAttribute "SomeAttribute" [("One", "1"); ("Two", "2")] "SomeAttribute(One = \"1\", Two = \"2\")"

    [<Test>]
    member this.``Named and positional parameters``() =
        verifyAttribute "SomeAttribute" [("One", "1"); ("_Parameter2", "2.2"); ("Two", "2")] "SomeAttribute(null, \"2.2\", One = \"1\", Two = \"2\")"

    [<Test>]
    member this.``Escaped string parameters``() =
        verifyAttribute "SomeAttribute" [("_Parameter1", "\"uno\"")] "SomeAttribute(\"\\\"uno\\\"\")"
        //                                     this should look like: SomeAttribute("\"uno\"")

