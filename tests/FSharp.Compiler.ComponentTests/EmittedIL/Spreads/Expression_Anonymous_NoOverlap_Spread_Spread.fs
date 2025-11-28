let r1 = {| A = 1 ; B = 2 |}
let r2 = {| C = 3; D = 4 |}

let r3 : {| A : int ; B : int; C : int; D : int |} = {| ...r1; ...r2 |}
let r4 : {| A : int ; B : int; C : int; D : int |} = {| ...r2; ...r3 |}
let r3' : {| A : int ; B : int; C : int; D : int |} = {| {||} with ...r1; ...r2 |}
let r4' : {| A : int ; B : int; C : int; D : int |} = {| {||} with ...r2; ...r3 |}
