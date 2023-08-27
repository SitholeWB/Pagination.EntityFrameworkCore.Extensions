namespace WebApi.Entities
{
    public class Country
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public DateTimeOffset DateAdded { get; set; }
        public DateTimeOffset LastModifiedDate { get; set; }

        public virtual ICollection<Person> Persons { get; set; } = new List<Person>();
    }
}