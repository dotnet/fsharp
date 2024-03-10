ImplFile
  (ParsedImplFileInput
     ("/root/LeadingKeyword/LetRecKeyword.fs", false,
      QualifiedNameOfFile LetRecKeyword, [], [],
      [SynModuleOrNamespace
         ([LetRecKeyword], false, AnonModule,
          [Let
             (true,
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
                       [Named (SynIdent (b, None), false, None, (2,10--2,11))],
                     None, (2,8--2,11)), None,
                  App
                    (NonAtomic, false,
                     App
                       (NonAtomic, true,
                        LongIdent
                          (false,
                           SynLongIdent
                             ([op_Addition], [], [Some (OriginalNotation "+")]),
                           None, (2,16--2,17)), Ident b, (2,14--2,17)),
                     Const (Int32 1, (2,18--2,19)), (2,14--2,19)), (2,8--2,11),
                  NoneAtLet, { LeadingKeyword = LetRec ((2,0--2,3), (2,4--2,7))
                               InlineKeyword = None
                               EqualsRange = Some (2,12--2,13) })], (2,0--2,19))],
          PreXmlDocEmpty, [], None, (2,0--3,0), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))
