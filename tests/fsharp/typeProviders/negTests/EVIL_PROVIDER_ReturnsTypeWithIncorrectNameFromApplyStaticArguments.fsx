#r "provider_EVIL_PROVIDER_ReturnsTypeWithIncorrectNameFromApplyStaticArguments.dll"


module GenerativeTypesWithStaticParameters = 
    
    type internal TheContainerTypeJ = FSharp.EvilGeneratedProviderThatReturnsTypeWithIncorrectNameFromApplyStaticArguments.TheContainerType<"J">

    let internal v1 : TheContainerTypeJ.TheGeneratedTypeJ = TheContainerTypeJ.TheGeneratedTypeJ()

