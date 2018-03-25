# ECMA-48 Progress/Compliance

## Reference documents

* [ECMA-6 7-Bit coded Character Set](http://www.ecma-international.org/publications/files/ECMA-ST/Ecma-006.pdf)

This document explains the formatting of the 7-bit characters used throughout most other documents. Instead of
simply calling it ASCII, in 1965 it was probably better to just call it whatever at the time. There is much
in here which is obviously highly obsolete, but there are many gems throughout the document which make it worthwhile.

* [ECMA-7 Representation of the Standard ECMA-6 (7 bit Code) on Punched Cards](https://www.ecma-international.org/publications/files/ECMA-ST-WITHDRAWN/ECMA-7,%201st%20Edition,%20April%201965.pdf)

This document is truly and oddity. Its purpose was to define how to use punch cards to represent characters. But to fully
understand the crazy 1/11 type of notation instead of purely 0x1B type of hexadecimal, this document clarifies it by making
it possible to choose column and rows on punch cards. I was so embarressed when I finally realized that all these old standards
were thinking in terms of rows and columns as opposed to hex ... and this was a huge issue for me because I never figured out
that 1/11 was what they did before hex. So I had not idea 1/11 was 0x1B.

* [ECMA-16](https://www.ecma-international.org/publications/files/ECMA-ST-WITHDRAWN/ECMA-16,%202nd%20Edition,%20June%201973.pdf)

This document is considered to be roughly similar to [ISO 1745:1975](https://www.iso.org/obp/ui/#iso:std:iso:1745:ed-1:v1:en) which
describes the functions of the C0 character set (stuff like line feed and form feed). It is likely complete enough to use as a
substitute for ISO 1745.

* [ECMA-48](https://www.ecma-international.org/publications/files/ECMA-ST/Ecma-048.pdf)

This is the document forming the premise of this document. It is the foundation for identifying how far VtNetCore has come
in standards compliance with ECMA.

## Control Functions

### Brief table of compliance

| Definition                                    | Notation   | Representation                              | Implemented |
|-----------------------------------------------|------------|---------------------------------------------|-------------|
| 8.3.1 ACK - ACKNOWLEDGE                       | C0         | 00/06 (0x06)                                | no          |
| 8.3.2 APC - APPLICATION PROGRAM COMMAND       | C1         | 09/15 (0x9F)                                | no          |
| 8.3.3 BEL - BELL                              | C0         | 00/07 (0x07)                                | decoded     |
| 8.3.4 BPH - BREAK PERMITTED HERE              | C1         | 08/02 (0x82) or ESC 04/02 (\eB)             | no          |
| 8.3.5 BS - BACKSPACE                          | C0         | 00/08 (0x08)                                | yes         |
| 8.3.6 CAN - CANCEL                            | C1         | 01/08 (0x18)                                | no          |
| 8.3.7 CBT - CURSOR BACKWARD TABULATION        | (Pn)       | CSI Pn 05/10 (CSI Pn Z)                     | yes         |
| 8.3.8 CCH - CANCEL CHARACTER                  | C1         | 09/04 (0x94) or ESC 05/04 (\eT)             | no          |
| 8.3.9 CHA - CURSOR CHARACTER ABSOLUTE         | Pn         | CSI Pn 04/07 (CSI Pn G)                     | yes         |
| 8.3.10 CHT - CURSOR FORWARD TABULATION        | Pn         | CSI Pn 04/09 (CSI Pn I)                     | yes         |
| 8.3.11 CMD - CODING METHOD DELIMITER          | Fs         | ESC 06/04 (\ed)                             | no          |
| 8.3.12 CNL - CURSOR NEXT LINE                 | Pn         | CSI Pn 04/05 (CSI Pn E)                     | yes         |
| 8.3.13 CPL - CURSOR PRECEDING LINE            | Pn         | CSI Pn 04/06 (CSI Pn F)                     | yes         |
| 8.3.14 CPR - ACTIVE POSITION REPORT           | (Pn1;Pn2)  | CSI Pn1;Pn2 05/02 (CSI Pn1;Pn2 Q)           | no          |
| 8.3.15 CR - CARRIAGE RETURN                   | C0         | 00/13 (0x0D) '\n'                           | yes         |
| 8.3.16 CSI - CONTROL SEQUENCE INTRODUCER      | C1         | 09/11 (0x9B) or ESC 05/11 (0x5B) '\e['      | partial     |
| 8.3.17 CTC - CURSOR TABULATION CONTROL        | (Ps...)    | CSI Ps... 05/07 (0x57) or CSI Ps... W       | No          |
| 8.3.18 CUB - CURSOR LEFT                      | Pn         | CSI Pn 04/04 (CSI Pn D)                     | yes         |
| 8.3.19 CUD - CURSOR DOWN                      | Pn         | CSI Pn 04/02 (CSI Pn B)                     | yes         |
| 8.3.20 CUF - CURSOR RIGHT                     | Pn         | CSI Pn 04/03 (CSI Pn C)                     | yes         |
| 8.3.21 CUP - CURSOR POSITION                  | (Pn1;Pn2)  | CSI Pn1;Pn2 04/08 (CSI Pn1;Pn2 H)           | yes         |
| 8.3.22 CUU - CURSOR UP                        | Pn         | CSI Pn 04/01 (CSI Pn A)                     | yes         |
| 8.3.23 CVT - CURSOR LINE TABULATION           | Pn         | CSI Pn 05/08 (CSI Pn Y)                     | no          |
| 8.3.24 DA - DEVICE ATTRIBUTES                 | Ps         | CSI Ps 06/03 (CSI Ps c)                     | partial     |
| 8.3.25 DAQ - DEFINE AREA QUALIFICATION        | (Ps...)    | CSI Ps... 06/15 (0x6F) (CSI Ps... o)        | no          |
| 8.3.26 DCH - DELETE CHARACTER                 | Pn         | CSI Pn 05/00 (CSI Pn P)                     | mostly      |
| 8.3.27 DCS - DEVICE CONTROL STRING            | C1         | 09/00 (0x90) or ESC 05/00 (\eP)             | no          |
| 8.3.28 DC1 - DEVICE CONTROL ONE               | C0         | 01/01 (0x11)                                | no          |
| 8.3.29 DC2 - DEVICE CONTROL TWO               | C0         | 01/02 (0x12)                                | no          |
| 8.3.30 DC3 - DEVICE CONTROL THREE             | C0         | 01/03 (0x13)                                | no          |
| 8.3.31 DC4 - DEVICE CONTROL FOUR              | C0         | 01/04 (0x14)                                | no          |
| 8.3.32 DL - DELETE LINE                       | Pn         | CSI Pn 04/13 (0x4D) (CSI Pn M)              | mostly      |
| 8.3.33 DLE - DATA LINK ESCAPE                 | C0         | 01/00 (0x10)                                | no          |
| 8.3.34 DMI - DISABLE MANUAL INPUT             | Fs         | ESC 06/00 (0x60) or (\e`)                   | no          |
| 8.3.35 DSR - DEVICE STATUS REPORT             | Ps         | CSI Ps 06/14 (0x6E) or CSI Ps n             | partial     |
| 8.3.36 DTA - DIMENSION TEXT AREA              | (Pn1;Pn2)  | CSI Pn1;Pn2 02/00 05/04 (CSI Pn1;Pn2 ' ' T) | no          |
| 8.3.37 EA - ERASE IN AREA                     | Ps         | CSI Ps 04/15 (0x4F) (CSI Ps O)              | no          |
| 8.3.38 ECH - ERASE CHARACTER                  | Pn         | CSI Pn 05/08 (0x58) (CSI Ps X)              | yes         |
| 8.3.39 ED - ERASE IN PAGE                     | Ps         | CSI Ps 04/10 (0x4A) (CSI Ps J)              | yes         |
| 8.3.40 EF - ERASE IN FIELD                    | Ps         | CSI Ps 04/14 (0x4E) (CSI Ps N)              | no          |
| 8.3.41 EL - ERASE IN LINE                     | Ps         | CSI Ps 04/11 (0x4B) (CSI Ps K)              | yes         |
| 8.3.42 EM - END OF MEDIUM                     | C0         | 01/09 (0x19)                                | no          | 
| 8.3.43 EMI - ENABLE MANUAL INPUT              | Fs         | ESC 06/02 (0x62) (ESC b)                    | no          |
| 8.3.44 ENQ - ENQUIRY                          | C0         | 00/05 (0x05)                                | no          |
| 8.3.45 EOT - END OF TRANSMISSION              | C0         | 00/04 (0x04)                                | no          |
| 8.3.46 EPA - END OF GUARDED AREA              | C1         | 09/07 (0x97) or ESC 05/07 (0x57 'W')        | no          |
| 8.3.47 ESA - END OF SELECTED AREA             | C1         | 08/07 (0x87) or ESC 04/07 (0x47 'G')        | no          |
| 8.3.48 ESC - ESCAPE                           | C0         | 01/11 (0x1B)                                | yes         |
| 8.3.49 ETB - END OF TRANSMISSION BLOCK        | C0         | 01/07 (0x17)                                | no          |
| 8.3.50 ETX - END OF TEXT                      | C0         | 00/03 (0x03)                                | no          |
| 8.3.51 FF - FORM FEED                         | C0         | 00/12='\f'                                  | yes-screen  |
| 8.3.52 FNK - FUNCTION KEY                     | Pn         | CSI Pn 02/00 05/07 (CSI Pn ' ' W)           | no          |
| 8.3.53 FNT - FONT SELECTION                   | (Ps1;Ps2)  | CSI Ps1;Ps2 02/00=' ' 04/04='D'             | no          |
| 8.3.54 GCC - GRAPHIC CHARACTER COMBINATION    | Ps         | CSI Ps 02/00=' ' 05/15='_'                  | no          |
| 8.3.55 GSM - GRAPHIC SIZE MODIFICATION        | (Pn1;Pn2)  | CSI Pn1;Pn2 02/00=' ' 04/02='B'             | no          |
| 8.3.56 GSS - GRAPHIC SIZE SELECTION           | Pn         | CSI Pn 02/00=' ' 04/03='C'                  | no          |
| 8.3.57 HPA - CHARACTER POSITION ABSOLUTE      | Pn         | CSI Pn 06/00='`'                            | yes         |
| 8.3.58 HPB - CHARACTER POSITION BACKWARD      | Pn         | CSI Pn 06/10='j'                            | yes         |
| 8.3.59 HPR - CHARACTER POSITION FORWARD       | Pn         | CSI Pn 06/01='a'                            | yes         |
| 8.3.60 HT - CHARACTER TABULATION              | C0         | 00/09='\t'                                  | yes         |
| 8.3.61 HTJ - CHARACTER TABULATION WITH JUSTIFICATION | C1  | 08/09 (0x89) or ESC 04/09='I'               | no          | 
| 8.3.62 HTS - CHARACTER TABULATION SET         | C1         | 08/08 or ESC 04/08='H'                      | yes (7-bit) |
| 8.3.63 HVP - CHARACTER AND LINE POSITION      | (Pn1;Pn2)  | CSI Pn1;Pn2 06/06='f'                       | yes         |
| 8.3.64 ICH - INSERT CHARACTER                 | Pn         | CSI Pn 04/00='@'                            | yes         |
| 8.3.65 IDCS - IDENTIFY DEVICE CONTROL STRING  | Ps         | CSI Ps 02/00=' ' 04/15='O'                  | no          |
| 8.3.66 IGS - IDENTIFY GRAPHIC SUBREPERTOIRE   | Ps         | CSI Ps 02/00=' ' 04/13='M'                  | no          |
| 8.3.67 IL - INSERT LINE                       | Pn         | CSI Pn 04/12='L'                            | yes         |
| 8.3.68 INT - INTERRUPT                        | Fs         | ESC 06/01='a'                               | no          |
| 8.3.69 IS1 - INFORMATION SEPARATOR ONE (US - UNIT SEPARATOR) | C0 | 01/15=0x1F                           | no          |
| 8.3.70 IS2 - INFORMATION SEPARATOR TWO (RS - RECORD SEPARATOR) | C0 | 01/14=0x1E                         | no          |
| 8.3.71 IS3 - INFORMATION SEPARATOR THREE (GS - GROUP SEPARATOR) | C0 | 01/13=0x1D                        | no          |
| 8.3.72 IS4 - INFORMATION SEPARATOR FOUR (FS - FILE SEPARATOR) | C0 | 01/12=0x1C                          | no          |
| 8.3.73 JFY - JUSTIFY                          | (Ps...)    | CSI Ps... 02/00=' ' 04/06='F'               | no          |
| 8.3.74 LF - LINE FEED                         | C0         | 00/10='\n'                                  | yes         |
| 8.3.75 LS0 - LOCKING-SHIFT ZERO               | C0         | 00/15=0x0F                                  | yes         |
| 8.3.76 LS1 - LOCKING-SHIFT ONE                | C0         | 00/14=0x0E                                  | yes         |
| 8.3.77 LS1R - LOCKING-SHIFT ONE RIGHT         | Fs         | ESC 07/14='~'                               | yes         |
| 8.3.78 LS2 - LOCKING-SHIFT TWO                | Fs         | ESC 06/14='n'                               | yes         |
| 8.3.79 LS2R - LOCKING-SHIFT TWO RIGHT         | Fs         | ESC 07/13='}'                               | yes         |
| 8.3.80 LS3 - LOCKING-SHIFT THREE              | Fs         | ESC 06/15='o'                               | yes         |
| 8.3.81 LS3R - LOCKING-SHIFT THREE RIGHT       | Fs         | ESC 07/12='\|'                              | yes         |
| 8.3.82 MC - MEDIA COPY                        | Ps         | CSI Ps 06/09='i'                            | no          |
| 8.3.83 MW - MESSAGE WAITING                   | C1         | 09/05 (0x95) or ESC 05/05='U'               | no          |
| 8.3.84 NAK - NEGATIVE ACKNOWLEDGE             | C0         | 01/05 (0x15)                                | no          |
| 8.3.85 NBH - NO BREAK HERE                    | C1         | 08/03 (0x83) or ESC 04/03='C'               | no          |
| 8.3.86 NEL - NEXT LINE                        | C1         | 08/05 (0x85) or ESC 04/05='E'               | yes (7-bit) |
| 8.3.87 NP - NEXT PAGE                         | Pn         | CSI Pn 05/05='U'                            | no          |
| 8.3.88 NUL - NULL                             | C0         | 00/00='\0'                                  | yes (ignore)|
| 8.3.89 OSC - OPERATING SYSTEM COMMAND         | C1         | 09/13=0x9D or ESC 05/13=']'                 | yes?        |
| 8.3.90 PEC - PRESENTATION EXPAND OR CONTRACT  | Ps         | CSI Ps 02/00=' ' 05/10='Z'                  | no          |
| 8.3.91 PFS - PAGE FORMAT SELECTION            | Ps         | CSI Ps 02/00=' ' 04/10='J'                  | no          |
| 8.3.92 PLD - PARTIAL LINE FORWARD             | C1         | 08/11 (0x8B) or ESC 04/11='K'               | no          |
| 8.3.93 PLU - PARTIAL LINE BACKWARD            | C1         | 08/12 (0x8C) or ESC 04/12='L'               | no          |
| 8.3.94 PM - PRIVACY MESSAGE                   | C1         | 09/14 (0x9E) or ESC 05/14='^'               | no          |
| 8.3.95 PP - PRECEDING PAGE                    | Pn         | CSI Pn 05/06='V'                            | no          |
| 8.3.96 PPA - PAGE POSITION ABSOLUTE           | Pn         | CSI Pn 02/00=' ' 05/00='P'                  | no          |
| 8.3.97 PPB - PAGE POSITION BACKWARD           | Pn         | CSI Pn 02/00=' ' 05/02='R'                  | no          |
| 8.3.98 PPR - PAGE POSITION FORWARD            | Pn         | CSI Pn 02/00=' ' 05/01='Q'                  | no          |
| 8.3.99 PTX - PARALLEL TEXTS                   | Ps         | CSI Ps 05/12='\\'                           | no          |
| 8.3.100 PU1 - PRIVATE USE ONE                 | C1         | 09/01 (0x91) or ESC 05/01='Q'               | no          |
| 8.3.101 PU2 - PRIVATE USE TWO                 | C1         | 09/02 (0x92) or ESC 05/02='R'               | no          |
| 8.3.102 QUAD - QUAD                           | (Ps...)    | CSI Ps... 02/00=' ' 04/08='H'               | no          |
| 8.3.103 REP - REPEAT                          | Pn         | CSI Pn 06/02='b'                            | yes         |
| 8.3.104 RI - REVERSE LINE FEED                | C1         | 08/13=0x8D or ESC 04/13='M'                 | yes         |
| 8.3.105 RIS - RESET TO INITIAL STATE          | Fs         | ESC 06/03='c'                               | yes         |
| 8.3.106 RM - RESET MODE                       | (Ps...)    | CSI Ps... 06/12='l'                         | partial     |
| 8.3.107 SACS - SET ADDITIONAL CHARACTER SEPARATION | (Pn)  | CSI Pn 02/00=' ' 05/12='\\'                 | no          |
| 8.3.108 SAPV - SELECT ALTERNATIVE PRESENTATION VARIANTS | (Ps...) |  CSI Ps... 02/00=' ' 05/13=']'       | no          |
| 8.3.110 SCO - SELECT CHARACTER ORIENTATION    | Ps         | CSI Ps 02/00=' ' 06/05='e'                  | no          |
| 8.3.111 SCP - SELECT CHARACTER PATH           | (Ps1;Ps2)  | CSI Ps1;Ps2 02/00=' ' 06/11='k'             | no          |
| 8.3.112 SCS - SET CHARACTER SPACING           | Pn         | CSI Pn 02/00=' ' 06/07='g'                  | no          |
| 8.3.113 SD - SCROLL DOWN                      | Pn         | CSI Pn 05/04='T'                            | yes         |
| 8.3.114 SDS - START DIRECTED STRING           | Ps         | CSI Ps 05/13=']'                            | no          |
| 8.3.115 SEE - SELECT EDITING EXTENT           | Ps         | CSI Ps 05/01='Q'                            | no          |
| 8.3.116 SEF - SHEET EJECT AND FEED            | (Ps1;Ps2)  | CSI Ps1;Ps2 02/00=' ' 05/09='Y'             | no          |
| 8.3.117 SGR - SELECT GRAPHIC RENDITION        | (Ps...)    | CSI Ps... 06/13='m'                         | partial     |
| 8.3.118 SHS - SELECT CHARACTER SPACING        | Ps         | CSI Ps 02/00=' ' 04/11='K'                  | no          |
| 8.3.119 SI - SHIFT-IN                         | C0         | 00/15 (0x0F)                                | yes         |
| 8.3.120 SIMD - SELECT IMPLICIT MOVEMENT DIRECTION | Ps     | CSI Ps 05/14 (^)                            | no          |
| 8.3.121 SL - SCROLL LEFT                      | Pn         | CSI Pn 02/00=' ' 04/00='@'                  | no          |
| 8.3.122 SLH - SET LINE HOME                   | Pn         | CSI Pn 02/00=' ' 05/05='U'                  | no          |
| 8.3.123 SLL - SET LINE LIMIT                  | Pn         | CSI Pn 02/00=' ' 05/06='V'                  | no          |
| 8.3.124 SLS - SET LINE SPACING                | Pn         | CSI Pn 02/00=' ' 06/08='h'                  | no          |
| 8.3.126 SO - SHIFT-OUT                        | C0         | 00/14 (0x0E)                                | yes         |
| 8.3.127 SOH - START OF HEADING                | C0         | 00/01 (0x01)                                | no          |
| 8.3.128 SOS - START OF STRING                 | C1         | 09/08 (0x98) or ESC 05/08='X'               | no          |
| 8.3.129 SPA - START OF GUARDED AREA           | C1         | 09/06 (0x96) or ESC 05/06='V'               | no          |
| 8.3.130 SPD - SELECT PRESENTATION DIRECTIONS  | (Ps1;Ps2)  | CSI Ps1;Ps2 02/00=' ' 05/03='S'             | no          |
| 8.3.131 SPH - SET PAGE HOME                   | Pn         | CSI Pn 02/00=' ' 06/09='i'                  | no          |
| 8.3.132 SPI - SPACING INCREMENT               | (Pn1;Pn2)  | CSI Pn1;Pn2 02/00=' ' 04/07='G'             | no          |
| 8.3.133 SPL - SET PAGE LIMIT                  | Pn         | CSI Pn 02/00=' ' 06/10='j'                  | no          |
| 8.3.134 SPQR - SELECT PRINT QUALITY AND RAPIDITY | Ps      | CSI Ps 02/00=' ' 05/08='X'                  | no          |
| 8.3.135 SR - SCROLL RIGHT                     | Pn         | CSI Pn 02/00=' ' 04/01='A'                  | no          |
| 8.3.136 SRCS - SET REDUCED CHARACTER SEPARATION | Pn       | CSI Pn 02/00=' ' 06/06='f'                  | no          |
| 8.3.137 SRS - START REVERSED STRING           | Ps         | CSI Ps 05/11='['                            | no          |
| 8.3.138 SSA - START OF SELECTED AREA          | C1         | 08/06 (0x86) or ESC 04/06='F'               | no          |
| 8.3.139 SSU - SELECT SIZE UNIT                | Ps         | CSI Ps 02/00=' ' 04/09='I'                  | no          |
| 8.3.140 SSW - SET SPACE WIDTH                 | Pn         | CSI Pn 02/00=' ' 05/11='['                  | no          |
| 8.3.141 SS2 - SINGLE-SHIFT TWO                | C1         | 08/14 (0x8E) or ESC 04/14='N'               | yes (7-bit) |
| 8.3.142 SS3 - SINGLE-SHIFT THREE              | C1         | 08/15 (0x8F) or ESC 04/15='O'               | yes (7-bit) |
| 8.3.143 ST - STRING TERMINATOR                | C1         | 09/12 (0x9C) or ESC 05/12='\\'              | yes (7-bit) |
| 8.3.144 STAB - SELECTIVE TABULATION           | Ps         | CSI Ps 02/00=' ' 05/14='^'                  | no          |
| 8.3.145 STS - SET TRANSMIT STATE              | C1         | 09/03 (0x93) or ESC 05/03='S'               | no          |
| 8.3.146 STX - START OF TEXT                   | C0         | 00/02 (0x02)                                | no          |
| 8.3.147 SU - SCROLL UP                        | Pn         | CSI Pn 05/03                                | yes         |
| 8.3.148 SUB - SUBSTITUTE                      | C0         | 01/10 (0x1A)                                | no          |
| 8.3.149 SVS - SELECT LINE SPACING             | Ps         | CSI Ps 02/00=' ' 04/12='L'                  | no          |
| 8.3.150 SYN - SYNCHRONOUS IDLE                | C0         | 01/06 (0x16)                                | no          |
| 8.3.151 TAC - TABULATION ALIGNED CENTRED      | Pn         | CSI Pn 02/00=' ' 06/02='b'                  | no          |
| 8.3.152 TALE - TABULATION ALIGNED LEADING EDGE | Pn        | CSI Pn 02/00=' ' 06/01='a'                  | no          |
| 8.3.153 TATE - TABULATION ALIGNED TRAILING EDGE | Pn       | CSI Pn 02/00=' ' 06/00='`'                  | no          |
| 8.3.154 TBC - TABULATION CLEAR                | Ps         | CSI Ps 06/07='g'                            | partial     |
| 8.3.155 TCC - TABULATION CENTRED ON CHARACTER | (Pn1;Pn2)  | CSI Pn1;Pn2 02/00=' ' 06/03='c'             | no          |
| 8.3.156 TSR - TABULATION STOP REMOVE          | Pn         | CSI Pn 02/00=' ' 06/04='d'                  | no          |
| 8.3.157 TSS - THIN SPACE SPECIFICATION        | Pn         | CSI Pn 02/00=' ' 04/05='E'                  | no          |
| 8.3.158 VPA - LINE POSITION ABSOLUTE          | Pn         | CSI Pn 06/04='d'                            | yes         |
| 8.3.159 VPB - LINE POSITION BACKWARD          | Pn         | CSI Pn 06/11='k'                            | yes         |
| 8.3.160 VPR - LINE POSITION FORWARD           | Pn         | CSI Pn 06/05='e'                            | yes         |
| 8.3.161 VT - LINE TABULATION                  | C0         | 00/11 (0x0B)                                | yes         |
| 8.3.162 VTS - LINE TABULATION SET             | C1         | 08/10 (0x8A) or ESC 04/10='J'               | no          | 

### Notes / Issues

#### 8.3.5 BS - BACKSPACE

> BS causes the active data position to be moved one character position in the data component in the direction opposite to that of the implicit movement.
>
> The direction of the implicit movement depends on the parameter value of SELECT IMPLICIT
MOVEMENT DIRECTION (SIMD). 

TODO : As SIMD is not implemented, the cursor direction will always be leftward. (complexity high)

#### 8.3.6 CAN - CANCEL

> CAN is used to indicate that the data preceding it in the data stream is in error. As a result, this data shall be ignored. The specific meaning of this control function shall be defined for each application and/or between sender and recipient.

TODO : Implement this in the sequence parser. (complexity, possibly high)

#### 8.3.8 CCH - CANCEL CHARACTER

> CCH is used to indicate that both the preceding graphic character in the data stream, (represented by one or more bit combinations) including SPACE, and the control function CCH itself are to be ignored for further interpretation of the data stream.
>
> If the character preceding CCH in the data stream is a control function (represented by one or more bit combinations), the effect of CCH is not defined by this Standard.

TODO : Consider implementation in sequence parser (complexity, possibly high)

#### 8.3.11 CMD - CODING METHOD DELIMITER

> CMD is used as the delimiter of a string of data coded according to Standard ECMA-35 and to switch > to a general level of control.
>
> The use of CMD is not mandatory if the higher level protocol defines means of delimiting the string, for instance, by specifying the length of the string.

TODO : Consider implementing.

#### 8.3.14 CPR - ACTIVE POSITION REPORT

> If the DEVICE COMPONENT SELECT MODE (DCSM) is set to PRESENTATION, CPR is used to report the > active presentation position of the sending device as residing in the presentation component at the n-th line position according to the line progression and at the m-th character position > according to the character path, where n equals the value of Pn1 and m equals the value of Pn2.
>
> If the DEVICE COMPONENT SELECT MODE (DCSM) is set to DATA, CPR is used to report the active data position of the sending device as residing in the data component at the n-th line > position according to the line progression and at the m-th character position according to the > character progression, where n equals the value of Pn1 and m equals the value of Pn2.
>
> CPR may be solicited by a DEVICE STATUS REPORT (DSR) or be sent unsolicited.

TODO : MUST implement!!! (complexity medium/high)

#### 8.3.16 CSI - CONTROL SEQUENCE INTRODUCER

> CSI is used as the first character of a control sequence, see 5.4.

TODO : MUST implement 0x9B (for 8-bit) (complexity, minimal)

#### 8.3.17 CTC - CURSOR TABULATION CONTROL

> CTC causes one or more tabulation stops to be set or cleared in the presentation component, > depending on the parameter values:
>
> 0) a character tabulation stop is set at the active presentation position
> 1) a line tabulation stop is set at the active line (the line that contains the active > presentation position)
> 2) the character tabulation stop at the active presentation position is cleared
> 3) the line tabulation stop at the active line is cleared
> 4) all character tabulation stops in the active line are cleared
> 5) all character tabulation stops are cleared
> 6) all line tabulation stops are cleared
>
> In the case of parameter values 0, 2 or 4 the number of lines affected depends on the setting of the TABULATION STOP MODE (TSM).

