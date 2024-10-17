ImplFile
  (ParsedImplFileInput
     ("/root/Nullness/GenericTypeNotNullAndOtherConstraint.fs", false,
      QualifiedNameOfFile GenericTypeNotNullAndOtherConstraint, [], [],
      [SynModuleOrNamespace
         ([GenericTypeNotNullAndOtherConstraint], false, AnonModule,
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
                                 NotRange = (1,19--1,22) });
                            WhereTyparIsEquatable
                              (SynTypar (T, None, false), (1,32--1,43))],
                           (1,6--1,44))), [], [C],
                     PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                     true, None, (1,5--1,6)),
                  ObjectModel (Class, [], (1,47--1,56)), [], None, (1,5--1,56),
                  { LeadingKeyword = Type (1,0--1,4)
                    EqualsRange = Some (1,45--1,46)
                    WithKeyword = None })], (1,0--1,56))], PreXmlDocEmpty, [],
          None, (1,0--2,0), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
