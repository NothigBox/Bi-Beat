using UnityEngine;
using UnityEngine.Serialization;

public class DiscMovement : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float angleLimit;
    
    public bool invertRotation;
    
    private float lastSence;
    private bool canRotate;
    
    private void Awake()
    {
        canRotate = true;
    }

    public void Move(float horizontal, bool canMove = true)
    {
        if(!canMove) return;
        
        transform.Rotate(Vector3.forward *
                         (horizontal * rotationSpeed *
                          (invertRotation ? -1 : 1)));

        float currentAngle = WrapAngle(transform.localEulerAngles.z);
        float clampedAngle = Mathf.Clamp(currentAngle, -angleLimit, angleLimit);

        currentAngle = UnwrapAngle(clampedAngle);

        transform.localEulerAngles = Vector3.forward * currentAngle;
    }
    
    private float WrapAngle(float angle)
    {
        angle%=360;
        if(angle >180)
            return angle - 360;
 
        return angle;
    }
 
    private float UnwrapAngle(float angle)
    {
        if(angle >=0)
            return angle;
 
        angle = -angle%360;
 
        return 360-angle;
    }
}
