// #Conformance #SyntacticSugar 
// Verify multiple ways to slice a type: [*,

type Foo<'a>() =
    let mutable m_last1LB : 'a option = None
    let mutable m_last1UB : 'a option = None    
    
    let mutable m_last2LB1 : 'a option = None
    let mutable m_last2UB1 : 'a option = None

    let mutable m_last2LB2 : 'a option = None
    let mutable m_last2UB2 : 'a option = None

    member this.GetSlice(lb1, ub1, lb2, ub2) = 
        m_last2LB1 <- lb1
        m_last2UB1 <- ub1
        m_last2LB2 <- lb2
        m_last2UB2 <- ub2
        ()
                
    member this.Last1LB = m_last1LB
    member this.Last1UB = m_last1UB

    member this.Last2LB1 = m_last2LB1
    member this.Last2UB1 = m_last2UB1
    member this.Last2LB2 = m_last2LB2
    member this.Last2UB2 = m_last2UB2

// -------------------------------

let f = new Foo<char>()

// 2D slice
let _ = f.[*, '0'..'9']
if f.Last2LB1 <> None then exit 1
if f.Last2UB1 <> None then exit 1
if f.Last2LB2 <> Some('0') then exit 1
if f.Last2UB2 <> Some('9') then exit 1

let _ = f.['a'..'z', *]
if f.Last2LB1 <> Some('a') then exit 1
if f.Last2UB1 <> Some('z') then exit 1
if f.Last2LB2 <> None then exit 1
if f.Last2UB2 <> None then exit 1

exit 0
