// #Conformance #DeclarationElements #MemberDefinitions #MethodsAndProperties 
#light

// Verify you can declare an indexer from an interface
type IAmReadOnly =
    abstract ROValue : int with get

type IAmWriteOnly =
    abstract WOValue : int with set


type IAmReadWrite =
    abstract RWValue : int with get, set

let mutable ROCalled = false
let mutable WOCalled = false
let mutable RWRCalled = false
let mutable RWWCalled = false

type Test() =
    interface IAmReadOnly with
        override this.ROValue with get() = ROCalled <- true; 42
    interface IAmWriteOnly with
        override this.WOValue with set x = WOCalled <- true; ()
    interface IAmReadWrite with
        override this.RWValue with get() = RWRCalled <- true; 42
                              and  set x = RWWCalled <- true; ()

let t = new Test()

if ROCalled <> false then exit 1
if (t :> IAmReadOnly).ROValue <> 42 then exit 1
if ROCalled <> true then exit 1

if WOCalled <> false then exit 1
(t :> IAmWriteOnly).WOValue <- 4 
if WOCalled <> true then exit 1

if RWRCalled <> false then exit 1
if RWWCalled <> false then exit 1
if (t :> IAmReadWrite).RWValue <> 42 then exit 1
(t :> IAmReadWrite).RWValue <- 4 
if RWRCalled <> true then exit 1
if RWWCalled <> true then exit 1

exit 0
