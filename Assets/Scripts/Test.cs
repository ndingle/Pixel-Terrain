using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition = transform.InverseTransformPoint(mousePosition);

            int xPixel = Mathf.RoundToInt(mousePosition.x * 100);
            int yPixel = Mathf.RoundToInt(mousePosition.y * 100);

            print(xPixel.ToString() + ", " + yPixel.ToString());

            GetComponent<DestructibleTerrain>().Explosion(xPixel,
                                                          yPixel,
                                                          50);
        }
    }
}
