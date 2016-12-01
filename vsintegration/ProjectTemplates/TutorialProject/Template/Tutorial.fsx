// This sample will guide you through elements of the F# language.
//
// *******************************************************************************************************
//   To execute the code in F# Interactive, highlight a section of code and press Alt-Enter or right-click 
//   and select "Execute in Interactive".  You can open the F# Interactive Window from the "View" menu. 
// *******************************************************************************************************
//
// For more about F#, see:
//     http://fsharp.org
//     https://docs.microsoft.com/en-us/dotnet/articles/fsharp/
//
// To learn more about applied F# programming, use
//     http://fsharp.org/guides/enterprise/
//     http://fsharp.org/guides/cloud/
//     http://fsharp.org/guides/web/
//     http://fsharp.org/guides/data-science/
//
// To install the Visual F# Power Tools, use
//     'Tools' --> 'Extensions and Updates' --> `Online` and search
//
// For additional templates to use with F#, see the 'Online Templates' in Visual Studio, 
//     'New Project' --> 'Online Templates'


// Open namespaces using 'open'
open System


/// First some basics on integers and numbers
module IntegersAndNumbers = 

    /// This is a sample integer.
    let sampleInteger = 176

    /// This is a sample floating point number.
    let sampleDouble = 4.1

    /// This computed a new number by some arithmetic.  Numeric types are converted using
    /// functions 'int', 'double' and so on.
    let sampleInteger2 = (sampleInteger/4 + 5 - 7) * 4 + int sampleDouble

    /// This is a list of the numbers from 0 to 99.
    let sampleNumbers = [ 0 .. 99 ]

    /// This is a list of all tuples containing all the numbers from 0 to 99 and their squares.
    let sampleTableOfSquares = [ for i in 0 .. 99 -> (i, i*i) ]

    // The next line prints a list that includes tuples, using %A for generic printing
    printfn "The table of squares from 0 to 99 is:\n%A" sampleTableOfSquares


/// Much of F# programming consists of defining functions that transform input data to produce
/// useful resutls.
module BasicFunctions = 

    /// You use 'let' to define a function that accepts an integer argument and returns an integer. 
    /// Parentheses are optional for function arguments,
    let sampleFunction1 x = x*x + 3

    /// Apply the function, naming the function return result using 'let'. 
    /// The variable type is inferred from the function return type.
    let result1 = sampleFunction1 4573
    printfn "The result of squaring the integer 4573 and adding 3 is %d" result1

    /// When needed, annotate the type of a parameter name using '(argument:type)'
    let sampleFunction2 (x:int) = 2*x*x - x/5 + 3

    let result2 = sampleFunction2 (7 + 4)
    printfn "The result of applying the 1st sample function to (7 + 4) is %d" result2

    /// Conditionals use if/then/elid/elif/else.
    ///
    /// Note that F# uses whitespace indentation-aware syntax like Python.
    let sampleFunction3 x = 
        if x < 100.0 then 
            2.0*x*x - x/5.0 + 3.0
        else 
            2.0*x*x + x/5.0 - 37.0

    let result3 = sampleFunction3 (6.5 + 4.5)
    printfn "The result of applying the 2nd sample function to (6.5 + 4.5) is %f" result3



/// Strings and booleans are fundamental data types in F# programming.
module StringManipulation = 

    /// Booleans use 'true', 'false', 'not', '&&' and '||'
    let boolean1 = true
    let boolean2 = false

    let boolean3 = not boolean1 && (boolean2 || false)

    printfn "The expression 'not boolean1 && (boolean2 || false)' is %A" boolean3

    /// Strings use double quotes.
    let string1 = "Hello"
    let string2  = "world"

    /// Strings can also use @ to create a verbatim string literal
    let string3 = @"c:\Program Files\"

    /// String literals can also use triple-quotes
    let string4 = """He said "hello world" after you did"""

    /// String concatenation is '+'. Also see String.concat, System.String.Join and others.
    let helloWorld = string1 + " " + string2 
    printfn "%s" helloWorld

    /// Substrings use the indexer notation, for example the first 7 characters.
    let substring = helloWorld.[0..6]
    printfn "%s" substring



/// Tuples are simple combinations of data values into a combined value.
module Tuples = 

    /// First define a simple tuple of integers.
    let tuple1 = (1, 2, 3)

    /// Next you define a function that swaps the order of two values in a tuple. 
    /// The function is inferred to have a generic type.
    let swapElems (a, b) = (b, a)

    printfn "The result of swapping (1, 2) is %A" (swapElems (1,2))

    /// A tuple consisting of an integer, a string, and a double-precision floating point number
    let tuple2 = (1, "fred", 3.1415)

    printfn "tuple1: %A    tuple2: %A" tuple1 tuple2

    /// A simple tuple of integers, represented as a flat inline struct (a value-type)
    let sampleStructTuple = struct (1, 2, 3)


