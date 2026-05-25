
using System.Collections.Generic;

namespace LabyrinthGenerator.Core;
/// <summary>
/// Some collection that allows you to get random element
/// </summary>
public interface IGetRandom<T> : IEnumerable<T>
{
    /// <summary>
    /// Get random element of collection
    /// </summary>
    /// <returns>a random element of the collection</returns>
    T GetRandom();
}
