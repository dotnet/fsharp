ImplFile
  (ParsedImplFileInput
     ("/root/Extern/ExternKeywordIsPresentInTrivia.fs", false,
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
                     Pats [Tuple (false, [], (2,26--2,27))], None, (2,12--2,26)),
                  Some
                    (SynBindingReturnInfo
                       (App
                          (LongIdent
                             (SynLongIdent
                                ([unit], [], [Some (OriginalNotation "void")])),
                           None, [], [], None, false, (2,7--2,11)), (2,7--2,11),
                        [], { ColonRange = None })),
                  Typed
                    (App
                       (NonAtomic, false, Ident failwith,
                        Const
                          (String
                             ("extern was not given a DllImport attribute",
                              Regular, (2,27--2,28)), (2,27--2,28)), (2,0--2,28)),
                     App
                       (LongIdent
                          (SynLongIdent
                             ([unit], [], [Some (OriginalNotation "void")])),
                        None, [], [], None, false, (2,7--2,11)), (2,0--2,28)),
                  (2,0--2,28), NoneAtInvisible,
                  { LeadingKeyword = Extern (2,0--2,6)
                    InlineKeyword = None
                    EqualsRange = None })], (2,0--2,28))], PreXmlDocEmpty, [],
          None, (2,0--2,28), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
