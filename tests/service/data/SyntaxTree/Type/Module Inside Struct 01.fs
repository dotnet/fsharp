// Testing: Module inside struct
module Module

type MyStruct =
    struct
        module InvalidModule = 
            let helper = 10
    end
