// #Regression #NoMT #CompilerOptions 
// Regression for internal compiler error (ICE), FSB 4674
// Compile with '--debug --optimize-'

open System.IO

let PrependOrReplaceByToString s = id

type StorageDirectory() =

  let rec ReplaceOrInsert (e:StorageDirectory) = 
      let newFiles =  PrependOrReplaceByToString 3 []
      e.Copy(newFiles)

  member self.Copy(files) = 1

(*

// From dsyme:
The call e.Copy([]) is a recursive call. We get to provide any 
instantiation, e.g. T[], and then T becomes a "free choice" 
type parameter. However we don't record a TEXpr_choose anywhere 
for the free choice.

*)

exit 0
