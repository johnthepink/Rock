//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by the Rock.CodeGeneration project
//     Changes to this file will be lost when the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
//
// THIS WORK IS LICENSED UNDER A CREATIVE COMMONS ATTRIBUTION-NONCOMMERCIAL-
// SHAREALIKE 3.0 UNPORTED LICENSE:
// http://creativecommons.org/licenses/by-nc-sa/3.0/
//

using System;
using System.Linq;

using Rock.Data;

namespace Rock.Model
{
    /// <summary>
    /// GroupLocation Service class
    /// </summary>
    public partial class GroupLocationService : Service<GroupLocation>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupLocationService"/> class
        /// </summary>
        public GroupLocationService()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupLocationService"/> class
        /// </summary>
        /// <param name="repository">The repository.</param>
        public GroupLocationService(IRepository<GroupLocation> repository) : base(repository)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupLocationService"/> class
        /// </summary>
        /// <param name="context">The context.</param>
        public GroupLocationService(RockContext context) : base(context)
        {
        }

        /// <summary>
        /// Determines whether this instance can delete the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns>
        ///   <c>true</c> if this instance can delete the specified item; otherwise, <c>false</c>.
        /// </returns>
        public bool CanDelete( GroupLocation item, out string errorMessage )
        {
            errorMessage = string.Empty;
            return true;
        }
    }

    /// <summary>
    /// Generated Extension Methods
    /// </summary>
    public static partial class GroupLocationExtensionMethods
    {
        /// <summary>
        /// Clones this GroupLocation object to a new GroupLocation object
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="deepCopy">if set to <c>true</c> a deep copy is made. If false, only the basic entity properties are copied.</param>
        /// <returns></returns>
        public static GroupLocation Clone( this GroupLocation source, bool deepCopy )
        {
            if (deepCopy)
            {
                return source.Clone() as GroupLocation;
            }
            else
            {
                var target = new GroupLocation();
                target.CopyPropertiesFrom( source );
                return target;
            }
        }

        /// <summary>
        /// Copies the properties from another GroupLocation object to this GroupLocation object
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="source">The source.</param>
        public static void CopyPropertiesFrom( this GroupLocation target, GroupLocation source )
        {
            target.GroupId = source.GroupId;
            target.LocationId = source.LocationId;
            target.GroupLocationTypeValueId = source.GroupLocationTypeValueId;
            target.IsMailingLocation = source.IsMailingLocation;
            target.IsMappedLocation = source.IsMappedLocation;
            target.GroupMemberPersonId = source.GroupMemberPersonId;
            target.Id = source.Id;
            target.Guid = source.Guid;

        }
    }
}
