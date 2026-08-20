using UnityEngine;

namespace Utilities.Platformer
{
    public interface ISpawnPointStrategy
    {
        Transform NextSpawnPoint();
    }
}