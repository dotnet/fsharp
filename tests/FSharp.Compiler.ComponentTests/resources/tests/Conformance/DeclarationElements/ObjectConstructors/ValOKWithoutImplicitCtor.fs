// #Conformance #DeclarationElements #ObjectConstructors 
#light

// Verify val fields OK without default value if no implicit ctor is provided

type King =
    class
        val mutable m_value : int
        member this.Value with get () = this.m_value 
                          and  set x  = this.m_value <- x
    end

// We can't construct type King, but it compiles OK, which is is all we care about   
    
exit 0
