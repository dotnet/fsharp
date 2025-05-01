ImplFile
  (ParsedImplFileInput
     ("/root/Expression/SynExprLetOrUseWhereBodyExprStartsWithTokenOfTwoCharactersDoesNotContainTheRangeOfInKeyword.fs",
      false,
      QualifiedNameOfFile
        SynExprLetOrUseWhereBodyExprStartsWithTokenOfTwoCharactersDoesNotContainTheRangeOfInKeyword,
      [], [],
      [SynModuleOrNamespace
         ([SynExprLetOrUseWhereBodyExprStartsWithTokenOfTwoCharactersDoesNotContainTheRangeOfInKeyword],
          false, AnonModule,
          [Expr
             (Do
                (LetOrUse
                   (false, false,
                    [SynBinding
                       (None, Normal, false, false, [],
                        PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                        SynValData
                          (None, SynValInfo ([], SynArgInfo ([], false, None)),
                           None),
                        Named (SynIdent (e1, None), false, None, (3,4--3,6)),
                        None,
                        Downcast
                          (Ident e,
                           LongIdent
                             (SynLongIdent
                                ([Collections; DictionaryEntry], [(3,26--3,27)],
                                 [None; None])), (3,9--3,42)), (3,4--3,6),
                        Yes (3,0--3,42), { LeadingKeyword = Let (3,0--3,3)
                                           InlineKeyword = None
                                           EqualsRange = Some (3,7--3,8) })],
                    Tuple
                      (false,
                       [LongIdent
                          (false,
                           SynLongIdent ([e1; Key], [(4,2--4,3)], [None; None]),
                           None, (4,0--4,6));
                        LongIdent
                          (false,
                           SynLongIdent
                             ([e1; Value], [(4,10--4,11)], [None; None]), None,
                           (4,8--4,16))], [(4,6--4,7)], (4,0--4,16)),
                    (3,0--4,16), { LetOrUseKeyword = (3,0--3,3)
                                   InKeyword = None }), (2,0--4,16)),
              (2,0--4,16))], PreXmlDocEmpty, [], None, (2,0--5,0),
          { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
