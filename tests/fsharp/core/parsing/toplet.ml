// #Conformance 
let rec loopF n x = function 0 -> x | i -> loopF n (x+1) (i-1) in
let rec loopE n x = function 0 -> x | i -> loopE n (loopF n x n) (i-1) in
let rec loopD n x = function 0 -> x | i -> loopD n (loopE n x n) (i-1) in
let rec loopC n x = function 0 -> x | i -> loopC n (loopD n x n) (i-1) in
let rec loopB n x = function 0 -> x | i -> loopB n (loopC n x n) (i-1) in
let rec loopA n x = function 0 -> x | i -> loopA n (loopB n x n) (i-1) in  
let n =
  try 
    System.Int32.Parse(System.Environment.GetCommandLineArgs().[1])
  with 
    _ -> 1
  in
Printf.printf "%d\n" (loopA n 0 n)

