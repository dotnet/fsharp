ImplFile
  (ParsedImplFileInput
     ("/root/Type/SynTypeDefnWithXmlDocContainsTheRangeOfTheTypeKeyword.fs",
      false,
      QualifiedNameOfFile SynTypeDefnWithXmlDocContainsTheRangeOfTheTypeKeyword,
      [], [],
      [SynModuleOrNamespace
         ([SynTypeDefnWithXmlDocContainsTheRangeOfTheTypeKeyword], false,
          AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [A],
                     PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (4,5--4,6)),
                  Simple
                    (TypeAbbrev
                       (Ok, LongIdent (SynLongIdent ([B], [], [None])),
                        (4,9--4,10)), (4,9--4,10)), [], None, (2,0--4,10),
                  { LeadingKeyword = Type (4,0--4,4)
                    EqualsRange = Some (4,7--4,8)
                    WithKeyword = None });
               SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [C],
                     PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (5,4--5,5)),
                  Simple
                    (TypeAbbrev
                       (Ok, LongIdent (SynLongIdent ([D], [], [None])),
                        (5,8--5,9)), (5,8--5,9)), [], None, (5,4--5,9),
                  { LeadingKeyword = And (5,0--5,3)
                    EqualsRange = Some (5,6--5,7)
                    WithKeyword = None })], (2,0--5,9))], PreXmlDocEmpty, [],
          None, (4,0--6,0), { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [LineComment (3,0--3,8)] }, set []))
