using System.ComponentModel.DataAnnotations;

namespace www.Models.ViewModels
{
    public class SchoolVotesView
    {
        [Key]
        public string SchoolName { get; set; }
        public int TotalVotes { get; set; }
    }
}