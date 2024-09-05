using BackEnd.DTO.Employee;
using BackEnd.DTO.Excel;
using BackEnd.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly HcfiDBContext _dbcontext;

        public EmployeeController(HcfiDBContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        [HttpGet]

        public async Task<IActionResult> get()
        {
            return Ok(await _dbcontext.Employees.ToListAsync());
        }

        [HttpPut("{id}")]

        public async Task<IActionResult> Put(int id,  EmployeeUpdate obj)
        {

            var EmployeeExist = _dbcontext.Employees.FirstOrDefault(x => x.Id == id);
            if (EmployeeExist == null)
            {
                //return NotFound($"Employee Not Found wit id : {id}");
                return NotFound();
            }
            EmployeeExist.Sarf_Id = obj.Sarf_Id;
            EmployeeExist.PhoneNumber = obj.PhoneNumber;
            EmployeeExist.Nationality = obj.Nationality;
            EmployeeExist.NationalId = obj.NationalId;
            EmployeeExist.BankAcc_No = obj.BankAcc_No;
            EmployeeExist.Ta2meen_No = obj.Ta2meen_No;
            await _dbcontext.SaveChangesAsync();
            //return Ok($"Employee with id : {id} Updated");
            return Ok();

        }

        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> post([FromBody] Employee employee)
        {
            // Validation logic can be adjusted here
            if (employee == null)
            {
                return BadRequest();
                //return BadRequest(new { message = "Employee data is required." });
                //return BadRequest("Employee data is required.");
            }

            
            await _dbcontext.Employees.AddAsync(new Employee()
            {
                FullName = employee.FullName,
                Sarf_Id = employee.Sarf_Id,
                PhoneNumber=employee.PhoneNumber,
                NationalId= employee.NationalId,
                Nationality= employee.Nationality,
                Ta2meen_No= employee.Ta2meen_No,
                BankAcc_No = employee.BankAcc_No,
               


            });
            _dbcontext.SaveChanges();
            return Ok();
            //return Ok(new { message = "Added successfully" });

            //return Ok("added successfully");
        }


        [HttpPost("uploadExcelEmployee")]

        public async Task<IActionResult> UploadExcel([FromForm] excelfile obj)
        {
            if (obj.file == null || obj.file.Length <= 0)
            {
                return BadRequest("File is empty");
            }

            // Check file extension
            string fileExtension = Path.GetExtension(obj.file.FileName);
            if (fileExtension != ".xlsx" && fileExtension != ".xls")
            {
                return BadRequest();
                //return BadRequest("Only Excel files are allowed");
            }

            using (var stream = new MemoryStream())
            {
                await obj.file.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    int rowCount = worksheet.Dimension.Rows;

                    List<EmployeeExcelData> excelDataList = new List<EmployeeExcelData>();

                    for (int row = 2; row <= rowCount; row++) // Assuming the first row is header
                    {
                        var excelData = new EmployeeExcelData
                        {
                            Sarf_Id = worksheet.Cells[row, 1].Value?.ToString(),
                            FullName = worksheet.Cells[row, 2].Value?.ToString()
                        };

                        excelDataList.Add(excelData);
                    }

                    // Add data to the database
                    foreach (var data in excelDataList)
                    {
                        await _dbcontext.Employees.AddAsync(new Employee
                        {
                            FullName = data.FullName,
                            Sarf_Id = data.Sarf_Id
                        });
                    }

                    await _dbcontext.SaveChangesAsync();
                }
            }
            return Ok();
            //return Ok("Data uploaded successfully");
        }

        [HttpPost("EmployeeSarf")]
        public async Task<IActionResult> EmployeeSarf(CreateEmployeeSarf dto)
        {
            var employeeSarf = new EmployeeSarf()
            {
                SarfDate = dto.SarfDate,
                EmployeeId = _dbcontext.Employees.FirstOrDefault(x => x.Sarf_Id == dto.SarfId).Id,

            };
            List<EmployeeSarf_Esthkak> employeeSarf_Esthkakss = new List<EmployeeSarf_Esthkak>();
            for (int i = 0; i < dto.Esthkaks.Count; i++)
            {

                employeeSarf_Esthkakss.Add(new EmployeeSarf_Esthkak()
                {
                    EsthkakId = dto.Esthkaks[i].EsthkakId,
                    EsthkakValue = dto.Esthkaks[i].EsthkakValue,
                });



            }
            employeeSarf.EmployeeSarf_Esthkaks = new List<EmployeeSarf_Esthkak>();
            employeeSarf.EmployeeSarf_Esthkaks.AddRange(employeeSarf_Esthkakss);


            List<EmployeeSarf_Estkta3> employeeSarf_Estkta3s = new List<EmployeeSarf_Estkta3>();
            for (int i = 0; i < dto.Estkta3s.Count; i++)
            {

                employeeSarf_Estkta3s.Add(new EmployeeSarf_Estkta3()
                {
                    Estkta3Id = dto.Estkta3s[i].Estkta3Id,
                    Estkta3Value = dto.Estkta3s[i].Estkta3Value,
                });

            }
            employeeSarf.EmployeeSarf_Estkta3s = new List<EmployeeSarf_Estkta3>();
            employeeSarf.EmployeeSarf_Estkta3s.AddRange(employeeSarf_Estkta3s);
            _dbcontext.EmployeeSarfs.Add(employeeSarf);
            _dbcontext.SaveChanges();
            return Ok("Added Successfully");
        }


        // DELETE: api/Employee/{id}
        [HttpGet]
        [ Route("delete/{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _dbcontext.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
                //return NotFound($"Employee with id {id} not found.");
            }

            _dbcontext.Employees.Remove(employee);
            await _dbcontext.SaveChangesAsync();
            return Ok();
            //return Ok($"Employee with id {id} deleted successfully.");
        }

        // DELETE: api/Employee/deleteSelected
        [HttpPost("deleteSelected")]
        public async Task<IActionResult> DeleteSelectedEmployees([FromBody] List<int> ids)
        {
            if (ids == null || !ids.Any())
            {
                return BadRequest();
            }

            var employeesToDelete = await _dbcontext.Employees
                .Where(e => ids.Contains(e.Id))
                .ToListAsync();

            if (!employeesToDelete.Any())
            {
                return NotFound();
            }

            _dbcontext.Employees.RemoveRange(employeesToDelete);
            await _dbcontext.SaveChangesAsync();

            return Ok();
        }


        // DELETE: api/Employee/deleteAll
        [HttpGet("deleteAll")]
        public async Task<IActionResult> DeleteAllEmployees()
        {
            var allEmployees = await _dbcontext.Employees.ToListAsync();

            if (!allEmployees.Any())
            {
                return NotFound();
            }

            _dbcontext.Employees.RemoveRange(allEmployees);
            await _dbcontext.SaveChangesAsync();
            return Ok();
        }
    }
}
