module Module

match 1 with
| A(a = { X = 1 } as res; b = { Y = 3 } as res2) -> ()
