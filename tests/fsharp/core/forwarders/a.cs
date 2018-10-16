// The test strategy here is:

//   1. take a C# DLL (a.dll)
//         - compile it in its original form (orig\a.dll)
//         - split it in two using forwarders (into split\a.dll, split\a-part1.dll)
//   2. compile a C# DLL (b.dll) against the unsplit original a.dll
//
//   3. compile an F# DLL (c.dll) against the unsplit original a.dll
//   4. From F#, reference both the _split_ DLLs and the original b.dll and the original c.dll 
//      and use a mix of types and functions from a.dll, b.dll and c.dll
//
// The aim is to shake out type identity issues associated with type forwarders.

#if PART1
public class C
{

}

public class D
{
    public class E
    {
        public class EE
        {
        }
    }


}
#endif


#if PART2
#if SPLIT
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(C)) ]

[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(D))]

// NOTE: no need to forward nested types
//[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(D.E))]
#endif

//------------- split will happen here, all types above will be forwarded to a new assembly


// note: can't forward generic types!! (what an awful restriction on the CLR feature!)
public class GenericD<T>
{
    void Generic(T x) { } 

}

public class F
{
    static public void ConsumeC(C x) { } 
    static public void ConsumeD(D x) { } 
    static public void ConsumeGenericD(GenericD<C> x) { } 
    static public void ConsumeE(D.E x) { } 
    static public void ConsumeEE(D.E.EE x) { } 

}

public class G
{
    public class H
    {
    }

}
#endif
