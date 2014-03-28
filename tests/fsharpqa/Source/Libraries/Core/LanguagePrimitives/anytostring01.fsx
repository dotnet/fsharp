// #Regression #Libraries #LanguagePrimitives #ReqNOMT 
// Regression test for FSHARP1.0:5894

//<Expects status="success">val it : string = "1"
//<Expects status="success">val it : string = "1.00001"
//<Expects status="success">val it : string = "-1.00001"
//<Expects status="success">val it : string = "1"
//<Expects status="success">val it : string = "1.00001"
//<Expects status="success">val it : string = "-1.00001"
//<Expects status="success">val it : string = "127"
//<Expects status="success">val it : string = "-128"
//<Expects status="success">val it : string = "0"
//<Expects status="success">val it : string = "-1"
//<Expects status="success">val it : string = "1"
//<Expects status="success">val it : string = "255"
//<Expects status="success">val it : string = "0"
//<Expects status="success">val it : string = "0"
//<Expects status="success">val it : string = "1"
//<Expects status="success">val it : string = "32767"
//<Expects status="success">val it : string = "-32768"
//<Expects status="success">val it : string = "0"
//<Expects status="success">val it : string = "-10"
//<Expects status="success">val it : string = "10"
//<Expects status="success">val it : string = "65535"
//<Expects status="success">val it : string = "0"
//<Expects status="success">val it : string = "0"
//<Expects status="success">val it : string = "110"
//<Expects status="success">val it : string = "2147483647"
//<Expects status="success">val it : string = "-2147483648"
//<Expects status="success">val it : string = "0"
//<Expects status="success">val it : string = "-10"
//<Expects status="success">val it : string = "10"
//<Expects status="success">val it : string = "4294967295"
//<Expects status="success">val it : string = "0"
//<Expects status="success">val it : string = "0"
//<Expects status="success">val it : string = "10"
//<Expects status="success">val it : string = "9223372036854775807"
//<Expects status="success">val it : string = "-9223372036854775808"
//<Expects status="success">val it : string = "0"
//<Expects status="success">val it : string = "-10"
//<Expects status="success">val it : string = "10"
//<Expects status="success">val it : string = "18446744073709551615"
//<Expects status="success">val it : string = "0"
//<Expects status="success">val it : string = "0"
//<Expects status="success">val it : string = "10"
//<Expects status="success">val it : string = "79228162514264337593543950335"
//<Expects status="success">val it : string = "-79228162514264337593543950335"
//<Expects status="success">val it : string = "0"
//<Expects status="success">val it : string = "12345678"
//<Expects status="success">val it : string = "-12345678"
//<Expects status="success">val it : string = "-Infinity"
//<Expects status="success">val it : string = "Infinity"
//<Expects status="success">val it : string = "NaN"
//<Expects status="success">val it : string = "-Infinity"
//<Expects status="success">val it : string = "Infinity"
//<Expects status="success">val it : string = "NaN"
//<Expects status="success">val it : string = "210f4d6b-cb42-4b09-baa1-f1aa8e59d4b0"

          (string 1.0f);;
          (string 1.00001f);;
          (string -1.00001f);; 

          (string 1.0);;
          (string  1.00001);;
          (string  -1.00001);;

          (string  System.SByte.MaxValue);;
          (string  System.SByte.MinValue);;
          (string  0y);;
          (string  -1y);;
          (string  1y);;

          (string  System.Byte.MaxValue);;
          (string  System.Byte.MinValue);;
          (string  0uy);;
          (string  1uy);;

          (string  System.Int16.MaxValue);;
          (string  System.Int16.MinValue);;
          (string  0s);;
          (string  -10s);;
          (string  10s);;

          (string  System.UInt16.MaxValue);;
          (string  System.UInt16.MinValue);;
          (string  0us);;
          (string  110us);;

          (string  System.Int32.MaxValue);;
          (string  System.Int32.MinValue);;
          (string  0);;
          (string  -10);;
          (string  10);;

          (string  System.UInt32.MaxValue);;
          (string  System.UInt32.MinValue);;
          (string  0u);;
          (string  10u);;

          (string  System.Int64.MaxValue);;
          (string  System.Int64.MinValue);;
          (string  0L);;
          (string  -10L);;
          (string  10L);;

          (string  System.UInt64.MaxValue);;
          (string  System.UInt64.MinValue);;
          (string  0UL);;
          (string  10UL);;

          (string  System.Decimal.MaxValue);;
          (string  System.Decimal.MinValue);;
          (string  System.Decimal.Zero);;
          (string  12345678M);;
          (string  -12345678M);;
          
          (string  -infinity);;
          (string  infinity);;
          (string  nan);;

          (string  -infinityf);;
          (string  infinityf);;
          (string  nanf);;

          (string (new System.Guid("210f4d6b-cb42-4b09-baa1-f1aa8e59d4b0")));;
          
#q;;

