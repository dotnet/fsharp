
type Currency =
    // Temporary fix until a new Thoth.Json.Net package is released
    // See https://github.com/MangelMaxime/Thoth/pull/70

#if !FABLE_COMPILER
    private
#endif
    | Code of string
