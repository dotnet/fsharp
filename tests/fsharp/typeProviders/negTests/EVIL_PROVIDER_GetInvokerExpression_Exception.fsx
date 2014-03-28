
#r "provider_EVIL_PROVIDER_GetInvokerExpression_Exception.dll"

open FSharp.EvilProviderWhereGetInvokerExpressionRaisesException

// It is enough to name the type to expose the validation check
let x = FSharp.EvilProviderWhereGetInvokerExpressionRaisesException.TheType.Foo
