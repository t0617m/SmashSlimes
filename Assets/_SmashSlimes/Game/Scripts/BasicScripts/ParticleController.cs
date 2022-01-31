using UniRx;
using UnityEngine;

namespace RescuePeople
{
    public class ParticleController : MonoBehaviour
    {
        [SerializeField] private ParticlePlayer _deadParticle;
        private ParticleObjectPool _deadParticleObjectPool;

        private void Start()
        {
            _deadParticleObjectPool = new ParticleObjectPool(_deadParticle);
            _deadParticleObjectPool.PreloadAsync(20, 1).Subscribe();
        }

        public void PlayDeadParticle(Vector2 position)
        {
            var breakParticle = _deadParticleObjectPool.Rent();
            breakParticle.transform.position = position;
            breakParticle.Play(_ => _deadParticleObjectPool.Return(breakParticle));
        }
    }
}