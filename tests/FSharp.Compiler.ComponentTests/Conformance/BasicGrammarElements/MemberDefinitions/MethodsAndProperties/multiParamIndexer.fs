// #Conformance #DeclarationElements #MemberDefinitions #MethodsAndProperties 
#light

open System.Text

type TypeWithIndexer() = 

    let m_val = new StringBuilder("one\ntwo\nthree\nfour")

    member x.Item with get (x', y) = let lines = m_val.ToString().Split([|"\n"|], System.StringSplitOptions.RemoveEmptyEntries)
                                     lines.[x'].[y]
                  and  set (x') (newChar) = m_val.Chars x' <- newChar
                  
    member x.Value = m_val.ToString()


let foo = new TypeWithIndexer()

// Use getter
if foo.Item(1, 1) <> "w".ToCharArray().[0] then failwith "Failed: 1"

// Use setter
foo.Item 0 <- ("!".ToCharArray().[0]) 
if foo.Value <> "!ne\ntwo\nthree\nfour" then failwith "Failed: 2"

// Use getter using getter syntax
if foo.[3,0] <> "f".ToCharArray().[0] then failwith "Failed: 3"

// Use setter
foo.[1] <- ("!".ToCharArray().[0]) 
if foo.Value <> "!!e\ntwo\nthree\nfour" then failwith "Failed: 4"

