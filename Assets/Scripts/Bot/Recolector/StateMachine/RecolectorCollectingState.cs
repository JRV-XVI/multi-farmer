using Unity.VisualScripting;
using UnityEngine;

public class RecolectorCollectingState : State<Recolector>
{
    public RecolectorCollectingState(Recolector owner) : base(owner) { }


    public override void OnEnterState()
    {
        Debug.Log("Recolector entro a CollectingState");

    }

    public override void OnStayState()
    {

    }

    public override void OnExitState()
    {

    }

    public override void OnTriggerEnter(Collider other)
    {

        if (other.gameObject == owner.targetPlant)
        {
            Debug.Log("Triger activado por planta");
            owner.TakeObject(other.gameObject);

            if(owner.numObjectsCarring < owner.numObjectsCapacity && owner.manager.numFreePlants > 0){
                owner.FindPlant();
                owner.destiny = new Vector2(owner.targetPlant.transform.position.x, owner.targetPlant.transform.position.z);
                owner.ChangeState(new RecolectorMovingState(owner));
                return;
            } 
            else if (owner.numObjectsCarring >= owner.numObjectsCapacity || owner.manager.numFreePlants == 0){
                owner.destiny = new Vector2(owner.safeZone.transform.position.x, owner.safeZone.transform.position.z);
                owner.ChangeState(new RecolectorMovingState(owner));
            }
            
        }
    }

    public override void OnTriggerStay(Collider other)
    {
        if (other.gameObject == owner.targetPlant)
        {
            Debug.Log("Triger activado por planta");
            owner.TakeObject(other.gameObject);

            if(owner.numObjectsCarring < owner.numObjectsCapacity && owner.manager.numFreePlants > 0){
                owner.FindPlant();
                owner.destiny = new Vector2(owner.targetPlant.transform.position.x, owner.targetPlant.transform.position.z);
                owner.ChangeState(new RecolectorMovingState(owner));
                return;
            } 
            else if (owner.numObjectsCarring >= owner.numObjectsCapacity || owner.manager.numFreePlants == 0){
                owner.destiny = new Vector2(owner.safeZone.transform.position.x, owner.safeZone.transform.position.z);
                owner.ChangeState(new RecolectorMovingState(owner));
            }
        }
    }





}
