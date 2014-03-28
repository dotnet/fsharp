// #Regression #Libraries #LanguagePrimitives 
// Regression test for FSHARP1.0:4120
// format specifier %d does not work correctly with UInt64 values

//
// This test covers the usage of %d
// All kinds of valid input are provided
//

let test = [ ((sprintf "%d" System.UInt64.MaxValue), "18446744073709551615");
             ((sprintf "%d" System.UInt64.MinValue), "0");
             ((sprintf "%d" System.UInt32.MaxValue), "4294967295");
             ((sprintf "%d" System.UInt32.MinValue), "0");
             ((sprintf "%d" System.UInt16.MaxValue), "65535");
             ((sprintf "%d" System.UInt16.MinValue), "0");
             ((sprintf "%d" System.Byte.MaxValue), "255");
             ((sprintf "%d" System.Byte.MinValue), "0");
             ((sprintf "%d" System.Int64.MaxValue), "9223372036854775807");
             ((sprintf "%d" System.Int64.MinValue), "-9223372036854775808");
             ((sprintf "%d" System.Int32.MaxValue), "2147483647");
             ((sprintf "%d" System.Int32.MinValue), "-2147483648");
             ((sprintf "%d" System.Int16.MaxValue), "32767");
             ((sprintf "%d" System.Int16.MinValue), "-32768");
             ((sprintf "%d" System.SByte.MaxValue), "127");
             ((sprintf "%d" System.SByte.MinValue), "-128");
             ((sprintf "%d" 1un), "1");
             ((sprintf "%d" -1n), "-1");
           ]

let res = List.forall (fun x -> (fst x) = (snd x)) test

(if res then 0 else 1) |> exit
