# VtNetCore

![Build status](https://ci.appveyor.com/api/projects/status/9ugagv9y9qqyl8ym?svg=true "Release")


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

### VT100 Series Video Terminal Technical Manual

[VT100 Series Video Terminal Technical Manual](http://bitsavers.trailing-edge.com/pdf/dec/terminal/vt100/EK-VT100-TM-003_VT100_Technical_Manual_Jul82.pdf) is what appears to be the original
technical manual for the VT100 terminal. I haven't reviewed this very far yet, but will spend a little time writing a series of unit tests to verify that I'm at least VT100 compliant.

## Progress

### 26-March-2018

Let's see... status report

* Colors are almost 100% identical to XTerm
* Midnight commander is completely functional now
* byobu is flawless, there is even support for XTerm's font colors and weights
* Mouse tracking is almost 100%, highlight mode is not implemented because it seems naughty. URXVT is not implemented yet either
* Character sets seem pretty good
* Bracketed paste mode isn't there yet. Will implement that soon. It's easy.
* ECMA-48 compliance is pretty complete except for Scroll Left and Scroll Right (probably tomorrow) and a lot of CJK, R-to-L, printing and serial communication stuff
* Performance is pretty good
* Unit tests are progressing pretty nicely
* Making a switch to simplify painting which will return rows and columns
* Blinking, concealment, all that stuff is basically implemented
* Pretty much all vttest tests which I consider important are passing now... way way more than Putty.
* Scroll back works almost perfect
* There is something weird with pipe.sh which I need to figure out, there's some strange case with painting past the end of the screen.
* There are some weird 7 vs 8 bit mode things I don't clearly understand but it shouldn't matter... for now.
* VT-52 mode is pretty fully implemented and tested
* I've been running A LOT of stuff and haven't had a single crash in weeks.
* I would like to work a little on memory, but there doesn't seem to be any real show stoppers here.
* Once I switch to spans, the host application shouldn't need to work too hard on color handling

So overall, I'm tempted to slap together a user app to get this thing out in the while and thoroughly tested to see how people like it.

I'll work a little on verification images and make a project wiki article on it.

It's time to start making projects and creating issues.

It's also really close to time for making a nuget package.

### 24-March-2018

Pleased to say that I've fixed the nano bug. It seems that saving scrolling margins isn't part of the cursor state. This seems silly since it means that
the scroll regions would need to be reset when flipping between main and alternate buffer. So be it.

### 24-March-2018

Well, I've been working on integrating [Nil.js](https://github.com/nilproject/NiL.JS) as a scripting engine to for the project and have run into complications
due to things being hairy as always when working with multithreaded environments and scripting engines (that also happen to be commented in Russian). The Nil.js
engine is quite lovely though and I've been able to spawn multiple terminals and control them using JavaScript async from within the engine.

There has been some considerable progress on the VtNetCore in the sense that thanks to [Dan Walmsley](https://github.com/danwalmsley) from the 
[AvalonStudio Project](https://github.com/VitalElement/AvalonStudio) getting involved. He has integrated the engine with his IDE and has been giving excellent
changes, bug reports and feedback.

So far, I have implemented the Windows UWP version and also the Mac Cocoa version and Dan has implemented the Avalon Studio version. So at this time, VtNetCore
should be operational on most major platforms. I've also been looking at the possibility of native ports to Android and GTK as porting doesn't really take more
than a few hours.

At this time, the control is reaching a pretty impressive level of compatibility. I don't think I'm anywhere near XTerm and possibly libvterm, but VTNetCore is 
certain further along than Putty at the moment. I have one really naughty looking bug associated with scrolling in Nano that I need to look into, but otherwise
it's pretty far along. I would easily refer to it as almost ready for production and I'm sure Dan will provide a lot of feedback over time which will help me
along.

I've been asked for pictures of what VtNetCore looks like. This is obviously useful since people don't like investing time into using libraries unless there
is some evidence that they are worth the effort. Once I fix the Nano thing, I'll put some work into making some pretty pictures and putting them online.

### 08-March-2018

So, thanks again to [Paul "LeoNerd" Evans](https://launchpad.net/~leonerd) libvterm unit tests which I've spent much time porting to C# and into
XUnit, my code is getting pretty good. I've passed many of the more interesting tests and many of the boring tests. I hope to port them all to
XUnit to QC purposes, but short of mouse support, the terminal is pretty good now. I am certainly not feature complete, but I'm at the point now
that other than compliance testing, I'm not encountering any escape sequences that I don't handle "properly" in real software. In many cases,
I'm further along than Putty, in others I have a LONG way to go.

I'll be cleaning up the API and adding some pretty important things (like maximum history length) to the system shortly. There's two really annoying bugs getting to me at the moment. The first is vttest's double wide character support. I think it's a tab thing. Second, is the vttest for scrolling. I'm not 100% sure what it should look like, so I'll spin up a Linux VM and look at xterm and check. I'm close, but I'll port both these tests to XUnit and debug them soon.

I still wouldn't call the code production ready, but it's clearly usable now. I'm going to continue working on VtConnect which is has a few blaring
problems like, there's no way to reliably detect a disconnect from a peer, and there's a glitch in Telnet support (reproducable using vttest). And soon I'll write the scripting engine!!!

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

