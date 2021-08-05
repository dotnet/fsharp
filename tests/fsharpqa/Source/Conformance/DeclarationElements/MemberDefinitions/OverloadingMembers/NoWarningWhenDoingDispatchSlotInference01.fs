// #Regression #Conformance #DeclarationElements #MemberDefinitions #Overloading 
// Regression test for FSHARP1.0:1894
// This code should compile without warnings!
#light

type MyTraceListener = class
                        inherit System.Diagnostics.TraceListener
                        override x.Write (_message:string) = ()
                        override x.WriteLine (_message:string) = ()
                        override x.Fail (_message:string) = exit 0
                       end

exit 0
