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

namespace SNICKERS.Client.Pages.School
{
    public partial class Course : SnickersUI
    {
        private List<CourseDTO> lstcourse { get; set; }

        private int itmCourse { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            IsLoading = true;
            await LoadLookupData();
            IsLoading = false;


        }
        private async Task LoadLookupData()
        {
             lstcourse = await Http.GetFromJsonAsync<List<CourseDTO>>("api/Course/GetCourses", options);
        }
    }
}
