module Test

type Opcode =
  | AAA = 0
  | AAD = 1
  | AAM = 2
  | AAS = 3
  | ADC = 4
  | ADCX = 5
  | ADD = 6
  | ADDPD = 7
  | ADDPS = 8
  | ADDSD = 9
  | ADDSS = 10
  | ADDSUBPD = 11
  | ADDSUBPS = 12
  | ADOX = 13
  | AESDEC = 14
  | AESDECLAST = 15
  | AESENC = 16
  | AESENCLAST = 17
  | AESIMC = 18
  | AESKEYGENASSIST = 19
  | AND = 20
  | ANDN = 21
  | ANDNPD = 22
  | ANDNPS = 23
  | ANDPD = 24
  | ANDPS = 25
  | ARPL = 26
  | BEXTR = 27
  | BLENDPD = 28
  | BLENDPS = 29
  | BLENDVPD = 30
  | BLENDVPS = 31
  | BLSI = 32
  | BLSMSK = 33
  | BLSR = 34
  | BNDCL = 35
  | BNDCN = 36
  | BNDCU = 37
  | BNDLDX = 38
  | BNDMK = 39
  | BNDMOV = 40
  | BNDSTX = 41
  | BOUND = 42
  | BSF = 43
  | BSR = 44
  | BSWAP = 45
  | BT = 46
  | BTC = 47
  | BTR = 48
  | BTS = 49
  | BZHI = 50
  | CALLFar = 51
  | CALLNear = 52
  | CBW = 53
  | CDQ = 54
  | CDQE = 55
  | CLAC = 56
  | CLC = 57
  | CLD = 58
  | CLFLUSH = 59
  | CLFLUSHOPT = 60
  | CLI = 61
  | CLRSSBSY = 62
  | CLTS = 63
  | CMC = 64
  | CMOVA = 65
  | CMOVAE = 66
  | CMOVB = 67
  | CMOVBE = 68
  | CMOVC = 69
  | CMOVG = 70
  | CMOVGE = 71
  | CMOVL = 72
  | CMOVLE = 73
  | CMOVNC = 74
  
  | CMOVNO = 75
  
  | CMOVNP = 76
  
  | CMOVNS = 77
  
  | CMOVNZ = 78
  
  | CMOVO = 79
  
  | CMOVP = 80
  
  | CMOVS = 81
  
  | CMOVZ = 82
  
  | CMP = 83
  
  | CMPPD = 84
  
  | CMPPS = 85
  
  | CMPSB = 86
  
  | CMPSD = 87
  
  | CMPSQ = 88
  
  | CMPSS = 89
  
  | CMPSW = 90
  
  | CMPXCHG = 91
  
  | CMPXCHG16B = 92
  
  | CMPXCHG8B = 93
  
  | COMISD = 94
  
  | COMISS = 95
  
  | CPUID = 96
  
  | CQO = 97
  
  | CRC32 = 98
  
  | CVTDQ2PD = 99
  
  | CVTDQ2PS = 100
  
  | CVTPD2DQ = 101
  
  | CVTPD2PI = 102
  
  | CVTPD2PS = 103
  
  | CVTPI2PD = 104
  
  | CVTPI2PS = 105
  
  | CVTPS2DQ = 106
  
  | CVTPS2PD = 107
  
  | CVTPS2PI = 108
  
  | CVTSD2SI = 109
  
  | CVTSD2SS = 110
  
  | CVTSI2SD = 111
  
  | CVTSI2SS = 112
  
  | CVTSS2SD = 113
  
  | CVTSS2SI = 114
  
  | CVTTPD2DQ = 115
  
  | CVTTPD2PI = 116
  
  | CVTTPS2DQ = 117
  
  | CVTTPS2PI = 118
  
  | CVTTSD2SI = 119
  
  | CVTTSS2SI = 120
  
  | CWD = 121
  
  | CWDE = 122
  
  | DAA = 123
  
  | DAS = 124
  
  | DEC = 125
  
  | DIV = 126
  
  | DIVPD = 127
  
  | DIVPS = 128
  
  | DIVSD = 129
  
  | DIVSS = 130
  
  | DPPD = 131
  
  | DPPS = 132
  
  | EMMS = 133
  
  | ENCLS = 134
  
  | ENCLU = 135
  
  | ENDBR32 = 136
  
  | ENDBR64 = 137
  
  | ENTER = 138
  
  | EXTRACTPS = 139
  
  | F2XM1 = 140
  
  | FABS = 141
  
  | FADD = 142
  
  | FADDP = 143
  
  | FBLD = 144
  
  | FBSTP = 145
  
  | FCHS = 146
  
  | FCLEX = 147
  
  | FCMOVB = 148
  
  | FCMOVBE = 149
  
  | FCMOVE = 150
  
  | FCMOVNB = 151
  
  | FCMOVNBE = 152
  
  | FCMOVNE = 153
  
  | FCMOVNU = 154
  
  | FCMOVU = 155
  
  | FCOM = 156
  
  | FCOMI = 157
  
  | FCOMIP = 158
  
  | FCOMP = 159
  
  | FCOMPP = 160
  
  | FCOS = 161
  
  | FDECSTP = 162
  
  | FDIV = 163
  
  | FDIVP = 164
  
  | FDIVR = 165
  
  | FDIVRP = 166
  
  | FFREE = 167
  
  | FIADD = 168
  
  | FICOM = 169
  
  | FICOMP = 170
  
  | FIDIV = 171
  
  | FIDIVR = 172
  
  | FILD = 173
  
  | FIMUL = 174
  
  | FINCSTP = 175
  
  | FINIT = 176
  
  | FIST = 177
  
  | FISTP = 178
  
  | FISTTP = 179
  
  | FISUB = 180
  
  | FISUBR = 181
  
  | FLD = 182
  
  | FLD1 = 183
  
  | FLDCW = 184
  
  | FLDENV = 185
  
  | FLDL2E = 186
  
  | FLDL2T = 187
  
  | FLDLG2 = 188
  
  | FLDLN2 = 189
  
  | FLDPI = 190
  
  | FLDZ = 191
  
  | FMUL = 192
  
  | FMULP = 193
  
  | FNCLEX = 194
  
  | FNINIT = 195
  
  | FNOP = 196
  
  | FNSAVE = 197
  
  | FNSTCW = 198
  
  | FNSTENV = 199
  
  | FNSTSW = 200
  
  | FPATAN = 201
  
  | FPREM = 202
  
  | FPREM1 = 203
  
  | FPTAN = 204
  
  | FRNDINT = 205
  
  | FRSTOR = 206
  
  | FSAVE = 207
  
  | FSCALE = 208
  
  | FSIN = 209
  
  | FSINCOS = 210
  
  | FSQRT = 211
  
  | FST = 212
  
  | FSTCW = 213
  
  | FSTENV = 214
  
  | FSTP = 215
  
  | FSTSW = 216
  
  | FSUB = 217
  
  | FSUBP = 218
  
  | FSUBR = 219
  
  | FSUBRP = 220
  
  | FTST = 221
  
  | FUCOM = 222
  
  | FUCOMI = 223
  
  | FUCOMIP = 224
  
  | FUCOMP = 225
  
  | FUCOMPP = 226
  
  | FWAIT = 227
  
  | FXAM = 228
  
  | FXCH = 229
  
  | FXRSTOR = 230
  
  | FXRSTOR64 = 231
  
  | FXSAVE = 232
  
  | FXSAVE64 = 233
  
  | FXTRACT = 234
  
  | FYL2X = 235
  
  | FYL2XP1 = 236
  
  | GETSEC = 237
  
  | GF2P8AFFINEINVQB = 238
  
  | GF2P8AFFINEQB = 239
  
  | GF2P8MULB = 240
  
  | HADDPD = 241
  
  | HADDPS = 242
  
  | HLT = 243
  
  | HSUBPD = 244
  
  | HSUBPS = 245
  
  | IDIV = 246
  
  | IMUL = 247
  
  | IN = 248
  
  | INC = 249
  
  | INCSSPD = 250
  
  | INCSSPQ = 251
  
  | INS = 252
  
  | INSB = 253
  
  | INSD = 254
  
  | INSERTPS = 255
  
  | INSW = 256
  
  | INT = 257
  
  | INT3 = 258
  
  | INTO = 259
  
  | INVD = 260
  
  | INVEPT = 261
  
  | INVLPG = 262
  
  | INVPCID = 263
  
  | INVVPID = 264
  
  | IRET = 265
  
  | IRETD = 266
  
  | IRETQ = 267
  
  | IRETW = 268
  
  | JAE = 269
  | JNC = 269
  | JNB = 269
  
  | JC = 270
  | JNAE = 270
  | JB = 270
  
  | JCXZ = 271
  
  | JECXZ = 272
  
  | JGE = 273
  | JNL = 273
  
  | JMPFar = 274
  
  | JMPNear = 275
  
  | JNA = 276
  | JBE = 276
  
  | JNBE = 277
  | JA = 277
  
  | JNG = 278
  | JLE = 278
  
  | JNGE = 279
  | JL = 279
  
  | JNLE = 280
  | JG = 280
  
  | JNO = 281
  
  | JNS = 282
  
  | JNZ = 283
  | JNE = 283
  
  | JO = 284
  
  | JP = 285
  | JPE = 285
  
  | JPO = 286
  | JNP = 286
  
  | JRCXZ = 287
  
  | JS = 288
  
  | JZ = 289
  | JE = 289
  
  | KADDB = 290
  
  | KADDD = 291
  
  | KADDQ = 292
  
  | KADDW = 293
  
  | KANDB = 294
  
  | KANDD = 295
  
  | KANDNB = 296
  
  | KANDND = 297
  
  | KANDNQ = 298
  
  | KANDNW = 299
  
  | KANDQ = 300
  
  | KANDW = 301
  
  | KMOVB = 302
  
  | KMOVD = 303
  
  | KMOVQ = 304
  
  | KMOVW = 305
  
  | KNOTB = 306
  
  | KNOTD = 307
  
  | KNOTQ = 308
  
  | KNOTW = 309
  
  | KORB = 310
  
  | KORD = 311
  
  | KORQ = 312
  
  | KORTESTB = 313
  
  | KORTESTD = 314
  
  | KORTESTQ = 315
  
  | KORTESTW = 316
  
  | KORW = 317
  
  | KSHIFTLB = 318
  
  | KSHIFTLD = 319
  
  | KSHIFTLQ = 320
  
  | KSHIFTLW = 321
  
  | KSHIFTRB = 322
  
  | KSHIFTRD = 323
  
  | KSHIFTRQ = 324
  
  | KSHIFTRW = 325
  
  | KTESTB = 326
  
  | KTESTD = 327
  
  | KTESTQ = 328
  
  | KTESTW = 329
  
  | KUNPCKBW = 330
  
  | KUNPCKDQ = 331
  
  | KUNPCKWD = 332
  
  | KXNORB = 333
  
  | KXNORD = 334
  
  | KXNORQ = 335
  
  | KXNORW = 336
  
  | KXORB = 337
  
  | KXORD = 338
  
  | KXORQ = 339
  
  | KXORW = 340
  
  | LAHF = 341
  
  | LAR = 342
  
  | LDDQU = 343
  
  | LDMXCSR = 344
  
  | LDS = 345
  
  | LEA = 346
  
  | LEAVE = 347
  
  | LES = 348
  
  | LFENCE = 349
  
  | LFS = 350
  
  | LGDT = 351
  
  | LGS = 352
  
  | LIDT = 353
  
  | LLDT = 354
  
  | LMSW = 355
  
  | LOCK = 356
  
  | LODSB = 357
  
  | LODSD = 358
  
  | LODSQ = 359
  
  | LODSW = 360
  
  | LOOP = 361
  
  | LOOPE = 362
  
  | LOOPNE = 363
  
  | LSL = 364
  
  | LSS = 365
  
  | LTR = 366
  
  | LZCNT = 367
  
  | MASKMOVDQU = 368
  
  | MASKMOVQ = 369
  
  | MAXPD = 370
  
  | MAXPS = 371
  
  | MAXSD = 372
  
  | MAXSS = 373
  
  | MFENCE = 374
  
  | MINPD = 375
  
  | MINPS = 376
  
  | MINSD = 377
  
  | MINSS = 378
  
  | MONITOR = 379
  
  | MOV = 380
  
  | MOVAPD = 381
  
  | MOVAPS = 382
  
  | MOVBE = 383
  
  | MOVD = 384
  
  | MOVDDUP = 385
  
  | MOVDQ2Q = 386
  
  | MOVDQA = 387
  
  | MOVDQU = 388
  
  | MOVHLPS = 389
  
  | MOVHPD = 390
  
  | MOVHPS = 391
  
  | MOVLHPS = 392
  
  | MOVLPD = 393
  
  | MOVLPS = 394
  
  | MOVMSKPD = 395
  
  | MOVMSKPS = 396
  
  | MOVNTDQ = 397
  
  | MOVNTDQA = 398
  
  | MOVNTI = 399
  
  | MOVNTPD = 400
  
  | MOVNTPS = 401
  
  | MOVNTQ = 402
  
  | MOVQ = 403
  
  | MOVQ2DQ = 404
  
  | MOVSB = 405
  
  | MOVSD = 406
  
  | MOVSHDUP = 407
  
  | MOVSLDUP = 408
  
  | MOVSQ = 409
  
  | MOVSS = 410
  
  | MOVSW = 411
  
  | MOVSX = 412
  
  | MOVSXD = 413
  
  | MOVUPD = 414
  
  | MOVUPS = 415
  
  | MOVZX = 416
  
  | MPSADBW = 417
  
  | MUL = 418
  
  | MULPD = 419
  
  | MULPS = 420
  
  | MULSD = 421
  
  | MULSS = 422
  
  | MULX = 423
  
  | MWAIT = 424
  
  | NEG = 425
  
  | NOP = 426
  
  | NOT = 427
  
  | OR = 428
  
  | ORPD = 429
  
  | ORPS = 430
  
  | OUT = 431
  
  | OUTS = 432
  
  | OUTSB = 433
  
  | OUTSD = 434
  
  | OUTSW = 435
  
  | PABSB = 436
  
  | PABSD = 437
  
  | PABSW = 438
  
  | PACKSSDW = 439
  
  | PACKSSWB = 440
  
  | PACKUSDW = 441
  
  | PACKUSWB = 442
  
  | PADDB = 443
  
  | PADDD = 444
  
  | PADDQ = 445
  
  | PADDSB = 446
  
  | PADDSW = 447
  
  | PADDUSB = 448
  
  | PADDUSW = 449
  
  | PADDW = 450
  
  | PALIGNR = 451
  
  | PAND = 452
  
  | PANDN = 453
  
  | PAUSE = 454
  
  | PAVGB = 455
  
  | PAVGW = 456
  
  | PBLENDVB = 457
  
  | PBLENDW = 458
  
  | PCLMULQDQ = 459
  
  | PCMPEQB = 460
  
  | PCMPEQD = 461
  
  | PCMPEQQ = 462
  
  | PCMPEQW = 463
  
  | PCMPESTRI = 464
  
  | PCMPESTRM = 465
  
  | PCMPGTB = 466
  
  | PCMPGTD = 467
  
  | PCMPGTQ = 468
  
  | PCMPGTW = 469
  
  | PCMPISTRI = 470
  
  | PCMPISTRM = 471
  
  | PDEP = 472
  
  | PEXT = 473
  
  | PEXTRB = 474
  
  | PEXTRD = 475
  
  | PEXTRQ = 476
  
  | PEXTRW = 477
  
  | PHADDD = 478
  
  | PHADDSW = 479
  
  | PHADDW = 480
  
  | PHMINPOSUW = 481
  
  | PHSUBD = 482
  
  | PHSUBSW = 483
  
  | PHSUBW = 484
  
  | PINSRB = 485
  
  | PINSRD = 486
  
  | PINSRQ = 487
  
  | PINSRW = 488
  
  | PMADDUBSW = 489
  
  | PMADDWD = 490
  
  | PMAXSB = 491
  
  | PMAXSD = 492
  
  | PMAXSW = 493
  
  | PMAXUB = 494
  
  | PMAXUD = 495
  
  | PMAXUW = 496
  
  | PMINSB = 497
  
  | PMINSD = 498
  
  | PMINSW = 499
  
  | PMINUB = 500
  
  | PMINUD = 501
  
  | PMINUW = 502
  
  | PMOVMSKB = 503
  
  | PMOVSXBD = 504
  
  | PMOVSXBQ = 505
  
  | PMOVSXBW = 506
  
  | PMOVSXDQ = 507
  
  | PMOVSXWD = 508
  
  | PMOVSXWQ = 509
  
  | PMOVZXBD = 510
  
  | PMOVZXBQ = 511
  
  | PMOVZXBW = 512
  
  | PMOVZXDQ = 513
  
  | PMOVZXWD = 514
  
  | PMOVZXWQ = 515
  
  | PMULDQ = 516
  
  | PMULHRSW = 517
  
  | PMULHUW = 518
  
  | PMULHW = 519
  
  | PMULLD = 520
  
  | PMULLW = 521
  
  | PMULUDQ = 522
  
  | POP = 523
  
  | POPA = 524
  
  | POPAD = 525
  
  | POPCNT = 526
  
  | POPF = 527
  
  | POPFD = 528
  
  | POPFQ = 529
  
  | POR = 530
  
  | PREFETCHNTA = 531
  
  | PREFETCHT0 = 532
  
  | PREFETCHT1 = 533
  
  | PREFETCHT2 = 534
  
  | PREFETCHW = 535
  
  | PREFETCHWT1 = 536
  
  | PSADBW = 537
  
  | PSHUFB = 538
  
  | PSHUFD = 539
  
  | PSHUFHW = 540
  
  | PSHUFLW = 541
  
  | PSHUFW = 542
  
  | PSIGNB = 543
  
  | PSIGND = 544
  
  | PSIGNW = 545
  
  | PSLLD = 546
  
  | PSLLDQ = 547
  
  | PSLLQ = 548
  
  | PSLLW = 549
  
  | PSRAD = 550
  
  | PSRAW = 551
  
  | PSRLD = 552
  
  | PSRLDQ = 553
  
  | PSRLQ = 554
  
  | PSRLW = 555
  
  | PSUBB = 556
  
  | PSUBD = 557
  
  | PSUBQ = 558
  
  | PSUBSB = 559
  
  | PSUBSW = 560
  
  | PSUBUSB = 561
  
  | PSUBUSW = 562
  
  | PSUBW = 563
  
  | PTEST = 564
  
  | PUNPCKHBW = 565
  
  | PUNPCKHDQ = 566
  
  | PUNPCKHQDQ = 567
  
  | PUNPCKHWD = 568
  
  | PUNPCKLBW = 569
  
  | PUNPCKLDQ = 570
  
  | PUNPCKLQDQ = 571
  
  | PUNPCKLWD = 572
  
  | PUSH = 573
  
  | PUSHA = 574
  
  | PUSHAD = 575
  
  | PUSHF = 576
  
  | PUSHFD = 577
  
  | PUSHFQ = 578
  
  | PXOR = 579
  
  | RCL = 580
  
  | RCPPS = 581
  
  | RCPSS = 582
  
  | RCR = 583
  
  | RDFSBASE = 584
  
  | RDGSBASE = 585
  
  | RDMSR = 586
  
  | RDPKRU = 587
  
  | RDPMC = 588
  
  | RDRAND = 589
  
  | RDSEED = 590
  
  | RDSSPD = 591
  
  | RDSSPQ = 592
  
  | RDTSC = 593
  
  | RDTSCP = 594
  
  | REP = 595
  
  | REPE = 596
  
  | REPNE = 597
  
  | REPNZ = 598
  
  | REPZ = 599
  
  | RETFar = 600
  
  | RETFarImm = 601
  
  | RETNear = 602
  
  | RETNearImm = 603
  
  | ROL = 604
  
  | ROR = 605
  
  | RORX = 606
  
  | ROUNDPD = 607
  
  | ROUNDPS = 608
  
  | ROUNDSD = 609
  
  | ROUNDSS = 610
  
  | RSM = 611
  
  | RSQRTPS = 612
  
  | RSQRTSS = 613
  
  | RSTORSSP = 614
  
  | SAHF = 615
  
  | SAR = 616
  
  | SARX = 617
  
  | SAVEPREVSSP = 618
  
  | SBB = 619
  
  | SCASB = 620
  
  | SCASD = 621
  
  | SCASQ = 622
  
  | SCASW = 623
  
  | SETA = 624
  
  | SETB = 625
  
  | SETBE = 626
  
  | SETG = 627
  
  | SETL = 628
  
  | SETLE = 629
  
  | SETNB = 630
  
  | SETNL = 631
  
  | SETNO = 632
  
  | SETNP = 633
  
  | SETNS = 634
  
  | SETNZ = 635
  
  | SETO = 636
  
  | SETP = 637
  
  | SETS = 638
  
  | SETSSBSY = 639
  
  | SETZ = 640
  
  | SFENCE = 641
  
  | SGDT = 642
  
  | SHA1MSG1 = 643
  
  | SHA1MSG2 = 644
  
  | SHA1NEXTE = 645
  
  | SHA1RNDS4 = 646
  
  | SHA256MSG1 = 647
  
  | SHA256MSG2 = 648
  
  | SHA256RNDS2 = 649
  
  | SHL = 650
  
  | SHLD = 651
  
  | SHLX = 652
  
  | SHR = 653
  
  | SHRD = 654
  
  | SHRX = 655
  
  | SHUFPD = 656
  
  | SHUFPS = 657
  
  | SIDT = 658
  
  | SLDT = 659
  
  | SMSW = 660
  
  | SQRTPD = 661
  
  | SQRTPS = 662
  
  | SQRTSD = 663
  
  | SQRTSS = 664
  
  | STAC = 665
  
  | STC = 666
  
  | STD = 667
  
  | STI = 668
  
  | STMXCSR = 669
  
  | STOSB = 670
  
  | STOSD = 671
  
  | STOSQ = 672
  
  | STOSW = 673
  
  | STR = 674
  
  | SUB = 675
  
  | SUBPD = 676
  
  | SUBPS = 677
  
  | SUBSD = 678
  
  | SUBSS = 679
  
  | SWAPGS = 680
  
  | SYSCALL = 681
  
  | SYSENTER = 682
  
  | SYSEXIT = 683
  
  | SYSRET = 684
  
  | TEST = 685
  
  | TZCNT = 686
  
  | UCOMISD = 687
  
  | UCOMISS = 688
  
  | UD = 689
  
  | UD2 = 690
  
  | UNPCKHPD = 691
  
  | UNPCKHPS = 692
  
  | UNPCKLPD = 693
  
  | UNPCKLPS = 694
  
  | VADDPD = 695
  
  | VADDPS = 696
  
  | VADDSD = 697
  
  | VADDSS = 698
  
  | VALIGND = 699
  
  | VALIGNQ = 700
  
  | VANDNPD = 701
  
  | VANDNPS = 702
  
  | VANDPD = 703
  
  | VANDPS = 704
  
  | VBLENDMPD = 705
  
  | VBLENDMPS = 706
  
  | VBLENDVPD = 707
  
  | VBROADCASTI128 = 708
  
  | VBROADCASTSD = 709
  
  | VBROADCASTSS = 710
  
  | VCMPPD = 711
  
  | VCMPPS = 712
  
  | VCMPSD = 713
  
  | VCMPSS = 714
  
  | VCOMISD = 715
  
  | VCOMISS = 716
  
  | VCOMPRESSPD = 717
  
  | VCOMPRESSPS = 718
  
  | VCVTDQ2PD = 719
  
  | VCVTDQ2PS = 720
  
  | VCVTPD2PS = 721
  
  | VCVTPD2QQ = 722
  
  | VCVTPD2UDQ = 723
  
  | VCVTPD2UQQ = 724
  
  | VCVTPH2PS = 725
  
  | VCVTPS2PD = 726
  
  | VCVTPS2PH = 727
  
  | VCVTPS2QQ = 728
  
  | VCVTPS2UDQ = 729
  
  | VCVTPS2UQQ = 730
  
  | VCVTQQ2PD = 731
  
  | VCVTQQ2PS = 732
  
  | VCVTSD2SI = 733
  
  | VCVTSD2SS = 734
  
  | VCVTSD2USI = 735
  
  | VCVTSI2SD = 736
  
  | VCVTSI2SS = 737
  
  | VCVTSS2SD = 738
  
  | VCVTSS2SI = 739
  
  | VCVTSS2USI = 740
  
  | VCVTTPD2DQ = 741
  
  | VCVTTPD2QQ = 742
  
  | VCVTTPD2UDQ = 743
  
  | VCVTTPD2UQQ = 744
  
  | VCVTTPS2QQ = 745
  
  | VCVTTPS2UDQ = 746
  
  | VCVTTPS2UQQ = 747
  
  | VCVTTSD2SI = 748
  
  | VCVTTSD2USI = 749
  
  | VCVTTSS2SI = 750
  
  | VCVTTSS2USI = 751
  
  | VCVTUDQ2PD = 752
  
  | VCVTUDQ2PS = 753
  
  | VCVTUQQ2PD = 754
  
  | VCVTUQQ2PS = 755
  
  | VCVTUSI2USD = 756
  
  | VCVTUSI2USS = 757
  
  | VDBPSADBW = 758
  
  | VDIVPD = 759
  
  | VDIVPS = 760
  
  | VDIVSD = 761
  
  | VDIVSS = 762
  
  | VERR = 763
  
  | VERW = 764
  
  | VEXP2PD = 765
  
  | VEXP2PS = 766
  
  | VEXP2SD = 767
  
  | VEXP2SS = 768
  
  | VEXPANDPD = 769
  
  | VEXPANDPS = 770
  
  | VEXTRACTF128 = 771
  
  | VEXTRACTF32X4 = 772
  
  | VEXTRACTF32X8 = 773
  
  | VEXTRACTF64X2 = 774
  
  | VEXTRACTF64X4 = 775
  
  | VEXTRACTI32X4 = 776
  
  | VEXTRACTI32X8 = 777
  
  | VEXTRACTI64X2 = 778
  
  | VEXTRACTI64X4 = 779
  
  | VFIXUPIMMPD = 780
  
  | VFIXUPIMMPS = 781
  
  | VFIXUPIMMSD = 782
  
  | VFIXUPIMMSS = 783
  
  | VFMADD132PD = 784
  
  | VFMADD132SD = 785
  
  | VFMADD132SS = 786
  
  | VFMADD213PD = 787
  
  | VFMADD213PS = 788
  
  | VFMADD213SD = 789
  
  | VFMADD213SS = 790
  
  | VFMADD231PD = 791
  
  | VFMADD231SD = 792
  
  | VFMADD231SS = 793
  
  | VFMSUB132SS = 794
  
  | VFMSUB213PD = 795
  
  | VFMSUB213SD = 796
  
  | VFMSUB231PD = 797
  
  | VFMSUB231SD = 798
  
  | VFNMADD132PD = 799
  
  | VFNMADD132SD = 800
  
  | VFNMADD213PD = 801
  
  | VFNMADD213SD = 802
  
  | VFNMADD231PD = 803
  
  | VFNMADD231SD = 804
  
  | VFPCLASSPD = 805
  
  | VFPCLASSPS = 806
  
  | VFPCLASSSD = 807
  
  | VFPCLASSSS = 808
  
  | VGATHERDPS = 809
  
  | VGATHERQPD = 810
  
  | VGETEXPPD = 811
  
  | VGETEXPPS = 812
  
  | VGETEXPSD = 813
  
  | VGETEXPSS = 814
  
  | VGETMANTPD = 815
  
  | VGETMANTPS = 816
  
  | VGETMANTSD = 817
  
  | VGETMANTSS = 818
  
  | VINSERTF128 = 819
  
  | VINSERTF32X4 = 820
  
  | VINSERTF64X2 = 821
  
  | VINSERTF64X4 = 822
  
  | VINSERTI128 = 823
  
  | VINSERTI32X8 = 824
  
  | VINSERTI64X2 = 825
  
  | VINSERTI64X4 = 826
  
  | VLDDQU = 827
  
  | VMAXPD = 828
  
  | VMAXPS = 829
  
  | VMAXSD = 830
  
  | VMAXSS = 831
  
  | VMCALL = 832
  
  | VMCLEAR = 833
  
  | VMFUNC = 834
  
  | VMINPD = 835
  
  | VMINPS = 836
  
  | VMINSD = 837
  
  | VMINSS = 838
  
  | VMLAUNCH = 839
  
  | VMOVAPD = 840
  
  | VMOVAPS = 841
  
  | VMOVD = 842
  
  | VMOVDDUP = 843
  
  | VMOVDQA = 844
  
  | VMOVDQA32 = 845
  
  | VMOVDQA64 = 846
  
  | VMOVDQU = 847
  
  | VMOVDQU16 = 848
  
  | VMOVDQU32 = 849
  
  | VMOVDQU64 = 850
  
  | VMOVDQU8 = 851
  
  | VMOVHLPS = 852
  
  | VMOVHPD = 853
  
  | VMOVHPS = 854
  
  | VMOVLHPS = 855
  
  | VMOVLPD = 856
  
  | VMOVLPS = 857
  
  | VMOVMSKPD = 858
  
  | VMOVMSKPS = 859
  
  | VMOVNTDQ = 860
  
  | VMOVNTPD = 861
  
  | VMOVNTPS = 862
  
  | VMOVQ = 863
  
  | VMOVSD = 864
  
  | VMOVSHDUP = 865
  
  | VMOVSLDUP = 866
  
  | VMOVSS = 867
  
  | VMOVUPD = 868
  
  | VMOVUPS = 869
  
  | VMPTRLD = 870
  
  | VMPTRST = 871
  
  | VMREAD = 872
  
  | VMRESUME = 873
  
  | VMULPD = 874
  
  | VMULPS = 875
  
  | VMULSD = 876
  
  | VMULSS = 877
  
  | VMWRITE = 878
  
  | VMXOFF = 879
  
  | VMXON = 880
  
  | VORPD = 881
  
  | VORPS = 882
  
  | VPABSB = 883
  
  | VPABSD = 884
  
  | VPABSW = 885
  
  | VPACKSSDW = 886
  
  | VPACKSSWB = 887
  
  | VPACKUSDW = 888
  
  | VPACKUSWB = 889
  
  | VPADDB = 890
  
  | VPADDD = 891
  
  | VPADDQ = 892
  
  | VPADDSB = 893
  
  | VPADDSW = 894
  
  | VPADDUSB = 895
  
  | VPADDUSW = 896
  
  | VPADDW = 897
  
  | VPALIGNR = 898
  
  | VPAND = 899
  
  | VPANDN = 900
  
  | VPAVGB = 901
  
  | VPAVGW = 902
  
  | VPBLENDMB = 903
  
  | VPBLENDMD = 904
  
  | VPBLENDMQ = 905
  
  | VPBLENDMW = 906
  
  | VPBROADCASTB = 907
  
  | VPBROADCASTD = 908
  
  | VPBROADCASTM = 909
  
  | VPBROADCASTQ = 910
  
  | VPBROADCASTW = 911
  
  | VPCMPB = 912
  
  | VPCMPD = 913
  
  | VPCMPEQB = 914
  
  | VPCMPEQD = 915
  
  | VPCMPEQQ = 916
  
  | VPCMPEQW = 917
  
  | VPCMPESTRI = 918
  
  | VPCMPESTRM = 919
  
  | VPCMPGTB = 920
  
  | VPCMPGTD = 921
  
  | VPCMPGTQ = 922
  
  | VPCMPGTW = 923
  
  | VPCMPISTRI = 924
  
  | VPCMPISTRM = 925
  
  | VPCMPQ = 926
  
  | VPCMPW = 927
  
  | VPCMUB = 928
  
  | VPCMUD = 929
  
  | VPCMUQ = 930
  
  | VPCMUW = 931
  
  | VPCOMPRESSD = 932
  
  | VPCOMPRESSQ = 933
  
  | VPCONFLICTD = 934
  
  | VPCONFLICTQ = 935
  
  | VPERMI2B = 936
  
  | VPERMI2D = 937
  
  | VPERMI2PD = 938
  
  | VPERMI2PS = 939
  
  | VPERMI2Q = 940
  
  | VPERMI2W = 941
  
  | VPERMT2D = 942
  
  | VPERMT2PD = 943
  
  | VPERMT2PS = 944
  
  | VPERMT2Q = 945
  
  | VPERMW = 946
  
  | VPEXPANDD = 947
  
  | VPEXPANDQ = 948
  
  | VPEXTRD = 949
  
  | VPEXTRW = 950
  
  | VPGATHERDD = 951
  
  | VPHADDD = 952
  
  | VPHADDSW = 953
  
  | VPHADDW = 954
  
  | VPHMINPOSUW = 955
  
  | VPHSUBD = 956
  
  | VPHSUBSW = 957
  
  | VPHSUBW = 958
  
  | VPINSRB = 959
  
  | VPINSRD = 960
  
  | VPINSRQ = 961
  
  | VPINSRW = 962
  
  | VPLZCNTD = 963
  
  | VPLZCNTQ = 964
  
  | VPMADDWD = 965
  
  | VPMAXSB = 966
  
  | VPMAXSD = 967
  
  | VPMAXSQ = 968
  
  | VPMAXSW = 969
  
  | VPMAXUB = 970
  
  | VPMAXUD = 971
  
  | VPMAXUQ = 972
  
  | VPMAXUW = 973
  
  | VPMINSB = 974
  
  | VPMINSD = 975
  
  | VPMINSQ = 976
  
  | VPMINSW = 977
  
  | VPMINUB = 978
  
  | VPMINUD = 979
  
  | VPMINUQ = 980
  
  | VPMINUW = 981
  
  | VPMOVB2D = 982
  
  | VPMOVB2M = 983
  
  | VPMOVDB = 984
  
  | VPMOVDW = 985
  
  | VPMOVM2B = 986
  
  | VPMOVM2D = 987
  
  | VPMOVM2Q = 988
  
  | VPMOVM2W = 989
  
  | VPMOVMSKB = 990
  
  | VPMOVQ2M = 991
  
  | VPMOVQB = 992
  
  | VPMOVQD = 993
  
  | VPMOVQW = 994
  
  | VPMOVSDB = 995
  
  | VPMOVSDW = 996
  
  | VPMOVSQB = 997
  
  | VPMOVSQD = 998
  
  | VPMOVSQW = 999
  
  | VPMOVSWB = 1000
  
  | VPMOVSXBD = 1001
  
  | VPMOVSXBQ = 1002
  
  | VPMOVSXBW = 1003
  
  | VPMOVSXDQ = 1004
  
  | VPMOVSXWD = 1005
  
  | VPMOVSXWQ = 1006
  
  | VPMOVUSDB = 1007
  
  | VPMOVUSDW = 1008
  
  | VPMOVUSQB = 1009
  
  | VPMOVUSQD = 1010
  
  | VPMOVUSQW = 1011
  
  | VPMOVUSWB = 1012
  
  | VPMOVW2M = 1013
  
  | VPMOVWB = 1014
  
  | VPMOVZXBD = 1015
  
  | VPMOVZXBQ = 1016
  
  | VPMOVZXBW = 1017
  
  | VPMOVZXDQ = 1018
  
  | VPMOVZXWD = 1019
  
  | VPMOVZXWQ = 1020
  
  | VPMULDQ = 1021
  
  | VPMULHRSW = 1022
  
  | VPMULHUW = 1023
  
  | VPMULHW = 1024
  
  | VPMULLD = 1025
  
  | VPMULLQ = 1026
  
  | VPMULLW = 1027
  
  | VPMULUDQ = 1028
  
  | VPOR = 1029
  
  | VPROLD = 1030
  
  | VPROLQ = 1031
  
  | VPROLVD = 1032
  
  | VPROLVQ = 1033
  
  | VPRORD = 1034
  
  | VPRORQ = 1035
  
  | VPRORRD = 1036
  
  | VPRORRQ = 1037
  
  | VPSADBW = 1038
  
  | VPSCATTERDD = 1039
  
  | VPSCATTERDQ = 1040
  
  | VPSCATTERQD = 1041
  
  | VPSCATTERQQ = 1042
  
  | VPSHUFB = 1043
  
  | VPSHUFD = 1044
  
  | VPSHUFHW = 1045
  
  | VPSHUFLW = 1046
  
  | VPSIGNB = 1047
  
  | VPSIGND = 1048
  
  | VPSIGNW = 1049
  
  | VPSLLD = 1050
  
  | VPSLLDQ = 1051
  
  | VPSLLQ = 1052
  
  | VPSLLVW = 1053
  
  | VPSLLW = 1054
  
  | VPSRAD = 1055
  
  | VPSRAQ = 1056
  
  | VPSRAVQ = 1057
  
  | VPSRAVW = 1058
  
  | VPSRAW = 1059
  
  | VPSRLD = 1060
  
  | VPSRLDQ = 1061
  
  | VPSRLQ = 1062
  
  | VPSRLVW = 1063
  
  | VPSRLW = 1064
  
  | VPSUBB = 1065
  
  | VPSUBD = 1066
  
  | VPSUBQ = 1067
  
  | VPSUBSB = 1068
  
  | VPSUBSW = 1069
  
  | VPSUBUSB = 1070
  
  | VPSUBUSW = 1071
  
  | VPSUBW = 1072
  
  | VPTERLOGD = 1073
  
  | VPTERLOGQ = 1074
  
  | VPTERNLOGD = 1075
  
  | VPTEST = 1076
  
  | VPTESTMB = 1077
  
  | VPTESTMD = 1078
  
  | VPTESTMQ = 1079
  
  | VPTESTMW = 1080
  
  | VPTESTNMB = 1081
  
  | VPTESTNMD = 1082
  
  | VPTESTNMQ = 1083
  
  | VPTESTNMW = 1084
  
  | VPUNPCKHBW = 1085
  
  | VPUNPCKHDQ = 1086
  
  | VPUNPCKHQDQ = 1087
  
  | VPUNPCKHWD = 1088
  
  | VPUNPCKLBW = 1089
  
  | VPUNPCKLDQ = 1090
  
  | VPUNPCKLQDQ = 1091
  
  | VPUNPCKLWD = 1092
  
  | VPXOR = 1093
  
  | VPXORD = 1094
  
  | VPXORQ = 1095
  
  | VRANGEPD = 1096
  
  | VRANGEPS = 1097
  
  | VRANGESD = 1098
  
  | VRANGESS = 1099
  
  | VRCP14PD = 1100
  
  | VRCP14PS = 1101
  
  | VRCP14SD = 1102
  
  | VRCP14SS = 1103
  
  | VRCP28PD = 1104
  
  | VRCP28PS = 1105
  
  | VRCP28SD = 1106
  
  | VRCP28SS = 1107
  
  | VRCPPS = 1108
  
  | VRCPSS = 1109
  
  | VREDUCEPD = 1110
  
  | VREDUCEPS = 1111
  
  | VREDUCESD = 1112
  
  | VREDUCESS = 1113
  
  | VRNDSCALEPD = 1114
  
  | VRNDSCALEPS = 1115
  
  | VRNDSCALESD = 1116
  
  | VRNDSCALESS = 1117
  
  | VROUNDPD = 1118
  
  | VROUNDPS = 1119
  
  | VROUNDSD = 1120
  
  | VRSQRT14PD = 1121
  
  | VRSQRT14PS = 1122
  
  | VRSQRT14SD = 1123
  
  | VRSQRT14SS = 1124
  
  | VRSQRT28PD = 1125
  
  | VRSQRT28PS = 1126
  
  | VRSQRT28SD = 1127
  
  | VRSQRT28SS = 1128
  
  | VRSQRTPS = 1129
  
  | VRSQRTSS = 1130
  
  | VSCALEPD = 1131
  
  | VSCALEPS = 1132
  
  | VSCALESD = 1133
  
  | VSCALESS = 1134
  
  | VSCATTERDD = 1135
  
  | VSCATTERDQ = 1136
  
  | VSCATTERQD = 1137
  
  | VSCATTERQQ = 1138
  
  | VSHUFF32X4 = 1139
  
  | VSHUFF64X2 = 1140
  
  | VSHUFI32X4 = 1141
  
  | VSHUFI64X2 = 1142
  
  | VSHUFPD = 1143
  
  | VSHUFPS = 1144
  
  | VSQRTPD = 1145
  
  | VSQRTPS = 1146
  
  | VSQRTSD = 1147
  
  | VSQRTSS = 1148
  
  | VSUBPD = 1149
  
  | VSUBPS = 1150
  
  | VSUBSD = 1151
  
  | VSUBSS = 1152
  
  | VUCOMISD = 1153
  
  | VUCOMISS = 1154
  
  | VUNPCKHPD = 1155
  
  | VUNPCKHPS = 1156
  
  | VUNPCKLPD = 1157
  
  | VUNPCKLPS = 1158
  
  | VXORPD = 1159
  
  | VXORPS = 1160
  
  | VZEROUPPER = 1161
  
  | WAIT = 1162
  
  | WBINVD = 1163
  
  | WRFSBASE = 1164
  
  | WRGSBASE = 1165
  
  | WRMSR = 1166
  
  | WRPKRU = 1167
  
  | WRSSD = 1168
  
  | WRSSQ = 1169
  
  | WRUSSD = 1170
  
  | WRUSSQ = 1171
  
  | XABORT = 1172
  
  | XACQUIRE = 1173
  
  | XADD = 1174
  
  | XBEGIN = 1175
  
  | XCHG = 1176
  
  | XEND = 1177
  
  | XGETBV = 1178
  
  | XLAT = 1179
  
  | XLATB = 1180
  
  | XOR = 1181
  
  | XORPD = 1182
  
  | XORPS = 1183
  
  | XRELEASE = 1184
  
  | XRSTOR = 1185
  
  | XRSTORS = 1186
  
  | XRSTORS64 = 1187
  
  | XSAVE = 1188
  
  | XSAVEC = 1189
  
  | XSAVEC64 = 1190
  
  | XSAVEOPT = 1191
  
  | XSAVES = 1192
  
  | XSAVES64 = 1193
  
  | XSETBV = 1194
  
  | XTEST = 1195
  
  | InvalOP = 1196


