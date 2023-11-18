namespace MinApiOnNet.Models
{
    public class User
    {
        public int ID { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public static List<User> UserDataSeed()
        {
            return new List<User>
            {
                new User{ ID = 1, FirstName = "Kevin", LastName = "Garnett"},
                new User{ ID = 2, FirstName = "Stephen", LastName = "Curry"},
                new User{ ID = 3, FirstName = "Kevin", LastName = "Durant"}
            };
        }
    }
}
