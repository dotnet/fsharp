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

\1.fsx
\2.fsx
\3.fsx]
Hello
World
-the end

namespace FSI_0002


namespace FSI_0002


namespace FSI_0002

> 
Test 4=================================================
Test 5=================================================

usesfsi.fsx(2,1): error FS0039: The namespace or module 'fsi' is not defined
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


flagcheck.fs(2,1): error FS0222: Files in libraries or multiple-file applications must begin with a namespace or module declaration, e.g. 'namespace SomeNamespace.SubNamespace' or 'module SomeNamespace.SomeModule'. Only the last source file of an application may omit such a declaration.
Test 14=================================================
INTERACTIVE is defined
Test 15=================================================
Result = 99
Type from referenced assembly = System.Web.Mobile.CookielessData
Test 16=================================================
Result = 99
Type from referenced assembly = System.Web.Mobile.CookielessData
Done ==================================================
