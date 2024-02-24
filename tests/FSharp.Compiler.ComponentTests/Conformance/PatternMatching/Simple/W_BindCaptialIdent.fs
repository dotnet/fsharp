// #Regression #Conformance #PatternMatching 
// Verify warning when capturing values with captial identifier
// FSB 3954

//<Expects id="FS0049" span="(9,16-9,19)" status="warning">Uppercase variable identifiers should not generally be used in patterns, and may indicate a missing open declaration or a misspelt pattern name</Expects>
//<Expects id="FS0049" span="(10,16-10,19)" status="warning">Uppercase variable identifiers should not generally be used in patterns, and may indicate a missing open declaration or a misspelt pattern name</Expects>

let test x = function 
             | Foo :: []      -> 1 
             | Bar :: _ :: [] -> 2 
             | _ -> 3
