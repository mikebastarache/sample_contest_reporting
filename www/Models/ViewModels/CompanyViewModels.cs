using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;

namespace www.Models.ViewModels
{
    public class CompanyView
    {
        [Key]
        public int CompanyId { get; set; }

        public string CompanyName { get; set; }

        [FileSize(10240)]
        [FileTypes("jpg,jpeg,png,gif")]
        public HttpPostedFileBase CompanyLogo { get; set; }
    }


    public class FileSizeAttribute : ValidationAttribute
    {
        private readonly int maxSize;

        public FileSizeAttribute(int localMaxSize)
        {
            maxSize = localMaxSize;
        }

        public override bool IsValid(object value)
        {
            var file = value as HttpPostedFileBase;
            if (file == null) return true;
            return maxSize > file.ContentLength;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format("The file size should not exceed {0}", maxSize);
        }
    }


    public class FileTypesAttribute : ValidationAttribute
    {
        private readonly List<string> types;

        public FileTypesAttribute(string myTtypes)
        {
            types = myTtypes.Split(',').ToList();
        }

        public override bool IsValid(object value)
        {
            var file = value as HttpPostedFileBase;
            if (file == null) return true;
            var fileExt = Path.GetExtension(file.FileName).Substring(1);
            return types.Contains(fileExt, StringComparer.OrdinalIgnoreCase);
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format("Invalid file type. Only the following types {0} are supported.", String.Join(", ", types));
        }
    }
}