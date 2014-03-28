using System;
namespace CSAttributes
{


    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public /*sealed*/ class System_Int16 : Attribute
    {
        readonly System.Int16 _p1;
        private System.Int16 _n1;

        // Positional argument (aka constructor parameters) - for required params
        public System_Int16(System.Int16 p1)
        {
            this._p1 = p1;
        }

        public System.Int16 P1
        {
            get { return _p1; }
        }

        // Named argument - for optional params
        //public System.Int16 N1 { get; set; }
        public System.Int16 N1 { get { return _n1; } set { System.Diagnostics.Debug.Assert(_p1 == value); _n1 = value; } }
    }
    /* * * * */


    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public /*sealed*/ class System_Int32 : Attribute
    {
        readonly System.Int32 _p1;
        private System.Int32 _n1;

        // Positional argument (aka constructor parameters) - for required params
        public System_Int32(System.Int32 p1)
        {
            this._p1 = p1;
        }

        public System.Int32 P1
        {
            get { return _p1; }
        }

        // Named argument - for optional params
        //public System.Int32 N1 { get; set; }
        public System.Int32 N1 { get { return _n1; } set { System.Diagnostics.Debug.Assert(_p1 == value); _n1 = value; } }
    }
    /* * * * */


    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public /*sealed*/ class System_Int64 : Attribute
    {
        readonly System.Int64 _p1;
        private System.Int64 _n1;

        // Positional argument (aka constructor parameters) - for required params
        public System_Int64(System.Int64 p1)
        {
            this._p1 = p1;
        }

        public System.Int64 P1
        {
            get { return _p1; }
        }

        // Named argument - for optional params
        //public System.Int64 N1 { get; set; }
        public System.Int64 N1 { get { return _n1; } set { System.Diagnostics.Debug.Assert(_p1 == value); _n1 = value; } }
    }
    /* * * * */


    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public /*sealed*/ class System_UInt16 : Attribute
    {
        readonly System.UInt16 _p1;
        private System.UInt16 _n1;

        // Positional argument (aka constructor parameters) - for required params
        public System_UInt16(System.UInt16 p1)
        {
            this._p1 = p1;
        }

        public System.UInt16 P1
        {
            get { return _p1; }
        }

        // Named argument - for optional params
        //public System.UInt16 N1 { get; set; }
        public System.UInt16 N1 { get { return _n1; } set { System.Diagnostics.Debug.Assert(_p1 == value); _n1 = value; } }
    }
    /* * * * */


    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public /*sealed*/ class System_UInt32 : Attribute
    {
        readonly System.UInt32 _p1;
        private System.UInt32 _n1;

        // Positional argument (aka constructor parameters) - for required params
        public System_UInt32(System.UInt32 p1)
        {
            this._p1 = p1;
        }

        public System.UInt32 P1
        {
            get { return _p1; }
        }

        // Named argument - for optional params
        //public System.UInt32 N1 { get; set; }
        public System.UInt32 N1 { get { return _n1; } set { System.Diagnostics.Debug.Assert(_p1 == value); _n1 = value; } }
    }
    /* * * * */


    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public /*sealed*/ class System_UInt64 : Attribute
    {
        readonly System.UInt64 _p1;
        private System.UInt64 _n1;

        // Positional argument (aka constructor parameters) - for required params
        public System_UInt64(System.UInt64 p1)
        {
            this._p1 = p1;
        }

        public System.UInt64 P1
        {
            get { return _p1; }
        }

        // Named argument - for optional params
        //public System.UInt64 N1 { get; set; }
        public System.UInt64 N1 { get { return _n1; } set { System.Diagnostics.Debug.Assert(_p1 == value); _n1 = value; } }
    }
    /* * * * */


    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public /*sealed*/ class System_Char : Attribute
    {
        readonly System.Char _p1;
        private System.Char _n1;

        // Positional argument (aka constructor parameters) - for required params
        public System_Char(System.Char p1)
        {
            this._p1 = p1;
        }

        public System.Char P1
        {
            get { return _p1; }
        }

        // Named argument - for optional params
        //public System.Char N1 { get; set; }
        public System.Char N1 { get { return _n1; } set { System.Diagnostics.Debug.Assert(_p1 == value); _n1 = value; } }
    }
    /* * * * */


    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public /*sealed*/ class System_Byte : Attribute
    {
        readonly System.Byte _p1;
        private System.Byte _n1;

        // Positional argument (aka constructor parameters) - for required params
        public System_Byte(System.Byte p1)
        {
            this._p1 = p1;
        }

        public System.Byte P1
        {
            get { return _p1; }
        }

        // Named argument - for optional params
        //public System.Byte N1 { get; set; }
        public System.Byte N1 { get { return _n1; } set { System.Diagnostics.Debug.Assert(_p1 == value); _n1 = value; } }
    }
    /* * * * */


    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public /*sealed*/ class System_SByte : Attribute
    {
        readonly System.SByte _p1;
        private System.SByte _n1;

        // Positional argument (aka constructor parameters) - for required params
        public System_SByte(System.SByte p1)
        {
            this._p1 = p1;
        }

        public System.SByte P1
        {
            get { return _p1; }
        }

        // Named argument - for optional params
        //public System.SByte N1 { get; set; }
        public System.SByte N1 { get { return _n1; } set { System.Diagnostics.Debug.Assert(_p1 == value); _n1 = value; } }
    }
    /* * * * */


    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public /*sealed*/ class System_Single : Attribute
    {
        readonly System.Single _p1;
        private System.Single _n1;

        // Positional argument (aka constructor parameters) - for required params
        public System_Single(System.Single p1)
        {
            this._p1 = p1;
        }

        public System.Single P1
        {
            get { return _p1; }
        }

        // Named argument - for optional params
        //public System.Single N1 { get; set; }
        public System.Single N1 { get { return _n1; } set { System.Diagnostics.Debug.Assert(_p1 == value); _n1 = value; } }
    }
    /* * * * */


    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public /*sealed*/ class System_Double : Attribute
    {
        readonly System.Double _p1;
        private System.Double _n1;

        // Positional argument (aka constructor parameters) - for required params
        public System_Double(System.Double p1)
        {
            this._p1 = p1;
        }

        public System.Double P1
        {
            get { return _p1; }
        }

        // Named argument - for optional params
        //public System.Double N1 { get; set; }
        public System.Double N1 { get { return _n1; } set { System.Diagnostics.Debug.Assert(_p1 == value); _n1 = value; } }
    }
    /* * * * */


    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public /*sealed*/ class System_String : Attribute
    {
        readonly System.String _p1;
        private System.String _n1;

        // Positional argument (aka constructor parameters) - for required params
        public System_String(System.String p1)
        {
            this._p1 = p1;
        }

        public System.String P1
        {
            get { return _p1; }
        }

        // Named argument - for optional params
        //public System.String N1 { get; set; }
        public System.String N1 { get { return _n1; } set { System.Diagnostics.Debug.Assert(_p1 == value); _n1 = value; } }
    }
    /* * * * */


    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public /*sealed*/ class System_DateTimeKind : Attribute
    {
        readonly System.DateTimeKind _p1;
        private System.DateTimeKind _n1;

        // Positional argument (aka constructor parameters) - for required params
        public System_DateTimeKind(System.DateTimeKind p1)
        {
            this._p1 = p1;
        }

        public System.DateTimeKind P1
        {
            get { return _p1; }
        }

        // Named argument - for optional params
        //public System.DateTimeKind N1 { get; set; }
        public System.DateTimeKind N1 { get { return _n1; } set { System.Diagnostics.Debug.Assert(_p1 == value); _n1 = value; } }
    }
    /* * * * */


    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public /*sealed*/ class System_Type : Attribute
    {
        readonly System.Type _p1;
        private System.Type _n1;

        // Positional argument (aka constructor parameters) - for required params
        public System_Type(System.Type p1)
        {
            this._p1 = p1;
        }

        public System.Type P1
        {
            get { return _p1; }
        }

        // Named argument - for optional params
        //public System.Type N1 { get; set; }
        public System.Type N1 { get { return _n1; } set { System.Diagnostics.Debug.Assert(_p1 == value); _n1 = value; } }
    }
    /* * * * */


    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public /*sealed*/ class System_Object : Attribute
    {
        readonly System.Object _p1;
        private System.Object _n1;

        // Positional argument (aka constructor parameters) - for required params
        public System_Object(System.Object p1)
        {
            this._p1 = p1;
        }

        public System.Object P1
        {
            get { return _p1; }
        }

        // Named argument - for optional params
        //public System.Object N1 { get; set; }
        public System.Object N1 { get { return _n1; } set { System.Diagnostics.Debug.Assert(_p1 == value); _n1 = value; } }
    }
    /* * * * */


    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public /*sealed*/ class System_TypeArray : Attribute
    {
        readonly System.Type[] _p1;
        private System.Type[] _n1;

        // Positional argument (aka constructor parameters) - for required params
        public System_TypeArray(System.Type[] p1)
        {
            this._p1 = p1;
        }

        public System.Type[] P1
        {
            get { return _p1; }
        }

        // Named argument - for optional params
        //public System.Type[] N1 { get; set; }
        public System.Type[] N1 { get { return _n1; } set { System.Diagnostics.Debug.Assert(_p1 == value); _n1 = value; } }
    }
    /* * * * */