TODO : Consider implementing (complexity simple but a lot of conditions)

#### 8.3.23 CVT - CURSOR LINE TABULATION

> CVT causes the active presentation position to be moved to the corresponding character position of the line corresponding to the n-th following line tabulation stop in the presentation component, where n equals the value of Pn.

TODO : Consider implementing (complexity pretty simple)

#### 8.3.25 DAQ - DEFINE AREA QUALIFICATION

SEE : Standard, too much to copy here

TODO : Implement (complexity medium, but many conditions)

#### 8.3.27 DCS - DEVICE CONTROL STRING

> DCS is used as the opening delimiter of a control string for device control use. The command string following may consist of bit combinations in the range 00/08 to 00/13 and 02/00 to 07/14. The control string is closed by the terminating delimiter STRING TERMINATOR (ST).
>
> The command string represents either one or more commands for the receiving device, or one or more status reports from the sending device. The purpose and the format of the command string are specified by the most recent occurrence of IDENTIFY DEVICE CONTROL STRING (IDCS), if any, or depend on the sending and/or the receiving device.

TODO : Consider implementing (complexity, TBD)

#### 8.3.34 DMI - DISABLE MANUAL INPUT

> DMI causes the manual input facilities of a device to be disabled.

TODO : Consider implementing (need more information)

