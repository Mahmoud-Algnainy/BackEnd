namespace BackEnd.Models
{
    public class EmployeeSarf_Esthkak
    {
        public int Id { get; set; }

        public virtual Esthkak? Esthkak { get; set; }
        public int EsthkakId { get; set; }
        public decimal EsthkakValue { get; set; }
        public EmployeeSarf EmployeeSarf { get; set; }
    }
}
