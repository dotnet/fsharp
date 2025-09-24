module ErrorMessages.ExtendedDiagnosticData
#nowarn "57"

open FSharp.Compiler.Text
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.Diagnostics.ExtendedData
open FSharp.Test.Assert
open FSharp.Test.Compiler
open Xunit

let checkTypes (diagnosticData: TypeMismatchDiagnosticExtendedData) expected actual =
    {| ExpectedType = expected; ActualType = actual |}
    |> shouldBe {| ExpectedType = diagnosticData.ExpectedType.Format(diagnosticData.DisplayContext)
                   ActualType = diagnosticData.ActualType.Format(diagnosticData.DisplayContext) |}

let inline checkDiagnosticData
    (diagnosticNumber, message)
    (check: 'a -> unit)
    (checkResults: 'b when 'b: (member Diagnostics: FSharpDiagnostic[])) =
    match checkResults.Diagnostics |> Array.tryFind (fun d -> d.ErrorNumber = diagnosticNumber) with
    | None -> failwith $"Expected diagnostic (number {diagnosticNumber}) not found"
    | Some diagnostic ->

    Assert.Equal(message, diagnostic.Message)
    match diagnostic.ExtendedData with
    | Some(:? 'a as data) -> check data
    | _ -> failwith "Expected diagnostic extended data not found"

    checkResults
    
let inline checkExpectedActualTypesInContext
    diagnosticNumber
    expectedType actualType
    message
    context
    checkResults =

    checkResults
    |> checkDiagnosticData (diagnosticNumber, message)
       (fun (data: TypeMismatchDiagnosticExtendedData) ->
        checkTypes data expectedType actualType
        Assert.Equal(context, data.ContextInfo))

let inline checkExpectedActualTypes
    diagnosticNumber
    expectedType actualType
    message
    checkResults =

    checkResults
    |> checkExpectedActualTypesInContext diagnosticNumber expectedType actualType message DiagnosticContextInfo.NoContext

[<Fact>]
let ``TypeMismatchDiagnosticExtendedData 01`` () =
    FSharp """
let x, y, z = 1, 2
"""
    |> typecheckResults
    |> checkExpectedActualTypes 1
       "obj * obj * obj"
       "int * int"
       "Type mismatch. Expecting a tuple of length 3 of type\n    'a * 'b * 'c    \nbut given a tuple of length 2 of type\n    int * int    \n"

[<Fact>]
let ``TypeMismatchDiagnosticExtendedData 02`` () =
    FSharp """
let x, y = 1
"""
    |> typecheckResults
    |> checkExpectedActualTypes 1
       "obj * obj"
       "int"
       "This expression was expected to have type\n    ''a * 'b'    \nbut here has type\n    'int'    "

[<Fact>]
let ``TypeMismatchDiagnosticExtendedData 03`` () =
    FSharp """
if true then 5
"""
    |> typecheckResults
    |> checkExpectedActualTypesInContext 1
       "unit"
       "int"
       "This 'if' expression is missing an 'else' branch. Because 'if' is an expression, and not a statement, add an 'else' branch which also returns a value of type 'int'."
       DiagnosticContextInfo.OmittedElseBranch

[<Fact>]
let ``TypeMismatchDiagnosticExtendedData 04`` () =
    FSharp """
1 :> string
"""
    |> typecheckResults
    |> checkExpectedActualTypes 193
       "string"
       "int"
       "Type constraint mismatch. The type \n    'int'    \nis not compatible with type\n    'string'    \n"

[<Fact>]
let ``TypeMismatchDiagnosticExtendedData 05`` () =
    FSharp """
match 0 with
| 0 -> 1
| 1 -> "a"
"""
    |> typecheckResults
    |> checkExpectedActualTypesInContext 1
       "int"
       "string"
       "All branches of a pattern match expression must return values implicitly convertible to the type of the first branch, which here is 'int'. This branch returns a value of type 'string'."
       DiagnosticContextInfo.FollowingPatternMatchClause

//TODO: FollowingPatternMatchClause should be provided for type equation diagnostics come from a return type only
[<Fact>]
let ``TypeMismatchDiagnosticExtendedData 05 - TODO: fix wrong context`` () =
    FSharp """
let f (x: int) = ()

match 0 with
| 0 -> 1
| 1 ->
    let _ = f "a"
    1
"""
    |> typecheckResults
    |> checkExpectedActualTypesInContext 1
       "int"
       "string"
       "This expression was expected to have type\n    'int'    \nbut here has type\n    'string'    "
       DiagnosticContextInfo.FollowingPatternMatchClause //Should be NoContext

[<Fact>]
let ``TypeMismatchDiagnosticExtendedData 06`` () =
    FSharp """
let _: bool =
    if true then "a" else "b"
"""
    |> typecheckResults
    |> checkExpectedActualTypesInContext 1
       "bool"
       "string"
       "The 'if' expression needs to have type 'bool' to satisfy context type requirements. It currently has type 'string'."
       DiagnosticContextInfo.IfExpression

[<Fact>]
let ``TypeMismatchDiagnosticExtendedData 07`` () =
    FSharp """
if true then 1 else "a"
"""
    |> typecheckResults
    |> checkExpectedActualTypesInContext 1
       "int"
       "string"
       "All branches of an 'if' expression must return values implicitly convertible to the type of the first branch, which here is 'int'. This branch returns a value of type 'string'."
       DiagnosticContextInfo.ElseBranchResult

[<Fact>]
let ``TypeMismatchDiagnosticExtendedData 08`` () =
    FSharp """
type R = { Field1: int }
let f (x: R) = "" + x.Field1
"""
    |> typecheckResults
    |> checkExpectedActualTypes 1
       "string"
       "int"
       "The type 'int' does not match the type 'string'"

[<Fact>]
let ``TypeMismatchDiagnosticExtendedData 09`` () =
    FSharp """
let x: string = 1
"""
    |> typecheckResults
    |> checkExpectedActualTypes 1
       "string"
       "int"
       "This expression was expected to have type\n    'string'    \nbut here has type\n    'int'    "

[<Fact>]
let ``TypeMismatchDiagnosticExtendedData 10`` () =
    FSharp """
let f1 (x: outref<'T>) = 1
let f2 (x: inref<'T>) = f1 &x
"""
    |> typecheckResults
    |> checkExpectedActualTypes 1
       "outref<'T>"
       "inref<'T>"
       // TODO: wrong additional generic parameters mismatch message,
       //  should be "The type 'ByRefKinds.In' does not match the type 'ByRefKinds.Out'"
       "Type mismatch. Expecting a\n    'outref<'T>'    \nbut given a\n    'inref<'T>'    \nThe type 'ByRefKinds.Out' does not match the type 'ByRefKinds.In'"

[<Fact>]
let ``TypeMismatchDiagnosticExtendedData 11`` () =
    FSharp """
type T() =
    static member P1 = T.P2 + 1
    static member P2 = ""
"""
    |> typecheckResults
    // static member P1 = T.P2 ->+<- 1
    |> checkExpectedActualTypes 43
       "string"
       "int"
       "The type 'int' does not match the type 'string'"

    // static member P2 = ->""<-
    |> checkExpectedActualTypes 1
       "int"
       "string"
       "The type 'string' does not match the type 'int'"

[<Fact>]
let ``TypeMismatchDiagnosticExtendedData 12`` () =
    FSharp """
let x: string = 1 + 1
"""
    |> typecheckResults
    |> checkExpectedActualTypes 1
       "string"
       "int"
       "The type 'int' does not match the type 'string'"

[<Fact>]
let ``TypeMismatchDiagnosticExtendedData 13`` () =
    FSharp """
let x: string -> string = id 
let y: unit -> string = x
"""
    |> typecheckResults
    |> checkExpectedActualTypes 1
       "unit -> string"
       "string -> string"
       "Type mismatch. Expecting a\n    'unit -> string'    \nbut given a\n    'string -> string'    \nThe type 'string' does not match the type 'unit'"

[<Theory>] 
[<InlineData true>]
[<InlineData false>]
let ``ArgumentsInSigAndImplMismatchExtendedData 01`` useTransparentCompiler =
    let encodeFsi =
        Fsi """
module Test
val f: x: int -> unit
"""
    let encodeFs =
        FsSource """
module Test
let f (y: int) = ()
    """
    encodeFsi
    |> withAdditionalSourceFile encodeFs
    |> typecheckProject true useTransparentCompiler
    |> checkDiagnosticData
       (3218, "The argument names in the signature 'x' and implementation 'y' do not match. The argument name from the signature file will be used. This may cause problems when debugging or profiling.")
       (fun (argsMismatch: ArgumentsInSigAndImplMismatchExtendedData) ->
        Assert.Equal("x", argsMismatch.SignatureName)
        Assert.Equal("y", argsMismatch.ImplementationName)
        Assert.True(argsMismatch.SignatureRange.FileName.EndsWith("fsi"))
        Assert.True(argsMismatch.ImplementationRange.FileName.EndsWith("fs")))

[<Theory>]
[<InlineData true>]
[<InlineData false>]
let ``FieldNotContainedDiagnosticExtendedData 01`` useTransparentCompiler =
    let encodeFsi =
        Fsi """
namespace rec Foo
type A =
    val public myStatic: int
    """
    let encodeFs =
        FsSource """
namespace rec Foo
type A =
    val private myStatic: int
    """
    encodeFsi
    |> withAdditionalSourceFile encodeFs
    |> typecheckProject true useTransparentCompiler
    |> checkDiagnosticData
       (193, "The module contains the field\n    myStatic: int    \nbut its signature specifies\n    myStatic: int    \nthe accessibility specified in the signature is more than that specified in the implementation")
       (fun (fieldsData: FieldNotContainedDiagnosticExtendedData) ->
        Assert.True(fieldsData.SignatureField.Accessibility.IsPublic)
        Assert.True(fieldsData.ImplementationField.Accessibility.IsPrivate))

[<Fact>]
let ``ExpressionIsAFunctionExtendedData 01`` () =
    FSharp """
module Test

id
"""
    |> typecheckResults
    |> checkDiagnosticData
       (193, "This expression is a function value, i.e. is missing arguments. Its type is 'a -> 'a.")
       (fun (wrongType: ExpressionIsAFunctionExtendedData) ->
        Assert.Equal("type 'a -> 'a", wrongType.ActualType.ToString()))

let private assertRange
    (expectedStartLine: int, expectedStartColumn: int)
    (expectedEndLine: int, expectedEndColumn: int)
    (actualRange: range)
    : unit =
    Assert.Equal(Position.mkPos expectedStartLine expectedStartColumn, actualRange.Start)
    Assert.Equal(Position.mkPos expectedEndLine expectedEndColumn, actualRange.End)

[<Theory>]
[<InlineData true>]
[<InlineData false>]
let ``DefinitionsInSigAndImplNotCompatibleAbbreviationsDifferExtendedData 01`` useTransparentCompiler =
    let signature =
        Fsi """
namespace Project

type Foo = {| bar: int |}
    """

    let implementation =
        FsSource """
namespace Project

type  Foo = {| bar: int; x: int |}
    """

    signature
    |> withAdditionalSourceFile implementation
    |> typecheckProject true useTransparentCompiler
    |> checkDiagnosticData
       (318, "The type definitions for type 'Foo' in the signature and implementation are not compatible because the abbreviations differ:\n    {| bar: int; x: int |}\nversus\n    {| bar: int |}")
       (fun (fieldsData: DefinitionsInSigAndImplNotCompatibleAbbreviationsDifferExtendedData) ->
        assertRange (4,5) (4,8) fieldsData.SignatureRange
        assertRange (4,6) (4,9) fieldsData.ImplementationRange)


[<FSharp.Test.FactForNETCOREAPP>]
let ``Warning - ObsoleteDiagnosticExtendedData 01`` () =
    FSharp """
open System
[<Obsolete("Message", false, DiagnosticId = "FS222", UrlFormat = "https://example.com")>]
type MyClass() = class end

let x = MyClass()
"""
    |> typecheckResults
    |> checkDiagnosticData
       (44, "This construct is deprecated. Message")
       (fun (obsoleteDiagnostic: ObsoleteDiagnosticExtendedData) ->
        Assert.Equal(Some "FS222", obsoleteDiagnostic.DiagnosticId)
        Assert.Equal(Some "https://example.com", obsoleteDiagnostic.UrlFormat))

[<FSharp.Test.FactForNETCOREAPP>]
let ``Warning - ObsoleteDiagnosticExtendedData 02`` () =
    FSharp """
open System
[<Obsolete("Message", false, DiagnosticId = "FS222")>]
type MyClass() = class end

let x = MyClass()
"""
    |> typecheckResults
    |> checkDiagnosticData
       (44, "This construct is deprecated. Message")
       (fun (obsoleteDiagnostic: ObsoleteDiagnosticExtendedData) ->
        Assert.Equal(Some "FS222", obsoleteDiagnostic.DiagnosticId)
        Assert.Equal(None, obsoleteDiagnostic.UrlFormat))
       
[<FSharp.Test.FactForNETCOREAPP>]
let ``Warning -  ObsoleteDiagnosticExtendedData 03`` () =
    FSharp """
open System
[<Obsolete("Message", false)>]
type MyClass() = class end

let x = MyClass()
"""
    |> typecheckResults
    |> checkDiagnosticData
       (44, "This construct is deprecated. Message")
       (fun (obsoleteDiagnostic: ObsoleteDiagnosticExtendedData) ->
        Assert.Equal(None, obsoleteDiagnostic.DiagnosticId)
        Assert.Equal(None, obsoleteDiagnostic.UrlFormat))
       
[<FSharp.Test.FactForNETCOREAPP>]
let ``Warning -  ObsoleteDiagnosticExtendedData 04`` () =
    FSharp """
open System
[<Obsolete(DiagnosticId = "FS222", UrlFormat = "https://example.com")>]
type MyClass() = class end

let x = MyClass()
"""
    |> typecheckResults
    |> checkDiagnosticData
       (44, "This construct is deprecated")
       (fun (obsoleteDiagnostic: ObsoleteDiagnosticExtendedData) ->
        Assert.Equal(Some "FS222", obsoleteDiagnostic.DiagnosticId)
        Assert.Equal(Some "https://example.com", obsoleteDiagnostic.UrlFormat))
       
       
[<FSharp.Test.FactForNETCOREAPP>]
let ``Warning -  ObsoleteDiagnosticExtendedData 05`` () =
    let CSLib =
        CSharp """
using System;
[Obsolete("Use something else", false, DiagnosticId = "FS222")]
public static class Class1
{
    public static string Test()
    {
        return "Hello";
    }
}
    """
        |> withName "CSLib"

    let app =
        FSharp """
open MyLib

let text = Class1.Test();
    """ |> withReferences [CSLib]

    app
    |> typecheckResults
    |> checkDiagnosticData
       (44, "This construct is deprecated. Use something else")
       (fun (obsoleteDiagnostic: ObsoleteDiagnosticExtendedData) ->
        Assert.Equal(Some "FS222", obsoleteDiagnostic.DiagnosticId)
        Assert.Equal(None, obsoleteDiagnostic.UrlFormat))
       
[<FSharp.Test.FactForNETCOREAPP>]
let ``Warning -  ObsoleteDiagnosticExtendedData 06`` () =
    let CSLib =
        CSharp """
using System;
[Obsolete("Use something else", false, DiagnosticId = "FS222", UrlFormat = "https://example.com")]
public static class Class1
{
    public static string Test()
    {
        return "Hello";
    }
}
    """
        |> withName "CSLib"

    let app =
        FSharp """
open MyLib

let text = Class1.Test();
    """ |> withReferences [CSLib]

    app
    |> typecheckResults
    |> checkDiagnosticData
       (44, "This construct is deprecated. Use something else")
       (fun (obsoleteDiagnostic: ObsoleteDiagnosticExtendedData) ->
        Assert.Equal(Some "FS222", obsoleteDiagnostic.DiagnosticId)
        Assert.Equal(Some "https://example.com", obsoleteDiagnostic.UrlFormat))
       
[<FSharp.Test.FactForNETCOREAPP>]
let ``Warning -  ObsoleteDiagnosticExtendedData 07`` () =
    let CSLib =
        CSharp """
using System;
[Obsolete("Use something else", false)]
public static class Class1
{
    public static string Test()
    {
        return "Hello";
    }
}
    """
        |> withName "CSLib"

    let app =
        FSharp """
open MyLib

let text = Class1.Test();
    """ |> withReferences [CSLib]

    app
    |> typecheckResults
    |> checkDiagnosticData
       (44, "This construct is deprecated. Use something else")
       (fun (obsoleteDiagnostic: ObsoleteDiagnosticExtendedData) ->
        Assert.Equal(None, obsoleteDiagnostic.DiagnosticId)
        Assert.Equal(None, obsoleteDiagnostic.UrlFormat))
       
[<FSharp.Test.FactForNETCOREAPP>]
let ``Warning -  ObsoleteDiagnosticExtendedData 08`` () =
    let CSLib =
        CSharp """
using System;
[Obsolete(DiagnosticId = "FS222", UrlFormat = "https://example.com")]
public static class Class1
{
    public static string Test()
    {
        return "Hello";
    }
}
    """
        |> withName "CSLib"

    let app =
        FSharp """
open MyLib

let text = Class1.Test();
    """ |> withReferences [CSLib]

    app
    |> typecheckResults
    |> checkDiagnosticData
       (44, "This construct is deprecated")
       (fun (obsoleteDiagnostic: ObsoleteDiagnosticExtendedData) ->
        Assert.Equal(Some "FS222", obsoleteDiagnostic.DiagnosticId)
        Assert.Equal(Some "https://example.com", obsoleteDiagnostic.UrlFormat))
       
[<FSharp.Test.FactForNETCOREAPP>]
let ``Warning - ObsoleteDiagnosticExtendedData 09`` () =
    FSharp """
open System
[<Obsolete>]
type MyClass() = class end

let x = MyClass()
"""
    |> typecheckResults
    |> checkDiagnosticData
       (44, "This construct is deprecated")
       (fun (obsoleteDiagnostic: ObsoleteDiagnosticExtendedData) ->
        Assert.Equal(None, obsoleteDiagnostic.DiagnosticId)
        Assert.Equal(None, obsoleteDiagnostic.UrlFormat))

[<FSharp.Test.FactForNETCOREAPP>]
let ``Warning -  ObsoleteDiagnosticExtendedData 10`` () =
    let CSLib =
        CSharp """
using System;
[Obsolete]
public static class Class1
{
    public static string Test()
    {
        return "Hello";
    }
}
    """
        |> withName "CSLib"

    let app =
        FSharp """
open MyLib

let text = Class1.Test();
    """ |> withReferences [CSLib]

    app
    |> typecheckResults
    |> checkDiagnosticData
       (44, "This construct is deprecated")
       (fun (obsoleteDiagnostic: ObsoleteDiagnosticExtendedData) ->
        Assert.Equal(None, obsoleteDiagnostic.DiagnosticId)
        Assert.Equal(None, obsoleteDiagnostic.UrlFormat))

[<FSharp.Test.FactForNETCOREAPP>]
let ``Error - ObsoleteDiagnosticExtendedData 01`` () =
    FSharp """
open System
[<Obsolete("Message", true, DiagnosticId = "FS222", UrlFormat = "https://example.com")>]
type MyClass() = class end

let x = MyClass()
"""
    |> typecheckResults
    |> checkDiagnosticData
       (101, "This construct is deprecated. Message")
       (fun (obsoleteDiagnostic: ObsoleteDiagnosticExtendedData) ->
        Assert.Equal(Some "FS222", obsoleteDiagnostic.DiagnosticId)
        Assert.Equal(Some "https://example.com", obsoleteDiagnostic.UrlFormat))
       
[<FSharp.Test.FactForNETCOREAPP>]
let ``Error - ObsoleteDiagnosticExtendedData 02`` () =
    FSharp """
open System
[<Obsolete("Message", true, DiagnosticId = "FS222")>]
type MyClass() = class end

let x = MyClass()
"""
    |> typecheckResults
    |> checkDiagnosticData
       (101, "This construct is deprecated. Message")
       (fun (obsoleteDiagnostic: ObsoleteDiagnosticExtendedData) ->
        Assert.Equal(Some "FS222", obsoleteDiagnostic.DiagnosticId)
        Assert.Equal(None, obsoleteDiagnostic.UrlFormat))
       
[<FSharp.Test.FactForNETCOREAPP>]
let ``Error -  ObsoleteDiagnosticExtendedData 03`` () =
    FSharp """
open System
[<Obsolete("Message", true)>]
type MyClass() = class end

let x = MyClass()
"""
    |> typecheckResults
    |> checkDiagnosticData
       (101, "This construct is deprecated. Message")
       (fun (obsoleteDiagnostic: ObsoleteDiagnosticExtendedData) ->
        Assert.Equal(None, obsoleteDiagnostic.DiagnosticId)
        Assert.Equal(None, obsoleteDiagnostic.UrlFormat))
       
[<FSharp.Test.FactForNETCOREAPP>]
let ``Error -  ObsoleteDiagnosticExtendedData 04`` () =
    FSharp """
open System
[<Obsolete("", true, DiagnosticId = "FS222", UrlFormat = "https://example.com")>]
type MyClass() = class end

let x = MyClass()
"""
    |> typecheckResults
    |> checkDiagnosticData
       (101, "This construct is deprecated")
       (fun (obsoleteDiagnostic: ObsoleteDiagnosticExtendedData) ->
        Assert.Equal(Some "FS222", obsoleteDiagnostic.DiagnosticId)
        Assert.Equal(Some "https://example.com", obsoleteDiagnostic.UrlFormat))

[<FSharp.Test.FactForNETCOREAPP>]
let ``Warning -  ExperimentalExtendedData 01`` () =
    let CSLib =
        CSharp """
using System.Diagnostics.CodeAnalysis;

[Experimental(diagnosticId: "FS222")]
public static class Class1
{
    public static string Test()
    {
        return "Hello";
    }
}
    """
        |> withName "CSLib"

    let app =
        FSharp """
open MyLib

let text = Class1.Test();
    """ |> withReferences [CSLib]

    app
    |> typecheckResults
    |> checkDiagnosticData
       (57, """This construct is experimental. This warning can be disabled using '--nowarn:57' or '#nowarn "57"'.""")
       (fun (experimental: ExperimentalExtendedData) ->
        Assert.Equal(Some "FS222", experimental.DiagnosticId)
        Assert.Equal(None, experimental.UrlFormat))
       
       
[<FSharp.Test.FactForNETCOREAPP>]
let ``Warning -  ExperimentalExtendedData 02`` () =
    let CSLib =
        CSharp """
using System.Diagnostics.CodeAnalysis;

[Experimental(diagnosticId: "FS222", UrlFormat = "https://example.com")]
public static class Class1
{
    public static string Test()
    {
        return "Hello";
    }
}
    """
        |> withName "CSLib"

    let app =
        FSharp """
open MyLib

let text = Class1.Test();
    """ |> withReferences [CSLib]

    app
    |> typecheckResults
    |> checkDiagnosticData
       (57, """This construct is experimental. This warning can be disabled using '--nowarn:57' or '#nowarn "57"'.""")
       (fun (experimental: ExperimentalExtendedData) ->
        Assert.Equal(Some "FS222", experimental.DiagnosticId)
        Assert.Equal(Some "https://example.com", experimental.UrlFormat))
       
[<FSharp.Test.FactForNETCOREAPP>]
let ``Warning -  ExperimentalExtendedData 03`` () =
    FSharp """
module Test

[<Experimental("Use with caution")>]
type Class1() =
    static member Test() = "Hello"

let text = Class1.Test();
    """
    |> typecheckResults
    |> checkDiagnosticData
       (57, """This construct is experimental. Use with caution. This warning can be disabled using '--nowarn:57' or '#nowarn "57"'.""")
       (fun (experimental: ExperimentalExtendedData) ->
        Assert.Equal(None, experimental.DiagnosticId)
        Assert.Equal(None, experimental.UrlFormat))