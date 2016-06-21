module ``FSharpQA-Tests-Conformance-ObjectOrientedTypeDefinitions``

open NUnit.Framework

open NUnitConf
open RunPlTest



module AbstractMembers =

    [<Test; FSharpQASuiteTest("Conformance/ObjectOrientedTypeDefinitions/AbstractMembers")>]
    let AbstractMembers () = runpl |> check


module ClassTypes =

    module AsDeclarations =
    
        [<Test; FSharpQASuiteTest("Conformance/ObjectOrientedTypeDefinitions/ClassTypes/AsDeclarations")>]
        let AsDeclarations () = runpl |> check

    module AutoProperties =

        [<Test; FSharpQASuiteTest("Conformance/ObjectOrientedTypeDefinitions/ClassTypes/AutoProperties")>]
        let AutoProperties () = runpl |> check

    module ExplicitFields =

        [<Test; FSharpQASuiteTest("Conformance/ObjectOrientedTypeDefinitions/ClassTypes/ExplicitFields")>]
        let ExplicitFields () = runpl |> check

    module ExplicitObjectConstructors =

        [<Test; FSharpQASuiteTest("Conformance/ObjectOrientedTypeDefinitions/ClassTypes/ExplicitObjectConstructors")>]
        let ExplicitObjectConstructors () = runpl |> check

    module ImplicitObjectConstructors =

        [<Test; FSharpQASuiteTest("Conformance/ObjectOrientedTypeDefinitions/ClassTypes/ImplicitObjectConstructors")>]
        let ImplicitObjectConstructors () = runpl |> check

    module InheritsDeclarations =

        [<Test; FSharpQASuiteTest("Conformance/ObjectOrientedTypeDefinitions/ClassTypes/InheritsDeclarations")>]
        let InheritsDeclarations () = runpl |> check

    module LetDoDeclarations =

        [<Test; FSharpQASuiteTest("Conformance/ObjectOrientedTypeDefinitions/ClassTypes/LetDoDeclarations")>]
        let LetDoDeclarations () = runpl |> check

    module MemberDeclarations =

        [<Test; FSharpQASuiteTest("Conformance/ObjectOrientedTypeDefinitions/ClassTypes/MemberDeclarations")>]
        let MemberDeclarations () = runpl |> check

    module Misc =

        [<Test; FSharpQASuiteTest("Conformance/ObjectOrientedTypeDefinitions/ClassTypes/Misc")>]
        let Misc () = runpl |> check

    module StaticLetDoDeclarations =

        [<Test; FSharpQASuiteTest("Conformance/ObjectOrientedTypeDefinitions/ClassTypes/StaticLetDoDeclarations")>]
        let StaticLetDoDeclarations () = runpl |> check

    module ValueRestriction =

        [<Test; FSharpQASuiteTest("Conformance/ObjectOrientedTypeDefinitions/ClassTypes/ValueRestriction")>]
        let ValueRestriction () = runpl |> check


module DelegateTypes =

    [<Test; FSharpQASuiteTest("Conformance/ObjectOrientedTypeDefinitions/DelegateTypes")>]
    let DelegateTypes () = runpl |> check


module EnumTypes =

    [<Test; FSharpQASuiteTest("Conformance/ObjectOrientedTypeDefinitions/EnumTypes")>]
    let EnumTypes () = runpl |> check


module InterfaceTypes =

    [<Test; FSharpQASuiteTest("Conformance/ObjectOrientedTypeDefinitions/InterfaceTypes")>]
    let InterfaceTypes () = runpl |> check


module StructTypes =

    [<Test; FSharpQASuiteTest("Conformance/ObjectOrientedTypeDefinitions/StructTypes")>]
    let StructTypes () = runpl |> check



module TypeExtensions =

    module basic =

        [<Test; FSharpQASuiteTest("Conformance/ObjectOrientedTypeDefinitions/TypeExtensions/basic")>]
        let basic () = runpl |> check

    module intrinsic =

        [<Test; FSharpQASuiteTest("Conformance/ObjectOrientedTypeDefinitions/TypeExtensions/intrinsic")>]
        let intrinsic () = runpl |> check

    module optional = 

        [<Test; FSharpQASuiteTest("Conformance/ObjectOrientedTypeDefinitions/TypeExtensions/optional")>]
        let optional () = runpl |> check


module TypeKindInference =

    [<Test; FSharpQASuiteTest("Conformance/ObjectOrientedTypeDefinitions/TypeKindInference")>]
    let TypeKindInference () = runpl |> check
