using System.Collections.Generic;
using System.Data.SqlTypes;

namespace ToyGame.Utilities
{
  public static class Extensions
  {
    #region Dictionary

    public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key,
      TValue defValue)
    {
      TValue outVal;
      return dictionary.TryGetValue(key, out outVal) ? outVal : defValue;
    }

    #endregion
  }
}