ImplFile
  (ParsedImplFileInput
     ("/root/LetKeyword.fs", false, QualifiedNameOfFile LetKeyword, [], [],
      [SynModuleOrNamespace
         ([LetKeyword], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
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
                           /root/LetKeyword.fs (1,6--1,7))], None,
                     /root/LetKeyword.fs (1,4--1,7)), None,
                  App
                    (NonAtomic, false,
                     App
                       (NonAtomic, true,
                        LongIdent
                          (false,
                           SynLongIdent
                             ([op_Addition], [], [Some (OriginalNotation "+")]),
                           None, /root/LetKeyword.fs (1,12--1,13)), Ident b,
                        /root/LetKeyword.fs (1,10--1,13)),
                     Const (Int32 1, /root/LetKeyword.fs (1,14--1,15)),
                     /root/LetKeyword.fs (1,10--1,15)),
                  /root/LetKeyword.fs (1,4--1,7), NoneAtLet,
                  { LeadingKeyword = Let /root/LetKeyword.fs (1,0--1,3)
                    InlineKeyword = None
                    EqualsRange = Some /root/LetKeyword.fs (1,8--1,9) })],
              /root/LetKeyword.fs (1,0--1,15))], PreXmlDocEmpty, [], None,
          /root/LetKeyword.fs (1,0--1,15), { LeadingKeyword = None })],
      (true, false), { ConditionalDirectives = []
                       CodeComments = [] }, set []))