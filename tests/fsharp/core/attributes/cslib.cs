namespace CSharpLibrary
{


    public class IntArrayPropAttribute : System.Attribute
    {
        int[] v;
        public int[] Value { set { v = value; } get { return v; } }
    }


    public class ObjArrayPropAttribute : System.Attribute
    {
        object[] v;
        public object[] Value { set { v = value; } get { return v; } }
    }

    public class AnyPropAttribute : System.Attribute
    {
        object v;
        public object Value { set { v = value; } get { return v; } }
    }

    public class IntArrayAttribute : System.Attribute
    {
        int[] values;
        public IntArrayAttribute(int[] value) { values = value; }
        public int[] Value { get { return values; } }
    }

    public class ObjArrayAttribute : System.Attribute
    {
        object[] values;
        public ObjArrayAttribute(object[] value) { values = value; }
        public object[] Value { get { return values; } }
    }



    public class AnyAttribute : System.Attribute
    {
        object v;
        public AnyAttribute(object value) { v = value; }
        public object Value { get { return v; } }
    }

    [Any(new int[] { 42 })]
    [IntArray(new int[] { 42 })]
    [ObjArray(new object[] { 42 })]
    [IntArrayProp(Value = new int[] { 42 })]
    [ObjArrayProp(Value = new object[] { 42 })]
    [AnyProp(Value = new int[] { 42 })]
    public class TestClass { }
}
