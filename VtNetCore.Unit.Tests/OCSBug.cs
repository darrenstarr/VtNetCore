using Xunit;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using ConnectionNode;
using VtNetCore.VirtualTerminal;
using VtNetCore.XTermParser;

namespace ConnectionNode.Tests.UnitTests
{

public class OCSBug
{

    // Created this so we can expect the input buffer and ensure the bug is fixed
    public class TransparentDataConsumer : DataConsumer
    {
        public TransparentDataConsumer(IVirtualTerminalController controller) : base(controller) {}
        public XTermInputBuffer GetInputBuffer() 
        { return this.InputBuffer; }
    }

    private void Push(DataConsumer d, string s)
    {
        d.Push(Encoding.UTF8.GetBytes(s));
    }

    [Fact]
    public void OSC112BugTest()
    {
        var TerminalController = new VirtualTerminalController();
        var d = new TransparentDataConsumer(TerminalController);

        int count = 250;
        
        Stopwatch timeBefore = new Stopwatch();
        timeBefore.Start();
        for (int i=0; i< count; i++) {
            Push(d, "TheQuickBrownFoxjumpedOverTheLazyDog.");
        }
        timeBefore.Stop();

        Assert.Equal(0, d.GetInputBuffer().Remaining);
        Assert.Equal(0, d.GetInputBuffer().Position);
        Assert.Equal(0, d.GetInputBuffer().Buffer.Length);

        // OSC-112
        // ESC-]-112-BELL
        Push(d, "\u001b]112\u0007");

        Assert.Equal(0, d.GetInputBuffer().Remaining);
        Assert.Equal(0, d.GetInputBuffer().Position);
        Assert.Equal(0, d.GetInputBuffer().Buffer.Length);

        Stopwatch timeAfter = new Stopwatch();
        timeAfter.Start();
        for (int i=0; i< count; i++) {
            Push(d, "TheQuickBrownFoxjumpedOverTheLazyDog.");
        }
        timeAfter.Stop();

        // Assert that same operations don't take ten times as long, the second time you do them.
        Assert.True((timeBefore.ElapsedMilliseconds*10) > timeAfter.ElapsedMilliseconds);

        Assert.Equal(0, d.GetInputBuffer().Remaining);
        Assert.Equal(0, d.GetInputBuffer().Position);
        Assert.Equal(0, d.GetInputBuffer().Buffer.Length);
    }
   
