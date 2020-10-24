#r "cs.dll"
#r "vb.dll"
#r "fs.dll"
type Maker =
  static member mkCs () = csharp.Class()
  static member mkVb () = basic.BasicClass()
  static member mkFs () = fsharp.Class()
// so long https://github.com/dotnet/fsharp/issues/8351 isn't fixed, Prop1 setters are failing
let a = csharp.Class(Prop1=1)
let b = basic.BasicClass(Prop1=1)
let c = fsharp.Class(Prop1=1) // this one works, inconsistent but correct.

let aa = Maker.mkCs(Prop1=1)
let bb = Maker.mkVb(Prop1=1)
let cc = Maker.mkFs(Prop1=1) // this one works, inconsistent but correct.

// those are expected to fail, albeit with inconsistent error messages / marked ranges
let aaa = csharp.Class(Prop2=1)
let bbb = basic.BasicClass(Prop2=1)
let ccc = fsharp.Class(Prop2=1)

let aaaa = Maker.mkCs(Prop2=1)
let bbbb = Maker.mkVb(Prop2=1)
let cccc = Maker.mkFs(Prop2=1)
