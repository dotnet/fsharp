// #Regression #NoMT #Printing 
#light

// Test for FSharp1.0:4086 - accessiblity not printed for exceptions
//<Expects status="success">exception internal A of int</Expects>
//<Expects status="success">exception private B of string</Expects>
//<Expects status="success">exception C of System\.DateTime</Expects>

exception internal A of int
exception private  B of string
exception public   C of System.DateTime

;;

#q;;
