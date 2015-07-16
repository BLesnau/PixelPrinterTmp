using System;
using UnityEngine;

public class TestScript : MonoBehaviour
{
   public event Action<string> DoStuffToWin;
   private int count = 0;

   // Use this for initialization
   void Start()
   {

   }

   // Update is called once per frame
   void Update()
   {
      count++;

      if ( DoStuffToWin != null )
      {
         DoStuffToWin( count.ToString() );
      }
   }

   void OnGUI()
   {
      if ( GUI.Button( new Rect( 0, 0, Screen.width, 40 ), "HEYYY" ) )
      {
      }
   }
}
