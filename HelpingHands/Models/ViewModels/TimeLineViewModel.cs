namespace HelpingHands.Models.ViewModels
{
    public class TimeLineViewModel
    {
        public List<TimelinePost> Posts { get; set; }
    }

    public class CreateTimeline 
    {
        public string PostTitle { get; set; }
        public string PostContent { get; set; }
    }
    public class CommentOnPost
    {
        public string CommentContent { get; set; }
    }
}
