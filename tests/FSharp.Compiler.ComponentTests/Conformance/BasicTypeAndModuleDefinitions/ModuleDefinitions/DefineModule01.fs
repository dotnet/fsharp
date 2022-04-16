// #Conformance #TypesAndModules #Modules 
#light

// Define a simple module and access its values

module MyModule = 

    let x = 1

    type Foo = A | B

    module MyNestedModule = 
        let f y = y + 1

        type Bar = C | D

    module MyOtherNestedModule =
        module MyOtherNestedModuleTwo =
            let x = 10



    if x <> 1 then failwith "Failed: 1"
    if MyNestedModule.f x <> 2 then failwith "Failed: 2"

    if MyOtherNestedModule.MyOtherNestedModuleTwo.x <> 10 then failwith "Failed: 3"
    if MyNestedModule.f MyOtherNestedModule.MyOtherNestedModuleTwo.x <> 11 then failwith "Failed: 4"
