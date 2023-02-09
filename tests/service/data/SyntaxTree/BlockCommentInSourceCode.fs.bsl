ImplFile
  (ParsedImplFileInput
     ("/root/BlockCommentInSourceCode.fs", false,
      QualifiedNameOfFile BlockCommentInSourceCode, [], [],
      [SynModuleOrNamespace
         ([BlockCommentInSourceCode], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None,
                     SynValInfo
                       ([[SynArgInfo ([], false, Some c)]],
                        SynArgInfo ([], false, None)), None),
                  LongIdent
                    (SynLongIdent ([a], [], [None]), None, None,
                     Pats
                       [Named
                          (SynIdent (c, None), false, None,
                           /root/BlockCommentInSourceCode.fs (1,15--1,16))],
                     None, /root/BlockCommentInSourceCode.fs (1,4--1,16)), None,
                  App
                    (NonAtomic, false,
                     App
                       (NonAtomic, true,
                        LongIdent
                          (false,
                           SynLongIdent
                             ([op_Addition], [], [Some (OriginalNotation "+")]),
                           None, /root/BlockCommentInSourceCode.fs (1,21--1,22)),
                        Ident c, /root/BlockCommentInSourceCode.fs (1,19--1,22)),
                     Const
                       (Int32 42, /root/BlockCommentInSourceCode.fs (1,23--1,25)),
                     /root/BlockCommentInSourceCode.fs (1,19--1,25)),
                  /root/BlockCommentInSourceCode.fs (1,4--1,16), NoneAtLet,
                  { LeadingKeyword =
                     Let /root/BlockCommentInSourceCode.fs (1,0--1,3)
                    InlineKeyword = None
                    EqualsRange =
                     Some /root/BlockCommentInSourceCode.fs (1,17--1,18) })],
              /root/BlockCommentInSourceCode.fs (1,0--1,25))], PreXmlDocEmpty,
          [], None, /root/BlockCommentInSourceCode.fs (1,0--1,25),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments =
         [BlockComment /root/BlockCommentInSourceCode.fs (1,6--1,13)] }, set []))