let src = {| A = 1; B = "B"; C = 3m |}

let typedTarget : {| B : string |} = {| ...src |}
let typedTarget' : {| B : string |} = {| {||} with ...src |}
