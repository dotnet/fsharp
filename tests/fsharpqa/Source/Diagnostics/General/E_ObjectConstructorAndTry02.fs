// #Regression #Diagnostics 
// Regression test for FSHARP1.0:1980
//<Expects id="FS0420" span="(9,13-9,16)" status="error">Object constructors cannot directly use try/with and try/finally prior to the initialization of the object\. This includes constructs such as 'for x in \.\.\.' that may elaborate to uses of these constructs\. This is a limitation imposed by Common IL\.$</Expects>
#light

type X = struct
          val f : decimal
          new (arg) = 
            for i in [1;2] do ()
            X(1)
         end     
