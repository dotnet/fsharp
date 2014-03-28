// #Regression 
module DF

type Datafile = 
  interface
    abstract member advance: unit -> unit
  end

type RawPcapfile =
  class
    new() = {}
    interface Datafile with
	member self.advance ()   = ()
    end
  end
