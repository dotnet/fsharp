// #DeclarationElements #LetBindings 
//<Expects status="success"></Expects>
open System

let answer =
    use x:IDisposable = new System.IO.MemoryStream()
    42