// ---------------------------------------------------------------
//         Lists, Arrays and data processing
// ---------------------------------------------------------------

/// Lists, arrays and sequences are basic collection types in F# programming.
/// Many other collection types are available in System.Collections.Generic,
/// System.Collections.Immutable and other libraries.
module ListsAndArrays = 

    /// Lists are defined using [ ... ].  This is an empty list.
    let list1 = [ ]  

    /// This is a list with 3 elements.
    let list2 = [ 1; 2; 3 ]

    /// This is a list of integers from 1 to 1000
    let numberList = [ 1 .. 1000 ]  

    /// Lists can also be generated by computations. This is a list containing 
    /// all the days of the year.
    let daysList = 
        [ for month in 1 .. 12 do
              for day in 1 .. System.DateTime.DaysInMonth(2012, month) do 
                  yield System.DateTime(2012, month, day) ]

    /// Computations can include conditionals.  This is a list containing the tuples
    /// which are the coordinates of the black squares on a chess board.
    let blackSquares = 
        [ for i in 0 .. 7 do
              for j in 0 .. 7 do 
                  if (i+j) % 2 = 1 then 
                      yield (i, j) ]

    /// Lists can be transformed using 'List.map' and other functional programming combinators.
    /// This definition produces a new list by squaring the numbers in numberList, using the pipeline 
    /// operator to pass an argument to List.map.
    let squares = 
        numberList 
        |> List.map (fun x -> x*x) 

    /// There are many other list combinations. The following computes the sum of the squares of the 
    /// numbers divisible by 3.
    let sumOfSquares = 
        numberList
        |> List.filter (fun x -> x % 3 = 0)
        |> List.sumBy (fun x -> x * x)

    /// Arrays are like lists, but are stored 'flat' (rather than as immutable linked lists), and
    /// are mutable (their contents can be changed). This is The empty array.
    let array1 = [| |]

    /// Arrays are specified using the same range of constructs as lists.
    let array2 = [| "hello"; "world"; "and"; "hello"; "world"; "again" |]

    /// This is an array of numbers from 1 to 1000.
    let array3 = [| 1 .. 1000 |]

    /// This is an array containing only the words "hello" and "world"
    let array4 = 
        [| for word in array2 do
               if word.Contains("l") then 
                   yield word |]

    /// This is an array initialized by index and containing the even numbers from 0 to 2000
    let evenNumbers = Array.init 1001 (fun n -> n * 2) 

    /// Sub-arrays are extracted using slicing notation.
    let evenNumbersSlice = evenNumbers.[0..500]

    /// You can loop over lists and arrays using 'for' loops.
    for word in array4 do 
        printfn "word: %s" word

    // You can modify the contents of an an array element by using the left arrow assignment operator.
    array2.[1] <- "WORLD!"

    /// You can transform arrays using 'Array'map' and other functional programming operations.
    /// The following calculates the sum of the lengths of the words that start with 'h'
    let sumOfLengthsOfWords = 
        array2
        |> Array.filter (fun x -> x.StartsWith "h")
        |> Array.sumBy (fun x -> x.Length)


/// Sequences are evaluated on-demand and are re-evaluated each time they are iterated. 
/// An F# sequence is just a System.Collections.Generic.IEnumerable<'T>.
/// Sequence processing functions can be applied to Lists and Arrays as well.
module Sequences = 

    /// This is the empty sequence.
    let seq1 = Seq.empty

    /// This a sequence of values.
    let seq2 = seq { yield "hello"; yield "world"; yield "and"; yield "hello"; yield "world"; yield "again" }

    /// This is an on-demand sequence from 1 to 100.
    let numbersSeq = seq { 1 .. 1000 }

    /// This is a sequence producing the words "hello" and "world"
    let seq3 = 
        seq { for word in seq2 do
                  if word.Contains("l") then 
                      yield word }

    /// This sequence producing the even numbers up to 2000.
    let evenNumbers = Seq.init 1001 (fun n -> n * 2) 

    let rnd = System.Random()

    /// This is an infinite sequence which is a random walk.
    /// This example uses yield! to return each element of a subsequence.
    let rec randomWalk x =
        seq { yield x
              yield! randomWalk (x + rnd.NextDouble() - 0.5) }

    /// This example shows the first 100 elements of the random walk.
    let first100ValuesOfRandomWalk = 
        randomWalk 5.0 
        |> Seq.truncate 100
        |> Seq.toList




