// #Conformance #SyntacticSugar 
#light

// Verify slices syntax on types with get and set slice 2D methods

type Foo<'a>() =
    let mutable m_lastLB1 : 'a option = None
    let mutable m_lastUB1 : 'a option = None

    let mutable m_lastLB2 : 'a option = None
    let mutable m_lastUB2 : 'a option = None

    let mutable m_lasti1 : seq<'a> = Seq.empty
    let mutable m_lasti2 : seq<'a> = Seq.empty

    let mutable m_lastFixed1 : 'a = Unchecked.defaultof<_>
    let mutable m_lastFixed2 : 'a = Unchecked.defaultof<_>

    let mutable m_lastV1D : 'a list = List.empty
    let mutable m_lastV2D : 'a list list = [List.empty]

    member this.GetSlice(lb1, ub1, lb2, ub2) = 
        m_lastLB1 <- lb1
        m_lastUB1 <- ub1
        m_lastLB2 <- lb2
        m_lastUB2 <- ub2
        ()

    member this.SetSlice(lb1, ub1, lb2, ub2, v2D) = 
        m_lastLB1 <- lb1
        m_lastUB1 <- ub1
        m_lastLB2 <- lb2
        m_lastUB2 <- ub2
        m_lastV2D <- v2D
        ()

    member this.GetSlice(lb1, ub1, f2) = 
        m_lastLB1 <- lb1
        m_lastUB1 <- ub1
        m_lastFixed2 <- f2
        ()

    member this.SetSlice(lb1, ub1, f2, v1D) = 
        m_lastLB1 <- lb1
        m_lastUB1 <- ub1
        m_lastFixed2 <- f2
        m_lastV1D <- v1D
        ()

    member this.GetSlice(f1, lb2, ub2) = 
        m_lastFixed1 <- f1
        m_lastLB2 <- lb2
        m_lastUB2 <- ub2        
        ()

    member this.SetSlice(f1, lb2, ub2, v1D) = 
        m_lastFixed1 <- f1
        m_lastLB2 <- lb2
        m_lastUB2 <- ub2
        m_lastV1D <- v1D      
        ()

    member this.GetSlice(i1 : seq<_>, lb2, ub2) = 
        m_lasti1 <- i1
        m_lastLB2 <- lb2
        m_lastUB2 <- ub2
        ()

    member this.SetSlice(i1 : seq<_>, lb2, ub2, v2D) = 
        m_lasti1 <- i1
        m_lastLB2 <- lb2
        m_lastUB2 <- ub2
        m_lastV2D <- v2D
        ()

    member this.GetSlice(lb1, ub1, i2 : seq<_>) = 
        m_lastLB1 <- lb1
        m_lastUB1 <- ub1
        m_lasti2 <- i2
        ()

    member this.SetSlice(lb1, ub1, i2 : seq<_>, v2D) = 
        m_lastLB1 <- lb1
        m_lastUB1 <- ub1
        m_lasti2 <- i2
        m_lastV2D <- v2D
        ()


    member this.LastLB1 = m_lastLB1
    member this.LastUB1 = m_lastUB1
    member this.LastLB2 = m_lastLB2
    member this.LastUB2 = m_lastUB2
    member this.LastI1  = m_lasti1
    member this.LastI2  = m_lasti2
    member this.LastF1  = m_lastFixed1
    member this.LastF2  = m_lastFixed2
    member this.LastV1D = m_lastV1D
    member this.LastV2D = m_lastV2D


// -------------------------------

let f = new Foo<char>()

let _ = f.['a'..'z', '0'..'9']
if f.LastLB1 <> Some('a') then exit 1
if f.LastUB1 <> Some('z') then exit 1
if f.LastLB2 <> Some('0') then exit 1
if f.LastUB2 <> Some('9') then exit 1

f.['b'..'y', '1'..'8'] <- [['x']]
if f.LastLB1 <> Some('b') then exit 1
if f.LastUB1 <> Some('y') then exit 1
if f.LastLB2 <> Some('1') then exit 1
if f.LastUB2 <> Some('8') then exit 1
if f.LastV2D <> [['x']] then exit 1

let _ = f.['?'.., ..'!']
if f.LastLB1 <> Some('?') then exit 1
if f.LastUB1 <> None      then exit 1
if f.LastLB2 <> None      then exit 1
if f.LastUB2 <> Some('!') then exit 1

f.['/'.., ..'1'] <- [['d']]
if f.LastLB1 <> Some('/') then exit 1
if f.LastUB1 <> None      then exit 1
if f.LastLB2 <> None      then exit 1
if f.LastUB2 <> Some('1') then exit 1
if f.LastV2D <> [['d']] then exit 1

let _ = f.[..'~', '^'..]
if f.LastLB1 <> None      then exit 1
if f.LastUB1 <> Some('~') then exit 1
if f.LastLB2 <> Some('^') then exit 1
if f.LastUB2 <> None      then exit 1

let _ = f.[*, *]
if f.LastLB1 <> None      then exit 1
if f.LastUB1 <> None      then exit 1
if f.LastLB2 <> None      then exit 1
if f.LastUB2 <> None      then exit 1

let _ = f.[None, *, None]
if f.LastLB1 <> None      then exit 1
if f.LastUB1 <> None      then exit 1
if f.LastLB2 <> None      then exit 1
if f.LastUB2 <> None      then exit 1

let _ = f.[['a'; 'b'], *]
if f.LastI1  <> Seq.ofList ['a'; 'b'] then exit 1
if f.LastLB2 <> None      then exit 1
if f.LastUB2 <> None      then exit 1

f.[['t'; 'g'], *] <- [['f']]
if f.LastI1  <> Seq.ofList ['t'; 'g'] then exit 1
if f.LastLB2 <> None      then exit 1
if f.LastUB2 <> None      then exit 1
if f.LastV2D <> [['f']]   then exit 1

let _ = f.[*, ['c'; 'd']]
if f.LastI2  <> Seq.ofList ['c'; 'd'] then exit 1
if f.LastLB1 <> None      then exit 1
if f.LastUB1 <> None      then exit 1

let _ = f.['x', *]
if f.LastF1  <> 'x' then exit 1
if f.LastLB2 <> None      then exit 1
if f.LastUB2 <> None      then exit 1

f.['z', *] <- ['i']
if f.LastF1  <> 'z' then exit 1
if f.LastLB2 <> None      then exit 1
if f.LastUB2 <> None      then exit 1
if f.LastV1D <> ['i']  then exit 1

let _ = f.['x', ..'y']
if f.LastF1  <> 'x' then exit 1
if f.LastLB2 <> None      then exit 1
if f.LastUB2 <> Some('y') then exit 1

let _ = f.['q'.., 'p']
if f.LastF2  <> 'p' then exit 1
if f.LastLB1 <> Some('q') then exit 1
if f.LastUB1 <> None      then exit 1

let _ = f.['v'..'d', 'h']
if f.LastLB1 <> Some('v') then exit 1
if f.LastUB1 <> Some('d') then exit 1
if f.LastF2  <> 'h' then exit 1

f.['r'..'w', 't'] <- ['b']
if f.LastLB1 <> Some('r') then exit 1
if f.LastUB1 <> Some('w') then exit 1
if f.LastF2  <> 't' then exit 1
if f.LastV1D <> ['b'] then exit 1

exit 0