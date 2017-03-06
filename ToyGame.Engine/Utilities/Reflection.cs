using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ToyGame
{

  /// <summary>
  /// Static reflection helpers, complements of my AudioApp project. Use these with care,
  /// some of them are VERY expensive to invoke. The heavy weight functions are fantastic
  /// for doing a core-dump over a TCP socket (That is in fact what they were used for)
  /// </summary>
  public class Reflection
  {
    /// <summary>
    /// Gets a List of all types that are decorated with the given System.Attribute type.
    /// </summary>
    /// <param name="assembly">The assembly to search.</param>
    /// <param name="attrType">The attribute type.</param>
    /// <returns>A List of all types that are decorated with the given System.Attribute type.</returns>
    public static List<Type> GetTypesWithAttributeList(Assembly assembly, Type attrType)
    {
      List<Type> types = new List<Type>();
      foreach (Type t in GetTypesWithAttribute(assembly, attrType))
      {
        types.Add(t);
      }
      return types;
    }

    /// <summary>
    /// Returns a list of System.Reflection.MethodInfo for all methods that are decorated with attrType and
    /// meet the flags criteria.
    /// </summary>
    /// <param name="assembly">The assembly to search.</param>
    /// <param name="attrType">The attribute type.</param>
    /// <param name="flags">Any BindingFlags for searching.</param>
    /// <returns></returns>
    public static List<MethodInfo> GetMethodsWithAttribute(Assembly assembly, Type attrType, BindingFlags flags)
    {
      List<MethodInfo> methods = new List<MethodInfo>();
      MethodInfo[] mInfos = assembly.GetTypes()
          .SelectMany(t => t.GetMethods(flags))
          .Where(m => m.GetCustomAttributes(attrType, false).Length > 0)
          .ToArray();
      methods.AddRange(mInfos);
      return methods;
    }

    /// <summary>
    /// Enumerate all types that are decorated with the given System.Attribute type.
    /// </summary>
    /// <param name="assembly">The assembly to search.</param>
    /// <param name="attrType">The attribute type.</param>
    /// <returns>IEnumerable of types that are decorated with the Attribute type.</returns>
    public static IEnumerable<Type> GetTypesWithAttribute(Assembly assembly, Type attrType)
    {
      foreach (Type type in assembly.GetTypes())
      {
        if (type.GetCustomAttributes(attrType, false).Length > 0)
        {
          yield return type;
        }
      }
    }

    /// <summary>
    /// Checks if the Type is decorated with an attribute. Does NOT check
    /// inherited types.
    /// </summary>
    /// <param name="type">They type to check.</param>
    /// <param name="attribute">The attribute to look for.</param>
    public static bool IsTypeDecoratedWith(Type type, Type attribute)
    {
      return (type.GetCustomAttributes(attribute, false).Length > 0);
    }

    /// <summary>
    /// Prints the field names and values of a generic object to a human-readable string. This function
    /// is hugely expensive to invoke. Only invoke when you want to generate an error core dump.
    /// </summary>
    /// <param name="obj">The object to be reflected.</param>
    /// <param name="recursive">If true, will recursively print out sub fields of complex objects.</param>
    /// <param name="maxStackDepth">The max stack depth when recursively printing complex objects.</param>
    /// <returns></returns>
    public static String GetFieldValues(Object obj, Boolean recursive, Int32 maxStackDepth = Int32.MaxValue)
    {
      return GetFieldValuesRecursive(obj, "", recursive, maxStackDepth);
    }


    // =======================================     PRIVATE     ===========
    private static String GetFieldValuesRecursive(Object obj, String strOffset, Boolean recursive, Int32 remainingDepth)
    {
      if (obj == null || remainingDepth <= 0)
      {
        return "";
      }
      String thisBuilder = "";
      foreach (FieldInfo info in obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
      {
        Object value = info.GetValue(obj);
        if (value is IEnumerable && !(info.FieldType == typeof(String)))
        {
          Type[] genericArgs = value.GetType().GetGenericArguments();
          foreach (Type iType in value.GetType().GetInterfaces())
          {
            if (iType.IsGenericType && iType.GetGenericTypeDefinition() == typeof(IDictionary<,>))
            {
              thisBuilder += strOffset + "- " + info.Name + ": [ IDictionary <" + iType.GetGenericArguments()[0].Name + ", " + iType.GetGenericArguments()[1].Name + "> ]\r\n";
              Object returnObj = typeof(Reflection).GetMethod("BuildFromIDictionary", BindingFlags.NonPublic | BindingFlags.Static)
                  .MakeGenericMethod(iType.GetGenericArguments())
                  .Invoke(null, new object[] { value, strOffset, recursive, remainingDepth - 1 });
              thisBuilder += (String) returnObj;
              break;
            }
            if (iType.IsGenericType && iType.GetGenericTypeDefinition() == typeof(IList<>))
            {
              thisBuilder += strOffset + "- " + info.Name + ": [ IList <" + iType.GetGenericArguments()[0].Name + "> ]\r\n";
              Object returnObj = typeof(Reflection).GetMethod("BuildFromIList", BindingFlags.NonPublic | BindingFlags.Static)
                  .MakeGenericMethod(iType.GetGenericArguments())
                  .Invoke(null, new object[] { value, strOffset, recursive, remainingDepth - 1 });
              thisBuilder += (String) returnObj;
              break;
            }
          }
        }
        else
        {
          thisBuilder += strOffset + "- " + info.Name + ": " + info.GetValue(obj) + "\r\n";
          if (recursive && remainingDepth > 0 && !(info.FieldType.IsPrimitive || info.FieldType == typeof(String)))
          {
            thisBuilder += GetFieldValuesRecursive(value, strOffset + "   ", true, remainingDepth - 1);
          }
        }
      }
      return thisBuilder;
    }



    private static String BuildFromIDictionary<TKey, TValue>(IDictionary<TKey, TValue> data, String strOffset, Boolean recursive, Int32 remainingDepth)
    {
      String builder = "";
      bool keyPrimitive = typeof(TKey).IsPrimitive || typeof(TKey) == typeof(String);
      bool valuePrimitive = typeof(TValue).IsPrimitive || typeof(TValue) == typeof(String);
      foreach (var pair in data)
      {
        builder += strOffset + "   # ";
        if (!keyPrimitive && recursive)
        {
          builder += " <" + typeof(TKey).Name + ">\r\n" + GetFieldValuesRecursive(pair.Key, strOffset + "       ", true, remainingDepth - 1);
        }
        else
        {
          builder += pair.Key + "\r\n";
        }
        builder += strOffset + "   -> ";
        if (!valuePrimitive && recursive)
        {
          builder += "<" + typeof(TValue).Name + ">\r\n" + GetFieldValuesRecursive(pair.Value, strOffset + "       ", true, remainingDepth - 1);
        }
        else
        {
          builder += pair.Value + "\r\n";
        }
      }
      return builder;
    }

    private static String BuildFromIList<TValue>(IList<TValue> data, String strOffset, Boolean recursive, Int32 remainingDepth)
    {
      String builder = "";
      Boolean isValuePrimitive = typeof(TValue).IsPrimitive || typeof(TValue) == typeof(String);
      foreach (var value in data)
      {
        builder += strOffset + "   -> ";
        if (!isValuePrimitive && recursive)
        {
          builder += "<" + typeof(TValue).Name + ">\r\n" + GetFieldValuesRecursive(value, strOffset + "       ", true, remainingDepth - 1);
        }
        else
        {
          builder += value + "\r\n";
        }
      }
      return builder;
    }

  }
}
