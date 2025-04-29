using System.ComponentModel.DataAnnotations;

namespace FInalProject.ViewModels
{
    public class FilteringViewModel
    {
        public string? Name { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateOfPublication { get; set; }

        public string? SortBy { get; set; } 
        public string? SortDirection { get; set; } 
    }
}