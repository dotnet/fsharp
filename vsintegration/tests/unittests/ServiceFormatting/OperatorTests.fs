// Copied from https://github.com/dungpa/fantomas and modified by Vasily Kirichenko

module FSharp.Compiler.Service.Tests.ServiceFormatting.OperatorTests

open NUnit.Framework
open FsUnit
open TestHelper

[<Test>]
let ``should format prefix operators``() =
    formatSourceString false """let x = -y
let z = !!x
    """ config
    |> should equal """let x = -y
let z = !!x
"""

[<Test>]
let ``should keep triple ~~~ operator``() =
    formatSourceString false """x ~~~FileAttributes.ReadOnly
    """ config
    |> should equal """x ~~~FileAttributes.ReadOnly
"""

[<Test>]
let ``should keep single triple ~~~ operator``() =
    formatSourceString false """~~~FileAttributes.ReadOnly
    """ config
    |> should equal """~~~FileAttributes.ReadOnly
"""

[<Test>]
let ``should keep parens around ? operator definition``() =
    formatSourceString false """let (?) f s = f s
    """ config
    |> should equal """let (?) f s = f s
"""

[<Test>]
let ``should keep parens around ?<- operator definition``() =
    formatSourceString false """let (?<-) f s = f s
    """ config
    |> should equal """let (?<-) f s = f s
"""

[<Test>]
let ``should keep parens around !+ prefix operator definition``() =
    formatSourceString false """let (!+) x = Include x
    """ config
    |> should equal """let (!+) x = Include x
"""

[<Test>]
let ``should keep parens around ++ infix operator definition``() =
    formatSourceString false """let (++) x y = { x with Includes = y :: x.Includes }
    """ config
    |> should equal """let (++) x y = { x with Includes = y :: x.Includes }
"""

[<Test>]
let ``should keep parens around inlined ==> operator definition``() =
    formatSourceString false """let inline (==>) x y = f x y
    """ config
    |> should equal """let inline (==>) x y = f x y
"""

[<Test>]
let ``should keep parens around inlined operator definition``() =
    formatSourceString false """let inline (@@) path1 path2 = Path.Combine(path1, path2)
    """ config
    |> should equal """let inline (@@) path1 path2 = Path.Combine(path1, path2)
"""

[<Test>]
let ``should pattern match on quotation expression``() =
    formatSourceString false """let rec print expr =
    match expr with
    | SpecificCall <@@ (+) @@> (_, _, exprList) ->        
        print exprList.Head
        printf " + "
        print exprList.Tail.Head
    | _ -> ()""" config
    |> should equal """let rec print expr =
    match expr with
    | SpecificCall <@@ (+) @@> (_, _, exprList) -> 
        print exprList.Head
        printf " + "
        print exprList.Tail.Head
    | _ -> ()
"""

[<Test>]
let ``should break on . operator``() =
    formatSourceString false """pattern.Replace(".", @"\.").Replace("$", @"\$").Replace("^", @"\^").Replace("{", @"\{").Replace("[", @"\[").Replace("(", @"\(").Replace(")", @"\)").Replace("+", @"\+")

    """ config
    |> prepend newline
    |> should equal """
pattern.Replace(".", @"\.").Replace("$", @"\$").Replace("^", @"\^")
       .Replace("{", @"\{").Replace("[", @"\[").Replace("(", @"\(")
       .Replace(")", @"\)").Replace("+", @"\+")
"""

// the current behavior results in a compile error since line break is before the parens and not before the .
[<Test>]
let ``should break on . operator and keep indentation``() =
    formatSourceString false """let pattern = 
    (x + y)
      .Replace(seperator + "**" + seperator, replacementSeparator + "(.|?" + replacementSeparator + ")?" )
      .Replace("**" + seperator, ".|(?<=^|" + replacementSeparator + ")" )
    """ config
    |> should equal """let pattern =
    (x + y)
        .Replace(seperator + "**" + seperator, 
                 replacementSeparator + "(.|?" + replacementSeparator + ")?")
        .Replace("**" + seperator, ".|(?<=^|" + replacementSeparator + ")")
"""

[<Test>]
let ``should keep space between ( and * in *** operator definition``() =
    formatSourceString false """let inline ( ***) l1 l2 = pair l2 l1
    """ config
    |> should equal """let inline ( *** ) l1 l2 = pair l2 l1
"""

[<Test>]
let ``should keep space between ( and * in *= operator definition``() =
    formatSourceString false """let inline ( *=) l v = update (( *) v) l
    """ config
    |> should equal """let inline ( *= ) l v = update ((*) v) l
"""

[<Test>]
let ``should not add space around ? operator``() =
    formatSourceString false """let x = y?z.d?c.[2]?d.xpto()""" config
    |> should equal """let x = y?z.d?c.[2]?d.xpto()
"""

[<Test>]
let ``should understand ? as an infix operator``() =
    formatSourceString false """try 
    item.MethodInfo.Method.Invoke(null, ipa)
    |> (fun x -> x?Invoke (true))
    |> fun (t : Task) -> t.Wait()
with _ -> ()""" config
    |> should equal """try 
    item.MethodInfo.Method.Invoke(null, ipa)
    |> (fun x -> x?Invoke (true))
    |> fun (t : Task) -> t.Wait()
with _ -> ()
"""

[<Test>]
let ``should not mess up ?<- operator``() =
    formatSourceString false """x?v <- 2""" config
    |> should equal """x?v <- 2
"""


[<Test>]
let ``should pipeline monadic bind``() =
    formatSourceString false """strToInt "1"
>>= strAddLong "A long argument that is ignored" "2"
>>= strAddLong "A long argument that is ignored" "2"
>>= strAddLong "A long argument that is ignored" "2"
>>= strAddLong "A long argument that is ignored" "2"
>>= strAddLong "A long argument that is ignored" "2"
>>= strAddLong "A long argument that is ignored" "2"
"""
        config
    |> should equal  """strToInt "1"
>>= strAddLong "A long argument that is ignored" "2"
>>= strAddLong "A long argument that is ignored" "2"
>>= strAddLong "A long argument that is ignored" "2"
>>= strAddLong "A long argument that is ignored" "2"
>>= strAddLong "A long argument that is ignored" "2"
>>= strAddLong "A long argument that is ignored" "2"
"""