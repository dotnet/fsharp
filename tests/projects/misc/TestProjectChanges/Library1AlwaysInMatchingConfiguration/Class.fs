
namespace Library1AlwaysInMatchingConfiguration

type Class() = 
        static member PropertyAlwaysAvailable = "F#"
#if DEBUG
        static member PropertyAvailableInProject1DebugConfiguration = "F#"
#endif
#if RELEASE
        static member PropertyAvailableInProject1ReleaseConfiguration = "F#"
#endif
