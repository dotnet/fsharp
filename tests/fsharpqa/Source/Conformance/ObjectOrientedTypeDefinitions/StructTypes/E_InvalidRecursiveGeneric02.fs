// #Regression #Conformance #ObjectOrientedTypes #Structs 
// Verify proper error for illegal recursive / generic structs
// Regression for FSB 3417

//<Expects id="FS0954" status="error">This type definition involves an immediate cyclic reference through a struct field or inheritance relation</Expects>
//<Expects id="FS0039" status="error">The value or constructor 'BadType4' is not defined</Expects>

type BadType4 =
    struct
        [<DefaultValueAttribute>]
        val mutable X : Naught4<BadBox4<BadType4>>
    end
and BadBox4<'T> = 
    struct
        val v : 'T
    end
and Naught4<'a> = 'a
     
let _ = BadType4()

exit 1
