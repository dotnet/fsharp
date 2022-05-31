// Generator for the test in the parent folder

//module OnePositionalStringOneNamedInt =
//    [<CSAttributes.A1("X", pa_int = 1)>]
//    type T1() = class end
//    let t1 = (typeof<T1>.GetCustomAttributes(false) |> Array.map (fun x -> x :?> System.Attribute)).[0]
//    t1  
//    // val it: System.Attribute =
//    //  CSAttributes.A1Attribute {PositionalString = "X";
//    //                            TypeId = CSAttributes.A1Attribute;
//    //                            pa_int = 1;}  
//
//module OnePositionalAndByNameStringOneNamedInt =
//    [<CSAttributes.A1(pa_string = "A", pa_int = 2)>]
//    type T1() = class end
//    let t1 = (typeof<T1>.GetCustomAttributes(false) |> Array.map (fun x -> x :?> System.Attribute)).[0]
//    t1  
//    // val it: System.Attribute =
//    //  CSAttributes.A1Attribute {PositionalString = "A";
//    //                            TypeId = CSAttributes.A1Attribute;
//    //                            pa_int = 2;}  

let attr_template = """
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public /*sealed*/ class {1} : Attribute
    {{
        readonly {0} _p1;
        private {0} _n1;

        // Positional argument (aka constructor parameters) - for required params
        public {1}({0} p1)
        {{
            this._p1 = p1;
        }}

        public {0} P1
        {{
            get {{ return _p1; }}
        }}

        // Named argument - for optional params
        //public {0} N1 {{ get; set; }}
        public {0} N1 {{ get {{ return _n1; }} set {{ System.Diagnostics.Debug.Assert(_p1 == value); _n1 = value; }} }}
    }}
    /* * * * */
    """
        
let attr_template_fsharp = """
[<AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)>]
type {1}(p1 : {0}) =
    [<DefaultValue>]
    val mutable _n1 : {0}

    inherit Attribute()
    member this._p1 = p1

    member this.N1 with get() = this._n1
                   and set(v) = System.Diagnostics.Debug.Assert(this._p1 = v)
                                this._n1 <- v
    (* * * * *)
"""

let types = [| 
               ("System.Int16",           "System_Int16", "10s", "10"); 
               ("System.Int32",           "System_Int32", "2147483647", "2147483647"); 
               ("System.Int64",           "System_Int64", "9223372036854775807L", "9223372036854775807L"); 
               ("System.UInt16",          "System_UInt16", "65535us", "65535"); 
               ("System.UInt32",          "System_UInt32", "4294967295u", "4294967295"); 
               ("System.UInt64",          "System_UInt64", "18446744073709551615UL", "18446744073709551615UL"); 
               ("System.Char",            "System_Char", "'A'", "'A'");
               ("System.Byte",            "System_Byte", "255uy", "255"); 
               ("System.SByte",           "System_SByte", "127y", "127");
               ("System.Single",          "System_Single", "System.Single.MaxValue", "System.Single.MaxValue");
               ("System.Double",          "System_Double", "System.Double.MaxValue", "System.Double.MaxValue");
               ("System.String",          "System_String", "\"hello\"", "\"hello\"");
               ("System.DateTimeKind",    "System_DateTimeKind", "System.DateTimeKind.Local", "System.DateTimeKind.Local");

               //("System.Action",          "System_Action", "new System.Action(fun _ -> ())", "new System.Action(() => ()));");
               //("System.Action<int>",     "System_ActionOfInt", "new System.Action(fun i -> ())", "new System.Action((i) => ()))");

               // TODO: Verify this are properly illegal in F# as well
               //("System.Guid",            "System_Guid", "System.Guid.Empty", "System.Guid.Empty");   -- illegal C#
               //("System.Decimal",         "System_Decimal", "79228162514264337593543950335M", "79228162514264337593543950335M");   -- illegal C#
               //("System.IntPtr",          "System_IntPtr", "0n", "System.IntPtr.Zero");    -- illegal C#
               //("System.UIntPtr",         "System_UIntPtr", "0un", "System.UIntPtr.Zero");   -- illegal C#
               //("System.Void",            "System_Void");         -- invalid in C#

               ("System.Type",            "System_Type", "typedefof<System.Func<_,_>>", "typeof(System.Func<,>)"); // TODO: how do I pass a type to an F# attribute??
               ("System.Object",          "System_Object", "110", "110");
               ("System.Type[]",          "System_TypeArray", "[| typeof<int> |]", "new System.Type [] { typeof(int) }")
               //("System.Type[,]",         "System_TypeArray2");
               //("System.ICloneable",      "System_ICloneable");
               |]