#### DSR - DEVICE STATUS REPORT

> DSR is used either to report the status of the sending device or to request a status report from the receiving device, depending on the parameter values:
>
> 0) ready, no malfunction detected
> 1) busy, another DSR must be requested later
> 2) busy, another DSR will be sent later
> 3) some malfunction detected, another DSR must be requested later
> 4) some malfunction detected, another DSR will be sent later
> 5) a DSR is requested
> 6) a report of the active presentation position or of the active data position in the form of ACTIVE POSITION > REPORT (CPR) is requested
>
> DSR with parameter value 0, 1, 2, 3 or 4 may be sent either unsolicited or as a response to a request such as a DSR with a parameter value 5 or MESSAGE WAITING (MW).

TODO : Complete implementation (Complexity medium)

#### 8.3.36 DTA - DIMENSION TEXT AREA

> DTA is used to establish the dimensions of the text area for subsequent pages.
>
> The established dimensions remain in effect until the next occurrence of DTA in the data > stream.
>
> Pn1) specifies the dimension in the direction perpendicular to the line orientation
> Pn2) specifies the dimension in the direction parallel to the line orientation
>
> The unit in which the parameter value is expressed is that established by the parameter value of SELECT SIZE UNIT (SSU).

TODO : Consider implementation (complexity high)

