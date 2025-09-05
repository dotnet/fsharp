module Module

match 1 with
| A(a = { X = 1 }, { X = 1 }; b = { X = 1 }) -> ()
