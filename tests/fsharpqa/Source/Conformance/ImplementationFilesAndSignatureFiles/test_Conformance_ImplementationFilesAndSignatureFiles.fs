module ``FSharpQA-Tests-Conformance-ImplementationFilesAndSignatureFiles``

open NUnit.Framework

open NUnitConf
open RunPlTest


module CheckingOfImplementationFiles =

    [<Test; FSharpQASuiteTest("Conformance/ImplementationFilesAndSignatureFiles/CheckingOfImplementationFiles")>]
    let CheckingOfImplementationFiles () = runpl |> check


module InitializationSemanticsForImplementationFiles =

    [<Test; FSharpQASuiteTest("Conformance/ImplementationFilesAndSignatureFiles/InitializationSemanticsForImplementationFiles")>]
    let InitializationSemanticsForImplementationFiles () = runpl |> check


module NamespacesFragmentsAndImplementationFiles =

    module Basic =

        [<Test; FSharpQASuiteTest("Conformance/ImplementationFilesAndSignatureFiles/NamespacesFragmentsAndImplementationFiles/basic")>]
        let Basic () = runpl |> check

    module Global =

        [<Test; FSharpQASuiteTest("Conformance/ImplementationFilesAndSignatureFiles/NamespacesFragmentsAndImplementationFiles/global")>]
        let Global () = runpl |> check


module SignatureFiles =

    [<Test; FSharpQASuiteTest("Conformance/ImplementationFilesAndSignatureFiles/SignatureFiles")>]
    let SignatureFiles () = runpl |> check

