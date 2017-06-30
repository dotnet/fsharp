namespace Microsoft.VisualStudio.FSharp.Editor

module internal IdentifierUtils =

    open System
    open Microsoft.FSharp.Compiler.PrettyNaming
    open Microsoft.FSharp.Compiler.SourceCodeServices.PrettyNaming

    let DoubleBackTickDelimiter = "``"

    let isDoubleBacktickIdent (s: string) =
        let doubledDelimiter = 2 * DoubleBackTickDelimiter.Length
        if s.StartsWith(DoubleBackTickDelimiter) && s.EndsWith(DoubleBackTickDelimiter) && s.Length > doubledDelimiter then
            let inner = s.Substring(DoubleBackTickDelimiter.Length, s.Length - doubledDelimiter)
            not (inner.Contains(DoubleBackTickDelimiter))
        else false

    let isIdentifier (s: string) =
        if isDoubleBacktickIdent s then
            true
        else
            s |> Seq.mapi (fun i c -> i, c)
              |> Seq.forall (fun (i, c) -> 
                    if i = 0 then IsIdentifierFirstCharacter c else IsIdentifierPartCharacter c) 

    let isOperator (s: string) = 
        let allowedChars = Set.ofList ['!'; '%'; '&'; '*'; '+'; '-'; '.'; '/'; '<'; '='; '>'; '?'; '@'; '^'; '|'; '~']
        (IsPrefixOperator s || IsInfixOperator s || IsTernaryOperator s)
        && (s.ToCharArray() |> Array.forall (fun c -> Set.contains c allowedChars))

    /// Encapsulates identifiers for rename operations if needed
    let encapsulateIdentifier symbolKind newName =
        let isKeyWord = List.exists ((=) newName) KeywordNames
        let isAlreadyEncapsulated = newName.StartsWith DoubleBackTickDelimiter && newName.EndsWith DoubleBackTickDelimiter

        if isAlreadyEncapsulated then newName
        elif (symbolKind = LexerSymbolKind.Operator) || (symbolKind = LexerSymbolKind.GenericTypeParameter) || (symbolKind = LexerSymbolKind.StaticallyResolvedTypeParameter) then newName
        elif isKeyWord || not (isIdentifier newName) then DoubleBackTickDelimiter + newName + DoubleBackTickDelimiter
        else newName

    let isFixableIdentifier (s: string) = 
        not (String.IsNullOrEmpty s) && encapsulateIdentifier LexerSymbolKind.Ident s |> isIdentifier

    let private forbiddenChars = ["."; "+"; "$"; "&"; "["; "]"; "/"; "\\"; "*"; "\""]

    let isTypeNameIdent (s: string) =
        not (String.IsNullOrEmpty s) &&
        forbiddenChars |> Seq.forall (fun c -> not (s.Contains c)) &&
        isFixableIdentifier s 

    let isUnionCaseIdent (s: string) =
        isTypeNameIdent s &&    
        Char.IsUpper(s.Replace(DoubleBackTickDelimiter,"").[0])