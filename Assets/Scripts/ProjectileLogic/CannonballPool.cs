using System.Collections.Generic;
using UnityEngine;

namespace ProjectileLogic
{
    public class CannonballPool : MonoBehaviour
    {
        [SerializeField] private Cannonball _prefab;
        [SerializeField] private int _initialSize = 10;

        private readonly Queue<Cannonball> _pool = new Queue<Cannonball>();
        private Transform _transform;

        private void Awake()
        {
            _transform = transform;

            for (int i = 0; i < _initialSize; i++)
                _pool.Enqueue(CreateEntity());
        }

        public Cannonball Get()
        {
            Cannonball entity = _pool.Count > 0 ? _pool.Dequeue() : CreateEntity();
            entity.gameObject.SetActive(true);
            return entity;
        }

        public void Return(Cannonball entity)
        {
            entity.gameObject.SetActive(false);
            entity.transform.SetParent(_transform);
            _pool.Enqueue(entity);
        }

        private Cannonball CreateEntity()
        {
            Cannonball entity = Instantiate(_prefab, _transform);
            entity.Initialize(this);
            entity.gameObject.SetActive(false);
            return entity;
        }
    }
}