    [Fact]
    public void TmuxStartingUpTest()
    {   

        // This bug was originally triggered by tmux. We use a recorded tmux as a test case here to ensure the bug doesn't reoccur.
        List<String> tmuxsession = new List<String>{"dA==", "bQ==", "dQ==", "eA==", "DQo=", "G1s/MTA0OWgbKEIbW20bWz8xbBs+G1tIG1syShtbPzEybBtbPzI1aBtbPzEwMDBsG1s/MTAwNmwbWz8xMDA1bBtbYxtbPjQ7MW0bWz8xMDA0aBtdMTEyBxtbPzI1bBtbMTsxSBtbSw0KG1tLDQobW0sNChtbSw0KG1tLDQobW0sNChtbSw0KG1tLDQobW0sNChtbSw0KG1tLDQobW0sNChtbSw0KG1tLDQobW0sNChtbSw0KG1tLDQobW0sNChtbSw0KG1tLDQobW0sNChtbSw0KG1tLDQobW0sNChtbSw0KG1tLDQobW0sNChtbSw0KG1tLDQobW0sNChtbSw0KG1tLDQobW0sNChtbSw0KG1tLDQobW0sNChtbSw0KG1tLDQobW0sNChtbSw0KG1tLDQobW0sNChtbSw0KG1tLDQobW0sNChtbSw0KG1tLDQobW0sNChtbSw0KG1tLDQobW0sNChtbSw0KG1tLDQobW0sNChtbSw0KG1tLDQobW0sNChtbSw0KG1tLDQobWzMwbRtbNDJtWzBdIDA6YmFzaCogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICJpcC0xNzItMzEtODEtODcuZWMyLmluIiAyMjowNSAwNS1PY3QtMjIbKEIbW20bWzYwOzFIG1sxOzYwchtbSBtbPzEybBtbPzI1aA==", "G1s/MjVsG1s2MGQbWzMwbRtbNDJtWzBdIDA6c3NtLXVzZXJAaXAtMTcyLTMxLTgxLTg3On4qICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICJpcC0xNzItMzEtODEtODcuZWMyLmluIiAyMjowNSAwNS1PY3QtMjIbKEIbW20bWzE7MUgbWz8xMmwbWz8yNWg=", "G1sxOzU5cltzc20tdXNlckBpcC0xNzItMzEtODEtODcgfl0kIBtbMTs2MHIbW0gbWzMwQw==", "G1sxOzU5chtbSBtbMzBDdBtbMTs2MHIbW0gbWzMxQw==", "G1sxOzU5chtbSBtbMzFDbxtbMTs2MHIbW0gbWzMyQw==", "G1sxOzU5chtbSBtbMzJDcBtbMTs2MHIbW0gbWzMzQw==", "DQo=", "G1s/MjVsG1s/MWgbPQ==", "G1sxOzU5chtbSBtbSw0KG1tLG1sxQhtbSxtbMUIbW0sbWzFCG1tLG1sxQhtbSxtbMUIbW0sbWzFCG1tLG1sxQhtbSxtbMUIbW0sbWzFCG1tLG1sxQhtbSxtbMUIbW0sbWzFCG1tLG1sxQhtbSxtbMUIbW0sbWzFCG1tLG1sxQhtbSxtbMUIbW0sbWzFCG1tLG1sxQhtbSxtbMUIbW0sbWzFCG1tLG1sxQhtbSxtbMUIbW0sbWzFCG1tLG1sxQhtbSxtbMUIbW0sbWzFCG1tLG1sxQhtbSxtbMUIbW0sbWzFCG1tLG1sxQhtbSxtbMUIbW0sbWzFCG1tLG1sxQhtbSxtbMUIbW0sbWzFCG1tLG1sxQhtbSxtbMUIbW0sbWzFCG1tLG1sxQhtbSxtbMUIbW0sbWzFCG1tLG1sxQhtbSxtbMUIbW0sbWzFCG1tLG1sxQhtbSxtbMUIbW0sbWzFCG1tLG1sxQhtbSxtbMUIbW0sbWzFCG1tLG1sxQhtbSxtbMUIbW0sbWzFCG1tLG1sxQhtbSxtbMUIbW0sbWzFCG1tLG1tIdG9wIC0gMjI6MDU6NTQgdXAgMzIgbWluLCAgMCB1c2VycywgIGxvYWQgYXZlcmFnZTogMC4wMCwgMC4wMCwgMC4wMBtbSw0KVGFza3M6G1sxbSAxMDIgGyhCG1ttdG90YWwsG1sxbSAgIDEgGyhCG1ttcnVubmluZywbWzFtICA2NCAbKEIbW21zbGVlcGluZywbWzFtICAgMCAbKEIbW21zdG9wcGVkLBtbMW0gICAwIBsoQhtbbXpvbWJpZRtbSw0KJUNwdShzKTobWzFtICAwLjAgGyhCG1ttdXMsG1sxbSAgMC4wIBsoQhtbbXN5LBtbMW0gIDAuMCAbKEIbW21uaSwbWzFtMTAwLjAgGyhCG1ttaWQsG1sxbSAgMC4wIBsoQhtbbXdhLBtbMW0gIDAuMCAbKEIbW21oaSwbWzFtICAwLjAgGyhCG1ttc2ksG1sxbSAgMC4wIBsoQhtbbXN0G1tLDQpLaUIgTWVtIDobWzFtICAxMDA1ODI0IBsoQhtbbXRvdGFsLBtbMW0gICAzODY5NDggGyhCG1ttZnJlZSwbWzFtICAgMTMzMzA4IBsoQhtbbXVzZWQsG1sxbSAgIDQ4NTU2OCAbKEIbW21idWZmL2NhY2hlG1tLDQpLaUIgU3dhcDobWzFtICAgICAgICAwIBsoQhtbbXRvdGFsLBtbMW0gICAgICAgIDAgGyhCG1ttZnJlZSwbWzFtICAgICAgICAwIBsoQhtbbXVzZWQuG1sxbSAgIDcxNDg5MiAbKEIbW21hdmFpbCBNZW0gG1tLDQobW0sNCg=="};

        var TerminalController = new VirtualTerminalController();
        var d = new TransparentDataConsumer(TerminalController);

        foreach (String consoleOut in tmuxsession){
            byte[] data = Convert.FromBase64String(consoleOut);
            d.Push(data);
        }

        Assert.Equal(0, d.GetInputBuffer().Remaining);
        Assert.Equal(0, d.GetInputBuffer().Position);
        Assert.Equal(0, d.GetInputBuffer().Buffer.Length);
    }
}

}