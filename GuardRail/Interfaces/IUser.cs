﻿using System;

namespace GuardRail.Interfaces
{
    /// <summary>
    /// A user in the system.
    /// </summary>
    public interface IUser
    {
        /// <summary>
        /// The ID of the user.
        /// </summary>
        Guid Id { get; }
    }
}