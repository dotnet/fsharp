// #Regression #Conformance #LexFilter 
// Regression for FSHARP1.0:5933
// fix #light syntax for cast operators

module M

let f (info: System.Reflection.MethodInfo) = 
  System.Attribute.GetCustomAttribute(info, typeof<ReflectedDefinitionAttribute>)
  :?> ReflectedDefinitionAttribute

let f2 x = 
  new ReflectedDefinitionAttribute()
  :> System.Attribute

exit 0
