ImplFile
  (ParsedImplFileInput
     ("/root/TypeParameters/Attribute.fs", false, QualifiedNameOfFile Attribute,
      [], [],
      [SynModuleOrNamespace
         ([Attribute], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([],
                     Some
                       (PostfixList
                          ([SynTyparDecl
                              ([], SynTypar (S, HeadType, false), [],
                               { AmpersandRanges = [] })], [], (1,14--1,18))),
                     [], [SomeThing],
                     PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                     true, None, (1,5--1,14)),
                  ObjectModel (Class, [], (1,21--1,30)), [], None, (1,5--1,30),
                  { LeadingKeyword = Type (1,0--1,4)
                    EqualsRange = Some (1,19--1,20)
                    WithKeyword = None })], (1,0--1,30));
           Attributes
             ([{ Attributes =
                  [{ TypeName = SynLongIdent ([AGoodAttribute], [], [None])
                     TypeArgs =
                      [Var (SynTypar (A, HeadType, false), (3,17--3,19));
                       App
                         (LongIdent (SynLongIdent ([SomeThing], [], [None])),
                          Some (3,30--3,31),
                          [LongIdent (SynLongIdent ([int], [], [None]))], [],
                          Some (3,34--3,35), false, (3,21--3,35))]
                     ArgExpr = Const (Unit, (3,2--3,16))
                     Target = None
                     AppliesToGetterAndSetter = false
                     Range = (3,2--3,16) }]
                 Range = (3,0--3,39) }], (3,0--3,39));
           Expr (Do (Const (Unit, (4,2--4,4)), (4,0--4,4)), (4,0--4,4))],
          PreXmlDocEmpty, [], None, (1,0--4,4), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))
