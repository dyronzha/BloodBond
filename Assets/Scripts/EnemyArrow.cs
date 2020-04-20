using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BloodBond {
    public class EnemyArrow
    {
        public EnemyArrow(Transform t, EnemyManager manager)
        {
            transform = t;
            enemyManager = manager;
            bloodParticle = t.Find("BloodSplash").GetComponent<ParticleSystem>();
            collider = t.GetComponent<BoxCollider>();
            render = t.GetComponent<MeshRenderer>();
        }
        bool disappear = false;
        float time = .0f;
        BoxCollider collider;
        Vector3 flyDir;
        ParticleSystem bloodParticle;
        EnemyManager enemyManager;
        MeshRenderer render;

        public Transform transform;

        public void Update(float dt)
        {
            time += dt;
            
            if (!disappear)
            {
                Vector3 nextPos = transform.position + flyDir * dt * enemyManager.EnemyArrowValue.Speed;
                RaycastHit hit;
                if (Physics.Linecast(transform.position, nextPos, out hit, (1 << LayerMask.NameToLayer("Wall") | 1 << LayerMask.NameToLayer("Barrier") | 1 << LayerMask.NameToLayer("Player")))) {
                    nextPos = hit.point;
                    collider.enabled = false;
                    bloodParticle.Play();
                    disappear = true;
                    time = .0f;
                    render.enabled = false;
                    if (hit.transform.tag.CompareTo("Player") == 0)
                    {
                        enemyManager.Player.DamageToPlayer(enemyManager.EnemyArrowValue.HurtValue);
                    }
                }
                transform.position = nextPos;
                if (time >= enemyManager.EnemyArrowValue.LifeTime) {
                    transform.gameObject.SetActive(false);
                    time = .0f;
                    enemyManager.RecycleArrow(this);
                }
            }
            else {
                if (time >= 1.0f) {
                    transform.gameObject.SetActive(false);
                    collider.enabled = true;
                    time = .0f;
                    disappear = false;
                    render.enabled = true;
                    enemyManager.RecycleArrow(this);
                }
            }
        }
        public void SetFly(Vector3 dir)
        {
            flyDir = dir;
            transform.rotation = Quaternion.LookRotation(dir);
        }
        public void HitPlayer() {
            Debug.Log("撞到  玩家");
            collider.enabled = false;
            bloodParticle.Play();
            disappear = true;
            time = .0f;
            render.enabled = false;
            enemyManager.Player.DamageToPlayer(enemyManager.EnemyArrowValue.HurtValue);
        }
    }
}

