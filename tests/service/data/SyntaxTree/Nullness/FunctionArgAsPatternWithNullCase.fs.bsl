ImplFile
  (ParsedImplFileInput
     ("/root/Nullness/FunctionArgAsPatternWithNullCase.fs", false,
      QualifiedNameOfFile FunctionArgAsPatternWithNullCase, [], [],
      [SynModuleOrNamespace
         ([FunctionArgAsPatternWithNullCase], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None,
                     SynValInfo
                       ([[SynArgInfo ([], false, None)]],
                        SynArgInfo ([], false, None)), None),
                  LongIdent
                    (SynLongIdent ([myFunc], [], [None]), None, None,
                     Pats
                       [Paren
                          (Or
                             (Or
                                (Const
                                   (String ("abc", Regular, (1,12--1,17)),
                                    (1,12--1,17)),
                                 Typed
                                   (Const
                                      (String ("", Regular, (1,20--1,22)),
                                       (1,20--1,22)),
                                    WithNull
                                      (LongIdent
                                         (SynLongIdent ([string], [], [None])),
                                       false, (1,25--1,38),
                                       { BarRange = (1,32--1,33) }),
                                    (1,20--1,38)), (1,12--1,38),
                                 { BarRange = (1,18--1,19) }),
                              Const
                                (String ("123", Regular, (1,41--1,46)),
                                 (1,41--1,46)), (1,12--1,46),
                              { BarRange = (1,39--1,40) }), (1,11--1,47))], None,
                     (1,4--1,47)), None, Const (Int32 15, (1,50--1,52)),
                  (1,4--1,47), NoneAtLet, { LeadingKeyword = Let (1,0--1,3)
                                            InlineKeyword = None
                                            EqualsRange = Some (1,48--1,49) })],
              (1,0--1,52))], PreXmlDocEmpty, [], None, (1,0--2,0),
          { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
