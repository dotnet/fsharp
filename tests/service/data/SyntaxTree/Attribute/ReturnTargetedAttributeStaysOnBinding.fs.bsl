ImplFile
  (ParsedImplFileInput
     ("/root/Attribute/ReturnTargetedAttributeStaysOnBinding.fs", false,
      QualifiedNameOfFile M, [],
      [SynModuleOrNamespace
         ([M], false, NamedModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false,
                  [{ Attributes =
                      [{ TypeName = SynLongIdent ([Struct], [], [None])
                         ArgExpr = Const (Unit, (3,10--3,16))
                         Target = Some return
                         AppliesToGetterAndSetter = false
                         Range = (3,2--3,16) }]
                     Range = (3,0--3,18) }],
                  PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None,
                     SynValInfo
                       ([[SynArgInfo ([], false, Some x)]],
                        SynArgInfo ([], false, None)), None),
                  LongIdent
                    (SynLongIdent
                       ([|Foo|_|], [],
                        [Some (HasParenthesis ((4,4--4,5), (4,12--4,13)))]),
                     None, None,
                     Pats
                       [Paren
                          (Typed
                             (Named
                                (SynIdent (x, None), false, None, (4,15--4,16)),
                              LongIdent (SynLongIdent ([int], [], [None])),
                              (4,15--4,21)), (4,14--4,22))], None, (4,4--4,22)),
                  None, Ident ValueNone, (3,0--4,22), NoneAtLet,
                  { LeadingKeyword = Let (4,0--4,3)
                    InlineKeyword = None
                    EqualsRange = Some (4,23--4,24) })], (3,0--4,34),
              { InKeyword = None })],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,34), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
