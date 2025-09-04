using System;

namespace PromoCodeFactory.WebHost.ExceptionHandling.Exceptions
{
    public class NotFoundEntityException : AppException
    {
        public NotFoundEntityException(string entity, Guid id)
            : base("NOT_FOUND", $"{entity} with ID {id} not found", 404)
        {
        }
    }
}
