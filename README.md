# VtNetCore

## Introduction

This is a VT100/XTerm style terminal emulator library for the .NET Standard 2.0 framework. 

## License Status

As of the time of this writing, more or less everything is up in the air. At this time, the code is not "free" and should not be considered as such.
We are running the MIT and Apache licenses through legal at the office and will release the code under one of these in the near future. We are also
deciding what name to place on the license.

## Maintainers

* Darren R. Starr - Original author and current maintainer.

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
