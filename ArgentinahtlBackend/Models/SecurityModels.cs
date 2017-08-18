using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

using ArgentinahtlMVC.Models.Services;

namespace ArgentinahtlMVC.Models
{
    [TypeConverter(typeof(PascalCaseWordSplittingEnumConverter))]
    public enum ObjectType
    {
        NullObject = -1,
        Application = 1,
        Report = 2,
        Table = 3,
        Column = 4
    }

    [TypeConverter(typeof(PascalCaseWordSplittingEnumConverter))]
    public enum Permission
    {
        EXEC = 1,
        PRINT = 2,
        READ = 3
    }

    public class ObjectPermissionModel
    {
        private static ObjectPermissionModel nullObject = new ObjectPermissionModel()
        {
            ObjectCode = 0,
            Type = ObjectType.NullObject,
            Description = string.Empty,
            Parent = -1,
            Permissions = new List<Permission>()
        };

        public static ObjectPermissionModel NullObject { get { return nullObject; } }

        public long ObjectCode { get; set; }
        public string Description { get; set; }
        public ObjectType Type { get; set; }
        public long Parent { get; set; }
        public IEnumerable<Permission> Permissions { get; set; }
        public IEnumerable<string> Children { get; set; }
    }

    public class UserPermissionModel
    {
        public Dictionary<long, ObjectPermissionModel> Objects { get; set; }

        public IEnumerable<ObjectPermissionModel> GetObjectsOfType(ObjectType objectType)
        {
            var objects = new List<ObjectPermissionModel>();

            foreach (ObjectPermissionModel model in Objects.Values)
                if (model.Type == objectType)
                    objects.Add(model);

            return objects;
        }

        public ObjectPermissionModel GetObjectByCode(long objectCode)
        {
            var model = ObjectPermissionModel.NullObject;

            if (Objects.ContainsKey(objectCode))
                model = Objects[objectCode];

            return model;
        }

        public ObjectPermissionModel GetObjectByDescription(string objectDescription)
        {
            var objects = new List<ObjectPermissionModel>();

            foreach (ObjectPermissionModel model in Objects.Values)
                if (model.Description == objectDescription)
                    return model;

            return null;
        }

        public bool CanUser(long objectCode, Permission permission)
        {
            return GetObjectByCode(objectCode).Permissions.Contains(permission);
        }

        [Display(Name = "Username")]
        [DataType(DataType.Text)]
        public string Username { get; set; }
    }
}