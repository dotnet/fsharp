// #Regression #Conformance #TypesAndModules #Unions 
// Regression test for FSHARP1.0:5223
// Overloading of Equals()
// Overloads differ for the return type only (from the one implemented by default)
//<Expects status="error" span="(7,23-7,29)" id="FS0438">Duplicate method\. The method 'Equals' has the same name and signature as another method in type 'DU'\.$</Expects>
type DU = | A
          member this.Equals(s:DU) = 1.
