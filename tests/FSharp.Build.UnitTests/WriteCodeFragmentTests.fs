// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Build.UnitTests

open Microsoft.Build.Framework
open Microsoft.Build.Utilities
open FSharp.Build
open Xunit

type WriteCodeFragmentFSharpTests() =

    let verifyAttribute (attributeName:string) (parameters:(string*string) list) (expectedAttributeText:string) =
        let taskItem = TaskItem(attributeName)
        parameters |> List.iter (fun (key, value) -> taskItem.SetMetadata(key, value))
        let actualAttributeText = (new WriteCodeFragment()).GenerateAttribute (taskItem :> ITaskItem, "f#")
        let fullExpectedAttributeText = "[<assembly: " + expectedAttributeText + ">]"
        Assert.Equal(fullExpectedAttributeText, actualAttributeText)

    [<Fact>]
    member _.``No parameters``() =
        verifyAttribute "SomeAttribute" [] "SomeAttribute()"

    [<Fact>]
    member _.``Skipped and out of order positional parameters``() =
        verifyAttribute "SomeAttribute" [("_Parameter3", "3"); ("_Parameter5", "5"); ("_Parameter2", "2")] "SomeAttribute(null, \"2\", \"3\", null, \"5\")"

    [<Fact>]
    member _.``Named parameters``() =
        verifyAttribute "SomeAttribute" [("One", "1"); ("Two", "2")] "SomeAttribute(One = \"1\", Two = \"2\")"

    [<Fact>]
    member _.``Named and positional parameters``() =
        verifyAttribute "SomeAttribute" [("One", "1"); ("_Parameter2", "2.2"); ("Two", "2")] "SomeAttribute(null, \"2.2\", One = \"1\", Two = \"2\")"

    [<Fact>]
    member _.``Escaped string parameters``() =
        verifyAttribute "SomeAttribute" [("_Parameter1", "\"uno\"")] "SomeAttribute(\"\\\"uno\\\"\")"

    [<Fact>]
    member _.``Named parameters with _IsLiteral suffix``() =
        verifyAttribute "SomeAttribute" [("Bool", "true"); ("Bool_IsLiteral", "true")] "SomeAttribute(Bool = true)"
        
    [<Fact>]
    member _.``Multiple named parameters with _IsLiteral suffix``() =
        verifyAttribute "SomeAttribute" 
            [
                ("Number", "42"); 
                ("Number_IsLiteral", "true"); 
                ("Bool", "false"); 
                ("Bool_IsLiteral", "true")
            ] 
            "SomeAttribute(Bool = false, Number = 42)"
            
    [<Fact>]
    member _.``Mixed named parameters with and without _IsLiteral suffix``() =
        verifyAttribute "SomeAttribute" 
            [
                ("Number", "42"); 
                ("Number_IsLiteral", "true"); 
                ("Text", "hello")
            ] 
            "SomeAttribute(Number = 42, Text = \"hello\")"
    
    [<Fact>]
    member _.``Mixed unnamed parameters and named parameters with _IsLiteral suffix``() =
        verifyAttribute "SomeAttribute" 
            [
                ("_Parameter1", "1");
                ("Number", "42"); 
                ("Number_IsLiteral", "true"); 
                ("Bool", "false"); 
                ("Bool_IsLiteral", "true")
            ] 
            "SomeAttribute(\"1\", Bool = false, Number = 42)"
    
    [<Fact>]
    member _.``Enum _IsLiteral suffix``() =
        verifyAttribute "SomeAttribute" 
            [
                ("EnumValue", "System.StringComparison.OrdinalIgnoreCase"); 
                ("EnumValue_IsLiteral", "true")
            ] 
            "SomeAttribute(EnumValue = System.StringComparison.OrdinalIgnoreCase)"

