// #Conformance #ObjectOrientedTypes #Classes #MethodsAndProperties
// <Expects status="error" id="FS0810" span="(7,1-7,11)">Property 'Property' cannot be set</Expects>
type T() =
    member val Property = 0 with get

let x = T()
x.Property <- 1

exit 1