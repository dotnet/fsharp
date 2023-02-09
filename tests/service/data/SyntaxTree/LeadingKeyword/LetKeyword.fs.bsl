ImplFile
  (ParsedImplFileInput
     ("/root/LeadingKeyword/LetKeyword.fs", false,
      QualifiedNameOfFile LetKeyword, [], [],
      [SynModuleOrNamespace
         ([LetKeyword], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None,
                     SynValInfo
                       ([[SynArgInfo ([], false, Some b)]],
                        SynArgInfo ([], false, None)), None),
                  LongIdent
                    (SynLongIdent ([a], [], [None]), None, None,
                     Pats
                       [Named
                          (SynIdent (b, None), false, None,
                           /root/LeadingKeyword/LetKeyword.fs (2,6--2,7))], None,
                     /root/LeadingKeyword/LetKeyword.fs (2,4--2,7)), None,
                  App
                    (NonAtomic, false,
                     App
                       (NonAtomic, true,
                        LongIdent
                          (false,
                           SynLongIdent
                             ([op_Addition], [], [Some (OriginalNotation "+")]),
                           None, /root/LeadingKeyword/LetKeyword.fs (2,12--2,13)),
                        Ident b, /root/LeadingKeyword/LetKeyword.fs (2,10--2,13)),
                     Const
                       (Int32 1, /root/LeadingKeyword/LetKeyword.fs (2,14--2,15)),
                     /root/LeadingKeyword/LetKeyword.fs (2,10--2,15)),
                  /root/LeadingKeyword/LetKeyword.fs (2,4--2,7), NoneAtLet,
                  { LeadingKeyword =
                     Let /root/LeadingKeyword/LetKeyword.fs (2,0--2,3)
                    InlineKeyword = None
                    EqualsRange =
                     Some /root/LeadingKeyword/LetKeyword.fs (2,8--2,9) })],
              /root/LeadingKeyword/LetKeyword.fs (2,0--2,15))], PreXmlDocEmpty,
          [], None, /root/LeadingKeyword/LetKeyword.fs (2,0--3,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
