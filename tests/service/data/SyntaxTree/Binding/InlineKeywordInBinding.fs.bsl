ImplFile
  (ParsedImplFileInput
     ("/root/Binding/InlineKeywordInBinding.fs", false,
      QualifiedNameOfFile InlineKeywordInBinding, [], [],
      [SynModuleOrNamespace
         ([InlineKeywordInBinding], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, true, false, [],
                  PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None,
                     SynValInfo
                       ([[SynArgInfo ([], false, Some y)];
                         [SynArgInfo ([], false, Some z)]],
                        SynArgInfo ([], false, None)), None),
                  LongIdent
                    (SynLongIdent ([x], [], [None]), None, None,
                     Pats
                       [Named (SynIdent (y, None), false, None, (2,13--2,14));
                        Named (SynIdent (z, None), false, None, (2,15--2,16))],
                     None, (2,11--2,16)), None,
                  LetOrUse
                    (false, false,
                     [SynBinding
                        (None, Normal, true, false, [],
                         PreXmlDoc ((3,4), FSharp.Compiler.Xml.XmlDocCollector),
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
                                 (SynIdent (b, None), false, None, (3,17--3,18));
                               Named
                                 (SynIdent (c, None), false, None, (3,19--3,20))],
                            None, (3,15--3,20)), None,
                         Const (Unit, (3,23--3,25)), (3,15--3,20), NoneAtLet,
                         { LeadingKeyword = Let (3,4--3,7)
                           InlineKeyword = Some (3,8--3,14)
                           EqualsRange = Some (3,21--3,22) })],
                     Const (Unit, (4,4--4,6)), (3,4--4,6), { InKeyword = None }),
                  (2,11--2,16), NoneAtLet, { LeadingKeyword = Let (2,0--2,3)
                                             InlineKeyword = Some (2,4--2,10)
                                             EqualsRange = Some (2,17--2,18) })],
              (2,0--4,6))], PreXmlDocEmpty, [], None, (2,0--5,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
