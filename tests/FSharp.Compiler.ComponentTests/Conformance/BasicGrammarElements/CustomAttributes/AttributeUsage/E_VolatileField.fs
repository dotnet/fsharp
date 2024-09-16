module VolatileFieldSanityChecks = begin

  [<VolatileField>]
  let mutable x = 1

  [<VolatileField>]
  let rec f x = 1

  [<VolatileField>]
  let x2 = 1

  type C() = 
    [<VolatileField>]
    static let sx2 = 1   // expect an error - not mutable

    [<VolatileField>]
    static let f x2 = 1   // expect an error - not mutable

    [<VolatileField>]
    let x2 = 1   // expect an error - not mutable

    [<VolatileField>]
    let f x2 = 1   // expect an error - not mutable

    [<VolatileField>]
    val mutable x : int // expect an error - not supported

    member x.P = 1

end
           