namespace Wpf_DeadLock.Library.Entity
{
    public class LineConnection
    {
        /// <summary>
        /// Line identification
        /// </summary>
        public int Id { get; set; }
        public int ProcessId { get; set; }
        public int ResourceId { get; set; }
        public bool Process { get; set; }

        /// <summary>
        /// The visual line draw on screen
        /// </summary>
        public System.Windows.Shapes.Path LineDraw { get; set; }

    }
}
