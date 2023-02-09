ImplFile
  (ParsedImplFileInput
     ("/root/InlineKeywordInBinding.fs", false,
      QualifiedNameOfFile InlineKeywordInBinding, [], [],
      [SynModuleOrNamespace
         ([InlineKeywordInBinding], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, true, false, [],
                  PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None,
                     SynValInfo
                       ([[SynArgInfo ([], false, Some y)];
                         [SynArgInfo ([], false, Some z)]],
                        SynArgInfo ([], false, None)), None),
                  LongIdent
                    (SynLongIdent ([x], [], [None]), None, None,
                     Pats
                       [Named
                          (SynIdent (y, None), false, None,
                           /root/InlineKeywordInBinding.fs (1,13--1,14));
                        Named
                          (SynIdent (z, None), false, None,
                           /root/InlineKeywordInBinding.fs (1,15--1,16))], None,
                     /root/InlineKeywordInBinding.fs (1,11--1,16)), None,
                  LetOrUse
                    (false, false,
                     [SynBinding
                        (None, Normal, true, false, [],
                         PreXmlDoc ((2,4), FSharp.Compiler.Xml.XmlDocCollector),
                         SynValData
                           (None,
                            SynValInfo
                              ([[SynArgInfo ([], false, Some b)];
                                [SynArgInfo ([], false, Some c)]],
                               SynArgInfo ([], false, None)), None),
                         LongIdent
                           (SynLongIdent ([a], [], [None]), None, None,
                            Pats
                              [Named
                                 (SynIdent (b, None), false, None,
                                  /root/InlineKeywordInBinding.fs (2,17--2,18));
                               Named
                                 (SynIdent (c, None), false, None,
                                  /root/InlineKeywordInBinding.fs (2,19--2,20))],
                            None, /root/InlineKeywordInBinding.fs (2,15--2,20)),
                         None,
                         Const
                           (Unit, /root/InlineKeywordInBinding.fs (2,23--2,25)),
                         /root/InlineKeywordInBinding.fs (2,15--2,20), NoneAtLet,
                         { LeadingKeyword =
                            Let /root/InlineKeywordInBinding.fs (2,4--2,7)
                           InlineKeyword =
                            Some /root/InlineKeywordInBinding.fs (2,8--2,14)
                           EqualsRange =
                            Some /root/InlineKeywordInBinding.fs (2,21--2,22) })],
                     Const (Unit, /root/InlineKeywordInBinding.fs (3,4--3,6)),
                     /root/InlineKeywordInBinding.fs (2,4--3,6),
                     { InKeyword = None }),
                  /root/InlineKeywordInBinding.fs (1,11--1,16), NoneAtLet,
                  { LeadingKeyword =
                     Let /root/InlineKeywordInBinding.fs (1,0--1,3)
                    InlineKeyword =
                     Some /root/InlineKeywordInBinding.fs (1,4--1,10)
                    EqualsRange =
                     Some /root/InlineKeywordInBinding.fs (1,17--1,18) })],
              /root/InlineKeywordInBinding.fs (1,0--3,6))], PreXmlDocEmpty, [],
          None, /root/InlineKeywordInBinding.fs (1,0--3,6),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))