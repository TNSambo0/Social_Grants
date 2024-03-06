namespace Social_Grants.Data
{
    public class AgeChecker
    {
        public static int Check(DateTime dateOfBirth)
        {
            int age = DateTime.Now.Year - dateOfBirth.Year;
            if (DateTime.Now.DayOfYear < dateOfBirth.DayOfYear)
                age--;
            return age;
        }
        public static DateTime DateOfBirth(string IdNumber)
        {
            string dob = "";
            if ($"{IdNumber[0]}" != "0")
            {
                dob += "19";
            }
            else
                dob += "20";
            for (int i = 0; i < 6; i++)
            {
                if (i != 0)
                {
                    if (i % 2 != 0)
                    {
                        if (i != 5)
                        {
                            dob += $"{IdNumber[i]}/";
                        }
                        else
                            dob += $"{IdNumber[i]}";
                    }
                    else
                        dob += IdNumber[i];
                }
                else
                    dob += IdNumber[i];
            }
            DateTime convertedDate = Convert.ToDateTime(dob);
            return convertedDate;
        }
    }
}
