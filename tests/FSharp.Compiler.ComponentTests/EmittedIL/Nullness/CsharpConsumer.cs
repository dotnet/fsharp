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
            var thisShouldNotSayAnything = MyTestModule.genericValueTypeTest(("I am not null",null,1,2,3,4));
            var thisShouldWarnFor2ndItem = MyTestModule.nonNullableInputOutputFunc(thisShouldNotSayAnything.Item2);
            var thisIsOkBecauseItem1IsNotNullable = MyTestModule.nonNullableInputOutputFunc(thisShouldNotSayAnything.Item1);

            var refTuple = MyTestModule.genericRefTypeTest(nullString,nullString,1,2,3,4);
            MyTestModule.genericRefTypeTest(refTuple.Item2,refTuple.Item2,1,2,3,4);
            

            return MyTestModule.multiArgumentTest(null,null);
        }
    }
}