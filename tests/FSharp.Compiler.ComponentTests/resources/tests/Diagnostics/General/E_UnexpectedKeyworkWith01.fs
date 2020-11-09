// #Regression #Diagnostics 
// Regression test for FSHARP1.0:1872
//<Expects id="FS0010" status="error">'with'</Expects>

#light

let x = 3 with			// (6,11): error FS0010: unexpected keyword 'with' in implementation file
