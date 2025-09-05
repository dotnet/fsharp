module Module

match 1 with
| A(a = { X = 1 } as res, { X = 1 }, b = { X = 1 }, { Y = 3 } as res2) -> ()
