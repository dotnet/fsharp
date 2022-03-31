// #Regression #NoMT #CodeGen #Interop 
// Regression test for FSHARP1.0:6150
// nativeptr<T> should not turn into IntPtr when used in method signature or return value
// it should be T*
namespace N

module M =
 
 open CodeGenHelper

 type T() = member x.F ( p : nativeptr<char> ) = ()

 type R() = member x.G ( ) : nativeptr<char>  = Unchecked.defaultof<nativeptr<char>>
 
 let res1 = System.Reflection.Assembly.GetExecutingAssembly() 
            |> getType "N.M+T" 
            |> getMember "F"

 let res2 = System.Reflection.Assembly.GetExecutingAssembly() 
            |> getType "N.M+R" 
            |> getMember "G"
           
 // Used to be (incorrectly) "Void F(IntPtr)"
 if res1.ToString() <> "Void F(Char*)" then exit 1

 // Used to be (incorrectly) "IntPtr G()"
 if res2.ToString() <> "Char* G()" then exit 1
