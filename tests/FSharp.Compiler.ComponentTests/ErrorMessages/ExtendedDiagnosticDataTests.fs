module ErrorMessages.ExtendedDiagnosticData
#nowarn "57"

open FSharp.Compiler.Diagnostics
open FSharp.Compiler.Diagnostics.ExtendedData
open FSharp.Test
open FSharp.Test.Compiler
open Xunit

let inline checkDiagnostic
    (diagnosticNumber, message)
    (check: 'a -> unit)
    (checkResults: 'b when 'b: (member Diagnostics: FSharpDiagnostic[])) =
    match checkResults.Diagnostics |> Array.tryFind (fun d -> d.ErrorNumber = diagnosticNumber) with
    | None -> failwith "Expected diagnostic not found"
    | Some diagnostic ->

    Assert.Equal(message, diagnostic.Message)
    match diagnostic.ExtendedData with
    | Some(:? 'a as data) -> check data
    | _ -> failwith "Expected diagnostic extended data not found"


[<Fact>]
let ``TypeMismatchDiagnosticExtendedData 01`` () =
    FSharp """
let x, y, z = 1, 2
"""
    |> typecheckResults
    |> checkDiagnostic
       (1, "Type mismatch. Expecting a tuple of length 3 of type\n    'a * 'b * 'c    \nbut given a tuple of length 2 of type\n    int * int    \n")
       (fun (typeMismatch: TypeMismatchDiagnosticExtendedData) ->
        Assert.Equal(DiagnosticContextInfo.NoContext, typeMismatch.ContextInfo)
        let displayContext = typeMismatch.DisplayContext
        Assert.Equal("obj * obj * obj", typeMismatch.ExpectedType.Format(displayContext))
        Assert.Equal("int * int", typeMismatch.ActualType.Format(displayContext)))

[<Fact>]
let ``TypeMismatchDiagnosticExtendedData 02`` () =
    FSharp """
let x, y = 1
"""
    |> typecheckResults
    |> checkDiagnostic
       (1, "This expression was expected to have type\n    ''a * 'b'    \nbut here has type\n    'int'    ")
       (fun (typeMismatch: TypeMismatchDiagnosticExtendedData) ->
        let displayContext = typeMismatch.DisplayContext
        Assert.Equal(DiagnosticContextInfo.NoContext, typeMismatch.ContextInfo)
        Assert.Equal("obj * obj", typeMismatch.ExpectedType.Format(displayContext))
        Assert.Equal("int", typeMismatch.ActualType.Format(displayContext)))

[<Fact>]
let ``TypeMismatchDiagnosticExtendedData 03`` () =
    FSharp """
if true then 5
"""
    |> typecheckResults
    |> checkDiagnostic
       (1, "This 'if' expression is missing an 'else' branch. Because 'if' is an expression, and not a statement, add an 'else' branch which also returns a value of type 'int'.")
       (fun (typeMismatch: TypeMismatchDiagnosticExtendedData) ->
        let displayContext = typeMismatch.DisplayContext
        Assert.Equal(DiagnosticContextInfo.OmittedElseBranch, typeMismatch.ContextInfo)
        Assert.Equal("unit", typeMismatch.ExpectedType.Format(displayContext))
        Assert.Equal("int", typeMismatch.ActualType.Format(displayContext)))

[<Fact>]
let ``TypeMismatchDiagnosticExtendedData 04`` () =
    FSharp """
1 :> string
"""
    |> typecheckResults
    |> checkDiagnostic
       (193, "Type constraint mismatch. The type \n    'int'    \nis not compatible with type\n    'string'    \n")
       (fun (typeMismatch: TypeMismatchDiagnosticExtendedData) ->
        let displayContext = typeMismatch.DisplayContext
        Assert.Equal(DiagnosticContextInfo.NoContext, typeMismatch.ContextInfo)
        Assert.Equal("string", typeMismatch.ExpectedType.Format(displayContext))
        Assert.Equal("int", typeMismatch.ActualType.Format(displayContext)))

[<Fact>]
let ``TypeMismatchDiagnosticExtendedData 05`` () =
    FSharp """
match 0 with
| 0 -> 1
| 1 -> "a"
"""
    |> typecheckResults
    |> checkDiagnostic
       (1, "All branches of a pattern match expression must return values implicitly convertible to the type of the first branch, which here is 'int'. This branch returns a value of type 'string'.")
       (fun (typeMismatch: TypeMismatchDiagnosticExtendedData) ->
        let displayContext = typeMismatch.DisplayContext
        Assert.Equal(DiagnosticContextInfo.FollowingPatternMatchClause, typeMismatch.ContextInfo)
        Assert.Equal("int", typeMismatch.ExpectedType.Format(displayContext))
        Assert.Equal("string", typeMismatch.ActualType.Format(displayContext)))

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
    |> checkDiagnostic
       (1, "This expression was expected to have type\n    'int'    \nbut here has type\n    'string'    ")
       (fun (typeMismatch: TypeMismatchDiagnosticExtendedData) ->
        let displayContext = typeMismatch.DisplayContext
        Assert.Equal(DiagnosticContextInfo.FollowingPatternMatchClause, typeMismatch.ContextInfo) //Should be NoContext
        Assert.Equal("int", typeMismatch.ExpectedType.Format(displayContext))
        Assert.Equal("string", typeMismatch.ActualType.Format(displayContext)))

[<Fact>]
let ``TypeMismatchDiagnosticExtendedData 06`` () =
    FSharp """
let _: bool =
    if true then "a" else "b"
"""
    |> typecheckResults
    |> checkDiagnostic
       (1, "The 'if' expression needs to have type 'bool' to satisfy context type requirements. It currently has type 'string'.")
       (fun (typeMismatch: TypeMismatchDiagnosticExtendedData) ->
        let displayContext = typeMismatch.DisplayContext
        Assert.Equal(DiagnosticContextInfo.IfExpression, typeMismatch.ContextInfo)
        Assert.Equal("bool", typeMismatch.ExpectedType.Format(displayContext))
        Assert.Equal("string", typeMismatch.ActualType.Format(displayContext)))

[<Fact>]
let ``TypeMismatchDiagnosticExtendedData 07`` () =
    FSharp """
if true then 1 else "a"
"""
    |> typecheckResults
    |> checkDiagnostic
       (1, "All branches of an 'if' expression must return values implicitly convertible to the type of the first branch, which here is 'int'. This branch returns a value of type 'string'.")
       (fun (typeMismatch: TypeMismatchDiagnosticExtendedData) ->
        let displayContext = typeMismatch.DisplayContext
        Assert.Equal(DiagnosticContextInfo.ElseBranchResult, typeMismatch.ContextInfo)
        Assert.Equal("int", typeMismatch.ExpectedType.Format(displayContext))
        Assert.Equal("string", typeMismatch.ActualType.Format(displayContext)))

[<Fact>]
let ``ArgumentsInSigAndImplMismatchExtendedData 01`` () =
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
    |> typecheckProject true
    |> checkDiagnostic
       (3218, "The argument names in the signature 'x' and implementation 'y' do not match. The argument name from the signature file will be used. This may cause problems when debugging or profiling.")
       (fun (argsMismatch: ArgumentsInSigAndImplMismatchExtendedData) ->
        Assert.Equal("x", argsMismatch.SignatureName)
        Assert.Equal("y", argsMismatch.ImplementationName)
        Assert.True(argsMismatch.SignatureRange.FileName.EndsWith("fsi"))
        Assert.True(argsMismatch.ImplementationRange.FileName.EndsWith("fs")))

[<Fact>]
let ``FieldNotContainedDiagnosticExtendedData 01`` () =
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
    |> typecheckProject true
    |> checkDiagnostic
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
    |> checkDiagnostic
       (193, "This expression is a function value, i.e. is missing arguments. Its type is 'a -> 'a.")
       (fun (wrongType: ExpressionIsAFunctionExtendedData) ->
        Assert.Equal("type 'a -> 'a", wrongType.ActualType.ToString()))
