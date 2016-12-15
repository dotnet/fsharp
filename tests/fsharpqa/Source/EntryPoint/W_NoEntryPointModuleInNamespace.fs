// #Regression #NoMT #EntryPoint 
// Empty program entry point warning
// Lack of entry point produces warning with correct source location when compiled to *.exe
// when single module declared inside namespace
//<Expects id="FS0988" span="(11,24-11,24)" status="warning">Main module of program is empty: nothing will happen when it is run</Expects>

#light
namespace MyNamespace1

module MyModule1 =
    let irrelevant = 10