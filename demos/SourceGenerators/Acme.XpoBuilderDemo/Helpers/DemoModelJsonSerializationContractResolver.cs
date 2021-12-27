
using System;
using System.Collections.Generic;
using System.Linq;

using System.Reflection;

using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;

using Newtonsoft.Json.Serialization;

namespace Acme.Module.Helpers
{
    public class DemoModelJsonSerializationContractResolver : DefaultContractResolver
    {
        private readonly XPDictionary dictionary;

        public bool SerializeCollections { get; set; } = true;
        public bool SerializeReferences { get; set; } = true;
        public bool SerializeByteArrays { get; set; } = true;

        public DemoModelJsonSerializationContractResolver() => dictionary = XpoDefault.Dictionary;

        protected override List<MemberInfo> GetSerializableMembers(Type objectType)
        {
            var classInfo = dictionary.QueryClassInfo(objectType);
            if (classInfo != null && classInfo.IsPersistent)
            {
                var allSerializableMembers = base.GetSerializableMembers(objectType);
                var serializableMembers = new List<MemberInfo>(allSerializableMembers.Count);
                foreach (var member in allSerializableMembers)
                {
                    var mi = classInfo.FindMember(member.Name);
                    if (!(mi.IsPersistent || mi.IsAliased || mi.IsCollection || mi.IsManyToManyAlias)
                        || ((mi.IsCollection || mi.IsManyToManyAlias) && !SerializeCollections)
                        || (mi.ReferenceType != null && !SerializeReferences)
                        || (mi.MemberType == typeof(byte[]) && !SerializeByteArrays))
                    {
                        continue;
                    }
                    serializableMembers.Add(member);
                }
                return serializableMembers;
            }
            return base.GetSerializableMembers(objectType);
        }
    }
}
