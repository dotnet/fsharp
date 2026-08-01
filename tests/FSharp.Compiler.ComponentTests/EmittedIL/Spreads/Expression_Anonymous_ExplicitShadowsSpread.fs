let r1 = {| A = 1; B = 2 |}

let r2 : {| A : string; B : int |} = {| ...r1; A = "A" |}