let csharpcode =
    printfn """
using System;
namespace CSAttributes
{
"""
    types |> Array.map (fun (ty,name,_,_) -> System.String.Format(attr_template, ty, name))
          |> Array.iter (fun s -> printfn "%s" s)

    types |> Array.iter (fun (ty,name,_,csvalue) -> printfn "    [%s(%s, N1 = %s)]" name csvalue csvalue)

    printfn "    public class ClassWithAttrs {"

    types |> Array.iter (fun (ty,name,_,csvalue) -> printfn "        [%s(%s, N1 = %s)]" name csvalue csvalue)
          
    printfn """        public static System.Collections.Generic.IEnumerable<System.Guid?> MethodWithAttrs() {
            for(int i = 0; i< 10; i++) yield return System.Guid.Empty;
        }
"""

    types |> Array.iter (fun (ty,name,_,csvalue) -> printfn "        [%s(%s, N1 = %s)]" name csvalue csvalue)
          
    printfn """        public static System.Type MethodWithAttrsThatReturnsAType() {
            return typeof(ClassWithAttrs);
        }
    }
}"""

    // Now same thing, with nested classes
    printfn """
namespace CSAttributes
{
    public class OuterClassWithNestedAttributes
    {
"""
    types |> Array.map (fun (ty,name,_,_) -> System.String.Format(attr_template, ty, name))
          |> Array.iter (fun s -> printfn "%s" s)

    types |> Array.iter (fun (ty,name,_,csvalue) -> printfn "    [%s(%s, N1 = %s)]" name csvalue csvalue)

    printfn "    public class ClassWithAttrs {"

    types |> Array.iter (fun (ty,name,_,csvalue) -> printfn "        [%s(%s, N1 = %s)]" name csvalue csvalue)
          
    printfn """        public static System.Collections.Generic.IEnumerable<System.Guid?> MethodWithAttrs() {
            for(int i = 0; i< 10; i++) yield return System.Guid.Empty;
        }
"""

    types |> Array.iter (fun (ty,name,_,csvalue) -> printfn "        [%s(%s, N1 = %s)]" name csvalue csvalue)
          
    printfn """        public static System.Type MethodWithAttrsThatReturnsAType() {
            return typeof(ClassWithAttrs);
        }
    }
    }
}"""


let fsharpcodeattr =
    printfn """
namespace FSAttributes

open System

"""
    types |> Array.map (fun (ty,name,_,_) -> System.String.Format(attr_template_fsharp, ty, name))
          |> Array.iter (fun s -> printfn "%s" s)

    types |> Array.iter (fun (ty,name,fsvalue,_) -> printfn "[<%s(%s, N1 = %s)>]" name fsvalue fsvalue)

    printfn "type ClassWithAttrs() = "

    types |> Array.iter (fun (ty,name,fsvalue,_) -> printfn "    [<%s(%s, N1 = %s)>]" name fsvalue fsvalue)
          
    printfn """    static member MethodWithAttrs() = seq {for i in [1 .. 10] -> System.Guid.Empty}"""

    types |> Array.iter (fun (ty,name,fsvalue,_) -> printfn "    [<%s(%s, N1 = %s)>]" name fsvalue fsvalue)
          
    printfn """    static member MethodWithAttrsThatReturnsAType() = typeof<ClassWithAttrs>"""

//    // Now same thing, with nested classes
//    printfn """
//namespace CSAttributes
//{
//    public class OuterClassWithNestedAttributes
//    {
//"""
//    types |> Array.map (fun (ty,name,_,_) -> System.String.Format(attr_template, ty, name))
//          |> Array.iter (fun s -> printfn "%s" s)
//
//    types |> Array.iter (fun (ty,name,_,csvalue) -> printfn "    [%s(%s, N1 = %s)]" name csvalue csvalue)
//
//    printfn "    public class ClassWithAttrs {"
//
//    types |> Array.iter (fun (ty,name,_,csvalue) -> printfn "        [%s(%s, N1 = %s)]" name csvalue csvalue)
//          
//    printfn """        public static System.Collections.Generic.IEnumerable<System.Guid?> MethodWithAttrs() {
//            for(int i = 0; i< 10; i++) yield return System.Guid.Empty;
//        }
//"""
//
//    types |> Array.iter (fun (ty,name,_,csvalue) -> printfn "        [%s(%s, N1 = %s)]" name csvalue csvalue)
//          
//    printfn """        public static System.Type MethodWithAttrsThatReturnsAType() {
//            return typeof(ClassWithAttrs);
//        }
//    }
//    }
//}"""

