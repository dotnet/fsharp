// #Regression #Conformance #TypeInference 
// Regression tests for FSHARP1.0:1348
//<Expects id="FS0023" span="(7,14-7,18)" status="error">The member 'Name' cannot be defined because the name 'Name' clashes with the field 'Name' in this type or module</Expects>
#light
type Repro = 
    { Name:decimal }
    member r.Name : decimal = r.Name

