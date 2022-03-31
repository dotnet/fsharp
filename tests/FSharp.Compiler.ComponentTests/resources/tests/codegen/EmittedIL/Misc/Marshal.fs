// #NoMono #NoMT #CodeGen #EmittedIL 
open System.Runtime.InteropServices

type Reader =
   delegate of
       [<Out;MarshalAs(UnmanagedType.LPArray,SizeParamIndex=1s)>]data : byte[] * [<Out>]length : int -> int
