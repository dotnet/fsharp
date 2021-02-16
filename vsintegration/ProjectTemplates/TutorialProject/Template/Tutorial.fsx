// @@@SampleHeader|This sample will guide you through elements of the F# language.@@@
//
// *******************************************************************************************************
//   @@@Instructions-Line1|To execute the code in F# Interactive, highlight a section of code and press Alt-Enter or right-click@@@
//   @@@Instructions-Line2|and select "Execute in Interactive".  You can open the F# Interactive Window from the "View" menu.@@@
// *******************************************************************************************************
//
// @@@MoreAbout|For more about F#, see:@@@
//     https://fsharp.org
//     https://docs.microsoft.com/en-us/dotnet/articles/fsharp/
//
// @@@SeeDocumentaton|To see this tutorial in documentation form, see:@@@
//     https://docs.microsoft.com/en-us/dotnet/articles/fsharp/tour
//
// @@@LearnMoreAbout|To learn more about applied F# programming, use@@@
//     https://fsharp.org/guides/enterprise/
//     https://fsharp.org/guides/cloud/
//     https://fsharp.org/guides/web/
//     https://fsharp.org/guides/data-science/
//
// @@@ToInstall-Line1|To install the Visual F# Power Tools, use@@@
//     @@@ToInstall-Line2|'Tools' --> 'Extensions and Updates' --> `Online` and search@@@
//
// @@@AdditionalTemplates-Line1|For additional templates to use with F#, see the 'Online Templates' in Visual Studio,@@@
//     @@@AdditionalTemplates-Line2|'New Project' --> 'Online Templates'@@@

// @@@SupportsComments|F# supports three kinds of comments:@@@

