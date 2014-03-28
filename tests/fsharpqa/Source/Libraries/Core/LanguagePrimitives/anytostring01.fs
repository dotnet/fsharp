// #Regression #Libraries #LanguagePrimitives #ReqNOMT 
// Regression test for FSHARP1.0:5894
//<Expects status="success"></Expects>

let v = [ 
          (string 1.0f);
          (string 1.00001f);
          (string -1.00001f); 

          (string 1.0);
          (string  1.00001);
          (string  -1.00001);

          (string  System.SByte.MaxValue);
          (string  System.SByte.MinValue);
          (string  0y);
          (string  -1y);
          (string  1y);

          (string  System.Byte.MaxValue);
          (string  System.Byte.MinValue);
          (string  0uy);
          (string  1uy);

          (string  System.Int16.MaxValue);
          (string  System.Int16.MinValue);
          (string  0s);
          (string  -10s);
          (string  10s);

          (string  System.UInt16.MaxValue);
          (string  System.UInt16.MinValue);
          (string  0us);
          (string  110us);

          (string  System.Int32.MaxValue);
          (string  System.Int32.MinValue);
          (string  0);
          (string  -10);
          (string  10);

          (string  System.UInt32.MaxValue);
          (string  System.UInt32.MinValue);
          (string  0u);
          (string  10u);

          (string  System.Int64.MaxValue);
          (string  System.Int64.MinValue);
          (string  0L);
          (string  -10L);
          (string  10L);

          (string  System.UInt64.MaxValue);
          (string  System.UInt64.MinValue);
          (string  0UL);
          (string  10UL);

          (string  System.Decimal.MaxValue);
          (string  System.Decimal.MinValue);
          (string  System.Decimal.Zero);
          (string  12345678M);
          (string  -12345678M);
          
          (string  -infinity);
          (string  infinity);
          (string  nan);

          (string  -infinityf);
          (string  infinityf);
          (string  nanf);

          (string (new System.Guid("210f4d6b-cb42-4b09-baa1-f1aa8e59d4b0")))
          
          ]
          
let expected = ["1"; "1.00001"; "-1.00001"; "1"; "1.00001"; "-1.00001"; "127"; "-128"; "0";
               "-1"; "1"; "255"; "0"; "0"; "1"; "32767"; "-32768"; "0"; "-10"; "10";
               "65535"; "0"; "0"; "110"; "2147483647"; "-2147483648"; "0"; "-10"; "10";
               "4294967295"; "0"; "0"; "10"; "9223372036854775807"; "-9223372036854775808";
               "0"; "-10"; "10"; "18446744073709551615"; "0"; "0"; "10";
               "79228162514264337593543950335"; "-79228162514264337593543950335"; "0";
               "12345678"; "-12345678"; "-Infinity"; "Infinity"; "NaN"; "-Infinity";
               "Infinity"; "NaN";
               "210f4d6b-cb42-4b09-baa1-f1aa8e59d4b0"]          

exit <| if v = expected then 0 else 1

