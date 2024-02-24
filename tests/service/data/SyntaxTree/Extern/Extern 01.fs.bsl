ImplFile
  (ParsedImplFileInput
     ("/root/Extern/Extern 01.fs", false, QualifiedNameOfFile Extern 01, [], [],
      [SynModuleOrNamespace
         ([Extern 01], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false,
                  [{ Attributes =
                      [{ TypeName = SynLongIdent ([DllImport], [], [None])
                         ArgExpr =
                          Paren
                            (Tuple
                               (false,
                                [Const
                                   (String
                                      ("__Internal", Verbatim, (1,12--1,25)),
                                    (1,12--1,25));
                                 App
                                   (NonAtomic, false,
                                    App
                                      (NonAtomic, true,
                                       LongIdent
                                         (false,
                                          SynLongIdent
                                            ([op_Equality], [],
                                             [Some (OriginalNotation "=")]),
                                          None, (1,45--1,46)),
                                       Ident CallingConvention, (1,27--1,46)),
                                    LongIdent
                                      (false,
                                       SynLongIdent
                                         ([CallingConvention; Cdecl],
                                          [(1,64--1,65)], [None; None]), None,
                                       (1,47--1,70)), (1,27--1,70))],
                                [(1,25--1,26)], (1,12--1,70)), (1,11--1,12),
                             Some (1,70--1,71), (1,11--1,71))
                         Target = None
                         AppliesToGetterAndSetter = false
                         Range = (1,2--1,71) }]
                     Range = (1,0--1,73) }],
                  PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None,
                     SynValInfo
                       ([[SynArgInfo ([], false, Some inRef);
                          SynArgInfo ([], false, Some outParentRef)]],
                        SynArgInfo ([], false, None)), None),
                  LongIdent
                    (SynLongIdent ([GetParent], [], [None]), None,
                     Some (SynValTyparDecls (None, false)),
                     Pats
                       [Tuple
                          (false,
                           [Attrib
                              (Typed
                                 (Named
                                    (SynIdent (inRef, None), false, None,
                                     (2,30--2,49)),
                                  App
                                    (LongIdent
                                       (SynLongIdent
                                          ([System; IntPtr], [(2,36--2,37)],
                                           [None; None])), None, [], [], None,
                                     false, (2,30--2,43)), (2,30--2,49)), [],
                               (2,30--2,49));
                            Attrib
                              (Typed
                                 (Named
                                    (SynIdent (outParentRef, None), false, None,
                                     (2,51--2,69)),
                                  App
                                    (LongIdent
                                       (SynLongIdent ([byref], [], [None])),
                                     None, [], [], None, false, (2,51--2,56)),
                                  (2,51--2,69)), [], (2,51--2,69))],
                           [(2,49--2,50)], (2,29--2,30))], None, (2,19--2,28)),
                  Some
                    (SynBindingReturnInfo
                       (App
                          (LongIdent (SynLongIdent ([ReturnCode], [], [None])),
                           None, [], [], None, false, (2,7--2,17)), (2,7--2,17),
                        [], { ColonRange = None })),
                  Typed
                    (App
                       (NonAtomic, false, Ident failwith,
                        Const
                          (String
                             ("extern was not given a DllImport attribute",
                              Regular, (2,69--2,70)), (2,69--2,70)), (2,0--2,70)),
                     App
                       (LongIdent (SynLongIdent ([ReturnCode], [], [None])),
                        None, [], [], None, false, (2,7--2,17)), (2,0--2,70)),
                  (1,0--2,70), NoneAtInvisible,
                  { LeadingKeyword = Extern (2,0--2,6)
                    InlineKeyword = None
                    EqualsRange = None })], (1,0--2,70))], PreXmlDocEmpty, [],
          None, (1,0--2,70), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(1,0)-(2,0) parse warning The declarations in this file will be placed in an implicit module 'Extern 01' based on the file name 'Extern 01.fs'. However this is not a valid F# identifier, so the contents will not be accessible from other files. Consider renaming the file or adding a 'module' or 'namespace' declaration at the top of the file.
