// #Conformance #Interop 
let checkNotInitialized s isInitialized = if isInitialized then (printf "FAILED: %s, expected module not to be initialized" s; exit 1)
let checkInitialized s isInitialized = if not isInitialized then (printf "FAILED: %s, expected module to be initialized" s; exit 1)
//-----------------
printfn "Touching value in module Lib1..."
printfn "    --> Lib1.x = %A" Lib1.x
printfn "Checking this did not cause initialization of module Lib1..."
printfn "Touching a mutable value in module Lib1..."
checkNotInitialized "Lib1" InitFlag1.init
printfn "Lib1.forceInit = %A" Lib1.forceInit
checkInitialized "Lib1" InitFlag1.init
//-----------------
printfn "Touching value in module Lib2..."
printfn "    --> Lib2.x = %A" Lib2.x
printfn "Checking this did not cause initialization of module Lib2..."
printfn "Touching a mutable value in module Lib2..."
checkNotInitialized "Lib2" InitFlag2.init
printfn "Lib2.forceInit = %A" Lib2.forceInit
checkInitialized "Lib2" InitFlag2.init
//-----------------
printfn "Touching value in module Lib3..."
printfn "    --> Lib3.x = %A" Lib3.x
printfn "Checking this did not cause initialization of module Lib3..."
printfn "Touching a mutable value in module Lib3..."
checkNotInitialized "Lib3" InitFlag3.init
printfn "Lib3.forceInit = %A" Lib3.forceInit
checkInitialized "Lib3" InitFlag3.init
//-----------------
printfn "Touching value in module Lib4..."
printfn "    --> Lib4.x = %A" Lib4.x
printfn "Checking this did not cause initialization of module Lib4..."
printfn "Touching a mutable value in module Lib4..."
checkNotInitialized "Lib4" InitFlag4.init
printfn "Lib4.forceInit = %A" Lib4.forceInit
checkInitialized "Lib4" InitFlag4.init
//-----------------
printfn "Touching value in module Lib5..."
printfn "    --> Lib5.x = %A" Lib5.x
printfn "Checking this did not cause initialization of module Lib5..."
printfn "Touching a mutable value in module Lib5..."
checkNotInitialized "Lib5" InitFlag5.init
printfn "Lib5.forceInit = %A" Lib5.forceInit
checkInitialized "Lib5" InitFlag5.init
//-----------------
printfn "Touching value in module Lib6..."
printfn "    --> Lib6.x = %A" Lib6.x
printfn "Checking this did not cause initialization of module Lib6..."
printfn "Touching a mutable value in module Lib6..."
checkNotInitialized "Lib6" InitFlag6.init
printfn "Lib6.forceInit = %A" Lib6.forceInit
checkInitialized "Lib6" InitFlag6.init
//-----------------
printfn "Touching value in module Lib7..."
printfn "    --> Lib7.x = %A" Lib7.x
printfn "Checking this did not cause initialization of module Lib7..."
printfn "Touching a mutable value in module Lib7..."
checkNotInitialized "Lib7" InitFlag7.init
printfn "Lib7.forceInit = %A" Lib7.forceInit
checkInitialized "Lib7" InitFlag7.init
//-----------------
printfn "Touching value in module Lib8..."
printfn "    --> Lib8.x = %A" Lib8.x
printfn "Checking this did not cause initialization of module Lib8..."
printfn "Touching a mutable value in module Lib8..."
checkNotInitialized "Lib8" InitFlag8.init
printfn "Lib8.forceInit = %A" Lib8.forceInit
checkInitialized "Lib8" InitFlag8.init
//-----------------
printfn "Touching value in module Lib9..."
printfn "    --> Lib9.x = %A" Lib9.x
printfn "Checking this did not cause initialization of module Lib9..."
printfn "Touching a mutable value in module Lib9..."
checkNotInitialized "Lib9" InitFlag9.init
printfn "Lib9.forceInit = %A" Lib9.forceInit
checkInitialized "Lib9" InitFlag9.init
//-----------------
printfn "Touching value in module Lib10..."
printfn "    --> Lib10.x = %A" Lib10.x
printfn "Checking this did not cause initialization of module Lib10..."
printfn "Touching a mutable value in module Lib10..."
checkNotInitialized "Lib10" InitFlag10.init
printfn "Lib10.forceInit = %A" Lib10.forceInit
checkInitialized "Lib10" InitFlag10.init
//-----------------
printfn "Touching value in module Lib11..."
printfn "    --> Lib11.x = %A" Lib11.x
printfn "Checking this did not cause initialization of module Lib11..."
printfn "Touching a mutable value in module Lib11..."
checkNotInitialized "Lib11" InitFlag11.init
printfn "Lib11.forceInit = %A" Lib11.forceInit
checkInitialized "Lib11" InitFlag11.init
//-----------------
printfn "Touching value in module Lib12..."
printfn "    --> Lib12.x = %A" Lib12.x
printfn "Checking this did not cause initialization of module Lib12..."
printfn "Touching a mutable value in module Lib12..."
checkNotInitialized "Lib12" InitFlag12.init
printfn "Lib12.forceInit = %A" Lib12.forceInit
checkInitialized "Lib12" InitFlag12.init
//-----------------
printfn "Touching value in module Lib13..."
printfn "    --> Lib13.x = %A" Lib13.x
printfn "Checking this did not cause initialization of module Lib13..."
printfn "Touching a mutable value in module Lib13..."
checkNotInitialized "Lib13" InitFlag13.init
printfn "Lib13.forceInit = %A" Lib13.forceInit
checkInitialized "Lib13" InitFlag13.init
//-----------------
printfn "Touching value in module Lib14..."
printfn "    --> Lib14.x = %A" Lib14.x
printfn "Checking this did not cause initialization of module Lib14..."
printfn "Touching a mutable value in module Lib14..."
checkNotInitialized "Lib14" InitFlag14.init
printfn "Lib14.forceInit = %A" Lib14.forceInit
checkInitialized "Lib14" InitFlag14.init
//-----------------
printfn "Touching value in module Lib15..."
printfn "    --> Lib15.x = %A" Lib15.x
printfn "Checking this did not cause initialization of module Lib15..."
printfn "Touching a mutable value in module Lib15..."
checkNotInitialized "Lib15" InitFlag15.init
printfn "Lib15.forceInit = %A" Lib15.forceInit
checkInitialized "Lib15" InitFlag15.init
//-----------------
printfn "Touching value in module Lib16..."
printfn "    --> Lib16.x = %A" Lib16.x
printfn "Checking this did not cause initialization of module Lib16..."
printfn "Touching a mutable value in module Lib16..."
checkNotInitialized "Lib16" InitFlag16.init
printfn "Lib16.forceInit = %A" Lib16.forceInit
checkInitialized "Lib16" InitFlag16.init
//-----------------
printfn "Touching value in module Lib17..."
printfn "    --> Lib17.x = %A" Lib17.x
printfn "Checking this did not cause initialization of module Lib17..."
printfn "Touching a mutable value in module Lib17..."
checkNotInitialized "Lib17" InitFlag17.init
printfn "Lib17.forceInit = %A" Lib17.forceInit
checkInitialized "Lib17" InitFlag17.init
//-----------------
printfn "Touching value in module Lib18..."
printfn "    --> Lib18.x = %A" Lib18.x
printfn "Checking this did not cause initialization of module Lib18..."
printfn "Touching a mutable value in module Lib18..."
checkNotInitialized "Lib18" InitFlag18.init
printfn "Lib18.forceInit = %A" Lib18.forceInit
checkInitialized "Lib18" InitFlag18.init
//-----------------
printfn "Touching value in module Lib19..."
printfn "    --> Lib19.x = %A" Lib19.x
printfn "Checking this did not cause initialization of module Lib19..."
printfn "Touching a mutable value in module Lib19..."
checkNotInitialized "Lib19" InitFlag19.init
printfn "Lib19.forceInit = %A" Lib19.forceInit
checkInitialized "Lib19" InitFlag19.init
//-----------------
printfn "Touching value in module Lib20..."
printfn "    --> Lib20.x = %A" Lib20.x
printfn "Checking this did not cause initialization of module Lib20..."
printfn "Touching a mutable value in module Lib20..."
checkNotInitialized "Lib20" InitFlag20.init
printfn "Lib20.forceInit = %A" Lib20.forceInit
checkInitialized "Lib20" InitFlag20.init
//-----------------
printfn "Touching value in module Lib21..."
printfn "    --> Lib21.x = %A" Lib21.x
printfn "Checking this did not cause initialization of module Lib21..."
printfn "Touching a mutable value in module Lib21..."
checkNotInitialized "Lib21" InitFlag21.init
printfn "Lib21.forceInit = %A" Lib21.forceInit
checkInitialized "Lib21" InitFlag21.init
//-----------------
printfn "Touching value in module Lib22..."
printfn "    --> Lib22.x = %A" Lib22.x
printfn "Checking this did not cause initialization of module Lib22..."
printfn "Touching a mutable value in module Lib22..."
checkNotInitialized "Lib22" InitFlag22.init
printfn "Lib22.forceInit = %A" Lib22.forceInit
checkInitialized "Lib22" InitFlag22.init
//-----------------
printfn "Touching value in module Lib23..."
printfn "    --> Lib23.x = %A" Lib23.x
printfn "Checking this did not cause initialization of module Lib23..."
printfn "Touching a mutable value in module Lib23..."
checkNotInitialized "Lib23" InitFlag23.init
printfn "Lib23.forceInit = %A" Lib23.forceInit
checkInitialized "Lib23" InitFlag23.init
//-----------------
printfn "Touching value in module Lib24..."
printfn "    --> Lib24.x = %A" Lib24.x
printfn "Checking this did not cause initialization of module Lib24..."
printfn "Touching a mutable value in module Lib24..."
checkNotInitialized "Lib24" InitFlag24.init
printfn "Lib24.forceInit = %A" Lib24.forceInit
checkInitialized "Lib24" InitFlag24.init
//-----------------
printfn "Touching value in module Lib25..."
printfn "    --> Lib25.x = %A" Lib25.x
printfn "Checking this did not cause initialization of module Lib25..."
printfn "Touching a mutable value in module Lib25..."
checkNotInitialized "Lib25" InitFlag25.init
printfn "Lib25.forceInit = %A" Lib25.forceInit
checkInitialized "Lib25" InitFlag25.init
//-----------------
printfn "Touching value in module Lib26..."
printfn "    --> Lib26.x = %A" Lib26.x
printfn "Checking this did not cause initialization of module Lib26..."
printfn "Touching a mutable value in module Lib26..."
checkNotInitialized "Lib26" InitFlag26.init
printfn "Lib26.forceInit = %A" Lib26.forceInit
checkInitialized "Lib26" InitFlag26.init
//-----------------
printfn "Touching value in module Lib27..."
printfn "    --> Lib27.x = %A" Lib27.x
printfn "Checking this did not cause initialization of module Lib27..."
printfn "Touching a mutable value in module Lib27..."
checkNotInitialized "Lib27" InitFlag27.init
printfn "Lib27.forceInit = %A" Lib27.forceInit
checkInitialized "Lib27" InitFlag27.init
//-----------------
printfn "Touching value in module Lib28..."
printfn "    --> Lib28.x = %A" Lib28.x
printfn "Checking this did not cause initialization of module Lib28..."
printfn "Touching a mutable value in module Lib28..."
checkNotInitialized "Lib28" InitFlag28.init
printfn "Lib28.forceInit = %A" Lib28.forceInit
checkInitialized "Lib28" InitFlag28.init
//-----------------
printfn "Touching value in module Lib29..."
printfn "    --> Lib29.x = %A" Lib29.x
printfn "Checking this did not cause initialization of module Lib29..."
printfn "Touching a mutable value in module Lib29..."
checkNotInitialized "Lib29" InitFlag29.init
printfn "Lib29.forceInit = %A" Lib29.forceInit
checkInitialized "Lib29" InitFlag29.init
//-----------------
printfn "Touching value in module Lib30..."
printfn "    --> Lib30.x = %A" Lib30.x
printfn "Checking this did not cause initialization of module Lib30..."
printfn "Touching a mutable value in module Lib30..."
checkNotInitialized "Lib30" InitFlag30.init
printfn "Lib30.forceInit = %A" Lib30.forceInit
checkInitialized "Lib30" InitFlag30.init
//-----------------
printfn "Touching value in module Lib31..."
printfn "    --> Lib31.x = %A" Lib31.x
printfn "Checking this did not cause initialization of module Lib31..."
printfn "Touching a mutable value in module Lib31..."
checkNotInitialized "Lib31" InitFlag31.init
printfn "Lib31.forceInit = %A" Lib31.forceInit
checkInitialized "Lib31" InitFlag31.init
//-----------------
printfn "Touching value in module Lib32..."
printfn "    --> Lib32.x = %A" Lib32.x
printfn "Checking this did not cause initialization of module Lib32..."
printfn "Touching a mutable value in module Lib32..."
checkNotInitialized "Lib32" InitFlag32.init
printfn "Lib32.forceInit = %A" Lib32.forceInit
checkInitialized "Lib32" InitFlag32.init
//-----------------
printfn "Touching value in module Lib33..."
printfn "    --> Lib33.x = %A" Lib33.x
printfn "Checking this did not cause initialization of module Lib33..."
printfn "Touching a mutable value in module Lib33..."
checkNotInitialized "Lib33" InitFlag33.init
printfn "Lib33.forceInit = %A" Lib33.forceInit
checkInitialized "Lib33" InitFlag33.init
//-----------------
printfn "Touching value in module Lib34..."
printfn "    --> Lib34.x = %A" Lib34.x
printfn "Checking this did not cause initialization of module Lib34..."
printfn "Touching a mutable value in module Lib34..."
checkNotInitialized "Lib34" InitFlag34.init
printfn "Lib34.forceInit = %A" Lib34.forceInit
checkInitialized "Lib34" InitFlag34.init
//-----------------
printfn "Touching value in module Lib35..."
printfn "    --> Lib35.x = %A" Lib35.x
printfn "Checking this did not cause initialization of module Lib35..."
printfn "Touching a mutable value in module Lib35..."
checkNotInitialized "Lib35" InitFlag35.init
printfn "Lib35.forceInit = %A" Lib35.forceInit
checkInitialized "Lib35" InitFlag35.init
//-----------------
printfn "Touching value in module Lib36..."
printfn "    --> Lib36.x = %A" Lib36.x
printfn "Checking this did not cause initialization of module Lib36..."
printfn "Touching a mutable value in module Lib36..."
checkNotInitialized "Lib36" InitFlag36.init
printfn "Lib36.forceInit = %A" Lib36.forceInit
checkInitialized "Lib36" InitFlag36.init
//-----------------
printfn "Touching value in module Lib37..."
printfn "    --> Lib37.x = %A" Lib37.x
printfn "Checking this did not cause initialization of module Lib37..."
printfn "Touching a mutable value in module Lib37..."
checkNotInitialized "Lib37" InitFlag37.init
printfn "Lib37.forceInit = %A" Lib37.forceInit
checkInitialized "Lib37" InitFlag37.init
//-----------------
printfn "Touching value in module Lib38..."
printfn "    --> Lib38.x = %A" Lib38.x
printfn "Checking this did not cause initialization of module Lib38..."
printfn "Touching a mutable value in module Lib38..."
checkNotInitialized "Lib38" InitFlag38.init
printfn "Lib38.forceInit = %A" Lib38.forceInit
checkInitialized "Lib38" InitFlag38.init
//-----------------
printfn "Touching value in module Lib39..."
printfn "    --> Lib39.x = %A" Lib39.x
printfn "Checking this did not cause initialization of module Lib39..."
printfn "Touching a mutable value in module Lib39..."
checkNotInitialized "Lib39" InitFlag39.init
printfn "Lib39.forceInit = %A" Lib39.forceInit
checkInitialized "Lib39" InitFlag39.init
//-----------------
printfn "Touching value in module Lib40..."
printfn "    --> Lib40.x = %A" Lib40.x
printfn "Checking this did not cause initialization of module Lib40..."
printfn "Touching a mutable value in module Lib40..."
checkNotInitialized "Lib40" InitFlag40.init
printfn "Lib40.forceInit = %A" Lib40.forceInit
checkInitialized "Lib40" InitFlag40.init
//-----------------
printfn "Touching value in module Lib41..."
printfn "    --> Lib41.x = %A" Lib41.x
printfn "Checking this did not cause initialization of module Lib41..."
printfn "Touching a mutable value in module Lib41..."
checkNotInitialized "Lib41" InitFlag41.init
printfn "Lib41.forceInit = %A" Lib41.forceInit
checkInitialized "Lib41" InitFlag41.init
//-----------------
printfn "Touching value in module Lib42..."
printfn "    --> Lib42.x = %A" Lib42.x
printfn "Checking this did not cause initialization of module Lib42..."
printfn "Touching a mutable value in module Lib42..."
checkNotInitialized "Lib42" InitFlag42.init
printfn "Lib42.forceInit = %A" Lib42.forceInit
checkInitialized "Lib42" InitFlag42.init
//-----------------
printfn "Touching value in module Lib43..."
printfn "    --> Lib43.x = %A" Lib43.x
printfn "Checking this did not cause initialization of module Lib43..."
printfn "Touching a mutable value in module Lib43..."
checkNotInitialized "Lib43" InitFlag43.init
printfn "Lib43.forceInit = %A" Lib43.forceInit
checkInitialized "Lib43" InitFlag43.init
//-----------------
printfn "Touching value in module Lib44..."
printfn "    --> Lib44.x = %A" Lib44.x
printfn "Checking this did not cause initialization of module Lib44..."
printfn "Touching a mutable value in module Lib44..."
checkNotInitialized "Lib44" InitFlag44.init
printfn "Lib44.forceInit = %A" Lib44.forceInit
checkInitialized "Lib44" InitFlag44.init
//-----------------
printfn "Touching value in module Lib45..."
printfn "    --> Lib45.x = %A" Lib45.x
printfn "Checking this did not cause initialization of module Lib45..."
printfn "Touching a mutable value in module Lib45..."
checkNotInitialized "Lib45" InitFlag45.init
printfn "Lib45.forceInit = %A" Lib45.forceInit
checkInitialized "Lib45" InitFlag45.init
//-----------------
printfn "Touching value in module Lib46..."
printfn "    --> Lib46.x = %A" Lib46.x
printfn "Checking this did not cause initialization of module Lib46..."
printfn "Touching a mutable value in module Lib46..."
checkNotInitialized "Lib46" InitFlag46.init
printfn "Lib46.forceInit = %A" Lib46.forceInit
checkInitialized "Lib46" InitFlag46.init
//-----------------
printfn "Touching value in module Lib47..."
printfn "    --> Lib47.x = %A" Lib47.x
printfn "Checking this did not cause initialization of module Lib47..."
printfn "Touching a mutable value in module Lib47..."
checkNotInitialized "Lib47" InitFlag47.init
printfn "Lib47.forceInit = %A" Lib47.forceInit
checkInitialized "Lib47" InitFlag47.init
//-----------------
printfn "Touching value in module Lib48..."
printfn "    --> Lib48.x = %A" Lib48.x
printfn "Checking this did not cause initialization of module Lib48..."
printfn "Touching a mutable value in module Lib48..."
checkNotInitialized "Lib48" InitFlag48.init
printfn "Lib48.forceInit = %A" Lib48.forceInit
checkInitialized "Lib48" InitFlag48.init
//-----------------
printfn "Touching value in module Lib49..."
printfn "    --> Lib49.x = %A" Lib49.x
printfn "Checking this did not cause initialization of module Lib49..."
printfn "Touching a mutable value in module Lib49..."
checkNotInitialized "Lib49" InitFlag49.init
printfn "Lib49.forceInit = %A" Lib49.forceInit
checkInitialized "Lib49" InitFlag49.init
//-----------------
printfn "Touching value in module Lib50..."
printfn "    --> Lib50.x = %A" Lib50.x
printfn "Checking this did not cause initialization of module Lib50..."
printfn "Touching a mutable value in module Lib50..."
checkNotInitialized "Lib50" InitFlag50.init
printfn "Lib50.forceInit = %A" Lib50.forceInit
checkInitialized "Lib50" InitFlag50.init
//-----------------
printfn "Touching value in module Lib51..."
printfn "    --> Lib51.x = %A" Lib51.x
printfn "Checking this did not cause initialization of module Lib51..."
printfn "Touching a mutable value in module Lib51..."
checkNotInitialized "Lib51" InitFlag51.init
printfn "Lib51.forceInit = %A" Lib51.forceInit
checkInitialized "Lib51" InitFlag51.init
//-----------------
printfn "Touching value in module Lib52..."
printfn "    --> Lib52.x = %A" Lib52.x
printfn "Checking this did not cause initialization of module Lib52..."
printfn "Touching a mutable value in module Lib52..."
checkNotInitialized "Lib52" InitFlag52.init
printfn "Lib52.forceInit = %A" Lib52.forceInit
checkInitialized "Lib52" InitFlag52.init
//-----------------
printfn "Touching value in module Lib53..."
printfn "    --> Lib53.x = %A" Lib53.x
printfn "Checking this did not cause initialization of module Lib53..."
printfn "Touching a mutable value in module Lib53..."
checkNotInitialized "Lib53" InitFlag53.init
printfn "Lib53.forceInit = %A" Lib53.forceInit
checkInitialized "Lib53" InitFlag53.init
//-----------------
printfn "Touching value in module Lib54..."
printfn "    --> Lib54.x = %A" Lib54.x
printfn "Checking this did not cause initialization of module Lib54..."
printfn "Touching a mutable value in module Lib54..."
checkNotInitialized "Lib54" InitFlag54.init
printfn "Lib54.forceInit = %A" Lib54.forceInit
checkInitialized "Lib54" InitFlag54.init
//-----------------
printfn "Touching value in module Lib55..."
printfn "    --> Lib55.x = %A" Lib55.x
printfn "Checking this did not cause initialization of module Lib55..."
printfn "Touching a mutable value in module Lib55..."
checkNotInitialized "Lib55" InitFlag55.init
printfn "Lib55.forceInit = %A" Lib55.forceInit
checkInitialized "Lib55" InitFlag55.init
//-----------------
printfn "Touching value in module Lib56..."
printfn "    --> Lib56.x = %A" Lib56.x
printfn "Checking this did not cause initialization of module Lib56..."
printfn "Touching a mutable value in module Lib56..."
checkNotInitialized "Lib56" InitFlag56.init
printfn "Lib56.forceInit = %A" Lib56.forceInit
checkInitialized "Lib56" InitFlag56.init
//-----------------
printfn "Touching value in module Lib57..."
printfn "    --> Lib57.x = %A" Lib57.x
printfn "Checking this did not cause initialization of module Lib57..."
printfn "Touching a mutable value in module Lib57..."
checkNotInitialized "Lib57" InitFlag57.init
printfn "Lib57.forceInit = %A" Lib57.forceInit
checkInitialized "Lib57" InitFlag57.init
//-----------------
printfn "Touching value in module Lib58..."
printfn "    --> Lib58.x = %A" Lib58.x
printfn "Checking this did cause initialization of module Lib58..."
checkInitialized "Lib58" InitFlag58.init
//-----------------
printfn "Touching value in module Lib59..."
printfn "    --> Lib59.x = %A" Lib59.x
printfn "Checking this did cause initialization of module Lib59..."
checkInitialized "Lib59" InitFlag59.init
//-----------------
printfn "Touching value in module Lib60..."
printfn "    --> Lib60.x = %A" Lib60.x
printfn "Checking this did cause initialization of module Lib60..."
checkInitialized "Lib60" InitFlag60.init
//-----------------
printfn "Touching value in module Lib61..."
printfn "    --> Lib61.x = %A" Lib61.x
printfn "Checking this did cause initialization of module Lib61..."
checkInitialized "Lib61" InitFlag61.init
//-----------------
printfn "Touching value in module Lib62..."
printfn "    --> Lib62.x = %A" Lib62.x
printfn "Checking this did cause initialization of module Lib62..."
checkInitialized "Lib62" InitFlag62.init
//-----------------
printfn "Touching value in module Lib63..."
printfn "    --> Lib63.x = %A" Lib63.x
printfn "Checking this did cause initialization of module Lib63..."
checkInitialized "Lib63" InitFlag63.init
//-----------------
printfn "Touching value in module Lib64..."
printfn "    --> Lib64.x = %A" Lib64.x
printfn "Checking this did cause initialization of module Lib64..."
checkInitialized "Lib64" InitFlag64.init
//-----------------
printfn "Touching value in module Lib65..."
printfn "    --> Lib65.x = %A" Lib65.x
printfn "Checking this did cause initialization of module Lib65..."
checkInitialized "Lib65" InitFlag65.init
//-----------------
printfn "Touching value in module Lib66..."
printfn "    --> Lib66.x = %A" Lib66.x
printfn "Checking this did cause initialization of module Lib66..."
checkInitialized "Lib66" InitFlag66.init
//-----------------
printfn "Touching value in module Lib67..."
printfn "    --> Lib67.x = %A" Lib67.x
printfn "Checking this did cause initialization of module Lib67..."
checkInitialized "Lib67" InitFlag67.init
//-----------------
printfn "Touching value in module Lib68..."
printfn "    --> Lib68.x = %A" Lib68.x
printfn "Checking this did cause initialization of module Lib68..."
checkInitialized "Lib68" InitFlag68.init
//-----------------
printfn "Touching value in module Lib69..."
printfn "    --> Lib69.x = %A" Lib69.x
printfn "Checking this did cause initialization of module Lib69..."
checkInitialized "Lib69" InitFlag69.init
//-----------------
printfn "Touching value in module Lib70..."
printfn "    --> Lib70.x = %A" Lib70.x
printfn "Checking this did cause initialization of module Lib70..."
checkInitialized "Lib70" InitFlag70.init
//-----------------
printfn "Touching value in module Lib71..."
printfn "    --> Lib71.x = %A" Lib71.x
printfn "Checking this did cause initialization of module Lib71..."
checkInitialized "Lib71" InitFlag71.init
//-----------------
printfn "Touching value in module Lib72..."
printfn "    --> Lib72.x = %A" Lib72.x
printfn "Checking this did cause initialization of module Lib72..."
checkInitialized "Lib72" InitFlag72.init
//-----------------
printfn "Touching value in module Lib73..."
printfn "    --> Lib73.x = %A" Lib73.x
printfn "Checking this did cause initialization of module Lib73..."
checkInitialized "Lib73" InitFlag73.init
//-----------------
printfn "Touching value in module Lib74..."
printfn "    --> Lib74.x = %A" Lib74.x
printfn "Checking this did cause initialization of module Lib74..."
checkInitialized "Lib74" InitFlag74.init
//-----------------
printfn "Touching value in module Lib75..."
printfn "    --> Lib75.x = %A" Lib75.x
printfn "Checking this did cause initialization of module Lib75..."
checkInitialized "Lib75" InitFlag75.init
//-----------------
printfn "Touching value in module Lib76..."
printfn "    --> Lib76.x = %A" Lib76.x
printfn "Checking this did cause initialization of module Lib76..."
checkInitialized "Lib76" InitFlag76.init
//-----------------
printfn "Touching value in module Lib77..."
printfn "    --> Lib77.x = %A" Lib77.x
printfn "Checking this did cause initialization of module Lib77..."
checkInitialized "Lib77" InitFlag77.init
//-----------------
printfn "Touching value in module Lib78..."
printfn "    --> Lib78.x = %A" Lib78.x
printfn "Checking this did cause initialization of module Lib78..."
checkInitialized "Lib78" InitFlag78.init
//-----------------
printfn "Touching value in module Lib79..."
printfn "    --> Lib79.x = %A" Lib79.x
printfn "Checking this did cause initialization of module Lib79..."
checkInitialized "Lib79" InitFlag79.init
//-----------------
printfn "Touching value in module Lib80..."
printfn "    --> Lib80.x = %A" Lib80.x
printfn "Checking this did cause initialization of module Lib80..."
checkInitialized "Lib80" InitFlag80.init
//-----------------
printfn "Touching value in module Lib81..."
printfn "    --> Lib81.x = %A" Lib81.x
printfn "Checking this did cause initialization of module Lib81..."
checkInitialized "Lib81" InitFlag81.init
//-----------------
printfn "Touching value in module Lib82..."
printfn "    --> Lib82.x = %A" Lib82.x
printfn "Checking this did cause initialization of module Lib82..."
checkInitialized "Lib82" InitFlag82.init
//-----------------
printfn "Touching value in module Lib83..."
printfn "    --> Lib83.x = %A" Lib83.x
printfn "Checking this did cause initialization of module Lib83..."
checkInitialized "Lib83" InitFlag83.init
//-----------------
printfn "Touching value in module Lib84..."
printfn "    --> Lib84.x = %A" Lib84.x
printfn "Checking this did cause initialization of module Lib84..."
checkInitialized "Lib84" InitFlag84.init
//-----------------
printfn "Touching value in module Lib85..."
printfn "    --> Lib85.x = %A" Lib85.x
printfn "Checking this did cause initialization of module Lib85..."
checkInitialized "Lib85" InitFlag85.init
printf "TEST PASSED OK" ;
exit 0
