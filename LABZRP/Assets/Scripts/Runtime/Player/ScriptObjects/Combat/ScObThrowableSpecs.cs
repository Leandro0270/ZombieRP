using UnityEngine;

namespace Runtime.Player.ScriptObjects.Combat
{
   [CreateAssetMenu(menuName = "Throwable")]
   public class ScObThrowableSpecs : ScriptableObject
   {

      public int throwableId;
      public enum Type
      {
         Grenade,
         Molotov,
         Flashbang,
         Decoy,
         Landmine,
         GlueGrenade,
         Adrenaline,
         HealthBomb
      }
      public Type throwableType;
      public GameObject throwablePrefab3DModel;
      public GameObject visualEffect;
      public GameObject throwablePrefabVendingMachine3DModel;
      public bool explodeOnImpact;
      public bool affectEnemies;
      public bool affectAllies;
      public bool isDamage;
      public float radius;
      public bool isSlowDown;
      public bool isHeal;
      public bool isStun;
      public bool isBurn;
      public bool affectCamera;
      public bool attractEnemies;
      public float attractionRadius;
      public float cameraShakeAmount;
      public float cameraShakeDuration;
      public float timeToExplode;
      public float damage;
      [Range(0,1f)]
      public float slowDown;
      public float slowDownDuration;
      public float health;
      public float stunDuration;
      public float burnDuration;
      public float effectDuration;
      public float maxDistance;
   }
}