#### 8.3.37 EA - ERASE IN AREA

SEE : Standard, too much to copy here

TODO : Consider implementing (complexity high, reason DCSM)

#### 8.3.40 EF - ERASE IN FIELD

SEE : Standard, too much to copy here

TODO : Consider implementing (complexity high, reason DCSM)

#### 8.3.42 EM - END OF MEDIUM

> EM is used to identify the physical end of a medium, or the end of the used portion of a medium, or the
> end of the wanted portion of data recorded on a medium. 

TODO : Will research what this means and find out whether other terminals have a reference to it

#### 8.3.43 EMI - ENABLE MANUAL INPUT

> EMI is used to enable the manual input facilities of a device. 

TODO : Will research this further

#### 8.3.44 ENQ - ENQUIRY

> ENQ is transmitted by a sender as a request for a response from a receiver.
> The use of ENQ is defined in ISO 1745.

TODO : Attempt to buy or borrow ISO 1745 and see whether this is interesting

#### 8.3.45 EOT - END OF TRANSMISSION

> EOT is used to indicate the conclusion of the transmission of one or more texts.
> The use of EOT is defined in ISO 1745.

TODO : Attempt to buy or borrow ISO 1745 and see whether this is interesting

#### 8.3.46 EPA - END OF GUARDED AREA

> EPA is used to indicate that the active presentation position is the last of a string of character positions
> in the presentation component, the contents of which are protected against manual alteration, are
> guarded against transmission or transfer, depending on the setting of the GUARDED AREA TRANSFER
> MODE (GATM), and may be protected against erasure, depending on the setting of the ERASURE
> MODE (ERM). The beginning of this string is indicated by START OF GUARDED AREA (SPA). 

TODO : Consider implementing (complexity high, reason GATM)

#### 8.3.47 ESA - END OF SELECTED AREA

> ESA is used to indicate that the active presentation position is the last of a string of character positions
> in the presentation component, the contents of which are eligible to be transmitted in the form of a data
> stream or transferred to an auxiliary input/output device. The beginning of this string is indicated by
> START OF SELECTED AREA (SSA). 

TODO : Consider implementing (complexity medium, reason selected area feature)

#### 8.3.49 ETB - END OF TRANSMISSION BLOCK

> ETB is used to indicate the end of a block of data where the data are divided into such blocks for
> transmission purposes.
> 
> The use of ETB is defined in ISO 1745. 

TODO : Attempt to buy or borrow ISO 1745 and see whether this is interesting

#### 8.3.50 ETX - END OF TEXT

> ETX is used to indicate the end of a text.
> 
> The use of ETX is defined in ISO 1745. 

TODO : Attempt to buy or borrow ISO 1745 and see whether this is interesting

#### 8.3.52 FNK - FUNCTION KEY

> FNK is a control function in which the parameter value identifies the function key which has been
> operated.

Most likely won't implement, though may do so.

#### 8.3.53 FNT - FONT SELECTION

> FNT is used to identify the character font to be selected as primary or alternative font by subsequent
> occurrences of SELECT GRAPHIC RENDITION (SGR) in the data stream. 

TODO : Looks like fun, probably won't implement

#### 8.3.54 GCC - GRAPHIC CHARACTER COMBINATION

> GCC is used to indicate that two or more graphic characters are to be imaged as one single graphic
> symbol. GCC with a parameter value of 0 indicates that the following two graphic characters are to be
> imaged as one single graphic symbol; GCC with a parameter value of 1 and GCC with a parameter value
> of 2 indicate respectively the beginning and the end of a string of graphic characters which are to be
> imaged as one single graphic symbol. 

TODO : Consider implementation for Japanese and Korean

#### 8.3.55 GSM - GRAPHIC SIZE MODIFICATION

> GSM is used to modify for subsequent text the height and/or the width of all primary and alternative
> fonts identified by FONT SELECTION (FNT) and established by GRAPHIC SIZE SELECTION (GSS).
> The established values remain in effect until the next occurrence of GSM or GSS in the data steam.
> 
> Pn1 specifies the height as a percentage of the height established by GSS
> Pn2 specifies the width as a percentage of the width established by GSS 

TODO : Consider implementation for Japanese and Korean

#### 8.3.56 GSS - GRAPHIC SIZE SELECTION

> GSS is used to establish for subsequent text the height and the width of all primary and alternative fonts
> identified by FONT SELECTION (FNT). The established values remain in effect until the next
> occurrence of GSS in the data stream.
> 
> Pn specifies the height, the width is implicitly defined by the height.
> 
> The unit in which the parameter value is expressed is that established by the parameter value of SELECT
> SIZE UNIT (SSU). 

TODO : Consider implementation for Japanese and Korean

#### 8.3.61 HTJ - CHARACTER TABULATION WITH JUSTIFICATION

> HTJ causes the contents of the active field (the field in the presentation component that contains the
> active presentation position) to be shifted forward so that it ends at the character position preceding the
> following character tabulation stop. The active presentation position is moved to that following character
> tabulation stop. The character positions which precede the beginning of the shifted string are put into the
> erased state

TODO : Consider implementation if field representation is implemented (complexity high)

#### 8.3.65 IDCS - IDENTIFY DEVICE CONTROL STRING

> IDCS is used to specify the purpose and format of the command string of subsequent DEVICE
> CONTROL STRINGs (DCS). The specified purpose and format remain in effect until the next
> occurrence of IDCS in the data stream.
> 
> The parameter values are
> 
> 1) reserved for use with the DIAGNOSTIC state of the STATUS REPORT TRANSFER MODE (SRTM)
> 2) reserved for Dynamically Redefinable Character Sets (DRCS) according to Standard ECMA-35.
> 
> The format and interpretation of the command string corresponding to these parameter values are to be
> defined in appropriate standards. If this control function is used to identify a private command string, a
> private parameter value shall be used. 

TODO : Most likely won't bother implementing unless I encounter it

#### 8.3.66 IGS - IDENTIFY GRAPHIC SUBREPERTOIRE

> IGS is used to indicate that a repertoire of the graphic characters of ISO/IEC 10367 is used in the
> subsequent text.
> 
> The parameter value of IGS identifies a graphic character repertoire registered in accordance with
> ISO/IEC 7350.

TODO : Buy/borrow ISO 10367 and 7350 and figure out what this means

#### 8.3.68 INT - INTERRUPT

> INT is used to indicate to the receiving device that the current process is to be interrupted and an agreed
> procedure is to be initiated. This control function is applicable to either direction of transmission. 

TODO : Is this the break sequence for Cisco devices?

#### 8.3.69 IS1 - INFORMATION SEPARATOR ONE (US - UNIT SEPARATOR) 

Won't implement

#### 8.3.70 IS2 - INFORMATION SEPARATOR TWO (RS - RECORD SEPARATOR) 

Won't implement

#### 8.3.71 IS3 - INFORMATION SEPARATOR THREE (GS - GROUP SEPARATOR)

Won't implement

#### 8.3.72 IS4 - INFORMATION SEPARATOR FOUR (FS - FILE SEPARATOR)

Won't implement

#### 8.3.73 JFY - JUSTIFY

