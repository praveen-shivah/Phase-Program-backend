namespace IdentityServerAPI
{
    using System.Xml;
    using APISupport;

    using ApplicationLifeCycle;
    using IdentityServerDatabaseModels;
    using LoggingLibrary;

    using SecurityUtilities;

    using SecurityUtilityTypes;

    using SimpleInjector;

    public class CompositeRoot : CompositeRootBase
    {
        protected override bool registerBindings()
        {
            var environment = this.getEnvironmentName();

            Console.WriteLine($"*********************  environment {environment}");
            this.GlobalContainer.RegisterInstance(typeof(IConfiguration), this.buildConfig(environment));
            this.GlobalContainer.Register<ILoggerAdapterFactory, LoggerAdapterFactory>(Lifestyle.Singleton);
            this.GlobalContainer.Register<ILoggerFactory, LoggerFactory>(Lifestyle.Singleton);
            this.GlobalContainer.Register<ISecretKeyRetrieval, SecretKeyRetrievalSettingsFile>(Lifestyle.Singleton);
            this.GlobalContainer.Register<IJwtValidate,JwtValidateDefault>(Lifestyle.Singleton);
            this.GlobalContainer.Register<ICalculatePassword, CalculatePassword>(Lifestyle.Singleton);
            this.GlobalContainer.Register<ICreatePasswordSalt, CreatePasswordSalt>(Lifestyle.Singleton);

            return true;
        }

        private string getEnvironmentName()
        {
            if (!File.Exists("web.config"))
            {
                return "Development";
            }

            XmlDocument doc = new XmlDocument();
            doc.Load("web.config");

            if (doc.DocumentElement == null)
            {
                return "Development";
            }

            XmlElement root = doc.DocumentElement;
            var node = root.SelectSingleNode("//environmentVariables");
            if (node == null)
            {
                return "Development";
            }

            var env = node["environmentVariable"];
            if (env == null)
            {
                return "Development";
            }

            return env.Attributes[1].Value;
        }

        private IConfiguration buildConfig(string environment)
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile($"appsettings.{environment}.json", true);
            IConfiguration config = builder.Build();
            return config;
        }

    }
}
