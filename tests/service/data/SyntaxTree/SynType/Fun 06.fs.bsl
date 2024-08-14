ImplFile
  (ParsedImplFileInput
     ("/root/SynType/Fun 06.fs", false, QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Wild (3,4--3,5),
                  Some
                    (SynBindingReturnInfo
                       (Fun
                          (LongIdent (SynLongIdent ([a], [], [None])),
                           LongIdent (SynLongIdent ([b], [], [None])),
                           (3,7--3,13), { ArrowRange = (3,9--3,11) }),
                        (3,7--3,13), [], { ColonRange = Some (3,5--3,6) })),
                  Typed
                    (Const (Unit, (3,16--3,18)),
                     Fun
                       (LongIdent (SynLongIdent ([a], [], [None])),
                        LongIdent (SynLongIdent ([b], [], [None])), (3,7--3,13),
                        { ArrowRange = (3,9--3,11) }), (3,16--3,18)), (3,4--3,5),
                  Yes (3,0--3,18), { LeadingKeyword = Let (3,0--3,3)
                                     InlineKeyword = None
                                     EqualsRange = Some (3,14--3,15) })],
              (3,0--3,18))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,18), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
