namespace DotNetWebAPI.Models
{
    public class Dator
    {
        public int DatorId { get; set; }
        public string FirstName { get; set; }="";
        public string LastName { get; set; }="";
        public string Email { get; set; }="";
        public string Gender { get; set; }="";
    }
}