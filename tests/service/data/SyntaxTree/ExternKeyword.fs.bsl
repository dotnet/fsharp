ImplFile
  (ParsedImplFileInput
     ("/root/ExternKeyword.fs", false, QualifiedNameOfFile ExternKeyword, [], [],
      [SynModuleOrNamespace
         ([ExternKeyword], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([[]], SynArgInfo ([], false, None)), None),
                  LongIdent
                    (SynLongIdent ([Meh], [], [None]), None,
                     Some (SynValTyparDecls (None, false)),
                     Pats
                       [Tuple (false, [], /root/ExternKeyword.fs (1,15--1,16))],
                     None, /root/ExternKeyword.fs (1,12--1,15)),
                  Some
                    (SynBindingReturnInfo
                       (App
                          (LongIdent
                             (SynLongIdent
                                ([unit], [], [Some (OriginalNotation "void")])),
                           None, [], [], None, false,
                           /root/ExternKeyword.fs (1,7--1,11)),
                        /root/ExternKeyword.fs (1,7--1,11), [],
                        { ColonRange = None })),
                  Typed
                    (App
                       (NonAtomic, false, Ident failwith,
                        Const
                          (String
                             ("extern was not given a DllImport attribute",
                              Regular, /root/ExternKeyword.fs (1,16--1,17)),
                           /root/ExternKeyword.fs (1,16--1,17)),
                        /root/ExternKeyword.fs (1,0--1,17)),
                     App
                       (LongIdent
                          (SynLongIdent
                             ([unit], [], [Some (OriginalNotation "void")])),
                        None, [], [], None, false,
                        /root/ExternKeyword.fs (1,7--1,11)),
                     /root/ExternKeyword.fs (1,0--1,17)),
                  /root/ExternKeyword.fs (1,0--1,17), NoneAtInvisible,
                  { LeadingKeyword = Extern /root/ExternKeyword.fs (1,0--1,6)
                    InlineKeyword = None
                    EqualsRange = None })], /root/ExternKeyword.fs (1,0--1,17))],
          PreXmlDocEmpty, [], None, /root/ExternKeyword.fs (1,0--1,17),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))