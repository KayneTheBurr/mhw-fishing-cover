using UnityEngine;

public class FishingLine : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Transform lure, lureStart;
    public AnimationCurve lineCurve;
    private void Start()
    {
        lineRenderer.positionCount = 11;
    }

    void Update()
    {
        Vector3 rodPosition = lureStart.position;
        Vector3 lurePosition = lure.position;

        // Set thge first poiint
        //lineRenderer.SetPosition(0, rodPosition);

        Vector3 delta = lurePosition - rodPosition;
        float distance = delta.magnitude;

        //float distanceInterp = (distance - data.minCurveDistance) / (data.maxCurveDistance - data.minCurveDistance);
        //float curveStrength = Mathf.Lerp(data.minCurveStrength, data.maxCurveStrength, distanceInterp);

        // Make up points inbetween
        for (int i = 0; i <= 10; i++)
        {
            float interp = i / ((float)10);

            Vector3 position = Vector3.Lerp(rodPosition, lurePosition, interp);

            float curveValue = lineCurve.Evaluate(interp);
            //curveValue *= curveStrength;

            position.y -= curveValue;

            lineRenderer.SetPosition(i, position);
        }
    }
}
