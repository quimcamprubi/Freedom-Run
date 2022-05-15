using UnityEngine;

public class GrapplingRope : MonoBehaviour
{
    [Header("General Refernces:")] public GrapplingGun grapplingGun;
    public LineRenderer m_lineRenderer;

    [Header("General Settings:")] [SerializeField]
    private int precision = 40;

    [Range(0, 20)] [SerializeField] private float straightenLineSpeed = 5;

    [Header("Rope Animation Settings:")] public AnimationCurve ropeAnimationCurve;
    [Range(0.01f, 4)] [SerializeField] private float StartWaveSize = 2;
    private float waveSize = 0;

    [Header("Rope Progression:")] public AnimationCurve ropeProgressionCurve;
    [SerializeField] [Range(1, 50)] private float ropeProgressionSpeed = 1;

    private float moveTime = 0;

    public bool isGrappling = false;

    private bool strightLine = true;

    private void OnEnable()
    {
        moveTime = 0;
        m_lineRenderer.enabled = true;
        m_lineRenderer.positionCount = precision;
        waveSize = StartWaveSize;
        strightLine = false;

        LinePointsToFirePoint();

        m_lineRenderer.enabled = true;
    }

    private void OnDisable()
    {
        m_lineRenderer.enabled = false;
        isGrappling = false;
    }

    private void LinePointsToFirePoint()
    {
        for (var i = 0; i < precision; i++) m_lineRenderer.SetPosition(i, grapplingGun.firePoint.position);
    }

    private void Update()
    {
        moveTime += Time.deltaTime;
        DrawRope();
    }

    private void DrawRope()
    {
        if (!strightLine)
        {
            if (m_lineRenderer.GetPosition(precision - 1).x == grapplingGun.grapplePoint.x)
                strightLine = true;
            else
                DrawRopeWaves();
        }
        else
        {
            if (!isGrappling)
            {
                grapplingGun.Grapple();
                isGrappling = true;
            }

            if (waveSize > 0)
            {
                waveSize -= Time.deltaTime * straightenLineSpeed;
                DrawRopeWaves();
            }
            else
            {
                waveSize = 0;

                if (m_lineRenderer.positionCount != 2) m_lineRenderer.positionCount = 2;

                DrawRopeNoWaves();
            }
        }
    }

    private void DrawRopeWaves()
    {
        for (var i = 0; i < precision; i++)
        {
            var delta = (float) i / ((float) precision - 1f);
            var offset = Vector2.Perpendicular(grapplingGun.grappleDistanceVector).normalized *
                         ropeAnimationCurve.Evaluate(delta) * waveSize;
            var targetPosition = Vector2.Lerp(grapplingGun.firePoint.position, grapplingGun.grapplePoint, delta) +
                                 offset;
            var currentPosition = Vector2.Lerp(grapplingGun.firePoint.position, targetPosition,
                ropeProgressionCurve.Evaluate(moveTime) * ropeProgressionSpeed);

            m_lineRenderer.SetPosition(i, currentPosition);
        }
    }

    private void DrawRopeNoWaves()
    {
        m_lineRenderer.SetPosition(0, grapplingGun.firePoint.position);
        m_lineRenderer.SetPosition(1, grapplingGun.grapplePoint);
    }
}