﻿namespace SavourySolutions.Data.Common.Models
{
    using System;

    public abstract class BaseDeletableModel<TKey> : BaseModel<TKey>, IDeletableEntity
    {
        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }
        public DateTime? CreatedOn { get; set; } = DateTime.Now;
    }
}
