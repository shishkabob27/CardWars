using UnityEngine;

public class CWTBDragToMove : MonoBehaviour
{
	public Collider DragPlaneCollider;
	public float DragPlaneOffset;
	public Camera RaycastCamera;
	public Camera GameCamera;
	public GameObject cardTray;
	public Transform originalPos;
}
