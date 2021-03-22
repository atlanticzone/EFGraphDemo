using HotChocolate.Types;

namespace GraphDemo.Entities
{
    public class OrderQueryType : ObjectType<OrderQueries>
    {
        protected override void Configure(IObjectTypeDescriptor<OrderQueries> descriptor)
        {
            base.Configure(descriptor);

            descriptor
                .Field(f => f.GetOrders(default, default))
                .Argument("customerId", a => a.Type<LongType>())
                .UsePaging()
                .UseProjection()
                .UseFiltering()
                .UseSorting();
        }
    }
}