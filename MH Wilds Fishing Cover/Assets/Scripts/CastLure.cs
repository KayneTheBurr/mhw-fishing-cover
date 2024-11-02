using UnityEngine;

public class CastLure : MonoBehaviour
{
    public float targetterSpeed;
    
    public bool inWater, reelCasted;

    
    void Update()
    {
        if(inWater && !reelCasted)
        {
            //move tragetter in the water 
            if (Input.GetKey(KeyCode.W))
            {
                transform.Translate(Vector3.forward * targetterSpeed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.A))
            {
                transform.Translate(Vector3.left * targetterSpeed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.S))
            {
                transform.Translate(Vector3.back * targetterSpeed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.D))
            {
                transform.Translate(Vector3.right * targetterSpeed * Time.deltaTime);
            }

        }
        
    }

}
