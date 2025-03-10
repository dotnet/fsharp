ImplFile
  (ParsedImplFileInput
     ("/root/UnionCase/Recover Function Type 03.fs", false,
      QualifiedNameOfFile Recover Function Type 03, [],
      [SynModuleOrNamespace
         ([Foo], false, DeclaredNamespace,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [B],
                     PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (3,5--3,6)),
                  Simple
                    (Union
                       (None,
                        [SynUnionCase
                           ([], SynIdent (A, None),
                            Fields
                              [SynField
                                 ([], false, None,
                                  Fun
                                    (LongIdent
                                       (SynLongIdent ([int], [], [None])),
                                     Fun
                                       (LongIdent
                                          (SynLongIdent ([int], [], [None])),
                                        LongIdent
                                          (SynLongIdent ([int], [], [None])),
                                        (3,23--3,33),
                                        { ArrowRange = (3,27--3,29) }),
                                     (3,16--3,33), { ArrowRange = (3,20--3,22) }),
                                  false,
                                  PreXmlDoc ((3,16), FSharp.Compiler.Xml.XmlDocCollector),
                                  None, (3,16--3,33), { LeadingKeyword = None
                                                        MutableKeyword = None })],
                            PreXmlDoc ((3,9), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (3,11--3,33), { BarRange = Some (3,9--3,10) })],
                        (3,9--3,33)), (3,9--3,33)), [], None, (3,5--3,33),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = Some (3,7--3,8)
                    WithKeyword = None })], (3,0--3,33));
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
                      WarnDirectives = []
                      CodeComments = [] }, set []))

(3,16)-(3,33) parse error Unexpected function type in union case field definition. If you intend the field to be a function, consider wrapping the function signature with parens, e.g. | Case of a -> b into | Case of (a -> b).