let csharpconsumercode = 

    use f = new System.IO.StreamWriter("CSharpConsumer.cs", false)

    let csharpconsumercodetemplate = """
        [FSAttributes.{0}({1}, N1 = {1})]
        public static bool M_{0}()
        {{
            var m = typeof(Program).GetMethods().Where(x => x.Name == "M_{0}").Single();
            var b = m.GetCustomAttributesData()[0].NamedArguments[0].TypedValue.Value;
            return ({2})b == {1};
        }}
"""
    fprintfn f """
using System.Linq;
namespace CSharpConsumer
{
    class Program
    {
"""
    types |> Array.iter (fun (ty,name,_,csvalue) -> let csharpmeth = System.String.Format(csharpconsumercodetemplate, name, csvalue, ty)
                                                    fprintfn f "%s" csharpmeth)

    fprintfn f """
        static int Main()
        {
"""
    types |> Array.iter (fun (ty,name,_,csvalue) -> fprintfn f "if(!M_%s()) return 1;" name)

    fprintfn f """
            return 0;
    }
    }
}"""


let fsharpcode =
    let fstemplate = """
module T_{1} =
    [<CSAttributes.{1}({2}, N1 = {2})>]
    type T1() = class end
    let t1 = (typeof<T1>.GetCustomAttributes(false) |> Array.map (fun x -> x :?> System.Attribute)).[0]
    printfn "%A" t1

    [<CSAttributes.OuterClassWithNestedAttributes.{1}({2}, N1 = {2})>]
    type T2() = class end
    let t2 = (typeof<T1>.GetCustomAttributes(false) |> Array.map (fun x -> x :?> System.Attribute)).[0]
    printfn "%A" t2

    // Create a new attribute that inherits from the C# one...
    type A1_{1}(typeofResult : System.Type, typedefofResult : System.Type) =
        inherit CSAttributes.{1}({2})
        member this.TypeofResult    = typeofResult
        member this.TypedefofResult = typedefofResult

#if WITHQUERY
    let q1 = query {{ for x in CSAttributes.ClassWithAttrs.MethodWithAttrs().AsQueryable() do
                     where (x.HasValue && CSAttributes.ClassWithAttrs.MethodWithAttrsThatReturnsAType() <> typeof<CSAttributes.ClassWithAttrs>)
                     select (x,CSAttributes.ClassWithAttrs.MethodWithAttrsThatReturnsAType()) }}
    q1.Distinct() |> Seq.iter (fun x -> printfn "%A" x)

    let q2 = query {{ for x in CSAttributes.OuterClassWithNestedAttributes.ClassWithAttrs.MethodWithAttrs().AsQueryable() do
                     where (x.HasValue && CSAttributes.OuterClassWithNestedAttributes.ClassWithAttrs.MethodWithAttrsThatReturnsAType() <> typeof<CSAttributes.OuterClassWithNestedAttributes.ClassWithAttrs>)
                     select (x,CSAttributes.OuterClassWithNestedAttributes.ClassWithAttrs.MethodWithAttrsThatReturnsAType()) }}
    q2.Distinct() |> Seq.iter (fun x -> printfn "%A" x)

#endif
    (* * * * *)
"""

    let fsconsumer = """
module C_{1} =
    // Consume the attribute from the F# assembly
    [<{1}.T_{1}.A1_{1}(typeof<list<int>>, typedefof<list<int>>)>]
    type T() = member __.M() = true
    
    let t = new T()
    exit <| if t.M() then 0 else 1
"""


    types |> Array.iter (fun (ty,name,fsvalue,_) -> let tc = System.String.Format(fstemplate, ty, name, fsvalue)
                                                    use f = new System.IO.StreamWriter(sprintf "%s.fsx" name)
                                                    fprintfn f "%s" tc)

    types |> Array.iter (fun (ty,name,fsvalue,_) -> let tc = System.String.Format(fsconsumer, ty, name, fsvalue)
                                                    use f = new System.IO.StreamWriter(sprintf "%s_Consumer.fsx" name)
                                                    fprintfn f "%s" tc)
    use f = new System.IO.StreamWriter("env.lst", true)
    let compilecslib = sprintf "NoMT\tSOURCE=dummy.fs PRECMD=\"\$CSC_PIPE /target:library CSLibraryWithAttributes.cs\"\t# CSLibraryWithAttributes.cs"
    fprintfn f "%s" compilecslib
    f.Close()

    use g = new System.IO.StreamWriter("dummy.fs", true)
    g.Close()

    types |> Array.iter (fun (ty,name,fsvalue,_) -> let tc1 = sprintf "NoMT\tSOURCE=%s.fsx          SCFLAGS=\"-a -r CSLibraryWithAttributes.dll\"\t# %s.fsx" name name
                                                    let tc2 = sprintf "NoMT\tSOURCE=%s_Consumer.fsx SCFLAGS=\"   -r %s.dll\"\t# %s_Consumer.fsx" name name name
                                                    use f = new System.IO.StreamWriter("env.lst", true)
                                                    fprintfn f "%s" tc1
                                                    fprintfn f "%s" tc2
                                                    )
