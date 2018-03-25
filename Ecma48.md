# ECMA-48 Progress/Compliance

## Control Functions

### Brief table of compliance

| Definition                                    | Notation   | Representation  | Implemented |
|-----------------------------------------------|------------|-----------------|-------------|
| 8.3.1 ACK - ACKNOWLEDGE                       | C0         | 00/06 (0x06)    | no          |
| 8.3.2 APC - APPLICATION PROGRAM COMMAND       | C1         | 09/15 (0x9F)    | no          |
| 8.3.3 BEL - BELL                              | C0         | 00/07 (0x07)    | decoded     |
| 8.3.4 BPH - BREAK PERMITTED HERE              | C1         | 08/02 (0x82) or ESC 04/02 (\eB) | no |
| 8.3.5 BS - BACKSPACE                          | C0         | 00/08 (0x08)    | yes         |
| 8.3.6 CAN - CANCEL                            | C1         | 01/08 (0x18)    | no          |
| 8.3.7 CBT - CURSOR BACKWARD TABULATION        | (Pn)       | CSI Pn 05/10 (CSI Pn Z) | yes |
| 8.3.8 CCH - CANCEL CHARACTER                  | C1         | 09/04 (0x94) or ESC 05/04 (\eT) | no |
| 8.3.9 CHA - CURSOR CHARACTER ABSOLUTE         | Pn         | CSI Pn 04/07 (CSI Pn G) | yes |
| 8.3.10 CHT - CURSOR FORWARD TABULATION        | Pn         | CSI Pn 04/09 (CSI Pn I) | yes |
| 8.3.11 CMD - CODING METHOD DELIMITER          | Fs         | ESC 06/04 (\ed) | no          |
| 8.3.12 CNL - CURSOR NEXT LINE                 | Pn         | CSI Pn 04/05 (CSI Pn E) | yes |
| 8.3.13 CPL - CURSOR PRECEDING LINE            | Pn         | CSI Pn 04/06 (CSI Pn F) | yes |
| 8.3.14 CPR - ACTIVE POSITION REPORT           | (Pn1;Pn2)  | CSI Pn1;Pn2 05/02 (CSI Pn1;Pn2 Q) | no |
| 8.3.15 CR - CARRIAGE RETURN                   | C0         | 00/13 (0x0D) '\n' | yes       |
| 8.3.16 CSI - CONTROL SEQUENCE INTRODUCER      | C1         | 09/11 (0x9B) or ESC 05/11 (0x5B) '\e[' | partial |
| 8.3.17 CTC - CURSOR TABULATION CONTROL        | (Ps...)    | CSI Ps... 05/07 (0x57) or CSI Ps... W | No |
| 8.3.18 CUB - CURSOR LEFT                      | Pn         | CSI Pn 04/04 (CSI Pn D) | yes |
| 8.3.19 CUD - CURSOR DOWN                      | Pn         | CSI Pn 04/02 (CSI Pn B) | yes |
| 8.3.20 CUF - CURSOR RIGHT                     | Pn         | CSI Pn 04/03 (CSI Pn C) | yes |
| 8.3.21 CUP - CURSOR POSITION                  | (Pn1;Pn2)  | CSI Pn1;Pn2 04/08 (CSI Pn1;Pn2 H) | yes |
| 8.3.22 CUU - CURSOR UP                        | Pn         | CSI Pn 04/01 (CSI Pn A) | yes |
| 8.3.23 CVT - CURSOR LINE TABULATION           | Pn         | CSI Pn 05/08 (CSI Pn Y) | no  |
| 8.3.24 DA - DEVICE ATTRIBUTES                 | Ps         | CSI Ps 06/03 (CSI Ps c) | partial |
| 8.3.25 DAQ - DEFINE AREA QUALIFICATION        | (Ps...)    | CSI Ps... 06/15 (0x6F) (CSI Ps... o) | no |
| 8.3.26 DCH - DELETE CHARACTER                 | Pn         | CSI Pn 05/00 (CSI Pn P) | mostly |
| 8.3.27 DCS - DEVICE CONTROL STRING            | C1         | 09/00 (0x90) or ESC 05/00 (\eP) | no |
| 8.3.28 DC1 - DEVICE CONTROL ONE               | C0         | 01/01 (0x11)    | no          |
| 8.3.29 DC2 - DEVICE CONTROL TWO               | C0         | 01/02 (0x12)    | no          |
| 8.3.30 DC3 - DEVICE CONTROL THREE             | C0         | 01/03 (0x13)    | no          |
| 8.3.31 DC4 - DEVICE CONTROL FOUR              | C0         | 01/04 (0x14)    | no          |
| 8.3.32 DL - DELETE LINE                       | Pn         | CSI Pn 04/13 (0x4D) (CSI Pn M) | mostly |
| 8.3.33 DLE - DATA LINK ESCAPE                 | C0         | 01/00 (0x10)    | no          |
| 8.3.34 DMI - DISABLE MANUAL INPUT             | Fs         | ESC 06/00 (0x60) or (\e`) | no |
| 8.3.35 DSR - DEVICE STATUS REPORT             | Ps         | CSI Ps 06/14 (0x6E) or CSI Ps n | partial |
| 8.3.36 DTA - DIMENSION TEXT AREA              | (Pn1;Pn2)  | CSI Pn1;Pn2 02/00 05/04 (CSI Pn1;Pn2 ' ' T) | no |

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