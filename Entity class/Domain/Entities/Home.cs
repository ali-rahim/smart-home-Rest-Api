namespace smart_home_Asp.net.Domain.Entities
{
    public class Home : CompositeEntity
    {
        public Home(string id) : base(id) { }
        protected override bool IsAllowedChild(Entity child) => child is Room;
    }
}
