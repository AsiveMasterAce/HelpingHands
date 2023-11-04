﻿using HelpingHands.Models.Users;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HelpingHands.Models
{
    public class TimelinePost
    {
        [Key]
        public int Id { get; set; }

        public string PostTitle { get; set; }
        public string PostContent { get; set; }
        public List<PostComment> Comments { get; set; }
        public int Likes { get; set; } = 0;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool Archived { get; set; } = false;
    }

    public class PostComment
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public string CommentContent { get; set; }
        public TimelinePost Post { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool Archived { get; set; } = false;
        public virtual UserModel? User { get; set; }
    }

}