let inline translate opcode =
  match opcode with
  | Opcode.AAA -> 1
  | Opcode.AAD -> 1
  | Opcode.AAM -> 1
  | Opcode.AAS -> 1
  | Opcode.ADC -> 1
  | Opcode.ADD -> 1
  | Opcode.AND -> 1
  | Opcode.ANDN -> 1
  | Opcode.ARPL -> 1
  | Opcode.BNDMOV -> 1
  | Opcode.BOUND -> 1
  | Opcode.BSF -> 1
  | Opcode.BSR -> 1
  | Opcode.BSWAP -> 1
  | Opcode.BT -> 1
  | Opcode.BTC -> 1
  | Opcode.BTR -> 1
  | Opcode.BTS -> 1
  | Opcode.CALLNear -> 1
  | Opcode.CALLFar -> 1
  | Opcode.CBW | Opcode.CWDE | Opcode.CDQE ->
    1
  | Opcode.CLC -> 1
  | Opcode.CLD -> 1
  | Opcode.CLI -> 1
  | Opcode.CLRSSBSY -> 1
  | Opcode.CLTS -> 1
  | Opcode.CMC -> 1
  | Opcode.CMOVO | Opcode.CMOVNO | Opcode.CMOVB | Opcode.CMOVAE
  | Opcode.CMOVZ | Opcode.CMOVNZ | Opcode.CMOVBE | Opcode.CMOVA
  | Opcode.CMOVS  | Opcode.CMOVNS | Opcode.CMOVP | Opcode.CMOVNP
  | Opcode.CMOVL | Opcode.CMOVGE | Opcode.CMOVLE | Opcode.CMOVG ->
    1
  | Opcode.CMP -> 1
  | Opcode.CMPSB | Opcode.CMPSW | Opcode.CMPSQ ->
    1
  | Opcode.CMPXCHG -> 1
  | Opcode.CMPXCHG8B | Opcode.CMPXCHG16B ->
    1
  | Opcode.CPUID -> 1
  | Opcode.CRC32 -> 1
  | Opcode.CWD | Opcode.CDQ | Opcode.CQO ->
    1
  | Opcode.DAA -> 1
  | Opcode.DAS -> 1
  | Opcode.DEC -> 1
  | Opcode.DIV | Opcode.IDIV -> 1
  | Opcode.ENDBR32 | Opcode.ENDBR64 -> 1
  | Opcode.ENTER -> 1
  | Opcode.HLT -> 1
  | Opcode.IMUL -> 1
  | Opcode.INC -> 1
  | Opcode.INCSSPD | Opcode.INCSSPQ -> 1
  | Opcode.INSB | Opcode.INSW | Opcode.INSD ->
    1
  | Opcode.INT | Opcode.INTO -> 1
  | Opcode.INT3 -> 1
  | Opcode.JMPFar | Opcode.JMPNear -> 1
  | Opcode.JO | Opcode.JNO | Opcode.JB | Opcode.JNB
  | Opcode.JZ | Opcode.JNZ | Opcode.JBE | Opcode.JA
  | Opcode.JS | Opcode.JNS | Opcode.JP | Opcode.JNP
  | Opcode.JL | Opcode.JNL | Opcode.JLE | Opcode.JG
  | Opcode.JECXZ | Opcode.JRCXZ -> 1
  | Opcode.LAHF -> 1
  | Opcode.LEA -> 1
  | Opcode.LEAVE -> 1
  | Opcode.LODSB | Opcode.LODSW | Opcode.LODSD | Opcode.LODSQ ->
    1
  | Opcode.LOOP | Opcode.LOOPE | Opcode.LOOPNE ->
    1
  | Opcode.LZCNT -> 1
  | Opcode.LDS | Opcode.LES | Opcode.LFS | Opcode.LGS | Opcode.LSS ->
    1
  | Opcode.MOV -> 1
  | Opcode.MOVBE -> 1
  | Opcode.MOVSB | Opcode.MOVSW | Opcode.MOVSQ ->
    1
  | Opcode.MOVSX | Opcode.MOVSXD -> 1
  | Opcode.MOVZX -> 1
  | Opcode.MUL -> 1
  | Opcode.NEG -> 1
  | Opcode.NOP -> 1
  | Opcode.NOT -> 1
  | Opcode.OR -> 1
  | Opcode.OUTSB | Opcode.OUTSW | Opcode.OUTSD ->
    1
  | Opcode.POP -> 1
  | Opcode.POPA -> 1
  | Opcode.POPAD -> 1
  | Opcode.POPCNT -> 1
  | Opcode.POPF | Opcode.POPFD | Opcode.POPFQ ->
    1
  | Opcode.PUSH -> 1
  | Opcode.PUSHA -> 1
  | Opcode.PUSHAD -> 1
  | Opcode.PUSHF | Opcode.PUSHFD | Opcode.PUSHFQ ->
    1
  | Opcode.RCL -> 1
  | Opcode.RCR -> 1
  | Opcode.RDMSR | Opcode.RSM ->
    1
  | Opcode.RDPKRU -> 1
  | Opcode.RDPMC -> 1
  | Opcode.RDRAND -> 1
  | Opcode.RDSSPD | Opcode.RDSSPQ -> 1
  | Opcode.RDTSC -> 1
  | Opcode.RDTSCP -> 1
  | Opcode.RETNear -> 1
  | Opcode.RETNearImm -> 1
  | Opcode.RETFar -> 1
  | Opcode.RETFarImm -> 1
  | Opcode.ROL -> 1
  | Opcode.ROR -> 1
  | Opcode.RORX -> 1
  | Opcode.RSTORSSP -> 1
  | Opcode.SAHF -> 1
  | Opcode.SAR | Opcode.SHR | Opcode.SHL ->
    1
  | Opcode.SAVEPREVSSP -> 1
  | Opcode.SBB -> 1
  | Opcode.SCASB | Opcode.SCASW | Opcode.SCASD | Opcode.SCASQ ->
    1
  | Opcode.SETO | Opcode.SETNO | Opcode.SETB | Opcode.SETNB
  | Opcode.SETZ | Opcode.SETNZ | Opcode.SETBE | Opcode.SETA
  | Opcode.SETS | Opcode.SETNS | Opcode.SETP | Opcode.SETNP
  | Opcode.SETL | Opcode.SETNL | Opcode.SETLE | Opcode.SETG ->
    1
  | Opcode.SETSSBSY -> 1
  | Opcode.SHLD -> 1
  | Opcode.SHLX -> 1
  | Opcode.SHRD -> 1
  | Opcode.STC -> 1
  | Opcode.STD -> 1
  | Opcode.STI -> 1
  | Opcode.STOSB | Opcode.STOSW | Opcode.STOSD | Opcode.STOSQ ->
    1
  | Opcode.SUB -> 1
  | Opcode.SYSCALL | Opcode.SYSENTER -> 1
  | Opcode.SYSEXIT | Opcode.SYSRET ->
    1
  | Opcode.TEST -> 1
  | Opcode.TZCNT -> 1
  | Opcode.UD2 -> 1
  | Opcode.WBINVD -> 1
  | Opcode.WRFSBASE -> 1
  | Opcode.WRGSBASE -> 1
  | Opcode.WRPKRU -> 1
  | Opcode.WRMSR -> 1
  | Opcode.WRSSD | Opcode.WRSSQ -> 1
  | Opcode.WRUSSD | Opcode.WRUSSQ -> 1
  | Opcode.XABORT -> 1
  | Opcode.XADD -> 1
  | Opcode.XBEGIN -> 1
  | Opcode.XCHG -> 1
  | Opcode.XEND -> 1
  | Opcode.XGETBV -> 1
  | Opcode.XLATB -> 1
  | Opcode.XOR -> 1
  | Opcode.XRSTOR | Opcode.XRSTORS | Opcode.XSAVE | Opcode.XSAVEC
  | Opcode.XSAVEC64 | Opcode.XSAVEOPT | Opcode.XSAVES | Opcode.XSAVES64 ->
    1
  | Opcode.XTEST -> 1
  | Opcode.IN | Opcode.INVD | Opcode.INVLPG | Opcode.IRET | Opcode.IRETQ | Opcode.IRETW | Opcode.IRETD
  | Opcode.LAR | Opcode.LGDT | Opcode.LIDT | Opcode.LLDT
  | Opcode.LMSW | Opcode.LSL | Opcode.LTR | Opcode.OUT | Opcode.SGDT
  | Opcode.SIDT | Opcode.SLDT | Opcode.SMSW | Opcode.STR | Opcode.VERR ->
    1
  | Opcode.MOVD -> 1
  | Opcode.MOVQ -> 1
  | Opcode.PACKSSDW -> 1
  | Opcode.PACKSSWB -> 1
  | Opcode.PACKUSWB -> 1
  | Opcode.PUNPCKHBW -> 1
  | Opcode.PUNPCKHWD -> 1
  | Opcode.PUNPCKHDQ -> 1
  | Opcode.PUNPCKLBW -> 1
  | Opcode.PUNPCKLWD -> 1
  | Opcode.PUNPCKLDQ -> 1
  | Opcode.PADDB -> 1
  | Opcode.PADDW -> 1
  | Opcode.PADDD -> 1
  | Opcode.PADDSB -> 1
  | Opcode.PADDSW -> 1
  | Opcode.PADDUSB -> 1
  | Opcode.PADDUSW -> 1
  | Opcode.PSUBB -> 1
  | Opcode.PSUBW -> 1
  | Opcode.PSUBD -> 1
  | Opcode.PSUBSB -> 1
  | Opcode.PSUBSW -> 1
  | Opcode.PSUBUSB -> 1
  | Opcode.PSUBUSW -> 1
  | Opcode.PMULHW -> 1
  | Opcode.PMULLW -> 1
  | Opcode.PMADDWD -> 1
  | Opcode.PCMPEQB -> 1
  | Opcode.PCMPEQW -> 1
  | Opcode.PCMPEQD -> 1
  | Opcode.PCMPGTB -> 1
  | Opcode.PCMPGTW -> 1
  | Opcode.PCMPGTD -> 1
  | Opcode.PAND -> 1
  | Opcode.PANDN -> 1
  | Opcode.POR -> 1
  | Opcode.PXOR -> 1
  | Opcode.PSLLW -> 1
  | Opcode.PSLLD -> 1
  | Opcode.PSLLQ -> 1
  | Opcode.PSRLW -> 1
  | Opcode.PSRLD -> 1
  | Opcode.PSRLQ -> 1
  | Opcode.PSRAW -> 1
  | Opcode.PSRAD -> 1
  | Opcode.EMMS -> 1
  | Opcode.MOVAPS -> 1
  | Opcode.MOVAPD -> 1
  | Opcode.MOVUPS -> 1
  | Opcode.MOVUPD -> 1
  | Opcode.MOVHPS -> 1
  | Opcode.MOVHPD -> 1
  | Opcode.MOVHLPS -> 1
  | Opcode.MOVLPS -> 1
  | Opcode.MOVLPD -> 1
  | Opcode.MOVLHPS -> 1
  | Opcode.MOVMSKPS -> 1
  | Opcode.MOVMSKPD -> 1
  | Opcode.MOVSS -> 1
  | Opcode.MOVSD -> 1
  | Opcode.ADDPS -> 1
  | Opcode.ADDPD -> 1
  | Opcode.ADDSS -> 1
  | Opcode.ADDSD -> 1
  | Opcode.SUBPS -> 1
  | Opcode.SUBPD -> 1
  | Opcode.SUBSS -> 1
  | Opcode.SUBSD -> 1
  | Opcode.MULPS -> 1
  | Opcode.MULPD -> 1
  | Opcode.MULSS -> 1
  | Opcode.MULSD -> 1
  | Opcode.DIVPS -> 1
  | Opcode.DIVPD -> 1
  | Opcode.DIVSS -> 1
  | Opcode.DIVSD -> 1
  | Opcode.RCPPS -> 1
  | Opcode.RCPSS -> 1
  | Opcode.SQRTPS -> 1
  | Opcode.SQRTPD -> 1
  | Opcode.SQRTSS -> 1
  | Opcode.SQRTSD -> 1
  | Opcode.RSQRTPS -> 1
  | Opcode.RSQRTSS -> 1
  | Opcode.MAXPS -> 1
  | Opcode.MAXPD -> 1
  | Opcode.MAXSS -> 1
  | Opcode.MAXSD -> 1
  | Opcode.MINPS -> 1
  | Opcode.MINPD -> 1
  | Opcode.MINSS -> 1
  | Opcode.MINSD -> 1
  | Opcode.CMPPS -> 1
  | Opcode.CMPPD -> 1
  | Opcode.CMPSS -> 1
  | Opcode.CMPSD -> 1
  | Opcode.COMISS | Opcode.VCOMISS ->
    1
  | Opcode.COMISD | Opcode.VCOMISD -> 
    1
  | Opcode.UCOMISS | Opcode.VUCOMISS ->
    1
  | Opcode.UCOMISD | Opcode.VUCOMISD -> 
    1
  | Opcode.ANDPS -> 1
  | Opcode.ANDPD -> 1
  | Opcode.ANDNPS -> 1
  | Opcode.ANDNPD -> 1
  | Opcode.ORPS -> 1
  | Opcode.ORPD -> 1
  | Opcode.XORPS -> 1
  | Opcode.XORPD -> 1
  | Opcode.XSETBV -> 1
  | Opcode.SHUFPS -> 1
  | Opcode.SHUFPD -> 1
  | Opcode.UNPCKHPS -> 1
  | Opcode.UNPCKHPD -> 1
  | Opcode.UNPCKLPS -> 1
  | Opcode.UNPCKLPD -> 1
  | Opcode.CVTPI2PS -> 1
  | Opcode.CVTPI2PD -> 1
  | Opcode.CVTSI2SS -> 1
  | Opcode.CVTSI2SD -> 1
  | Opcode.CVTPS2PI -> 1
  | Opcode.CVTPS2PD -> 1
  | Opcode.CVTPD2PS -> 1
  | Opcode.CVTPD2PI -> 1
  | Opcode.CVTPD2DQ -> 1
  | Opcode.CVTTPD2DQ -> 1
  | Opcode.CVTDQ2PS -> 1
  | Opcode.CVTDQ2PD -> 1
  | Opcode.CVTPS2DQ -> 1
  | Opcode.CVTTPS2DQ -> 1
  | Opcode.CVTTPS2PI -> 1
  | Opcode.CVTTPD2PI -> 1
  | Opcode.CVTSS2SI | Opcode.VCVTSS2SI ->
    1
  | Opcode.CVTSS2SD -> 1
  | Opcode.CVTSD2SS -> 1
  | Opcode.CVTSD2SI | Opcode.VCVTSD2SI -> 
    1
  | Opcode.CVTTSS2SI | Opcode.VCVTTSS2SI ->
    1
  | Opcode.CVTTSD2SI | Opcode.VCVTTSD2SI -> 
    1
  | Opcode.LDMXCSR -> 1
  | Opcode.STMXCSR -> 1
  | Opcode.PAVGB -> 1
  | Opcode.PAVGW -> 1
  | Opcode.PEXTRW -> 1
  | Opcode.PINSRW -> 1
  | Opcode.PMAXUB -> 1
  | Opcode.PMAXSW -> 1
  | Opcode.PMAXSB -> 1
  | Opcode.PMINUB -> 1
  | Opcode.PMINSW -> 1
  | Opcode.PMINUD -> 1
  | Opcode.PMINSB -> 1
  | Opcode.PMOVMSKB -> 1
  | Opcode.PMULHUW -> 1
  | Opcode.PSADBW -> 1
  | Opcode.PSHUFW -> 1
  | Opcode.PSHUFD -> 1
  | Opcode.PSHUFLW -> 1
  | Opcode.PSHUFHW -> 1
  | Opcode.PSHUFB -> 1
  | Opcode.MOVDQA -> 1
  | Opcode.MOVDQU -> 1
  | Opcode.MOVQ2DQ -> 1
  | Opcode.MOVDQ2Q -> 1
  | Opcode.PMULUDQ -> 1
  | Opcode.PADDQ -> 1
  | Opcode.PSUBQ -> 1
  | Opcode.PSLLDQ -> 1
  | Opcode.PSRLDQ -> 1
  | Opcode.PUNPCKHQDQ -> 1
  | Opcode.PUNPCKLQDQ -> 1
  | Opcode.MOVNTQ -> 1
  | Opcode.MOVNTPS -> 1
  | Opcode.PREFETCHNTA
  | Opcode.PREFETCHT0 | Opcode.PREFETCHT1
  | Opcode.PREFETCHW | Opcode.PREFETCHT2 -> 1
  | Opcode.SFENCE -> 1
  | Opcode.CLFLUSH -> 1
  | Opcode.LFENCE -> 1
  | Opcode.MFENCE -> 1
  | Opcode.PAUSE -> 1
  | Opcode.MOVNTPD -> 1
  | Opcode.MOVNTDQ -> 1
  | Opcode.MOVNTI -> 1
  | Opcode.LDDQU -> 1
  | Opcode.MOVSHDUP -> 1
  | Opcode.MOVSLDUP -> 1
  | Opcode.MOVDDUP -> 1
  | Opcode.PALIGNR -> 1
  | Opcode.ROUNDSD -> 1
  | Opcode.PINSRB -> 1
  | Opcode.PTEST -> 1
  | Opcode.PCMPEQQ -> 1
  | Opcode.PCMPESTRI | Opcode.PCMPESTRM | Opcode.PCMPISTRI | Opcode.PCMPISTRM ->
    1
  | Opcode.VSQRTPS -> 1
  | Opcode.VSQRTPD -> 1
  | Opcode.VSQRTSS -> 1
  | Opcode.VSQRTSD -> 1
  | Opcode.VADDPS -> 1
  | Opcode.VADDPD -> 1
  | Opcode.VADDSS -> 1
  | Opcode.VADDSD -> 1
  | Opcode.VSUBPS -> 1
  | Opcode.VSUBPD -> 1
  | Opcode.VSUBSS -> 1
  | Opcode.VSUBSD -> 1
  | Opcode.VMULPS -> 1
  | Opcode.VMULPD -> 1
  | Opcode.VMULSS -> 1
  | Opcode.VMULSD -> 1
  | Opcode.VDIVPS -> 1
  | Opcode.VDIVPD -> 1
  | Opcode.VDIVSS -> 1
  | Opcode.VDIVSD -> 1
  | Opcode.VCVTSI2SS -> 1
  | Opcode.VCVTSI2SD -> 1
  | Opcode.VCVTSD2SS -> 1
  | Opcode.VCVTSS2SD -> 1
  | Opcode.VMOVD -> 1
  | Opcode.VMOVQ -> 1
  | Opcode.VMOVAPS -> 1
  | Opcode.VMOVAPD -> 1
  | Opcode.VMOVDQU -> 1
  | Opcode.VMOVDQU16 -> 1
  | Opcode.VMOVDQU64 -> 1
  | Opcode.VMOVDQA -> 1
  | Opcode.VMOVDQA64 -> 1
  | Opcode.VMOVNTDQ -> 1
  | Opcode.VMOVUPS -> 1
  | Opcode.VMOVUPD -> 1
  | Opcode.VMOVDDUP -> 1
  | Opcode.VMOVNTPS -> 1
  | Opcode.VMOVNTPD -> 1
  | Opcode.VMOVHLPS -> 1
  | Opcode.VMOVHPD | Opcode.VMOVHPS -> 1
  | Opcode.VMOVLHPS -> 1
  | Opcode.VMOVLPD | Opcode.VMOVLPS -> 1
  | Opcode.VMOVMSKPD -> 1
  | Opcode.VMOVMSKPS -> 1
  | Opcode.VMOVSD -> 1
  | Opcode.VMOVSHDUP -> 1
  | Opcode.VMOVSLDUP -> 1
  | Opcode.VMOVSS -> 1
  | Opcode.VANDPS -> 1
  | Opcode.VANDPD -> 1
  | Opcode.VANDNPS -> 1
  | Opcode.VANDNPD -> 1
  | Opcode.VORPS -> 1
  | Opcode.VORPD -> 1
  | Opcode.VSHUFI32X4 -> 1
  | Opcode.VSHUFPS -> 1
  | Opcode.VSHUFPD -> 1
  | Opcode.VUNPCKHPS -> 1
  | Opcode.VUNPCKHPD -> 1
  | Opcode.VUNPCKLPS -> 1
  | Opcode.VUNPCKLPD -> 1
  | Opcode.VXORPS -> 1
  | Opcode.VXORPD -> 1
  | Opcode.VBROADCASTI128 -> 1
  | Opcode.VBROADCASTSS -> 1
  | Opcode.VEXTRACTF32X8 -> 1
  | Opcode.VEXTRACTI64X4 -> 1
  | Opcode.VINSERTI128 -> 1
  | Opcode.VMPTRLD -> 1
  | Opcode.VPADDB -> 1
  | Opcode.VPADDD -> 1
  | Opcode.VPADDQ -> 1
  | Opcode.VPALIGNR -> 1
  | Opcode.VPAND -> 1
  | Opcode.VPANDN -> 1
  | Opcode.VPBROADCASTB -> 1
  | Opcode.VPBROADCASTD -> 1
  | Opcode.VPCMPEQB -> 1
  | Opcode.VPCMPEQD -> 1
  | Opcode.VPCMPEQQ -> 1
  | Opcode.VPCMPESTRI | Opcode.VPCMPESTRM | Opcode.VPCMPISTRI
  | Opcode.VPCMPISTRM -> 1
  | Opcode.VPCMPGTB -> 1
  | Opcode.VPINSRD -> 1
  | Opcode.VPMINUB -> 1
  | Opcode.VPMINUD -> 1
  | Opcode.VPMOVMSKB -> 1
  | Opcode.VPMULUDQ -> 1
  | Opcode.VPOR -> 1
  | Opcode.VPSHUFB -> 1
  | Opcode.VPSHUFD -> 1
  | Opcode.VPSLLD -> 1
  | Opcode.VPSLLDQ -> 1
  | Opcode.VPSLLQ -> 1
  | Opcode.VPSRLD -> 1
  | Opcode.VPSRLDQ -> 1
  | Opcode.VPSRLQ -> 1
  | Opcode.VPSUBB -> 1
  | Opcode.VPTEST -> 1
  | Opcode.VPUNPCKHDQ -> 1
  | Opcode.VPUNPCKHQDQ -> 1
  | Opcode.VPUNPCKLDQ -> 1
  | Opcode.VPUNPCKLQDQ -> 1
  | Opcode.VPXOR -> 1
  | Opcode.VPXORD -> 1
  | Opcode.VZEROUPPER -> 1
  | Opcode.VINSERTI64X4
  | Opcode.VPMOVWB | Opcode.VMOVDQU32 | Opcode.VPMOVZXWD
  | Opcode.VPSRLW | Opcode.VFMADD213SS ->
    1
  | Opcode.VERW -> 1
  | Opcode.VFMADD132SD -> 1
  | Opcode.VFMADD213SD -> 1
  | Opcode.VFMADD231SD -> 1
  | Opcode.VBROADCASTSD | Opcode.VCVTDQ2PD | Opcode.VCVTPD2PS
  | Opcode.VCVTPS2PD | Opcode.VEXTRACTF64X2 | Opcode.VEXTRACTF64X4
  | Opcode.VFMADD132PD | Opcode.VFMADD213PS | Opcode.VFMADD231PD
  | Opcode.VFMSUB132SS | Opcode.VFMSUB231SD | Opcode.VFNMADD132PD
  | Opcode.VFNMADD231PD | Opcode.VFNMADD132SD | Opcode.VFNMADD213SD
  | Opcode.VFNMADD231SD | Opcode.VINSERTF128 | Opcode.VINSERTF64X4
  | Opcode.VMAXPS | Opcode.VMAXSD | Opcode.VMAXSS | Opcode.VMINSS
  | Opcode.VPERMI2D | Opcode.VPERMI2PD | Opcode.VPERMI2W | Opcode.VPMOVWB
  | Opcode.VPTERNLOGD | Opcode.VCMPPD | Opcode.VCMPPS | Opcode.VGATHERDPS
  | Opcode.VPGATHERDD | Opcode.VMOVDQU8 ->
    1
  | Opcode.VRSQRTSS | Opcode.VFMSUB213SD | Opcode.VRSQRT28SD | Opcode.VRCP28SD | Opcode.VPEXTRD
  | Opcode.VFMADD213PD | Opcode.VPBROADCASTQ | Opcode.VPSUBW | Opcode.VFMSUB213PD | Opcode.VPSUBD
  | Opcode.VRCPSS | Opcode.VGETMANTSD | Opcode.VGETEXPSD | Opcode.VRCP14SD | Opcode.VRNDSCALESD
  | Opcode.VEXTRACTF128 -> 1
  | Opcode.VPCMPGTD | Opcode.MULX | Opcode.VREDUCESD | Opcode.VROUNDPD | Opcode.VMINPD | Opcode.VRSQRTPS
  | Opcode.VBLENDVPD | Opcode.VFNMADD213PD | Opcode.VFMSUB231PD | Opcode.BLENDVPD | Opcode.ROUNDPD
  | Opcode.VRCPPS | Opcode.VGATHERQPD | Opcode.VPSRAD | Opcode.VCVTDQ2PS | Opcode.VCVTTPD2DQ
  | Opcode.VPMULLD | Opcode.PMULLD | Opcode.VROUNDPS | Opcode.ROUNDPS ->
    1
  | Opcode.FLD -> 1
  | Opcode.FST -> 1
  | Opcode.FSTP -> 1
  | Opcode.FILD -> 1
  | Opcode.FIST -> 1
  | Opcode.FISTP -> 1
  | Opcode.FISTTP -> 1
  | Opcode.FBLD -> 1
  | Opcode.FBSTP -> 1
  | Opcode.FXCH -> 1
  | Opcode.FCMOVE -> 1
  | Opcode.FCMOVNE -> 1
  | Opcode.FCMOVB -> 1
  | Opcode.FCMOVBE -> 1
  | Opcode.FCMOVNB -> 1
  | Opcode.FCMOVNBE -> 1
  | Opcode.FCMOVU -> 1
  | Opcode.FCMOVNU -> 1
  | Opcode.FADD -> 1
  | Opcode.FADDP -> 1
  | Opcode.FIADD -> 1
  | Opcode.FSUB -> 1
  | Opcode.FSUBP -> 1
  | Opcode.FISUB -> 1
  | Opcode.FSUBR -> 1
  | Opcode.FSUBRP -> 1
  | Opcode.FISUBR  -> 1
  | Opcode.FMUL -> 1
  | Opcode.FMULP -> 1
  | Opcode.FIMUL -> 1
  | Opcode.FDIV -> 1
  | Opcode.FDIVP -> 1
  | Opcode.FIDIV -> 1
  | Opcode.FDIVR -> 1
  | Opcode.FDIVRP -> 1
  | Opcode.FIDIVR -> 1
  | Opcode.FPREM -> 1
  | Opcode.FPREM1 -> 1
  | Opcode.FABS -> 1
  | Opcode.FCHS -> 1
  | Opcode.FRNDINT -> 1
  | Opcode.FSCALE -> 1
  | Opcode.FSQRT -> 1
  | Opcode.FXTRACT -> 1
  | Opcode.FCOM -> 1
  | Opcode.FCOMP -> 1
  | Opcode.FCOMPP -> 1
  | Opcode.FUCOM -> 1
  | Opcode.FUCOMP -> 1
  | Opcode.FUCOMPP -> 1
  | Opcode.FICOM -> 1
  | Opcode.FICOMP -> 1
  | Opcode.FCOMI -> 1
  | Opcode.FUCOMI -> 1
  | Opcode.FCOMIP -> 1
  | Opcode.FUCOMIP -> 1
  | Opcode.FTST -> 1
  | Opcode.FXAM -> 1
  | Opcode.FSIN -> 1
  | Opcode.FCOS -> 1
  | Opcode.FSINCOS -> 1
  | Opcode.FPTAN -> 1
  | Opcode.FPATAN -> 1
  | Opcode.F2XM1 -> 1
  | Opcode.FYL2X -> 1
  | Opcode.FYL2XP1 -> 1
  | Opcode.FLD1 -> 1
  | Opcode.FLDZ -> 1
  | Opcode.FLDPI -> 1
  | Opcode.FLDL2E -> 1
  | Opcode.FLDLN2 -> 1
  | Opcode.FLDL2T -> 1
  | Opcode.FLDLG2 -> 1
  | Opcode.FINCSTP -> 1
  | Opcode.FDECSTP -> 1
  | Opcode.FFREE -> 1
  | Opcode.FINIT -> 1
  | Opcode.FNINIT -> 1
  | Opcode.FCLEX -> 1
  | Opcode.FSTCW -> 1
  | Opcode.FNSTCW -> 1
  | Opcode.FLDCW -> 1
  | Opcode.FSTENV -> 1
  | Opcode.FLDENV -> 1
  | Opcode.FSAVE -> 1
  | Opcode.FRSTOR -> 1
  | Opcode.FSTSW -> 1
  | Opcode.FNSTSW -> 1
  | Opcode.WAIT -> 1
  | Opcode.FNOP -> 1
  | Opcode.FXSAVE | Opcode.FXSAVE64 -> 1
  | Opcode.FXRSTOR | Opcode.FXRSTOR64 -> 1
  | o -> failwith ""

// vim: set tw=80 sts=2 sw=2:
let translate2 opcode = translate opcode
