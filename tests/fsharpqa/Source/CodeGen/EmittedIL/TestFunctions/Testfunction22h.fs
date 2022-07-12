// #NoMono #NoMT #CodeGen #EmittedIL   
#light
let test1() =
   try 
      System.Console.WriteLine() 
   with
      // should not generate a filter
      | _ -> System.Console.WriteLine()

let test2() =
   try 
      System.Console.WriteLine() 
   with
      // should generate a filter
      | :? System.ArgumentException -> System.Console.WriteLine()

let test3() =
   try 
      System.Console.WriteLine() 
   with
      // should generate a filter
      | :? System.ArgumentException as a -> System.Console.WriteLine(a.Message)

let test4() =
   try 
      System.Console.WriteLine() 
   with
      // should generate a filter
      | MatchFailureException ( msg, _, _) -> System.Console.WriteLine(msg)

let test5() =
   try 
      System.Console.WriteLine() 
   with
      // should generate a filter
      | :? System.ArgumentException as a -> System.Console.WriteLine(a.Message)
      | MatchFailureException ( msg, _, _) -> System.Console.WriteLine(msg)
