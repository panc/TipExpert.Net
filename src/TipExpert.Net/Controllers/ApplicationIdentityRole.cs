using System;

namespace TipExpert.Net.Controllers
{
    /// <summary>
    /// Represents a Role entity
    /// </summary>
    public sealed class ApplicationIdentityRole
    {
        /// <summary>
        /// A random value that should change whenever a role is persisted to the store
        /// </summary>
        public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Role id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Role name
        /// </summary>
        public string Name { get; set; }

        public string NormalizedName { get; set; }

        public ApplicationIdentityRole()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="roleName"/>
        public ApplicationIdentityRole(string roleName)
          : this()
        {
            Name = roleName;
        }
    }
}