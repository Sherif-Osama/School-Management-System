using System.Text.RegularExpressions;

namespace School.BLL.Helpers
{
    internal class AcademicYearHelper
    {
        public static (DateOnly Start, DateOnly End) GetAcademicYearRange(string academicYear)
        {
            string[] parts = academicYear.Split('-');

            if (parts.Length != 2 ||
                !int.TryParse(parts[0], out int startYear) ||
                !int.TryParse(parts[1], out int endYear))
            {
                throw new ArgumentException(
                    $"Academic year '{academicYear}' is invalid.",
                    nameof(academicYear));
            }

            if (endYear != startYear + 1)
            {
                throw new ArgumentException(
                    $"Academic year '{academicYear}' is invalid.",
                    nameof(academicYear));
            }

            return (new DateOnly(startYear, 9, 1), new DateOnly(endYear, 8, 31));
        }

        public static string ValidateAcademicYear(string academicYear)
        {
            if (string.IsNullOrWhiteSpace(academicYear))
                throw new ArgumentException("Academic year is required.", nameof(academicYear));

            academicYear = academicYear.Trim();

            if (!Regex.IsMatch(academicYear, @"^\d{4}-\d{4}$"))
                throw new ArgumentException("Academic year must be in the format YYYY-YYYY.", nameof(academicYear));

            int startYear = int.Parse(academicYear[..4]);
            int endYear = int.Parse(academicYear[5..]);

            if (endYear != startYear + 1)
                throw new ArgumentException("Academic year is invalid.", nameof(academicYear));

            return academicYear;
        }
    }
}
