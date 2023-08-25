// #Conformance #PatternMatching #TypeTests 
#light

let mutable testPassed = false
try
    raise (System.Exception(""))
with
    | :? _ -> testPassed <- true

if testPassed <> true then exit 1

exit 0
