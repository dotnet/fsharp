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
                  PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([[]], SynArgInfo ([], false, None)), None),
                  LongIdent
                    (SynLongIdent ([GetProcessHeap], [], [None]), None,
                     Some (SynValTyparDecls (None, false)),
                     Pats
                       [Tuple
                          (false, [],
                           /root/ExternKeywordIsPresentInTrivia.fs (2,26--2,27))],
                     None, /root/ExternKeywordIsPresentInTrivia.fs (2,12--2,26)),
                  Some
                    (SynBindingReturnInfo
                       (App
                          (LongIdent
                             (SynLongIdent
                                ([unit], [], [Some (OriginalNotation "void")])),
                           None, [], [], None, false,
                           /root/ExternKeywordIsPresentInTrivia.fs (2,7--2,11)),
                        /root/ExternKeywordIsPresentInTrivia.fs (2,7--2,11), [],
                        { ColonRange = None })),
                  Typed
                    (App
                       (NonAtomic, false, Ident failwith,
                        Const
                          (String
                             ("extern was not given a DllImport attribute",
                              Regular,
                              /root/ExternKeywordIsPresentInTrivia.fs (2,27--2,28)),
                           /root/ExternKeywordIsPresentInTrivia.fs (2,27--2,28)),
                        /root/ExternKeywordIsPresentInTrivia.fs (2,0--2,28)),
                     App
                       (LongIdent
                          (SynLongIdent
                             ([unit], [], [Some (OriginalNotation "void")])),
                        None, [], [], None, false,
                        /root/ExternKeywordIsPresentInTrivia.fs (2,7--2,11)),
                     /root/ExternKeywordIsPresentInTrivia.fs (2,0--2,28)),
                  /root/ExternKeywordIsPresentInTrivia.fs (2,0--2,28),
                  NoneAtInvisible,
                  { LeadingKeyword =
                     Extern /root/ExternKeywordIsPresentInTrivia.fs (2,0--2,6)
                    InlineKeyword = None
                    EqualsRange = None })],
              /root/ExternKeywordIsPresentInTrivia.fs (2,0--2,28))],
          PreXmlDocEmpty, [], None,
          /root/ExternKeywordIsPresentInTrivia.fs (2,0--2,28),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))