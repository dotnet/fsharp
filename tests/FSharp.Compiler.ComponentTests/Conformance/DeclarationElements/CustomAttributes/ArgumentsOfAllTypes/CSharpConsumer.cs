// Consumes all the attributes exposed by the F# library
// Return 0 if everything is ok.
//

using System.Linq;

namespace CSharpConsumer
{
    class Program
    {
        [FSAttributes.System_Int16(10, N1 = 10)]
        public static bool M_System_Int16()
        {
            var m = typeof(Program).GetMethods().Where(x => x.Name == "M_System_Int16").Single();
            var b = m.GetCustomAttributesData()[0].NamedArguments[0].TypedValue.Value;
            return (System.Int16)b == 10;
        }


        [FSAttributes.System_Int32(2147483647, N1 = 2147483647)]
        public static bool M_System_Int32()
        {
            var m = typeof(Program).GetMethods().Where(x => x.Name == "M_System_Int32").Single();
            var b = m.GetCustomAttributesData()[0].NamedArguments[0].TypedValue.Value;
            return (System.Int32)b == 2147483647;
        }


        [FSAttributes.System_Int64(9223372036854775807L, N1 = 9223372036854775807L)]
        public static bool M_System_Int64()
        {
            var m = typeof(Program).GetMethods().Where(x => x.Name == "M_System_Int64").Single();
            var b = m.GetCustomAttributesData()[0].NamedArguments[0].TypedValue.Value;
            return (System.Int64)b == 9223372036854775807L;
        }


        [FSAttributes.System_UInt16(65535, N1 = 65535)]
        public static bool M_System_UInt16()
        {
            var m = typeof(Program).GetMethods().Where(x => x.Name == "M_System_UInt16").Single();
            var b = m.GetCustomAttributesData()[0].NamedArguments[0].TypedValue.Value;
            return (System.UInt16)b == 65535;
        }


        [FSAttributes.System_UInt32(4294967295, N1 = 4294967295)]
        public static bool M_System_UInt32()
        {
            var m = typeof(Program).GetMethods().Where(x => x.Name == "M_System_UInt32").Single();
            var b = m.GetCustomAttributesData()[0].NamedArguments[0].TypedValue.Value;
            return (System.UInt32)b == 4294967295;
        }


        [FSAttributes.System_UInt64(18446744073709551615UL, N1 = 18446744073709551615UL)]
        public static bool M_System_UInt64()
        {
            var m = typeof(Program).GetMethods().Where(x => x.Name == "M_System_UInt64").Single();
            var b = m.GetCustomAttributesData()[0].NamedArguments[0].TypedValue.Value;
            return (System.UInt64)b == 18446744073709551615UL;
        }


        [FSAttributes.System_Char('A', N1 = 'A')]
        public static bool M_System_Char()
        {
            var m = typeof(Program).GetMethods().Where(x => x.Name == "M_System_Char").Single();
            var b = m.GetCustomAttributesData()[0].NamedArguments[0].TypedValue.Value;
            return (System.Char)b == 'A';
        }


        [FSAttributes.System_Byte(255, N1 = 255)]
        public static bool M_System_Byte()
        {
            var m = typeof(Program).GetMethods().Where(x => x.Name == "M_System_Byte").Single();
            var b = m.GetCustomAttributesData()[0].NamedArguments[0].TypedValue.Value;
            return (System.Byte)b == 255;
        }


        [FSAttributes.System_SByte(127, N1 = 127)]
        public static bool M_System_SByte()
        {
            var m = typeof(Program).GetMethods().Where(x => x.Name == "M_System_SByte").Single();
            var b = m.GetCustomAttributesData()[0].NamedArguments[0].TypedValue.Value;
            return (System.SByte)b == 127;
        }


        [FSAttributes.System_Single(System.Single.MaxValue, N1 = System.Single.MaxValue)]
        public static bool M_System_Single()
        {
            var m = typeof(Program).GetMethods().Where(x => x.Name == "M_System_Single").Single();
            var b = m.GetCustomAttributesData()[0].NamedArguments[0].TypedValue.Value;
            return (System.Single)b == System.Single.MaxValue;
        }


        [FSAttributes.System_Double(System.Double.MaxValue, N1 = System.Double.MaxValue)]
        public static bool M_System_Double()
        {
            var m = typeof(Program).GetMethods().Where(x => x.Name == "M_System_Double").Single();
            var b = m.GetCustomAttributesData()[0].NamedArguments[0].TypedValue.Value;
            return (System.Double)b == System.Double.MaxValue;
        }


        [FSAttributes.System_String("hello", N1 = "hello")]
        public static bool M_System_String()
        {
            var m = typeof(Program).GetMethods().Where(x => x.Name == "M_System_String").Single();
            var b = m.GetCustomAttributesData()[0].NamedArguments[0].TypedValue.Value;
            return (System.String)b == "hello";
        }


        [FSAttributes.System_DateTimeKind(System.DateTimeKind.Local, N1 = System.DateTimeKind.Local)]
        public static bool M_System_DateTimeKind()
        {
            var m = typeof(Program).GetMethods().Where(x => x.Name == "M_System_DateTimeKind").Single();
            var b = m.GetCustomAttributesData()[0].NamedArguments[0].TypedValue.Value;
            return (System.DateTimeKind)b == System.DateTimeKind.Local;
        }


        [FSAttributes.System_Type(typeof(System.Func<,>), N1 = typeof(System.Func<,>))]
        public static bool M_System_Type()
        {
            var m = typeof(Program).GetMethods().Where(x => x.Name == "M_System_Type").Single();
            var b = m.GetCustomAttributesData()[0].NamedArguments[0].TypedValue.Value;
            return (System.Type)b == typeof(System.Func<,>);
        }


        [FSAttributes.System_Object(110, N1 = 110)]
        public static bool M_System_Object()
        {
            var m = typeof(Program).GetMethods().Where(x => x.Name == "M_System_Object").Single();
            var b = m.GetCustomAttributesData()[0].NamedArguments[0].TypedValue.Value;
            return (int)b == 110;
        }


        [FSAttributes.System_TypeArray(new System.Type[] { typeof(int) }, N1 = new System.Type[] { typeof(int) })]
        public static bool M_System_TypeArray()
        {
            var m = typeof(Program).GetMethods().Where(x => x.Name == "M_System_TypeArray").Single();
            var b = m.GetCustomAttributesData()[0].NamedArguments[0].TypedValue.Value as System.Collections.ObjectModel.ReadOnlyCollection<System.Reflection.CustomAttributeTypedArgument>;
            return b[0].Value == typeof(int);
        }


        static int Main()
        {

            if (!M_System_Int16()) return 1;
            if (!M_System_Int32()) return 1;
            if (!M_System_Int64()) return 1;
            if (!M_System_UInt16()) return 1;
            if (!M_System_UInt32()) return 1;
            if (!M_System_UInt64()) return 1;
            if (!M_System_Char()) return 1;
            if (!M_System_Byte()) return 1;
            if (!M_System_SByte()) return 1;
            if (!M_System_Single()) return 1;
            if (!M_System_Double()) return 1;
            if (!M_System_String()) return 1;
            if (!M_System_DateTimeKind()) return 1;
            if (!M_System_Type()) return 1;
            if (!M_System_Object()) return 1;
            if (!M_System_TypeArray()) return 1;

            return 0;
        }
    }
}
