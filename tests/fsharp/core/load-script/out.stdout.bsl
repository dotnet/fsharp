// #Conformance #FSI 
printfn "Hello"
// #Conformance #FSI 
#load "1.fsx"
printfn "World"
// #Conformance #FSI 
#load "2.fsx"
printfn "-the end"
Test 1=================================================
Hello
World
-the end
Test 2=================================================
Hello
World
-the end
Test 3=================================================

> [Loading D:\staging\staging\src\tests\fsharp\core\load-script\1.fsx
 Loading D:\staging\staging\src\tests\fsharp\core\load-script\2.fsx
 Loading D:\staging\staging\src\tests\fsharp\core\load-script\3.fsx]
Hello
World
-the end
module FSI_0002.1

module FSI_0002.2

module FSI_0002.3

> 
Test 4=================================================
Test 5=================================================
Test 6=================================================
Test 7=================================================
Hello
Test 8=================================================
Hello
World
-the end
Test 9=================================================
Hello
World
-the end
Test 10=================================================
Hello
World
-the end
Test 11=================================================
COMPILED is defined
Test 12=================================================
COMPILED is defined
Test 13=================================================
INTERACTIVE is defined
Test 14=================================================
INTERACTIVE is defined
Test 15=================================================
Result = 99
Type from referenced assembly = System.Web.Mobile.CookielessData
Test 16=================================================
Result = 99
Type from referenced assembly = System.Web.Mobile.CookielessData
Test 17=================================================
Done ==================================================
