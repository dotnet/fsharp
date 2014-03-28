// #Conformance #Quotations 
#light

let quote = 
            <@
              let facts = function
                | (1,"john","marge")
                | (1,"ted","sue")
                | (2,"john","ted")
                | (2,"ted","mike") -> true
                | _                -> false
              facts
            @>

exit 0