> JFY is used to indicate the beginning of a string of graphic characters in the presentation component that
> are to be justified according to the layout specified by the parameter values, see annex C:
> 
> 0) no justification, end of justification of preceding text
> 1) word fill
> 2) word space
> 3) letter space
> 4) hyphenation
> 5) flush to line home position margin
> 6) centre between line home position and line limit position margins
> 7) flush to line limit position margin
> 8) Italian hyphenation
> 
> The end of the string to be justified is indicated by the next occurrence of JFY in the data stream. 
> 
> The line home position is established by the parameter value of SET LINE HOME (SLH). The line limit
> position is established by the parameter value of SET LINE LIMIT (SLL).

TODO : Identify whether this is interesting

#### 8.3.82 MC - MEDIA COPY

> MC is used either to initiate a transfer of data from or to an auxiliary input/output device or to enable or
> disable the relay of the received data stream to an auxiliary input/output device, depending on the
> parameter value:
> 
> 0) initiate transfer to a primary auxiliary device
> 1) initiate transfer from a primary auxiliary device
> 2) initiate transfer to a secondary auxiliary device
> 3) initiate transfer from a secondary auxiliary device
> 4) stop relay to a primary auxiliary device
> 5) start relay to a primary auxiliary device
> 6) stop relay to a secondary auxiliary device
> 7) start relay to a secondary auxiliary device
> 
> This control function may not be used to switch on or off an auxiliary device.

TODO : Not on my current list... this is printing support

#### 8.3.83 MW - MESSAGE WAITING

> MW is used to set a message waiting indicator in the receiving device. An appropriate acknowledgement
> to the receipt of MW may be given by using DEVICE STATUS REPORT (DSR). 

TODO : Could be interesting will look into this

#### 8.3.84 NAK - NEGATIVE ACKNOWLEDGE

> NAK is transmitted by a receiver as a negative response to the sender.
> The use of NAK is defined in ISO 1745. 

TODO : Attempt to buy or borrow ISO 1745 and see whether this is interesting

#### 8.3.85 NBH - NO BREAK HERE

> NBH is used to indicate a point where a line break shall not occur when text is formatted. NBH may
> occur between two graphic characters either or both of which may be SPACE.

TODO : Not sure whether this is relevant. Will check

#### 8.3.87 NP - NEXT PAGE

> NP causes the n-th following page in the presentation component to be displayed, where n equals the
> value of Pn.
> 
> The effect of this control function on the active presentation position is not defined by this Standard.

Most likely will not implement as it seems to be focused on printing

#### 8.3.90 PEC - PRESENTATION EXPAND OR CONTRACT

> PEC is used to establish the spacing and the extent of the graphic characters for subsequent text. The
> spacing is specified in the line as multiples of the spacing established by the most recent occurrence of
> SET CHARACTER SPACING (SCS) or of SELECT CHARACTER SPACING (SHS) or of SPACING
> INCREMENT (SPI) in the data stream. The extent of the characters is implicitly established by these 
> control functions. The established spacing and the extent remain in effect until the next occurrence of
> PEC, of SCS, of SHS or of SPI in the data stream. The parameter values are
> 
> 0) normal (as specified by SCS, SHS or SPI)
> 1) expanded (multiplied by a factor not greater than 2)
> 2) condensed (multiplied by a factor not less than 0,5)

Unlikely to implement unless I decide to support half-width characters. This would make portability far more
complex.

#### 8.3.91 PFS - PAGE FORMAT SELECTION 

See standard, too much to quote

Will not implement any time soon as it is about page formatting on printers. It may be interesting at some
point, but highly unlikely.

#### 8.3.92 PLD - PARTIAL LINE FORWARD

> PLD causes the active presentation position to be moved in the presentation component to the
> corresponding position of an imaginary line with a partial offset in the direction of the line progression.
> This offset should be sufficient either to image following characters as subscripts until the first
> following occurrence of PARTIAL LINE BACKWARD (PLU) in the data stream, or, if preceding
> characters were imaged as superscripts, to restore imaging of following characters to the active line (the
> line that contains the active presentation position).
> 
> Any interactions between PLD and format effectors other than PLU are not defined by this Standard.

I honestly have no idea what this means.

#### 8.3.93 PLU - PARTIAL LINE BACKWARD

> PLU causes the active presentation position to be moved in the presentation component to the
> corresponding position of an imaginary line with a partial offset in the direction opposite to that of the
> line progression. This offset should be sufficient either to image following characters as superscripts
> until the first following occurrence of PARTIAL LINE FORWARD (PLD) in the data stream, or, if
> preceding characters were imaged as subscripts, to restore imaging of following characters to the active
> line (the line that contains the active presentation position).
> 
> Any interactions between PLU and format effectors other than PLD are not defined by this Standard.

I honestly have no idea what this means.

#### 8.3.94 PM - PRIVACY MESSAGE

> PM is used as the opening delimiter of a control string for privacy message use. The command string
> following may consist of a sequence of bit combinations in the range 00/08 to 00/13 and 02/00 to 07/14.
> The control string is closed by the terminating delimiter STRING TERMINATOR (ST). The
> interpretation of the command string depends on the relevant privacy discipline. 

This may be an interesting form of out of band communication, but unless I encounter it elsewhere,
it is unlikely it will be of interest.

#### 8.3.95 PP - PRECEDING PAGE

> PP causes the n-th preceding page in the presentation component to be displayed, where n equals the
> value of Pn. The effect of this control function on the active presentation position is not defined by this
> Standard.

TODO : Figure out what presentation page means in the ECMA standard.

#### 8.3.96 PPA - PAGE POSITION ABSOLUTE

> PPA causes the active data position to be moved in the data component to the corresponding character
> position on the n-th page, where n equals the value of Pn. 

TODO : Figure out what presentation page means in the ECMA standard.

#### 8.3.96 PPA - PAGE POSITION ABSOLUTE

> PPA causes the active data position to be moved in the data component to the corresponding character
> position on the n-th page, where n equals the value of Pn.

TODO : Figure out what presentation page means in the ECMA standard.

#### 8.3.97 PPB - PAGE POSITION BACKWARD

> PPB causes the active data position to be moved in the data component to the corresponding character
> position on the n-th preceding page, where n equals the value of Pn.

TODO : Figure out what presentation page means in the ECMA standard.

#### 8.3.98 PPR - PAGE POSITION FORWARD

> PPR causes the active data position to be moved in the data component to the corresponding character
> position on the n-th following page, where n equals the value of Pn.

TODO : Figure out what presentation page means in the ECMA standard.

####  8.3.99 PTX - PARALLEL TEXTS

See standard, too much to quote

TODO : Consider if supporting CJK

#### 8.3.100 PU1 - PRIVATE USE ONE

> PU1 is reserved for a function without standardized meaning for private use as required, subject to the
> prior agreement between the sender and the recipient of the data.

Not implemented, basically a standard way of being proprietary

#### 8.3.101 PU2 - PRIVATE USE TWO

> PU2 is reserved for a function without standardized meaning for private use as required, subject to the
> prior agreement between the sender and the recipient of the data. 

Not implemented, basically a standard way of being proprietary

#### 8.3.102 QUAD - QUAD

> QUAD is used to indicate the end of a string of graphic characters that are to be positioned on a single
> line according to the layout specified by the parameter values, see annex C:
> 
> 0)  flush to line home position margin
> 1)  flush to line home position margin and fill with leader
> 2)  centre between line home position and line limit position margins
> 3)  centre between line home position and line limit position margins and fill with leader
> 4)  flush to line limit position margin
> 5)  flush to line limit position margin and fill with leader
> 6)  flush to both margins

See the documentation for more information.

TODO : Consider implementation as it seems to be useful for defining margins in a single command.

#### 8.3.106 RM - RESET MODE

> RM causes the modes of the receiving device to be reset as specified by the parameter values:
> 
> 1) GUARDED AREA TRANSFER MODE (GATM)
> 2) KEYBOARD ACTION MODE (KAM)
> 3) CONTROL REPRESENTATION MODE (CRM)
> 4) INSERTION REPLACEMENT MODE (IRM)
> 5) STATUS REPORT TRANSFER MODE (SRTM)
> 6) ERASURE MODE (ERM)
> 7) LINE EDITING MODE (VEM)
> 8) BI-DIRECTIONAL SUPPORT MODE (BDSM)
> 9) DEVICE COMPONENT SELECT MODE (DCSM)
> 10) CHARACTER EDITING MODE (HEM)
> 11) POSITIONING UNIT MODE (PUM) (see F.4.1 in annex F)
> 12) SEND/RECEIVE MODE (SRM)
> 13) FORMAT EFFECTOR ACTION MODE (FEAM)
> 14) FORMAT EFFECTOR TRANSFER MODE (FETM)
> 15) MULTIPLE AREA TRANSFER MODE (MATM)
> 16) TRANSFER TERMINATION MODE (TTM)
> 17) SELECTED AREA TRANSFER MODE (SATM)
> 18) TABULATION STOP MODE (TSM)
> 19) (Shall not be used; see F.5.1 in annex F)
> 20) (Shall not be used; see F.5.2 in annex F)
> 21) GRAPHIC RENDITION COMBINATION MODE (GRCM)
> 22) ZERO DEFAULT MODE (ZDM) (see F.4.2 in annex F)

Currently only modes 4 and 20 are implemented. 

TODO : Implement more and also see wht is said in F.5.2 when able

#### 8.3.107 SACS - SET ADDITIONAL CHARACTER SEPARATION

