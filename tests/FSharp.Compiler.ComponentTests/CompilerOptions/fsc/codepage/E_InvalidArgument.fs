// #Regression #NoMT #CompilerOptions 
// --codepage 65536
// Actual message is: error FS0191: Valid values are between 0 and 65535, inclusive.
//                    Parameter Name: codepage
// but is is localized!
//<Expects id="FS1000" status="error">Problem with codepage '65536': Valid values are between 0 and 65535, inclusive</Expects>


exit 1
