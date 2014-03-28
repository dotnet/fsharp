

namespace MyTPRuntime

open System
open Microsoft.FSharp.Core.CompilerServices

#if WITHDLLEXT
[<assembly:TypeProviderAssembly("MyTPDesignTime.dll")>]
#endif

#if FULLNAME
[<assembly:TypeProviderAssembly("MyTPDesignTime, Version=2.2.2.2, Culture=neutral, PublicKeyToken=940f4698116c96c9, processorArchitecture=MSIL")>]
#endif

#if NODLLEXT
[<assembly:TypeProviderAssembly("MyTPDesignTime")>]
#endif


do()
