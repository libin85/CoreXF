

namespace CoreXF
{
    public abstract class DBModelAbsrtact
    {
        public void Save()
        {
            SystemDB.Connection.InsertOrReplace(this);
        }

        public void Delete()
        {
            SystemDB.Connection.Delete(this);
        }
    }
}