//  @@@DoubleSlash|1. Double-slash comments.  These are used in most situations.@@@
(*  @@@MLStyle|2. ML-style Block comments.  These aren't used that often.@@@ *)
/// @@@TripleSlash-Line1|3. Triple-slash comments.  These are used for documenting functions, types, and so on.@@@
///    @@@TripleSlash-Line2|They will appear as text when you hover over something which is decorated with these comments.@@@
///
///    @@@XmlComments-Line1|They also support .NET-style XML comments, which allow you to generate reference documentation,@@@
///    @@@XmlComments-Line2|and they also allow editors (such as Visual Studio) to extract information from them.@@@
///    @@@XmlComments-Line3|To learn more, see: https://docs.microsoft.com/en-us/dotnet/articles/fsharp/language-reference/xml-documentation@@@


// @@@OpenNamespaces|Open namespaces using the 'open' keyword.@@@
//
// @@@LearnMore|To learn more, see: https://docs.microsoft.com/en-us/dotnet/articles/fsharp/language-reference/import-declarations-the-open-keyword@@@
open System


/// @@@Module-Line1|A module is a grouping of F# code, such as values, types, and function values.@@@
/// @@@Module-Line2|Grouping code in modules helps keep related code together and helps avoid name conflicts in your program.@@@
///
/// @@@Module-Line3|To learn more, see: https://docs.microsoft.com/en-us/dotnet/articles/fsharp/language-reference/modules@@@
module IntegersAndNumbers =

    /// @@@SampleInt|This is a sample integer.@@@
    let sampleInteger = 176

    /// @@@SampleFloat|This is a sample floating point number.@@@
    let sampleDouble = 4.1

    /// @@@Computed-Line1|This computed a new number by some arithmetic.  Numeric types are converted using@@@
    /// @@@Computed-Line2|functions 'int', 'double' and so on.@@@
    let sampleInteger2 = (sampleInteger/4 + 5 - 7) * 4 + int sampleDouble

    /// @@@ListNumbers|This is a list of the numbers from 0 to 99.@@@
    let sampleNumbers = [ 0 .. 99 ]

    /// @@@ListSquares|This is a list of all tuples containing all the numbers from 0 to 99 and their squares.@@@
    let sampleTableOfSquares = [ for i in 0 .. 99 -> (i, i*i) ]

    // @@@PrintList1|The next line prints a list that includes tuples, using '%A' for generic printing.@@@
    printfn "The table of squares from 0 to 99 is:\n%A" sampleTableOfSquares

    // @@@SampleIntType|This is a sample integer with a type annotation@@@
    let sampleInteger3: int = 1


/// @@@ValuesImmutable-Line1|Values in F# are immutable by default.  They cannot be changed@@@
/// @@@ValuesImmutable-Line2|in the course of a program's execution unless explicitly marked as mutable.@@@
///
/// @@@ValuesImmutable-Line3|To learn more, see: https://docs.microsoft.com/en-us/dotnet/articles/fsharp/language-reference/values/index#why-immutable@@@
module Immutability =

    /// @@@LetKeyword-Line1|Binding a value to a name via 'let' makes it immutable.@@@
    ///
    /// @@@LetKeyword-Line2|The second line of code fails to compile because 'number' is immutable and bound.@@@
    /// @@@LetKeyword-Line3|Re-defining 'number' to be a different value is not allowed in F#.@@@
    let number = 2
    // let number = 3

    /// @@@MutableKeyword|A mutable binding.  This is required to be able to mutate the value of 'otherNumber'.@@@
    let mutable otherNumber = 2

    printfn "'otherNumber' is %d" otherNumber

    // @@@MutableAssignment-Line1|When mutating a value, use '<-' to assign a new value.@@@
    //
    // @@@MutableAssignment-Line2|You could not use '=' here for this purpose since it is used for equality@@@
    // @@@MutableAssignment-Line3|or other contexts such as 'let' or 'module'@@@
    otherNumber <- otherNumber + 1

    printfn "'otherNumber' changed to be %d" otherNumber


/// @@@FunctionsModule-Line1|Much of F# programming consists of defining functions that transform input data to produce@@@
/// @@@FunctionsModule-Line2|useful results.@@@
///
/// @@@FunctionsModule-Line3|To learn more, see: https://docs.microsoft.com/en-us/dotnet/articles/fsharp/language-reference/functions/@@@
module BasicFunctions =

    /// @@@LetFunction-Line1|You use 'let' to define a function. This one accepts an integer argument and returns an integer.@@@
    /// @@@LetFunction-Line2|Parentheses are optional for function arguments, except for when you use an explicit type annotation.@@@
    let sampleFunction1 x = x*x + 3

    /// @@@ApplyFunction-Line1|Apply the function, naming the function return result using 'let'.@@@
    /// @@@ApplyFunction-Line2|The variable type is inferred from the function return type.@@@
    let result1 = sampleFunction1 4573

    // @@@printf-Line1|This line uses '%d' to print the result as an integer. This is type-safe.@@@
    // @@@printf-Line2|If 'result1' were not of type 'int', then the line would fail to compile.@@@
    printfn "The result of squaring the integer 4573 and adding 3 is %d" result1

    /// @@@TypeAnnotation|When needed, annotate the type of a parameter name using '(argument:type)'.  Parentheses are required.@@@
    let sampleFunction2 (x:int) = 2*x*x - x/5 + 3

    let result2 = sampleFunction2 (7 + 4)
    printfn "The result of applying the 2nd sample function to (7 + 4) is %d" result2

    /// @@@Conditionals-Line1|Conditionals use if/then/elid/elif/else.@@@
    ///
    /// @@@Conditionals-Line2|Note that F# uses whitespace indentation-aware syntax, similar to languages like Python.@@@
    let sampleFunction3 x =
        if x < 100.0 then
            2.0*x*x - x/5.0 + 3.0
        else
            2.0*x*x + x/5.0 - 37.0

    let result3 = sampleFunction3 (6.5 + 4.5)

    // @@@printf-Line3|This line uses '%f' to print the result as a float.  As with '%d' above, this is type-safe.@@@
    printfn "The result of applying the 3rd sample function to (6.5 + 4.5) is %f" result3


/// @@@Booleans-Line1|Booleans are fundamental data types in F#.  Here are some examples of Booleans and conditional logic.@@@
///
/// @@@Booleans-Line2|To learn more, see:@@@
///     https://docs.microsoft.com/en-us/dotnet/articles/fsharp/language-reference/primitive-types
///     @@@Booleans-Line3|and@@@
///     https://docs.microsoft.com/en-us/dotnet/articles/fsharp/language-reference/symbol-and-operator-reference/boolean-operators
module Booleans =

    /// @@@BooleanValues|Booleans values are 'true' and 'false'.@@@
    let boolean1 = true
    let boolean2 = false

    /// @@@BooleanOperators|Operators on booleans are 'not', '&&' and '||'.@@@
    let boolean3 = not boolean1 && (boolean2 || false)

    // @@@BooleanPrintf|This line uses '%b'to print a boolean value.  This is type-safe.@@@
    printfn "The expression 'not boolean1 && (boolean2 || false)' is %b" boolean3


/// @@@Strings-Line1|Strings are fundamental data types in F#.  Here are some examples of Strings and basic String manipulation.@@@
///
/// @@@Strings-Line2|To learn more, see: https://docs.microsoft.com/en-us/dotnet/articles/fsharp/language-reference/strings@@@
module StringManipulation =

    /// @@@StringQuotes|Strings use double quotes.@@@
    let string1 = "Hello"
    let string2  = "world"

    /// @@@StringLiterals-Line1|Strings can also use @ to create a verbatim string literal.@@@
    /// @@@StringLiterals-Line2|This will ignore escape characters such as '\', '\n', '\t', etc.@@@
    let string3 = @"C:\Program Files\"

    /// @@@StringTripleQuotes|String literals can also use triple-quotes.@@@
    let string4 = """The computer said "hello world" when I told it to!"""

    /// @@@StringConcatenation|String concatenation is normally done with the '+' operator.@@@
    let helloWorld = string1 + " " + string2

    // @@@StringPrinting|This line uses '%s' to print a string value.  This is type-safe.@@@
    printfn "%s" helloWorld

    /// @@@Substrings-Line1|Substrings use the indexer notation.  This line extracts the first 7 characters as a substring.@@@
    /// @@@Substrings-Line2|Note that like many languages, Strings are zero-indexed in F#.@@@
    let substring = helloWorld.[0..6]
    printfn "%s" substring


/// @@@Tuples-Line1|Tuples are simple combinations of data values into a combined value.@@@
///
/// @@@Tuples-Line2|To learn more, see: https://docs.microsoft.com/en-us/dotnet/articles/fsharp/language-reference/tuples@@@
module Tuples =

    /// @@@TupleInteger|A simple tuple of integers.@@@
    let tuple1 = (1, 2, 3)

    /// @@@TupleFunction-Line1|A function that swaps the order of two values in a tuple.@@@
    ///
    /// @@@TupleFunction-Line2|F# Type Inference will automatically generalize the function to have a generic type,@@@
    /// @@@TupleFunction-Line3|meaning that it will work with any type.@@@
    let swapElems (a, b) = (b, a)

    printfn "The result of swapping (1, 2) is %A" (swapElems (1,2))

    /// @@@TupleMultiType-Line1|A tuple consisting of an integer, a string,@@@
    /// @@@TupleMultiType-Line2|and a double-precision floating point number.@@@
    let tuple2 = (1, "fred", 3.1415)

    printfn "tuple1: %A\ttuple2: %A" tuple1 tuple2

    /// @@@TupleTypeAnnotation-Line1|A simple tuple of integers with a type annotation.@@@
    /// @@@TupleTypeAnnotation-Line2|Type annotations for tuples use the * symbol to separate elements@@@
    let tuple3: int * int = (5, 9)

    /// @@@StructTuple-Line1|Tuples are normally objects, but they can also be represented as structs.@@@
    ///
    /// @@@StructTuple-Line2|These interoperate completely with structs in C# and Visual Basic.NET; however,@@@
    /// @@@StructTuple-Line3|struct tuples are not implicitly convertable with object tuples (often called reference tuples).@@@
    ///
    /// @@@StructTuple-Line4|The second line below will fail to compile because of this.  Uncomment it to see what happens.@@@
    let sampleStructTuple = struct (1, 2)
    //let thisWillNotCompile: (int*int) = struct (1, 2)

    // @@@TupleConvert-Line1|Although you cannot implicitly convert between struct tuples and reference tuples,@@@
    // @@@TupleConvert-Line2|you can explicitly convert via pattern matching, as demonstrated below.@@@
    let convertFromStructTuple (struct(a, b)) = (a, b)
    let convertToStructTuple (a, b) = struct(a, b)

    printfn "Struct Tuple: %A\nReference tuple made from the Struct Tuple: %A" sampleStructTuple (sampleStructTuple |> convertFromStructTuple)


/// @@@Pipes-Line1|The F# pipe operators ('|>', '<|', etc.) and F# composition operators ('>>', '<<')@@@
/// @@@Pipes-Line2|are used extensively when processing data.  These operators are themselves functions@@@
/// @@@Pipes-Line3|which make use of Partial Application.@@@
///
/// @@@Pipes-Line4|To learn more about these operators, see: https://docs.microsoft.com/en-us/dotnet/articles/fsharp/language-reference/functions/#function-composition-and-pipelining@@@
/// @@@Pipes-Line5|To learn more about Partial Application, see: https://docs.microsoft.com/en-us/dotnet/articles/fsharp/language-reference/functions/#partial-application-of-arguments@@@
module PipelinesAndComposition =

    /// @@@Squares|Squares a value.@@@
    let square x = x * x

    /// @@@AddOne|Adds 1 to a value.@@@
    let addOne x = x + 1

    /// @@@TestOdd|Tests if an integer value is odd via modulo.@@@
    let isOdd x = x % 2 <> 0

    /// @@@NumberList1|A list of 5 numbers.  More on lists later.@@@
    let numbers = [ 1; 2; 3; 4; 5 ]

    /// @@@FilterWithoutPipes-Line1|Given a list of integers, it filters out the even numbers,@@@
    /// @@@FilterWithoutPipes-Line2|squares the resulting odds, and adds 1 to the squared odds.@@@
    let squareOddValuesAndAddOne values =
        let odds = List.filter isOdd values
        let squares = List.map square odds
        let result = List.map addOne squares
        result

    printfn "processing %A through 'squareOddValuesAndAddOne' produces: %A" numbers (squareOddValuesAndAddOne numbers)

    /// @@@FilterShorter-Line1|A shorter way to write 'squareOddValuesAndAddOne' is to nest each@@@
    /// @@@FilterShorter-Line2|sub-result into the function calls themselves.@@@
    ///
    /// @@@FilterShorter-Line3|This makes the function much shorter, but it's difficult to see the@@@
    /// @@@FilterShorter-Line4|order in which the data is processed.@@@
    let squareOddValuesAndAddOneNested values =
        List.map addOne (List.map square (List.filter isOdd values))

    printfn "processing %A through 'squareOddValuesAndAddOneNested' produces: %A" numbers (squareOddValuesAndAddOneNested numbers)

    /// @@@FilterWithPipes-Line1|A preferred way to write 'squareOddValuesAndAddOne' is to use F# pipe operators.@@@
    /// @@@FilterWithPipes-Line2|This allows you to avoid creating intermediate results, but is much more readable@@@
    /// @@@FilterWithPipes-Line3|than nesting function calls like 'squareOddValuesAndAddOneNested'@@@
    let squareOddValuesAndAddOnePipeline values =
        values
        |> List.filter isOdd
        |> List.map square
        |> List.map addOne

    printfn "processing %A through 'squareOddValuesAndAddOnePipeline' produces: %A" numbers (squareOddValuesAndAddOnePipeline numbers)

    /// @@@PipeInLambda-Line1|You can shorten 'squareOddValuesAndAddOnePipeline' by moving the second `List.map` call@@@
    /// @@@PipeInLambda-Line2|into the first, using a Lambda Function.@@@
    ///
    /// @@@PipeInLambda-Line3|Note that pipelines are also being used inside the lambda function.  F# pipe operators@@@
    /// @@@PipeInLambda-Line4|can be used for single values as well.  This makes them very powerful for processing data.@@@
    let squareOddValuesAndAddOneShorterPipeline values =
        values
        |> List.filter isOdd
        |> List.map(fun x -> x |> square |> addOne)

    printfn "processing %A through 'squareOddValuesAndAddOneShorterPipeline' produces: %A" numbers (squareOddValuesAndAddOneShorterPipeline numbers)

    /// @@@PipesComposition-Line1|Lastly, you can eliminate the need to explicitly take 'values' in as a parameter by using '>>'@@@
    /// @@@PipesComposition-Line2|to compose the two core operations: filtering out even numbers, then squaring and adding one.@@@
    /// @@@PipesComposition-Line3|Likewise, the 'fun x -> ...' bit of the lambda expression is also not needed, because 'x' is simply@@@
    /// @@@PipesComposition-Line4|being defined in that scope so that it can be passed to a functional pipeline.  Thus, '>>' can be used@@@
    /// @@@PipesComposition-Line5|there as well.@@@
    ///
    /// @@@PipesComposition-Line6|The result of 'squareOddValuesAndAddOneComposition' is itself another function which takes a@@@
    /// @@@PipesComposition-Line7|list of integers as its input.  If you execute 'squareOddValuesAndAddOneComposition' with a list@@@
    /// @@@PipesComposition-Line8|of integers, you'll notice that it produces the same results as previous functions.@@@
    ///
    /// @@@PipesComposition-Line9|This is using what is known as function composition.  This is possible because functions in F#@@@
    /// @@@PipesComposition-Line10|use Partial Application and the input and output types of each data processing operation match@@@
    /// @@@PipesComposition-Line11|the signatures of the functions we're using.@@@
    let squareOddValuesAndAddOneComposition =
        List.filter isOdd >> List.map (square >> addOne)

    printfn "processing %A through 'squareOddValuesAndAddOneComposition' produces: %A" numbers (squareOddValuesAndAddOneComposition numbers)


/// @@@Lists-Line1|Lists are ordered, immutable, singly-linked lists.  They are eager in their evaluation.@@@
///
/// @@@Lists-Line2|This module shows various ways to generate lists and process lists with some functions@@@
/// @@@Lists-Line3|in the 'List' module in the F# Core Library.@@@
///
/// @@@Lists-Line4|To learn more, see: https://docs.microsoft.com/en-us/dotnet/articles/fsharp/language-reference/lists@@@
module Lists =

    /// @@@ListEmptyDefinition|Lists are defined using [ ... ].  This is an empty list.@@@
    let list1 = [ ]

    /// @@@ListElementDefinition|This is a list with 3 elements.  ';' is used to separate elements on the same line.@@@
    let list2 = [ 1; 2; 3 ]

    /// @@@ListNewlineElements|You can also separate elements by placing them on their own lines.@@@
    let list3 = [
        1
        2
        3
    ]

    /// @@@NumberList2|This is a list of integers from 1 to 1000@@@
    let numberList = [ 1 .. 1000 ]

    /// @@@ListComputation-Line1|Lists can also be generated by computations. This is a list containing@@@
    /// @@@ListComputation-Line2|all the days of the year.@@@
    let daysList =
        [ for month in 1 .. 12 do
              for day in 1 .. System.DateTime.DaysInMonth(2017, month) do
                  yield System.DateTime(2017, month, day) ]

    // @@@PrintList2|Print the first 5 elements of 'daysList' using 'List.take'.@@@
    printfn "The first 5 days of 2017 are: %A" (daysList |> List.take 5)

    /// @@@ListComputationConditional-Line1|Computations can include conditionals.  This is a list containing the tuples@@@
    /// @@@ListComputationConditional-Line2|which are the coordinates of the black squares on a chess board.@@@
    let blackSquares =
        [ for i in 0 .. 7 do
              for j in 0 .. 7 do
                  if (i+j) % 2 = 1 then
                      yield (i, j) ]

    /// @@@ListMap-Line1|Lists can be transformed using 'List.map' and other functional programming combinators.@@@
    /// @@@ListMap-Line2|This definition produces a new list by squaring the numbers in numberList, using the pipeline@@@
    /// @@@ListMap-Line3|operator to pass an argument to List.map.@@@
    let squares =
        numberList
        |> List.map (fun x -> x*x)

    /// @@@ListFilter-Line1|There are many other list combinations. The following computes the sum of the squares of the@@@
    /// @@@ListFilter-Line2|numbers divisible by 3.@@@
    let sumOfSquares =
        numberList
        |> List.filter (fun x -> x % 3 = 0)
        |> List.sumBy (fun x -> x * x)

    printfn "The sum of the squares of numbers up to 1000 that are divisible by 3 is: %d" sumOfSquares


/// @@@Arrays-Line1|Arrays are fixed-size, mutable collections of elements of the same type.@@@
///
/// @@@Arrays-Line2|Although they are similar to Lists (they support enumeration and have similar combinators for data processing),@@@
/// @@@Arrays-Line3|they are generally faster and support fast random access.  This comes at the cost of being less safe by being mutable.@@@
///
/// @@@Arrays-Line4|To learn more, see: https://docs.microsoft.com/en-us/dotnet/articles/fsharp/language-reference/arrays@@@
module Arrays =

    /// @@@EmptyArray|This is The empty array.  Note that the syntax is similar to that of Lists, but uses `[| ... |]` instead.@@@
    let array1 = [| |]

    /// @@@ArrayConstructionList|Arrays are specified using the same range of constructs as lists.@@@
    let array2 = [| "hello"; "world"; "and"; "hello"; "world"; "again" |]

    /// @@@ArrayConstructionRange|This is an array of numbers from 1 to 1000.@@@
    let array3 = [| 1 .. 1000 |]

    /// @@@ArrayComputationConstruction|This is an array containing only the words "hello" and "world".@@@
    let array4 =
        [| for word in array2 do
               if word.Contains("l") then
                   yield word |]

    /// @@@ArrayInit|This is an array initialized by index and containing the even numbers from 0 to 2000.@@@
    let evenNumbers = Array.init 1001 (fun n -> n * 2)

    /// @@@ArraySlicing|Sub-arrays are extracted using slicing notation.@@@
    let evenNumbersSlice = evenNumbers.[0..500]

    /// @@@ArrayLooping|You can loop over arrays and lists using 'for' loops.@@@
    for word in array4 do
        printfn "word: %s" word

    // @@@ArrayAssignment-Line1|You can modify the contents of an an array element by using the left arrow assignment operator.@@@
    //
    // @@@ArrayAssignment-Line2|To learn more about this operator, see: https://docs.microsoft.com/en-us/dotnet/articles/fsharp/language-reference/values/index#mutable-variables@@@
    array2.[1] <- "WORLD!"

    /// @@@ArrayMap-Line1|You can transform arrays using 'Array.map' and other functional programming operations.@@@
    /// @@@ArrayMap-Line2|The following calculates the sum of the lengths of the words that start with 'h'.@@@
    let sumOfLengthsOfWords =
        array2
        |> Array.filter (fun x -> x.StartsWith "h")
        |> Array.sumBy (fun x -> x.Length)

    printfn "The sum of the lengths of the words in Array 2 is: %d" sumOfLengthsOfWords


/// @@@Sequences-Line1|Sequences are a logical series of elements, all of the same type.  These are a more general type than Lists and Arrays.@@@
///
/// @@@Sequences-Line2|Sequences are evaluated on-demand and are re-evaluated each time they are iterated.@@@
/// @@@Sequences-Line3|An F# sequence is an alias for a .NET System.Collections.Generic.IEnumerable<'T>.@@@
///
/// @@@Sequences-Line4|Sequence processing functions can be applied to Lists and Arrays as well.@@@
///
/// @@@Sequences-Line5|To learn more, see: https://docs.microsoft.com/en-us/dotnet/articles/fsharp/language-reference/sequences@@@
module Sequences =

    /// @@@EmptySequence|This is the empty sequence.@@@
    let seq1 = Seq.empty

    /// @@@SequenceOfValues|This a sequence of values.@@@
    let seq2 = seq { yield "hello"; yield "world"; yield "and"; yield "hello"; yield "world"; yield "again" }

    /// @@@OnDemandSequence|This is an on-demand sequence from 1 to 1000.@@@
    let numbersSeq = seq { 1 .. 1000 }

    /// @@@SequenceComposition|This is a sequence producing the words "hello" and "world"@@@
    let seq3 =
        seq { for word in seq2 do
                  if word.Contains("l") then
                      yield word }

    /// @@@SequenceInit|This sequence producing the even numbers up to 2000.@@@
    let evenNumbers = Seq.init 1001 (fun n -> n * 2)

    let rnd = System.Random()

    /// @@@InfiniteSequence-Line1|This is an infinite sequence which is a random walk.@@@
    /// @@@InfiniteSequence-Line2|This example uses yield! to return each element of a subsequence.@@@
    let rec randomWalk x =
        seq { yield x
              yield! randomWalk (x + rnd.NextDouble() - 0.5) }

    /// @@@Sequence100Elements|This example shows the first 100 elements of the random walk.@@@
    let first100ValuesOfRandomWalk =
        randomWalk 5.0
        |> Seq.truncate 100
        |> Seq.toList

    printfn "First 100 elements of a random walk: %A" first100ValuesOfRandomWalk


/// @@@RecursiveFunctions-Line1|Recursive functions can call themselves. In F#, functions are only recursive@@@
/// @@@RecursiveFunctions-Line2|when declared using 'let rec'.@@@
///
/// @@@RecursiveFunctions-Line3|Recursion is the preferred way to process sequences or collections in F#.@@@
///
/// @@@RecursiveFunctions-Line4|To learn more, see: https://docs.microsoft.com/en-us/dotnet/articles/fsharp/language-reference/functions/index#recursive-functions@@@
module RecursiveFunctions =

    /// @@@RecFunDef-Line1|This example shows a recursive function that computes the factorial of an@@@
    /// @@@RecRunDef-Line2|integer. It uses 'let rec' to define a recursive function.@@@
    let rec factorial n =
        if n = 0 then 1 else n * factorial (n-1)

    printfn "Factorial of 6 is: %d" (factorial 6)

    /// @@@RecGcd-Line1|Computes the greatest common factor of two integers.@@@
    ///
    /// @@@RecGcd-Line2|Since all of the recursive calls are tail calls,@@@
    /// @@@RecGcd-Line3|the compiler will turn the function into a loop,@@@
    /// @@@RecGcd-Line4|which improves performance and reduces memory consumption.@@@
    let rec greatestCommonFactor a b =
        if a = 0 then b
        elif a < b then greatestCommonFactor a (b - a)
        else greatestCommonFactor (a - b) b

    printfn "The Greatest Common Factor of 300 and 620 is %d" (greatestCommonFactor 300 620)

    /// @@@RecSumList|This example computes the sum of a list of integers using recursion.@@@
    let rec sumList xs =
        match xs with
        | []    -> 0
        | y::ys -> y + sumList ys

    /// @@@RecSumListTail|This makes 'sumList' tail recursive, using a helper function with a result accumulator.@@@
    let rec private sumListTailRecHelper accumulator xs =
        match xs with
        | []    -> accumulator
        | y::ys -> sumListTailRecHelper (accumulator+y) ys

    /// @@@RecSumListTailInvoke-Line1|This invokes the tail recursive helper function, providing '0' as a seed accumulator.@@@
    /// @@@RecSumListTailInvoke-Line2|An approach like this is common in F#.@@@
    let sumListTailRecursive xs = sumListTailRecHelper 0 xs

    let oneThroughTen = [1; 2; 3; 4; 5; 6; 7; 8; 9; 10]

    printfn "The sum 1-10 is %d" (sumListTailRecursive oneThroughTen)


/// @@@Records-Line1|Records are an aggregate of named values, with optional members (such as methods).@@@
/// @@@Records-Line2|They are immutable and have structural equality semantics.@@@
///
/// @@@Records-Line3|To learn more, see: https://docs.microsoft.com/en-us/dotnet/articles/fsharp/language-reference/records@@@
module RecordTypes =

    /// @@@RecordDefinition|This example shows how to define a new record type.@@@
    type ContactCard =
        { Name     : string
          Phone    : string
          Verified : bool }

    /// @@@RecordInstantiation1|This example shows how to instantiate a record type.@@@
    let contact1 =
        { Name = "Alf"
          Phone = "(206) 555-0157"
          Verified = false }

    /// @@@RecordInstantiation2|You can also do this on the same line with ';' separators.@@@
    let contactOnSameLine = { Name = "Alf"; Phone = "(206) 555-0157"; Verified = false }

    /// @@@UpdateRecord-Line1|This example shows how to use "copy-and-update" on record values. It creates@@@
    /// @@@UpdateRecord-Line2|a new record value that is a copy of contact1, but has different values for@@@
    /// @@@UpdateRecord-Line3|the 'Phone' and 'Verified' fields.@@@
    ///
    /// @@@UpdateRecord-Line4|To learn more, see: https://docs.microsoft.com/en-us/dotnet/articles/fsharp/language-reference/copy-and-update-record-expressions@@@
    let contact2 =
        { contact1 with
            Phone = "(206) 555-0112"
            Verified = true }

    /// @@@ProcessRecord-Line1|This example shows how to write a function that processes a record value.@@@
    /// @@@ProcessRecord-Line2|It converts a 'ContactCard' object to a string.@@@
    let showContactCard (c: ContactCard) =
        c.Name + " Phone: " + c.Phone + (if not c.Verified then " (unverified)" else "")

    printfn "Alf's Contact Card: %s" (showContactCard contact1)

    /// @@@RecordWithMember-Line1|This is an example of a Record with a member.@@@
    type ContactCardAlternate =
        { Name     : string
          Phone    : string
          Address  : string
          Verified : bool }

        /// @@@RecordWithMember-Line2|Members can implement object-oriented members.@@@
        member this.PrintedContactCard =
            this.Name + " Phone: " + this.Phone + (if not this.Verified then " (unverified)" else "") + this.Address

    let contactAlternate =
        { Name = "Alf"
          Phone = "(206) 555-0157"
          Verified = false
          Address = "111 Alf Street" }

    // @@@RecordAccess|Members are accessed via the '.' operator on an instantiated type.@@@
    printfn "Alf's alternate contact card is %s" contactAlternate.PrintedContactCard

    /// @@@RecordStruct-Line1|Records can also be represented as structs via the 'Struct' attribute.@@@
    /// @@@RecordStruct-Line2|This is helpful in situations where the performance of structs outweighs@@@
    /// @@@RecordStruct-Line3|the flexibility of reference types.@@@
    [<Struct>]
    type ContactCardStruct =
        { Name     : string
          Phone    : string
          Verified : bool }


/// @@@DiscriminatedUnions-Line1|Discriminated Unions (DU for short) are values which could be a number of named forms or cases.@@@
/// @@@DiscriminatedUnions-Line2|Data stored in DUs can be one of several distinct values.@@@
///
/// @@@DiscriminatedUnions-Line3|To learn more, see: https://docs.microsoft.com/en-us/dotnet/articles/fsharp/language-reference/discriminated-unions@@@
module DiscriminatedUnions =

    /// @@@CardSuit|The following represents the suit of a playing card.@@@
    type Suit =
        | Hearts
        | Clubs
        | Diamonds
        | Spades

    /// @@@CardRank|A Disciminated Union can also be used to represent the rank of a playing card.@@@
    type Rank =
        /// @@@CardRankValue|Represents the rank of cards 2 .. 10@@@
        | Value of int
        | Ace
        | King
        | Queen
        | Jack

        /// @@@CardMember|Discriminated Unions can also implement object-oriented members.@@@
        static member GetAllRanks() =
            [ yield Ace
              for i in 2 .. 10 do yield Value i
              yield Jack
              yield Queen
              yield King ]

    /// @@@CardType-Line1|This is a record type that combines a Suit and a Rank.@@@
    /// @@@CardType-Line2|It's common to use both Records and Disciminated Unions when representing data.@@@
    type Card = { Suit: Suit; Rank: Rank }

    /// @@@ComputeFullDeck|This computes a list representing all the cards in the deck.@@@
    let fullDeck =
        [ for suit in [ Hearts; Diamonds; Clubs; Spades] do
              for rank in Rank.GetAllRanks() do
                  yield { Suit=suit; Rank=rank } ]

    /// @@@CardToString|This example converts a 'Card' object to a string.@@@
    let showPlayingCard (c: Card) =
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

    /// @@@PrintAllCards|This example prints all the cards in a playing deck.@@@
    let printAllCards() =
        for card in fullDeck do
            printfn "%s" (showPlayingCard card)

    // @@@SingleCaseDu-Line1|Single-case DUs are often used for domain modeling.  This can buy you extra type safety@@@
    // @@@SingleCaseDu-Line2|over primitive types such as strings and ints.@@@
    //
    // @@@SingleCaseDu-Line3|Single-case DUs cannot be implicitly converted to or from the type they wrap.@@@
    // @@@SingleCaseDu-Line4|For example, a function which takes in an Address cannot accept a string as that input,@@@
    // @@@SingleCaseDu-Line5|or vive/versa.@@@
    type Address = Address of string
    type Name = Name of string
    type SSN = SSN of int

    // @@@InstantiateSingleCaseDu|You can easily instantiate a single-case DU as follows.@@@
    let address = Address "111 Alf Way"
    let name = Name "Alf"
    let ssn = SSN 1234567890

    /// @@@UnwrapSingleCaseDu|When you need the value, you can unwrap the underlying value with a simple function.@@@
    let unwrapAddress (Address a) = a
    let unwrapName (Name n) = n
    let unwrapSSN (SSN s) = s

    // @@@PrintSingleCaseDu|Printing single-case DUs is simple with unwrapping functions.@@@
    printfn "Address: %s, Name: %s, and SSN: %d" (address |> unwrapAddress) (name |> unwrapName) (ssn |> unwrapSSN)

    /// @@@DuRecursiveDef-Line1|Disciminated Unions also support recursive definitions.@@@
    ///
    /// @@@DuRecursiveDef-Line2|This represents a Binary Search Tree, with one case being the Empty tree,@@@
    /// @@@DuRecursiveDef-Line3|and the other being a Node with a value and two subtrees.@@@
    type BST<'T> =
        | Empty
        | Node of value:'T * left: BST<'T> * right: BST<'T>

    /// @@@SearchBinaryTree-Line1|Check if an item exists in the binary search tree.@@@
    /// @@@SearchBinaryTree-Line2|Searches recursively using Pattern Matching.  Returns true if it exists; otherwise, false.@@@
    let rec exists item bst =
        match bst with
        | Empty -> false
        | Node (x, left, right) ->
            if item = x then true
            elif item < x then (exists item left) // @@@CheckLeftSubtree|Check the left subtree.@@@
            else (exists item right) // @@@CheckRightSubtree|Check the right subtree.@@@

    /// @@@BinaryTreeInsert-Line1|Inserts an item in the Binary Search Tree.@@@
    /// @@@BinaryTreeInsert-Line2|Finds the place to insert recursively using Pattern Matching, then inserts a new node.@@@
    /// @@@BinaryTreeInsert-Line3|If the item is already present, it does not insert anything.@@@
    let rec insert item bst =
        match bst with
        | Empty -> Node(item, Empty, Empty)
        | Node(x, left, right) as node ->
            if item = x then node // @@@BinaryTreeInsert-Line4|No need to insert, it already exists; return the node.@@@
            elif item < x then Node(x, insert item left, right) // @@@BinaryTreeInsert-Line5|Call into left subtree.@@@
            else Node(x, left, insert item right) // @@@BinaryTreeInsert-Line6|Call into right subtree.@@@

    /// @@@DuStruct-Line1|Discriminated Unions can also be represented as structs via the 'Struct' attribute.@@@
    /// @@@DuStruct-Line2|This is helpful in situations where the performance of structs outweighs@@@
    /// @@@DuStruct-Line3|the flexibility of reference types.@@@
    ///
    /// @@@DuStruct-Line4|However, there are two important things to know when doing this:@@@
    ///     @@@DuStruct-Line5|1. A struct DU cannot be recursively-defined.@@@
    ///     @@@DuStruct-Line6|2. A struct DU must have unique names for each of its cases.@@@
    [<Struct>]
    type Shape =
        | Circle of radius: float
        | Square of side: float
        | Triangle of height: float * width: float


/// @@@PatternMatching-Line1|Pattern Matching is a feature of F# that allows you to utilize Patterns,@@@
/// @@@PatternMatching-Line2|which are a way to compare data with a logical structure or structures,@@@
/// @@@PatternMatching-Line3|decompose data into constituent parts, or extract information from data in various ways.@@@
/// @@@PatternMatching-Line4|You can then dispatch on the "shape" of a pattern via Pattern Matching.@@@
///
/// @@@PatternMatching-Line5|To learn more, see: https://docs.microsoft.com/en-us/dotnet/articles/fsharp/language-reference/pattern-matching@@@
module PatternMatching =

    /// @@@PersonRecord|A record for a person's first and last name@@@
    type Person = {
        First : string
        Last  : string
    }

    /// @@@EmployeeDu|A Discriminated Union of 3 different kinds of employees@@@
    type Employee =
        | Engineer of engineer: Person
        | Manager of manager: Person * reports: List<Employee>
        | Executive of executive: Person * reports: List<Employee> * assistant: Employee

    /// @@@CountEmployees-Line1|Count everyone underneath the employee in the management hierarchy,@@@
    /// @@@CountEmployees-Line2|including the employee.@@@
    let rec countReports(emp : Employee) =
        1 + match emp with
            | Engineer(id) ->
                0
            | Manager(id, reports) ->
                reports |> List.sumBy countReports
            | Executive(id, reports, assistant) ->
                (reports |> List.sumBy countReports) + countReports assistant


    /// @@@FindDave-Line1|Find all managers/executives named "Dave" who do not have any reports.@@@
    /// @@@FindDave-Line2|This uses the 'function' shorthand to as a lambda expression.@@@
    let rec findDaveWithOpenPosition(emps : List<Employee>) =
        emps
        |> List.filter(function
                       | Manager({First = "Dave"}, []) -> true // @@@MatchEmptyList|[] matches an empty list.@@@
                       | Executive({First = "Dave"}, [], _) -> true
                       | _ -> false) // @@@MatchWildcard-Line1|'_' is a wildcard pattern that matches anything.@@@
                                     // @@@MatchWildCard-Line2|This handles the "or else" case.@@@

    /// @@@MatchShorthand-Line1|You can also use the shorthand function construct for pattern matching,@@@
    /// @@@MatchShorthand-Line2|which is useful when you're writing functions which make use of Partial Application.@@@
    let private parseHelper f = f >> function
        | (true, item) -> Some item
        | (false, _) -> None

    let parseDateTimeOffset = parseHelper DateTimeOffset.TryParse

    let result = parseDateTimeOffset "1970-01-01"
    match result with
    | Some dto -> printfn "It parsed!"
    | None -> printfn "It didn't parse!"

    // @@@ParseHelpers|Define some more functions which parse with the helper function.@@@
    let parseInt = parseHelper Int32.TryParse
    let parseDouble = parseHelper Double.TryParse
    let parseTimeSpan = parseHelper TimeSpan.TryParse

    // @@@ActivePatterns-Line1|Active Patterns are another powerful construct to use with pattern matching.@@@
    // @@@ActivePatterns-Line2|They allow you to partition input data into custom forms, decomposing them at the pattern match call site.@@@
    //
    // @@@ActivePatterns-Line3|To learn more, see: https://docs.microsoft.com/en-us/dotnet/articles/fsharp/language-reference/active-patterns@@@
    let (|Int|_|) = parseInt
    let (|Double|_|) = parseDouble
    let (|Date|_|) = parseDateTimeOffset
    let (|TimeSpan|_|) = parseTimeSpan

    /// @@@MatchActivePattern|Pattern Matching via 'function' keyword and Active Patterns often looks like this.@@@
    let printParseResult = function
        | Int x -> printfn "%d" x
        | Double x -> printfn "%f" x
        | Date d -> printfn "%s" (d.ToString())
        | TimeSpan t -> printfn "%s" (t.ToString())
        | _ -> printfn "Nothing was parse-able!"

    // @@@PrintParse|Call the printer with some different values to parse.@@@
    printParseResult "12"
    printParseResult "12.045"
    printParseResult "12/28/2016"
    printParseResult "9:01PM"
    printParseResult "banana!"


/// @@@Option-Line1|Option values are any kind of value tagged with either 'Some' or 'None'.@@@
/// @@@Option-Line2|They are used extensively in F# code to represent the cases where many other@@@
/// @@@Option-Line3|languages would use null references.@@@
///
/// @@@Option-Line4|To learn more, see: https://docs.microsoft.com/en-us/dotnet/articles/fsharp/language-reference/options@@@
module OptionValues =

    /// @@@ZipCode|First, define a zipcode defined via Single-case Discriminated Union.@@@
    type ZipCode = ZipCode of string

    /// @@@Customer|Next, define a type where the ZipCode is optional.@@@
    type Customer = { ZipCode: ZipCode option }

    /// @@@ShippingCalculator-Line1|Next, define an interface type that represents an object to compute the shipping zone for the customer's zip code,@@@
    /// @@@ShippingCalculator-Line2|given implementations for the 'getState' and 'getShippingZone' abstract methods.@@@
    type ShippingCalculator =
        abstract GetState : ZipCode -> string option
        abstract GetShippingZone : string -> int

    /// @@@CalcShippingZone-Line1|Next, calculate a shipping zone for a customer using a calculator instance.@@@
    /// @@@CalcShippingZone-Line2|This uses combinators in the Option module to allow a functional pipeline for@@@
    /// @@@CalcShippingZone-Line3|transforming data with Optionals.@@@
    let CustomerShippingZone (calculator: ShippingCalculator, customer: Customer) =
        customer.ZipCode
        |> Option.bind calculator.GetState
        |> Option.map calculator.GetShippingZone


/// @@@UnitsOfMeasure-Line1|Units of measure are a way to annotate primitive numeric types in a type-safe way.@@@
/// @@@UnitsOfMeasure-Line2|You can then perform type-safe arithmetic on these values.@@@
///
/// @@@UnitsOfMeasure-Line3|To learn more, see: https://docs.microsoft.com/en-us/dotnet/articles/fsharp/language-reference/units-of-measure@@@
module UnitsOfMeasure =

    /// @@@CommonUnits|First, open a collection of common unit names@@@
    open Microsoft.FSharp.Data.UnitSystems.SI.UnitNames

    /// @@@DefineUnitConstant|Define a unitized constant@@@
    let sampleValue1 = 1600.0<meter>

    /// @@@MileUnit|Next, define a new unit type@@@
    [<Measure>]
    type mile =
        /// @@@MileToMeter|Conversion factor mile to meter.@@@
        static member asMeter = 1609.34<meter/mile>

    /// @@@DefineMileConstant|Define a unitized constant@@@
    let sampleValue2  = 500.0<mile>

    /// @@@ComputeMileToMeter|Compute  metric-system constant@@@
    let sampleValue3 = sampleValue2 * mile.asMeter

    // @@@PrintUnitsOfMeasure|Values using Units of Measure can be used just like the primitive numeric type for things like printing.@@@
    printfn "After a %f race I would walk %f miles which would be %f meters" sampleValue1 sampleValue2 sampleValue3


/// @@@Classes-Line1|Classes are a way of defining new object types in F#, and support standard Object-oriented constructs.@@@
/// @@@Classes-Line2|They can have a variety of members (methods, properties, events, etc.)@@@
///
/// @@@Classes-Line3|To learn more about Classes, see: https://docs.microsoft.com/en-us/dotnet/articles/fsharp/language-reference/classes@@@
///
/// @@@Classes-Line4|To learn more about Members, see: https://docs.microsoft.com/en-us/dotnet/articles/fsharp/language-reference/members@@@
module DefiningClasses =

    /// @@@Vector-Line1|A simple two-dimensional Vector class.@@@
    ///
    /// @@@Vector-Line2|The class's constructor is on the first line,@@@
    /// @@@Vector-Line3|and takes two arguments: dx and dy, both of type 'double'.@@@
    type Vector2D(dx : double, dy : double) =

        /// @@@ClassInternalField-Line1|This internal field stores the length of the vector, computed when the@@@
        /// @@@ClassInternalField-Line2|object is constructed@@@
        let length = sqrt (dx*dx + dy*dy)

        // @@@ThisKeyword-Line1|'this' specifies a name for the object's self identifier.@@@
        // @@@ThisKeyword-Line2|In instance methods, it must appear before the member name.@@@
        member this.DX = dx

        member this.DY = dy

        member this.Length = length

        /// @@@MemberMethod|This member is a method.  The previous members were properties.@@@
        member this.Scale(k) = Vector2D(k * this.DX, k * this.DY)

    /// @@@InstantiateClass|This is how you instantiate the Vector2D class.@@@
    let vector1 = Vector2D(3.0, 4.0)

    /// @@@ScaledVector|Get a new scaled vector object, without modifying the original object.@@@
    let vector2 = vector1.Scale(10.0)

    printfn "Length of vector1: %f\nLength of vector2: %f" vector1.Length vector2.Length


/// @@@GenericClasses-Line1|Generic classes allow types to be defined with respect to a set of type parameters.@@@
/// @@@GenericClasses-Line2|In the following, 'T is the type parameter for the class.@@@
///
/// @@@GenericClasses-Line3|To learn more, see: https://docs.microsoft.com/en-us/dotnet/articles/fsharp/language-reference/generics/@@@
module DefiningGenericClasses =

    type StateTracker<'T>(initialElement: 'T) =

        /// @@@InternalField|This internal field store the states in a list.@@@
        let mutable states = [ initialElement ]

        /// @@@AddElement|Add a new element to the list of states.@@@
        member this.UpdateState newState =
            states <- newState :: states  // @@@MutableAssignment|use the '<-' operator to mutate the value.@@@

        /// @@@History|Get the entire list of historical states.@@@
        member this.History = states

        /// @@@Current|Get the latest state.@@@
        member this.Current = states.Head

    /// @@@InferredTypeParameter|An 'int' instance of the state tracker class. Note that the type parameter is inferred.@@@
    let tracker = StateTracker 10

    // @@@AddState|Add a state@@@
    tracker.UpdateState 17


