namespace Test
open System.IO

type Foo = 
    static member Bar() =
        use stream = File.OpenRead("foo")

        use readerGood = new StreamReader(stream)
        use readerBad1 = StreamReader(stream)
        use readerBad2 = stream |> StreamReader 
        ()        
  
//<Expects status="warning" span="(9,26-9,46)" id="FS0760">It is recommended that objects supporting the IDisposable interface are created using the syntax 'new Type\(args\)', rather than 'Type\(args\)' or 'Type' as a function value representing the constructor, to indicate that resources may be owned by the generated value</Expects>
//<Expects status="warning" span="(10,36-10,48)" id="FS0760">It is recommended that objects supporting the IDisposable interface are created using the syntax 'new Type\(args\)', rather than 'Type\(args\)' or 'Type' as a function value representing the constructor, to indicate that resources may be owned by the generated value</Expects>
