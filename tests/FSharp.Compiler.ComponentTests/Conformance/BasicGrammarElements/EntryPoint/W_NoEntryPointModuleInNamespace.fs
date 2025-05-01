// #Regression #NoMT #EntryPoint 
// Empty program entry point warning
// Lack of entry point produces warning with correct source location when compiled to *.exe
// when single module declared inside namespace


#light
namespace MyNamespace1

module MyModule1 =
    let irrelevant = 10