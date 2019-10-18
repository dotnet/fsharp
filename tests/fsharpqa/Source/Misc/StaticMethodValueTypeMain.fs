// #Regression #Misc 
#light

// Verify ability to call a static method on a generic type
// defined in another assembly. (FSB 1.0, 1529)

let _ = StructType<int>.StaticMethod()
let _ = StructType<unit>.StaticMethod()

exit 0
