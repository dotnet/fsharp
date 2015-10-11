// #Regression #NoMT #EntryPoint 
// Empty program entry point warning
// Lack of entry point produces warning with correct source location when compiled to *.exe
// when source file declares type inside namespace
//<Expects id="FS0988" span="(1,81-1,81)" status="warning">Main module of program is empty: nothing will happen when it is run</Expects>

#light
namespace MyNamespace1

type T = T of int