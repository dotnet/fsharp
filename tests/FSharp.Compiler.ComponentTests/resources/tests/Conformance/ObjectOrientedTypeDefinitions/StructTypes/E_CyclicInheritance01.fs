// #Regression #Conformance #ObjectOrientedTypes #Structs 
#light
// Regression test for FSHARP1.0:1452, Complete checks for structs
// Also, 
//<Expects id="FS0954" span="(11,10-11,20)" status="error">This type definition involves an immediate cyclic reference through a struct field or inheritance relation</Expects>
//<Expects id="FS0954" span="(15,9-15,19)" status="error">This type definition involves an immediate cyclic reference through a struct field or inheritance relation</Expects>
//<Expects id="FS0954" span="(26,10-26,19)" status="error">This type definition involves an immediate cyclic reference through a struct field or inheritance relation</Expects>
//<Expects id="FS0953" span="(31,10-31,25)" status="error">This type definition involves an immediate cyclic reference through an abbreviation</Expects>

module BadStructTest2 =  begin
    type BadStruct1 = 
        struct
            val x : BadStruct2
        end    
    and BadStruct2 =
        struct
            val x : BadStruct1
        end    
end

module BadStructTest3 =  begin
    type One<'a> = 
        struct
            val x : 'a
        end    
    type BadStruct =
        struct
            val x : One<BadStruct>
        end    

    type BadAbbreviation = One<BadAbbreviation>
end

// Shouldn't compile
exit 1

