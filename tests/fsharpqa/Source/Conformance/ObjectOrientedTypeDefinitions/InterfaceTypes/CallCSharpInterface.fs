// #Conformance #ObjectOrientedTypes #InterfacesAndImplementations #ReqNOMT 
// Verify consumption of C# interfaces from F#

{new I_003<int> with member x.Home(i) = i }.Home({new I_002<int> with member x.Me() = 0}.Me()) |> exit
