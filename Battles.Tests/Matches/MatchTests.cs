using System.Collections;
using System.Linq;
using System.Reflection;
using Battles.Models;
using Xunit;

namespace Battles.Tests.Matches
{
    public class MatchTests
    {
        [Fact]
        public void MatchHasNoNulls_WhenCreated()
        {
            var match = new Match();
            var props = match.GetType().GetProperties();

            var collections = props
                .Where(PropIsCollection)
                .Select(x => x.GetValue(match));

            foreach (dynamic x in collections)
            {
                Assert.NotNull(x);
            }
        }

        private static bool PropIsCollection(PropertyInfo property)
        {
            var propType = property.PropertyType.GetTypeInfo();

            return TypeHasInterfaces(propType) && TypeInheritsIEnumerable(propType);
        }

        private static bool TypeHasInterfaces(TypeInfo type) =>
            type.ImplementedInterfaces.Any();

        private static bool TypeInheritsIEnumerable(TypeInfo type) =>
            type.ImplementedInterfaces
                .Any(y => y.GetTypeInfo() == typeof(IEnumerable));
    }
}
