// #Regression #NoMT #EntryPoint 
// Empty program entry point warning
// Lack of entry point produces warning with correct source location when compiled to *.exe
// when multiple namespaces with multiple modules declared in source file 


#light
namespace MyNamespace1

module MyModule1 =
    let irrelevant = 10
    
module MyModule2 =
    let irrelevant = 10
    
namespace MyNamespace2

module MyModule3 =
    let irrelevant = 10
    
module MyModule4 =
    let irrelevant = 10