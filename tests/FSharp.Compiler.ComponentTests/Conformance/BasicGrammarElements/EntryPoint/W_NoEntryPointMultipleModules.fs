// #Regression #NoMT #EntryPoint 
// Empty program entry point warning
// Lack of entry point produces warning with correct source location when compiled to *.exe
// when multiple modules declared without declaring namespace


#light

module MyModule1 =
    let irrelevant = 10

module MyModule2 =
    let irrelevant = 10