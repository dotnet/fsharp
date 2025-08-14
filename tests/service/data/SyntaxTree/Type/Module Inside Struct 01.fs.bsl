ImplFile
  (ParsedImplFileInput
     ("/root/Type/Module Inside Struct 01.fs", false, QualifiedNameOfFile Module,
      [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([{ Attributes =
                         [{ TypeName = SynLongIdent ([Struct], [], [None])
                            ArgExpr = Const (Unit, (4,2--4,8))
                            Target = None
                            AppliesToGetterAndSetter = false
                            Range = (4,2--4,8) }]
                        Range = (4,0--4,10) }], None, [], [MyStruct],
                     PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (5,5--5,13)),
                  ObjectModel
                    (Unspecified,
                     [ValField
                        (SynField
                           ([], false, Some X,
                            LongIdent (SynLongIdent ([int], [], [None])), false,
                            PreXmlDoc ((6,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (6,4--6,14),
                            { LeadingKeyword = Some (Val (6,4--6,7))
                              MutableKeyword = None }), (6,4--6,14));
                      ValField
                        (SynField
                           ([], false, Some Y,
                            LongIdent (SynLongIdent ([int], [], [None])), false,
                            PreXmlDoc ((7,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (7,4--7,14),
                            { LeadingKeyword = Some (Val (7,4--7,7))
                              MutableKeyword = None }), (7,4--7,14))],
                     (6,4--7,14)), [], None, (4,0--7,14),
                  { LeadingKeyword = Type (5,0--5,4)
                    EqualsRange = Some (5,14--5,15)
                    WithKeyword = None })], (4,0--7,14));
           NestedModule
             (SynComponentInfo
                ([], None, [], [InvalidModule],
                 PreXmlDoc ((8,4), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (8,4--8,24)), false,
              [Let
                 (false,
                  [SynBinding
                     (None, Normal, false, false, [],
                      PreXmlDoc ((9,8), FSharp.Compiler.Xml.XmlDocCollector),
                      SynValData
                        (None, SynValInfo ([], SynArgInfo ([], false, None)),
                         None),
                      Named (SynIdent (helper, None), false, None, (9,12--9,18)),
                      None, Const (Int32 10, (9,21--9,23)), (9,12--9,18),
                      Yes (9,8--9,23), { LeadingKeyword = Let (9,8--9,11)
                                         InlineKeyword = None
                                         EqualsRange = Some (9,19--9,20) })],
                  (9,8--9,23))], false, (8,4--9,23),
              { ModuleKeyword = Some (8,4--8,10)
                EqualsRange = Some (8,25--8,26) })],
          PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (2,0--9,23), { LeadingKeyword = Module (2,0--2,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [LineComment (1,0--1,45)] }, set []))

(8,4)-(8,10) parse error Modules cannot be nested inside types. Define modules at module or namespace level.
