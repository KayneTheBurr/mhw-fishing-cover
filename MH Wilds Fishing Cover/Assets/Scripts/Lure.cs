using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;
using NUnit.Framework;
using UnityEngine.UI;
using TMPro;


public class Lure : MonoBehaviour
{
    //vCam is the inwater cam
    public CinemachineCamera vCam, targetterCam, fishCam;
    public CastLure castLure;
    public GameObject rippleFX, winLabel, loseLabel, castingUI, castedUI, fishCaughtPanel;
    public bool reelCasted, hooked, fishCaught;
    public Transform lureStart, lureTarget;
    public float castStartTime, castDuration, reelDuration, lureFraction;
    public float horizontalSpeed, reelFraction, reelJig, lureRadius;
    public LayerMask landLayer;
    public float hookWindow, shakeStrength;
    public int numberFishesCaught, numberFishesTotal;
    public List<GameObject> fishList = new List<GameObject>();
    public TextMeshProUGUI fishCaughtLabel;

    private void Start()
    {
        FishLogic[] fishArray = FindObjectsByType<FishLogic>(FindObjectsSortMode.None);


        foreach (FishLogic fish in fishArray)
        {
            fishList.Add(fish.gameObject);
            numberFishesTotal++;
        }
        castingUI.SetActive(true);
    }

    public IEnumerator FishBite(GameObject fish)
    {
        //fish on hook 
        fishCaught = false;
        hooked = true;
        float hookedTime = hookWindow;
        ShakeCamera(shakeStrength); //start shake while fish is on hook 
        while(hookedTime > 0)
        {
            hookedTime -= Time.deltaTime;

            if(Input.GetKeyDown(KeyCode.Space))
            {
                reelCasted = false;
                fishCam.Priority = 20;
                vCam.Priority = 10;
                targetterCam.Priority = 10;
                StartCoroutine(ReelIn());
                //reel in while fish is on hook = caught
                Debug.Log("Fish Caught");
                StopShakeCamera();
                fishCaught=true;
                numberFishesCaught++;
                hookWindow -= hookWindow / 10;
                fishCaughtPanel.SetActive(true);
                fishCaughtLabel.text = fish.name;
                
                
            }
            if(hookWindow - hookedTime > 1)
            {
                castedUI.SetActive(false);
            }
            yield return null;
        }
        //if i get here it means a fish was hooked and ran out of time
        //currently not cutting out of this section 

        Debug.Log("determine state of game");

        if (!fishCaught)
        {
            Debug.Log("Fishing Failure");
            StopShakeCamera();
            Debug.Log("You Lose");
            loseLabel.SetActive(true);
            
        }
        if(numberFishesCaught >= numberFishesTotal)
        {
            Debug.Log("You win");
            winLabel.SetActive(true);
        }
        else if(fishCaught && numberFishesCaught < numberFishesTotal)
        {
            Debug.Log("Keep Fishing");
            castingUI.SetActive(true);
        }
        yield return new WaitForSeconds(2);
        Destroy(fish);
        fishCaughtPanel.SetActive(false);
    }
    

    public void ShakeCamera(float intensity)
    {
        //vCam = Camera.main.GetComponent<CinemachineCamera>();
        CinemachineBasicMultiChannelPerlin cineShaker = vCam.GetComponent<CinemachineBasicMultiChannelPerlin>();
        cineShaker.AmplitudeGain = intensity;
    }
    public void StopShakeCamera()
    {
        //vCam = Camera.main.GetComponent<CinemachineCamera>();
        CinemachineBasicMultiChannelPerlin cineShaker = vCam.GetComponent<CinemachineBasicMultiChannelPerlin>();
        cineShaker.AmplitudeGain = 0;
    }

    void Update()
    {
        #region Cast Reel
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(!reelCasted)
            {
                reelCasted = true;
                castLure.reelCasted = true;
                castStartTime = Time.time;
                lureTarget = castLure.gameObject.transform;
                
                castLure.gameObject.SetActive(false);
                lureFraction = 0;
                reelFraction = 0;
                
            }
            else
            {
                if(!hooked)
                {
                    StartCoroutine(ReelIn());
                }
                
            }
        }

        if (reelCasted && lureFraction < 1f)
        {
            
            Vector3 center = (lureStart.position + lureTarget.position) * 0.5f;
            center -= new Vector3(0, 1, 0);
            Vector3 startShiftedCenter = (lureStart.position - center);
            Vector3 targetShiftedCenter = (lureTarget.position - center);

            lureFraction = (Time.time - castStartTime) / castDuration;
            transform.position = Vector3.Slerp(startShiftedCenter, targetShiftedCenter, lureFraction);
            transform.position += center;

            if (lureFraction >= 1f)
            {
                lureFraction = 1f;
            }
        }
        #endregion

        else if (reelCasted && lureFraction >= 1f)
        {
            vCam.Priority = 20;
            fishCam.Priority = 10;
            targetterCam.Priority = 10;
            castingUI.SetActive(false);
            castedUI.SetActive(true);
            lureTarget = transform;

            // Small jigging reel with 'S'
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow) && !hooked)
            {
                Instantiate(rippleFX, transform.position, Quaternion.identity);
                reelFraction = Mathf.Clamp01(reelFraction + reelJig);
                Vector3 reelPosition = Vector3.Lerp(lureTarget.position, lureStart.position, reelFraction);
                reelPosition.y = transform.position.y; // constant y to stay on water height 
                if(CanMoveTo(reelPosition))
                {
                    transform.position = reelPosition;
                }
                
            }

            // Left and right movement with A and D keys
            float horizontalInput = Input.GetAxis("Horizontal");
            if (Mathf.Abs(horizontalInput) > 0.1f && !hooked)
            {
                Vector3 reelPos = transform.position;
                reelPos.x += horizontalInput * horizontalSpeed * Time.deltaTime;
                if(CanMoveTo(reelPos))
                {
                    transform.position = reelPos;
                }
                
            }

        }
    }
    private bool CanMoveTo(Vector3 targetPosition)
    {
        RaycastHit hit;
        Vector3 direction = (targetPosition - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, targetPosition);

        // check for land smae as tagetter
        if (Physics.SphereCast(transform.position, lureRadius, direction, out hit, distance, landLayer))
        {
            return false; 
        }
        return true;
    }

    IEnumerator ReelIn()
    {
        while (reelFraction < 1f)
        {
            reelFraction += Time.deltaTime / reelDuration; 
            reelFraction = Mathf.Clamp01(reelFraction);

            Vector3 reelPosition = Vector3.Lerp(lureTarget.position, lureStart.position, reelFraction);
            reelPosition.y = lureTarget.position.y; 
            transform.position = reelPosition;

            yield return null; 
        }

        // Reset 
        reelCasted = false;
        castLure.gameObject.SetActive(true);
        castLure.reelCasted = false;
        transform.position = lureStart.position;
        reelFraction = 0;
        lureFraction = 0;
        if(!hooked)
        {
            Debug.Log("No fish go to targetter");
            vCam.Priority = 10;
            fishCam.Priority = 10;
            targetterCam.Priority = 20;
        }

        hooked = false;

        yield return new WaitForSeconds(2);
        Debug.Log("Delayed Go to Targetter");
        vCam.Priority = 10;
        fishCam.Priority = 10;
        targetterCam.Priority = 20;



    }
}