    [System_Int16(10, N1 = 10)]
    [System_Int32(2147483647, N1 = 2147483647)]
    [System_Int64(9223372036854775807L, N1 = 9223372036854775807L)]
    [System_UInt16(65535, N1 = 65535)]
    [System_UInt32(4294967295, N1 = 4294967295)]
    [System_UInt64(18446744073709551615UL, N1 = 18446744073709551615UL)]
    [System_Char('A', N1 = 'A')]
    [System_Byte(255, N1 = 255)]
    [System_SByte(127, N1 = 127)]
    [System_Single(System.Single.MaxValue, N1 = System.Single.MaxValue)]
    [System_Double(System.Double.MaxValue, N1 = System.Double.MaxValue)]
    [System_String("hello", N1 = "hello")]
    [System_DateTimeKind(System.DateTimeKind.Local, N1 = System.DateTimeKind.Local)]
    [System_Type(typeof(System.Func<,>), N1 = typeof(System.Func<,>))]
    [System_Object(110, N1 = 110)]
    [System_TypeArray(new System.Type[] { typeof(int) }, N1 = new System.Type[] { typeof(int) })]
    public class ClassWithAttrs
    {
        [System_Int16(10, N1 = 10)]
        [System_Int32(2147483647, N1 = 2147483647)]
        [System_Int64(9223372036854775807L, N1 = 9223372036854775807L)]
        [System_UInt16(65535, N1 = 65535)]
        [System_UInt32(4294967295, N1 = 4294967295)]
        [System_UInt64(18446744073709551615UL, N1 = 18446744073709551615UL)]
        [System_Char('A', N1 = 'A')]
        [System_Byte(255, N1 = 255)]
        [System_SByte(127, N1 = 127)]
        [System_Single(System.Single.MaxValue, N1 = System.Single.MaxValue)]
        [System_Double(System.Double.MaxValue, N1 = System.Double.MaxValue)]
        [System_String("hello", N1 = "hello")]
        [System_DateTimeKind(System.DateTimeKind.Local, N1 = System.DateTimeKind.Local)]
        [System_Type(typeof(System.Func<,>), N1 = typeof(System.Func<,>))]
        [System_Object(110, N1 = 110)]
        [System_TypeArray(new System.Type[] { typeof(int) }, N1 = new System.Type[] { typeof(int) })]
        public static System.Collections.Generic.IEnumerable<System.Guid?> MethodWithAttrs()
        {
            for (int i = 0; i < 10; i++) yield return System.Guid.Empty;
        }

