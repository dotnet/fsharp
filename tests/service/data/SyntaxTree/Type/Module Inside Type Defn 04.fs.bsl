ImplFile
  (ParsedImplFileInput
     ("/root/Type/Module Inside Type Defn 04.fs", false,
      QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [R],
                     PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (3,5--3,6)),
                  Simple
                    (Record
                       (None,
                        [SynField
                           ([], false, Some A,
                            LongIdent (SynLongIdent ([int], [], [None])), false,
                            PreXmlDoc ((4,6), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (4,6--4,13), { LeadingKeyword = None
                                                 MutableKeyword = None })],
                        (4,4--4,15)), (4,4--4,15)), [], None, (3,5--4,15),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = Some (3,7--3,8)
                    WithKeyword = None })], (3,0--4,15));
           NestedModule
             (SynComponentInfo
                ([], None, [], [M4],
                 PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (5,4--5,13)), false,
              [Let
                 (false,
                  [SynBinding
                     (None, Normal, false, false, [],
                      PreXmlDoc ((6,8), FSharp.Compiler.Xml.XmlDocCollector),
                      SynValData
                        (None, SynValInfo ([[]], SynArgInfo ([], false, None)),
                         None),
                      LongIdent
                        (SynLongIdent ([f], [], [None]), None, None,
                         Pats [Paren (Const (Unit, (6,14--6,16)), (6,14--6,16))],
                         None, (6,12--6,16)), None, Const (Unit, (6,19--6,21)),
                      (6,12--6,16), NoneAtLet,
                      { LeadingKeyword = Let (6,8--6,11)
                        InlineKeyword = None
                        EqualsRange = Some (6,17--6,18) })], (6,8--6,21))],
              false, (5,4--6,21), { ModuleKeyword = Some (5,4--5,10)
                                    EqualsRange = Some (5,14--5,15) })],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--6,21), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(5,4)-(5,10) parse warning Invalid declaration syntax
