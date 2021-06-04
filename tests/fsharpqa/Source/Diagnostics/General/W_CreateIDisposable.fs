namespace Test
open System.IO

type Foo = 
    static member Bar() =
        use stream = File.OpenRead("foo")

        use readerGood = new StreamReader(stream)
        use readerBad1 = StreamReader(stream)
        use readerBad2 = stream |> StreamReader 
        ()        
