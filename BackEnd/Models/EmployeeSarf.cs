namespace BackEnd.Models
{
    public class EmployeeSarf
    {
        public int Id { get; set; }
        public DateTime SarfDate { get; set; }

        public virtual Employee? Employee { get; set;}
        public int? EmployeeId { get; set; }

        public List<EmployeeSarf_Esthkak>? EmployeeSarf_Esthkaks { get; set;}
         
        public List<EmployeeSarf_Estkta3>? EmployeeSarf_Estkta3s { get; set; }


    }
}
