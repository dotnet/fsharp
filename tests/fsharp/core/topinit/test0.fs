// #Conformance #Interop 

module InstanceTests

let mutable trigger = 1

let check s b1 b2 = 
    if b1 = b2 then printfn "%s OK" s
    else (printfn "FAIL %s: expected %A, got %A" s b2 b1; exit 1)

module ClassWithInheritAndImmediateReferenceToThisInLet = 
    type B() = 
        member __.P = 1

    let id x = x

    type C() as x = 
        inherit B()
        let y = id x // it is ok to access the this pointer and pass it to external code as long as it doesn't call any members
        member __.ThisPointer1 = x
        member __.ThisPointer2 = y


    let checkA() = 
        let c = C()
        check "cwknecw021" c.ThisPointer1 c.ThisPointer2
   
module ClassWithNoInheritAndImmediateReferenceToThisInLet = 
    type C() as x = 
        let y = id x // it is ok to access the this pointer and pass it to external code as long as it doesn't call any members
        member __.ThisPointer1 = x
        member __.ThisPointer2 = y


    let checkB() = 
        let c = C()
        check "cwknecw021" c.ThisPointer1 c.ThisPointer2
 
module ClassWithInheritAndDelayedReferenceToThisInLet = 
    type B() = 
        member __.P = 1

    let id x = x

    type C() as x = 
        inherit B()
        let y() = x 
        member __.ThisPointer1 = x
        member __.ThisPointer2 = y()


    let checkC() = 
        let c = C()
        check "cwknecw021" c.ThisPointer1 c.ThisPointer2
   


module TestPassingThisToBase = 
    type B(thisC: unit -> obj) = 
        member __.ThisPointerFromC = thisC()

    let id x = x

    type C() as this = 
        inherit B(fun () -> box this)
        member c.ThisPointer1 = (c.ThisPointerFromC :?> C)
        member c.ThisPointer2 = this


    let checkD() = 
        let c = C()
        check "cwknecw022" c.ThisPointer1 c.ThisPointer2

module TestAccessingMemberFromBaseAfterInit = 
    let id x = x

    type B(thisC: unit -> C) = 
        member __.ThisPointerFromC = thisC()

    and C() as this = 
        inherit B(fun () -> this)
        do check "cwknecw023" this.ThisPointer1 this.ThisPointer2
        member c.ThisPointer1 = c.ThisPointerFromC 
        member c.ThisPointer2 = this

    let checkE() = 
        let c = C()
        ()

let checkAll() = 
    ClassWithInheritAndImmediateReferenceToThisInLet.checkA()
    ClassWithNoInheritAndImmediateReferenceToThisInLet.checkB()
    ClassWithInheritAndDelayedReferenceToThisInLet.checkC()
    TestPassingThisToBase.checkD()
    TestAccessingMemberFromBaseAfterInit.checkE()
    