/// Recursive functions can call themselves. In F#, functions are only recursive
/// when declared using 'let rec'.  The members of a set of type declarations are
/// always implicitly recursive. Entire namespaces and modules can be made recursive
/// through 'namespace rec' and 'module rec'.
module RecursiveFunctions  = 
              
    /// This example shows a recursive function that computes the factorial of an 
    /// integer. It uses 'let rec' to define a recursive function.
    let rec factorial n = 
        if n = 0 then 1 else n * factorial (n-1)

    /// This example shows a recursive function that computes the greatest common factor of two integers. 
    //  Since all of the recursive calls are tail calls, the compiler will turn the function into a loop,
    //  which improves performance and reduces memory consumption.
    let rec greatestCommonFactor a b =
        if a = 0 then b
        elif a < b then greatestCommonFactor a (b - a)
        else greatestCommonFactor (a - b) b

    /// Thsi example shows a recursive function that computes the sum of a list of integers.
    let rec sumList xs =
        match xs with
        | []    -> 0
        | y::ys -> y + sumList ys


/// A record is a collection of data items brought together into one object.
module RecordTypes = 

    /// This example shows how to define a new record type.  
    type ContactCard = 
        { Name     : string
          Phone    : string
          Verified : bool }
              
    /// This example shows how to define an instance of a record type.
    let contact1 = 
        { Name = "Alf" 
          Phone = "(206) 555-0157" 
          Verified = false }

    /// This example shows how to use "copy-and-update" on record values. It creates 
    /// a new record value that is a copy of contact1, but has different values for 
    /// the 'Phone' and 'Verified' fields
    let contact2 = 
        { contact1 with 
            Phone = "(206) 555-0112"
            Verified = true }

    /// This example shows how to write a function that processes a record value.
    /// It converts a 'ContactCard' object to a string.
    let showCard (c: ContactCard) = 
        c.Name + " Phone: " + c.Phone + (if not c.Verified then " (unverified)" else "")
        


/// Union types represent different possibilites for kinds of data.
module UnionTypes = 

    /// For example, the following represents the suit of a playing card.
    type Suit = 
        | Hearts 
        | Clubs 
        | Diamonds 
        | Spades

    /// A union type can also be used to represent the rank of a playing card.
    type Rank = 
        /// Represents the rank of cards 2 .. 10
        | Value of int
        | Ace
        | King
        | Queen
        | Jack

        /// Union and record types can implement object-oriented members.
        static member GetAllRanks() = 
            [ yield Ace
              for i in 2 .. 10 do yield Value i
              yield Jack
              yield Queen
              yield King ]
                                   
    /// This is a record type that combines a Suit and a Rank.
    type Card =  { Suit: Suit; Rank: Rank }
              
    /// This computed a list representing all the cards in the deck.
    let fullDeck = 
        [ for suit in [ Hearts; Diamonds; Clubs; Spades] do
              for rank in Rank.GetAllRanks() do 
                  yield { Suit=suit; Rank=rank } ]

    /// This example converts a 'Card' object to a string.
    let showCard (c: Card) = 
        let rankString = 
            match c.Rank with 
            | Ace -> "Ace"
            | King -> "King"
            | Queen -> "Queen"
            | Jack -> "Jack"
            | Value n -> string n
        let suitString = 
            match c.Suit with 
            | Clubs -> "clubs"
            | Diamonds -> "diamonds"
            | Spades -> "spades"
            | Hearts -> "hearts"
        rankString  + " of " + suitString

    /// This example prints all the cards in a playing deck.
    let printAllCards() = 
        for card in fullDeck do 
            printfn "%s" (showCard card)



/// Classes are one way of defining new object types in F#. The following example shows
/// how a simple vector class is defined.
module DefiningClasses = 

    /// The class's constructor takes two arguments: dx and dy, both of type 'double'. 
    type Vector2D(dx : double, dy : double) = 

        /// This internal field stores the length of the vector, computed when the 
        /// object is constructed
        let length = sqrt (dx*dx + dy*dy)

        // 'this' specifies a name for the object's self identifier.
        // In instance methods, it must appear before the member name.
        member this.DX = dx

        member this.DY = dy

        member this.Length = length

        member this.Scale(k) = Vector2D(k * this.DX, k * this.DY)
    
    /// This example defines an instance of the Vector2D class
    let vector1 = Vector2D(3.0, 4.0)

    /// Get a new scaled vector object, without modifying the original object 
    let vector2 = vector1.Scale(10.0)

    printfn "Length of vector1: %f      Length of vector2: %f" vector1.Length vector2.Length


