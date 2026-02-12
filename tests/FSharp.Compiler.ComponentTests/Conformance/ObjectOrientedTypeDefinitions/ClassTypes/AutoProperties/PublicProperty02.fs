// #Conformance #ObjectOrientedTypes #Classes #MethodsAndProperties #Accessibility

module M2

open M
let x = T()

x.PublicProperty <- 1
if x.PublicProperty <> 1 then exit 1

T.StaticPublicProperty <- 1
if T.StaticPublicProperty <> 1 then exit 1