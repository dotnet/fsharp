// #Conformance #ObjectOrientedTypes #TypeExtensions 
#light

// Verify type extensions on non-fully qualified names

// Fully qualified
type System.String with
    member a.Extension1() = 1

// Non fully qualified
open System
type String with
    member a.Extension2() = 2


if "".Extension1() <> 1 then exit 1
if "".Extension2() <> 2 then exit 1

exit 0
