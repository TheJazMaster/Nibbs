using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Loader;

namespace TheJazMaster.Nibbs;

public static class ReflectionExt
{
	public static AssemblyLoadContext CurrentAssemblyLoadContext
		=> AssemblyLoadContext.GetLoadContext(typeof(ReflectionExt).Assembly) ?? AssemblyLoadContext.CurrentContextualReflectionContext ?? AssemblyLoadContext.Default;

	public static bool IsBuiltInDebugConfiguration(this Assembly assembly)
		=> assembly.GetCustomAttributes(false).OfType<DebuggableAttribute>().Any(attr => attr.IsJITTrackingEnabled);

	public static Func<TValue> EmitStaticGetter<TValue>(this PropertyInfo property)
	{
		DynamicMethod method = new($"get_{property.Name}", typeof(TValue), Array.Empty<Type>());
		var il = method.GetILGenerator();
		il.Emit(OpCodes.Call, property.GetGetMethod(true)!);
		il.Emit(OpCodes.Ret);
		return method.CreateDelegate<Func<TValue>>();
	}

	public static Func<TValue> EmitStaticGetter<TValue>(this FieldInfo field)
	{
		DynamicMethod method = new($"get_{field.Name}", typeof(TValue), Array.Empty<Type>());
		var il = method.GetILGenerator();
		il.Emit(OpCodes.Ldsfld, field);
		il.Emit(OpCodes.Ret);
		return method.CreateDelegate<Func<TValue>>();
	}

	public static Action<TValue> EmitStaticSetter<TValue>(this PropertyInfo property)
	{
		DynamicMethod method = new($"set_{property.Name}", typeof(void), new Type[] { typeof(TValue) });
		var il = method.GetILGenerator();
		il.Emit(OpCodes.Ldarg_0);
		il.Emit(OpCodes.Call, property.GetSetMethod(true)!);
		il.Emit(OpCodes.Ret);
		return method.CreateDelegate<Action<TValue>>();
	}

	public static Action<TValue> EmitStaticSetter<TValue>(this FieldInfo field)
	{
		DynamicMethod method = new($"set_{field.Name}", typeof(void), new Type[] { typeof(TValue) });
		var il = method.GetILGenerator();
		il.Emit(OpCodes.Ldarg_0);
		il.Emit(OpCodes.Stsfld, field);
		il.Emit(OpCodes.Ret);
		return method.CreateDelegate<Action<TValue>>();
	}

	public static Func<TOwner, TValue> EmitInstanceGetter<TOwner, TValue>(this PropertyInfo property)
	{
		DynamicMethod method = new($"get_{property.Name}", typeof(TValue), new Type[] { typeof(TOwner) });
		var il = method.GetILGenerator();
		il.Emit(OpCodes.Ldarg_0);
		il.Emit(OpCodes.Call, property.GetGetMethod(true)!);
		il.Emit(OpCodes.Ret);
		return method.CreateDelegate<Func<TOwner, TValue>>();
	}

	public static Func<TOwner, TValue> EmitInstanceGetter<TOwner, TValue>(this FieldInfo field)
	{
		DynamicMethod method = new($"get_{field.Name}", typeof(TValue), new Type[] { typeof(TOwner) });
		var il = method.GetILGenerator();
		il.Emit(OpCodes.Ldarg_0);
		il.Emit(OpCodes.Ldfld, field);
		il.Emit(OpCodes.Ret);
		return method.CreateDelegate<Func<TOwner, TValue>>();
	}

	public static Action<TOwner, TValue> EmitInstanceSetter<TOwner, TValue>(this PropertyInfo property)
	{
		DynamicMethod method = new($"set_{property.Name}", typeof(void), new Type[] { typeof(TOwner), typeof(TValue) });
		var il = method.GetILGenerator();
		il.Emit(OpCodes.Ldarg_0);
		il.Emit(OpCodes.Ldarg_1);
		il.Emit(OpCodes.Call, property.GetSetMethod(true)!);
		il.Emit(OpCodes.Ret);
		return method.CreateDelegate<Action<TOwner, TValue>>();
	}

	public static Action<TOwner, TValue> EmitInstanceSetter<TOwner, TValue>(this FieldInfo field)
	{
		DynamicMethod method = new($"set_{field.Name}", typeof(void), new Type[] { typeof(TOwner), typeof(TValue) });
		var il = method.GetILGenerator();
		il.Emit(OpCodes.Ldarg_0);
		il.Emit(OpCodes.Ldarg_1);
		il.Emit(OpCodes.Stfld, field);
		il.Emit(OpCodes.Ret);
		return method.CreateDelegate<Action<TOwner, TValue>>();
	}
}