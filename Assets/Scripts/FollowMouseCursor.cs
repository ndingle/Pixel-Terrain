using UnityEngine;
using System.Collections;

[RequireComponent( typeof(SpriteRenderer))]
public class FollowMouseCursor : MonoBehaviour {

	// Update is called once per frame
	void Update () {

        Vector3 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        mousePosition.z = 0f;
        transform.position = mousePosition;

	}
}
