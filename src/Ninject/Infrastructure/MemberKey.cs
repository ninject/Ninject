using System;
using System.Reflection;

namespace Ninject.Infrastructure
{
	internal struct MemberKey
	{
		public readonly int MemberToken;
		public readonly int ModuleToken;

		public MemberKey(MemberInfo member)
		{
			MemberToken = member.MetadataToken;
			ModuleToken = member.Module.MetadataToken;
		}

		public override bool Equals(object obj)
		{
			if (obj is MemberKey)
			{
				var other = (MemberKey)obj;
				return other.MemberToken.Equals(MemberToken) && other.ModuleToken.Equals(ModuleToken);
			}

			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return MemberToken ^ ModuleToken;
		}
	}
}