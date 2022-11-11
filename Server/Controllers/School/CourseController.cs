using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SNICKERS.EF;
using SNICKERS.EF.Models;
using SNICKERS.Server.Models;
using SNICKERS.Shared;
using SNICKERS.Shared.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;


namespace SNICKERS.Server.Controllers.School
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : Controller
    {
        protected readonly SNICKERSOracleContext _context;
        protected readonly IHttpContextAccessor _httpContextAccessor;

        public CourseController(SNICKERSOracleContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            this._context = context;
            this._httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [Route("GetCourses")]
        public async Task<IActionResult> GetCourses()
        {
            List<CourseDTO> lstCourses = await _context.Courses.OrderBy(x => x.CourseNo)
               .Select(sp => new CourseDTO
               {
                   Cost = sp.Cost,
                   CourseNo = sp.CourseNo,
                   CreatedBy = sp.CreatedBy,
                   CreatedDate = sp.CreatedDate,
                   Description = sp.Description,
                   ModifiedBy = sp.ModifiedBy,
                   ModifiedDate = sp.ModifiedDate,
                   Prerequisite = sp.Prerequisite,
                   PrerequisiteSchoolId = sp.PrerequisiteSchoolId,
                   SchoolId = sp.SchoolId
               }).ToListAsync();

            return Ok(lstCourses);
        }

        [HttpGet]
        [Route("GetCourses/{pCourseNo}")]
        public async Task<IActionResult> GetCourses(int pCourseNo)
        {
            CourseDTO itmCourse = await _context.Courses
                .Where(x=>x.CourseNo == pCourseNo)
                .OrderBy(x => x.CourseNo)
               .Select(sp => new CourseDTO
               {
                   Cost = sp.Cost,
                   CourseNo = sp.CourseNo,
                   CreatedBy = sp.CreatedBy,
                   CreatedDate = sp.CreatedDate,
                   Description = sp.Description,
                   ModifiedBy = sp.ModifiedBy,
                   ModifiedDate = sp.ModifiedDate,
                   Prerequisite = sp.Prerequisite,
                   PrerequisiteSchoolId = sp.PrerequisiteSchoolId,
                   SchoolId = sp.SchoolId
               }).FirstOrDefaultAsync();

            return Ok(itmCourse);
        }

        [HttpPost]
        public async Task<IActionResult> PostCourse(CourseDTO _CourseDTO)
        {
            var trans = await  _context.Database.BeginTransactionAsync();
            Course c = new Course
            {
                Cost = _CourseDTO.Cost,
                CourseNo = _CourseDTO.CourseNo,
                Description = _CourseDTO.Description,
                PrerequisiteSchoolId = _CourseDTO.PrerequisiteSchoolId,
                SchoolId = _CourseDTO.SchoolId
            };
            _context.Courses.Add(c);
            await _context.SaveChangesAsync();
            await  _context.Database.CommitTransactionAsync();
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> PutCourse(CourseDTO _CourseDTO)
        {
            var trans = await _context.Database.BeginTransactionAsync();
            Course c = await _context.Courses.Where(x => x.CourseNo.Equals(_CourseDTO.CourseNo)).FirstOrDefaultAsync();

            if (c != null)
            {
                c.Cost = _CourseDTO.Cost;
                c.Description = _CourseDTO.Description;
                c.SchoolId = _CourseDTO.SchoolId;
                c.PrerequisiteSchoolId = _CourseDTO.PrerequisiteSchoolId;
                c.Prerequisite = _CourseDTO.Prerequisite;

                _context.Courses.Update(c);
                await _context.SaveChangesAsync();
                await _context.Database.CommitTransactionAsync();
            }



            return Ok();
        }

        [HttpDelete]
        [Route("DeleteCourse/{pCourseNo}")]
        public async Task<IActionResult> DeleteCourse(int pCourseNo)
        {
            var trans = await _context.Database.BeginTransactionAsync();
            Course c = await _context.Courses.Where(x => x.CourseNo.Equals(pCourseNo)).FirstOrDefaultAsync();
            _context.Courses.Remove(c);
             
            await _context.SaveChangesAsync();
            await _context.Database.CommitTransactionAsync();

            return Ok();
        }
    }
}
