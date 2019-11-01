using System;
using System.Collections.Generic;
using System.Text;

namespace BackendFunctionApp.Utils
{
    public static class ClaimConstants
    {
        public const string Name = "name";
        public const string ObjectId = "http://schemas.microsoft.com/identity/claims/objectidentifier";
        public const string Oid = "oid";
        public const string PreferredUserName = "preferred_username";
        public const string TenantId = "http://schemas.microsoft.com/identity/claims/tenantid";
        public const string Tid = "tid";
        public const string ScopeClaimType = "http://schemas.microsoft.com/identity/claims/scope";
        public const string RolesClaimType = "roles";

        public const string ScopeClaimValue = "user_impersonation";

    }
}
