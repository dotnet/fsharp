// #Conformance #PatternMatching #TypeTests 
#light

// Perform multiple dynamic type tests at once

let printCode x y =
    match box x, box y with
    | (:? int as ix), (:? int as iy) 
        -> sprintf "%d-%d" ix iy
    | (:? string as sx), (:? string as sy) 
        -> sprintf "%s-%s" sx sy
    | (:? int as ix), (:? string as sy) 
        -> sprintf "%d-%s" ix sy
    | (:? string as sx), (:? int as iy) 
        -> sprintf "%s-%d" sx iy
    | _ -> ""

let test1 = printCode 1 2
if test1 <> "1-2" then exit 1

let test2 = printCode "foo" "baz"
if test2 <> "foo-baz" then exit 1

let test3 = printCode 4 "aire"
if test3 <> "4-aire" then exit 1

let test4 = printCode "Z" 26
if test4 <> "Z-26" then exit 1

let test5 = printCode 'a' 1
if test5 <> "" then exit 1

exit 0
