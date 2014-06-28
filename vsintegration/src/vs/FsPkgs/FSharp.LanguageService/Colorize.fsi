// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.LanguageService
    open Microsoft.VisualStudio.FSharp.LanguageService
    open Microsoft.FSharp.Compiler.SourceCodeServices
    open Microsoft.VisualStudio.TextManager.Interop
    open Microsoft.VisualStudio.Text
    
    /// An instance of this is stored in the IVsUserData for the IVsTextLines buffer
    /// and retrieved using languageServiceState.GetColorizer(IVsTextLines).
    type internal FSharpScanner =
        /// Construct a single scanner object which can be used to scan different lines of text.
        /// Each time a scan of new line of text is started the makeLineTokenizer function is called.
        new : makeLineTokenizer:(string -> LineTokenizer) -> FSharpScanner

        /// Scan a token from a line. This should only be used in cases where color information is irrelevant. 
        /// Used by GetFullLineInfo (and only thus in a small workaroud in GetDeclarations) and GetTokenInformationAt (thus GetF1KeywordString).
        member ScanTokenWithDetails: lexState:LexState ref -> TokenInformation option

        /// Scan a token from a line and write information about it into the tokeninfo object.
        member ScanTokenAndProvideInfoAboutIt: line:int * tokenInfo:TokenInfo * lexState:LexState ref -> bool

        /// Start tokenizing a line
        member SetLineText: lineText:string -> unit
 
    type internal FSharpColorizer =
        inherit Colorizer
        new : onClose:(FSharpColorizer -> unit) * buffer:IVsTextLines * scanner:FSharpScanner -> FSharpColorizer
        member GetFullLineInfo : line:string * lastColorState:int -> TokenInformation[]
        member Buffer: IVsTextLines 

        /// Adjust the set of extra colorizations and return a sorted list of affected lines.
        member SetExtraColorizations : (* (ITextSnapshot * Range -> bool) * *) (Range * TokenColorKind)[] -> int[]

        /// Provide token information for the token at the given line and column
        member GetTokenInfoAt: colorState:IVsTextColorState * line:int * column:int -> TokenInfo

        /// Provide token information for the token at the given line and column (2nd variation - allows caller to get token info if an additional string were to be inserted)
        member GetTokenInfoAt: colorState:IVsTextColorState * line:int * column:int * trialString:string * trialStringInsertionCol:int-> TokenInfo

        /// Provide token information for the token at the given line and column (3rd variation)
        member GetTokenInformationAt: colorState:IVsTextColorState * line:int * column:int -> option<TokenInformation>

    // ~- These are unittest-only ~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~
    module internal ColorStateLookup = 
        val LexStateOfColorState : int -> LexState
    // ~- These are unittest-only ~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~
