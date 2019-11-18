using P01_HospitalDatabase.Data;

namespace P01_HospitalDatabase
{
    public class StartUp
    {
        private static void Main()
        {
            var db = new HospitalContext();
            db.Database.EnsureCreated();
        }
    }
}