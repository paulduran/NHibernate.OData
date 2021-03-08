using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iesi.Collections.Generic;
using NHibernate.Type;

namespace NHibernate.OData
{
    internal class ODataSessionFactoryContext
    {
        internal static ODataSessionFactoryContext Empty = new ODataSessionFactoryContext();

        public IDictionary<System.Type, MappedClassMetadata> MappedClassMetadata { get; private set; }

        private ODataSessionFactoryContext()
        {
            MappedClassMetadata = new Dictionary<System.Type, MappedClassMetadata>();
        }

        public ODataSessionFactoryContext(ISessionFactory sessionFactory)
        {
            Require.NotNull(sessionFactory, "sessionFactory");

            var lookup =
                sessionFactory.GetAllClassMetadata().Values.ToLookup(
                    x => x.MappedClass,
                    x => new MappedClassMetadata(x));

            MappedClassMetadata = lookup.ToDictionary(k => k.Key, v => v.First());
        }
    }
}
