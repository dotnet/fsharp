ImplFile
  (ParsedImplFileInput
     ("/root/Nullness/StringOrNull.fs", false, QualifiedNameOfFile StringOrNull,
      [], [],
      [SynModuleOrNamespace
         ([StringOrNull], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named (SynIdent (x, None), false, None, (1,4--1,5)),
                  Some
                    (SynBindingReturnInfo
                       (LongIdent (SynLongIdent ([string], [], [None])),
                        (1,8--1,14), [], { ColonRange = Some (1,6--1,7) })),
                  Typed
                    (ArbitraryAfterError ("localBinding2", (1,14--1,14)),
                     LongIdent (SynLongIdent ([string], [], [None])),
                     (1,14--1,14)), (1,4--1,5), Yes (1,0--1,14),
                  { LeadingKeyword = Let (1,0--1,3)
                    InlineKeyword = None
                    EqualsRange = None })], (1,0--1,14))], PreXmlDocEmpty, [],
          None, (1,0--2,0), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(1,15)-(1,16) parse error Unexpected symbol '|' in binding. Expected '=' or other token.
