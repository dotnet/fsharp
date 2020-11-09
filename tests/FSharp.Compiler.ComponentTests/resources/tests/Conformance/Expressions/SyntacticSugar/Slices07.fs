// #Conformance #SyntacticSugar 
// Verify 1D slices.

type Foo<'a>() =
    let mutable m_lastLB : 'a option = None
    let mutable m_lastUB : 'a option = None    
    let mutable m_lastValue : 'a option = None    
    
    member this.GetSlice(lb, ub) = 
        m_lastLB <- lb
        m_lastUB <- ub
        ()

    member this.SetSlice(lb, ub, value) = 
        m_lastLB <- lb
        m_lastUB <- ub
        m_lastValue <- Some value
        ()
                
    member this.LastLB = m_lastLB
    member this.LastUB = m_lastUB
    member this.LastValue = m_lastValue

// -------------------------------

let f = new Foo<char>()

// GetSlice
let _ = f.['0'..'9']
if f.LastLB <> Some('0') then exit 1
if f.LastUB <> Some('9') then exit 1

let _ = f.['a'..]
if f.LastLB <> Some('a') then exit 1
if f.LastUB <> None then exit 1

let _ = f.[..'z']
if f.LastLB <> None then exit 1
if f.LastUB <> Some('z') then exit 1

let _ = f.[*]
if f.LastLB <> None then exit 1
if f.LastUB <> None then exit 1

// SetSlice
f.['0'..'9'] <- 'a'
if f.LastLB <> Some('0') then exit 1
if f.LastUB <> Some('9') then exit 1
if f.LastValue <> Some('a') then exit 1

f.['a'..] <- 'A'
if f.LastLB <> Some('a') then exit 1
if f.LastUB <> None then exit 1
if f.LastValue <> Some('A') then exit 1

f.[..'z'] <- '0'
if f.LastLB <> None then exit 1
if f.LastUB <> Some('z') then exit 1
if f.LastValue <> Some('0') then exit 1

f.[*] <- 'z'
if f.LastLB <> None then exit 1
if f.LastUB <> None then exit 1
if f.LastValue <> Some('z') then exit 1

exit 0