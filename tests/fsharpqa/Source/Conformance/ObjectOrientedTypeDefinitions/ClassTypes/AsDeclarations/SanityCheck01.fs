// #Conformance #ObjectOrientedTypes #Classes 
#light

// Verify the use of 'as' in classes with implicit constructors to 
// access the 'this' pointer

type Point(x : float, y : float) as otherThisPtr =

    member this.X = x
    member this.Y = y

    member this.Length1 = sqrt (this.X * this.X + this.Y * this.Y)
    member this.Length2 = sqrt (otherThisPtr.X * otherThisPtr.X + otherThisPtr.Y * otherThisPtr.Y)

let pt = new Point(1.0, 0.0)

if pt.Length1 <> 1.0 then exit 1
if pt.Length2 <> 1.0 then exit 1

exit 0
