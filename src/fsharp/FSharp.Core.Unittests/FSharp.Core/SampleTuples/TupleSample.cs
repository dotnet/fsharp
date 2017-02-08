using System;

namespace TupleSample
{
    public class TupleReturns
    {
        // Basic Tuple
        public static (int, int) GetTuple(int one, int two)
        {
            return (one, two);
        }
        // Basic Tuple with three elements
        public static (int, int, int) GetTuple(int one, int two, int three)
        {
            return (one, two, three);
        }
        // Basic Tuple with four elements
        public static (int, int, int, int) GetTuple(int one, int two, int three, int four)
        {
            return (one, two, three, four);
        }
        // Basic Tuple with five elements
        public static (int, int, int, int, int) GetTuple(int one, int two, int three, int four, int five)
        {
            return (one, two, three, four, five);
        }
        // Basic Tuple with six elements
        public static (int, int, int, int, int, int) GetTuple(int one, int two, int three, int four, int five, int six)
        {
            return (one, two, three, four, five, six);
        }
        // Basic Tuple with seven elements
        public static (int, int, int, int, int, int, int) GetTuple(int one, int two, int three, int four, int five, int six, int seven)
        {
            return (one, two, three, four, five, six, seven);
        }
        // Basic Tuple with eight elements 7tuple+1tuple via .Rest
        public static (int, int, int, int, int, int, int, int) GetTuple(int one, int two, int three, int four, int five, int six, int seven, int eight)
        {
            return (one, two, three, four, five, six, seven, eight);
        }
        // Basic Tuple with nine elements 7tuple+2ttuple via .Rest
        public static (int, int, int, int, int, int, int, int, int) GetTuple(int one, int two, int three, int four, int five, int six, int seven, int eight, int nine)
        {
            return (one, two, three, four, five, six, seven, eight, nine);
        }
        // Basic Tuple with ten elements  7tuple+3ttuple via .Rest
        public static (int, int, int, int, int, int, int, int, int, int) GetTuple(int one, int two, int three, int four, int five, int six, int seven, int eight, int nine, int ten)
        {
            return (one, two, three, four, five, six, seven, eight, nine, ten);
        }
        // Basic Tuple with fifteen elements  7tuple+7ttuple+1tuple via .Rest
        public static (int, int, int, int, int, int, int, int, int, int, int, int, int, int, int) GetTuple(int one, int two, int three, int four, int five, int six, int seven, int eight, int nine, int ten, int eleven, int twelve, int thirteen, int fourteen, int fifteen)
        {
            return (one, two, three, four, five, six, seven, eight, nine, ten, eleven, twelve, thirteen, fourteen, fifteen);
        }
        // Basic Tuple with sixteen elements  7tuple+7ttuple+2tuple via .Rest
        public static (int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int) GetTuple(int one, int two, int three, int four, int five, int six, int seven, int eight, int nine, int ten, int eleven, int twelve, int thirteen, int fourteen, int fifteen, int sixteen)
        {
            return (one, two, three, four, five, six, seven, eight, nine, ten, eleven, twelve, thirteen, fourteen, fifteen, sixteen);
        }
        // Tuple with various field types
        public static (int, float, double, string, int[], object) GetTuple(int one, float two, double three, string four, int[] five, object six)
        {
            return (one, two, three, four, five, six);
        }
    }
    public class TupleArguments
    {
        // Basic Tuple
        public static (int, int) GetTuple( (int, int) t)
        {
            return (t.Item1, t.Item2);
        }
        // Basic Tuple with three elements
        public static (int, int, int) GetTuple( (int, int , int) t)
        {
            return (t.Item1, t.Item2, t.Item3);
        }
        // Basic Tuple with four elements
        public static (int, int, int, int) GetTuple( (int, int, int , int) t)
        {
            return (t.Item1, t.Item2, t.Item3, t.Item4);
        }
        // Basic Tuple with five elements
        public static (int, int, int, int, int) GetTuple( (int, int, int, int, int) t)
        {
            return (t.Item1, t.Item2, t.Item3, t.Item4, t.Item5);
        }
        // Basic Tuple with six elements
        public static (int, int, int, int, int, int) GetTuple( (int, int, int, int, int, int) t)
        {
            return (t.Item1, t.Item2, t.Item3, t.Item4, t.Item5, t.Item6);
        }
        // Basic Tuple with seven elements
        public static (int, int, int, int, int, int, int) GetTuple((int, int, int, int, int, int, int) t)
        {
            return (t.Item1, t.Item2, t.Item3, t.Item4, t.Item5, t.Item6, t.Item7);
        }
        // Basic Tuple with eight elements 7tuple+1tuple via .Rest
        public static (int, int, int, int, int, int, int, int) GetTuple((int, int, int, int, int, int, int, int) t)
        {
            return (t.Item1, t.Item2, t.Item3, t.Item4, t.Item5, t.Item6, t.Item7, t.Item8);
        }
        // Basic Tuple with nine elements 7tuple+2ttuple via .Rest
        public static (int, int, int, int, int, int, int, int, int) GetTuple((int, int, int, int, int, int, int, int, int) t)
        {
            return (t.Item1, t.Item2, t.Item3, t.Item4, t.Item5, t.Item6, t.Item7, t.Item8, t.Item9);
        }
        // Basic Tuple with ten elements  7tuple+3ttuple via .Rest
        public static (int, int, int, int, int, int, int, int, int, int) GetTuple((int, int, int, int, int, int, int, int, int, int) t)
        {
            return (t.Item1, t.Item2, t.Item3, t.Item4, t.Item5, t.Item6, t.Item7, t.Item8, t.Item9, t.Item10);
        }
        // Basic Tuple with fifteen elements  7tuple+7ttuple+1tuple via .Rest
        public static (int, int, int, int, int, int, int, int, int, int, int, int, int, int, int) GetTuple((int, int, int, int, int, int, int, int, int, int, int, int, int, int, int) t)
        {
            return (t.Item1, t.Item2, t.Item3, t.Item4, t.Item5, t.Item6, t.Item7, t.Item8, t.Item9, t.Item10, t.Item11, t.Item12, t.Item13, t.Item14, t.Item15);
        }
        // Basic Tuple with sixteen elements  7tuple+7ttuple+2tuple via .Rest
        public static (int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int) GetTuple((int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int) t)
        {
            return (t.Item1, t.Item2, t.Item3, t.Item4, t.Item5, t.Item6, t.Item7, t.Item8, t.Item9, t.Item10, t.Item11, t.Item12, t.Item13, t.Item14, t.Item15, t.Item16);
        }
        // Tuple with various field types
        public static (int, float, double, string, int[], object) GetTuple( (int, float, double, string, int[], object) t)
        {
            return (t.Item1, t.Item2, t.Item3, t.Item4, t.Item5, t.Item6);
        }
    }
}
