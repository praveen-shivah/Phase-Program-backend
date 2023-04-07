namespace IdentityServerAPI
{
    using Microsoft.AspNetCore.Mvc;

    public abstract class ApiControllerBase : Controller
    {
        protected int OrganizationId
        {
            get
            {
                var values = this.HttpContext.Request.Headers["OrganizationId"];

                if (values.Count > 0)
                {
                    return int.Parse(values[0]);
                }

                return 0;
            }
        }

        protected int UserId
        {
            get
            {
                var values = this.HttpContext.Request.Headers["UserId"];

                if (values.Count > 0)
                {
                    return int.Parse(values[0]);
                }

                return 0;
            }
        }
    }
}