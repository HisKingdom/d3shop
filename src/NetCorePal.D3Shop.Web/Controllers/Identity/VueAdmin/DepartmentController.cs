using Microsoft.AspNetCore.Mvc;
using NetCorePal.D3Shop.Web.Controllers.Identity.VueAdmin.Models;

namespace PlaygroundApi.Controllers
{
    [ApiController]
    [Route("api/system/dept")]
    public class DepartmentController : ControllerBase
    {
        private static readonly List<Department> _departments = new List<Department>
        {
            new Department
            {
                Id = 1,
                Name = "技术部",
                Code = "TECH",
                Description = "技术部门",
                CreateTime = DateTime.Now,
                IsActive = true,
                Children = new List<Department>
                {
                    new Department
                    {
                        Id = 2,
                        Name = "前端组",
                        Code = "FRONTEND",
                        ParentId = 1,
                        Description = "前端开发组",
                        CreateTime = DateTime.Now,
                        IsActive = true
                    },
                    new Department
                    {
                        Id = 3,
                        Name = "后端组",
                        Code = "BACKEND",
                        ParentId = 1,
                        Description = "后端开发组",
                        CreateTime = DateTime.Now,
                        IsActive = true
                    }
                }
            },
            new Department
            {
                Id = 4,
                Name = "产品部",
                Code = "PROD",
                Description = "产品部门",
                CreateTime = DateTime.Now,
                IsActive = true
            }
        };

        [HttpGet("list")]
        public ActionResult<object> GetDepartments()
        {
            try
            {
                var items = _departments.Select(d => new
                {
                    id = d.Id.ToString(),
                    name = d.Name,
                    code = d.Code,
                    parentId = d.ParentId?.ToString(),
                    status = d.IsActive ? 1 : 0,
                    remark = d.Description,
                    createTime = d.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    children = d.Children?.Select(c => new
                    {
                        id = c.Id.ToString(),
                        name = c.Name,
                        code = c.Code,
                        parentId = c.ParentId?.ToString(),
                        status = c.IsActive ? 1 : 0,
                        remark = c.Description,
                        createTime = c.CreateTime.ToString("yyyy-MM-dd HH:mm:ss")
                    }).ToList()
                }).ToList();

                return new JsonResult(new
                {
                    code = 0,
                    data = items,
                    error = "",
                    message = "ok"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { code = -1, message = "服务器内部错误", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public ActionResult<Department> GetDepartment(int id)
        {
            var department = _departments.FirstOrDefault(d => d.Id == id);
            if (department == null)
            {
                return NotFound(new { message = "部门不存在" });
            }

            return new JsonResult(department);
        }

        [HttpPost]
        public ActionResult<Department> CreateDepartment([FromBody] Department department)
        {
            department.Id = _departments.Max(d => d.Id) + 1;
            department.CreateTime = DateTime.Now;
            department.IsActive = true;
            department.Children = new List<Department>();

            if (department.ParentId.HasValue)
            {
                var parent = _departments.FirstOrDefault(d => d.Id == department.ParentId);
                if (parent == null)
                {
                    return BadRequest(new { message = "父部门不存在" });
                }
                // parent.Children.Add(department);
            }
            else
            {
                _departments.Add(department);
            }

            return CreatedAtAction(nameof(GetDepartment), new { id = department.Id }, department);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateDepartment(int id, [FromBody] Department department)
        {
            var existingDepartment = _departments.FirstOrDefault(d => d.Id == id);
            if (existingDepartment == null)
            {
                return NotFound(new { message = "部门不存在" });
            }

            existingDepartment.Name = department.Name;
            existingDepartment.Code = department.Code;
            existingDepartment.Description = department.Description;

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteDepartment(int id)
        {
            var department = _departments.FirstOrDefault(d => d.Id == id);
            if (department == null)
            {
                return NotFound(new { message = "部门不存在" });
            }

            if (department.Children?.Any() == true)
            {
                return BadRequest(new { message = "该部门下还有子部门，无法删除" });
            }

            if (department.ParentId.HasValue)
            {
                var parent = _departments.FirstOrDefault(d => d.Id == department.ParentId);
                //  parent?.Children.Remove(department);
            }
            else
            {
                _departments.Remove(department);
            }

            return NoContent();
        }
    }
}

