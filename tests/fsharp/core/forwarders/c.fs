// See notes in a.cs



module UseForwardedTypes 

let CreateC() = new C()
let CreateD() = new D()
let CreateGenericD() = new GenericD<C>()
let CreateE() = new D.E()
let CreateEE() = new D.E.EE()

let CreateF() = new F()
let CreateG() = new G()
let CreateH() = new G.H()

let ConsumeC(x) = F.ConsumeC(x)
let ConsumeD(x) = F.ConsumeD(x)
let ConsumeGenericD(x) = F.ConsumeGenericD(x)
let ConsumeE(x) = F.ConsumeE(x)
let ConsumeEE(x) = F.ConsumeEE(x)

