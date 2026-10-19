module FSharp.Compiler.Service.Tests.TooltipMembersTests

open System
open Xunit
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Symbols
open FSharp.Compiler.Tokenization

[<Fact>]
let ``Regression.InDeclaration.Bug3176c`` () =
    assertTooltipContains
        "aaaa"
        """type C =
                val aa{caret}aa: int"""

[<Fact>]
let ``Declaration.CyclicalDeclarationDoesNotCrash`` () =
    assertTooltipContains "type A" """type (*1*)A = int * A{caret} """

[<Fact>]
let ``LongPaths`` () =
    let source =
        String.concat
            "\n"
            [ "let test0 = System.Console.In"
              "let test0b = System.Collections.Generic.List<int>()"
              "let test0c = System.Collections.Generic.KeyNotFoundException()"
              "type Test0d = System.Collections.Generic.List<int>"
              "type Test0e = System.Collections.Generic.KeyNotFoundException" ]

    let walk = EditorServiceAsserts.walk source

    walk "let test0 = " "System" "namespace System"
    walk "let test0 = System." "Console" "Console ="
    walk "let test0 = System.Console." "In" "System.Console.In"
    walk "let test0 = System.Console." "In" "TextReader"
    walk "let test0b = " "System" "namespace System"
    walk "let test0b = System." "Collections" "namespace System.Collections"
    walk "let test0b = System.Collections." "Generic" "namespace System.Collections.Generic"
    walk "let test0b = System.Collections.Generic." "List" "List()"
    walk "let test0c = " "System" "namespace System"
    walk "let test0c = System." "Collections" "namespace System.Collections"
    walk "let test0c = System.Collections." "Generic" "namespace System.Collections.Generic"
    walk "let test0c = System.Collections.Generic." "KeyNotFoundException" "KeyNotFoundException()"
    walk "type Test0d = " "System" "namespace System"
    walk "type Test0d = System." "Collections" "namespace System.Collections"
    walk "type Test0d = System.Collections." "Generic" "namespace System.Collections.Generic"
    walk "type Test0d = System.Collections.Generic." "List" "Generic.List"
    walk "type Test0e = " "System" "namespace System"
    walk "type Test0e = System." "Collections" "namespace System.Collections"
    walk "type Test0e = System.Collections." "Generic" "namespace System.Collections.Generic"
    walk "type Test0e = System.Collections.Generic." "KeyNotFoundException" "Generic.KeyNotFoundException"

[<Fact>]
let ``AtEndOfLine`` () =
    let (ToolTipText elements) = Checker.getTooltip "//{caret}"

    let meaningfulElements =
        elements
        |> List.filter (function
            | ToolTipElement.None -> false
            | _ -> true)

    match meaningfulElements with
    | [] -> ()
    | _ -> failwithf "Expected an empty tooltip at the end of a comment line, but got: %A" elements

#if !NETCOREAPP
let private getTooltipWithoutSystemDrawing (markedSource: string) =
    getTooltipWithReferences
        "MissingDependencyReferences"
        [ fsCoreDefaultReference ()
          sysLib "mscorlib"
          sysLib "System"
          sysLib "System.Core"
          sysLib "System.Windows.Forms" ] // System.Drawing.dll omitted on purpose (Bug 5409's missing transitive dependency)
        markedSource

[<Fact>]
let ``MissingDependencyReferences.QuickInfo.Bug5409`` () =
    let actual =
        getTooltipWithoutSystemDrawing "let myFo{caret}rm = new System.Windows.Forms.Form()"
        |> foldToolTip

    if not (actual.Contains "Form") then
        failwithf "Expected tooltip to contain %A when System.Drawing is absent, but the actual tooltip was:\n%s" "Form" actual
#endif

[<Fact>]
let ``Regression.Bug4642`` () =
    assertTooltipContains "int -> char" """ "AA".Ch{caret}ars """

[<Fact>]
let ``Regression.MemberDefinition.DocComments.Bug5856_10`` () =
    let source = "let _ = System.String.Form{caret}at"
    assertTooltipContains "System.String.Format(" source

    match (Checker.getSymbolUse source).Symbol with
    | :? FSharpMemberOrFunctionOrValue as m ->
        if m.XmlDocSig <> "M:System.String.Format(System.String,System.Object[])" then
            failwithf "Unexpected XmlDocSig for String.Format: %s" m.XmlDocSig

        let expectedAssembly =
#if NETCOREAPP
            "System.Runtime.dll"
#else
            "mscorlib.dll"
#endif
        let basename = m.Assembly.FileName |> Option.map System.IO.Path.GetFileName

        if basename <> Some expectedAssembly then
            failwithf "Expected String.Format to be defined in %s, but got %A" expectedAssembly basename
    | sym -> failwithf "Expected a member symbol for String.Format, but got %A" sym

[<Fact>]
let ``Regression.MemberDefinition.DocComments.Bug5856_13`` () =
    assertTooltipContainsInOrder
        [ "type KeyCollection<"
          "member CopyTo"
          """<summary>Represents the collection of keys in a <see cref="T:System.Collections.Generic.Dictionary`2" />. This class cannot be inherited.</summary>""" ]
        "let _ = typeof<System.Collections.Generic.Dictionary<int, int>.KeyColl{caret}ection>"

[<Fact>]
let ``Regression.MemberDefinition.DocComments.Bug5856_14`` () =
    assertTooltipContainsInOrder
        [ "type ArgumentException"
          "member Message"
          "<summary>The exception that is thrown when one of the arguments provided to a method is not valid.</summary" ]
        "let _ = typeof<System.ArgumentExcep{caret}tion>"

[<Fact>]
let ``Regression.MemberDefinition.DocComments.Bug5856_15`` () =
    assertTooltipContainsInOrder
        [ "property System.AppDomain.CurrentDomain: System.AppDomain"
          """<summary>Gets the current application domain for the current <see cref="T:System.Threading.Thread" />.</summary>""" ]
        "let _ = System.AppDomain.CurrentDom{caret}ain"
