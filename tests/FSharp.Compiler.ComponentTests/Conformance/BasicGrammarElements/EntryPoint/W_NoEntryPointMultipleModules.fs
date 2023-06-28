// #Regression #NoMT #EntryPoint 
// Empty program entry point warning
// Lack of entry point produces warning with correct source location when compiled to *.exe
// when multiple modules declared without declaring namespace
//<Expects id="FS0988" span="(13,24-13,24)" status="warning">Main module of program is empty: nothing will happen when it is run</Expects>

#light

module MyModule1 =
    let irrelevant = 10

module MyModule2 =
    let irrelevant = 10