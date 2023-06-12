module OrderMatters

let f<'a, 'b> (x: 'b) (y: 'a) = ()

type T() =
    member this.i<'a, 'b> (x: 'b) (y: 'a) = printfn "%A %A" x y
