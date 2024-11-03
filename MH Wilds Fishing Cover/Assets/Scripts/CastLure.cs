using UnityEngine;

public class CastLure : MonoBehaviour
{
    public float targetterSpeed, targetRadius;
    
    public bool reelCasted;
    public LayerMask landLayer;

    
    void Update()
    {
        if (!reelCasted)
        {
            //movement inputs
            float moveVertical = Input.GetAxis("Vertical");
            float moveHorizontal = Input.GetAxis("Horizontal");

            
            Vector3 moveDirection = new Vector3(moveHorizontal, 0, moveVertical).normalized;

            // check for land before moving 
            if (CanMove(moveDirection))
            {
                transform.Translate(moveDirection * targetterSpeed * Time.deltaTime, Space.World);
            }
        }
    }
    
    private bool CanMove(Vector3 direction)
    {
        
        RaycastHit hit;
        float checkDistance = targetterSpeed * Time.deltaTime; //ask chad about this 

        //spherecast lets me use radius 
        if (Physics.SphereCast(transform.position, targetRadius, direction, out hit, checkDistance, landLayer))
        {
            return false; // Land is in the way
        }

        return true; // can move 
    }

}
