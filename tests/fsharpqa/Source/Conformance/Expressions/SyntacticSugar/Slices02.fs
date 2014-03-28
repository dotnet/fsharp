// #Conformance #SyntacticSugar 
#light

// Verify slices syntax on types with a get slice method

type Foo<'a>() =
    let mutable m_lastLB : 'a option = None
    let mutable m_lastUB : 'a option = None

    member this.GetSlice(lb, ub) = 
        m_lastLB <- lb
        m_lastUB <- ub
        ()

    member this.LastLB = m_lastLB
    member this.LastUB = m_lastUB


let f = new Foo<char>()

let _ = f.['a'..'z']
if f.LastLB <> Some('a') then exit 1
if f.LastUB <> Some('z') then exit 1


let _ = f.['?'..]
if f.LastLB <> Some('?') then exit 1
if f.LastUB <> None      then exit 1

let _ = f.[..'~']
if f.LastLB <> None      then exit 1
if f.LastUB <> Some('~') then exit 1

let _ = f.[*]
if f.LastLB <> None      then exit 1
if f.LastUB <> None      then exit 1

exit 0
