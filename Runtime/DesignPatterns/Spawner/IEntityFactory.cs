using UnityEngine;

namespace Utilities.Platformer
{
    public interface IEntityFactory<T> where T : Entity
    {
        T Create(Transform spawnPoint);
    }
}