        [System_Int16(10, N1 = 10)]
        [System_Int32(2147483647, N1 = 2147483647)]
        [System_Int64(9223372036854775807L, N1 = 9223372036854775807L)]
        [System_UInt16(65535, N1 = 65535)]
        [System_UInt32(4294967295, N1 = 4294967295)]
        [System_UInt64(18446744073709551615UL, N1 = 18446744073709551615UL)]
        [System_Char('A', N1 = 'A')]
        [System_Byte(255, N1 = 255)]
        [System_SByte(127, N1 = 127)]
        [System_Single(System.Single.MaxValue, N1 = System.Single.MaxValue)]
        [System_Double(System.Double.MaxValue, N1 = System.Double.MaxValue)]
        [System_String("hello", N1 = "hello")]
        [System_DateTimeKind(System.DateTimeKind.Local, N1 = System.DateTimeKind.Local)]
        [System_Type(typeof(System.Func<,>), N1 = typeof(System.Func<,>))]
        [System_Object(110, N1 = 110)]
        [System_TypeArray(new System.Type[] { typeof(int) }, N1 = new System.Type[] { typeof(int) })]
        public static System.Type MethodWithAttrsThatReturnsAType()
        {
            return typeof(ClassWithAttrs);
        }
    }
}

namespace CSAttributes
{
    public class OuterClassWithNestedAttributes
    {


