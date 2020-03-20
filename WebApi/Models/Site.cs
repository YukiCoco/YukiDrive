using System.ComponentModel.DataAnnotations;
namespace YukiDrive.Models
{
    public class Site
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string SiteId { get; set; }
        public string NickName { get; set; }
    }
}