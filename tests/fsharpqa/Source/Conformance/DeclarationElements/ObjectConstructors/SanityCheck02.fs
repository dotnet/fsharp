// #Conformance #DeclarationElements #ObjectConstructors 
#light

type King =
    class
        val mutable m_value : int
        member this.Value with get ()   = this.m_value 
                          and  set x  = this.m_value <- x
        new() = {m_value = 0}
    end
    

let test1 = new King()
if test1.Value <> 0 then exit 1
test1.Value <- 2
if test1.Value <> 2 then exit 1

let test2 = new King()
if test2.Value <> 0 then exit 1
test2.Value <- 42
if test2.Value <> 42 then exit 1
    
exit 0
