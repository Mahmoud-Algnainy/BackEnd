namespace BackEnd.Models
{
    public class EmployeeSarf_Estkta3
    {
        public int Id { get; set; }

        public virtual Estkta3 Estkta3 { get; set; }
        public int  Estkta3Id { get; set; }
        public decimal Estkta3Value { get; set;}
        public EmployeeSarf EmployeeSarf { get; set; }
    }
}
