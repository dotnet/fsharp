// #Regression #Import
// Regression test for DevDiv:33846 ("F# project references not working when targeting x64 platform in Visual Studio 2010")
// The idea is that compiling this file for different platforms and referencing the C# library
// also compiled for different platforms should not give any errors!
//<Expects status="success"></Expects>

module Module1

open MyLib 
let answer = Class1.GetAnswer()
