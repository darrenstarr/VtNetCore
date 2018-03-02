namespace VtNetCore.XTermParser
{
    using System;
    using System.Text;
    using VtNetCore.VirtualTerminal;

    /// <summary>
    /// Consumes pushed data, parses it and processes it through the virtual terminal controller
    /// </summary>
    public class DataConsumer
    {
        /// <summary>
        /// Enables logging of received sequences.
        /// </summary>
        /// <remarks>
        /// When enabled, this adds a tremendous amount of overhead to the terminal. It should only be used for troubleshooting
        /// </remarks>
        public bool SequenceDebugging { get; set; }

        /// <summary>
        /// The buffer to hold state for processing and parsing of incoming data
        /// </summary>
        private XTermInputBuffer InputBuffer { get; set; } = new XTermInputBuffer();

        /// <summary>
        /// State information for when continuing to process data from a previously starved buffer condition
        /// </summary>
        private bool ResumingStarvedBuffer { get; set; }

        /// <summary>
        /// The controller which processes the sequences parsed from the stream.
        /// </summary>
        public IVirtualTerminalController Controller { get; private set; }

        /// <summary>
        /// Base constructor
        /// </summary>
        /// <param name="controller">The controller to consume the sequences parsed.</param>
        public DataConsumer(IVirtualTerminalController controller)
        {
            Controller = controller;
        }

        /// <summary>
        /// Consume raw byte data, parse it and then process it through the controller.
        /// </summary>
        /// <param name="data">Raw terminal data.</param>
        /// <remarks>
        /// The data passed to this function IS not telnet or SSH protocol aware. It is necessary
        /// to ensure that OOB control sequences are stripped from the incoming data.
        /// It is safe to call this function repetitively with small buffers.
        /// </remarks>
        public void Push(byte[] data)
        {
            InputBuffer.Add(data);

            Controller.ClearChanges();
            while (!InputBuffer.AtEnd)
            {
                try
                {
                    if (SequenceDebugging && ResumingStarvedBuffer)
                    {
                        System.Diagnostics.Debug.WriteLine("Resuming from starved buffer [" + Encoding.UTF8.GetString(InputBuffer.Buffer).Replace("\u001B", "<esc>") + "]");
                        ResumingStarvedBuffer = false;
                    }

                    var sequence = XTermSequenceReader.ConsumeNextSequence(InputBuffer);

                    // Handle poorly injected sequences
                    if (sequence.ProcessFirst != null)
                    {
                        foreach (var item in sequence.ProcessFirst)
                        {
                            if (SequenceDebugging)
                                System.Diagnostics.Debug.WriteLine(item.ToString());

                            XTermSequenceHandlers.ProcessSequence(item, Controller);
                        }
                    }

                    if (SequenceDebugging)
                        System.Diagnostics.Debug.WriteLine(sequence.ToString());

                    XTermSequenceHandlers.ProcessSequence(sequence, Controller);
                }
                catch (IndexOutOfRangeException)
                {
                    ResumingStarvedBuffer = true;
                    InputBuffer.PopAllStates();
                    break;
                }
                catch (ArgumentException)
                {
                    // We've reached an invalid state of the stream.
                    InputBuffer.ReadRaw();
                    InputBuffer.Commit();
                }
                catch (Exception e)
                {
                    // This is less than attractive, but until such time as the code is considered highly
                    // reliable, this is the gateway for where nearly all crashes will be caught.
                    System.Diagnostics.Debug.WriteLine("Unknown exception " + e.Message);
                }
            }

            InputBuffer.Flush();
        }
    }
}
