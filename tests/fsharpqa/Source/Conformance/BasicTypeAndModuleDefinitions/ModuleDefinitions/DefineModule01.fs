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



    if x <> 1 then exit 1
    if MyNestedModule.f x <> 2 then exit 1

    if MyOtherNestedModule.MyOtherNestedModuleTwo.x <> 10 then exit 1
    if MyNestedModule.f MyOtherNestedModule.MyOtherNestedModuleTwo.x <> 11 then exit 1

    exit 0
