module ``FSharpQA-Tests-Conformance-DeclarationElements``

open NUnit.Framework

open NUnitConf
open RunPlTest



module AccessibilityAnnotations =

    module basic =
        [<Test; FSharpQASuiteTest("Conformance/DeclarationElements/AccessibilityAnnotations/basic")>]
        let basic () = runpl |> check

    module OnOverridesAndIFaceImpl =
        [<Test; FSharpQASuiteTest("Conformance/DeclarationElements/AccessibilityAnnotations/OnOverridesAndIFaceImpl")>]
        let OnOverridesAndIFaceImpl () = runpl |> check

    module OnTypeMembers =
        [<Test; FSharpQASuiteTest("Conformance/DeclarationElements/AccessibilityAnnotations/OnTypeMembers")>]
        let OnTypeMembers () = runpl |> check

    module PermittedLocations =
        [<Test; FSharpQASuiteTest("Conformance/DeclarationElements/AccessibilityAnnotations/PermittedLocations")>]
        let PermittedLocations () = runpl |> check


module CustomAttributes =


    module ArgumentsOfAllTypes =
        [<Test; FSharpQASuiteTest("Conformance/DeclarationElements/CustomAttributes/ArgumentsOfAllTypes")>]
        let ArgumentsOfAllTypes () = runpl |> check

    module AttributeUsage =
        [<Test; FSharpQASuiteTest("Conformance/DeclarationElements/CustomAttributes/AttributeUsage")>]
        let AttributeUsage () = runpl |> check

    module Basic =
        [<Test; FSharpQASuiteTest("Conformance/DeclarationElements/CustomAttributes/Basic")>]
        let Basic () = runpl |> check

    module ImportedAttributes =
        [<Test; FSharpQASuiteTest("Conformance/DeclarationElements/CustomAttributes/ImportedAttributes")>]
        let ImportedAttributes () = runpl |> check


module Events =

    module basic =

        [<Test; FSharpQASuiteTest("Conformance/DeclarationElements/Events/basic")>]
        let basic () = runpl |> check


module FieldMembers =

    [<Test; FSharpQASuiteTest("Conformance/DeclarationElements/FieldMembers")>]
    let FieldMembers () = runpl |> check


module ImportDeclarations =

    [<Test; FSharpQASuiteTest("Conformance/DeclarationElements/ImportDeclarations")>]
    let ImportDeclarations () = runpl |> check


module InterfaceSpecificationsAndImplementations =

    [<Test; FSharpQASuiteTest("Conformance/DeclarationElements/InterfaceSpecificationsAndImplementations")>]
    let InterfaceSpecificationsAndImplementations () = runpl |> check


module LetBindings =

    module ActivePatternBindings =

        [<Test; FSharpQASuiteTest("Conformance/DeclarationElements/LetBindings/ActivePatternBindings")>]
        let ActivePatternBindings () = runpl |> check

    module Basic =

        [<Test; FSharpQASuiteTest("Conformance/DeclarationElements/LetBindings/Basic")>]
        let Basic () = runpl |> check

    module ExplicitTypeParameters =

        [<Test; FSharpQASuiteTest("Conformance/DeclarationElements/LetBindings/ExplicitTypeParameters")>]
        let ExplicitTypeParameters () = runpl |> check

    module TypeFunctions =

        [<Test; FSharpQASuiteTest("Conformance/DeclarationElements/LetBindings/TypeFunctions")>]
        let TypeFunctions () = runpl |> check


module MemberDefinitions =

    [<Test; FSharpQASuiteTest("Conformance/DeclarationElements/MemberDefinitions")>]
    let MemberDefinitions () = runpl |> check


module ModuleAbbreviations =

    [<Test; FSharpQASuiteTest("Conformance/DeclarationElements/ModuleAbbreviations")>]
    let ModuleAbbreviations () = runpl |> check


module ObjectConstructors =

    [<Test; FSharpQASuiteTest("Conformance/DeclarationElements/ObjectConstructors")>]
    let ObjectConstructors () = runpl |> check


module PinvokeDeclarations =

    [<Test; FSharpQASuiteTest("Conformance/DeclarationElements/P-invokeDeclarations")>]
    let PinvokeDeclarations () = runpl |> check
