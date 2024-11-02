using UnityEngine;

public class Lure : MonoBehaviour
{
    public CastLure castLure;
    public bool reelCasted;
    public Transform lureStart, lureTarget;
    public float castStartTime, castDuration, lureFraction; 
    

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !reelCasted)
        {
            reelCasted = true;
            castLure.reelCasted = true;
            castStartTime = Time.time;
        }

        if (reelCasted)
        {
            //castStartTime += Time.deltaTime;
            Vector3 center = (lureStart.position + lureTarget.position) * 0.5f;
            center -= new Vector3(0, 1, 0);
            Vector3 startShiftedCenter = (lureStart.position - center);
            Vector3 targetShiftedCenter = (lureTarget.position - center);

            lureFraction = (Time.time - castStartTime) / castDuration;
            transform.position = Vector3.Slerp(startShiftedCenter, targetShiftedCenter, lureFraction);
            transform.position += center;
        }
        if (lureFraction >= 1f)
        {
            reelCasted = false;
            castLure.reelCasted = false;
        }
    }
}