> SACS is used to establish extra inter-character escapement for subsequent text. The established extra
> escapement remains in effect until the next occurrence of SACS or of SET REDUCED CHARACTER
> SEPARATION (SRCS) in the data stream or until it is reset to the default value by a subsequent
> occurrence of CARRIAGE RETURN/LINE FEED (CR LF) or of NEXT LINE (NEL) in the data stream,
> see annex C.
> 
> Pn specifies the number of units by which the inter-character escapement is enlarged.
> 
> The unit in which the parameter value is expressed is that established by the parameter value of SELECT
> SIZE UNIT (SSU). 

I don't know what this means and will have to read this somewhere.

#### 8.3.108 SAPV - SELECT ALTERNATIVE PRESENTATION VARIANTS

[TL;DR](https://www.urbandictionary.com/define.php?term=tl%3Bdr)

TODO: Read this at some point and figure out whether it matters.

#### 8.3.110 SCO - SELECT CHARACTER ORIENTATION

> SCO is used to establish the amount of rotation of the graphic characters following in the data stream.
> The established value remains in effect until the next occurrence of SCO in the data stream.
> The parameter values are
> 
> 0) 0°
> 1) 45°
> 2) 90°
> 3) 135°
> 4) 180°
> 5) 225°
> 6) 270°
> 7) 315°
> 
> Rotation is positive, i.e. counter-clockwise and applies to the normal presentation of the graphic
> characters along the character path. The centre of rotation of the affected graphic characters is not
> defined by this Standard.

TODO : Don't need it, but really want it.... thought SCO was some stupid "System V broke everything" thing
and then I saw this and realized... COOL. I have to do this.

#### 8.3.111 SCP - SELECT CHARACTER PATH 

See the documentation for details

TODO : Consider implementing when Bidi becomes interesting.


#### 8.3.112 SCS - SET CHARACTER SPACING

> SCS is used to establish the character spacing for subsequent text. The established spacing remains in
> effect until the next occurrence of SCS, or of SELECT CHARACTER SPACING (SHS) or of SPACING
> INCREMENT (SPI) in the data stream, see annex C.
> 
> Pn specifies the character spacing.
> 
> The unit in which the parameter value is expressed is that established by the parameter value of SELECT
> SIZE UNIT (SSU). 

TODO : Find out whether this is interesting or not.

#### 8.3.114 SDS - START DIRECTED STRING

This will never have bidi so long as the documentation for every feature is that long. 

TODO : Device whether I am willing to bore myself reading that much to find out 1=l-to-r and 2=r-to-l or
something silly like that.

#### 8.3.115 SEE - SELECT EDITING EXTENT

> SEE is used to establish the editing extent for subsequent character or line insertion or deletion. The
> established extent remains in effect until the next occurrence of SEE in the data stream. The editing
> extent depends on the parameter value:
> 
> 0) the shifted part is limited to the active page in the presentation component
> 1) the shifted part is limited to the active line in the presentation component
> 2) the shifted part is limited to the active field in the presentation component
> 3) the shifted part is limited to the active qualified area
> 4) the shifted part consists of the relevant part of the entire presentation component.

TODO : Consider implementation. This one seems useful.

#### 8.3.116 SEF - SHEET EJECT AND FEED 

See standard for this one

Will not likely implement as it's printing related.


#### 8.3.117 SGR - SELECT GRAPHIC RENDITION

> SGR is used to establish one or more graphic rendition aspects for subsequent text. The established
> aspects remain in effect until the next occurrence of SGR in the data stream, depending on the setting of
> the GRAPHIC RENDITION COMBINATION MODE (GRCM). Each graphic rendition aspect is
> specified by a parameter value:
> 
> 0) default rendition (implementation-defined), cancels the effect of any preceding occurrence of SGR in
> the data stream regardless of the setting of the GRAPHIC RENDITION COMBINATION MODE
> (GRCM)
> 1) bold or increased intensity
> 2) faint, decreased intensity or second colour
> 3) italicized
> 4) singly underlined
> 5) slowly blinking (less then 150 per minute)
> 6) rapidly blinking (150 per minute or more)
> 7) negative image
> 8) concealed characters
> 9) crossed-out (characters still legible but marked as to be deleted)
> 10) primary (default) font
> 11) first alternative font
> 12) second alternative font
> 13) third alternative font
> 14) fourth alternative font
> 15) fifth alternative font
> 16) sixth alternative font
> 17) seventh alternative font
> 18) eighth alternative font
> 19) ninth alternative font
> 20) Fraktur (Gothic)
> 21) doubly underlined
> 22) normal colour or normal intensity (neither bold nor faint)
> 23) not italicized, not fraktur
> 24) not underlined (neither singly nor doubly)
> 25) steady (not blinking)
> 26) (reserved for proportional spacing as specified in CCITT Recommendation T.61)
> 27) positive image
> 28) revealed characters 
> 29) not crossed out
> 30) black display
> 31) red display
> 32) green display
> 33) yellow display
> 34) blue display
> 35) magenta display
> 36) cyan display
> 37) white display
> 38) (reserved for future standardization; intended for setting character foreground colour as specified in ISO 8613-6 [CCITT Recommendation T.416])
> 39) default display colour (implementation-defined)
> 40) black background
> 41) red background
> 42) green background
> 43) yellow background
> 44) blue background
> 45) magenta background
> 46) cyan background
> 47) white background
> 48) (reserved for future standardization; intended for setting character background colour as specified in ISO 8613-6 [CCITT Recommendation T.416])
> 49) default background colour (implementation-defined)
> 50) (reserved for cancelling the effect of the rendering aspect established by parameter value 26)
> 51) framed
> 52) encircled
> 53) overlined
> 54) not framed, not encircled
> 55) not overlined
> 56) (reserved for future standardization)
> 57) (reserved for future standardization)
> 58) (reserved for future standardization)
> 59) (reserved for future standardization)
> 60) ideogram underline or right side line
> 61) ideogram double underline or double line on the right side
> 62) ideogram overline or left side line
> 63) ideogram double overline or double line on the left side
> 64) ideogram stress marking
> 65) cancels the effect of the rendition aspects established by parameter values 60 to 64 
> 
> NOTE
> The usable combinations of parameter values are determined by the implementation.

TODO : Make a separate table/document for this one.

#### 8.3.118 SHS - SELECT CHARACTER SPACING

> SHS is used to establish the character spacing for subsequent text. The established spacing remains in
> effect until the next occurrence of SHS or of SET CHARACTER SPACING (SCS) or of SPACING
> INCREMENT (SPI) in the data stream. The parameter values are
> 
> 0) 10 characters per 25,4 mm
> 1) 12 characters per 25,4 mm
> 2) 15 characters per 25,4 mm
> 3) 6 characters per 25,4 mm
> 4) 3 characters per 25,4 mm
> 5) 9 characters per 50,8 mm
> 6) 4 characters per 25,4 mm

This is making a little more sense now. It seems to be related to print size. I'll see if I can find an old book on the topic
written on things like unix programming where printing is concerned but predating postscript.

#### 8.3.120 SIMD - SELECT IMPLICIT MOVEMENT DIRECTION

> SIMD is used to select the direction of implicit movement of the data position relative to the character
> progression. The direction selected remains in effect until the next occurrence of SIMD.
> The parameter values are:
> 
> 0) the direction of implicit movement is the same as that of the character progression
> 1) the direction of implicit movement is opposite to that of the character progression.

There are times where having a picture which describes these things would have been far more useful. I'll consider what this
means and then see if I can find something which I can use as a reference... this of course depends on whether I want to
bother with bidi.

#### 8.3.121 SL - SCROLL LEFT

> SL causes the data in the presentation component to be moved by n character positions if the line
> orientation is horizontal, or by n line positions if the line orientation is vertical, such that the data appear
> to move to the left; where n equals the value of Pn.
> 
> The active presentation position is not affected by this control function.

TODO : On my list to implement. It could be a bit difficult, but I want it in there and in the unit tests before
I start optimizing the scrolling and text layout engine for the model.

#### 8.3.122 SLH - SET LINE HOME

> If the DEVICE COMPONENT SELECT MODE is set to PRESENTATION, SLH is used to establish at
> character position n in the active line (the line that contains the active presentation position) and lines of
> subsequent text in the presentation component the position to which the active presentation position will
> be moved by subsequent occurrences of CARRIAGE RETURN (CR), DELETE LINE (DL), INSERT
> LINE (IL) or NEXT LINE (NEL) in the data stream; where n equals the value of Pn. In the case of a
> device without data component, it is also the position ahead of which no implicit movement of the active
> presentation position shall occur.
> 
> If the DEVICE COMPONENT SELECT MODE is set to DATA, SLH is used to establish at character
> position n in the active line (the line that contains the active data position) and lines of subsequent text
> in the data component the position to which the active data position will be moved by subsequent
> occurrences of CARRIAGE RETURN (CR), DELETE LINE (DL), INSERT LINE (IL) or NEXT LINE
> (NEL) in the data stream; where n equals the value of Pn. It is also the position ahead of which no
> implicit movement of the active data position shall occur.
> 
> The established position is called the line home position and remains in effect until the next occurrence
> of SLH in the data stream.

TODO : When I spend some more time identifying where I've got DCSM sorted out, I'll look into this.

#### 8.3.123 SLL - SET LINE LIMIT

