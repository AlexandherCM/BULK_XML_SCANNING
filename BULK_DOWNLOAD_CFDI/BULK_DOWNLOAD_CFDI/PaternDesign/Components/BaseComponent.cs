using Microsoft.Extensions.Configuration;

#pragma warning disable CS8618
#pragma warning disable CS8601
#pragma warning disable CS8602
#pragma warning disable CS8604

namespace BULK_DOWNLOAD_CFDI.PaternDesign.Components
{
    public class BaseComponent  
    {
        protected IConfiguration _configuration;

        protected Mediator? _mediator;

        public BaseComponent() { }
        public BaseComponent(IConfiguration configuration)  
        {
            _configuration = configuration;
        }
    }
}
