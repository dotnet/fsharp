
#r "provider_EVIL_PROVIDER_ResolveTypeName_Exception.dll"

open FSharp.EvilProviderWhereResolveTypeNameRaisesException

// It is enough to try to resolve a type to expose the validation check
type Negative1 = FSharp.EvilProviderWhereResolveTypeNameRaisesException.SomeNameThatDoesntExist
