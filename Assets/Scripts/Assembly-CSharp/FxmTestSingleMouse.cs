using UnityEngine;

public class FxmTestSingleMouse : MonoBehaviour
{
	protected const float m_fDefaultDistance = 8f;

	protected const float m_fDefaultWheelSpeed = 5f;

	public Transform m_TargetTrans;

	public Camera m_GrayscaleCamara;

	public Shader m_GrayscaleShader;

	protected bool m_bRaycastHit;

	public float m_fDistance = 8f;

	public float m_fXSpeed = 350f;

	public float m_fYSpeed = 300f;

	public float m_fWheelSpeed = 5f;

	public float m_fYMinLimit = -90f;

	public float m_fYMaxLimit = 90f;

	public float m_fDistanceMin = 1f;

	public float m_fDistanceMax = 50f;

	public int m_nMoveInputIndex = 1;

	public int m_nRotInputIndex;

	public float m_fXRot;

	public float m_fYRot;

	protected bool m_bHandEnable = true;

	protected Vector3 m_MovePostion;

	protected Vector3 m_OldMousePos;

	protected bool m_bLeftClick;

	protected bool m_bRightClick;

	public void ChangeAngle(float angle)
	{
		m_fYRot = angle;
		m_fXRot = 0f;
		m_MovePostion = Vector3.zero;
	}

	public void SetHandControl(bool bEnable)
	{
		m_bHandEnable = bEnable;
	}

	public void SetDistance(float fDistance)
	{
		m_fDistance *= fDistance;
		PlayerPrefs.SetFloat("FxmTestSingleMouse.m_fDistance", m_fDistance);
	}

	private void OnEnable()
	{
		m_fDistance = PlayerPrefs.GetFloat("FxmTestSingleMouse.m_fDistance", m_fDistance);
	}

	private void Start()
	{
		if (!(Camera.main == null) && (bool)GetComponent<Rigidbody>())
		{
			GetComponent<Rigidbody>().freezeRotation = true;
		}
	}

	private bool IsGUIMousePosition()
	{
		Vector2 point = new Vector2(Input.mousePosition.x, (float)Screen.height - Input.mousePosition.y);
		if (FxmTestSingleMain.GetButtonRect().Contains(point))
		{
			return true;
		}
		return false;
	}

	private void Update()
	{
		if (!IsGUIMousePosition() || m_bLeftClick || m_bRightClick)
		{
			UpdateCamera();
		}
	}

	public void UpdateCamera()
	{
		if (Camera.main == null)
		{
			return;
		}
		if (m_fWheelSpeed < 0f)
		{
			m_fWheelSpeed = 5f;
		}
		float num = m_fDistance / 8f;
		float fDistance = m_fDistance;
		if (!m_TargetTrans)
		{
			return;
		}
		if (!IsGUIMousePosition())
		{
			m_fDistance = Mathf.Clamp(m_fDistance - Input.GetAxis("Mouse ScrollWheel") * m_fWheelSpeed * num, m_fDistanceMin, m_fDistanceMax);
		}
		if (Camera.main.orthographic)
		{
			Camera.main.orthographicSize = m_fDistance * 0.6f;
			if (m_GrayscaleCamara != null)
			{
				m_GrayscaleCamara.orthographicSize = m_fDistance * 0.6f;
			}
		}
		if (m_bRightClick && Input.GetMouseButton(m_nRotInputIndex))
		{
			m_fXRot += Input.GetAxis("Mouse X") * m_fXSpeed * 0.02f;
			m_fYRot -= Input.GetAxis("Mouse Y") * m_fYSpeed * 0.02f;
		}
		if (Input.GetMouseButtonDown(m_nRotInputIndex))
		{
			m_bRightClick = true;
		}
		if (Input.GetMouseButtonUp(m_nRotInputIndex))
		{
			m_bRightClick = false;
		}
		m_fYRot = ClampAngle(m_fYRot, m_fYMinLimit, m_fYMaxLimit);
		Quaternion quaternion = Quaternion.Euler(m_fYRot, m_fXRot, 0f);
		RaycastHit hitInfo;
		if (m_bRaycastHit && Physics.Linecast(m_TargetTrans.position, Camera.main.transform.position, out hitInfo))
		{
			m_fDistance -= hitInfo.distance;
		}
		Vector3 vector = new Vector3(0f, 0f, 0f - m_fDistance);
		Vector3 position = quaternion * vector + m_TargetTrans.position;
		Camera.main.transform.rotation = quaternion;
		Camera.main.transform.position = position;
		UpdatePosition(Camera.main.transform);
		if (m_GrayscaleCamara != null)
		{
			m_GrayscaleCamara.transform.rotation = Camera.main.transform.rotation;
			m_GrayscaleCamara.transform.position = Camera.main.transform.position;
		}
		if (fDistance != m_fDistance)
		{
			PlayerPrefs.SetFloat("FxmTestSingleMouse.m_fDistance", m_fDistance);
		}
	}

	public static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360f)
		{
			angle += 360f;
		}
		if (angle > 360f)
		{
			angle -= 360f;
		}
		return Mathf.Clamp(angle, min, max);
	}

	private void UpdatePosition(Transform camera)
	{
		if (m_bHandEnable)
		{
			if (Input.GetMouseButtonDown(m_nMoveInputIndex))
			{
				m_OldMousePos = Input.mousePosition;
				m_bLeftClick = true;
			}
			if (m_bLeftClick && Input.GetMouseButton(m_nMoveInputIndex))
			{
				Vector3 mousePosition = Input.mousePosition;
				float worldPerScreenPixel = GetWorldPerScreenPixel(m_TargetTrans.transform.position);
				m_MovePostion += (m_OldMousePos - mousePosition) * worldPerScreenPixel;
				m_OldMousePos = mousePosition;
			}
			if (Input.GetMouseButtonUp(m_nMoveInputIndex))
			{
				m_bLeftClick = false;
			}
		}
		camera.Translate(m_MovePostion, Space.Self);
	}

	public float GetWorldPerScreenPixel(Vector3 worldPoint)
	{
		Camera main = Camera.main;
		if (main == null)
		{
			return 0f;
		}
		float distanceToPoint = new Plane(main.transform.forward, main.transform.position).GetDistanceToPoint(worldPoint);
		float num = 100f;
		return Vector3.Distance(main.ScreenToWorldPoint(new Vector3(Screen.width / 2, (float)(Screen.height / 2) - num / 2f, distanceToPoint)), main.ScreenToWorldPoint(new Vector3(Screen.width / 2, (float)(Screen.height / 2) + num / 2f, distanceToPoint))) / num;
	}
}
