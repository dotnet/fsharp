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
                  Typed
                    (Named (SynIdent (x, None), false, None, (1,4--1,5)),
                     WithNull
                       (LongIdent (SynLongIdent ([string], [], [None])), false,
                        (1,8--1,21), { BarRange = (1,15--1,16) }), (1,4--1,21)),
                  None, Null (1,24--1,28), (1,4--1,21), Yes (1,0--1,28),
                  { LeadingKeyword = Let (1,0--1,3)
                    InlineKeyword = None
                    EqualsRange = Some (1,22--1,23) })], (1,0--1,28))],
          PreXmlDocEmpty, [], None, (1,0--2,0), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))
