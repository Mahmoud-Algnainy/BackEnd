using BackEnd.DTO.Employee;
using BackEnd.DTO.Excel;
using BackEnd.DTO.SarfsDTO;
using BackEnd.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SarfsController : ControllerBase
    {
        private readonly HcfiDBContext _dbcontext;

        public SarfsController(HcfiDBContext dbcontext)
        {
            _dbcontext = dbcontext;
        }


        [HttpPost("EmployeeSarfUploadExcel")]
        public async Task<IActionResult> EmployeeSarfUploadExcel([FromForm] excelfile obj)
        {
          
            {
                if (obj.file == null || obj.file.Length <= 0)
                {
                    return BadRequest("File is empty");
                }

                // Parse Excel file
                using (var stream = new MemoryStream())
                {
                    await obj.file.CopyToAsync(stream);
                    stream.Position = 0; // Reset stream position

                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                        int rowCount = worksheet.Dimension.Rows;

                        for (int row = 2; row <= rowCount; row++) // Assuming the first row is header
                        {
                            // Extract data from Excel
                            string sarfId = worksheet.Cells[row, 1].Value?.ToString();
                            DateTime sarfDate = DateTime.Parse(worksheet.Cells[row, 33].Value?.ToString());

                            // Extract Esthkaks values
                            List<EsthkakDto> esthkaks = new List<EsthkakDto>();
                            for (int col = 2; col <= 23; col++)
                            {
                                decimal value = decimal.Parse(worksheet.Cells[row, col].Value?.ToString() ?? "0");
                                esthkaks.Add(new EsthkakDto { EsthkakId = col - 1, EsthkakValue = value });
                            }

                            // Extract Estkta3s values
                            List<Estkta3Dto> estkta3s = new List<Estkta3Dto>();
                            for (int col = 24; col <= 32; col++)
                            {
                                decimal value = decimal.Parse(worksheet.Cells[row, col].Value?.ToString() ?? "0");
                                estkta3s.Add(new Estkta3Dto { Estkta3Id = col - 23, Estkta3Value = value });
                            }
                            try
                            {
                                // Try to find the employee
                                var employee = _dbcontext.Employees.FirstOrDefault(x => x.Sarf_Id == sarfId);

                                // If employee is null, log a warning and skip adding this EmployeeSarf entity
                                if (employee == null)
                                {
                                      continue; // Skip this iteration and continue to the next row
                                }

                                // Create EmployeeSarf entity
                                var employeeSarf = new EmployeeSarf()
                                {
                                    SarfDate = sarfDate,
                                    EmployeeId = employee.Id
                                };

                                // Add Esthkaks and Estkta3s
                                employeeSarf.EmployeeSarf_Esthkaks = esthkaks.Select(e => new EmployeeSarf_Esthkak { EsthkakId = e.EsthkakId, EsthkakValue = e.EsthkakValue }).ToList();
                                employeeSarf.EmployeeSarf_Estkta3s = estkta3s.Select(e => new EmployeeSarf_Estkta3 { Estkta3Id = e.Estkta3Id, Estkta3Value = e.Estkta3Value }).ToList();

                                // Add to database
                                _dbcontext.EmployeeSarfs.Add(employeeSarf);
                            }
                            catch (Exception ex)
                            {
                                // Log the exception
                           
                                // Handle the exception, for example, return a StatusCode 500
                                return StatusCode(500, "An error occurred while processing the request");
                            }


                            await _dbcontext.SaveChangesAsync();

                        }
                       


                    }
                }

                return Ok("Added Successfully");
            }
 
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

        [HttpGet("GetAllEmployeesSarf")]
        public async Task<IActionResult> GetAllEmployeesSarf()
        {
            var EmployeeSarfs = await _dbcontext.EmployeeSarfs.Include(e=>e.Employee).ToListAsync();
            return Ok(EmployeeSarfs);
        }
        [HttpGet("EmployeeSarf/{sarfId}/{month}/{year}")]
        public async Task<IActionResult> GetEmployeesSarf(string sarfId,  int month,  int year)
        {
         
            var query = _dbcontext.EmployeeSarfs
        .Include(x => x.EmployeeSarf_Esthkaks)
            .ThenInclude(e => e.Esthkak) // Include the Esthkak entity
        .Include(z => z.EmployeeSarf_Estkta3s)
            .ThenInclude(e => e.Estkta3) // Include the Estkta3 entity
        .Include(emp => emp.Employee).Where(employee => employee.Employee.Sarf_Id == sarfId &&
                                   employee.SarfDate.Month == month &&
                                   employee.SarfDate.Year == year); // Include the Employee entity
            var employees = await query.ToListAsync(); // Materialize the query by converting it to a list

            // Project the data to include Esthkak and Estkta3 names for each employee
            var resultList = employees.Select(employee => new
            {
                Id = employee.Id,
                SarfDate = employee.SarfDate.ToString("MM/yyyy"),
                SarfId = employee.Employee.Sarf_Id,
                EmployeeId = employee.EmployeeId,
                EmployeeName = employee.Employee?.FullName, // Assuming Employee class has FullName property
                EmployeeSarf_Esthkaks = employee.EmployeeSarf_Esthkaks.Select(es => new
                {
                    Id = es.Id,
                    EsthkakName = es.Esthkak?.Name, // Assuming Esthkak class has Name property
                    EsthkakValue = es.EsthkakValue
                }),
                EmployeeSarf_Estkta3s = employee.EmployeeSarf_Estkta3s.Select(es => new
                {
                    Id = es.Id,
                    Estkta3Name = es.Estkta3?.Name, // Assuming Estkta3 class has Name property
                    Estkta3Value = es.Estkta3Value
                }),
                TotalEsthkakValue = employee.EmployeeSarf_Esthkaks.Sum(es => es.EsthkakValue),
                TotalEstkta3Value = employee.EmployeeSarf_Estkta3s.Sum(es => es.Estkta3Value),
                TotalNet = employee.EmployeeSarf_Esthkaks.Sum(es => es.EsthkakValue) - employee.EmployeeSarf_Estkta3s.Sum(es => es.Estkta3Value)
            }).ToList();

            return Ok(resultList);
        }


        [HttpGet("EmployeeSarfBySarfID/{sarfId}")]
        public async Task<IActionResult> EmployeeSarfBySarfID(string sarfId)
        {

            var query = _dbcontext.EmployeeSarfs
        .Include(x => x.EmployeeSarf_Esthkaks)
            .ThenInclude(e => e.Esthkak) // Include the Esthkak entity
        .Include(z => z.EmployeeSarf_Estkta3s)
            .ThenInclude(e => e.Estkta3) // Include the Estkta3 entity
        .Include(emp => emp.Employee).Where(employee => employee.Employee.Sarf_Id == sarfId ); // Include the Employee entity
            var employees = await query.ToListAsync(); // Materialize the query by converting it to a list

            // Project the data to include Esthkak and Estkta3 names for each employee
            var resultList = employees.Select(employee => new
            {
                Id = employee.Id,
                SarfDate = employee.SarfDate.ToString("MM/yyyy"),
                SarfId = employee.Employee.Sarf_Id,
                EmployeeId = employee.EmployeeId,
                EmployeeName = employee.Employee?.FullName, // Assuming Employee class has FullName property
                EmployeeSarf_Esthkaks = employee.EmployeeSarf_Esthkaks.Select(es => new
                {
                    Id = es.Id,
                    EsthkakName = es.Esthkak?.Name, // Assuming Esthkak class has Name property
                    EsthkakValue = es.EsthkakValue
                }),
                EmployeeSarf_Estkta3s = employee.EmployeeSarf_Estkta3s.Select(es => new
                {
                    Id = es.Id,
                    Estkta3Name = es.Estkta3?.Name, // Assuming Estkta3 class has Name property
                    Estkta3Value = es.Estkta3Value
                }),
                TotalEsthkakValue = employee.EmployeeSarf_Esthkaks.Sum(es => es.EsthkakValue),
                TotalEstkta3Value = employee.EmployeeSarf_Estkta3s.Sum(es => es.Estkta3Value),
                TotalNet = employee.EmployeeSarf_Esthkaks.Sum(es => es.EsthkakValue) - employee.EmployeeSarf_Estkta3s.Sum(es => es.Estkta3Value)
            }).ToList();

            return Ok(resultList);
        }





        [HttpPut("EmployeeSarf/{id}")]
        public async Task<IActionResult> UpdateEmployeeSarf(int id, UpdateEmployeeSarf dto)
        {
            var existingEmployeeSarf = _dbcontext.EmployeeSarfs
                .Include(e => e.EmployeeSarf_Esthkaks)
                .Include(e => e.EmployeeSarf_Estkta3s)
                .FirstOrDefault(e => e.Id == id);

            if (existingEmployeeSarf == null)
            {
                return NotFound();
            }

            // Update the properties of the existing EmployeeSarf
            existingEmployeeSarf.SarfDate = dto.SarfDate;
            existingEmployeeSarf.EmployeeId = dto.EmployeeId; 

            // Update EmployeeSarf_Estkta3s
            foreach (var estkta3Dto in dto.Estkta3s)
            {
                var existingEmployeeSarf_Estkta3 = existingEmployeeSarf.EmployeeSarf_Estkta3s
                    .FirstOrDefault(e => e.Id == estkta3Dto.Id);

                if (existingEmployeeSarf_Estkta3 != null)
                {
                    existingEmployeeSarf_Estkta3.Estkta3Id = estkta3Dto.Estkta3Id;
                    existingEmployeeSarf_Estkta3.Estkta3Value = estkta3Dto.Estkta3Value;
                }
                else
                {
                    // If the Estkta3 does not exist, you may choose to add it or handle the scenario accordingly.
                    // For now, let's assume we add a new one.
                    existingEmployeeSarf.EmployeeSarf_Estkta3s.Add(new EmployeeSarf_Estkta3
                    {
                        Estkta3Id = estkta3Dto.Estkta3Id,
                        Estkta3Value = estkta3Dto.Estkta3Value
                    });
                }
            }

            _dbcontext.EmployeeSarfs.Update(existingEmployeeSarf);
            await _dbcontext.SaveChangesAsync();

            return Ok("Updated Successfully");
        }
    }
}
