module Module

match { A = 1 } with
| { Foo.Bar.A = 1; B = 2; C = 3 } -> ()
