ImplFile
  (ParsedImplFileInput
     ("/root/Nullness/GenericFunctionReturnTypeNotStructNull.fs", false,
      QualifiedNameOfFile GenericFunctionReturnTypeNotStructNull, [], [],
      [SynModuleOrNamespace
         ([GenericFunctionReturnTypeNotStructNull], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([[]], SynArgInfo ([], false, None)), None),
                  LongIdent
                    (SynLongIdent ([myFunc], [], [None]), None, None,
                     Pats [Paren (Const (Unit, (1,10--1,12)), (1,10--1,12))],
                     None, (1,4--1,12)),
                  Some
                    (SynBindingReturnInfo
                       (WithGlobalConstraints
                          (Var (SynTypar (T, None, false), (1,15--1,17)),
                           [WhereTyparIsReferenceType
                              (SynTypar (T, None, false), (1,23--1,38));
                            WhereTyparSupportsNull
                              (SynTypar (T, None, false), (1,43--1,50))],
                           (1,15--1,50)), (1,15--1,50), [],
                        { ColonRange = Some (1,13--1,14) })),
                  Typed
                    (Null (1,53--1,57),
                     WithGlobalConstraints
                       (Var (SynTypar (T, None, false), (1,15--1,17)),
                        [WhereTyparIsReferenceType
                           (SynTypar (T, None, false), (1,23--1,38));
                         WhereTyparSupportsNull
                           (SynTypar (T, None, false), (1,43--1,50))],
                        (1,15--1,50)), (1,53--1,57)), (1,4--1,12), NoneAtLet,
                  { LeadingKeyword = Let (1,0--1,3)
                    InlineKeyword = None
                    EqualsRange = Some (1,51--1,52) })], (1,0--1,57))],
          PreXmlDocEmpty, [], None, (1,0--2,0), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))
