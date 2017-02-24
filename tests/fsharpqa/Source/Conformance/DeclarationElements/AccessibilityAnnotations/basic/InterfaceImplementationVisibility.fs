
module Definitions =

    type private PrivateArg() = class end

    type internal InternalArg() = class end

    type PublicArg() = class end

    type private IPrivateInterface =  
        abstract A : PublicArg -> InternalArg
        abstract B : PublicArg -> PrivateArg
        abstract C : InternalArg  -> PrivateArg

    type internal IInternalInterface =
        abstract A : PublicArg -> InternalArg

    type IPublicInterface =
        abstract A : PublicArg -> PublicArg

    // class accessibility should have no bearing on
    // being able to implement any of these; as long
    // as all the types are accessible at the implementation
    // location the compiler need not complain.

    // this already worked in F# 4.0
    type private PrivateClass() =
        interface IPrivateInterface with
            member __.A _ = InternalArg()
            member __.B _ = PrivateArg()
            member __.C _ = PrivateArg()
        interface IInternalInterface with
            member __.A _ = InternalArg()
        interface IPublicInterface with
            member __.A _ = PublicArg()

    // these two did no work in F# 4.0.
    type internal InternalClass() =
        interface IPrivateInterface with
            member __.A _ = InternalArg()
            member __.B _ = PrivateArg()
            member __.C _ = PrivateArg()
        interface IInternalInterface with
            member __.A _ = InternalArg()
        interface IPublicInterface with
            member __.A _ = PublicArg()

    type public PublicClass() =
        interface IPrivateInterface with
            member __.A _ = InternalArg()
            member __.B _ = PrivateArg()
            member __.C _ = PrivateArg()
        interface IInternalInterface with
            member __.A _ = InternalArg()
        interface IPublicInterface with
            member __.A _ = PublicArg()

    let private privateValue = PrivateClass()
    let private privateValueAsPrivateInterface = privateValue :> IPrivateInterface
    let internal privateValueAsInternalInterface = privateValue :> IInternalInterface
    let privateValueAsPublicInterface = privateValue :> IPublicInterface

    let internal internalValue = InternalClass()
    let private internalValueAsPrivateInterface  = internalValue :> IPrivateInterface
    let internal internalValueAsInternalInterface = internalValue :> IInternalInterface
    let internalValueAsPublicInterface   = internalValue :> IPublicInterface

    let publicValue = PublicClass()
    let private publicValueAsPrivateInterface  = publicValue :> IPrivateInterface
    let internal publicValueAsInternalInterface = publicValue :> IInternalInterface
    let publicValueAsPublicInterface   = publicValue :> IPublicInterface

module OtherModule =
    open Definitions

    // internal and public is all you can see here; private interface are not visible.

    let internal internalValue = InternalClass()
    let internal internalValueAsInternalInterface = internalValue :> IInternalInterface
    let internalValueAsPublicInterface   = internalValue :> IPublicInterface

    let publicValue = PublicClass()
    let internal publicValueAsInternalInterface = publicValue :> IInternalInterface
    let publicValueAsPublicInterface   = publicValue :> IPublicInterface
