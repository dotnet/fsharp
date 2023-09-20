// #Conformance #DeclarationElements #MemberDefinitions #MethodsAndProperties 
module M

type GenericIndexer<'indexerArgs,'indexerOutput,'indexerInput>() =
    let mutable m_lastArgs  = Unchecked.defaultof<'indexerArgs>
    let mutable m_lastInput = Unchecked.defaultof<'indexerOutput>

    member this.LastArgs  = m_lastArgs
    member this.LastInput = m_lastInput

    member this.Item with get (args : 'indexerArgs) = 
                                                m_lastArgs <- args;
                                                Unchecked.defaultof<'indexerOutput>
                     and  set (args : 'indexerArgs) (input : 'indexerOutput) = 
                                                m_lastArgs  <- args
                                                m_lastInput <- input

let t1 = new GenericIndexer<int * int, float, decimal * unit>()
if t1.[1, 2] <> 0.0 then failwith "Failed: 1"
if t1.LastArgs <> (1, 2) then failwith "Failed: 2"
t1.[(0, 0)] <- -1.0
if t1.LastInput <> -1.0 then failwith "Failed: 3"
