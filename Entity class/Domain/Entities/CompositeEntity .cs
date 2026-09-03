using smart_home_Asp.net.Exceptions;
namespace smart_home_Asp.net.Domain.Entities
{
    public abstract class CompositeEntity : Entity
    {

        protected List<Entity> BelowEntities = new List<Entity>();
        protected CompositeEntity(string id) : base(id) { }

        protected virtual bool IsAllowedChild(Entity child) => false;

    

        public void AddBelowEntity(Entity child)
        {
            if (child == null)
                throw new ArgumentNullException(nameof(child));

            if (!IsAllowedChild(child))
                throw new InvalidChildException(Id, child.Id);

            if (BelowEntities.Any(e => e.Id == child.Id))
                throw new EntityAlreadyExistsException(child.Id);

            BelowEntities.Add(child);
        }


       
        public void RemoveBelowEntities(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Id cannot be null or empty.", nameof(id));

            var result = BelowEntities.Find(r => r.Id == id);
            if (result == null)
                throw new EntityNotFoundException(id);

            BelowEntities.Remove(result);
        }



        public IReadOnlyList<Entity> GetBelowEntities() => BelowEntities.AsReadOnly();


        public Entity FindEntity(string id)

        {
            Entity entity =  BelowEntities.Find(r => r.Id == id);
            if (entity == null)
            {
                throw new EntityNotFoundException(id);
            }
            else
                return entity;


        }


    }
}