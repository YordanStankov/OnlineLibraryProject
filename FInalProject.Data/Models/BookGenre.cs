﻿using System.ComponentModel.DataAnnotations.Schema;

namespace FInalProject.Data.Models
{
    public class BookGenre
    {

        [ForeignKey(nameof(Book))]
        public int BookId { get; set; }
        public Book Book { get; set; }
        [ForeignKey(nameof(Genre))]
        public int GenreId { get; set; }
        public Genre Genre { get; set; }
    }
}
