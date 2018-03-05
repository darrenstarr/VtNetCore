# VtNetCore

## Introduction

This is a VT100/XTerm style terminal emulator library for the .NET Standard 2.0 framework. 

## License Status

As of the time of this writing, more or less everything is up in the air. At this time, the code is not "free" and should not be considered as such.
We are running the MIT and Apache licenses through legal at the office and will release the code under one of these in the near future. We are also
deciding what name to place on the license.

## Maintainers

* Darren R. Starr - Original author and current maintainer.

## References

### XTerm Control Sequences

[Thomas Dickey's XTerm ctrlseqs.txt](https://github.com/ThomasDickey/xterm-snapshots/blob/master/ctlseqs.txt) which I would consider the "reference" on
what xterm should support. This is probably because... well it is the actual XTerm. I know Thomas gets a bit grumpy over other terminals misrepresenting
themselves as XTerm when reporting their device attributes. I hope to test VIM without this shortly. For now, as with Putty, I'll borrow the ID from xterm
until I can figure out if there is a proper registry where I can apply for a new number.

### VT100.net VT102 User Guide

[VT102 User Guide](https://vt100.net/docs/vt102-ug/) this seems to be some of the most readily available documentation and at times the most detailed
regarding the VT102 terminal. This is a very incomplete source of information on the topic since VT102 is REALLY old and barely covers the basics of
what is needed in a modern terminal (think in terms of 1990's is better than 1970's).

### ECMA-43 - 8-Bit Coded Character Set Structure and Rules

[ECMA-43](http://www.ecma-international.org/publications/files/ECMA-ST/Ecma-043.pdf)

This is the documentation from ECMA that describes the character set encodings of these old terminals. Surprisingly, they did not operate on
unicode back in the 1960's and 1970's. As such, this is the document which describes the most current version of "something other than just ASCII".
The coding format is based on the principles of punch card columns and rows. This allows for 7-bit character coding as represented in
[ECMA-6 - 7-Bit coded Character Set](http://www.ecma-international.org/publications/files/ECMA-ST/Ecma-006.pdf), as well as being more thoroughly
described in archaic language I don't comprehend at times in the document
[ECMA-7 - Representation of the Standard ECMA-6 (7 bit code) on Punch Cards](https://www.ecma-international.org/publications/files/ECMA-ST-WITHDRAWN/ECMA-7,%201st%20Edition,%20April%201965.pdf).

### ECMA-48 - Control Functions for Coded Character Sets

[ECMA-48](http://www.ecma-international.org/publications/files/ECMA-ST/Ecma-048.pdf)

This is the documentation from ECMA regarding the standardized components of what we generally refer to as DEC VT style escape coding. This document
is archaic and difficult to read, but is more verbose than most other sources regarding the definitions of most escape codes. It of course is
incomplete as there have been many vendor proprietary extensions since. In this case, it is necessary to refer to projects like libvterm for additional
information.

## Progress

### 05-March-2018

Considerable progress has been made in standards compliance. Thanks to finding the excellent [libvterm](https://launchpad.net/libvterm) toolkit
I've managed to start porting [Paul "LeoNerd" Evans](https://launchpad.net/~leonerd) test suite into XUnit so that I can use live unit testing.
It's amazing how massive the improvements have been so far. Simply passing tests "11state_movecursor.test", "12state_scroll.test" and the half of "13state_edit.test" I've implemented so far has improved the overall quality of the terminal. I'm at the point where I feel as if I'm simply
implementing features for the sake of features since Cisco devices, VIM, Less, More, Midnight commander, etc... all seem to work pretty well.

I will finish up tests for the known to be supported escape sequences for now. Hoepfully a few more hours and I'll be done with those. Then I'll
move onto making a better connection library with support for SSH (via Renci), Telnet, and serial ports (Windows only unless I can find a good reason
to do Mac and Linux as well).

### 03-March-2018

The code is highly functional though is very early in development. After less than a week of work, it is able to pass a large part of the
excellent [vttest test suite from Thomas Dickey](https://invisible-island.net/vttest/vttest.html). And most of the code thus far has been
written using the [XTerm ctrlseqs.txt](https://github.com/ThomasDickey/xterm-snapshots/blob/master/ctlseqs.txt) document also maintained
by Thomas Dickey.

At this time, the code is sort of reactively developed in the sense that I coded it by trying it as a Linux terminal with [Renci's SSH.NET](https://github.com/sshnet/SSH.NET)
library of which I maintain my own version for now.

There is not nearly enough documentation in the code at this point and there are not nearly enough unit tests. Now that I
believe I have a foothold on how it should work, I'm in the process of trying to correct this. There is a TODO list that is
far too long to document at this time. As soon as I have VIM working nearly flawlessly, I'll work on correcting this.