> If the DEVICE COMPONENT SELECT MODE is set to PRESENTATION, SLL is used to establish at
> character position n in the active line (the line that contains the active presentation position) and lines of
> subsequent text in the presentation component the position to which the active presentation position will
> be moved by subsequent occurrences of CARRIAGE RETURN (CR), or NEXT LINE (NEL) in the data
> stream if the parameter value of SELECT IMPLICIT MOVEMENT DIRECTION (SIMD) is equal to 1;
> where n equals the value of Pn. In the case of a device without data component, it is also the position
> beyond which no implicit movement of the active presentation position shall occur.
> 
> If the DEVICE COMPONENT SELECT MODE is set to DATA, SLL is used to establish at character
> position n in the active line (the line that contains the active data position) and lines of subsequent text
> in the data component the position beyond which no implicit movement of the active data position shall
> occur. It is also the position in the data component to which the active data position will be moved by
> subsequent occurrences of CR or NEL in the data stream, if the parameter value of SELECT IMPLICIT
> MOVEMENT DIRECTION (SIMD) is equal to 1.
> 
> The established position is called the line limit position and remains in effect until the next occurrence
> of SLL in the data stream.

TODO : When I spend some more time identifying where I've got DCSM sorted out, I'll look into this.

#### 8.3.124 SLS - SET LINE SPACING

> SLS is used to establish the line spacing for subsequent text. The established spacing remains in effect
> until the next occurrence of SLS or of SELECT LINE SPACING (SVS) or of SPACING INCREMENT
> (SPI) in the data stream.
> 
> Pn specifies the line spacing.
> 
> The unit in which the parameter value is expressed is that established by the parameter value of SELECT
> SIZE UNIT (SSU). 

Pretty sure this is printing related.

#### 8.3.125 SM - SET MODE

> SM causes the modes of the receiving device to be set as specified by the parameter values:
> 
> 1) GUARDED AREA TRANSFER MODE (GATM)
> 2) KEYBOARD ACTION MODE (KAM)
> 3) CONTROL REPRESENTATION MODE (CRM)
> 4) INSERTION REPLACEMENT MODE (IRM)
> 5) STATUS REPORT TRANSFER MODE (SRTM)
> 6) ERASURE MODE (ERM)
> 7) LINE EDITING MODE (VEM)
> 8) BI-DIRECTIONAL SUPPORT MODE (BDSM)
> 9) DEVICE COMPONENT SELECT MODE (DCSM)
> 10) CHARACTER EDITING MODE (HEM)
> 11) POSITIONING UNIT MODE (PUM) (see F.4.1 in annex F)
> 12) SEND/RECEIVE MODE (SRM)
> 13) FORMAT EFFECTOR ACTION MODE (FEAM)
> 14) FORMAT EFFECTOR TRANSFER MODE (FETM)
> 15) MULTIPLE AREA TRANSFER MODE (MATM)
> 16) TRANSFER TERMINATION MODE (TTM)
> 17) SELECTED AREA TRANSFER MODE (SATM)
> 18) TABULATION STOP MODE (TSM)
> 19) (Shall not be used; see F.5.1 in annex F)
> 20) (Shall not be used; see F.5.2 in annex F)
> 21) GRAPHIC RENDITION COMBINATION (GRCM)
> 22) ZERO DEFAULT MODE (ZDM) (see F.4.2 in annex F)
> 
> NOTE
> Private modes may be implemented using private parameters, see 5.4.1 and 7.4

Currently only mode 4 and 20 are implemented.

TODO : Look more into getting the other features of interest implemented.

#### 8.3.127 SOH - START OF HEADING

> SOH is used to indicate the beginning of a heading.
> 
> The use of SOH is defined in ISO 1745.

TODO : Read ISO1745 and identify whether this is interesting

#### 8.3.128 SOS - START OF STRING

> SOS is used as the opening delimiter of a control string. The character string following may consist of
> any bit combination, except those representing SOS or STRING TERMINATOR (ST). The control string
> is closed by the terminating delimiter STRING TERMINATOR (ST). The interpretation of the character
> string depends on the application.

TODO : Look more into whether this is interesting. I've implemented ST, but I don't know whether SOS is
meaningful.

#### 8.3.129 SPA - START OF GUARDED AREA

> SPA is used to indicate that the active presentation position is the first of a string of character positions
> in the presentation component, the contents of which are protected against manual alteration, are
> guarded against transmission or transfer, depending on the setting of the GUARDED AREA TRANSFER
> MODE (GATM) and may be protected against erasure, depending on the setting of the ERASURE
> MODE (ERM). The end of this string is indicated by END OF GUARDED AREA (EPA).
> 
> NOTE
> The control functions for area definition (DAQ, EPA, ESA, SPA, SSA) should not be used within an SRS
> string or an SDS string.

TODO : Consider implementing GATM

#### 8.3.130 SPD - SELECT PRESENTATION DIRECTIONS

[TL;DR](https://www.urbandictionary.com/define.php?term=tl%3Bdr)

TODO: Read this at some point and figure out whether it matters.

#### 8.3.131 SPH - SET PAGE HOME

> If the DEVICE COMPONENT SELECT MODE is set to PRESENTATION, SPH is used to establish at
> line position n in the active page (the page that contains the active presentation position) and subsequent
> pages in the presentation component the position to which the active presentation position will be moved
> by subsequent occurrences of FORM FEED (FF) in the data stream; where n equals the value of Pn. In
> the case of a device without data component, it is also the position ahead of which no implicit movement
> of the active presentation position shall occur.
> 
> If the DEVICE COMPONENT SELECT MODE is set to DATA, SPH is used to establish at line position
> n in the active page (the page that contains the active data position) and subsequent pages in the data
> component the position to which the active data position will be moved by subsequent occurrences of
> FORM FEED (FF) in the data stream; where n equals the value of Pn. It is also the position ahead of
> which no implicit movement of the active presentation position shall occur.
> 
> The established position is called the page home position and remains in effect until the next occurrence
> of SPH in the data stream.

TODO : Consider implementing as DCSM

#### 8.3.132 SPI - SPACING INCREMENT

> SPI is used to establish the line spacing and the character spacing for subsequent text. The established
> line spacing remains in effect until the next occurrence of SPI or of SET LINE SPACING (SLS) or of
> SELECT LINE SPACING (SVS) in the data stream. The established character spacing remains in effect
> until the next occurrence of SET CHARACTER SPACING (SCS) or of SELECT CHARACTER
> SPACING (SHS) in the data stream, see annex C.
> 
> * Pn1 specifies the line spacing
> * Pn2 specifies the character spacing 
> 
> The unit in which the parameter values are expressed is that established by the parameter value of
> SELECT SIZE UNIT (SSU).

TODO : Consider if necessary when looking at half-width scripts

#### 8.3.133 SPL - SET PAGE LIMIT

> If the DEVICE COMPONENT SELECT MODE is set to PRESENTATION, SPL is used to establish at
> line position n in the active page (the page that contains the active presentation position) and pages of
> subsequent text in the presentation component the position beyond which the active presentation position
> can normally not be moved; where n equals the value of Pn. In the case of a device without data
> component, it is also the position beyond which no implicit movement of the active presentation position
> shall occur.
> 
> If the DEVICE COMPONENT SELECT MODE is set to DATA, SPL is used to establish at line position
> n in the active page (the page that contains the active data position) and pages of subsequent text in the
> data component the position beyond which no implicit movement of the active data position shall occur.
> The established position is called the page limit position and remains in effect until the next occurrence
> of SPL in the data stream.

TODO : Consider implementing as DCSM

#### 8.3.134 SPQR - SELECT PRINT QUALITY AND RAPIDITY

> SPQR is used to select the relative print quality and the print speed for devices the output quality and
> speed of which are inversely related. The selected values remain in effect until the next occurrence of
> SPQR in the data stream. The parameter values are
> 
> 0) highest available print quality, low print speed
> 1) medium print quality, medium print speed
> 2) draft print quality, highest available print speed

Clearly printing related and REALLY REALLY never going to be interesting

#### 8.3.135 SR - SCROLL RIGHT

> SR causes the data in the presentation component to be moved by n character positions if the line
> orientation is horizontal, or by n line positions if the line orientation is vertical, such that the data appear
> to move to the right; where n equals the value of Pn.
> 
> The active presentation position is not affected by this control function. 

TODO : Absolutely will implement ASAP :)

#### 8.3.136 SRCS - SET REDUCED CHARACTER SEPARATION

> SRCS is used to establish reduced inter-character escapement for subsequent text. The established
> reduced escapement remains in effect until the next occurrence of SRCS or of SET ADDITIONAL
> CHARACTER SEPARATION (SACS) in the data stream or until it is reset to the default value by a
> subsequent occurrence of CARRIAGE RETURN/LINE FEED (CR/LF) or of NEXT LINE (NEL) in the
> data stream, see annex C.
> 
> Pn specifies the number of units by which the inter-character escapement is reduced. 
> 
> The unit in which the parameter values are expressed is that established by the parameter value of
> SELECT SIZE UNIT (SSU).

Most likely won't implement.

#### 8.3.137 SRS - START REVERSED STRING

