﻿using System.ComponentModel.DataAnnotations;

namespace MyOwnSummary_API.Models.Dtos.LanguageDtos
{
    public class LanguageDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
