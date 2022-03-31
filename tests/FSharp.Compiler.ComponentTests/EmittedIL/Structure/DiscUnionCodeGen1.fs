// #NoMT #CodeGen #Interop 
// Verify the compiled structure of discriminated unions

namespace Test

    type Suit =
        | Diamonds
        | Hearts
        | Spades
        | Clubs
        
    type Card =
        | ValueCard of int * Suit
        | Jack  of Suit
        | Queen of Suit
        | King  of Suit
        | Ace   of Suit
        | Joker

// --------------------------------

    module Tester =

        open CodeGenHelper
        open System

        printfn "Testing..."

        try

            let asm = System.Reflection.Assembly.GetExecutingAssembly()

            let cardType =
                System.Reflection.Assembly.GetExecutingAssembly()
                |> getType "Test.Card"
            
            // Expected properties
            [ "Tag"; "IsJoker"; "IsKing"; "IsQueen"; "IsJack"; "IsValueCard" ]        
            |> List.iter (fun propName -> cardType |> should containProp propName)

            [ "Joker" ]
            |> List.iter (fun propName -> cardType |> should containProp propName)

            // Expected members
            [ "NewAce"; "NewKing"; "NewQueen"; "NewJack"; "NewValueCard" ]
            |> List.iter (fun membName -> cardType |> should containMember membName)

            // Nested classes
            let verifyProperties propNames ty =
                propNames   |> List.iter (fun propName -> ty |> should containProp propName)
                ()
                
            asm
            |> getType "Test.Card+Ace"
            |> verifyProperties [ "Item" ]
          
            asm
            |> getType "Test.Card+King"
            |> verifyProperties [ "Item" ]
          
            asm
            |> getType "Test.Card+Queen"
            |> verifyProperties [ "Item" ]
          
            asm
            |> getType "Test.Card+Jack"
            |> verifyProperties [ "Item" ]
            
            asm
            |> getType "Test.Card+ValueCard"
            |> verifyProperties [ "Item1"; "Item2" ]        
          
        with
        | e -> printfn "Unhandled Exception: %s" e.Message 
               raise (Exception($"Oops: {e}"))
