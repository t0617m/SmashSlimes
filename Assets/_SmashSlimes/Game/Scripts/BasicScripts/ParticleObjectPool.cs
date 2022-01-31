using UniRx.Toolkit;
using UnityEngine;

namespace RescuePeople
{
    public class ParticleObjectPool : ObjectPool<ParticlePlayer>
    {
        private readonly ParticlePlayer _prefab;
        private readonly Transform _root;

        public ParticleObjectPool(ParticlePlayer prefab)
        {
            _prefab = prefab;
            _root = new GameObject().transform;
            _root.name = "Particles";
            _root.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        }

        protected override ParticlePlayer CreateInstance()
        {
            return Object.Instantiate(_prefab, _root);
        }
    }
}