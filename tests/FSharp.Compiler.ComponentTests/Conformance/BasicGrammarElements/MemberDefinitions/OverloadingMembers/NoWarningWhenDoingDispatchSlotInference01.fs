// #Regression #Conformance #DeclarationElements #MemberDefinitions #Overloading 
// Regression test for FSHARP1.0:1894
// This code should compile without warnings!
#light

type MyTraceListener = class
                        inherit System.Diagnostics.TraceListener
                        override x.Write (message:string) = ()
                        override x.WriteLine (message:string) = ()
                        override x.Fail (message:string) = exit 0
                       end
