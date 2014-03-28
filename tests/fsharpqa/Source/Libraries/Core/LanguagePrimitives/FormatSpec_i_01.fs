// #Regression #Libraries #LanguagePrimitives 
// Regression test for FSHARP1.0:4120
// format specifier %i does not work correctly with UInt64 values

//
// This test covers the usage of %i
// All kinds of valid input are provided
//

let test = [ ((sprintf "%i" System.UInt64.MaxValue), "18446744073709551615");
             ((sprintf "%i" System.UInt64.MinValue), "0");
             ((sprintf "%i" System.UInt32.MaxValue), "4294967295");
             ((sprintf "%i" System.UInt32.MinValue), "0");
             ((sprintf "%i" System.UInt16.MaxValue), "65535");
             ((sprintf "%i" System.UInt16.MinValue), "0");
             ((sprintf "%i" System.Byte.MaxValue), "255");
             ((sprintf "%i" System.Byte.MinValue), "0");
             ((sprintf "%i" System.Int64.MaxValue), "9223372036854775807");
             ((sprintf "%i" System.Int64.MinValue), "-9223372036854775808");
             ((sprintf "%i" System.Int32.MaxValue), "2147483647");
             ((sprintf "%i" System.Int32.MinValue), "-2147483648");
             ((sprintf "%i" System.Int16.MaxValue), "32767");
             ((sprintf "%i" System.Int16.MinValue), "-32768");
             ((sprintf "%i" System.SByte.MaxValue), "127");
             ((sprintf "%i" System.SByte.MinValue), "-128");
             ((sprintf "%i" 1un), "1");
             ((sprintf "%i" -1n), "-1");
           ]

let res = List.forall (fun x -> (fst x) = (snd x)) test

(if res then 0 else 1) |> exit
