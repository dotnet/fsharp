// #Regression #Diagnostics 
// Regression test for FSHARP1.0:2124
//<Expects status="notin">NONTERM</Expects>
//<Expects id="FS0010" status="error"></Expects>

#light "off"

let take n (ie : seq<'a>) = 
        if n < 0 then ()
        if n = 0 then Seq.empty else 
        { use e = ie.GetEnumerator() 
          for i in 0 .. n - 1 do
              yield e.Current };;
