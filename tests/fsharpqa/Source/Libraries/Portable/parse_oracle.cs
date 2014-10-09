

class Program
{
    /// <summary>
    /// Compare the value returned by F# with the value returned by System.Numerics.BigInteger.Parse
    /// It expects the 2 values to be identical.
    /// </summary>
    /// <param name="bi_as_string">The string that represent the number to be parsed</param>
    static void AssertEqual(string bi_as_string)
    {
        if(System.Numerics.BigInteger.Compare( FSLib.BigIntTest.T(bi_as_string), System.Numerics.BigInteger.Parse(bi_as_string)) != 0)
        {
            throw new System.Exception(bi_as_string + " did not parse as " + System.Numerics.BigInteger.Parse(bi_as_string));
        }
    }

    /// <summary>
    /// Compare the value returned by F# with the passed value 'bi'
    /// It expects the 2 values to be identical.
    /// </summary>
    /// <param name="bi_as_string">The string that represent the number to be parsed</param>
    /// <param name="bi">BigInteger value (expected)</param>
    static void AssertEqual(string bi_as_string, System.Numerics.BigInteger bi)
    {
        if (System.Numerics.BigInteger.Compare(FSLib.BigIntTest.T(bi_as_string), bi) != 0)
        {
            throw new System.Exception(bi_as_string + " did not parse as " + bi.ToString());
        }
    }

    /// <summary>
    /// Compare the value returned by F# with the value returned by System.Numerics.BigInteger.Parse
    /// It expects both the call to F# and directly to System.Numerics.BigInteger.Parse to throw.
    /// </summary>
    /// <param name="bi_as_string">The string that represent the number to be parsed</param>
    static void ThrowException(string bi_as_string)
    {
        bool f_threw;
        bool c_threw;

        try
        {
            FSLib.BigIntTest.T(bi_as_string);
            f_threw = false;
        }
        catch
        {
            f_threw = true;
        }

        try
        {
            System.Numerics.BigInteger.Parse(bi_as_string);
            c_threw = false;
        }
        catch
        {
            c_threw = true;
        }

        
        if(f_threw != c_threw )
        {
            throw new System.Exception(bi_as_string + ": F# threw when C# did not (or vice-versa)");
        }
    }

    

    static void Main(string[] args)
    {
        // Some "constants"
        var onetwothee = new System.Numerics.BigInteger(123);

        // No leading/trailing spaces (basic cases / one and two digits)
        AssertEqual("1");
        AssertEqual("-1");
        AssertEqual("0");
        AssertEqual("12");
        AssertEqual("-12");
        AssertEqual("00");
        AssertEqual("-00");
        AssertEqual("+00");

#if PORTABLE
        // With leading spaces
        AssertEqual(" 123");
        AssertEqual("  123");
        AssertEqual("                                                                                                              123");

        // With trailing spaces
        AssertEqual("123 ");
        AssertEqual("123  ");
        AssertEqual("123                                                                                                              ");

        // With both trailing and leading spaces
        AssertEqual(" 123 ");
        AssertEqual("  123  ");
        AssertEqual("                                                                                                              123                                                                                                              ");


        // Misc whitespaces
        var ws = "\x2000\x2001\x2002\x2003\x2004\x2005\x2006\x2007\x2008\x2009\x200A\x202F\x205F\x3000\x2028\x2029\x0009\x000A\x000B\x000C\x000D\x0085\x00A0";
        AssertEqual(ws + "123" + ws, onetwothee);

#endif

        // Optional sign
        AssertEqual("+123");
        AssertEqual("-123");

        // Leading zeros are ignored
        AssertEqual("0123");
        AssertEqual("00123");
        AssertEqual("000000000000000000000000000000000000000000000123");
        AssertEqual("-0123");
        AssertEqual("-00123");
        AssertEqual("-000000000000000000000000000000000000000000000123");
        AssertEqual("+0123");
        AssertEqual("+00123");
        AssertEqual("+000000000000000000000000000000000000000000000123");

        // Really big numbers: System.Int64.MaxValue, System.UInt64.MaxValue, System.Int64.MinValue
        AssertEqual("9223372036854775807", new System.Numerics.BigInteger(9223372036854775807L));
        AssertEqual("-9223372036854775808", new System.Numerics.BigInteger(-9223372036854775808L));
        AssertEqual("18446744073709551615", new System.Numerics.BigInteger(18446744073709551615UL));

        // Really big numbers: System.Int64.MaxValue+1, System.UInt64.MaxValue+1, System.Int64.MinValue-1
        AssertEqual("9223372036854775808");
        AssertEqual("-9223372036854775809");
        AssertEqual("18446744073709551616");


        // Rejected
        ThrowException("0 123");
        ThrowException("00 123");
        ThrowException("000000000000000000000000000000000000000000000  123");
        ThrowException("- 0123");
        ThrowException("- 00123");
        ThrowException("- 000000000000000000000000000000000000000000000123");
        ThrowException("+ 0123");
        ThrowException("+ 00123");
        ThrowException("+ 000000000000000000000000000000000000000000000123");
        ThrowException("1.3");
        ThrowException("123AA");
        ThrowException("AA123");
        ThrowException("+A");
        ThrowException("-A");
        ThrowException("1-");
        ThrowException("1+");
        ThrowException("");
        ThrowException(" ");
        ThrowException("+");
        ThrowException("-");
        ThrowException(".");
        ThrowException("++");
        ThrowException("--");
        ThrowException("-+");
        ThrowException("+-");

        // These are currently broken on PORTABLE
#if PORTABLE
        ThrowException("0x20");
        ThrowException("-0x20");
#else
        AssertEqual("0x7B", onetwothee);
        ThrowException("-0x7b");    // I suppose 0x values and negatives don't mix?
        ThrowException("0x-7b");    // I suppose 0x values and negatives don't mix? 
#endif
    }
}

