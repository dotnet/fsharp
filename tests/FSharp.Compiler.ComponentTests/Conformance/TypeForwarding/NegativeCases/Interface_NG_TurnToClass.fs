let f = new TurnsToClass()
let i = f :> TurnsToClass
let rv = f.getValue() - i.getValue()

System.Console.WriteLine(rv)
exit rv
