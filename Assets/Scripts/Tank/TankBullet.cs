using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetcodePlus.Demo
{
    public class TankBullet : MonoBehaviour
    {
        public float move_speed = 20f;
        public GameObject explode_prefab;

        [HideInInspector]
        public Vector3 direction;

        [HideInInspector]
        public int player_id;
        public int damage;
        float time = 0;

        void Start()
        {
            time = 0;
        }

        void Update()
        {
            time += Time.deltaTime;
            transform.position += direction * move_speed * Time.deltaTime;
            if (time > 2) {
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.isTrigger)
                return;

            Wall wall = other.GetComponentInParent<Wall>();
            if (wall != null)
                wall.Damage();

            Tower tower = other.GetComponentInParent<Tower>();
            if (tower != null)
                tower.Damage();

            Tank tank = other.GetComponentInParent<Tank>();
            //if(tank.PlayerID == player_id) return;
            if (tank != null)
                tank.Damage(damage);

            FXTool.FX(explode_prefab, transform.position);
            Destroy(gameObject);
        }
    }
}
