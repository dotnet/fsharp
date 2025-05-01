// #Regression #Conformance #DeclarationElements #MemberDefinitions #MethodsAndProperties 
// Regression for DEV10:841423
// Previously it was an error to have a static and instance member share the same name

module M

let s1 = "member method"
let s2 = "static method"

type T() = 
  member v.a() = "member method"
  static member a() = "static method"

let t = T()

if s1 = t.a() && s2 = T.a() then
    ()
else
    failwith "Failed: 1"
