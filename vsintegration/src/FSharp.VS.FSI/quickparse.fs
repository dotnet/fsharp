// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Interactive
open System.Globalization

// Duplication note:
//      Inlined from Language Service.
//      If/when there is a common DLL this code can be shared.

/// Methods for cheaply and innacurately parsing F#
module internal QuickParse =
    /// The characters that are allowed to be in an identifier.
    let private IsIdentifierPartCharacter c =
        let cat = System.Char.GetUnicodeCategory(c)
        (
        // Letters
           cat = UnicodeCategory.UppercaseLetter 
        || cat = UnicodeCategory.LowercaseLetter 
        || cat = UnicodeCategory.TitlecaseLetter
        || cat = UnicodeCategory.ModifierLetter
        || cat = UnicodeCategory.OtherLetter
        || cat = UnicodeCategory.LetterNumber 
        // Numbers
        || cat = UnicodeCategory.DecimalDigitNumber
        // Connectors
        || cat = UnicodeCategory.ConnectorPunctuation 
        // Combiners
        || cat = UnicodeCategory.NonSpacingMark 
        || cat = UnicodeCategory.SpacingCombiningMark
        ) 
    /// Is this character a part of a long identifier 
    let private IsLongIdentifierPartCharacter c = 
        (IsIdentifierPartCharacter c) || (c='.')
    /// Given a line of text and an index into the line return the
    /// "island" of text that could be an identifier around index.
    let GetIdentifierIsland(line:string,index)= 
        if index<line.Length then 
            let hoverChar = line.[index]
            if not (IsLongIdentifierPartCharacter hoverChar) then 
                // The character is not an identifier character. Return nothing
                None
            else
                let mutable left = index
                let mutable right = if line.[index]='.' then index+1 else index
                let len = line.Length
                while left>=0 && IsLongIdentifierPartCharacter line.[left] do left<-left-1
                while right<len && IsIdentifierPartCharacter line.[right] do right<-right+1
                let result = line.Substring(left+1,right-left-1)
                Some(result)
        else
            None
    /// Get the partial long name of the identifier to the left of index.
    let GetPartialLongName(line:string,index) =
        let IsIdentifierPartCharacter(pos) = IsIdentifierPartCharacter(line.[pos])
        let IsLongIdentifierPartCharacter(pos) = IsLongIdentifierPartCharacter(line.[pos])
        let IsDot(pos) = line.[pos]='.'

        let rec InLeadingIdentifier(pos,right,prior) = 
            let PushName() = 
                (line.Substring(pos+1,right-pos-1),None)::prior
            if pos < 0 then PushName()
            else if IsIdentifierPartCharacter(pos) then InLeadingIdentifier(pos-1,right,prior)
            else if IsDot(pos) then InLeadingIdentifier(pos-1,pos,PushName())
            else PushName()
        let rec InName(pos,startResidue,right) =
            let NameAndResidue() = 
                [line.Substring(pos+1,startResidue-pos-1),Some(line.Substring(startResidue+1,right-startResidue))]
            if pos < 0 then [line.Substring(pos+1,startResidue-pos-1),Some(line.Substring(startResidue+1,right-startResidue))]
            else if IsIdentifierPartCharacter(pos) then InName(pos-1,startResidue,right) 
            else if IsDot(pos) then InLeadingIdentifier(pos-1,pos,NameAndResidue())
            else NameAndResidue()
        and InResidueOrName(pos,right) =
            if pos < 0 then [line.Substring(pos+1,right-pos),None]
            else if IsDot(pos) then InName(pos-1,pos,right)
            else if IsLongIdentifierPartCharacter(pos) then InResidueOrName(pos-1, right)
            else [line.Substring(pos+1,right-pos),None]
            
        let result = InResidueOrName(index,index)
        result
           
