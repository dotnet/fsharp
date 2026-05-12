// #Conformance #DeclarationElements #MemberDefinitions #MethodsAndProperties 


type MusicFormat = CD | Record | MP3

type MusicPlayer() =
    member this.PlayMusic x = printfn "Playing %s" x

type CDPlayer() =
    inherit MusicPlayer() with
        
    static member GetFormat() = CD

if CDPlayer.GetFormat() <> CD then failwith "Failed: 1"
