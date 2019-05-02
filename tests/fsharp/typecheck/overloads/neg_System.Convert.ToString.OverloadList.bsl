
neg_System.Convert.ToString.OverloadList.fsx(2,1,2,31): typecheck error FS0041: No overloads match for method 'ToString'.



Arguments given:

 - char

 - int



 Available overloads:

 - System.Convert.ToString(value: obj, provider: System.IFormatProvider) : string

 - System.Convert.ToString(value: bool, provider: System.IFormatProvider) : string

 - System.Convert.ToString(value: char, provider: System.IFormatProvider) : string

 - System.Convert.ToString(value: sbyte, provider: System.IFormatProvider) : string

 - System.Convert.ToString(value: byte, provider: System.IFormatProvider) : string

 - System.Convert.ToString(value: int16, provider: System.IFormatProvider) : string

 - System.Convert.ToString(value: uint16, provider: System.IFormatProvider) : string

 - System.Convert.ToString(value: int, provider: System.IFormatProvider) : string

 - System.Convert.ToString(value: uint32, provider: System.IFormatProvider) : string

 - System.Convert.ToString(value: int64, provider: System.IFormatProvider) : string

 - System.Convert.ToString(value: uint64, provider: System.IFormatProvider) : string

 - System.Convert.ToString(value: float32, provider: System.IFormatProvider) : string

 - System.Convert.ToString(value: float, provider: System.IFormatProvider) : string

 - System.Convert.ToString(value: decimal, provider: System.IFormatProvider) : string

 - System.Convert.ToString(value: System.DateTime, provider: System.IFormatProvider) : string

 - System.Convert.ToString(value: string, provider: System.IFormatProvider) : string

 - System.Convert.ToString(value: byte, toBase: int) : string

 - System.Convert.ToString(value: int16, toBase: int) : string

 - System.Convert.ToString(value: int, toBase: int) : string

 - System.Convert.ToString(value: int64, toBase: int) : string



neg_System.Convert.ToString.OverloadList.fsx(3,1,3,47): typecheck error FS0041: No overloads match for method 'ToString'.



Arguments given:

 - (provider) : char

 - (value) : int



 Available overloads:

 - System.Convert.ToString(value: obj, provider: System.IFormatProvider) : string

 - System.Convert.ToString(value: bool, provider: System.IFormatProvider) : string

 - System.Convert.ToString(value: char, provider: System.IFormatProvider) : string

 - System.Convert.ToString(value: sbyte, provider: System.IFormatProvider) : string

 - System.Convert.ToString(value: byte, provider: System.IFormatProvider) : string

 - System.Convert.ToString(value: int16, provider: System.IFormatProvider) : string

 - System.Convert.ToString(value: uint16, provider: System.IFormatProvider) : string

 - System.Convert.ToString(value: int, provider: System.IFormatProvider) : string

 - System.Convert.ToString(value: uint32, provider: System.IFormatProvider) : string

 - System.Convert.ToString(value: int64, provider: System.IFormatProvider) : string

 - System.Convert.ToString(value: uint64, provider: System.IFormatProvider) : string

 - System.Convert.ToString(value: float32, provider: System.IFormatProvider) : string

 - System.Convert.ToString(value: float, provider: System.IFormatProvider) : string

 - System.Convert.ToString(value: decimal, provider: System.IFormatProvider) : string

 - System.Convert.ToString(value: System.DateTime, provider: System.IFormatProvider) : string

 - System.Convert.ToString(value: string, provider: System.IFormatProvider) : string



neg_System.Convert.ToString.OverloadList.fsx(4,1,4,48): typecheck error FS0041: A unique overload for method 'ToString' could not be determined based on type information prior to this program point. A type annotation may be needed.



Arguments given:

 - (provider) : 'a when 'a : null

 - (value) : int







Candidates:

 - System.Convert.ToString(value: int, provider: System.IFormatProvider) : string

 - System.Convert.ToString(value: obj, provider: System.IFormatProvider) : string


