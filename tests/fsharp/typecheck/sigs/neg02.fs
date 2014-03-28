
module Lib

type TestBadKeywords = 
   class
       virtual x.M() = 3
   end

type TestBadKeywords2 = 
   class
       method x.M() = 3
   end

type staticInInterface =
   class
    interface System.ComponentModel.INotifyPropertyChanged with 
      static member Foo() = ()
      member x.add_PropertyChanged(_) = ()
      member x.remove_PropertyChanged(_) = ()
    end
   end

