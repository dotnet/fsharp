module Module

match 1 with
| A(a = B(b = b, Some { C = 2 }); _) -> ()
