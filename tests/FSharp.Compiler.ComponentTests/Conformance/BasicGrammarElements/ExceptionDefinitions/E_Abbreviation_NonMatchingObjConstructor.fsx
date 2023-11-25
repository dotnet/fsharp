// #Regression #Conformance #TypesAndModules #Exceptions 
// Exception type - incorrect abbreviation

//<Expects id="FS0920" span="(8,1-8,28)" status="error">Abbreviations for Common IL exception types must have a matching object constructor</Expects>


// F# exception definition + abbreviation
exception F = System.DBNull
