ImplFile
  (ParsedImplFileInput
     ("/root/Nullness/GenericTypeNull.fs", false,
      QualifiedNameOfFile GenericTypeNull, [], [],
      [SynModuleOrNamespace
         ([GenericTypeNull], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([],
                     Some
                       (PostfixList
                          ([SynTyparDecl
                              ([], SynTypar (T, None, false), [],
                               { AmpersandRanges = [] })],
                           [WhereTyparSupportsNull
                              (SynTypar (T, None, false), (1,15--1,23))],
                           (1,6--1,24))), [], [C],
                     PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                     true, None, (1,5--1,6)),
                  ObjectModel (Class, [], (1,27--1,36)), [], None, (1,5--1,36),
                  { LeadingKeyword = Type (1,0--1,4)
                    EqualsRange = Some (1,25--1,26)
                    WithKeyword = None })], (1,0--1,36))], PreXmlDocEmpty, [],
          None, (1,0--2,0), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
