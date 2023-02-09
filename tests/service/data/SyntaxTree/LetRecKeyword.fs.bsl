ImplFile
  (ParsedImplFileInput
     ("/root/LetRecKeyword.fs", false, QualifiedNameOfFile LetRecKeyword, [], [],
      [SynModuleOrNamespace
         ([LetRecKeyword], false, AnonModule,
          [Let
             (true,
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
                           /root/LetRecKeyword.fs (1,10--1,11))], None,
                     /root/LetRecKeyword.fs (1,8--1,11)), None,
                  App
                    (NonAtomic, false,
                     App
                       (NonAtomic, true,
                        LongIdent
                          (false,
                           SynLongIdent
                             ([op_Addition], [], [Some (OriginalNotation "+")]),
                           None, /root/LetRecKeyword.fs (1,16--1,17)), Ident b,
                        /root/LetRecKeyword.fs (1,14--1,17)),
                     Const (Int32 1, /root/LetRecKeyword.fs (1,18--1,19)),
                     /root/LetRecKeyword.fs (1,14--1,19)),
                  /root/LetRecKeyword.fs (1,8--1,11), NoneAtLet,
                  { LeadingKeyword =
                     LetRec
                       (/root/LetRecKeyword.fs (1,0--1,3),
                        /root/LetRecKeyword.fs (1,4--1,7))
                    InlineKeyword = None
                    EqualsRange = Some /root/LetRecKeyword.fs (1,12--1,13) })],
              /root/LetRecKeyword.fs (1,0--1,19))], PreXmlDocEmpty, [], None,
          /root/LetRecKeyword.fs (1,0--1,19), { LeadingKeyword = None })],
      (true, false), { ConditionalDirectives = []
                       CodeComments = [] }, set []))