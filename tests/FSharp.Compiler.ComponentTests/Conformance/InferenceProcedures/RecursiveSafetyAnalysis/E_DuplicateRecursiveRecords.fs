// #Regression #Conformance #TypeInference #Recursion 
// Regression for FSHARP1.0:5601
// ICE when compiling code with duplicate record type defined recursively
//<Expects id="FS0037" span="(10,5-10,8)" status="error">Duplicate definition of type, exception or module 'Foo'</Expects>

type Foo =
    { Name:  string;
      Value: int }

and Foo =
    { Elephant: char }
