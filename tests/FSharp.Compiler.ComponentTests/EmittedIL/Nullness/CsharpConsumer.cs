namespace CsharpNamespace
{
#nullable enable
    public static class CsharpClass
    {
        public static string MyApiWhichHatesNulls(string x)
        {
            return x.Length.ToString();
        }

        public static string JustUseAllOfItHere()
        {
            var thisShouldWarn = MyTestModule.nonNullableInputOutputFunc(null);
            var thisIsPossiblyNullAndIsOk = MyTestModule.nullableStringInputOutputFunc(thisShouldWarn);
            var thereforeThisShouldWarnAgain = MyTestModule.nonNullableInputOutputFunc(thisIsPossiblyNullAndIsOk);

            string? nullString = null;

            var myStructTuple = (nullString,nullString,1,2,3,4);
            var thisShouldBeAWarningForSecondTypar = MyTestModule.genericValueTypeTest(myStructTuple);
            // The line below gives a warning, but should not => FIX IT
            var thisShouldNotSayAnything = MyTestModule.genericValueTypeTest(("I am not nulll",null,1,2,3,4));
            // The line below should give a warning, but it is missing => FIX IT
            var thisShouldWarnFor2ndItem = MyTestModule.nonNullableInputOutputFunc(thisShouldNotSayAnything.Item2);
            var thisIsOkBecauseItem1IsNotNullable = MyTestModule.nonNullableInputOutputFunc(thisShouldNotSayAnything.Item1);

            var refTuple = MyTestModule.genericRefTypeTest(nullString,nullString,1,2,3,4);
            // The one below should warn for using .Item2 for 1st argument, but it does not => FIX IT
            MyTestModule.genericRefTypeTest(refTuple.Item2,refTuple.Item2,1,2,3,4);
            

            return MyTestModule.multiArgumentTest(null,null);
        }
    }
}