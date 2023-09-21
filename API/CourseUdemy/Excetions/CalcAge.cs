using System.Runtime.CompilerServices;

namespace CourseUdemy.Excetions
{
    public static class CalcAge
    {
        public static int CalcAgetion ( this DateOnly dob )
        {
            var today=DateOnly.FromDateTime(DateTime.UtcNow);
            var age=today.Year-dob.Year;
            if ( dob > today.AddYears (-age) ) age--;
            return age;
        }
    }
}
