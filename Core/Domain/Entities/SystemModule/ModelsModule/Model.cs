namespace Domain.Entities.SystemModule.ModelsModule
{
    public abstract class Model : BaseEntity<int>
    {
        public string Name { get; set; }
        public int SystemId { get; set; }
        public SystemRoot System { get; set; }
    }
}
