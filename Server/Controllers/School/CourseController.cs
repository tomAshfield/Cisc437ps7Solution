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
using Telerik.DataSource;
using Telerik.DataSource.Extensions;

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
            List<CourseDTO> lstCourses = await _context.Courses.OrderBy(x => x.Description)
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
                .Where(x => x.CourseNo == pCourseNo)
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
            var trans = await _context.Database.BeginTransactionAsync();
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
            await _context.Database.CommitTransactionAsync();
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

        [HttpPost]
        [Route("GetCourses")]
        public async Task<DataEnvelope<CourseDTO>> GetCoursesPost([FromBody] DataSourceRequest gridRequest)
        {
            DataEnvelope<CourseDTO> dataToReturn = null;
            IQueryable<CourseDTO> queriableStates = _context.Courses
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
                        SchoolId = sp.SchoolId,
                        SchoolName = sp.School.SchoolName
                    }) ;

            // use the Telerik DataSource Extensions to perform the query on the data
            // the Telerik extension methods can also work on "regular" collections like List<T> and IQueriable<T>
            try
            {

                DataSourceResult processedData = await queriableStates.ToDataSourceResultAsync(gridRequest);

                if (gridRequest.Groups.Count > 0)
                {
                    // If there is grouping, use the field for grouped data
                    // The app must be able to serialize and deserialize it
                    // Example helper methods for this are available in this project
                    // See the GroupDataHelper.DeserializeGroups and JsonExtensions.Deserialize methods
                    dataToReturn = new DataEnvelope<CourseDTO>
                    {
                        GroupedData = processedData.Data.Cast<AggregateFunctionsGroup>().ToList(),
                        TotalItemCount = processedData.Total
                    };
                }
                else
                {
                    // When there is no grouping, the simplistic approach of 
                    // just serializing and deserializing the flat data is enough
                    dataToReturn = new DataEnvelope<CourseDTO>
                    {
                        CurrentPageData = processedData.Data.Cast<CourseDTO>().ToList(),
                        TotalItemCount = processedData.Total
                    };
                }
            }
            catch (Exception e)
            {
                //fixme add decent exception handling
            }
            return dataToReturn;
        }

    }
}

