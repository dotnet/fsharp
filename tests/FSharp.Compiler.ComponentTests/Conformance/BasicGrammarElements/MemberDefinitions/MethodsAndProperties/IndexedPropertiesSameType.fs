// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

type MyIndexerClass() =
    member x.Indexer1
        with get (index: int): string = ""
        and set (index: int) (value: float) = ()
    
    member x.Indexer2
        with get (index) = 1
        and set (index) (value: float) = ()

    member x.Indexer3
        with get index = 1
    member x.Indexer3
        with set index (value: float) = ()

    member x.Indexer4
        with get (index: int, index2: int): float = 0.0
        and set (index1: int, index2: int) (value: string) = ()

    member x.Indexer5
        with get (index, index2) = 0.0
        and set (index1, index2) value = ()

type GenericIndexer<'indexerArgs,'indexerOutput,'indexerInput>() =
    let mutable m_lastArgs  = Unchecked.defaultof<'indexerArgs>
    let mutable m_lastInput = Unchecked.defaultof<'indexerInput>

    member this.LastArgs  = m_lastArgs
    member this.LastInput = m_lastInput

    member this.Item with get (args : 'indexerArgs) = 
                                                m_lastArgs <- args;
                                                Unchecked.defaultof<'indexerOutput>
                     and  set (args : 'indexerArgs) (input : 'indexerInput) = 
                                                m_lastArgs  <- args
                                                m_lastInput <- input