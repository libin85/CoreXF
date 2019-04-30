
using System;
using System.Threading.Tasks;

namespace CoreXF
{
    public class PageParameters 
    {
        public Action OnClose;

        public Func<object, Task> OnSuccessfulCloseAsync;

        public override string ToString() =>
            this.SerializeShallowToJson(maxparamlength: 50);
      
    }
}
