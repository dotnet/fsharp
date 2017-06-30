namespace Library2AlwaysInDebugConfiguration

type Class() = 
        static member PropertyAlwaysAvailable = "F#"
#if DEBUG
        static member PropertyAvailableInProject2DebugConfiguration = "F#"
#endif
#if RELEASE
        static member PropertyAvailableInProject2ReleaseConfiguration = "F#"
#endif