        [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
        public /*sealed*/ class System_Int16 : Attribute
        {
            readonly System.Int16 _p1;
            private System.Int16 _n1;

            // Positional argument (aka constructor parameters) - for required params
            public System_Int16(System.Int16 p1)
            {
                this._p1 = p1;
            }

            public System.Int16 P1
            {
                get { return _p1; }
            }

            // Named argument - for optional params
            //public System.Int16 N1 { get; set; }
            public System.Int16 N1 { get { return _n1; } set { System.Diagnostics.Debug.Assert(_p1 == value); _n1 = value; } }
        }
        /* * * * */


        [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
        public /*sealed*/ class System_Int32 : Attribute
        {
            readonly System.Int32 _p1;
            private System.Int32 _n1;

            // Positional argument (aka constructor parameters) - for required params
            public System_Int32(System.Int32 p1)
            {
                this._p1 = p1;
            }

            public System.Int32 P1
            {
                get { return _p1; }
            }

            // Named argument - for optional params
            //public System.Int32 N1 { get; set; }
            public System.Int32 N1 { get { return _n1; } set { System.Diagnostics.Debug.Assert(_p1 == value); _n1 = value; } }
        }
        /* * * * */


        [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
        public /*sealed*/ class System_Int64 : Attribute
        {
            readonly System.Int64 _p1;
            private System.Int64 _n1;

            // Positional argument (aka constructor parameters) - for required params
            public System_Int64(System.Int64 p1)
            {
                this._p1 = p1;
            }

            public System.Int64 P1
            {
                get { return _p1; }
            }

            // Named argument - for optional params
            //public System.Int64 N1 { get; set; }
            public System.Int64 N1 { get { return _n1; } set { System.Diagnostics.Debug.Assert(_p1 == value); _n1 = value; } }
        }
        /* * * * */


        [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
        public /*sealed*/ class System_UInt16 : Attribute
        {
            readonly System.UInt16 _p1;
            private System.UInt16 _n1;

            // Positional argument (aka constructor parameters) - for required params
            public System_UInt16(System.UInt16 p1)
            {
                this._p1 = p1;
            }

            public System.UInt16 P1
            {
                get { return _p1; }
            }

            // Named argument - for optional params
            //public System.UInt16 N1 { get; set; }
            public System.UInt16 N1 { get { return _n1; } set { System.Diagnostics.Debug.Assert(_p1 == value); _n1 = value; } }
        }
        /* * * * */


        [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
        public /*sealed*/ class System_UInt32 : Attribute
        {
            readonly System.UInt32 _p1;
            private System.UInt32 _n1;

            // Positional argument (aka constructor parameters) - for required params
            public System_UInt32(System.UInt32 p1)
            {
                this._p1 = p1;
            }

            public System.UInt32 P1
            {
                get { return _p1; }
            }

            // Named argument - for optional params
            //public System.UInt32 N1 { get; set; }
            public System.UInt32 N1 { get { return _n1; } set { System.Diagnostics.Debug.Assert(_p1 == value); _n1 = value; } }
        }
        /* * * * */


        [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
        public /*sealed*/ class System_UInt64 : Attribute
        {
            readonly System.UInt64 _p1;
            private System.UInt64 _n1;

            // Positional argument (aka constructor parameters) - for required params
            public System_UInt64(System.UInt64 p1)
            {
                this._p1 = p1;
            }

            public System.UInt64 P1
            {
                get { return _p1; }
            }

            // Named argument - for optional params
            //public System.UInt64 N1 { get; set; }
            public System.UInt64 N1 { get { return _n1; } set { System.Diagnostics.Debug.Assert(_p1 == value); _n1 = value; } }
        }
        /* * * * */


        [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
        public /*sealed*/ class System_Char : Attribute
        {
            readonly System.Char _p1;
            private System.Char _n1;

            // Positional argument (aka constructor parameters) - for required params
            public System_Char(System.Char p1)
            {
                this._p1 = p1;
            }

            public System.Char P1
            {
                get { return _p1; }
            }

            // Named argument - for optional params
            //public System.Char N1 { get; set; }
            public System.Char N1 { get { return _n1; } set { System.Diagnostics.Debug.Assert(_p1 == value); _n1 = value; } }
        }
        /* * * * */


        [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
        public /*sealed*/ class System_Byte : Attribute
        {
            readonly System.Byte _p1;
            private System.Byte _n1;

            // Positional argument (aka constructor parameters) - for required params
            public System_Byte(System.Byte p1)
            {
                this._p1 = p1;
            }

            public System.Byte P1
            {
                get { return _p1; }
            }

            // Named argument - for optional params
            //public System.Byte N1 { get; set; }
            public System.Byte N1 { get { return _n1; } set { System.Diagnostics.Debug.Assert(_p1 == value); _n1 = value; } }
        }
        /* * * * */


        [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
        public /*sealed*/ class System_SByte : Attribute
        {
            readonly System.SByte _p1;
            private System.SByte _n1;

            // Positional argument (aka constructor parameters) - for required params
            public System_SByte(System.SByte p1)
            {
                this._p1 = p1;
            }

            public System.SByte P1
            {
                get { return _p1; }
            }

            // Named argument - for optional params
            //public System.SByte N1 { get; set; }
            public System.SByte N1 { get { return _n1; } set { System.Diagnostics.Debug.Assert(_p1 == value); _n1 = value; } }
        }
        /* * * * */


        [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
        public /*sealed*/ class System_Single : Attribute
        {
            readonly System.Single _p1;
            private System.Single _n1;

            // Positional argument (aka constructor parameters) - for required params
            public System_Single(System.Single p1)
            {
                this._p1 = p1;
            }

            public System.Single P1
            {
                get { return _p1; }
            }

            // Named argument - for optional params
            //public System.Single N1 { get; set; }
            public System.Single N1 { get { return _n1; } set { System.Diagnostics.Debug.Assert(_p1 == value); _n1 = value; } }
        }
        /* * * * */


        [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
        public /*sealed*/ class System_Double : Attribute
        {
            readonly System.Double _p1;
            private System.Double _n1;

            // Positional argument (aka constructor parameters) - for required params
            public System_Double(System.Double p1)
            {
                this._p1 = p1;
            }

            public System.Double P1
            {
                get { return _p1; }
            }

            // Named argument - for optional params
            //public System.Double N1 { get; set; }
            public System.Double N1 { get { return _n1; } set { System.Diagnostics.Debug.Assert(_p1 == value); _n1 = value; } }
        }
        /* * * * */


        [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
        public /*sealed*/ class System_String : Attribute
        {
            readonly System.String _p1;
            private System.String _n1;

            // Positional argument (aka constructor parameters) - for required params
            public System_String(System.String p1)
            {
                this._p1 = p1;
            }

            public System.String P1
            {
                get { return _p1; }
            }

            // Named argument - for optional params
            //public System.String N1 { get; set; }
            public System.String N1 { get { return _n1; } set { System.Diagnostics.Debug.Assert(_p1 == value); _n1 = value; } }
        }
        /* * * * */


        [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
        public /*sealed*/ class System_DateTimeKind : Attribute
        {
            readonly System.DateTimeKind _p1;
            private System.DateTimeKind _n1;

            // Positional argument (aka constructor parameters) - for required params
            public System_DateTimeKind(System.DateTimeKind p1)
            {
                this._p1 = p1;
            }

            public System.DateTimeKind P1
            {
                get { return _p1; }
            }

            // Named argument - for optional params
            //public System.DateTimeKind N1 { get; set; }
            public System.DateTimeKind N1 { get { return _n1; } set { System.Diagnostics.Debug.Assert(_p1 == value); _n1 = value; } }
        }
        /* * * * */


        [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
        public /*sealed*/ class System_Type : Attribute
        {
            readonly System.Type _p1;
            private System.Type _n1;

            // Positional argument (aka constructor parameters) - for required params
            public System_Type(System.Type p1)
            {
                this._p1 = p1;
            }

            public System.Type P1
            {
                get { return _p1; }
            }

            // Named argument - for optional params
            //public System.Type N1 { get; set; }
            public System.Type N1 { get { return _n1; } set { System.Diagnostics.Debug.Assert(_p1 == value); _n1 = value; } }
        }
        /* * * * */


        [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
        public /*sealed*/ class System_Object : Attribute
        {
            readonly System.Object _p1;
            private System.Object _n1;

            // Positional argument (aka constructor parameters) - for required params
            public System_Object(System.Object p1)
            {
                this._p1 = p1;
            }

            public System.Object P1
            {
                get { return _p1; }
            }

            // Named argument - for optional params
            //public System.Object N1 { get; set; }
            public System.Object N1 { get { return _n1; } set { System.Diagnostics.Debug.Assert(_p1 == value); _n1 = value; } }
        }
        /* * * * */


        [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
        public /*sealed*/ class System_TypeArray : Attribute
        {
            readonly System.Type[] _p1;
            private System.Type[] _n1;

            // Positional argument (aka constructor parameters) - for required params
            public System_TypeArray(System.Type[] p1)
            {
                this._p1 = p1;
            }

            public System.Type[] P1
            {
                get { return _p1; }
            }

            // Named argument - for optional params
            //public System.Type[] N1 { get; set; }
            public System.Type[] N1 { get { return _n1; } set { System.Diagnostics.Debug.Assert(_p1 == value); _n1 = value; } }
        }
        /* * * * */

        [System_Int16(10, N1 = 10)]
        [System_Int32(2147483647, N1 = 2147483647)]
        [System_Int64(9223372036854775807L, N1 = 9223372036854775807L)]
        [System_UInt16(65535, N1 = 65535)]
        [System_UInt32(4294967295, N1 = 4294967295)]
        [System_UInt64(18446744073709551615UL, N1 = 18446744073709551615UL)]
        [System_Char('A', N1 = 'A')]
        [System_Byte(255, N1 = 255)]
        [System_SByte(127, N1 = 127)]
        [System_Single(System.Single.MaxValue, N1 = System.Single.MaxValue)]
        [System_Double(System.Double.MaxValue, N1 = System.Double.MaxValue)]
        [System_String("hello", N1 = "hello")]
        [System_DateTimeKind(System.DateTimeKind.Local, N1 = System.DateTimeKind.Local)]
        [System_Type(typeof(System.Func<,>), N1 = typeof(System.Func<,>))]
        [System_Object(110, N1 = 110)]
        [System_TypeArray(new System.Type[] { typeof(int) }, N1 = new System.Type[] { typeof(int) })]
        public class ClassWithAttrs
        {
            [System_Int16(10, N1 = 10)]
            [System_Int32(2147483647, N1 = 2147483647)]
            [System_Int64(9223372036854775807L, N1 = 9223372036854775807L)]
            [System_UInt16(65535, N1 = 65535)]
            [System_UInt32(4294967295, N1 = 4294967295)]
            [System_UInt64(18446744073709551615UL, N1 = 18446744073709551615UL)]
            [System_Char('A', N1 = 'A')]
            [System_Byte(255, N1 = 255)]
            [System_SByte(127, N1 = 127)]
            [System_Single(System.Single.MaxValue, N1 = System.Single.MaxValue)]
            [System_Double(System.Double.MaxValue, N1 = System.Double.MaxValue)]
            [System_String("hello", N1 = "hello")]
            [System_DateTimeKind(System.DateTimeKind.Local, N1 = System.DateTimeKind.Local)]
            [System_Type(typeof(System.Func<,>), N1 = typeof(System.Func<,>))]
            [System_Object(110, N1 = 110)]
            [System_TypeArray(new System.Type[] { typeof(int) }, N1 = new System.Type[] { typeof(int) })]
            public static System.Collections.Generic.IEnumerable<System.Guid?> MethodWithAttrs()
            {
                for (int i = 0; i < 10; i++) yield return System.Guid.Empty;
            }

            [System_Int16(10, N1 = 10)]
            [System_Int32(2147483647, N1 = 2147483647)]
            [System_Int64(9223372036854775807L, N1 = 9223372036854775807L)]
            [System_UInt16(65535, N1 = 65535)]
            [System_UInt32(4294967295, N1 = 4294967295)]
            [System_UInt64(18446744073709551615UL, N1 = 18446744073709551615UL)]
            [System_Char('A', N1 = 'A')]
            [System_Byte(255, N1 = 255)]
            [System_SByte(127, N1 = 127)]
            [System_Single(System.Single.MaxValue, N1 = System.Single.MaxValue)]
            [System_Double(System.Double.MaxValue, N1 = System.Double.MaxValue)]
            [System_String("hello", N1 = "hello")]
            [System_DateTimeKind(System.DateTimeKind.Local, N1 = System.DateTimeKind.Local)]
            [System_Type(typeof(System.Func<,>), N1 = typeof(System.Func<,>))]
            [System_Object(110, N1 = 110)]
            [System_TypeArray(new System.Type[] { typeof(int) }, N1 = new System.Type[] { typeof(int) })]
            public static System.Type MethodWithAttrsThatReturnsAType()
            {
                return typeof(ClassWithAttrs);
            }
        }
    }
}