/// @@@Interfaces-Line1|Interfaces are object types with only 'abstract' members.@@@
/// @@@Interfaces-Line2|Object types and object expressions can implement interfaces.@@@
///
/// @@@Interfaces-Line3|To learn more, see: https://docs.microsoft.com/en-us/dotnet/articles/fsharp/language-reference/interfaces@@@
module ImplementingInterfaces =

    /// @@@IDisposable|This is a type that implements IDisposable.@@@
    type ReadFile() =

        let file = new System.IO.StreamReader("readme.txt")

        member this.ReadLine() = file.ReadLine()

        // @@@IDisposableImplementation|This is the implementation of IDisposable members.@@@
        interface System.IDisposable with
            member this.Dispose() = file.Close()


    /// @@@IDisposableObjectExpression-Line1|This is an object that implements IDisposable via an Object Expression@@@
    /// @@@IDisposableObjectExpression-Line2|Unlike other languages such as C# or Java, a new type definition is not needed@@@
    /// @@@IDisposableObjectExpression-Line3|to implement an interface.@@@
    let interfaceImplementation =
        { new System.IDisposable with
            member this.Dispose() = printfn "disposed" }


/// @@@Parallel-Line1|The FSharp.Core library defines a range of parallel processing functions.  Here@@@
/// @@@Parallel-Line2|you use some functions for parallel processing over arrays.@@@
///
/// @@@Parallel-Line3|To learn more, see: https://msdn.microsoft.com/en-us/visualfsharpdocs/conceptual/array.parallel-module-%5Bfsharp%5D@@@
module ParallelArrayProgramming =

    /// @@@InputArray|First, an array of inputs.@@@
    let oneBigArray = [| 0 .. 100000 |]

    // @@@ExpensiveFunction|Next, define a functions that does some CPU intensive computation.@@@
    let rec computeSomeFunction x =
        if x <= 2 then 1
        else computeSomeFunction (x - 1) + computeSomeFunction (x - 2)

    // @@@ParallelMap|Next, do a parallel map over a large input array.@@@
    let computeResults() =
        oneBigArray
        |> Array.Parallel.map (fun x -> computeSomeFunction (x % 20))

    // @@@PrintParallel|Next, print the results.@@@
    printfn "Parallel computation results: %A" (computeResults())



