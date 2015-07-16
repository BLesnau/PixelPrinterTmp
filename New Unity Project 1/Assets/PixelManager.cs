using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vectrosity;
using Random = UnityEngine.Random;

public class PixelManager : MonoBehaviour
{
   private struct PixelConfig
   {
      public int xIndex;
      public int yIndex;
      public int zIndex;

      public Vector3 position;
      public Color color;
   }

   public Pixel pixelPrefab = null;
   public GameObject gridPrefab = null;
   public int depthCount = 10;
   public int colCount = 10;
   public int rowCount = 10;
   public float pixelScale = 1;
   public float gridScale = .1f;
   public float popInDelaySeconds = .1f;

   private Pixel[, ,] _pixels = null;
   private List<PixelConfig> _pixelsToPopIn = null;
   private int _poppedInCount = 0;
   private TimeSpan _popInTimeElapsed = TimeSpan.Zero;
   private AudioSource _audio;

   void Start()
   {
      _audio = GetComponent<AudioSource>();

      _pixels = new Pixel[colCount, rowCount, depthCount];
      _pixelsToPopIn = new List<PixelConfig>();

      var startZ = -1 * ( ( ( depthCount * pixelScale ) / 2.0f ) - ( pixelScale / 2.0f ) );
      var startX = -1 * ( ( ( colCount * pixelScale ) / 2.0f ) - ( pixelScale / 2.0f ) );
      var startY = -1 * ( ( ( rowCount * pixelScale ) / 2.0f ) - ( pixelScale / 2.0f ) );

      if ( pixelPrefab )
      {
         for ( int y = 0; y < rowCount; y++ )
         {
            var itemsLeft = ( colCount ) * ( depthCount );
            int x = 0;
            int z = 0;
            while ( itemsLeft > 0 )
            {
               int xStartIndex = x;
               int zStartIndex = z;

               while ( x < colCount - xStartIndex )
               {
                  AddPixelToPopIn( x, y, z, startX, startY, startZ );
                  x++;
                  itemsLeft--;
               }
               x--;

               if ( itemsLeft <= 0 )
               {
                  break;
               }

               z++;
               while ( z < depthCount - zStartIndex )
               {
                  AddPixelToPopIn( x, y, z, startX, startY, startZ );
                  z++;
                  itemsLeft--;
               }
               z--;

               if ( itemsLeft <= 0 )
               {
                  break;
               }

               x--;
               while ( x >= xStartIndex )
               {
                  AddPixelToPopIn( x, y, z, startX, startY, startZ );
                  x--;
                  itemsLeft--;
               }
               x++;

               if ( itemsLeft <= 0 )
               {
                  break;
               }

               z--;
               while ( z >= zStartIndex + 1 )
               {
                  AddPixelToPopIn( x, y, z, startX, startY, startZ );
                  z--;
                  itemsLeft--;
               }

               x = xStartIndex + 1;
               z = zStartIndex + 1;
            }

            if ( itemsLeft < 0 )
            {
               Debug.Log( itemsLeft );
            }
         }
      }


      var startZ2 = -1 * ( depthCount * pixelScale ) / 2.0f;
      var startX2 = -1 * ( colCount * pixelScale ) / 2.0f;
      var startY2 = -1 * ( rowCount * pixelScale ) / 2.0f;
      //var startZ2 = startZ - ( gridScale / 2.0f );
      //var startX2 = startX - ( gridScale / 2.0f );
      //var startY2 = startY - ( gridScale / 2.0f );
      //for ( int y = 0; y < rowCount; y++ )
      //{
      //   for ( int x = 0; x < colCount; x++ )
      //   {
      //      VectorLine.SetRay3D( Color.white, new Vector3( startX2 + ( x * pixelScale ), startY2 + ( y * pixelScale ), startZ2), new Vector3( 0, 0, 1 ) * depthCount * pixelScale );
      //   }

      //   for ( int z = 0; z < depthCount; z++ )
      //   {
      //      VectorLine.SetRay3D( Color.white, new Vector3( startX2 - pixelScale, startY2 + ( y * pixelScale ), startZ2 + ( z * pixelScale ) ), new Vector3( 1, 0, 0 ) * depthCount * pixelScale );
      //   }
      //}

      //for ( int x = 0; x < colCount; x++ )
      //{
      //   for ( int z = 0; z < depthCount; z++ )
      //   {
      //      for ( int y = 0; y < rowCount; y++ )
      //      {
      //         VectorLine.SetRay3D( Color.white, new Vector3( startX2 + ( x * pixelScale ), startY2 + ( y * pixelScale ), startZ2 + ( z * pixelScale ) ), new Vector3( 0, 1, 0 ) * ( depthCount + extraAmount * 2 ) * pixelScale );
      //      }
      //   }
      //}

      for ( int y = 0; y <= rowCount; y++ )
      {
         for ( int x = 0; x <= colCount; x++ )
         {
            var pos = new Vector3( startX2 + ( x * pixelScale ), startY2 + ( y * pixelScale ), startZ2 );
            var gridLine = (GameObject) Instantiate( gridPrefab, pos, Quaternion.identity );
            if ( gridLine )
            {
               var scale = gridLine.transform.localScale;
               scale.z = depthCount * pixelScale;
               gridLine.transform.localScale = scale;
            }
         }

         for ( int z = 0; z < depthCount; z++ )
         {
            //VectorLine.SetRay3D( Color.white, new Vector3( startX2 - pixelScale, startY2 + ( y * pixelScale ), startZ2 + ( z * pixelScale ) ), new Vector3( 1, 0, 0 ) * depthCount * pixelScale );
         }
      }

      for ( int x = 0; x < colCount; x++ )
      {
         for ( int z = 0; z < depthCount; z++ )
         {
            for ( int y = 0; y < rowCount; y++ )
            {
               //VectorLine.SetRay3D( Color.white, new Vector3( startX2 + ( x * pixelScale ), startY2 + ( y * pixelScale ), startZ2 + ( z * pixelScale ) ), new Vector3( 0, 1, 0 ) * ( depthCount + extraAmount * 2 ) * pixelScale );
            }
         }
      }
   }

