﻿using FInalProject.Data.Models;
namespace FInalProject.ViewModels
{
    public class BooksLeaderboardViewModel
    {
        public int BookId { get; set; }
        public string BookName { get; set; }
        public string AuthorName { get; set; }
        public string CategoryString { get; set; }
        public int PositiveReviews { get; set; }
        public int NegativeReviews { get; set; }
    }
}