/// Generic classes allow types to be defined with respect to a set of type parameters.
/// In the followiung, 'T is the type parameter for the class.
module DefiningGenericClasses = 

    type StateTracker<'T>(initialElement: 'T) = 

        /// This internal field store the states in a list
        let mutable states = [ initialElement ]

        /// Add a new element to the list of states
        member this.UpdateState newState = 
            states <- newState :: states  // use the '<-' operator to mutate the value

        /// Get the entire list of historical states
        member this.History = states

        /// Get the latest state
        member this.Current = states.Head

    /// An 'int' instance of the state tracker class. Note that the type parameter is inferred.
    let tracker = StateTracker 10

    // Add a state
    tracker.UpdateState 17



/// Interfaces are object types with only 'abstract' members.
/// Object types and object expressions can implement interfaces. 
module ImplementingInterfaces = 

    /// This is a type that implements IDisposable
    type ReadFile() =

        let file = new System.IO.StreamReader("readme.txt")

        member this.ReadLine() = file.ReadLine()

        // This is the implementation of IDisposable members
        interface System.IDisposable with
            member this.Dispose() = file.Close()


    /// This is an object that implements IDisposable.  Unlike
    /// C#, a new type definition is not needed to implement an interface
    /// you can just use an object expression instead.
    let interfaceImplementation =
        { new System.IDisposable with
            member this.Dispose() = printfn "disposed" }




/// Option values are any kind of value tagged with either 'Some' or 'None'.
/// They are used extensively in F# code to represent the cases where many other
/// languages would use null references.
module OptionValues = 

    /// First, define a type of ZipCodes.
    type ZipCode = ZipCode of string

    /// Next, define a type where the ZipCode is optionsl.
    type Customer = { ZipCode : ZipCode option }

    /// Next, define an interface type the represents an object to compute the shipping zone for the customer's zip code, 
    /// given implementations for the 'getState' and 'getShippingZone' abstract methods.
    type ShippingCalculator =
        abstract GetState : ZipCode -> string option
        abstract GetShippingZone : string -> int

    /// Next calculate a shipping zone for a customer using a calculator instance.
    let CustomerShippingZone (calculator: ShippingCalculator, customer: Customer) =
        customer.ZipCode |> Option.bind calculator.GetState |> Option.map calculator.GetShippingZone



/// Numeric types can be annotated with units.  F# arithmetic correctly computes the adjusted
/// numeric types.
module UnitsOfMeasure = 

    /// First open a collection of common unit names
    open Microsoft.FSharp.Data.UnitSystems.SI.UnitNames

    /// Define a unitized constant
    let sampelValue1  = 1600.0<meter>          

    /// Next, define a new unit type
    [<Measure>]
    type mile =
        /// Conversion factor mile to meter.
        static member asMeter = 1609.34<meter/mile>

    /// Define a unitized constant
    let sampleValue2  = 500.0<mile>          

    /// Compute  metric-system constant
    let sampleValue3 = sampleValue2 * mile.asMeter   

    printfn "After a %A race I would walk %A miles which would be %A meters" sampelValue1 sampleValue2 sampleValue3



/// The FSharp.Core library defines a range of parallel processing functions.  Here
/// you use some functions for parallel processing over arrays.
module ParallelArrayProgramming = 
              
    ///
    /// First, an array of inputs.
    let oneBigArray = [| 0 .. 100000 |]
    
    // Next, define a functions that does some CPU intensive computation.
    let rec computeSomeFunction x = 
        if x <= 2 then 1 
        else computeSomeFunction (x - 1) + computeSomeFunction (x - 2)
       
    // Next, do a parallel map over a large input array
    let computeResults() = oneBigArray |> Array.Parallel.map (fun x -> computeSomeFunction (x % 20))

    // Next, print the results.
    printfn "Parallel computation results: %A" (computeResults())



/// Events are a common idiom for .NET programming.
module Events = 

    /// First, create instance of Event object that consists of subscription point (event.Publish) and event trigger (event.Trigger)
    let simpleEvent = new Event<int>() 

    // Next, add handler to the event
    simpleEvent.Publish.Add(
        fun x -> printfn "this is handler was added with Publish.Add: %d" x)

    // Next, trigger the event
    simpleEvent.Trigger(5)

    // Next, create an instance of Event that follows standard .NET convention: (sender, EventArgs)
    let eventForDelegateType = new Event<EventHandler, EventArgs>()

    // Next, add a handler for this new event
    eventForDelegateType.Publish.AddHandler(
        EventHandler(fun _ _ -> printfn "this is handler was added with Publish.AddHandler"))

    // Next, trigger this event (note that sender argument should be set)
    eventForDelegateType.Trigger(null, EventArgs.Empty)



#if COMPILED
module BoilerPlateForForm = 
    [<System.STAThread>]
    do ()
    do System.Windows.Forms.Application.Run()
#endif
