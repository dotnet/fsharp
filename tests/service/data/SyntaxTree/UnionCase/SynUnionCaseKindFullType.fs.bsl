ImplFile
  (ParsedImplFileInput
     ("/root/UnionCase/SynUnionCaseKindFullType.fs", false,
      QualifiedNameOfFile SynUnionCaseKindFullType, [], [],
      [SynModuleOrNamespace
         ([SynUnionCaseKindFullType], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [X],
                     PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (2,5--2,6)),
                  Simple
                    (Union
                       (None,
                        [SynUnionCase
                           ([], SynIdent (a, None),
                            FullType
                              (Tuple
                                 (false,
                                  [Type
                                     (LongIdent
                                        (SynLongIdent ([int], [], [None])));
                                   Star (3,13--3,14);
                                   Type
                                     (SignatureParameter
                                        ([], false, Some z,
                                         LongIdent
                                           (SynLongIdent ([int], [], [None])),
                                         (3,15--3,20)))], (3,9--3,20)),
                               SynValInfo ([], SynArgInfo ([], false, None))),
                            PreXmlDoc ((3,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (3,6--3,20), { BarRange = Some (3,4--3,5) })],
                        (3,4--3,20)), (3,4--3,20)), [], None, (2,5--3,20),
                  { LeadingKeyword = Type (2,0--2,4)
                    EqualsRange = Some (2,7--2,8)
                    WithKeyword = None })], (2,0--3,20))], PreXmlDocEmpty, [],
          None, (2,0--4,0), { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
