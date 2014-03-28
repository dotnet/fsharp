#r "provider.dll"

// This one exposes the types of a provider, one of which is an array
open FSharp.EvilProvider

// It is enough to name the type to expose the validation check
type Negative18 = FSharp.EvilProvider.TypeWhereGetInterfacesRaisesException
