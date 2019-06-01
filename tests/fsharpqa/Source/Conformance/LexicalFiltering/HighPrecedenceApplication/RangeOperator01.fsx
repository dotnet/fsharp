// #Regression #Conformance #LexFilter #Precedence #ReqNOMT 
// Regression test for FSHARP1.0:4161 - Error when trying to lex/parse a range involving biggest negative number
//<Expects status="success"></Expects>

#light

{-128y..1y};;
{-128y.. 1y};;
{-128y ..1y};;
{-128y .. 1y};;

{-32768s..1s};;
{-32768s.. 1s};;
{-32768s ..1s};;
{-32768s .. 1s};;

{-2147483648..1};;
{-2147483648.. 1};;
{-2147483648 ..1};;
{-2147483648 .. 1};;

{-9223372036854775808L..1L};;
{-9223372036854775808L.. 1L};;
{-9223372036854775808L ..1L};;
{-9223372036854775808L .. 1L};;



