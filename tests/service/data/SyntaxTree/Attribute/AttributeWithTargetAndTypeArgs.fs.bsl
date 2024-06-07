ImplFile
  (ParsedImplFileInput
     ("/root/Attribute/AttributeWithTargetAndTypeArgs.fs", false,
      QualifiedNameOfFile AttributeWithTargetAndTypeArgs, [], [],
      [SynModuleOrNamespace
         ([AttributeWithTargetAndTypeArgs], false, AnonModule,
          [Attributes
             ([{ Attributes =
                  [{ TypeName = SynLongIdent ([MyAttribute], [], [None])
                     TypeArgs =
                      [App
                         (LongIdent (SynLongIdent ([List], [], [None])),
                          Some (2,27--2,28),
                          [LongIdent (SynLongIdent ([int], [], [None]))], [],
                          Some (2,31--2,32), false, (2,23--2,32))]
                     ArgExpr = Const (Unit, (2,11--2,33))
                     Target = Some assembly
                     AppliesToGetterAndSetter = false
                     Range = (2,2--2,33) }]
                 Range = (2,0--2,35) }], (2,0--2,35));
           Expr (Do (Const (Unit, (3,3--3,5)), (3,0--3,5)), (3,0--3,5))],
          PreXmlDocEmpty, [], None, (2,0--3,5), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))
