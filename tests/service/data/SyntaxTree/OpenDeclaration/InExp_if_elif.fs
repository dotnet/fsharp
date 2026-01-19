if (open type System.Int32; MaxValue <> MinValue) then
    open type System.Console
    WriteLine "MaxValue is not equal to MinValue"
elif (open type System.Int32; MaxValue < 0) then
    open type System.Console
    WriteLine "MaxValue is negative"
else
    open type System.Console
    WriteLine "MaxValue is positive"