[TL;DR](https://www.urbandictionary.com/define.php?term=tl%3Bdr)

No point wasting time on this. Pretty sure I have given up on bidi support by now.

#### 8.3.138 SSA - START OF SELECTED AREA

> SSA is used to indicate that the active presentation position is the first of a string of character positions
> in the presentation component, the contents of which are eligible to be transmitted in the form of a data
> stream or transferred to an auxiliary input/output device.
> 
> The end of this string is indicated by END OF SELECTED AREA (ESA). The string of characters
> actually transmitted or transferred depends on the setting of the GUARDED AREA TRANSFER MODE
> (GATM) and on any guarded areas established by DEFINE AREA QUALIFICATION (DAQ), or by
> START OF GUARDED AREA (SPA) and END OF GUARDED AREA (EPA).
> 
> NOTE
> The control functions for area definition (DAQ, EPA, ESA, SPA, SSA) should not be used within an SRS
> string or an SDS string.

TODO : Look into whether this have value.

#### 8.3.139 SSU - SELECT SIZE UNIT

> SSU is used to establish the unit in which the numeric parameters of certain control functions are
> expressed. The established unit remains in effect until the next occurrence of SSU in the data stream. 
> 
> The parameter values are
> 0)  CHARACTER - The dimensions of this unit are device-dependent
> 1)  MILLIMETRE
> 2)  COMPUTER DECIPOINT - 0,035 28 mm (1/720 of 25,4 mm)
> 3)  DECIDIDOT - 0,037 59 mm (10/266 mm)
> 4)  MIL - 0,025 4 mm (1/1 000 of 25,4 mm)
> 5)  BASIC MEASURING UNIT (BMU) - 0,021 17 mm (1/1 200 of 25,4 mm)
> 6)  MICROMETRE - 0,001 mm
> 7)  PIXEL - The smallest increment that can be specified in a device
> 8)  DECIPOINT - 0,035 14 mm (35/996 mm)

Not sure whether this has value. Will consider.

#### 8.3.140 SSW - SET SPACE WIDTH

> SSW is used to establish for subsequent text the character escapement associated with the character
> SPACE. The established escapement remains in effect until the next occurrence of SSW in the data
> stream or until it is reset to the default value by a subsequent occurrence of CARRIAGE RETURN/LINE
> FEED (CR/LF), CARRIAGE RETURN/FORM FEED (CR/FF), or of NEXT LINE (NEL) in the data
> stream, see annex C.
> 
> Pn specifies the escapement.
> 
> The unit in which the parameter value is expressed is that established by the parameter value of SELECT
> SIZE UNIT (SSU).
> 
> The default character escapement of SPACE is specified by the most recent occurrence of SET
> CHARACTER SPACING (SCS) or of SELECT CHARACTER SPACING (SHS) or of SELECT
> SPACING INCREMENT (SPI) in the data stream if the current font has constant spacing, or is specified
> by the nominal width of the character SPACE in the current font if that font has proportional spacing.

Pretty sure this is printing related and of value for making things pretty. I'm not sure whether this
has value in my program.

#### 8.3.144 STAB - SELECTIVE TABULATION

> STAB causes subsequent text in the presentation component to be aligned according to the position and
> the properties of a tabulation stop which is selected from a list according to the value of the parameter
> Ps.
> 
> The use of this control function and means of specifying a list of tabulation stops to be referenced by the
> control function are specified in other standards, for example ISO 8613-6.

TODO : Get a copy of ISO 8613-6 and identify whether this is interesting

#### 8.3.145 STS - SET TRANSMIT STATE

> STS is used to establish the transmit state in the receiving device. In this state the transmission of data
> from the device is possible.
> The actual initiation of transmission of data is performed by a data communication or input/output
> interface control procedure which is outside the scope of this Standard.
> 
> The transmit state is established either by STS appearing in the received data stream or by the operation
> of an appropriate key on a keyboard.

TODO : Consider whether this is interesting for serial communication

#### 8.3.146 STX - START OF TEXT

> STX is used to indicate the beginning of a text and the end of a heading.
> 
> The use of STX is defined in ISO 1745.

TODO : Refer to ISO 1745 and identify whether this is interesting

#### 8.3.148 SUB - SUBSTITUTE

> SUB is used in the place of a character that has been found to be invalid or in error. SUB is intended to
> be introduced by automatic means.

I'm not sure whether this is something which needs to be implemented. I'm pretty sure this is meant as a
function of the font.

#### 8.3.149 SVS - SELECT LINE SPACING

> SVS is used to establish the line spacing for subsequent text. The established spacing remains in effect
> until the next occurrence of SVS or of SET LINE SPACING (SLS) or of SPACING INCREMENT (SPI)
> in the data stream. The parameter values are:
> 
> 0) 6 lines per 25,4 mm
> 1) 4 lines per 25,4 mm 
> 2) 3 lines per 25,4 mm
> 3) 12 lines per 25,4 mm
> 4) 8 lines per 25,4 mm
> 5) 6 lines per 30,0 mm
> 6) 4 lines per 30,0 mm
> 7) 3 lines per 30,0 mm
> 8) 12 lines per 30,0 mm
> 9) 2 lines per 25,4 mm

Seems to be very printing related. Will skip this for now.

#### 8.3.150 SYN - SYNCHRONOUS IDLE

> SYN is used by a synchronous transmission system in the absence of any other character (idle condition) to
> provide a signal from which synchronism may be achieved or retained between data terminal equipment.
>
> The use of SYN is defined in ISO 1745.

TODO : Refer to ISO 1745. This seems to be the way I may be able to make keep-alive happen.

#### 8.3.151 TAC - TABULATION ALIGNED CENTRED

> TAC causes a character tabulation stop calling for centring to be set at character position n in the active
> line (the line that contains the active presentation position) and lines of subsequent text in the
> presentation component, where n equals the value of Pn. TAC causes the replacement of any tabulation
> stop previously set at that character position, but does not affect other tabulation stops.
> 
> A text string centred upon a tabulation stop set by TAC will be positioned so that the (trailing edge of
> the) first graphic character and the (leading edge of the) last graphic character are at approximately equal
> distances from the tabulation stop.

Most likely won't implement this unless there's a reason to do so.

#### 8.3.152 TALE - TABULATION ALIGNED LEADING EDGE

> TALE causes a character tabulation stop calling for leading edge alignment to be set at character
> position n in the active line (the line that contains the active presentation position) and lines of
> subsequent text in the presentation component, where n equals the value of Pn. TALE causes the
> replacement of any tabulation stop previously set at that character position, but does not affect other
> tabulation stops.
> 
> A text string aligned with a tabulation stop set by TALE will be positioned so that the (leading edge of
> the) last graphic character of the string is placed at the tabulation stop.

Most likely won't implement this unless there's a reason to do so.

#### 8.3.153 TATE - TABULATION ALIGNED TRAILING EDGE

> TATE causes a character tabulation stop calling for trailing edge alignment to be set at character
> position n in the active line (the line that contains the active presentation position) and lines of
> subsequent text in the presentation component, where n equals the value of Pn. TATE causes the
> replacement of any tabulation stop previously set at that character position, but does not affect other
> tabulation stops. 
> 
> A text string aligned with a tabulation stop set by TATE will be positioned so that the (trailing edge of
> the) first graphic character of the string is placed at the tabulation stop.

Most likely won't implement this unless there's a reason to do so.

#### 8.3.154 TBC - TABULATION CLEAR

> TBC causes one or more tabulation stops in the presentation component to be cleared, depending on the
> parameter value:
> 
> 0) the character tabulation stop at the active presentation position is cleared
> 1) the line tabulation stop at the active line is cleared
> 2) all character tabulation stops in the active line are cleared
> 3) all character tabulation stops are cleared
> 4) all line tabulation stops are cleared
> 5) all tabulation stops are cleared
> 
> In the case of parameter value 0 or 2 the number of lines affected depends on the setting of the
> TABULATION STOP MODE (TSM)

Currently modes 0 and 3 are implemented.

TODO : Implement modes 1 and 2, possibly 4 and 5. I'm seeing 1 and 2 when running some software and some tests.

#### 8.3.155 TCC - TABULATION CENTRED ON CHARACTER

> TCC causes a character tabulation stop calling for alignment of a target graphic character to be set at
> character position n in the active line (the line that contains the active presentation position) and lines of
> subsequent text in the presentation component, where n equals the value of Pn1, and the target character
> about which centring is to be performed is specified by Pn2. TCC causes the replacement of any
> tabulation stop previously set at that character position, but does not affect other tabulation stops.
> 
> The positioning of a text string aligned with a tabulation stop set by TCC will be determined by the first
> occurrence in the string of the target graphic character; that character will be centred upon the tabulation
> stop. If the target character does not occur within the string, then the trailing edge of the first character
> of the string will be positioned at the tabulation stop.
> 
> The value of Pn2 indicates the code table position (binary value) of the target character in the currently
> invoked code. For a 7-bit code, the permissible range of values is 32 to 127; for an 8-bit code, the
> permissible range of values is 32 to 127 and 160 to 255.

Most likely won't implement this. Some of these features seem to be really focused on echo mode and I don't see
local echo as a priority.

#### 8.3.156 TSR - TABULATION STOP REMOVE

> TSR causes any character tabulation stop at character position n in the active line (the line that contains
> the active presentation position) and lines of subsequent text in the presentation component to be
> cleared, but does not affect other tabulation stops. n equals the value of Pn.

I'm not sure I could find a proper test case to verify I got this working. Besides, it seems to be printer head
motion related as opposed to screen related.

#### 8.3.157 TSS - THIN SPACE SPECIFICATION

> TSS is used to establish the width of a thin space for subsequent text. The established width remains in
> effect until the next occurrence of TSS in the data stream, see annex C.
> 
> Pn specifies the width of the thin space.
> 
> The unit in which the parameter value is expressed is that established by the parameter value of SELECT
> SIZE UNIT (SSU).

Most likely won't implement as it seems strictly printing related.

#### 8.3.162 VTS - LINE TABULATION SET

> VTS causes a line tabulation stop to be set at the active line (the line that contains the active presentation
> position). 

TODO : Seems interesting as it is cursor movement, but I don't want to waste a lot of time implementing vertical tabs.
