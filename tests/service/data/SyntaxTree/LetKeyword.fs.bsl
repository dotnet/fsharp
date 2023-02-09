ImplFile
  (ParsedImplFileInput
     ("/root/LetKeyword.fs", false, QualifiedNameOfFile LetKeyword, [], [],
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
                           /root/LetKeyword.fs (2,6--2,7))], None,
                     /root/LetKeyword.fs (2,4--2,7)), None,
                  App
                    (NonAtomic, false,
                     App
                       (NonAtomic, true,
                        LongIdent
                          (false,
                           SynLongIdent
                             ([op_Addition], [], [Some (OriginalNotation "+")]),
                           None, /root/LetKeyword.fs (2,12--2,13)), Ident b,
                        /root/LetKeyword.fs (2,10--2,13)),
                     Const (Int32 1, /root/LetKeyword.fs (2,14--2,15)),
                     /root/LetKeyword.fs (2,10--2,15)),
                  /root/LetKeyword.fs (2,4--2,7), NoneAtLet,
                  { LeadingKeyword = Let /root/LetKeyword.fs (2,0--2,3)
                    InlineKeyword = None
                    EqualsRange = Some /root/LetKeyword.fs (2,8--2,9) })],
              /root/LetKeyword.fs (2,0--2,15))], PreXmlDocEmpty, [], None,
          /root/LetKeyword.fs (2,0--3,0), { LeadingKeyword = None })],
      (true, false), { ConditionalDirectives = []
                       CodeComments = [] }, set []))