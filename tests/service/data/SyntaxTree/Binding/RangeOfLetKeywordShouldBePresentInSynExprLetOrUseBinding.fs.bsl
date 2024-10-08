ImplFile
  (ParsedImplFileInput
     ("/root/Binding/RangeOfLetKeywordShouldBePresentInSynExprLetOrUseBinding.fs",
      false,
      QualifiedNameOfFile
        RangeOfLetKeywordShouldBePresentInSynExprLetOrUseBinding, [], [],
      [SynModuleOrNamespace
         ([RangeOfLetKeywordShouldBePresentInSynExprLetOrUseBinding], false,
          AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named (SynIdent (a, None), false, None, (2,4--2,5)), None,
                  LetOrUse
                    (false, false,
                     [SynBinding
                        (None, Normal, false, false, [],
                         PreXmlDoc ((3,4), FSharp.Compiler.Xml.XmlDocCollector),
                         SynValData
                           (None,
                            SynValInfo
                              ([[SynArgInfo ([], false, Some c)]],
                               SynArgInfo ([], false, None)), None),
                         LongIdent
                           (SynLongIdent ([b], [], [None]), None, None,
                            Pats
                              [Named
                                 (SynIdent (c, None), false, None, (3,10--3,11))],
                            None, (3,8--3,11)), None, Ident d, (3,8--3,11),
                         NoneAtLet, { LeadingKeyword = Let (3,4--3,7)
                                      InlineKeyword = None
                                      EqualsRange = Some (3,12--3,13) })],
                     Const (Unit, (4,4--4,6)), (3,4--4,6),
                     { LetOrUseKeyword = (3,4--3,7)
                       InKeyword = None }), (2,4--2,5), NoneAtLet,
                  { LeadingKeyword = Let (2,0--2,3)
                    InlineKeyword = None
                    EqualsRange = Some (2,6--2,7) })], (2,0--4,6))],
          PreXmlDocEmpty, [], None, (2,0--5,0), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))
