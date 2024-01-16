module rec Test


[<RequireQualifiedAccess>]
type DuA = 
    | A1
    | B of DuB
    | C of DuC

    static do printfn "DuA init"
    static let allVals = 
        printfn "DuA allVals access"
        seq {yield DuA.A1.ToString();yield! DuB.AllValues; yield! DuC.AllValues}
    static member AllValues = allVals

[<RequireQualifiedAccess>]
type DuB =
    | B1
    | A of DuA
    | C of DuC

    static do printfn "DuB init"
    static let allVals = 
        printfn "DuB allVals access"
        seq {yield DuB.B1.ToString();yield! DuA.AllValues; yield! DuC.AllValues}
    static member AllValues = allVals

[<RequireQualifiedAccess>]
type DuC =
    | C1
    | A of DuA
    | B of DuB

    static do printfn "DuC init"
    static let allVals = 
        printfn "DuC allVals access"
        seq {yield DuC.C1.ToString();yield! DuA.AllValues; yield! DuB.AllValues}
    static member AllValues = allVals


let all = Seq.concat [ DuA.AllValues; DuB.AllValues; DuC.AllValues] |> Seq.truncate 999 |> List.ofSeq

printfn "total = %i" (all |> List.length)
printfn "uniq = %i" (all |> List.distinct |> List.length)