class P
{
    static int Main()
    {
        // Instantiate type defined in F# Portable library 
        var g = new PL.G();

        // Invoke an instance method that returns a BigInt type
        var k = g.M();

        // Do minimal runtime validation
        return (k == new System.Numerics.BigInteger(123)) ? 0 : 1;
    }
}