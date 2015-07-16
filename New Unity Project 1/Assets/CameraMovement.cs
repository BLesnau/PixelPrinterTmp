using UnityEngine;

public class CameraMovement : MonoBehaviour
{
   public GameObject targetObject;

   private Camera cam;
   // Use this for initialization
   void Start()
   {
      cam = GetComponent<Camera>();

      if ( cam )
      {
         transform.LookAt( targetObject.transform );
      }
   }

   // Update is called once per frame
   void Update()
   {
      transform.RotateAround( targetObject.transform.position, 45 );
   }
}
