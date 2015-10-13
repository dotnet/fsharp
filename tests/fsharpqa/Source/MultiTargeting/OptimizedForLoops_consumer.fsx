#if INTERACTIVE
#r "author.dll"
#else
module Foo
#endif

open System

let myLst = [ 1 .. 10]
let myArr = [| 1 .. 10|]
let mySeq = seq { for i in 1 .. 10 -> i }
let myStr = "0123456789"

Test.listFor myLst
Test.stringFor myStr
Test.arrayFor myArr
Test.seqFor mySeq

for x in myLst do Console.WriteLine(x)
for x in myArr do Console.WriteLine(x)
for x in mySeq do Console.WriteLine(x)
for x in myStr do Console.WriteLine(x)

#if INTERACTIVE
#q ;;
#endif