// #Regression #Conformance #ObjectOrientedTypes #MethodsAndProperties #MemberDefinitions 
// Dev11:175889, used to not be an error to define a class like this
//<Expects status="error" id="FS3151" span="(11,19-11,45)">This member, function or value declaration may not be declared 'inline'</Expects>
//<Expects status="error" id="FS3151" span="(16,20-16,34)">This member, function or value declaration may not be declared 'inline'</Expects>

type Ellipse(a0 : float, b0 : float, theta0 : float) =
   let mutable axis1 = a0
   let mutable axis2 = b0
   let mutable rotAngle = theta0
   abstract member Rotate: float -> unit
   default inline this.Rotate(delta : float) = rotAngle <- rotAngle + delta
 
type Circle(radius : float) =
   inherit Ellipse(radius, radius, 0.0)
   // Circles are invariant to rotation, so do nothing.
   override inline this.Rotate(_) = ()

exit 0