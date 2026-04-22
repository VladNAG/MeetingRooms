using MeetingRooms.Application.Exceptions;

namespace MeetingRooms.Application.Extensions
{
    public static class NotFoundExtensions
    {
        public static T EnsureExists<T>(this T entity, Guid id) where T : class
        {
            if (entity is null)
                throw NotFoundException.For<T>(id);

            return entity;
        }
    }
}
