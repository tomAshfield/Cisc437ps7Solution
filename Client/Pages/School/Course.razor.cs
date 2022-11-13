using SNICKERS.Client.Helper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

using SNICKERS.Shared;
using SNICKERS.Shared.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Telerik.Blazor.Components;
using SNICKERS.Client.Services;

namespace SNICKERS.Client.Pages.School
{
    public partial class Course : SnickersUI
    {
        private List<CourseDTO>? lstcourse { get; set; }

        [Inject]
        CourseService _CourseService { get; set; }

        private int itmCourse { get; set; }

        public TelerikGrid<CourseDTO>? Grid { get; set; }

        public List<int?> PageSizes => true ? new List<int?> { 15, 25, 50, null } : null;
        private int PageSize = 15;
        private int PageIndex { get; set; } = 2;
        private async Task PageChangedHandler(int currPage)
        {
            PageIndex = currPage;
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            IsLoading = true;
            //await LoadLookupData();
            IsLoading = false;


        }
        //private async Task LoadLookupData()
        //{
        //     lstcourse = await Http.GetFromJsonAsync<List<CourseDTO>>("api/Course/GetCourses", options);
        //}

        public async Task ReadItems(GridReadEventArgs args)
        {
            IsLoading = true;
            DataEnvelope<CourseDTO> result = await _CourseService.GetCoursesService(args.Request);

            if (args.Request.Groups.Count > 0)
            {
                /***
                NO GROUPING FOR THE TIME BEING
                var data = GroupDataHelpers.DeserializeGroups<WeatherForecast>(result.GroupedData);
                GridData = data.Cast<object>().ToList();
                ***/
            }
            else
            {
                lstcourse = result.CurrentPageData.ToList();
            }

            args.Total = result.TotalItemCount;
            args.Data = result.CurrentPageData.ToList();

            IsLoading = false;

            StateHasChanged();
        }

        private void NewCourse(GridCommandEventArgs e)
        {
            CourseDTO _CourseDTO = e.Item as CourseDTO;

        }
        private void DeleteCourse(GridCommandEventArgs e)
        {
            CourseDTO _CourseDTO = e.Item as CourseDTO;
        }

    }
}
