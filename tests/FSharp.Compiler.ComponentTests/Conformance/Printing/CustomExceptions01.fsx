// #Regression #NoMT #Printing 
#light

// Test for FSharp1.0:4045 - exception printing adds Exception to name of exn constructor
// Test for FSharp1.0:4054 - exception printing of constant exceptions adds a unit value (no arguments)

type HtmlBody = { Body : string }
type WeekendDay = Saturday | Sunday

exception Foo
exception Bar of int
exception FooBaz of System.DateTime
exception MarkupEx of HtmlBody
exception WeekendEx of WeekendDay

if  sprintf "%A" (Foo) <> "Foo"
    || sprintf "%A" (Bar 10) <> "Bar 10"
    || sprintf "%A" (MarkupEx {Body = "<body>"}) <> "MarkupEx { Body = \"<body>\" }"
    || sprintf "%A" (WeekendEx Saturday) <> "WeekendEx Saturday"
then raise (new System.Exception("CustomExceptions01.fs failed"))
