ImplFile
  (ParsedImplFileInput
     ("/root/ExternKeywordIsPresentInTrivia.fs", false,
      QualifiedNameOfFile ExternKeywordIsPresentInTrivia, [], [],
      [SynModuleOrNamespace
         ([ExternKeywordIsPresentInTrivia], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([[]], SynArgInfo ([], false, None)), None),
                  LongIdent
                    (SynLongIdent ([GetProcessHeap], [], [None]), None,
                     Some (SynValTyparDecls (None, false)),
                     Pats
                       [Tuple
                          (false, [],
                           /root/ExternKeywordIsPresentInTrivia.fs (1,26--1,27))],
                     None, /root/ExternKeywordIsPresentInTrivia.fs (1,12--1,26)),
                  Some
                    (SynBindingReturnInfo
                       (App
                          (LongIdent
                             (SynLongIdent
                                ([unit], [], [Some (OriginalNotation "void")])),
                           None, [], [], None, false,
                           /root/ExternKeywordIsPresentInTrivia.fs (1,7--1,11)),
                        /root/ExternKeywordIsPresentInTrivia.fs (1,7--1,11), [],
                        { ColonRange = None })),
                  Typed
                    (App
                       (NonAtomic, false, Ident failwith,
                        Const
                          (String
                             ("extern was not given a DllImport attribute",
                              Regular,
                              /root/ExternKeywordIsPresentInTrivia.fs (1,27--1,28)),
                           /root/ExternKeywordIsPresentInTrivia.fs (1,27--1,28)),
                        /root/ExternKeywordIsPresentInTrivia.fs (1,0--1,28)),
                     App
                       (LongIdent
                          (SynLongIdent
                             ([unit], [], [Some (OriginalNotation "void")])),
                        None, [], [], None, false,
                        /root/ExternKeywordIsPresentInTrivia.fs (1,7--1,11)),
                     /root/ExternKeywordIsPresentInTrivia.fs (1,0--1,28)),
                  /root/ExternKeywordIsPresentInTrivia.fs (1,0--1,28),
                  NoneAtInvisible,
                  { LeadingKeyword =
                     Extern /root/ExternKeywordIsPresentInTrivia.fs (1,0--1,6)
                    InlineKeyword = None
                    EqualsRange = None })],
              /root/ExternKeywordIsPresentInTrivia.fs (1,0--1,28))],
          PreXmlDocEmpty, [], None,
          /root/ExternKeywordIsPresentInTrivia.fs (1,0--1,28),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))