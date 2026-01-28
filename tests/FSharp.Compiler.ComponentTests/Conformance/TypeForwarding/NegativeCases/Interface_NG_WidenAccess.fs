let f = new N_003.Foo()
let b = new N_003.Bar()
let i = f :> N_003.IFoo
let rv = f.getValue() + b.getValue() + i.getValue()

System.Console.WriteLine(rv)
exit rv

