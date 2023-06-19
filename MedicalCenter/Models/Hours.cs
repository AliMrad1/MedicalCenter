namespace MedicalCenter.Models
{
    public class Hours
    {
        public int Id { get; set; }
        public List<string>? DayOfWeek { get; set; } = new List<string>();
        public List<string>? time { get; set; } = new List<string>();
       
    }
}
