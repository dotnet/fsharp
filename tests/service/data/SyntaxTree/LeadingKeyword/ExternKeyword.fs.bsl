ImplFile
  (ParsedImplFileInput
     ("/root/LeadingKeyword/ExternKeyword.fs", false,
      QualifiedNameOfFile ExternKeyword, [], [],
      [SynModuleOrNamespace
         ([ExternKeyword], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([[]], SynArgInfo ([], false, None)), None),
                  LongIdent
                    (SynLongIdent ([Meh], [], [None]), None,
                     Some (SynValTyparDecls (None, false)),
                     Pats
                       [Tuple
                          (false, [],
                           /root/LeadingKeyword/ExternKeyword.fs (2,15--2,16))],
                     None, /root/LeadingKeyword/ExternKeyword.fs (2,12--2,15)),
                  Some
                    (SynBindingReturnInfo
                       (App
                          (LongIdent
                             (SynLongIdent
                                ([unit], [], [Some (OriginalNotation "void")])),
                           None, [], [], None, false,
                           /root/LeadingKeyword/ExternKeyword.fs (2,7--2,11)),
                        /root/LeadingKeyword/ExternKeyword.fs (2,7--2,11), [],
                        { ColonRange = None })),
                  Typed
                    (App
                       (NonAtomic, false, Ident failwith,
                        Const
                          (String
                             ("extern was not given a DllImport attribute",
                              Regular,
                              /root/LeadingKeyword/ExternKeyword.fs (2,16--2,17)),
                           /root/LeadingKeyword/ExternKeyword.fs (2,16--2,17)),
                        /root/LeadingKeyword/ExternKeyword.fs (2,0--2,17)),
                     App
                       (LongIdent
                          (SynLongIdent
                             ([unit], [], [Some (OriginalNotation "void")])),
                        None, [], [], None, false,
                        /root/LeadingKeyword/ExternKeyword.fs (2,7--2,11)),
                     /root/LeadingKeyword/ExternKeyword.fs (2,0--2,17)),
                  /root/LeadingKeyword/ExternKeyword.fs (2,0--2,17),
                  NoneAtInvisible,
                  { LeadingKeyword =
                     Extern /root/LeadingKeyword/ExternKeyword.fs (2,0--2,6)
                    InlineKeyword = None
                    EqualsRange = None })],
              /root/LeadingKeyword/ExternKeyword.fs (2,0--2,17))],
          PreXmlDocEmpty, [], None,
          /root/LeadingKeyword/ExternKeyword.fs (2,0--2,17),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
