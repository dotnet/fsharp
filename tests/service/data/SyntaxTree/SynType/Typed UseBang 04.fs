module Module

use! _:int = async { return 1 }
use! (_:int) = async { return 2 }