// #Globalization 
#light

// Surrogates: in comments

(* 𦺘𣹌𠷿𨒉𥐽𢏱𩩻𦨮𣧢𠦖  *)
// 𦺘𣹌𠷿𨒉𥐽𢏱𩩻𦨮𣧢𠦖
/// 𦺘𣹌𠷿𨒉𥐽𢏱𩩻𦨮𣧢𠦖
type T = | T


(* 𝘗𝒷𝝖𝗶𝒖𝜵𝗕𝑵𝜔𝖴 *)
// 𝘗𝒷𝝖𝗶𝒖𝜵𝗕𝑵𝜔𝖴
/// 𝘗𝒷𝝖𝗶𝒖𝜵𝗕𝑵𝜔𝖴
type T1 = | T1


// Surrogates: in string literals

let string_literal = "𨅆抆𝑘f𤊀㧤𝛖C𠎻絀"
let string_literal2 = """𨅆抆𝑘f𤊀㧤𝛖C𠎻絀"""

(if string_literal.Length = 15 then 0 else 1) |> exit
(if string_literal2.Length = 15 then 0 else 1) |> exit

