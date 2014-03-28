
#r "provider_EVIL_PROVIDER_ResolveTypeName_Null.dll"

open FSharp.EvilProviderWhereResolveTypeNameReturnsNull

// It is enough to try to resolve a type to expose the validation check
type Negative1 = FSharp.EvilProviderWhereResolveTypeNameReturnsNull.SomeNameThatDoesntExist
