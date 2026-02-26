// #Regression #Conformance #ObjectOrientedTypes #InterfacesAndImplementations 
// Regression test for FSHARP1.0:3748
// This code used to compile and then throw at runtime
// Now we emit an error.

//<Expects id="FS0366" span="(31,19-31,45)" status="error">No implementation was given for 'abstract INodeWrapper\.Node: Node'\. Note that all interface members must be implemented and listed under an appropriate 'interface' declaration, e\.g\. 'interface \.\.\. with member \.\.\.'</Expects>

namespace Foo

    open System

    type Node =
        TestVariableNode of seq<Object>
      | InterleavingNode of seq<Node>
      | SynthesizingNode of seq<Node> * Delegate

    type INodeWrapper =
        abstract member Node: Node


    open System.Collections


    type ITestCaseEnumeratorFactory =
        inherit INodeWrapper
        abstract member CreateEnumerator: uint32 -> IEnumerator;
        abstract member MaximumStrength: System.UInt32;

    [<AbstractClass>]
    type TestCaseEnumeratorFactoryCommonImplementation () as this =
        interface ITestCaseEnumeratorFactory with
            override this.CreateEnumerator desiredStrength =
                (Seq.empty :> IEnumerable).GetEnumerator ()    // Stub
            override this.MaximumStrength =
                9u    // Stub

    type TestVariableLevelEnumeratorFactory (levels: seq<Object>) =
            inherit TestCaseEnumeratorFactoryCommonImplementation ()
            let node =
               TestVariableNode levels

    module Throwaway =
        let foo =
            TestVariableLevelEnumeratorFactory Seq.empty // Surely I can't do this at compile time?

