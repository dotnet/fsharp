// #Regression #NoMT #EntryPoint 
// Empty program entry point warning
// Lack of entry point produces warning with correct source location when compiled to *.exe
// when source file declares type inside namespace


#light
namespace MyNamespace1

type T = T of int