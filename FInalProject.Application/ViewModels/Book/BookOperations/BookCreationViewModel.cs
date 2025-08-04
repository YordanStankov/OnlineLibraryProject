﻿using FInalProject.Data.Models;
using System.ComponentModel.DataAnnotations;
namespace FInalProject.Application.ViewModels.Book
{
    public class BookCreationViewModel
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; }
       
        [Required]
        [Range(1, 10000, ErrorMessage = "Pages must be between 1 and 10000 pages")]
        public int Pages { get; set; }
        public string AuthorName { get; set; }
        [Required]
        [Range(1, 1000, ErrorMessage = "Reading time must be between 1 and 1000 hours")]
        public double ReadingTime { get; set; }
        [Required]
        public string CoverImage { get; set; }
        [Required]
        public DateTime DateWritten { get; set; }

        [Required]
        [StringLength(1000, MinimumLength = 5, ErrorMessage = "Description must be between 5 and 1000 words")]
        public string Description { get; set; }
        public int AmountInStock { get; set; }
        public Category Category { get; set; }
        public int? editor { get; set; }
        public ICollection<FInalProject.Data.Models.Genre>? GenreOptions { get; set; } 
        public ICollection<int>? SelectedGenreIds { get; set; } 
    }
}
