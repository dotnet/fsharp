// #Conformance #DeclarationElements #MemberDefinitions #MethodsAndProperties 
#light

// Testing instance methods on regular classes
type VanillaClass(x) =
    let mutable m_value = x
    member this.InstanceProperty with get() = m_value and set x = m_value <- x
    
let vt1 = new VanillaClass(42)
vt1.InstanceProperty <- vt1.InstanceProperty * -1
let vt2 = new VanillaClass(-42)

if vt1.InstanceProperty <> vt2.InstanceProperty then failwith "Failed: 1"
