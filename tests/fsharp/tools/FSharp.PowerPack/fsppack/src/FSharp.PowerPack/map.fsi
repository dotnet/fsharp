namespace Microsoft.FSharp.Compatibility

#nowarn "62" // compat

module Map = 

    open System
    open System.Collections.Generic
    open Microsoft.FSharp.Collections // Tagged.Map etc.

    /// A provider for creating and using maps based on a particular comparison function.
    /// The 'Tag type parameter is used to track information about the comparison function.
    [<CompilerMessage("This construct is for ML compatibility. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    type Provider<'Key,'T,'Tag> when 'Tag :> IComparer<'Key> =
        interface
          abstract empty: Tagged.Map<'Key,'T,'Tag>;
          abstract add: 'Key -> 'T -> Tagged.Map<'Key,'T,'Tag> -> Tagged.Map<'Key,'T,'Tag>;
          abstract find: 'Key -> Tagged.Map<'Key,'T,'Tag> -> 'T;
          abstract first: ('Key -> 'T -> 'U option) -> Tagged.Map<'Key,'T,'Tag> -> 'U option;
          abstract tryfind: 'Key -> Tagged.Map<'Key,'T,'Tag> -> 'T option;
          abstract remove: 'Key -> Tagged.Map<'Key,'T,'Tag> -> Tagged.Map<'Key,'T,'Tag>;
          abstract mem: 'Key -> Tagged.Map<'Key,'T,'Tag> -> bool;
          abstract iter: ('Key -> 'T -> unit) -> Tagged.Map<'Key,'T,'Tag> -> unit;
          abstract map:  ('T -> 'U) -> Tagged.Map<'Key,'T,'Tag> -> Tagged.Map<'Key,'U,'Tag>;
          abstract mapi: ('Key -> 'T -> 'U) -> Tagged.Map<'Key,'T,'Tag> -> Tagged.Map<'Key,'U,'Tag>;
          abstract fold: ('Key -> 'T -> 'State -> 'State) -> Tagged.Map<'Key,'T,'Tag> -> 'State -> 'State
        end

    [<CompilerMessage("This construct is for ML compatibility. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    type Provider<'Key,'T> = Provider<'Key,'T,IComparer<'Key>>
    
    [<CompilerMessage("This construct is for ML compatibility. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val Make: ('Key -> 'Key -> int) -> Provider<'Key,'T>

    /// A functor to build a collection of operations for creating and using 
    /// maps based on the given comparison function. This returns a record that 
    /// contains the functions you use to create and manipulate maps of
    /// this kind.  The returned value is much like an ML module. 
    ///
    /// Language restrictions related to polymorphism may mean you
    /// have to create a new instantiation of for each toplevel
    /// key/value type pair.
    ///
    /// To use this function you need to define a new named class that implements IComparer and
    /// pass an instance of that class as the first argument. For example:
    ///      type MyComparer = 
    ///          new() = { }
    ///          interface IComparer<string> with 
    ///            member self.Compare(x,y) = ...
    ///
    /// let MyStringMapProvider : Map.Provider < string,int > = Map.MakeTagged(new MyComparer())

    [<CompilerMessage("This construct is for ML compatibility. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val MakeTagged: ('Tag :> IComparer<'Key>) -> Provider<'Key,'T,'Tag>

