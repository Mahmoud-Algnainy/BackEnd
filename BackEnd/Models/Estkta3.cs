namespace BackEnd.Models
{
    public class Estkta3
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<EmployeeSarf_Estkta3> EmployeeSarf_Estkta3s { get; set; }

    }
}
