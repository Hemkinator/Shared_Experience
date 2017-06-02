using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HoloToolkit.Unity.SpatialMapping.Tests
{
    public class GetVertices : MonoBehaviour
    {
        public float scanTime = 30.0f;
        public List<GameObject> ActiveCube;
        // Use this for initialization
        void Start()
        {
            ActiveCube = new List<GameObject>();
            ActiveCube.Add(this.gameObject);
        }

        // Update is called once per frame
        void Update()
        {
            if ((Time.time - SpatialMappingManager.Instance.StartTime) < scanTime)
            {
                // If we have a limited scanning time, then we should wait until
                // enough time has passed before processing the mesh.
            }
            else
            {
                // The user should be done scanning their environment,
                // so start processing the spatial mapping data...

                if (SpatialMappingManager.Instance.IsObserverRunning())
                {
                    // Stop the observer.
                    SpatialMappingManager.Instance.StopObserver();
                }

                // Call CreatePlanes() to generate planes.
                RemoveVertices(ActiveCube);

                // Set meshesProcessed to true.
                
            }
        }


        private void RemoveVertices(IEnumerable<GameObject> boundingObjects)
        {
            RemoveSurfaceVertices removeVerts = RemoveSurfaceVertices.Instance;
            if (removeVerts != null && removeVerts.enabled)
            {
                removeVerts.RemoveSurfaceVerticesWithinBounds(boundingObjects);
            }
        }

    }
}
