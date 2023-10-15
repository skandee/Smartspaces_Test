using App.Enum;

namespace App.Entities
{
    public class Company
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public Classification Classification { get; set; }
    }
}