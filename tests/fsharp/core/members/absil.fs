#indent "off"



  type MyComponent =
    { noiseListeners: ListenerList<NoiseLevel>;
      paintListeners: ListenerListForDelegateType<PaintEventHandler,PaintEventArgs> }
    member this.OnNoise() = noiseListeners.AsEvent
    member this.OnPaint() = paintListeners.AsEvent

  let NewMyComponent () = 
    let noiseListeners = NewListenerList() in 
    let paintListeners = NewListenerListForDelegateType() in 
    let fireNoise() = noiseListeners.Fire(noiseLevel) in 
    let firePaint() = paintListeners.Fire(noiseLevel) in 
    // Here we would connect to some other resource that triggers events
    // and call fireNoise and firePaint in response
    { noiseListeners = noiseListeners;
      paintListeners = paintListeners }

type code = 
    | BasicBlock of basic_block
    | GroupBlock of local_debug_info list * code list
    | RestrictBlock of CodeLabel list * code
    | TryBlock of code * seh
    
    member this.X() = match this with ...
    static member op_Addition(a,b) = GroupBlock([],[a;b])
    static member op_Implicit(a: code, b: ICode) = match this with ...

    interface Object 
      with this.ToString() = ()
      and this.GetHashCode() = ()
      and this.Finalize() = ()
    interface IComparable
      with this.CompareTo(that) = ()

  member this.Name = ()
  member this.X = ()

type local = 
    { localType: Type;
      localPinned: bool  }
    interface Object 
      with ToString(this) = ()
      and GetHashCode(this) = ()
      and Finalize(this) = ()
    interface IComparable
      with StructuralComparisonFastTo(this,that) = ()
