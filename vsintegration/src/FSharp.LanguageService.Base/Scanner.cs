// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;

namespace Microsoft.VisualStudio.FSharp.LanguageService {
    internal enum TokenColor {
        Text,
        Keyword,
        Comment,
        Identifier,
        String,
        Number
    }
    internal class TokenInfo {
        int startIndex;
        int endIndex;
        TokenColor color;
        TokenType type;
        TokenTriggers trigger;
        int token;

        public int StartIndex {
            get { return this.startIndex; }
            set { this.startIndex = value; }
        }
        public int EndIndex {
            get { return this.endIndex; }
            set { this.endIndex = value; }
        }
        public TokenColor Color {
            get { return this.color; }
            set { this.color = value; }
        }
        public TokenType Type {
            get { return this.type; }
            set { this.type = value; }
        }
        public TokenTriggers Trigger {
            get { return this.trigger; }
            set { this.trigger = value; }
        }

        /// <summary>Language Specific</summary>
        public int Token {
            get { return token; }
            set { token = value; }
        }
        
        public TokenInfo() { }
        public TokenInfo(int startIndex, int endIndex, TokenType type) { this.startIndex = startIndex; this.endIndex = endIndex; this.type = type; }
    }

    /// <summary>
    /// TokenTriggers:
    /// If a character has (a) trigger(s) associated with it, it may
    /// fire one or both of the following triggers:
    /// MemberSelect - a member selection tip window
    /// MatchBraces - highlight matching braces
    /// or the following trigger:
    /// MethodTip - a method tip window
    ///     
    /// The following triggers exist for speed reasons: the (fast) lexer 
    /// determines when a (slow) parse might be needed. 
    /// The Trigger.MethodTip trigger is subdivided in 4 
    /// other triggers. It is the best to be as specific as possible;
    /// it is better to return Trigger.ParamStart than Trigger.Param
    /// (or Trigger.MethodTip) 
    /// </summary>
    [FlagsAttribute]
    internal enum TokenTriggers {
        None = 0x00,
        MemberSelect = 0x01,
        MatchBraces = 0x02,
        MethodTip = 0xF0,
        ParameterStart = 0x10,
        ParameterNext = 0x20,
        ParameterEnd = 0x40,
        Parameter = 0x80
    }

    internal enum TokenType {
        Unknown,
        Text,
        Keyword,
        Identifier,
        String,
        Literal,
        Operator,
        Delimiter,
        WhiteSpace,
        LineComment,
        Comment,
    }
}
