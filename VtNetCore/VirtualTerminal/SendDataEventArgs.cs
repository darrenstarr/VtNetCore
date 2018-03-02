namespace VtNetCore.VirtualTerminal
{
    using System;

    /// <summary>
    /// Argument data structure for processing bytes to be transmitted
    /// </summary>
    public class SendDataEventArgs : EventArgs
    {
        /// <summary>
        /// The data to transmit
        /// </summary>
        /// <remarks>
        /// This data does not represent a format. It's simply raw data. For systems like Telnet
        /// it may be necessary to escape the data or SSH would encrypt it.
        /// </remarks>
        public byte [] Data { get; set; }
    }
}
