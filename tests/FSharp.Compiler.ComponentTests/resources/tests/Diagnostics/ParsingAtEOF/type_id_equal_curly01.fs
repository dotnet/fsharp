// #Regression #Diagnostics 
//<Expects status="error" span="(5,10-5,11)" id="FS0604">Unmatched '{'$</Expects>
//<Expects status="error" span="(6,1-6,1)" id="FS0010">Incomplete structured construct at or before this point in type definition\. Expected '}' or other token\.$</Expects>

type C = { a : int
