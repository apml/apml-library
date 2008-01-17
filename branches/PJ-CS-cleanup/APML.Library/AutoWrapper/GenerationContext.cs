using System;
using System.Collections.Generic;
using System.Reflection;

namespace APML.AutoWrapper {
  /// <summary>
  /// Context used during generation operations.
  /// </summary>
  public class GenerationContext {
    private readonly IDictionary<Type, string> mGeneratedTypes;
    private readonly Queue<Type> mGenerateQueue;

    /// <summary>
    /// Creates a new GenerationContext, adding the given seed type to the queue of items to be generated.
    /// </summary>
    /// <param name="pSeedType">the seed type to place in the queue</param>
    public GenerationContext(Type pSeedType) {
      mGeneratedTypes = new Dictionary<Type, string>();
      mGenerateQueue = new Queue<Type>();

      // Force the seed into the maps and queue
      LookupRequiredTypeName(pSeedType);
    }

    /// <summary>
    /// Looks up the generated name for a given real type. If the type has not yet been generated,
    /// ensures that the type will be generated.
    /// </summary>
    /// <param name="pType"></param>
    /// <returns></returns>
    public string LookupRequiredTypeName(Type pType) {
      if (mGeneratedTypes.ContainsKey(pType)) {
        return mGeneratedTypes[pType];
      }

      string result = GetTargetClassName(pType);
      mGeneratedTypes[pType] = result;
      mGenerateQueue.Enqueue(pType);

      return result;
    }

    /// <summary>
    /// Whether further generation iterators are required.
    /// </summary>
    public bool GenerationRequired {
      get { return mGenerateQueue.Count > 0; }
    }

    /// <summary>
    /// Retrieves the generated types.
    /// </summary>
    public IEnumerable<KeyValuePair<Type, string>> GeneratedTypes {
      get { return mGeneratedTypes; }
    }

    /// <summary>
    /// Dequeues the next type requiring generation.
    /// </summary>
    /// <returns>the next type requiring generation</returns>
    public Type DequeueNextForGeneration() {
      return mGenerateQueue.Dequeue();
    }

    /// <summary>
    /// Determines the name of the class that should be generated for the given type.
    /// </summary>
    /// <param name="pType">the type having the class generated</param>
    /// <returns>the target classname, not including the namespace</returns>
    private static string GetTargetClassName(Type pType) {
      return pType.FullName.Replace('.', '_');
    }
  }
}
