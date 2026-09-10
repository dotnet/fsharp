let orig1 () = {| Nested = {| A = "value1"; B = "value1" |}; Other = {| A = "value2"; B = "value2" |} |}
let orig2 () = {| Nested = {| A = "value3"; B = "value3" |} |}

let actual = {| ...orig1 (); Nested.B = "value4"; ...orig2 (); Other.B = "value5" |}