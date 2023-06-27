// #Regression #Conformance #ObjectOrientedTypes #Classes 

#light

// Verify error when using 'as' to get a thisPointer when you don't have an
// implicit constructor.

//<Expects id="FS0963" status="error">This definition may only be used in a type with a primary constructor\. Consider adding arguments to your type definition, e\.g\. 'type X\(args\) = \.\.\.'\.</Expects>

type Point as otherThisPtr =

    val m_x : float
    val m_y : float

    new (x : float, y : float) = { m_x = x; m_y = y }


    member this.X = m_x
    member this.Y = m_y

    member this.Length1 = sqrt (this.X * this.X + this.Y * this.Y)
    member this.Length2 = sqrt (otherThisPtr.X * otherThisPtr.X + otherThisPtr.Y * otherThisPtr.Y)

let pt = new Point(1.0, 0.0)

if pt.Length1 <> 1.0 then exit 1
if pt.Length2 <> 1.0 then exit 1

exit 1

