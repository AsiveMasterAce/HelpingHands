namespace HelpingHands.Models
{
    public class Business
    {
        public int? ID { get; set; }
        public string Name { get; set; }
        public string NPONumber { get; set; }
        public string CellNo { get; set; }
        public string Email { get; set; }
        public string OperationgHrs { get; set; }
        public string Logo { get; set; }
        public int? SuburdID { get; set; }
        public bool? Archived { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
    }
}
