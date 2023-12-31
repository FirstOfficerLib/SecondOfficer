using SecondOfficer.Generator.Enums;

namespace SecondOfficer.Generator.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class RestConfigAttribute : Attribute
    {
        private readonly string _roles;

        public RestConfigAttribute(Actions actions, string roles)
        {
            Actions = actions;
            _roles = roles;
        }

        public string[] Roles => _roles.Split(',').Select(a => a.Trim()).ToArray();
        public Actions Actions { get; }
    }
}