type WriteCodeFragmentCSharpTests() =

    let verifyAttribute (attributeName:string) (parameters:(string*string) list) (expectedAttributeText:string) =
        let taskItem = TaskItem(attributeName)
        parameters |> List.iter (fun (key, value) -> taskItem.SetMetadata(key, value))
        let actualAttributeText = (new WriteCodeFragment()).GenerateAttribute (taskItem :> ITaskItem, "c#")
        let fullExpectedAttributeText = "[assembly: " + expectedAttributeText + "]"
        Assert.Equal(fullExpectedAttributeText, actualAttributeText)
        
    [<Fact>]
    member _.``No parameters``() =
        verifyAttribute "SomeAttribute" [] "SomeAttribute()"

    [<Fact>]
    member _.``Skipped and out of order positional parameters``() =
        verifyAttribute "SomeAttribute" [("_Parameter3", "3"); ("_Parameter5", "5"); ("_Parameter2", "2")] "SomeAttribute(null, \"2\", \"3\", null, \"5\")"

    [<Fact>]
    member _.``Named parameters``() =
        verifyAttribute "SomeAttribute" [("One", "1"); ("Two", "2")] "SomeAttribute(One = \"1\", Two = \"2\")"

    [<Fact>]
    member _.``Named and positional parameters``() =
        verifyAttribute "SomeAttribute" [("One", "1"); ("_Parameter2", "2.2"); ("Two", "2")] "SomeAttribute(null, \"2.2\", One = \"1\", Two = \"2\")"

    [<Fact>]
    member _.``Escaped string parameters``() =
        verifyAttribute "SomeAttribute" [("_Parameter1", "\"uno\"")] "SomeAttribute(\"\\\"uno\\\"\")"
        //                                     this should look like: SomeAttribute("\"uno\"")
        
    [<Fact>]
    member _.``Named parameters with _IsLiteral suffix``() =
        verifyAttribute "SomeAttribute" [("Bool", "true"); ("Bool_IsLiteral", "true")] "SomeAttribute(Bool = true)"
        
    [<Fact>]
    member _.``Multiple named parameters with _IsLiteral suffix``() =
        verifyAttribute "SomeAttribute" 
            [
                ("Number", "42"); 
                ("Number_IsLiteral", "true"); 
                ("Bool", "false"); 
                ("Bool_IsLiteral", "true")
            ] 
            "SomeAttribute(Bool = false, Number = 42)"
            
    [<Fact>]
    member _.``Mixed named parameters with and without _IsLiteral suffix``() =
        verifyAttribute "SomeAttribute" 
            [
                ("Number", "42"); 
                ("Number_IsLiteral", "true"); 
                ("Text", "hello")
            ] 
            "SomeAttribute(Number = 42, Text = \"hello\")"
            
    [<Fact>]
    member _.``Mixed unnamed parameters and named parameters with _IsLiteral suffix``() =
        verifyAttribute "SomeAttribute" 
            [
                ("_Parameter1", "1");
                ("Number", "42"); 
                ("Number_IsLiteral", "true"); 
                ("Bool", "false"); 
                ("Bool_IsLiteral", "true")
            ] 
            "SomeAttribute(\"1\", Bool = false, Number = 42)"
            
    [<Fact>]
    member _.``Enum _IsLiteral suffix``() =
        verifyAttribute "SomeAttribute" 
            [
                ("EnumValue", "System.StringComparison.OrdinalIgnoreCase"); 
                ("EnumValue_IsLiteral", "true")
            ] 
            "SomeAttribute(EnumValue = System.StringComparison.OrdinalIgnoreCase)"


type WriteCodeFragmentVisualBasicTests() =

    let verifyAttribute (attributeName:string) (parameters:(string*string) list) (expectedAttributeText:string) =
        let taskItem = TaskItem(attributeName)
        parameters |> List.iter (fun (key, value) -> taskItem.SetMetadata(key, value))
        let actualAttributeText = (new WriteCodeFragment()).GenerateAttribute (taskItem :> ITaskItem, "vb")
        let fullExpectedAttributeText = "<Assembly: " + expectedAttributeText + ">"
        Assert.Equal(fullExpectedAttributeText, actualAttributeText)

    [<Fact>]
    member _.``No parameters``() =
        verifyAttribute "SomeAttribute" [] "SomeAttribute()"

    [<Fact>]
    member _.``Skipped and out of order positional parameters``() =
        verifyAttribute "SomeAttribute" [("_Parameter3", "3"); ("_Parameter5", "5"); ("_Parameter2", "2")] "SomeAttribute(null, \"2\", \"3\", null, \"5\")"

    [<Fact>]
    member _.``Named parameters``() =
        verifyAttribute "SomeAttribute" [("One", "1"); ("Two", "2")] "SomeAttribute(One = \"1\", Two = \"2\")"

    [<Fact>]
    member _.``Named and positional parameters``() =
        verifyAttribute "SomeAttribute" [("One", "1"); ("_Parameter2", "2.2"); ("Two", "2")] "SomeAttribute(null, \"2.2\", One = \"1\", Two = \"2\")"

    [<Fact>]
    member _.``Escaped string parameters``() =
        verifyAttribute "SomeAttribute" [("_Parameter1", "\"uno\"")] "SomeAttribute(\"\\\"uno\\\"\")"
        //                                     this should look like: SomeAttribute("\"uno\"")
        
    [<Fact>]
    member _.``Named parameters with _IsLiteral suffix``() =
        verifyAttribute "SomeAttribute" [("Bool", "true"); ("Bool_IsLiteral", "true")] "SomeAttribute(Bool = true)"
        
    [<Fact>]
    member _.``Multiple named parameters with _IsLiteral suffix``() =
        verifyAttribute "SomeAttribute" 
            [
                ("Number", "42"); 
                ("Number_IsLiteral", "true"); 
                ("Bool", "false"); 
                ("Bool_IsLiteral", "true")
            ] 
            "SomeAttribute(Bool = false, Number = 42)"
            
    [<Fact>]
    member _.``Mixed named parameters with and without _IsLiteral suffix``() =
        verifyAttribute "SomeAttribute" 
            [
                ("Number", "42"); 
                ("Number_IsLiteral", "true"); 
                ("Text", "hello")
            ] 
            "SomeAttribute(Number = 42, Text = \"hello\")"
            
    [<Fact>]
    member _.``Mixed unnamed parameters and named parameters with _IsLiteral suffix``() =
        verifyAttribute "SomeAttribute" 
            [
                ("_Parameter1", "1");
                ("Number", "42"); 
                ("Number_IsLiteral", "true"); 
                ("Bool", "false"); 
                ("Bool_IsLiteral", "true")
            ] 
            "SomeAttribute(\"1\", Bool = false, Number = 42)"
            
    [<Fact>]
    member _.``Enum _IsLiteral suffix``() =
        verifyAttribute "SomeAttribute" 
            [
                ("EnumValue", "System.StringComparison.OrdinalIgnoreCase"); 
                ("EnumValue_IsLiteral", "true")
            ] 
            "SomeAttribute(EnumValue = System.StringComparison.OrdinalIgnoreCase)"

