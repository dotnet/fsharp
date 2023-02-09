ImplFile
  (ParsedImplFileInput
     ("/root/CodeComment/BlockCommentInSourceCode.fs", false,
      QualifiedNameOfFile BlockCommentInSourceCode, [], [],
      [SynModuleOrNamespace
         ([BlockCommentInSourceCode], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
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
                           /root/CodeComment/BlockCommentInSourceCode.fs (2,15--2,16))],
                     None,
                     /root/CodeComment/BlockCommentInSourceCode.fs (2,4--2,16)),
                  None,
                  App
                    (NonAtomic, false,
                     App
                       (NonAtomic, true,
                        LongIdent
                          (false,
                           SynLongIdent
                             ([op_Addition], [], [Some (OriginalNotation "+")]),
                           None,
                           /root/CodeComment/BlockCommentInSourceCode.fs (2,21--2,22)),
                        Ident c,
                        /root/CodeComment/BlockCommentInSourceCode.fs (2,19--2,22)),
                     Const
                       (Int32 42,
                        /root/CodeComment/BlockCommentInSourceCode.fs (2,23--2,25)),
                     /root/CodeComment/BlockCommentInSourceCode.fs (2,19--2,25)),
                  /root/CodeComment/BlockCommentInSourceCode.fs (2,4--2,16),
                  NoneAtLet,
                  { LeadingKeyword =
                     Let
                       /root/CodeComment/BlockCommentInSourceCode.fs (2,0--2,3)
                    InlineKeyword = None
                    EqualsRange =
                     Some
                       /root/CodeComment/BlockCommentInSourceCode.fs (2,17--2,18) })],
              /root/CodeComment/BlockCommentInSourceCode.fs (2,0--2,25))],
          PreXmlDocEmpty, [], None,
          /root/CodeComment/BlockCommentInSourceCode.fs (2,0--3,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments =
         [BlockComment /root/CodeComment/BlockCommentInSourceCode.fs (2,6--2,13)] },
      set []))