using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Runtime.Player.ScriptObjects.Combat;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Runtime.Player.Combat.Throwables
{
    public class ThrowablePlayerStats : MonoBehaviourPunCallbacks
    {
        [SerializeField] private ScObThrowableSpecs[] throwableSpecsList;
        [SerializeField] private DecalProjector explosionArea;
        [SerializeField] private GameObject ThrowerHand;
        [SerializeField] private GameObject DecalSpawnPoint;
        [SerializeField] private float coolDownBetweenThrows = 1.5f;
        [SerializeField] private bool isOnline = false;
        private int maxCapacity;
        private List<ScObThrowableSpecs> throwableInventory = new List<ScObThrowableSpecs>();
        private int itemIndex = 0;
        private float maxThrowDistance;
        private bool isAiming;
        public GameObject throwableItemPrefab;
        private GameObject decalObject;
        private bool canceledThrow = false;
        private bool canThrowItem = false;
        private float currentThrowDistance = 0f;
        private bool isInThrowableChallenge = false;


        private void Update()
        {
            if (isAiming)
            {
                ControlDecalDistance();
            }
        }

        public bool addThrowable(ScObThrowableSpecs throwable)
        {
            if (throwableInventory.Count < maxCapacity)
            {
                throwableInventory.Add(throwable);
                maxThrowDistance = throwableInventory[itemIndex].maxDistance;
                if(!isInThrowableChallenge)
                    canThrowItem = true;
                return true;
            }
            return false;
        }

        public void changeToNextItem()
        {
            if (throwableInventory.Count == 0)
                return;

            if (itemIndex < throwableInventory.Count - 1)
            {
                itemIndex++;
            }
            else
            {
                itemIndex = 0;
            }
            maxThrowDistance = throwableInventory[itemIndex].maxDistance;
        }

        private Vector3 CalculaTrajetoriaParabolica(Vector3 origem, Vector3 destino, float alturaMaxima)
        {
            Vector3 direcao = destino - origem;
            float distHorizontal = Mathf.Sqrt(direcao.x * direcao.x + direcao.z * direcao.z);
            float distVertical = destino.y - origem.y;

            float alturaAdicional = Mathf.Clamp(alturaMaxima, 0f, alturaMaxima - distVertical);

            float t = Mathf.Sqrt(-2 * alturaAdicional / Physics.gravity.y);
            float velocidadeVertical = -Physics.gravity.y * t;

            t += Mathf.Sqrt(2 * (distVertical - alturaAdicional) / Physics.gravity.y);
            float velocidadeHorizontal = distHorizontal / t;

            Vector3 velocidade = new Vector3(direcao.x / distHorizontal * velocidadeHorizontal, velocidadeVertical,
                direcao.z / distHorizontal * velocidadeHorizontal);

            return velocidade;
        }

        private void ControlDecalDistance()
        {
            if(decalObject == null){
                decalObject = Instantiate(explosionArea.gameObject, DecalSpawnPoint.transform.position, DecalSpawnPoint.transform.rotation);
                decalObject.transform.SetParent(DecalSpawnPoint.transform);
                DecalProjector decalProjector = decalObject.GetComponent<DecalProjector>();
                decalProjector.size = new Vector3(throwableInventory[itemIndex].radius * 2, throwableInventory[itemIndex].radius * 2, decalProjector.size.z);

            }

            if (currentThrowDistance < maxThrowDistance)
            {
                currentThrowDistance += Time.deltaTime * 5;
            }
     
            Vector3 offset = new Vector3(-currentThrowDistance,0,-0.1f);
            decalObject.transform.localPosition = offset;
            
        }

        //Getters and Setters
        public void setMaxCapacity(int maxCapacity)
        {
            this.maxCapacity = maxCapacity;
        }
    
        public void coolDown()
        {
            canThrowItem = false;
            StartCoroutine(coolDownCoroutine());
        }
    
        private IEnumerator coolDownCoroutine()
        {
            yield return new WaitForSeconds(coolDownBetweenThrows);
            if(throwableInventory.Count > 0)
                canThrowItem = true;
            canceledThrow = false;

        }
    
        public void setAiming(bool isAiming)
        {
            if (canThrowItem)
            {
                if (!canceledThrow)
                {
                    if(isAiming)
                    {
                        this.isAiming = isAiming;
                    }
                    else
                    {
                        ThrowItem();
                    }
                }
            
                else
                {
                    if(decalObject)
                        Destroy(decalObject);
                }
            }
        }




        [PunRPC]
        public void applyThrowableSpecs(int ThrowableSpecsID, int instancePhotonID)
        {
            ScObThrowableSpecs selectedThrowableSpecs = null;
            //Irá procurar na lista de throwableSpecs o throwableSpecs com o ID passado como parâmetro
            foreach (ScObThrowableSpecs throwableSpecs in throwableSpecsList)
            {
                if (throwableSpecs.throwableId == ThrowableSpecsID)
                {
                    selectedThrowableSpecs = throwableSpecs;
                    break;
                }
            }

            if (selectedThrowableSpecs != null)
            {
                GameObject throwableItemInstance = PhotonView.Find(instancePhotonID).gameObject;
                throwableItemInstance.GetComponent<ThrowableItem>().setThrowableSpecs(selectedThrowableSpecs);
            }
        }
        public void ThrowItem()
        {
            if (decalObject)
            {
                GameObject throwableItemInstance;
                if (isOnline)
                {
                    throwableItemInstance = PhotonNetwork.Instantiate("ThrowableItem", ThrowerHand.transform.position, Quaternion.identity);
                    int instancePhotonID = throwableItemInstance.GetComponent<PhotonView>().ViewID;
                    photonView.RPC("applyThrowableSpecs", RpcTarget.All, throwableInventory[itemIndex].throwableId, instancePhotonID);
                }
                else
                {
                    throwableItemInstance =
                        Instantiate(throwableItemPrefab, ThrowerHand.transform.position, Quaternion.identity);
                    throwableItemInstance.GetComponent<ThrowableItem>().setThrowableSpecs(throwableInventory[itemIndex]);
                }

                Rigidbody rb = throwableItemInstance.GetComponent<Rigidbody>();
                Vector3 trajetoria =
                    CalculaTrajetoriaParabolica(ThrowerHand.transform.position, decalObject.transform.position,
                        9);
                rb.AddForce(trajetoria, ForceMode.VelocityChange);
                throwableInventory.Remove(throwableInventory[itemIndex]);
                changeToNextItem();
                if (throwableInventory.Count == 0)
                    canThrowItem = false;
                currentThrowDistance = 0f;
                isAiming = false;
                Destroy(decalObject);
                coolDown();
            }
        }

        public void cancelThrowAction()
        {
            canceledThrow = true;
            coolDown();
            currentThrowDistance = 0f;
        }
    
        public void setCanceledThrow(bool canceledThrow)
        {
            this.canceledThrow = canceledThrow;
            isAiming = false;
            Destroy(decalObject);
            currentThrowDistance = 0f;
        }
    
        public void setIsInThrowableChallenge(bool isInThrowableChallenge)
        {
            if (isInThrowableChallenge)
            {
                this.isInThrowableChallenge = isInThrowableChallenge;
                canThrowItem = false;
            }
            else
            {
                this.isInThrowableChallenge = isInThrowableChallenge;
                if(throwableInventory.Count > 0)
                    canThrowItem = true;
            }
        
        }
    }
}