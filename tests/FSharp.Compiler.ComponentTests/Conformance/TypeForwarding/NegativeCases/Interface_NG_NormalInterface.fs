let nc = new NormalInterface()
let i = nc :> INormal
let rv = nc.getValue() + i.getValue()

System.Console.WriteLine(rv)
exit rv
