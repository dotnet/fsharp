#r "provider_EVIL_PROVIDER_GetStaticParametersForMethod_Exception.dll"


open FSharp.EvilProviderWhereGetStaticParametersForMethodRaisesException

// It is enough to name the type to expose the validation check
let Negative1 = FSharp.EvilProviderWhereGetStaticParametersForMethodRaisesException.TheType.Boo<3>()
