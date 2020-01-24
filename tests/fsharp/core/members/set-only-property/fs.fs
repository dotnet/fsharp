type Class () =
  let mutable v = 0
  member x.Prop with set(value) = v <- value

let a = csharp.Class(Prop=1)
let b = basic.BasicClass(Prop=1)
let c = Class(Prop=1)

type Maker =
  static member mkCs () = csharp.Class()
  static member mkVb () = basic.BasicClass()
  static member mkFs () = Class()

let aa = Maker.mkCs(Prop=1)
let bb = Maker.mkVb(Prop=1)
let cc = Maker.mkFs(Prop=1)