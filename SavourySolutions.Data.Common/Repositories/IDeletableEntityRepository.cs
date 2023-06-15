﻿namespace SavourySolutions.Data.Common.Repositories
{
    using System.Linq;

    using SavourySolutions.Data.Common.Models;

    public interface IDeletableEntityRepository<TEntity> : IRepository<TEntity>
        where TEntity : class, IDeletableEntity
    {
        IQueryable<TEntity> AllWithDeleted();

        IQueryable<TEntity> AllAsNoTrackingWithDeleted();

        void HardDelete(TEntity entity);

        void Undelete(TEntity entity);
        Task<TEntity> GetByIdAsync(object id);
    }
}
