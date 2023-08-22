ImplFile
  (ParsedImplFileInput
     ("/root/UnionCase/Recover Function Type 01.fs", false,
      QualifiedNameOfFile Recover Function Type 01, [], [],
      [SynModuleOrNamespace
         ([Foo], false, DeclaredNamespace,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [Bar],
                     PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (3,5--3,8)),
                  Simple
                    (Union
                       (None,
                        [SynUnionCase
                           ([], SynIdent (Bar, None),
                            Fields
                              [SynField
                                 ([], false, None,
                                  Fun
                                    (LongIdent
                                       (SynLongIdent ([int], [], [None])),
                                     LongIdent
                                       (SynLongIdent ([int], [], [None])),
                                     (3,20--3,30), { ArrowRange = (3,24--3,26) }),
                                  false,
                                  PreXmlDoc ((3,20), FSharp.Compiler.Xml.XmlDocCollector),
                                  None, (3,20--3,30), { LeadingKeyword = None })],
                            PreXmlDoc ((3,11), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (3,13--3,30), { BarRange = Some (3,11--3,12) })],
                        (3,11--3,30)), (3,11--3,30)), [], None, (3,5--3,30),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = Some (3,9--3,10)
                    WithKeyword = None })], (3,0--3,30));
           Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [Other],
                     PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (4,5--4,10)),
                  Simple
                    (TypeAbbrev
                       (Ok, LongIdent (SynLongIdent ([int], [], [None])),
                        (4,13--4,16)), (4,13--4,16)), [], None, (4,5--4,16),
                  { LeadingKeyword = Type (4,0--4,4)
                    EqualsRange = Some (4,11--4,12)
                    WithKeyword = None })], (4,0--4,16))], PreXmlDocEmpty, [],
          None, (1,0--4,16), { LeadingKeyword = Namespace (1,0--1,9) })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))

(3,20)-(3,30) parse error Unexpected function type in union case field definition. If you intend the field to be a function, consider wrapping the function signature with parens, e.g. | Case of a -> b into | Case of (a -> b).
