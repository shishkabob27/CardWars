public interface ULAnimControllerInterface
{
	bool HasAnimation(string animationName);

	bool AnimationEnabled();

	void EnableAnimation(bool enabled);

	void PlayAnimation(string animationName);

	void StopAnimation(string animationName);

	void StopAnimations();

	void Sample(string animationName, float normalizedTime);

	float NormalizedTimePerFrame(string animationName);
}
