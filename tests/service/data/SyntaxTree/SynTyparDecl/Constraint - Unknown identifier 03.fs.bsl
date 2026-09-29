ImplFile
  (ParsedImplFileInput
     ("/root/SynTyparDecl/Constraint - Unknown identifier 03.fs", false,
      QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
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
                              (SynTypar (T, None, false), (3,15--3,30),
                               { ColonRange = (3,18--3,19)
                                 NotRange = (3,20--3,25) })], (3,6--3,31))), [],
                     Some (LongIdent (SynLongIdent ([I], [], [None]))),
                     PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                     true, None, (3,5--3,6)),
                  ObjectModel (Interface, [], (3,34--3,47)), [], None,
                  (3,5--3,47), { LeadingKeyword = Type (3,0--3,4)
                                 EqualsRange = Some (3,32--3,33)
                                 WithKeyword = None })], (3,0--3,47))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,47), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(3,20)-(3,25) parse error Unexpected identifier: 'maybe'
