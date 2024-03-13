ImplFile
  (ParsedImplFileInput
     ("/root/Nullness/GenericFunctionTyparNull.fs", false,
      QualifiedNameOfFile GenericFunctionTyparNull, [], [],
      [SynModuleOrNamespace
         ([GenericFunctionTyparNull], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None,
                     SynValInfo
                       ([[SynArgInfo ([], false, Some x)]],
                        SynArgInfo ([], false, None)), None),
                  LongIdent
                    (SynLongIdent ([myFunc], [], [None]), None, None,
                     Pats
                       [Paren
                          (Typed
                             (Named
                                (SynIdent (x, None), false, None, (1,12--1,13)),
                              WithGlobalConstraints
                                (Var (SynTypar (T, None, false), (1,15--1,17)),
                                 [WhereTyparSupportsNull
                                    (SynTypar (T, None, false), (1,23--1,31))],
                                 (1,15--1,31)), (1,12--1,31)), (1,11--1,32))],
                     None, (1,4--1,32)), None, Const (Int32 42, (1,35--1,37)),
                  (1,4--1,32), NoneAtLet, { LeadingKeyword = Let (1,0--1,3)
                                            InlineKeyword = None
                                            EqualsRange = Some (1,33--1,34) })],
              (1,0--1,37))], PreXmlDocEmpty, [], None, (1,0--2,0),
          { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
