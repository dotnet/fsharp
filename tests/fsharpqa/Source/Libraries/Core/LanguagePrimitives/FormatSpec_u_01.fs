// #Regression #Libraries #LanguagePrimitives 
// Regression test for FSHARP1.0:4120
// format specifier %u does not work correctly with UInt64 values

//
// This test covers the usage of %u
// All kinds of valid input are provided
//

let test = [ ((sprintf "%u" System.UInt64.MaxValue), "18446744073709551615");
             ((sprintf "%u" System.UInt64.MinValue), "0");
             ((sprintf "%u" System.UInt32.MaxValue), "4294967295");
             ((sprintf "%u" System.UInt32.MinValue), "0");
             ((sprintf "%u" System.UInt16.MaxValue), "65535");
             ((sprintf "%u" System.UInt16.MinValue), "0");
             ((sprintf "%u" System.Byte.MaxValue), "255");
             ((sprintf "%u" System.Byte.MinValue), "0");
             ((sprintf "%u" System.Int64.MaxValue), "9223372036854775807");
             ((sprintf "%u" System.Int64.MinValue), "9223372036854775808");
             ((sprintf "%u" System.Int32.MaxValue), "2147483647");
             ((sprintf "%u" System.Int32.MinValue), "2147483648");
             ((sprintf "%u" System.Int16.MaxValue), "32767");
             ((sprintf "%u" System.Int16.MinValue), "32768");
             ((sprintf "%u" System.SByte.MaxValue), "127");
             ((sprintf "%u" System.SByte.MinValue), "128");
             ((sprintf "%u" 1un), "1");
             ((sprintf "%u" -1n), if System.IntPtr.Size = 4 then "4294967295" else "18446744073709551615");
           ]

let res = List.forall (fun x -> (fst x) = (snd x)) test

(if res then 0 else 1) |> exit
