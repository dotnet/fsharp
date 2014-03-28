using System;
using System.Collections.Generic;

namespace RootNamespace
{
	public class ClassOfT<T>
	{
		public ClassOfT()
		{
			this.DefaultOfT = default(T);
		}

		public T DefaultOfT { get; protected set; }

		// Nested Class
		public class NestedClassOfU<U>
		{
			public NestedClassOfU()
			{
				this.DefaultOfU = default(U);
			}

			public U DefaultOfU { get; protected set; }
		}
	}
}