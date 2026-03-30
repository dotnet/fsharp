module RecordTest.Program

open RecordTest.Types
open RecordTest.Usage

[<EntryPoint>]
let main _argv =
    let p = makePerson ()
    let c = makeCompany ()
    let pet = makePet ()
    let p2 = birthday p
    printfn "%s is %d" p.Name p.Age
    printfn "%s founded %d" c.Name c.Founded
    printfn "%s is a %s" pet.Name pet.Species
    printfn "%s is now %d" p2.Name p2.Age
    0
