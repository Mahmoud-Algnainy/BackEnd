using BackEnd.Models;

namespace BackEnd.DTO.Employee
{
    public class CreateEmployeeSarf
    {
        public int Id { get; set; }
        public DateTime SarfDate { get; set; }

    
        public string? SarfId { get; set; }

        public  List<CreateEmployeeEsthkak>? Esthkaks { get; set; }

        public List<CreateEmployeeEstkta3>? Estkta3s { get; set; }
    }
}