/// @@@Events-Line1|Events are a common idiom for .NET programming, especially with WinForms or WPF applications.@@@
///
/// @@@Events-Line2|To learn more, see: https://docs.microsoft.com/en-us/dotnet/articles/fsharp/language-reference/members/events@@@
module Events =

    /// @@@SimpleEvent|First, create instance of Event object that consists of subscription point (event.Publish) and event trigger (event.Trigger).@@@
    let simpleEvent = new Event<int>()

    // @@@AddEventHandler1|Next, add handler to the event.@@@
    simpleEvent.Publish.Add(
        fun x -> printfn "this handler was added with Publish.Add: %d" x)

    // @@@TriggerEvent|Next, trigger the event.@@@
    simpleEvent.Trigger(5)

    // @@@EventWithArgs|Next, create an instance of Event that follows standard .NET convention: (sender, EventArgs).@@@
    let eventForDelegateType = new Event<EventHandler, EventArgs>()

    // @@@AddEventHandler2|Next, add a handler for this new event.@@@
    eventForDelegateType.Publish.AddHandler(
        EventHandler(fun _ _ -> printfn "this handler was added with Publish.AddHandler"))

    // @@@TriggerEventWithArgs|Next, trigger this event (note that sender argument should be set).@@@
    eventForDelegateType.Trigger(null, EventArgs.Empty)
