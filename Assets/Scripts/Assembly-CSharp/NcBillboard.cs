public class NcBillboard : NcEffectBehaviour
{
	public enum AXIS_TYPE
	{
		AXIS_FORWARD = 0,
		AXIS_BACK = 1,
		AXIS_RIGHT = 2,
		AXIS_LEFT = 3,
		AXIS_UP = 4,
		AXIS_DOWN = 5,
	}

	public enum ROTATION
	{
		NONE = 0,
		RND = 1,
		ROTATE = 2,
	}

	public enum AXIS
	{
		X = 0,
		Y = 1,
		Z = 2,
	}

	public bool m_bCameraLookAt;
	public bool m_bFixedObjectUp;
	public bool m_bFixedStand;
	public AXIS_TYPE m_FrontAxis;
	public ROTATION m_RatationMode;
	public AXIS m_RatationAxis;
	public float m_fRotationValue;
}
