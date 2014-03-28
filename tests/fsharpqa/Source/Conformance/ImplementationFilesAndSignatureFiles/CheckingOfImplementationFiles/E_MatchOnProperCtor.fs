// #Conformance #SignatureFiles  

// Regression test for bug 6465
namespace MyNamespace

type MyOtherType(_member : int ) =
    member x.Member = _member

type MyType ( _member1 : int , _member2 : int) =
    member x.Member1 = _member1
    member x.Member2 = _member2

