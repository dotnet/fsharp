ImplFile
  (ParsedImplFileInput
     ("/root/Nullness/GenericTypeNotNull.fs", false,
      QualifiedNameOfFile GenericTypeNotNull, [], [],
      [SynModuleOrNamespace
         ([GenericTypeNotNull], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([],
                     Some
                       (PostfixList
                          ([SynTyparDecl
                              ([], SynTypar (T, None, false), [],
                               { AmpersandRanges = [] })],
                           [WhereTyparNotSupportsNull
                              (SynTypar (T, None, false), (1,15--1,27),
                               { ColonRange = (1,17--1,18)
                                 NotRange = (1,19--1,22) })], (1,6--1,28))), [],
                     [C], PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                     true, None, (1,5--1,6)),
                  ObjectModel (Class, [], (1,31--1,40)), [], None, (1,5--1,40),
                  { LeadingKeyword = Type (1,0--1,4)
                    EqualsRange = Some (1,29--1,30)
                    WithKeyword = None })], (1,0--1,40))], PreXmlDocEmpty, [],
          None, (1,0--2,0), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
