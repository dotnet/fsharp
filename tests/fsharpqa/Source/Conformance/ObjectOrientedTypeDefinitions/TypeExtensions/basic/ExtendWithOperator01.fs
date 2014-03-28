// #Conformance #ObjectOrientedTypes #TypeExtensions 
#light

// Verify ability to add a type extension for an operator
// Note that C# and VB do NOT support this.

type System.Int32 with 
    member x.GetSlice(idx1,idx2) = 
        let idx1 = defaultArg idx1 0
        let idx2 = defaultArg idx2 10
        idx2 - idx1

if (0).[3..4] <> 1 then exit 1
if (0).[3..]  <> 7 then exit 1
if (0).[..5]  <> 5 then exit 1

exit 0
