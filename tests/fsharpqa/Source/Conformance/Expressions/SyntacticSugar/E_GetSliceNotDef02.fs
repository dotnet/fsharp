// #Regression #Conformance #SyntacticSugar 
#light

// Verify error using a 1D slice if only a 2D version added
//<Expects id="FS0501" status="error" span="(31,9)">The member or object constructor 'GetSlice' takes 4 argument\(s\) but is here given 2\. The required signature is 'member Foo\.GetSlice: lb1: 'a option \* ub1: 'a option \* lb2: 'a option \* ub2: 'a option -> unit'</Expects>

type Foo<'a>() =
    let mutable m_lastLB1 : 'a option = None
    let mutable m_lastUB1 : 'a option = None

    let mutable m_lastLB2 : 'a option = None
    let mutable m_lastUB2 : 'a option = None


    member this.GetSlice(lb1, ub1, lb2, ub2) = 
        m_lastLB1 <- lb1
        m_lastUB1 <- ub1
        m_lastLB2 <- lb2
        m_lastUB2 <- ub2
        ()

    member this.LastLB1 = m_lastLB1
    member this.LastUB1 = m_lastUB1
    member this.LastLB2 = m_lastLB2
    member this.LastUB2 = m_lastUB2

// -------------------------------

let f = new Foo<char>()

let _ = f.[*]

exit 1
