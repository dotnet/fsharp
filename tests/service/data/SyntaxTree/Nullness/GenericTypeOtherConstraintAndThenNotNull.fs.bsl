ImplFile
  (ParsedImplFileInput
     ("/root/Nullness/GenericTypeOtherConstraintAndThenNotNull.fs", false,
      QualifiedNameOfFile GenericTypeOtherConstraintAndThenNotNull, [], [],
      [SynModuleOrNamespace
         ([GenericTypeOtherConstraintAndThenNotNull], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([],
                     Some
                       (PostfixList
                          ([SynTyparDecl
                              ([], SynTypar (T, None, false), [],
                               { AmpersandRanges = [] })],
                           [WhereTyparIsEquatable
                              (SynTypar (T, None, false), (1,15--1,26));
                            WhereTyparNotSupportsNull
                              (SynTypar (T, None, false), (1,31--1,43),
                               { ColonRange = (1,33--1,34)
                                 NotRange = (1,35--1,38) })], (1,6--1,44))), [],
                     [C], PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                     true, None, (1,5--1,6)),
                  ObjectModel (Class, [], (1,47--1,56)), [], None, (1,5--1,56),
                  { LeadingKeyword = Type (1,0--1,4)
                    EqualsRange = Some (1,45--1,46)
                    WithKeyword = None })], (1,0--1,56))], PreXmlDocEmpty, [],
          None, (1,0--2,0), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
