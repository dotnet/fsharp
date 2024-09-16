ImplFile
  (ParsedImplFileInput
     ("/root/Nullness/StringOrNullInFunctionArg.fs", false,
      QualifiedNameOfFile StringOrNullInFunctionArg, [], [],
      [SynModuleOrNamespace
         ([StringOrNullInFunctionArg], false, AnonModule,
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
                              Paren
                                (WithNull
                                   (LongIdent
                                      (SynLongIdent ([string], [], [None])),
                                    false, (1,16--1,29),
                                    { BarRange = (1,23--1,24) }), (1,15--1,30)),
                              (1,12--1,30)), (1,11--1,31))], None, (1,4--1,31)),
                  None, Const (Int32 42, (1,34--1,36)), (1,4--1,31), NoneAtLet,
                  { LeadingKeyword = Let (1,0--1,3)
                    InlineKeyword = None
                    EqualsRange = Some (1,32--1,33) })], (1,0--1,36))],
          PreXmlDocEmpty, [], None, (1,0--2,0), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))
