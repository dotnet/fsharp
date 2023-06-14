// #Regression #Conformance #TypesAndModules #Unions 
// Regression test for FSHARP1.0:5223
// Overloading of GetHashCode()
// Overloads differ for the return type only (from the one implemented by default)
//<Expects status="error" span="(8,25-8,36)" id="FS0438">Duplicate method\. The method 'GetHashCode' has the same name and signature as another method in type 'DU'\.$</Expects>

type DU = | A
            member this.GetHashCode() = 1.
            
