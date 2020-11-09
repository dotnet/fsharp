// #Conformance #TypeConstraints 
// Test the ability to specify an anon, generic type

let Name<'a> = typeof<'a>.Name

if Name< Map<_,_> > <> "FSharpMap`2"  then exit 1
if Name< string >   <> "String" then exit 1

exit 0
