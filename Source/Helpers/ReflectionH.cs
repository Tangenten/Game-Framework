using System;
using System.Collections.Generic;
using System.Reflection;

namespace Helpers {
	public static class ReflectionH {
		public static void RunStaticMethodByName(in string classAndMethod) {
			string[] split = classAndMethod.Split(".");
			string className = split[0];
			string methodName = split[1];

			foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies()) {
				Type[] assemblyTypes = a.GetTypes();
				for (int j = 0; j < assemblyTypes.Length; j++) {
					if (assemblyTypes[j].Name.ToUpper() == className.ToUpper()) {
						MethodInfo staticMethodInfo = assemblyTypes[j].GetMethod(methodName);
						staticMethodInfo.Invoke(null, null);
						break;
					}
				}
			}
		}

		public static Type GetTypeByClassName(in string className) {
			List<Type> returnVal = new List<Type>();

			foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies()) {
				Type[] assemblyTypes = a.GetTypes();
				for (int j = 0; j < assemblyTypes.Length; j++) {
					if (assemblyTypes[j].Name.ToUpper() == className.ToUpper()) {
						returnVal.Add(assemblyTypes[j]);
					}
				}
			}

			return returnVal[0];
		}

		public static T GetObjectByClassName<T>(in string className) {
			return (T) Activator.CreateInstance(GetTypeByClassName(className));
		}
	}
}