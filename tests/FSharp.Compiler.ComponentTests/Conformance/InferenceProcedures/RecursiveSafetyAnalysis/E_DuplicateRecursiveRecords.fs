// #Regression #Conformance #TypeInference #Recursion 
// Regression for FSHARP1.0:5601
// ICE when compiling code with duplicate record type defined recursively


type Foo =
    { Name:  string;
      Value: int }

and Foo =
    { Elephant: char }