   private void AddPixelToPopIn( int x, int y, int z, float startX, float startY, float startZ )
   {
      var pixelConfig = new PixelConfig()
      {
         xIndex = x,
         yIndex = y,
         zIndex = z,
         position = new Vector3( startX + ( x * pixelScale ), startY + ( y * pixelScale ), startZ + ( z * pixelScale ) ),
         color = new Color( Random.value, Random.value, Random.value, 1 )
      };

      _pixelsToPopIn.Add( pixelConfig );
   }

   void Update()
   {
      if ( _pixelsToPopIn.Any() )
      {
         _popInTimeElapsed += TimeSpan.FromSeconds( Time.deltaTime );
         if ( ( _popInTimeElapsed.TotalSeconds / popInDelaySeconds ) > _poppedInCount )
         {
            var config = _pixelsToPopIn[0];
            var clr = _pixelsToPopIn[0].color;
            var pixel = (Pixel) Instantiate( pixelPrefab, config.position, Quaternion.identity );
            if ( pixel )
            {
               pixel.Color = config.color;
               pixel.transform.localScale = new Vector3( pixelScale, pixelScale, pixelScale );
               _pixels[config.xIndex, config.yIndex, config.zIndex] = pixel;
               _pixelsToPopIn.RemoveAt( 0 );
               _poppedInCount++;

               if ( _audio )
               {
                  if ( !_audio.isPlaying )
                  {
                     _audio.Play();
                  }

                  if ( !_pixelsToPopIn.Any() )
                  {
                     _audio.Stop();
                  }
               }

               pixel.transform.parent = this.transform;
            }
         }
      }
   }
}
